namespace Client
{
    partial class ClientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.sendMessageButton = new System.Windows.Forms.Button();
            this.messageTextbox = new System.Windows.Forms.TextBox();
            this.callButton = new System.Windows.Forms.Button();
            this.tbDialog = new System.Windows.Forms.TextBox();
            this.lbUsers = new System.Windows.Forms.ListBox();
            this.tcChat = new System.Windows.Forms.TabControl();
            this.tpGlobal = new ChatTabPage();
            this.tcChat.SuspendLayout();
            this.tpGlobal.SuspendLayout();
            this.SuspendLayout();
            // 
            // sendMessageButton
            // 
            this.sendMessageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendMessageButton.Location = new System.Drawing.Point(793, 538);
            this.sendMessageButton.Name = "sendMessageButton";
            this.sendMessageButton.Size = new System.Drawing.Size(134, 69);
            this.sendMessageButton.TabIndex = 2;
            this.sendMessageButton.Text = "Send message";
            this.sendMessageButton.UseVisualStyleBackColor = true;
            this.sendMessageButton.Click += new System.EventHandler(this.sendMessage_Click);
            // 
            // messageTextbox
            // 
            this.messageTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.messageTextbox.Location = new System.Drawing.Point(18, 540);
            this.messageTextbox.Multiline = true;
            this.messageTextbox.Name = "messageTextbox";
            this.messageTextbox.Size = new System.Drawing.Size(651, 69);
            this.messageTextbox.TabIndex = 3;
            this.messageTextbox.TextChanged += new System.EventHandler(this.messageTextbox_TextChanged);
            this.messageTextbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.messageTextbox_KeyDown);
            // 
            // callButton
            // 
            this.callButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.callButton.Location = new System.Drawing.Point(675, 538);
            this.callButton.Name = "callButton";
            this.callButton.Size = new System.Drawing.Size(112, 69);
            this.callButton.TabIndex = 4;
            this.callButton.Text = "Call";
            this.callButton.UseVisualStyleBackColor = true;
            this.callButton.Click += new System.EventHandler(this.call_Click);
            // 
            // tbDialog
            // 
            this.tbDialog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbDialog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbDialog.Location = new System.Drawing.Point(3, 3);
            this.tbDialog.Multiline = true;
            this.tbDialog.Name = "tbDialog";
            this.tbDialog.Size = new System.Drawing.Size(685, 481);
            this.tbDialog.TabIndex = 6;
            // 
            // lbUsers
            // 
            this.lbUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbUsers.BackColor = System.Drawing.SystemColors.Window;
            this.lbUsers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbUsers.FormattingEnabled = true;
            this.lbUsers.ItemHeight = 25;
            this.lbUsers.Location = new System.Drawing.Point(18, 7);
            this.lbUsers.Name = "lbUsers";
            this.lbUsers.Size = new System.Drawing.Size(195, 527);
            this.lbUsers.TabIndex = 7;
            this.lbUsers.SelectedValueChanged += new System.EventHandler(this.lbUsers_SelectedValueChanged);
            // 
            // tcChat
            // 
            this.tcChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcChat.Controls.Add(this.tpGlobal);
            this.tcChat.Location = new System.Drawing.Point(228, 7);
            this.tcChat.Name = "tcChat";
            this.tcChat.SelectedIndex = 0;
            this.tcChat.Size = new System.Drawing.Size(699, 525);
            this.tcChat.TabIndex = 8;
            // 
            // tpGlobal
            // 
            this.tpGlobal.Controls.Add(this.tbDialog);
            this.tpGlobal.Location = new System.Drawing.Point(4, 34);
            this.tpGlobal.Name = "tpGlobal";
            this.tpGlobal.Padding = new System.Windows.Forms.Padding(3);
            this.tpGlobal.Size = new System.Drawing.Size(691, 487);
            this.tpGlobal.TabIndex = 0;
            this.tpGlobal.Text = "Global";
            this.tpGlobal.UseVisualStyleBackColor = true;
            // 
            // ClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(958, 619);
            this.Controls.Add(this.tcChat);
            this.Controls.Add(this.lbUsers);
            this.Controls.Add(this.callButton);
            this.Controls.Add(this.messageTextbox);
            this.Controls.Add(this.sendMessageButton);
            this.Name = "ClientForm";
            this.ShowIcon = false;
            this.Text = "Voice Chat Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientForm_FormClosing);
            this.Load += new System.EventHandler(this.ClientForm_Load);
            this.tcChat.ResumeLayout(false);
            this.tpGlobal.ResumeLayout(false);
            this.tpGlobal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendMessageButton;
        private System.Windows.Forms.TextBox messageTextbox;
        private System.Windows.Forms.Button callButton;
        private System.Windows.Forms.TextBox tbDialog;
        private System.Windows.Forms.ListBox lbUsers;
        private System.Windows.Forms.TabControl tcChat;
        private ChatTabPage tpGlobal;
    }
}

