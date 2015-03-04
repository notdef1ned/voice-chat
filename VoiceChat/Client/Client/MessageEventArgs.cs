using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Client
{
    public class MessageEventArgs : EventArgs
    {
        public string Sender { get; set; }

        public MessageEventArgs(string sender)
        {
            Sender = sender;
        }
    }
}
