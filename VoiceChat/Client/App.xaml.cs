using System.Windows;
using Backend.Client;
using Backend.Helpers;
using ChatControls.Client;

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

        /// <summary>
        /// Launching client window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var connectForm = new ConnectForm();
            if (connectForm.ShowDialog() != true)
                return;
            var chatClient = new ChatClient(connectForm.PortNumber, connectForm.IpAddress, connectForm.UserName);
            if (!chatClient.IsConnected)
            {
                Current.Shutdown();
                return;
            }

            RegistryHelper.Read(chatClient);

            var clientForm = new ClientWindow(chatClient);
            clientForm.Show();
        }
    }
}
