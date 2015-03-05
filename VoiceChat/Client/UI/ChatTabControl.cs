using System.Windows.Forms;

namespace Client.UI
{
    public class ChatTabControl : TabControl
    {
        public ChatTabPage GlobalPage { get; set; }

        public ChatTabControl()
        {
            GlobalPage = new ChatTabPage {Text = Chat.Chat.Global};
            TabPages.Add(GlobalPage);
        }
    }
}
