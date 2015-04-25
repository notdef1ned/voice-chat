namespace Backend.Helper
{
    public static class Log
    {
        #region Server
        public const string ServerStarted = "Server started";
        public const string ServerStopped = "Server stopped";
        #endregion

        #region Client
        public const string ClientConnected = "Client conencted";
        public const string ClientDisconnected = "Client disconnected";
        public const string TcpClientUnexpected = "TCP Connection closed unexpectedly";
        public const string UdpClientUnexpected = "UDP Connection closed unexpectedly";
        #endregion

        #region Common
        public const string ApplicationName = "Voice Chat";
        public const string Application = "Application";
        #endregion

        

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="msg">Custom message</param>
        /// <param name="e">Event</param>
        /// <returns></returns>
        public static string Message(string entity, string msg, string e)
        {
            return string.Format("{0} {1} {2}", entity, msg, e);
        }

        #endregion
    }
}
