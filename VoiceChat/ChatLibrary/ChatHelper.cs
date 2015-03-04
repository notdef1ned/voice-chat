using System;
using System.Net.Sockets;
using System.Text;

namespace ChatLibrary
{
    public static class Chat
    {
        #region Fields

        #region Interaction Type
        public const string Message = "message";
        public const string Request = "request";
        public const string Response = "response";
        #endregion

        #region Interaction Subject
        public const string Server = "server";
        #endregion

        #region Messages

        public const string Accept = "accept";
        public const string Decline = "decline";

        #endregion

        #region Titles

        public const string Conversation = "Conversation";
        public const string IncomingCall = "Incoming Call";
        public const string OutcomingCall = "Calling";
        #endregion

        #endregion



        public class StateObject
        {
            // Client  socket.
            public Socket WorkSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 100000000;
            // Receive buffer.
            public byte[] Buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder Sb = new StringBuilder();
        }
    }
}
