using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Server.Core;

namespace Server.UI
{
    public partial class ServerForm : Form
    {
        private ChatServer server;
        public delegate void SetListBoxItem(String str, String type); 
        public ServerForm()
        {
            InitializeComponent();
            ObtainNetworkInterfaces();
            if (EventLog.SourceExists(Log.ApplicationName)) 
                return;
            EventLog.CreateEventSource(Log.ApplicationName, Log.ApplicationName);
            
        }


        private void ckbServerControl_CheckedChanged(object sender, EventArgs e)
        {
            if (cbStartStop.Checked)
            {
                // validate the port number
                try
                {
                    var port = Int32.Parse(tbPort.Text);
                    server = new ChatServer(port, cbInterfaces.SelectedItem,tbServerName.Text);
                    server.ClientConnected += ServerOnClientConnected;
                    server.ClientDisconnected += ServerOnClientDisconnected;
                    server.StartServer();
                    SetControls(false);
                    WriteToEventLog(Log.Message(Log.Server, " on port " + port,Log.Start),EventLogEntryType.Information);
                }
                catch
                {
                    MessageBox.Show(@"Please enter valid port number");
                    cbStartStop.Checked = false;
                }
            }

            else
            {
                if (server == null) 
                    return;
                server.StopServer();
                SetControls(true);
                WriteToEventLog(Log.Message(Log.Server," ",Log.Stop),EventLogEntryType.Information);
            }
        }

        private void SetControls(bool enabled)
        {
            tbPort.Enabled = enabled;
            tbServerName.Enabled = enabled;
            cbInterfaces.Enabled = enabled;
        }

        private void ServerOnClientDisconnected(object sender, EventArgs e)
        {
            var userName = (string) sender;
            var item = FormatClient(userName, e);
            UpdateConnectedClients(item, "Delete"); 
        }

        private void ServerOnClientConnected(object sender, EventArgs e)
        {
            var userName = (string) sender;
            var item = FormatClient(userName, e);
            UpdateConnectedClients(item, "Add"); 
        }

        private static string FormatClient(string userName,EventArgs e)
        {
            var args = e as CustomEventArgs;
            if (args == null)
                return string.Empty;

            var client = args.ClientSocket;
            var remoteEndPoint = (IPEndPoint)client.RemoteEndPoint;
            var remoteIp = remoteEndPoint.Address.ToString();
            var remotePort = remoteEndPoint.Port.ToString();

            return string.Format("{0} ({1}:{2})", userName, remoteIp, remotePort);

        }


        private void UpdateConnectedClients(string str, string type)
        {
         
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (connectedClients.InvokeRequired)
            {
                SetListBoxItem d = UpdateConnectedClients;
                Invoke(d, str, type);
            }
            else
            {
                // If type is Add, the add Client info in Tree View
                if (type.Equals("Add"))
                {
                    connectedClients.Items.Add(str);
                }
                // Else delete Client information from Tree View
                else
                {
                   connectedClients.Items.Remove(str);
                }

            }
        }
        

        private void ObtainNetworkInterfaces()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var i in interfaces)
                cbInterfaces.Items.Add(i.Description);
        }

        private static void WriteToEventLog(string message, EventLogEntryType type)
        {
            EventLog.WriteEntry(Log.ApplicationName, message, type);
        }

        

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (server != null)
                server.StopServer();
        }




    }
}
