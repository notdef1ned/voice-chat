using System.Windows.Forms;
using ChatLibrary;

namespace Client.UI
{
    public class ChatTabControl : TabControl
    {
        public ChatTabPage GlobalPage { get; set; }

        public ChatTabControl()
        {
            GlobalPage = new ChatTabPage {Text = Chat.Global};
            TabPages.Add(GlobalPage);
        }
    }
}
