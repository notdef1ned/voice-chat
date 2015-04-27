using System.Windows.Controls;
using Backend.Helpers;

namespace BaseControls
{
    public class BaseTabControl : TabControl
    {
        public ChatTabItem GlobalPage { get; set; }

        public BaseTabControl()
        {
            GlobalPage = new ChatTabItem {Header = ChatHelper.GLOBAL};
            Items.Add(GlobalPage);
        }
    }
}
