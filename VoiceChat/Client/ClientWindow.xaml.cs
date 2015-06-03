using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Backend.Client;
using Backend.Helpers;
using BaseControls;
using ChatControls.Client;
using ChatControls.Client.Pages;
using Microsoft.Win32;

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

        #region WndProc Hook
        public const int WM_SYSCOMMAND = 0x112;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;
        #endregion


        private CallForm callForm;
        private delegate void SetUserList(object sender, EventArgs e);
        private delegate void RecieveMessage(object sender, EventArgs e);
        private delegate void EndConversation();

        public ChatClient ChatClient { get; set; }
        public string Recipient { get; set; }

        public ClientWindow(ChatClient client)
        {
            InitializeComponent();
            ChatClient = client;
            ChatClient.UserListReceived += chatClient_UserListReceived;
            ChatClient.MessageReceived += chatClient_MessageReceived;
            ChatClient.FileRecieved += ChatClient_FileRecieved;
            ChatClient.CallRecieved += ChatClient_CallRecieved;
            ChatClient.CallRequestResponded += ChatClient_CallRequestResponded;
            tbChat.SelectionChanged +=tbChat_SelectionChanged;
            Loaded += ClientWindow_Loaded;
            KeyDown += tbChat_KeyDown;
            title.Text = ChatClient.Init() ? string.Format("{0} connected to {1} ({2})", ChatClient.UserName, ChatClient.ServerName,
                ChatClient.ServerAddress) : "Disconnected";
            SetButtons();
        }

        private void ChatClient_FileRecieved(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                RecieveMessage r = ChatClient_FileRecieved;
                Dispatcher.Invoke(r, sender, e);
            }
            else
            {
                var args = (FileEventArgs) e;

                var page = FindTabPage(args.From) ?? AddTabPage(args.From);
                var time = DateTime.Now.ToString("HH:mm:ss");

                var window = new CallForm(args.From, FormType.File);
                var result = window.ShowDialog();
                string strResult;
               
                if (result == true)
                {
                    strResult = string.Format(ChatHelper.TRANSFERED,args.FileName);

                    var saveDialog = new SaveFileDialog 
                    {
                        FileName = args.FileName,
                        AddExtension = true,
                        DefaultExt = args.Extenstion,
                        Filter = ChatHelper.FILE_FILTER_ALL
                    };

                    if (saveDialog.ShowDialog(this) == true)
                    {
                        File.WriteAllBytes(saveDialog.FileName, args.File);
                    }
                }
                else
                {
                    strResult = ChatHelper.TRANSFER_CANCELED;  
                }
                page.DialogBox.AppendText(string.Format("[{0}] {1}", time, strResult));
                page.DialogBox.AppendText(Environment.NewLine);
            }
        }

        

        private void ClientWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            if (source != null) source.AddHook(WndProc);
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
                var args = e as ServerEventArgs;
                if (args == null)
                    return;

                var page = FindTabPage((string) sender) ?? AddTabPage((string) sender);
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
            page.DialogBox.Text += ChatHelper.ChatWith(name);
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
                if (!users.Any())
                    allPanel.Children.Add(new TextBlock {Text = ChatHelper.NO_USERS_ONLINE});
                else
                    foreach (var user in users)
                        AddUserButton(user);
            }

        }
        
        private void tbChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) || !Keyboard.IsKeyDown(Key.W) || tbChat.SelectedItem.Equals(tbChat.GlobalPage))
                return;
            CloseTab();
        }

        private void AddUserButton(string userName)
        {
            var userBtn = new Button
            {
                Content = userName,
                BorderBrush = Brushes.Transparent,
                Background = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            userBtn.Click += user_DoubleClick;
            allPanel.Children.Add(userBtn);
        }

        private void call_Click(object sender, RoutedEventArgs e)
        {
            var currentTabPage = tbChat.SelectedItem as ChatTabItem;
            if (currentTabPage != null && (string) currentTabPage.Header != ChatHelper.GLOBAL)
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
            var isUserSelected = tbChat.SelectedItem != null && !IsServiceTab();
            sendMsg.IsEnabled = !string.IsNullOrWhiteSpace(tbMessage.Text) && isUserSelected;
            callBtn.IsEnabled = fileBtn.IsEnabled = isUserSelected;
            tbMessage.Visibility = sendMsg.Visibility = callBtn.Visibility = fileBtn.Visibility = isUserSelected ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool IsServiceTab()
        {
            return Equals(tbChat.SelectedItem, tbChat.GlobalPage)
                   || Equals(tbChat.SelectedItem, tbChat.SettingsPage)
                   || Equals(tbChat.SelectedItem, tbChat.ProfilePage);
        }

        private void tbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || !sendMsg.IsEnabled)
                return;
            SendMessage();
        }

        private void user_DoubleClick(object sender, RoutedEventArgs e)
        {
            var item = e.Source as Button;
            if (item == null)
                return;
            var name = (string)item.Content;
            tbChat.SelectedItem = FindTabPage(name) ?? AddTabPage(name);
        }


        private void messageTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetButtons();
        }

        private void tbChat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tabItem = (TabItem) tbChat.SelectedItem;
            Recipient = tabItem != null ? (string) tabItem.Header : null;
            SetButtons();
        }

        private void ClientForm_OnClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CloseTab_OnClick(object sender, RoutedEventArgs e)
        {
            CloseTab();
        }


        private void CloseTab()
        {
            var selected = tbChat.SelectedItem;
            tbChat.Items.Remove(selected);

            if (ReferenceEquals(selected, tbChat.SettingsPage))
                tbChat.SettingsPage = null;

            if (ReferenceEquals(selected, tbChat.ProfilePage))
                tbChat.ProfilePage = null;

            tbChat.SelectedItem = tbChat.GlobalPage;
        }

        private void FrameworkElement_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var tab = tbChat.SelectedItem as ChatTabItem;
            if (tab != null && (string) tab.Header != ChatHelper.GLOBAL)
                closeTab.IsEnabled = true;
            else
                closeTab.IsEnabled = false;
        }

        
        #region Command Bindings
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void CommandBinding_Executed_Maximize(object sender, ExecutedRoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                SystemCommands.RestoreWindow(this);
                Maximize.Content = Resources["Maximize"];
            }
            else
            {
                SystemCommands.MaximizeWindow(this);
                Maximize.Content = Resources["Restore"];
            }
        }

        #region WndProc Hook
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WM_SYSCOMMAND) 
                return IntPtr.Zero;
            switch (wParam.ToInt32())
            {
                case SC_MAXIMIZE:
                    Maximize.Content = Resources["Restore"];
                    break;
                case SC_SIZE:
                    Maximize.Content = Resources["Maximize"];
                    break;
            }
            return IntPtr.Zero;
        }
        #endregion

        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }
        #endregion

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            if (tbChat.SettingsPage == null)
                tbChat.SettingsPage = new SettingsPage(ChatClient);
            else
                tbChat.SelectedItem = tbChat.SettingsPage;
        }

        private void ProfileClick(object sender, RoutedEventArgs e)
        {
            if (tbChat.ProfilePage == null)
               tbChat.ProfilePage = new ProfilePage(ChatClient);
            else
               tbChat.SelectedItem = tbChat.ProfilePage;
            SetButtons();
        }

        private void FileTransferClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog {Multiselect = false};
            var userClickOk = openFileDialog.ShowDialog();
            if (userClickOk != true) 
                return;
            var file = File.ReadAllBytes(openFileDialog.FileName);
            ChatClient.SendFile(file,Recipient,openFileDialog.SafeFileName);
        }
    }
}
