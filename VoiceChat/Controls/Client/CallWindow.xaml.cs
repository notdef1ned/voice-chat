using System.Windows;
using System.Windows.Controls;
using Backend.Helpers;
using BaseControls;
using ChatControls.Client.Controls;

namespace ChatControls.Client
{
    public partial class CallForm
    {
        private readonly string caller;

        private delegate void ChangeControl(FormType type);
        public ChatStackPanel StackPanel { get; set; }

        public CallForm(string caller, FormType type)
        {
            this.caller = caller;
            InitializeComponent();
            SetControl(type);
        }

        public string Text
        {
            get { return Title; }
            set { Title = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public void SetControl(FormType type)
        {
            if (StackPanel != null && !Dispatcher.CheckAccess())
            {
                ChangeControl d = SetControl;
                Dispatcher.Invoke(d, type);
            }
            else
            {
                switch (type)
                {
                    case FormType.Conversation:
                        MainGrid.Children.Remove(StackPanel);
                        StackPanel = new Conversation();
                        Info.Text = string.Format("{0}: {1}", ChatHelper.CONVERSATION, caller);
                        break;
                    case FormType.Incoming:
                        StackPanel = new IncomingCall();
                        Info.Text = string.Format("{0}: {1}", ChatHelper.INCOMING_CALL, caller);
                        break;
                    case FormType.Outcoming:
                        StackPanel = new OutcomingCall();
                        Info.Text = string.Format("{0}: {1}", ChatHelper.OUTCOMING_CALL, caller);
                        break;
                    case FormType.File:
                        StackPanel = new RecieveFile();
                        Info.Text = string.Format(ChatHelper.FILE_TRANSFER, caller);
                        break;
                }

                if (StackPanel == null)
                    return;
                
                SubscribePanel();
                SetControl();
            }
        }

        /// <summary>
        /// Subscribe controls
        /// </summary>
        private void SubscribePanel()
        {
            if (StackPanel.AcceptButton != null)
                StackPanel.AcceptButton.Click += AcceptButton_Click;
            if (StackPanel.CancelButton != null)
                StackPanel.CancelButton.Click += CancelButton_Click;
        }

        /// <summary>
        /// Sets window control
        /// </summary>
        private void SetControl()
        {
            StackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            StackPanel.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(StackPanel, 1);
            Grid.SetColumn(StackPanel, 0);
            Grid.SetColumnSpan(StackPanel, 2);
            MainGrid.Children.Add(StackPanel);
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }

    public enum FormType
    {
        Conversation,
        Incoming,
        Outcoming,
        File
    }
}
