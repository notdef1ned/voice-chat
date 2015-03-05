using System;
using System.Linq;
using System.Windows.Forms;
using Client.Client;

namespace Client.UI
{
    public partial class ClientForm : Form
    {
        private CallForm callForm;

        private delegate void SetUserList(object sender, EventArgs e);
        private delegate void RecieveMessage(object sender, EventArgs e);
        
        public ChatClient ChatClient { get; set; }
        public ClientForm(ChatClient client)
        {
            InitializeComponent();
            ChatClient = client;
            ChatClient.UserListReceived += chatClient_UserListReceived;
            ChatClient.MessageReceived += chatClient_MessageReceived;
            ChatClient.CallRecieved += ChatClient_CallRecieved;
            ChatClient.CallRequestResponded += ChatClient_CallRequestResponded;
            ChatClient.Init();
            Text = string.Format("{0} connected to {1}", ChatClient.UserName, ChatClient.ServerAddress);
            SetButtons();
        }

        private void ChatClient_CallRequestResponded(object sender, EventArgs e)
        {
            var message = (string) sender;
            switch (message)
            {
                case Chat.Chat.Accept:
                    StartConversation();
                break;
                case Chat.Chat.Decline:
                    callForm.Close();
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
                var message = (string)sender;
                var args = e as MessageEventArgs;
                if (args == null)
                    return;

                var page = FindTabPage(args.Message) ?? AddTabPage(args.Message);
                var time = DateTime.Now.ToString("HH:mm:ss");
               
                page.DialogBox.AppendText(string.Format("[{0}] {1}", time, message));
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
            var list = (string) sender;
            var args = e as MessageEventArgs;
            if (args == null)
                return;
            var userInfo = args.Message.Split('|');
            
            if (lbUsers.InvokeRequired)
            {
                SetUserList ul = chatClient_UserListReceived;
                Invoke(ul, list, e);
            }
            else
            {
                lbUsers.Items.Clear();
                var connStr = string.Format("{0} {1}", userInfo[0], userInfo[1]);
                tcChat.GlobalPage.DialogBox.AppendText(connStr);
                tcChat.GlobalPage.DialogBox.AppendText(Environment.NewLine);
                var userPage = FindTabPage(userInfo[0]);
                if (userPage != null)
                {
                    userPage.DialogBox.AppendText(connStr);
                    userPage.DialogBox.AppendText(Environment.NewLine);
                }
                if (list == string.Empty)
                    return;
                var users = list.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
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
            var args = e as MessageEventArgs;
            if (args == null)
                return;

            callForm = new CallForm(args.Message, FormType.Incoming);
            var dialogResult = callForm.ShowDialog() == DialogResult.OK 
                ? Chat.Chat.Accept : Chat.Chat.Decline;
            ChatClient.AnswerIncomingCall(args.Message, address, dialogResult);
            switch (dialogResult)
            {
                case Chat.Chat.Accept:
                    StartConversation();
                    break;
                case Chat.Chat.Decline:
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

        private void callForm_Closed(object sender, EventArgs e)
        {
            ChatClient.EndChat();
            callForm.Closed -= callForm_Closed;
        }


        private void sendMessage_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            var selectedUser = lbUsers.SelectedItem.ToString();
            var message = string.Format("{0}: {1}", ChatClient.UserName, messageTextbox.Text);
            ChatClient.SendMessage(message, selectedUser);
            var page = FindTabPage(selectedUser) ?? AddTabPage(selectedUser);
            tcChat.SelectedTab = page;
            var time = DateTime.Now.ToString("HH:mm:ss");
            page.DialogBox.AppendText(string.Format("[{0}] {1}", time, message));
            page.DialogBox.AppendText(Environment.NewLine);
            messageTextbox.Text = String.Empty;
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
            var isSelected = lbUsers.SelectedItem != null;
            sendMessageButton.Enabled = messageTextbox.Text != String.Empty && isSelected;
            callButton.Enabled = isSelected;
        }

        private void messageTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && sendMessageButton.Enabled)
                SendMessage();
        }


    }
}
