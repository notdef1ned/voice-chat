using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Backend.Helper;

namespace Backend.Helpers
{
    public static class ChatHelper
    {
        #region Titles

        public const string CONVERSATION = "Conversation";
        public const string INCOMING_CALL = "Incoming call from";
        public const string OUTCOMING_CALL = "Calling to";
        public const string FILE_TRANSFER = "Recieve file from {0}?";
        public const string TRANSFER_CANCELED = "File Transfer canceled";
        public const string TRANSFERED = "File {0} successfully transfered";
        public const string GLOBAL = "Global";
        public const string SETTINGS = "Settings";
        public const string CONNECTED = "connected";
        public const string DISCONNECTED = "disconnected";
        public const string LOCAL = "127.0.0.1";
        public const string VERSION = "1.0";
        public const string APP_NAME = "VoiceChat";
        public const string SOFTWARE = "Software";
        public const string NO_USERS_ONLINE = "no users online";
        public const string PROFILE = "Profile";
        public const string FILE_FILTER_ALL = "All files (*.*)|*.*";
        #endregion

        #region Registry Keys

        public const string LAUNCH_ON_STARTUP = "LaunchOnStartup";
        public const string DOUBLE_CLICK_TO_CALL = "DoubleClickToCall";
        public const string SCHEME = "Scheme";
        public const string DARK = "Dark";
        public const string LIGHT = "Light";

        #endregion

        #region Errors
        public const string PORT_ERROR = "Port number should be between 0 and 65535";
        #endregion

        #region Messages 
        public static string WelcomeMessage = string.Format("{0}: ** Welcome to main chat room, Click on any user to start chat**\n", DateTime.Now.ToString("HH:mm:ss"));
        #endregion

        public class StateObject
        {
            // Client  socket.
            public Socket WorkSocket = null;
            // Size of receive buffer.
            public const int BUFFER_SIZE = 5242880;
            // Receive buffer.
            public byte[] Buffer = new byte[BUFFER_SIZE];
            // Received data string.
            public StringBuilder Sb = new StringBuilder();
        }

        public static void WriteToEventLog(string message, EventLogEntryType type)
        {
            EventLog.WriteEntry(Log.ApplicationName, message, type);
        }

        public static string ChatWith(string name)
        {
            return string.Format("** Conversation with {0} **\n", name);
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
        public byte[] File { get; set; }
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
            // Next 4 bytes store length of file
            var fileLength = BitConverter.ToInt32(data, next);
            next += sizeof (int);
            if (fileLength > 0)
            {
                var file = new byte[fileLength];
                Array.Copy(data, next, file, 0, fileLength);
                File = file;
                next += fileLength;
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
            var zeroBytes = BitConverter.GetBytes(0);
            var result = new List<byte>();
            result.AddRange(BitConverter.GetBytes((int)Command));
            if (To != null)
            {
                Encode(To, result);
            }
            else
                result.AddRange(zeroBytes);

            if (From != null)
            {
                Encode(From, result);
            }
            else
                result.AddRange(zeroBytes);
            
            if (Message != null)
            {
                Encode(Message, result);
            }
            else
                result.AddRange(zeroBytes);

            if (File != null)
            {
                Encode(File, result);
            }
            else
                result.AddRange(zeroBytes);

            if (ClientAddress != null)
            {
                Encode(ClientAddress, result);
            }
            else
                result.AddRange(zeroBytes);
            
            return result.ToArray();
        }

        private static void Encode(string str, List<byte> result)
        {
            var encoded = Encoding.Unicode.GetBytes(str);
            result.AddRange(BitConverter.GetBytes(str.Length));
            result.AddRange(encoded);
        }

        private static void Encode(IReadOnlyCollection<byte> file, List<byte> result)
        {
            result.AddRange(BitConverter.GetBytes(file.Count));
            result.AddRange(file);
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
        SendFile,
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
