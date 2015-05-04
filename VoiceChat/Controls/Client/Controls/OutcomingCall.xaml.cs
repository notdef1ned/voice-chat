using BaseControls;

namespace ChatControls.Client.Controls
{
    /// <summary>
    /// Interaction logic for OurcomingCall.xaml
    /// </summary>
    public partial class OutcomingCall : ChatStackPanel
    {
        public OutcomingCall()
        {
            InitializeComponent();
            CancelButton = reject;
        }

    }
}
