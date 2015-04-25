using System.Windows.Controls;
using BaseControls;

namespace Controls.Client.Controls
{
    /// <summary>
    /// Interaction logic for IncomingCall.xaml
    /// </summary>
    public partial class IncomingCall : ChatStackPanel
    {
        public IncomingCall()
        {
            InitializeComponent();
            AcceptButton = btnAccept;
            CancelButton = btnDecline;
        }

    }
}
