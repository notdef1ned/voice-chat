namespace Server.UI
{
    partial class ServerForm
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
            this.connectionInfo = new System.Windows.Forms.GroupBox();
            this.lblName = new System.Windows.Forms.Label();
            this.tbServerName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbInterfaces = new System.Windows.Forms.ComboBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.cbStartStop = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gbConnectedClients = new System.Windows.Forms.GroupBox();
            this.connectedClients = new System.Windows.Forms.ListBox();
            this.connectionInfo.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gbConnectedClients.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectionInfo
            // 
            this.connectionInfo.Controls.Add(this.lblName);
            this.connectionInfo.Controls.Add(this.tbServerName);
            this.connectionInfo.Controls.Add(this.label2);
            this.connectionInfo.Controls.Add(this.label1);
            this.connectionInfo.Controls.Add(this.cbInterfaces);
            this.connectionInfo.Controls.Add(this.tbPort);
            this.connectionInfo.Controls.Add(this.cbStartStop);
            this.connectionInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectionInfo.Location = new System.Drawing.Point(3, 3);
            this.connectionInfo.Name = "connectionInfo";
            this.connectionInfo.Size = new System.Drawing.Size(348, 423);
            this.connectionInfo.TabIndex = 1;
            this.connectionInfo.TabStop = false;
            this.connectionInfo.Text = "Connection Information";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 99);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(140, 25);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "Server name:";
            // 
            // tbServerName
            // 
            this.tbServerName.Location = new System.Drawing.Point(16, 134);
            this.tbServerName.Name = "tbServerName";
            this.tbServerName.Size = new System.Drawing.Size(265, 31);
            this.tbServerName.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 275);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Network Interface:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 189);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Port Number:";
            // 
            // cbInterfaces
            // 
            this.cbInterfaces.FormattingEnabled = true;
            this.cbInterfaces.Location = new System.Drawing.Point(16, 303);
            this.cbInterfaces.Name = "cbInterfaces";
            this.cbInterfaces.Size = new System.Drawing.Size(265, 33);
            this.cbInterfaces.TabIndex = 2;
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(17, 217);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(121, 31);
            this.tbPort.TabIndex = 1;
            // 
            // cbStartStop
            // 
            this.cbStartStop.AutoSize = true;
            this.cbStartStop.Location = new System.Drawing.Point(19, 49);
            this.cbStartStop.Name = "cbStartStop";
            this.cbStartStop.Size = new System.Drawing.Size(208, 29);
            this.cbStartStop.TabIndex = 0;
            this.cbStartStop.Text = "Start/Stop Server";
            this.cbStartStop.UseVisualStyleBackColor = true;
            this.cbStartStop.CheckedChanged += new System.EventHandler(this.ckbServerControl_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.gbConnectedClients, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.connectionInfo, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(708, 429);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // gbConnectedClients
            // 
            this.gbConnectedClients.Controls.Add(this.connectedClients);
            this.gbConnectedClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbConnectedClients.Location = new System.Drawing.Point(357, 3);
            this.gbConnectedClients.Name = "gbConnectedClients";
            this.gbConnectedClients.Size = new System.Drawing.Size(348, 423);
            this.gbConnectedClients.TabIndex = 5;
            this.gbConnectedClients.TabStop = false;
            this.gbConnectedClients.Text = "Connected Clients";
            // 
            // connectedClients
            // 
            this.connectedClients.BackColor = System.Drawing.SystemColors.Menu;
            this.connectedClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.connectedClients.FormattingEnabled = true;
            this.connectedClients.ItemHeight = 25;
            this.connectedClients.Location = new System.Drawing.Point(6, 30);
            this.connectedClients.Name = "connectedClients";
            this.connectedClients.Size = new System.Drawing.Size(333, 375);
            this.connectedClients.TabIndex = 1;
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 429);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ServerForm";
            this.Text = "Voice Chat Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ServerForm_FormClosing);
            this.connectionInfo.ResumeLayout(false);
            this.connectionInfo.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.gbConnectedClients.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox connectionInfo;
        private System.Windows.Forms.ComboBox cbInterfaces;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.CheckBox cbStartStop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gbConnectedClients;
        private System.Windows.Forms.ListBox connectedClients;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox tbServerName;
    }
}

