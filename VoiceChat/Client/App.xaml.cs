using System.Windows;
using ChatLibrary.ClientCore;
using Controls.Client;

namespace ClientBase
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var connectForm = new ConnectForm();
            if (connectForm.ShowDialog() != true)
                return;
            var chatClient = new ChatClient(connectForm.PortNumber, connectForm.IpAddress, connectForm.UserName);
            if (!chatClient.IsConnected) 
                return;
            var clientForm = new ClientForm(chatClient);
            clientForm.Show();
        }
    }
}
