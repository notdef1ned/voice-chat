using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ChatLibrary;
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
                clientSocket.Bind(server.Client.LocalEndPoint);
                ServerAddress = serverAddress;
                UserName = userName;
            }
            catch (SocketException e)
            {
                MessageBox.Show(@"Unable to connect to server");
                return;
            }
           // Send username to server
            var bytes = Encoding.ASCII.GetBytes(userName);
            server.Client.Send(bytes);
        }


        public void Init()
        {
            var state = new Chat.StateObject
            {
                WorkSocket = server.Client
            };

            //Receive list of users online 
            ReceiveUsersList();

            heartBeatThread = new Thread(HeartBeat);
            heartBeatThread.Start();

            server.Client.BeginReceive(state.Buffer, 0, Chat.StateObject.BufferSize, 0,
                OnReceive, state);
        }

        /// <summary>
        /// Heartbeat request to server
        /// </summary>
        private void HeartBeat()
        {
            var heartbeat = string.Format("{0}|{1}", Chat.Heartbeat, UserName);
            var bytes = Encoding.ASCII.GetBytes(heartbeat);
            Timer timer = null;
            while (IsConnected)
            {
                if (timer == null)
                {
                    timer = new Timer(1000);
                    timer.Elapsed += (sender, args) =>
                    {
                        server.Client.Send(bytes);
                    };
                }
                timer.Enabled = true;
            }
            
        }

        private void ReceiveUsersList()
        {
            var bytes = new byte[1024];
            var bytesRead = server.Client.Receive(bytes);
            var content = Encoding.ASCII.GetString(bytes, 0, bytesRead);
            var splittedStr = content.Split('|');
            var list = splittedStr[2];
            OnUserListReceived(list,splittedStr[3],splittedStr[4]);
        }

        public void OnReceive(IAsyncResult ar)
        {
            var state = ar.AsyncState as Chat.StateObject;
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
                state.Sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                var content = state.Sb.ToString();  

                ParseMessage(content);
                

                server.Client.BeginReceive(state.Buffer, 0, Chat.StateObject.BufferSize, 0, OnReceive, state);
            }
            catch (SocketException socketException)
            {
                MessageBox.Show("GOVNO!!!!!!");
                //WSAECONNRESET, the other side closed impolitely
                if (socketException.ErrorCode == 10054 || ((socketException.ErrorCode != 10004) && (socketException.ErrorCode != 10053)))
                {
                    handler.Close();
                }
                throw;
            }
        }

        public void OnUdpSend(IAsyncResult ar)
        {

        }

        public void OnUdpRecieve(IAsyncResult ar)
        {
            var state = ar.AsyncState as Chat.StateObject;
            if (state == null) 
                return;
            var handler = clientSocket;
           
            var bytesRead = handler.EndReceive(ar);
            
            IWaveProvider provider = new RawSourceWaveStream(
                         new MemoryStream(state.Buffer,0,bytesRead), new WaveFormat());
            recievedStream.Init(provider);
            recievedStream.Play();
            
            var endPoint = (EndPoint) remoteEndPoint;
            handler.BeginReceiveFrom(state.Buffer, 0, Chat.StateObject.BufferSize, SocketFlags.None, ref endPoint, OnUdpRecieve, state);
            
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
                case Chat.Message:
                    switch (splittedMessage[1])
                    {
                        case Chat.Server:
                            OnUserListReceived(splittedMessage[2],splittedMessage[3],splittedMessage[4]);
                        break;
                        default:
                            OnMessageReceived(splittedMessage[3], splittedMessage[2]);  
                        break;
                    }
                break;
                case Chat.Request:
                    OnCallRecieved(splittedMessage[2],splittedMessage[3]);
                break;
                case Chat.Response:
                    ParseResponse(splittedMessage[3],splittedMessage[4]);
                    OnCallRequestResponded(splittedMessage[3]);
                break;
            }
        }

        private void ParseResponse(string response,string address)
        {
            switch (response)
            {
                case Chat.Accept:
                    StartVoiceChat(address);
                break;
                case Chat.Decline:
                break;
            }
        }

        private void StartVoiceChat(string address)
        {
            var splittedAddress = address.Split(':');
            var ip = splittedAddress[0];
            var port = splittedAddress[1];
            
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), Int32.Parse(port));

            recievedStream = new WaveOut(WaveCallbackInfo.FunctionCallback());

            sourceStream = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(0).Channels)
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
            var state = new Chat.StateObject
            {
                WorkSocket = clientSocket
            };
            var endPoint = (EndPoint) remoteEndPoint;
            clientSocket.BeginSendTo(buf, 0, bytesRecorded, SocketFlags.None, endPoint, OnUdpSend, state);
        }

        private void ReceiveUdpData()
        {
            var state = new Chat.StateObject
            {
                WorkSocket = clientSocket
            };
            
            var endPoint = (EndPoint) remoteEndPoint;
            clientSocket.BeginReceiveFrom(state.Buffer, 0, Chat.StateObject.BufferSize, SocketFlags.None, ref endPoint, OnUdpRecieve, state);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="recipient"></param>
        public void SendMessage(string message,string recipient)
        {
            var str = string.Format("{0}|{1}|{2}|{3}",Chat.Message, recipient, UserName, message);
            var bytes = Encoding.ASCII.GetBytes(str);
            server.Client.Send(bytes);
        }
        /// <summary>
        /// Call to user
        /// </summary>
        /// <param name="recipient"></param>
        public void SendChatRequest(string recipient)
        {
            var str = string.Format("{0}|{1}|{2}",Chat.Request, recipient, UserName);
            var bytes = Encoding.ASCII.GetBytes(str);
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


        public void AnswerIncomingCall(string caller, string address, string answer)
        {
            var str = string.Format("{0}|{1}|{2}|{3}", Chat.Response, caller, UserName, answer);
            var bytes = Encoding.ASCII.GetBytes(str);
            if (answer == Chat.Accept)
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
