using System;

namespace Client.Client
{
    public class ServerEventArgs : EventArgs
    {
        public string Message { get; set; }

        public string ServerName { get; set; }

        public ServerEventArgs(string sender)
        {
            Message = sender;
        }

    }
}
