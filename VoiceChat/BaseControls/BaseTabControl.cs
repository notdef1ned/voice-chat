using System.Windows.Controls;
using System.Windows.Input;
using Backend.Helpers;

namespace BaseControls
{
    public class BaseTabControl : TabControl
    {
        
        #region Fields
        private TabItem settingsPage;
        #endregion

        #region Properties
        public ChatTabItem GlobalPage { get; set; }
        public TabItem SettingsPage
        {
            get
            {
                return settingsPage;
            }
            set
            {
                if (value != null)
                {
                    Items.Add(value);
                }
                else
                {
                    Items.Remove(settingsPage);
                }
                SelectedItem = value;
                settingsPage = value;
            } 
        }
        #endregion

        public BaseTabControl()
        {
            GlobalPage = new ChatTabItem {Header = ChatHelper.GLOBAL};
            GlobalPage.DialogBox.Text += ChatHelper.WelcomeMessage;
            Items.Add(GlobalPage);
        }




        
    }
}
