using System.Windows.Controls;
using Backend.Helpers;

namespace BaseControls
{
    public class BaseTabControl : TabControl
    {
        
        #region Fields
        private TabItem settingsPage;
        private TabItem profilePage;
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
                   Items.Add(value);
                else
                    Items.Remove(settingsPage);
                
                SelectedItem = value;
                settingsPage = value;
            } 
        }

        public TabItem ProfilePage
        {
            get
            {
                return profilePage;
            }
            set
            {
                if (value != null)
                    Items.Add(value);
                else
                    Items.Remove(profilePage);
                
                SelectedItem = value;
                profilePage = value;
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
