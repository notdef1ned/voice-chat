using BaseControls;

namespace ChatControls.Client.Controls
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
