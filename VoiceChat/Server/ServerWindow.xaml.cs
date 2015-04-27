using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using Backend.Server;

namespace Server
{
    public partial class ServerWindow
    {
        private ChatServer server;
        public delegate void SetListBoxItem(string str, string type);
        public ServerWindow()
        {
            InitializeComponent();
            ObtainNetworkInterfaces();
        }


        private void cbStartStop_Checked(object sender, RoutedEventArgs e)
        {
            if (cbStartStop.IsChecked == true)
            {
                // validate the port number
                try
                {
                    var port = Int32.Parse(tbPortNumber.Text);
                    server = new ChatServer(port, cbInterfaces.SelectedItem, tbServerName.Text);
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
            cbStartStop.IsChecked = false;
        }

        private void SetControls(bool enabled)
        {
            tbPortNumber.IsEnabled = enabled;
            tbServerName.IsEnabled = enabled;
            cbInterfaces.IsEnabled = enabled;
        }

        private void ServerOnClientDisconnected(object sender, EventArgs e)
        {
            var userName = (string)sender;
            var item = FormatClient(userName, e);
            UpdateConnectedClients(item, "Delete");
        }

        private void ServerOnClientConnected(object sender, EventArgs e)
        {
            var userName = (string)sender;
            var item = FormatClient(userName, e);
            UpdateConnectedClients(item, "Add");
        }

        private static string FormatClient(string userName, EventArgs e)
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
            if (!connectedClients.Dispatcher.CheckAccess())
            {
                SetListBoxItem d = UpdateConnectedClients;
                connectedClients.Dispatcher.Invoke(d, str, type);
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
            var anyInterface = new NetworkInterfaceDescription("Any");
            var list = new BindingList<object>();
            var allInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var nic in allInterfaces.Where(i => (i.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                || (i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)))
            {
                list.Add(nic);
            }
            list.Add(anyInterface);
            cbInterfaces.ItemsSource = list;
            cbInterfaces.DisplayMemberPath = "Description";
            cbInterfaces.SelectedItem = anyInterface;
        }


        private void ServerForm_OnClosing(object sender, CancelEventArgs e)
        {
            if (server != null)
                server.StopServer();
        }

    }
}
