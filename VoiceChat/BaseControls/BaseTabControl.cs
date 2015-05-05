using System;
using System.Windows;
using System.Windows.Controls;
using Backend.Helpers;

namespace BaseControls
{
    public class BaseTabControl : TabControl
    {
        
        #region Fields
        private ChatTabItem settingsPage;
        #endregion

        #region Properties
        public ChatTabItem GlobalPage { get; set; }
        public ChatTabItem SettingsPage
        {
            get
            {
                return settingsPage;
            }
            set
            {
                Items.Add(value);
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
