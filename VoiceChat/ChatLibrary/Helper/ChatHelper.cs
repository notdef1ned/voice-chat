using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ChatLibrary.Helper
{
    public static class ChatHelper
    {
        #region Titles

        public const string Conversation = "Conversation";
        public const string IncomingCall = "Incoming Call";
        public const string OutcomingCall = "Calling";
        public const string Global = "Global";
        public const string Connected = "connected";
        public const string Disconnected = "disconnected";
        #endregion

        #region Errors
        public const string PortError = "Port number should be between 0 and 65535";
        #endregion

        public class StateObject
        {
            // Client  socket.
            public Socket WorkSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 65536;
            // Receive buffer.
            public byte[] Buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder Sb = new StringBuilder();
        }

        public static void WriteToEventLog(string message, EventLogEntryType type)
        {
            EventLog.WriteEntry(Log.ApplicationName, message, type);
        }
    }

    /// <summary>
    /// Data structure to interact with server
    /// </summary>
    public class Data
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public string ClientAddress { get; set; }
        public Command Command { get; set; }
        public Data()
        {
            Command = Command.Null;
            From = null;
            To = null;
            Message = null;
            ClientAddress = null;
        }

        public Data(byte[] data)
        {
            // First four bytes are for the Command
            Command = (Command)BitConverter.ToInt32(data, 0);
            // Next 4 bytes store length of the recipient name
            var next = sizeof(int);
            var nameLength = BitConverter.ToInt32(data, next) * 2;
            next += sizeof (int);
            if (nameLength > 0)
            {
                To = Encoding.Unicode.GetString(data, next, nameLength);
                next += nameLength;
            }
            // Next 4 bytes store length of the sender name
            var senderNameLength = BitConverter.ToInt32(data, next) * 2;
            next += sizeof(int);
            if (senderNameLength > 0)
            {
                From = Encoding.Unicode.GetString(data, next, senderNameLength);
                next += senderNameLength;
            }
            // Next 4 bytes store length of the message
            var messageLength = BitConverter.ToInt32(data, next) * 2;
            next += sizeof(int);
            if (messageLength > 0)
            {
                Message = Encoding.Unicode.GetString(data, next, messageLength);
                next += messageLength;
            }
            // Next 4 bytes store length of the client address (UDP)
            var clientAddressLength = BitConverter.ToInt32(data, next) * 2;
            next += sizeof (int);
            if (clientAddressLength > 0)
            {
                ClientAddress = Encoding.Unicode.GetString(data, next, clientAddressLength);
            }
        }
        /// <summary>
        /// Encodes data structure
        /// </summary>
        /// <returns></returns>
        public byte[] ToByte()
        {
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((int)Command));
            if (To != null)
            {
                Encode(To, result);
            }
            else
                result.AddRange(BitConverter.GetBytes(0));

            if (From != null)
            {
                Encode(From, result);
            }
            else
                result.AddRange(BitConverter.GetBytes(0));
            
            if (Message != null)
            {
                Encode(Message, result);
            }
            else
                result.AddRange(BitConverter.GetBytes(0));
            
            if (ClientAddress != null)
            {
                Encode(ClientAddress, result);
            }
            else
                result.AddRange(BitConverter.GetBytes(0));
            
            return result.ToArray();
        }

        private void Encode(string str, List<byte> result)
        {
            var encoded = Encoding.Unicode.GetBytes(str);
            result.AddRange(BitConverter.GetBytes(str.Length));
            result.AddRange(encoded);
        }
    }

    /// <summary>
    /// List of availlable commands
    /// </summary>
    public enum Command
    {
        Broadcast,
        Disconnect,
        SendMessage,
        Call,
        AcceptCall,
        CancelCall,
        EndCall,
        Busy,
        NameExist,
        Null
    }
    /// <summary>
    /// Represents connected client
    /// </summary>
    public class ConnectedClient
    {
        private readonly string userName;
        private readonly Socket connection;
        public bool IsConnected { get; set; }
        public Socket Connection 
        {
            get { return connection; }
        }
        public string UserName 
        {
            get { return userName; }
        }

        public ConnectedClient(string userName, Socket connection)
        {
            this.userName = userName;
            this.connection = connection;
        }
    }


}
