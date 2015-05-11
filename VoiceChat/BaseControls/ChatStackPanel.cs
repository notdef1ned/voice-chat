using System.Windows.Controls;

namespace BaseControls
{
    public class ChatStackPanel : StackPanel
    {
        public ChatStackPanel()
        {
            MaxHeight = 100;
            MaxWidth = 300;
            MinHeight = 100;
            MinWidth = 300;
        }

        public Button AcceptButton { get; set; }
        public Button CancelButton { get; set; }
    }
}
