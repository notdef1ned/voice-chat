using System;
using System.Windows.Forms;

namespace Client
{
    public partial class ConnectForm : Form
    {

        #region Properties

        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public int PortNumber { get; set; }
        
        #endregion
        public ConnectForm()
        {
            InitializeComponent();
        }

        private void tbHost_TextChanged(object sender, EventArgs e)
        {
            IpAddress = tbHost.Text;
        }


        private void tbName_TextChanged(object sender, EventArgs e)
        {
            UserName = tbName.Text;
        }

        private void tbPort_TextChanged(object sender, EventArgs e)
        {
            PortNumber = Int32.Parse(tbPort.Text);
        }
    }
}
