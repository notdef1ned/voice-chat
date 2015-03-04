using System;
using System.Windows.Forms;
using Client.Client;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var connectForm = new ConnectForm();
            if (connectForm.ShowDialog() != DialogResult.OK)
                return;
            var chatClient = new ChatClient(connectForm.PortNumber, connectForm.IpAddress, connectForm.UserName);
            Application.Run(new ClientForm(chatClient));
        }
    }
}
