using System;
using System.ComponentModel;
using System.Linq;
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
                    var serverName = tbServerName.Text;
                    if (string.IsNullOrWhiteSpace(serverName))
                    {
                        ShowError();
                    }
                    else
                    {
                        server.StartServer();
                        SetControls(false);
                    }
                }
                catch
                {
                   ShowError();
                }
            }

            else
            {
                if (server == null) 
                    return;
                server.StopServer();
                SetControls(true);
            }
        }

        private void ShowError()
        {
            MessageBox.Show(@"Please enter valid port number and/or server name");
            cbStartStop.Checked = false;
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
        
        /// <summary>
        /// Obtain all network interfaces
        /// </summary>
        private void ObtainNetworkInterfaces()
        {
            const string anyInterface = "Any";
            var list = new BindingList<object>();
            var allInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var nic in allInterfaces.Where(i => (i.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                || (i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)))
            {
                list.Add(nic);
            }
            list.Add(anyInterface);
            cbInterfaces.DataSource = list;
            cbInterfaces.DisplayMember = "Description";
            cbInterfaces.ValueMember = null;
            cbInterfaces.SelectedItem = anyInterface;
        }

        

        
        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (server != null)
                server.StopServer();
        }




    }
}
