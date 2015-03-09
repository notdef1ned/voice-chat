using System.Windows.Forms;
using ChatLibrary.Helper;

namespace Client.UI
{
    public class ChatTabControl : TabControl
    {
        public ChatTabPage GlobalPage { get; set; }

        public ChatTabControl()
        {
            GlobalPage = new ChatTabPage {Text = ChatHelper.Global};
            TabPages.Add(GlobalPage);
        }
    }
}
