using BaseControls;

namespace ChatControls.Client.Controls
{
    /// <summary>
    /// Interaction logic for Conversation.xaml
    /// </summary>
    public partial class Conversation : ChatStackPanel
    {
        public Conversation()
        {
            InitializeComponent();
            AcceptButton = endConversation;
        }
    }
}
