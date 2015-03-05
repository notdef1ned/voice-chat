using System.Windows.Forms;

namespace Client.UI
{
    public class ChatTabPage : TabPage
    {
        public TextBox DialogBox { get; set; }

        public ChatTabPage()
        {
            DialogBox = new TextBox {WordWrap = true, Dock = DockStyle.Fill, Multiline = true, ReadOnly = true,ScrollBars = ScrollBars.Vertical};
            Controls.Add(DialogBox);
        }
    }
}
