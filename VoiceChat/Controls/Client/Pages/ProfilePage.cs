using System;
using System.Windows;
using System.Windows.Controls;
using Backend.Client;
using Backend.Helpers;
using BaseControls;
using ChatControls.Client.Controls;

namespace ChatControls.Client.Pages
{
    public class ProfilePage : ChatTabItem
    {
        #region Fields
        private readonly ProfileControl profile;
        private readonly ChatClient client;
        private bool isEdited;
        #endregion


        #region Constructors
        public ProfilePage(ChatClient client)
        {
            Header = ChatHelper.PROFILE;
            profile = new ProfileControl
            {
                Margin = new Thickness(0),
                Padding = new Thickness(0)
            };

            this.client = client;
            Content = profile;

            #region Event Subscription
            profile.EmailAddress.TextChanged += EmailAddressOnTextChanged;
            profile.FirstName.TextChanged += FirstNameOnTextChanged;
            profile.LastName.TextChanged += LastNameOnTextChanged;
            profile.UserName.TextChanged += UserNameOnTextChanged;
            #endregion
        }
        #endregion

        #region Properties
        public TextBox UserName 
        {
            get { return profile.UserName; }
        }
        public TextBox FirstName
        {
            get { return profile.FirstName; }
        }
        public TextBox LastName
        {
            get { return profile.LastName; }
        }
        public TextBox EmailAddress
        {
            get { return profile.EmailAddress; }
        }
        #endregion

        #region Methods
        private void SetEdited()
        {
            if (isEdited)
                return;
            isEdited = true;
            Header += "*";
        }

        #endregion

        #region Event Handlers
        private void UserNameOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            SetEdited();
        }

        private void LastNameOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            SetEdited();
        }

        private void FirstNameOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            SetEdited();
        }

        private void EmailAddressOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            SetEdited();
        }

        #endregion
    }
}
