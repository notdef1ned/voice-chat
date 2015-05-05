using System;
using System.Windows;
using System.Windows.Controls;

namespace BaseControls
{
    public class SettingsTabControl : TabControl
    {
        public SettingsTabControl()
        {
            SetStyle();
        }

        public void SetStyle()
        {
           //Resources.Source = new Uri("/BaseControls;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
           //Style = (Style)FindResource("TabControlStyle");
        }

    }
}
