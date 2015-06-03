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

    public class FileEventArgs : EventArgs
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string From { get; set; }
        public string Extenstion { get; set; }

        public FileEventArgs(byte[] file, string from, string fileName, string extension)
        {
            File = file;
            From = from;
            FileName = fileName;
            Extenstion = extension;
        }
    }
}
