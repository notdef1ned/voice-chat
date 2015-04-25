using System;
using System.Net.Sockets;

namespace Backend.Server
{
    public class CustomEventArgs : EventArgs
    {
        public Socket ClientSocket { get; set; }

        public CustomEventArgs(Socket clientSocket)
        {
            ClientSocket = clientSocket;
        }
    }
}
