using System.Windows;
using Controls.Server;

namespace Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var serverForm = new ServerForm();
            serverForm.Show();
        }
    }
}
