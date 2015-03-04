using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class CustomEventArgs : EventArgs
    {
        public TcpClient ClientSock { get; set; }

        public CustomEventArgs(TcpClient tcpClient)
        {
            ClientSock = tcpClient;
        }
    }
}
