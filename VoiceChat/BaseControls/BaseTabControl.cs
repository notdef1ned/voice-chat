using System.Windows.Controls;
using Backend.Helper;

namespace BaseControls
{
    public class BaseTabControl : TabControl
    {
        public ChatTabItem GlobalPage { get; set; }

        public BaseTabControl()
        {
            GlobalPage = new ChatTabItem {Header = ChatHelper.Global};
            Items.Add(GlobalPage);
        }
    }
}
