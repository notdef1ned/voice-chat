using System.Windows.Forms;

namespace Client
{
    public class ChatTabPage : TabPage
    {
        public TextBox DialogBox { get; set; }

        public ChatTabPage()
        {
            DialogBox = new TextBox {WordWrap = true, Dock = DockStyle.Fill, Multiline = true};
            Controls.Add(DialogBox);
        }
    }
}
