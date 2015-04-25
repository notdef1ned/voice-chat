using System.Windows;
using System.Windows.Controls;

namespace Controls.Client
{
    public class ChatTabItem : TabItem
    {
        public TextBox DialogBox { get; set; }

        public ChatTabItem()
        {
            DialogBox = new TextBox {TextWrapping = TextWrapping.Wrap, VerticalAlignment =  VerticalAlignment.Stretch,  IsReadOnly = true,VerticalScrollBarVisibility = ScrollBarVisibility.Visible};
            // Creating the Grid (create Canvas or StackPanel or other panel here)
            var grid = new Grid();
            grid.Children.Add(DialogBox);
            // Adding content 
            this.Content = grid;
        }
    }
}
