using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChatLibrary;

namespace Server.Server
{
    public class ChatServer
    {
        #region Fields
        private TcpListener tcpServer;
        private TcpClient tcpClient;
        private Thread mainThread;
        private readonly int portNumber;
        private NetworkInterface networkInterface;
        private readonly Dictionary<TcpClient,Thread> clients = new Dictionary<TcpClient, Thread>();
        private readonly Dictionary<string, TcpClient> userNames = new Dictionary<string, TcpClient>();  
        public event EventHandler ClientConnected;
        public event EventHandler ClientDisconnected;
        #endregion

       

        #region Constructor

        public ChatServer(int portNumber, object networkInterface)
        {
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
            tcpServer = new TcpListener(IPAddress.Any,portNumber);
            tcpServer.Start();

            // Keep accepting client connection
            while (true)
            {
                // New client is connected, call event to handle it
                var clientThread = new Thread(NewClient);
                tcpClient = tcpServer.AcceptTcpClient();
                clientThread.Start(tcpClient);
            }
        }
        /// <summary>
        /// Method to stop TCP communication, it kills the thread and closes clients' connection
        /// </summary>
        public void StopServer()
        {
            if (tcpServer == null)
                return;

            foreach (var record in clients)
            {
                record.Key.Client.Close();
                record.Value.Abort();
            }

            clients.Clear();
            userNames.Clear();
            
            mainThread.Abort();
            tcpServer.Stop();
        }

        #endregion


        #region Add/Remove Clients
        public void NewClient(object obj)
        {
            ClientAdded(this, new CustomEventArgs((TcpClient)obj));
        }


        public void ClientAdded(object sender, EventArgs e)
        {
            tcpClient = ((CustomEventArgs) e).ClientSock;
           
            // update clients list
            var bytes = new byte[1024];
            var bytesRead = tcpClient.Client.Receive(bytes);
            var str = Encoding.ASCII.GetString(bytes, 0, bytesRead);

            OnClientConnected(tcpClient, str);
            clients.Add(tcpClient, Thread.CurrentThread);
            userNames.Add(str, tcpClient);

            foreach (var user in userNames)
                SendUsersList(user.Value,user.Key);
           
            var state = new Chat.StateObject
            {
                WorkSocket = tcpClient.Client
            };
            
            tcpClient.Client.BeginReceive(state.Buffer, 0, Chat.StateObject.BufferSize, 0,
                OnReceive, state);

        }

        public void SendUsersList(TcpClient client, string userName)
        {
            var userList = string.Format("{0}|{1}|{2}", Chat.Message, Chat.Server,
                string.Join(",", userNames.Keys.Where(u => u != userName).ToArray()));
            var bytes = Encoding.ASCII.GetBytes(userList);
            client.Client.Send(bytes);
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

                var recievedString = Encoding.ASCII.GetString(state.Buffer, 0, bytesRead);
                var str = recievedString.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                var sender = userNames[str[2]];
                
                TcpClient client;
                if (userNames.TryGetValue(str[1], out client))
                {
                    recievedString += "|" + GetRemoteAddress(sender);
                    var bytes = Encoding.ASCII.GetBytes(recievedString);
                    client.Client.Send(bytes);
                }
                
                
                // Restore receiving
                handler.BeginReceive(state.Buffer, 0, Chat.StateObject.BufferSize, 0,
                OnReceive, state);

            }
            catch (Exception)
            {
                DisconnectClient(tcpClient);
                
            }
        }


        private string GetRemoteAddress(TcpClient client)
        {
            var endPoint = (IPEndPoint) client.Client.RemoteEndPoint;
            return endPoint.Address + ":" + endPoint.Port;
        }




        public void DisconnectClient(TcpClient client)
        {
            Thread thread;
            if (!clients.TryGetValue(client, out thread)) 
                return;
            var userName = userNames.FirstOrDefault(k => k.Value == client).Key;
            OnClientDisconnected(client, userName);

            client.Client.Close();
            thread.Abort();
            clients.Remove(client);
            userNames.Remove(userName);

            foreach (var user in userNames)
                SendUsersList(user.Value,user.Key);
           }

       

        #endregion




        protected virtual void OnClientConnected(TcpClient client, string name)
        {
            var handler = ClientConnected;
            if (handler != null) handler(name, new CustomEventArgs(client));
        }

        protected virtual void OnClientDisconnected(TcpClient client, string name)
        {
            var handler = ClientDisconnected;
            if (handler != null) handler(name, new CustomEventArgs(client));
        }
    }
}
