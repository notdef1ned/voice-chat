using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChatLibrary.Helper;

namespace Server.Core
{
    public class ChatServer
    {
        #region Fields
        private TcpListener tcpServer;
        private Thread mainThread;
        private readonly int portNumber;
        private bool isRunning;
        private NetworkInterface networkInterface;
        private readonly string serverName;
        private readonly Dictionary<string, Socket> userNames = new Dictionary<string, Socket>(); 
        public event EventHandler ClientConnected;
        public event EventHandler ClientDisconnected;
        #endregion

        #region Constructor

        public ChatServer(int portNumber, object networkInterface, string serverName)
        {
            this.serverName = serverName;
            this.portNumber = portNumber;
            this.networkInterface = networkInterface as NetworkInterface;
        }

        #endregion

        #region Server Start/Stop

        public void StartServer()
        {
            mainThread = new Thread(StartListen);
            mainThread.Start();
        }
        /// <summary>
        /// Server listens to specified port and accepts connection from client
        /// </summary>
        public void StartListen()
        {
            var ip = (networkInterface != null)
                ? GetInterfaceIpAddress()
                : IPAddress.Any;

            tcpServer = new TcpListener(ip, portNumber);
            tcpServer.Start();
            
            isRunning = true;
            // Keep accepting client connection
            while (isRunning)
            {
                if (!tcpServer.Pending())
                {
                    Thread.Sleep(500);
                    continue;
                }
                // New client is connected, call event to handle it
                var clientThread = new Thread(NewClient);
                var tcpClient = tcpServer.AcceptTcpClient();
                tcpClient.ReceiveTimeout = 20000;
                clientThread.Start(tcpClient.Client);
            }
        }

        private IPAddress GetInterfaceIpAddress()
        {
            var ipAddresses = networkInterface.GetIPProperties().UnicastAddresses;
            return (from ip in ipAddresses where ip.Address.AddressFamily == AddressFamily.InterNetwork select ip.Address).FirstOrDefault();
        }

        /// <summary>
        /// Method to stop TCP communication, it kills the thread and closes clients' connection
        /// </summary>
        public void StopServer()
        {
            isRunning = false;
            if (tcpServer == null)
                return;
            userNames.Clear();
            tcpServer.Stop();
        }

        #endregion

        #region Add/Remove Clients
        public void NewClient(object obj)
        {
            ClientAdded(this, new CustomEventArgs((Socket)obj));
        }


        public void ClientAdded(object sender, EventArgs e)
        {
            var socket = ((CustomEventArgs) e).ClientSocket;
            var bytes = new byte[1024];
            var bytesRead = socket.Receive(bytes);
           
            var newUserName = Encoding.Unicode.GetString(bytes, 0, bytesRead);

            if (userNames.ContainsKey(newUserName))
            {
                SendNameAlreadyExist(socket, newUserName);
                return;
            }

            userNames.Add(newUserName, socket);
            OnClientConnected(socket, newUserName);

            foreach (var user in userNames)
                SendUsersList(user.Value, user.Key, newUserName, ChatHelper.Connected);
           
            var state = new ChatHelper.StateObject
            {
                WorkSocket = socket
            };
            
            socket.BeginReceive(state.Buffer, 0, ChatHelper.StateObject.BufferSize, 0,
            OnReceive, state);
        }


        public void SendNameAlreadyExist(Socket clientSocket, string name)
        {
            var data = new Data {Command = Command.NameExist, To = name};
            clientSocket.Send(data.ToByte());
        }

        public void SendUsersList(Socket clientSocket, string userName, string changedUser, string state)
        {
            var data = new Data
            {
                Command = Command.Broadcast,
                To = userName,
                Message = string.Format("{0}|{1}|{2}|{3}",
                    string.Join(",", userNames.Keys.Where(u => u != userName).ToArray()), changedUser, state,
                    serverName)
            };

                        
            clientSocket.Send(data.ToByte());
        }

        public void OnReceive(IAsyncResult ar)
        {
            var state = ar.AsyncState as ChatHelper.StateObject;
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

                ParseRequest(state,handler);
                
                // Restore receiving
                handler.BeginReceive(state.Buffer, 0, ChatHelper.StateObject.BufferSize, 0,
                OnReceive, state);

            }
            catch (Exception)
            {
                DisconnectClient(handler);
            }
        }

        private void ParseRequest(ChatHelper.StateObject state, Socket incomingClient)
        {
            var data = new Data(state.Buffer);
            Socket clientSocket;
            if (userNames.TryGetValue(data.To, out clientSocket))
                clientSocket.Send(data.ToByte());
        }


        public void DisconnectClient(Socket clientSocket)
        {
            var userName = userNames.FirstOrDefault(k => k.Value == clientSocket).Key;
            OnClientDisconnected(clientSocket, userName);

            clientSocket.Close();
            userNames.Remove(userName);

            foreach (var user in userNames)
                SendUsersList(user.Value, user.Key, userName, ChatHelper.Disconnected);
        }

       #endregion

        #region Event Invokers

        protected virtual void OnClientConnected(Socket clientSocket, string name)
        {
            var handler = ClientConnected;
            if (handler != null) handler(name, new CustomEventArgs(clientSocket));
        }

        protected virtual void OnClientDisconnected(Socket clientSocket, string name)
        {
            var handler = ClientDisconnected;
            if (handler != null) handler(name, new CustomEventArgs(clientSocket));
        }

        #endregion
    }
}
