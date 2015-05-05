using System;
using System.Linq;
using System.Windows;
using Backend.Helpers;

namespace ChatControls.Client
{
    public partial class ConnectForm
    {
        #region Properties
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public int PortNumber { get; set; }
        
        #endregion
        public ConnectForm()
        {
            InitializeComponent();
            Closing += ConnectForm_Closing;
        }
        void ConnectForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == false)
                Application.Current.Shutdown();
        }

        private void tbHost_TextChanged(object sender, EventArgs e)
        {
            IpAddress = tbHost.Text;
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            UserName = tbUser.Text;
        }

        private void tbPort_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var number = int.Parse(tbPort.Text);
                if (Enumerable.Range(0, 65536).Contains(number))
                    PortNumber = number;
            }
            catch (Exception)
            {
                MessageBox.Show(ChatHelper.PORT_ERROR);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CbIsLocal_OnChecked(object sender, RoutedEventArgs e)
        {
            SetHostTextBox(false);
        }

        private void SetHostTextBox(bool isEnabled)
        {
            tbHost.IsEnabled = isEnabled;
            tbHost.Text = (isEnabled) ? string.Empty : ChatHelper.LOCAL;
        }

        private void CbIsLocal_OnUnchecked(object sender, RoutedEventArgs e)
        {
            SetHostTextBox(true);
        }
    }
}
