using System;
using System.Windows;
using System.Windows.Controls;

namespace BaseControls
{
    public class ChatTabItem : TabItem
    {
        public TextBox DialogBox { get; set; }

        public ChatTabItem()
        {
            DialogBox = new TextBox
            {
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Stretch,
                IsReadOnly = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0)
            };
            SetStyle();
            // Creating the Grid (create Canvas or StackPanel or other panel here)
            var grid = new Grid();
            grid.Children.Add(DialogBox);
            // Adding content 
            Content = grid;
        }

        private void SetStyle()
        {
            SetTabStyle();
        }


        protected virtual void SetTabStyle()
        {
            Resources.Source = new Uri("/BaseControls;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            Style = (Style)FindResource("TabItemStyle");
        }
    }
}
