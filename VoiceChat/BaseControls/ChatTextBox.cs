using System;
using System.Windows;
using System.Windows.Controls;

namespace BaseControls
{
    public class ChatTextBox :TextBox
    {
        public ChatTextBox()
        {
            SetStyle();
        }

        protected void SetStyle()
        {
            Resources.Source = new Uri("/BaseControls;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            Style = (Style)FindResource("ChatTextBox");
        }
    }
}
