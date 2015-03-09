using System;
using System.Windows.Forms;
using ChatLibrary.Helper;
using Client.UI.Controls;

namespace Client.UI
{
    public partial class CallForm : Form
    {
        private readonly string caller;

        private delegate void ChangeControl(FormType type);
        public Control Control { get; set; }

        public CallForm(string caller,FormType type)
        {
            this.caller = caller;
            InitializeComponent();
            SetControl(type);
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set 
            { 
                base.Text = value; 
            }
        }


        public void SetControl(FormType type)
        {
            if (Control!= null && Control.InvokeRequired)
            {
                ChangeControl d = SetControl;
                Invoke(d, type);
            }
            else
            {
                switch (type)
                {
                    case FormType.Conversation:
                        Control = new Conversation();
                        Text = string.Format("{0}: {1}", ChatHelper.Conversation, caller);
                        break;
                    case FormType.Incoming:
                        Control = new IncomingCall();
                        Text = string.Format("{0}: {1}", ChatHelper.IncomingCall, caller);
                        break;
                    case FormType.Outcoming:
                        Control = new OutcomingCall();
                        Text = string.Format("{0}: {1}", ChatHelper.OutcomingCall, caller);
                        break;
                }
                if (Control == null) 
                    return;
                Control.Dock = DockStyle.Fill;
                Controls.Clear();
                Controls.Add(Control);
            }
            
        }

    }

    public enum FormType
    {
        Conversation,
        Incoming,
        Outcoming
    }
}
