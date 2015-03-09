using System;
using System.Linq;
using System.Windows.Forms;
using ChatLibrary.Helper;

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
            try
            {
                var number = Int32.Parse(tbPort.Text);
                if (Enumerable.Range(0,65536).Contains(number))
                    PortNumber = number;    
            }
            catch (Exception)
            {
                MessageBox.Show(ChatHelper.PortError);
            }
        }
    }
}
