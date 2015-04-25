using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Backend.Client;
using Backend.Helper;
using Controls.Client;

namespace ClientBase
{
    /// <summary>
    /// Interaction logic for ClientForm.xaml
    /// </summary>
    public partial class ClientWindow
    {
        public ClientWindow()
        {
            InitializeComponent();
        }

        private CallForm callForm;

        private delegate void SetUserList(object sender, EventArgs e);
        private delegate void RecieveMessage(object sender, EventArgs e);
        private delegate void EndConversation();

        public ChatClient ChatClient { get; set; }
        public ClientWindow(ChatClient client)
        {
            InitializeComponent();
            ChatClient = client;
            ChatClient.UserListReceived += chatClient_UserListReceived;
            ChatClient.MessageReceived += chatClient_MessageReceived;
            ChatClient.CallRecieved += ChatClient_CallRecieved;
            ChatClient.CallRequestResponded += ChatClient_CallRequestResponded;

            Title = ChatClient.Init() ? string.Format("{0} connected to {1} ({2})", ChatClient.UserName, ChatClient.ServerName,
                ChatClient.ServerAddress) : "Disconnected";
            SetButtons();
        }

        private void ChatClient_CallRequestResponded(object sender, EventArgs e)
        {
            var command = (Command)sender;
            switch (command)
            {
                case Command.AcceptCall:
                    StartConversation();
                    break;
                default:
                    StopConversation();
                    break;
            }
        }

        private void chatClient_MessageReceived(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                RecieveMessage r = chatClient_MessageReceived;
                Dispatcher.Invoke(r, sender, e);
            }
            else
            {
                var user = (string)sender;
                var args = e as ServerEventArgs;
                if (args == null)
                    return;

                var page = FindTabPage(user) ?? AddTabPage(user);
                var time = DateTime.Now.ToString("HH:mm:ss");

                page.DialogBox.AppendText(string.Format("[{0}] {1}", time, args.Message));
                page.DialogBox.AppendText(Environment.NewLine);
            }

        }

        private ChatTabItem FindTabPage(string name)
        {
            return tbChat.Items.Cast<ChatTabItem>().FirstOrDefault(page => (string) page.Header == name);
        }

        private ChatTabItem AddTabPage(string name)
        {
            var page = new ChatTabItem { Header = name };
            tbChat.Items.Add(page);
            return page;
        }

        private void chatClient_UserListReceived(object sender, EventArgs e)
        {
            var userList = (string)sender;
            var args = e as ServerEventArgs;
            if (args == null)
                return;

            if (!Dispatcher.CheckAccess())
            {
                SetUserList ul = chatClient_UserListReceived;
                Dispatcher.Invoke(ul, sender, e);
            }
            else
            {
                var connStr = string.Format("{0} {1}", args.User, args.Info);
                tbChat.GlobalPage.DialogBox.AppendText(connStr);
                tbChat.GlobalPage.DialogBox.AppendText(Environment.NewLine);

                var userPage = FindTabPage(args.User);
                if (userPage != null)
                {
                    userPage.DialogBox.AppendText(connStr);
                    userPage.DialogBox.AppendText(Environment.NewLine);
                }
                allPanel.Children.Clear();
                var users = userList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var user in users)
                {
                    AddUserButton(user);        
                }
            }

        }

        private void AddUserButton(string userName)
        {
            var userBtn = new Button
            {
                Content = userName,
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            userBtn.MouseDoubleClick += user_DoubleClick;
            allPanel.Children.Add(userBtn);
        }

        private void call_Click(object sender, RoutedEventArgs e)
        {
            var currentTabPage = tbChat.SelectedItem as ChatTabItem;
            if (currentTabPage != null && (string) currentTabPage.Header != ChatHelper.Global)
            {
                var user = (string) currentTabPage.Header;
                callForm = new CallForm(user, FormType.Outcoming);
                ChatClient.SendChatRequest(user);
            }
            callForm.ShowDialog();
            if (callForm.DialogResult == true)
                ChatClient.SendResponse(Command.EndCall);
        }

        private void ChatClient_CallRecieved(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(delegate { ChatClient_CallRecieved(sender, e); });
                return;
            }
            var address = (string)sender;
            var args = e as ServerEventArgs;
            if (args == null)
                return;
            
            callForm = new CallForm(args.Message, FormType.Incoming);
            var dialogResult = callForm.ShowDialog() == true
                ? Command.AcceptCall : Command.CancelCall;
            ChatClient.AnswerIncomingCall(args.Message, address, dialogResult);
            switch (dialogResult)
            {
                case Command.AcceptCall:
                    callForm = new CallForm(args.Message, FormType.Conversation);
                    StartConversation();
                    break;
                case Command.CancelCall:
                    callForm.Close();
                    break;
            }
        }

        private void StartConversation()
        {
            if (!callForm.Dispatcher.CheckAccess())
            {
                callForm.Dispatcher.Invoke(StartConversation);
            }
            else
            {
                callForm.SetControl(FormType.Conversation);
                if (!callForm.IsVisible)
                    callForm.ShowDialog();
                callForm.Closing += callForm_Closed;
            }
        }

        private void StopConversation()
        {
            if (!Dispatcher.CheckAccess())
            {
                EndConversation d = StopConversation;
                Dispatcher.Invoke(d);
            }
            else
            {
                callForm.Close();
            }
        }

        private void callForm_Closed(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ChatClient.EndChat(true);
            callForm.Closing -= callForm_Closed;
        }


        private void sendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        /// <summary>
        /// Sends message to selected user
        /// </summary>
        private void SendMessage()
        {
            var tabItem = tbChat.SelectedItem as ChatTabItem;
            if (tabItem != null)
            {
                var selectedUser = (string) tabItem.Header;
                var message = string.Format("{0}: {1}", ChatClient.UserName, tbMessage.Text);
                ChatClient.SendMessage(message, selectedUser);

                var page = FindTabPage(selectedUser) ?? AddTabPage(selectedUser);
                tbChat.SelectedItem = page;
                var time = DateTime.Now.ToString("HH:mm:ss");

                page.DialogBox.AppendText(string.Format("[{0}] {1}", time, message));
                page.DialogBox.AppendText(Environment.NewLine);
            }
            tbMessage.Clear();
        }

        private void ClientForm_OnClosing(object sender, CancelEventArgs e)
        {
            ChatClient.IsConnected = false;
            ChatClient.CloseConnection();
        }


        private void ClientForm_Load(object sender, RoutedEventArgs e)
        {
            SetButtons();
        }

        private void SetButtons()
        {
            var isSelected = tbChat.SelectedItem != null && !Equals(tbChat.SelectedItem, tbChat.GlobalPage);
            sendMsg.IsEnabled = !String.IsNullOrWhiteSpace(tbMessage.Text) && isSelected;
            callBtn.IsEnabled = isSelected;
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || !sendMsg.IsEnabled)
                return;
            SendMessage();
        }

        private void user_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = e.Source as Button;
            if (item == null)
                return;
            var name = (string)item.Content;
            tbChat.SelectedItem = FindTabPage(name) ?? AddTabPage(name);
        }


        private void tcChat_MouseClick(object sender, MouseEventArgs e)
        {
           // if (e.Button != MouseButtons.Right)
           //     return;
           // closeItem.Enabled = tcChat.SelectedTab != null
           //                     && tcChat.SelectedTab != tcChat.GlobalPage;
           // contextMenu.Show(Cursor.Position);
        }


        private void lbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetButtons();
        }

        private void messageTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetButtons();
        }

        private void tbChat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetButtons();
        }

        private void ClientForm_OnClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CloseTab_OnClick(object sender, RoutedEventArgs e)
        {
            tbChat.Items.Remove(tbChat.SelectedItem);
        }

        private void FrameworkElement_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var tab = tbChat.SelectedItem as ChatTabItem;
            if (tab != null && (string) tab.Header != ChatHelper.Global)
                closeTab.IsEnabled = true;
            else
                closeTab.IsEnabled = false;

        }
    }
}
