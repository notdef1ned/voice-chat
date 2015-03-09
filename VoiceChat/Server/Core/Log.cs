using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class Log
    {
        #region Server
        public const string Server = "Server";
        public const string Stop =  "stopped";
        public const string Start = "started";
        #endregion

        #region Client
        public const string Client = "Client";
        public const string Connected = "connected";
        public const string Disconnected = "disconnected";
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
