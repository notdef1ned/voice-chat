using System;
using System.Windows;
using System.Windows.Controls;

namespace BaseControls
{
    public class ChatButton : Button
    {

        public ChatButton()
        {
            MaxHeight = MinHeight = 25;
            SetTabStyle();
        }

        protected void SetTabStyle()
        {
            Resources.Source = new Uri("/BaseControls;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
            Style = (Style)FindResource("ChatButtonStyle");
        }
    }


}
