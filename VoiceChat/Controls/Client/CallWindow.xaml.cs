using System.Windows;
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
                        StackPanel = new Conversation();
                        Title = string.Format("{0}: {1}", ChatHelper.CONVERSATION, caller);
                        break;
                    case FormType.Incoming:
                        StackPanel = new IncomingCall();
                        Title = string.Format("{0}: {1}", ChatHelper.INCOMING_CALL, caller);
                        break;
                    case FormType.Outcoming:
                        StackPanel = new OutcomingCall();
                        Title = string.Format("{0}: {1}", ChatHelper.OUTCOMING_CALL, caller);
                        break;
                    case FormType.File:
                        StackPanel = new RecieveFile();
                        Title = string.Format("{0}:{1}", ChatHelper.FILE_TRANSFER, caller);
                        break;
                }

                if (StackPanel == null)
                    return;

                StackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                StackPanel.VerticalAlignment = VerticalAlignment.Center;
                SubscribePanel();
                MainGrid.Children.Add(StackPanel);
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
