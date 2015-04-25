using System;

namespace Backend.Client
{
    public class ServerEventArgs : EventArgs
    {
        public string Message { get; set; }

        public string ServerName { get; set; }

        public string Info { get; set; }

        public string User { get; set; }

        public ServerEventArgs(string changedUser, string info, string serverName)
        {
            User = changedUser;
            ServerName = serverName;
            Info = info;
        }

        public ServerEventArgs(string msg)
        {
            Message = msg;
        }

    }
}
