using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NAudio.Wave;
using Timer = System.Timers.Timer;

namespace Client.Client
{
    public class ChatClient
    {
        #region Fields

        private readonly TcpClient server;
        private readonly Socket clientSocket;
        
        private WaveInEvent sourceStream;
        private WaveOut recievedStream;
        private BufferedWaveProvider waveProvider;
        private readonly IPEndPoint localEndPoint;
        private IPEndPoint remoteEndPoint;
        private Thread udpReceiveThread;
        private Thread heartBeatThread;
        #endregion

        #region Events
        public event EventHandler UserListReceived;
        public event EventHandler MessageReceived;
        public event EventHandler CallRecieved;
        public event EventHandler CallRequestResponded;
        #endregion

        #region Properties

        public string ClientAddress { get; set; }
        public string ServerAddress { get; set; }
        public string UserName { get; set; }

        public bool IsConnected { get; set; }

        #endregion


        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="port"></param>
        /// <param name="serverAddress"></param>
        /// <param name="userName"></param>
        public ChatClient(int port, string serverAddress, string userName)
        {
            try
            {
                server = new TcpClient(serverAddress,port);
                IsConnected = true;
                // Creating new udp client socket
                clientSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);
                localEndPoint = GetHostEndPoint();
                clientSocket.Bind(localEndPoint);
                ServerAddress = serverAddress;
                UserName = userName;
            }
            catch (SocketException)
            {
                MessageBox.Show(@"Unable to connect to server");
                return;
            }
           // Send username to server
            var bytes = Encoding.Unicode.GetBytes(userName);
            server.Client.Send(bytes);
        }


        private IPEndPoint GetHostEndPoint()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            if (ipAddress == null) 
                return null;
            var random = new Random();
            var endPoint = new IPEndPoint(ipAddress,random.Next(65520,65530) );
            ClientAddress = string.Format("{0}:{1}",endPoint.Address,endPoint.Port);
            return endPoint;
        }


        public void Init()
        {
            var state = new Chat.Chat.StateObject
            {
                WorkSocket = server.Client
            };

            //Receive list of users online 
            ReceiveUsersList();

            //heartBeatThread = new Thread(HeartBeat);
            //heartBeatThread.Start();
            waveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 2));
            recievedStream = new WaveOut();
            recievedStream.Init(waveProvider);

            server.Client.BeginReceive(state.Buffer, 0, Chat.Chat.StateObject.BufferSize, 0,
                OnReceive, state);
        }

        /// <summary>
        /// Heartbeat request to server
        /// </summary>
        private void HeartBeat()
        {
            var heartbeat = string.Format("{0}|{1}", Chat.Chat.Heartbeat, UserName);
            var bytes = Encoding.Unicode.GetBytes(heartbeat);
            Timer timer = null;
            while (IsConnected)
            {
                if (timer == null)
                {
                    timer = new Timer(1000);
                    timer.Elapsed += (sender, args) =>
                    {
                        try
                        {
                            server.Client.Send(bytes);
                        }
                        catch
                        {
                            server.Client.Disconnect(true);
                        }
                        
                    };
                }
                timer.Enabled = true;
            }
            
        }

        private void ReceiveUsersList()
        {
            var bytes = new byte[1024];
            var bytesRead = server.Client.Receive(bytes);
            var content = Encoding.Unicode.GetString(bytes, 0, bytesRead);
            var splittedStr = content.Split('|');
            var list = splittedStr[2];
            OnUserListReceived(list,splittedStr[3],splittedStr[4]);
        }

        public void OnReceive(IAsyncResult ar)
        {
            var state = ar.AsyncState as Chat.Chat.StateObject;
            if (state == null)
                return;
            var handler = state.WorkSocket;
            if (!handler.Connected)
                return;
            try
            {
                var bytesRead = handler.EndReceive(ar);
                if (bytesRead <= 0)
                    return;

                state.Sb.Remove(0, state.Sb.Length);
                state.Sb.Append(Encoding.Unicode.GetString(state.Buffer, 0, bytesRead));

                var content = state.Sb.ToString();  

                ParseMessage(content);
                
                server.Client.BeginReceive(state.Buffer, 0, Chat.Chat.StateObject.BufferSize, 0, OnReceive, state);
            }
            catch (SocketException)
            {
                server.Client.Disconnect(true);
            }
        }


        public void OnUdpRecieve(IAsyncResult ar)
        {
            var state = ar.AsyncState as Chat.Chat.StateObject;
            if (state == null) 
                return;
            var handler = clientSocket;

            try
            {
                var bytesRead = handler.EndReceive(ar);
                waveProvider.AddSamples(state.Buffer, 0, bytesRead);
                recievedStream.Play();

                var ep = (EndPoint)localEndPoint;
                handler.BeginReceiveFrom(state.Buffer, 0, Chat.Chat.StateObject.BufferSize, SocketFlags.None, ref ep, OnUdpRecieve, state);
            }
            catch (Exception)
            {
                // remote user disconnected
                clientSocket.Close();
            }
            
            
        }

        /// <summary>
        /// Parse received message
        /// </summary>
        /// <param name="message"></param>
        public void ParseMessage(string message)
        {
            var splittedMessage = message.Split('|');
            var interaction = splittedMessage[0];
            switch (interaction)
            {
                case Chat.Chat.Message:
                    switch (splittedMessage[1])
                    {
                        case Chat.Chat.Server:
                            OnUserListReceived(splittedMessage[2],splittedMessage[3],splittedMessage[4]);
                        break;
                        default:
                            OnMessageReceived(splittedMessage[3], splittedMessage[2]);  
                        break;
                    }
                break;
                case Chat.Chat.Request:
                    OnCallRecieved(splittedMessage[2],splittedMessage[3]);
                break;
                case Chat.Chat.Response:
                    ParseResponse(splittedMessage[3],splittedMessage[4]);
                    OnCallRequestResponded(splittedMessage[3]);
                break;
            }
        }

        private void ParseResponse(string response,string address)
        {
            switch (response)
            {
                case Chat.Chat.Accept:
                    StartVoiceChat(address);
                break;
                case Chat.Chat.Decline:
                break;
            }
        }

        private void StartVoiceChat(string address)
        {
            var splittedAddress = address.Split(':');
            var ip = splittedAddress[0];
            var port = splittedAddress[1];
            
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip),Int32.Parse(port));

            

            sourceStream = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(8000, 16, 2)
            };
            
            sourceStream.DataAvailable += sourceStream_DataAvailable;
            sourceStream.StartRecording();
            udpReceiveThread = new Thread(ReceiveUdpData);
            udpReceiveThread.Start();
        }

        private void sourceStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (sourceStream == null)
                return;
            SendUdpData(e.Buffer,e.BytesRecorded);
        }


        private void SendUdpData(byte[] buf, int bytesRecorded)
        {
            var ep = remoteEndPoint as EndPoint;
            clientSocket.SendTo(buf, 0, bytesRecorded, SocketFlags.None, ep);
        }

        private void ReceiveUdpData()
        {
            var state = new Chat.Chat.StateObject
            {
                WorkSocket = clientSocket
            };
            var ep = remoteEndPoint as EndPoint;
            clientSocket.BeginReceiveFrom(state.Buffer, 0, Chat.Chat.StateObject.BufferSize, SocketFlags.None, ref ep,
                OnUdpRecieve, state);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="recipient"></param>
        public void SendMessage(string message,string recipient)
        {
            var str = string.Format("{0}|{1}|{2}|{3}",Chat.Chat.Message, recipient, UserName, message);
            var bytes = Encoding.Unicode.GetBytes(str);
            server.Client.Send(bytes);
        }
        /// <summary>
        /// Call to user
        /// </summary>
        /// <param name="recipient"></param>
        public void SendChatRequest(string recipient)
        {
            var str = string.Format("{0}|{1}|{2}|{3}",Chat.Chat.Request, recipient, UserName,ClientAddress);
            var bytes = Encoding.Unicode.GetBytes(str);
            server.Client.Send(bytes);
        }


        private void OnUserListReceived(string str, string userStr, string state) 
        {
            var handler = UserListReceived;
            if (handler != null) handler(str, new MessageEventArgs(string.Format("{0}|{1}",userStr,state)));
        }

        protected virtual void OnMessageReceived(string str, string sender)
        {
            var handler = MessageReceived;
            if (handler != null) handler(str, new MessageEventArgs(sender));
        }


        public void CloseConnection()
        {
            IsConnected = false;
            server.Client.Close();
        }

        public void EndChat()
        {
            clientSocket.Close();
        }


        public void AnswerIncomingCall(string caller, string address, string answer)
        {
            var str = string.Format("{0}|{1}|{2}|{3}|{4}", Chat.Chat.Response, caller, UserName, answer, ClientAddress);
            var bytes = Encoding.Unicode.GetBytes(str);
            if (answer == Chat.Chat.Accept)
                StartVoiceChat(address);
            server.Client.Send(bytes);
        }

       

        protected virtual void OnCallRecieved(string caller,string address)
        {
            var handler = CallRecieved;
            if (handler != null) handler(address, new MessageEventArgs(caller));
        }

        protected virtual void OnCallRequestResponded(string response)
        {
            var handler = CallRequestResponded;
            if (handler != null) handler(response, EventArgs.Empty);
        }
    }

    
}
