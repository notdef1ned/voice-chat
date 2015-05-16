using System;
using System.Windows;
using System.Windows.Controls;

namespace BaseControls
{
    public class ChatCheckBox : CheckBox
    {
        public ChatCheckBox()
        {
            SetStyle();
        }

        public void SetStyle()
        {
            Resources.Source = new Uri("/BaseControls;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            Style = (Style)FindResource("ChatCheckBox");
        }
    }
}
