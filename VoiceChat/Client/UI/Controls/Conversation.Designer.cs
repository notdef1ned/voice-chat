namespace Client.UI.Controls
{
    partial class Conversation
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnEndConversation = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnEndConversation
            // 
            this.btnEndConversation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEndConversation.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnEndConversation.Location = new System.Drawing.Point(86, 121);
            this.btnEndConversation.Name = "btnEndConversation";
            this.btnEndConversation.Size = new System.Drawing.Size(229, 42);
            this.btnEndConversation.TabIndex = 0;
            this.btnEndConversation.Text = "End Conversation";
            this.btnEndConversation.UseVisualStyleBackColor = true;
            // 
            // Conversation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnEndConversation);
            this.Name = "Conversation";
            this.Size = new System.Drawing.Size(400, 179);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEndConversation;
    }
}
