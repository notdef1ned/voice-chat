using System;
using System.Linq;
using System.Windows.Forms;
using ChatLibrary.Helper;
using Client.Client;
using Client.Core;

namespace Client.UI
{
    public partial class ClientForm : Form
    {
        public const string Conversation =
            "Double click on user to start conversation";
        private const string NoUsersOnline = "There is no users online now";


        private CallForm callForm;

        private delegate void SetUserList(object sender, EventArgs e);
        private delegate void RecieveMessage(object sender, EventArgs e);

        private delegate void EndConversation();
        
        public ChatClient ChatClient { get; set; }
        public ClientForm(ChatClient client)
        {
            InitializeComponent();
            ChatClient = client;
            ChatClient.UserListReceived += chatClient_UserListReceived;
            ChatClient.MessageReceived += chatClient_MessageReceived;
            ChatClient.CallRecieved += ChatClient_CallRecieved;
            ChatClient.CallRequestResponded += ChatClient_CallRequestResponded;

            Text = ChatClient.Init() ? string.Format("{0} connected to {1} ({2})", ChatClient.UserName, ChatClient.ServerName,
                ChatClient.ServerAddress) : "Disconnected";
            SetButtons();
        }

        private void ChatClient_CallRequestResponded(object sender, EventArgs e)
        {
            var command = (Command) sender;
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

       
        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void chatClient_MessageReceived(object sender, EventArgs e)
        {
            if (tcChat.InvokeRequired)
            {
                RecieveMessage r = chatClient_MessageReceived;
                Invoke(r, sender, e);
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

        private ChatTabPage FindTabPage(string name)
        {
            return tcChat.TabPages.Cast<ChatTabPage>().FirstOrDefault(page => page.Text == name);
        }

        private ChatTabPage AddTabPage(string name)
        {
            var page = new ChatTabPage { Text = name };
            tcChat.TabPages.Add(page);
            return page;
        }

        private void chatClient_UserListReceived(object sender, EventArgs e)
        {
            var userList = (string) sender;
            var args = e as ServerEventArgs;
            if (args == null)
                return;
            
            if (lbUsers.InvokeRequired)
            {
                SetUserList ul = chatClient_UserListReceived;
                Invoke(ul, sender, e);
            }
            else
            {
                lbUsers.Items.Clear();
                var connStr = string.Format("{0} {1}", args.User, args.Info);
                tcChat.GlobalPage.DialogBox.AppendText(connStr);
                tcChat.GlobalPage.DialogBox.AppendText(Environment.NewLine);
                
                var userPage = FindTabPage(args.User);
                if (userPage != null)
                {
                    userPage.DialogBox.AppendText(connStr);
                    userPage.DialogBox.AppendText(Environment.NewLine);
                }
                if (userList == string.Empty)
                    return;
                var users = userList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var user in users)
                    lbUsers.Items.Add(user);
             }
            
        }

        private void call_Click(object sender, EventArgs e)
        {
            var user = lbUsers.SelectedItem.ToString();
            callForm = new CallForm(user,FormType.Outcoming);
            ChatClient.SendChatRequest(user);
            callForm.ShowDialog();
        }

        private void ChatClient_CallRecieved(object sender, EventArgs e)
        {
            var address = (string) sender;
            var args = e as ServerEventArgs;
            if (args == null)
                return;

            callForm = new CallForm(args.Message, FormType.Incoming);
            var dialogResult = callForm.ShowDialog() == DialogResult.OK 
                ? Command.AcceptCall : Command.CancelCall;
            ChatClient.AnswerIncomingCall(args.Message, address, dialogResult);
            
            switch (dialogResult)
            {
                case Command.AcceptCall:
                    StartConversation();
                    break;
                case Command.CancelCall:
                    callForm.Close();
                    break;
            }
        }

        private void StartConversation()
        {
            callForm.SetControl(FormType.Conversation);
            if (!callForm.Visible)
                callForm.ShowDialog();
            callForm.Closed += callForm_Closed;
        }

        private void StopConversation()
        {
            if (callForm.InvokeRequired)
            {
                EndConversation d = StopConversation;
                Invoke(d);
            }
            else
            {
                callForm.Close();
            }
        }

        private void callForm_Closed(object sender, EventArgs e)
        {
            ChatClient.EndChat(true);
            callForm.Closed -= callForm_Closed;
        }


        private void sendMessage_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            var selectedUser = tcChat.SelectedTab.Text;
            var message = string.Format("{0}: {1}", ChatClient.UserName, messageTextbox.Text);
            ChatClient.SendMessage(message, selectedUser);
            
            var page = FindTabPage(selectedUser) ?? AddTabPage(selectedUser);
            tcChat.SelectedTab = page;
            var time = DateTime.Now.ToString("HH:mm:ss");
        
            page.DialogBox.AppendText(string.Format("[{0}] {1}", time, message));
            page.DialogBox.AppendText(Environment.NewLine);
            messageTextbox.Clear();
        }

        private void lbUsers_SelectedValueChanged(object sender, EventArgs e)
        {
            SetButtons();
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ChatClient.IsConnected = false;
            ChatClient.CloseConnection();
        }

        private void messageTextbox_TextChanged(object sender, EventArgs e)
        {
            SetButtons();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            SetButtons();
        }

        private void SetButtons()
        {
            var isSelected = tcChat.SelectedTab != null && tcChat.SelectedTab != tcChat.GlobalPage;
            sendMessageButton.Enabled = !String.IsNullOrWhiteSpace(messageTextbox.Text) && isSelected;
            callButton.Enabled = isSelected;
        }

        private void messageTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || !sendMessageButton.Enabled) 
                return;
            SendMessage();
        }

        private void lbUsers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var index = lbUsers.IndexFromPoint(e.Location);
            if (index == ListBox.NoMatches) 
                return;
            var name = lbUsers.SelectedItem.ToString();
            tcChat.SelectedTab = FindTabPage(name) ?? AddTabPage(name);
        }

        private void tcChat_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtons();
        }

        private void lbUsers_MouseMove(object sender, MouseEventArgs e)
        {
            var str = toolTip.GetToolTip(lbUsers);
            if (lbUsers.Items.Count > 0)
            {
                if (str != Conversation)
                    toolTip.SetToolTip(lbUsers, Conversation);
            }
            else
            {
                if (str != NoUsersOnline)
                    toolTip.SetToolTip(lbUsers, NoUsersOnline);
            }
        }

        private void tcChat_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            closeItem.Enabled = tcChat.SelectedTab != null 
                                && tcChat.SelectedTab != tcChat.GlobalPage;
            contextMenu.Show(Cursor.Position);
        }

        private void closeItem_Click(object sender, EventArgs e)
        {
             tcChat.TabPages.Remove(tcChat.SelectedTab);
        }
    }
}
