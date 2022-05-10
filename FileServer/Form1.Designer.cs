namespace FileServer
{
    partial class server
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.ServerIp = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ServerPort = new System.Windows.Forms.MaskedTextBox();
            this.startServer = new System.Windows.Forms.Button();
            this.connClient = new System.Windows.Forms.ComboBox();
            this.recvShow = new System.Windows.Forms.TextBox();
            this.sendEdit = new System.Windows.Forms.TextBox();
            this.sendPath = new System.Windows.Forms.TextBox();
            this.sendPathBtn = new System.Windows.Forms.Button();
            this.sendBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.inputHint = new System.Windows.Forms.Label();
            this.recvPath = new System.Windows.Forms.TextBox();
            this.recvPathBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.getSelfIpBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP:";
            // 
            // ServerIp
            // 
            this.ServerIp.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ServerIp.Location = new System.Drawing.Point(34, 9);
            this.ServerIp.Mask = "999.999.999.999";
            this.ServerIp.Name = "ServerIp";
            this.ServerIp.Size = new System.Drawing.Size(129, 23);
            this.ServerIp.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(270, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "端口号:";
            // 
            // ServerPort
            // 
            this.ServerPort.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ServerPort.Location = new System.Drawing.Point(319, 9);
            this.ServerPort.Name = "ServerPort";
            this.ServerPort.Size = new System.Drawing.Size(70, 23);
            this.ServerPort.TabIndex = 3;
            // 
            // startServer
            // 
            this.startServer.Location = new System.Drawing.Point(403, 8);
            this.startServer.Name = "startServer";
            this.startServer.Size = new System.Drawing.Size(89, 23);
            this.startServer.TabIndex = 4;
            this.startServer.Text = "开始监听";
            this.startServer.UseVisualStyleBackColor = true;
            this.startServer.Click += new System.EventHandler(this.startServer_Click);
            // 
            // connClient
            // 
            this.connClient.FormattingEnabled = true;
            this.connClient.Location = new System.Drawing.Point(319, 42);
            this.connClient.Name = "connClient";
            this.connClient.Size = new System.Drawing.Size(173, 20);
            this.connClient.TabIndex = 6;
            // 
            // recvShow
            // 
            this.recvShow.Location = new System.Drawing.Point(12, 68);
            this.recvShow.Multiline = true;
            this.recvShow.Name = "recvShow";
            this.recvShow.ReadOnly = true;
            this.recvShow.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.recvShow.Size = new System.Drawing.Size(480, 129);
            this.recvShow.TabIndex = 7;
            this.recvShow.TextChanged += new System.EventHandler(this.recvShow_TextChanged);
            // 
            // sendEdit
            // 
            this.sendEdit.Location = new System.Drawing.Point(12, 205);
            this.sendEdit.Multiline = true;
            this.sendEdit.Name = "sendEdit";
            this.sendEdit.Size = new System.Drawing.Size(441, 68);
            this.sendEdit.TabIndex = 7;
            this.sendEdit.Enter += new System.EventHandler(this.sendEdit_Enter);
            this.sendEdit.Leave += new System.EventHandler(this.sendEdit_Leave);
            // 
            // sendPath
            // 
            this.sendPath.Location = new System.Drawing.Point(12, 296);
            this.sendPath.Name = "sendPath";
            this.sendPath.Size = new System.Drawing.Size(441, 21);
            this.sendPath.TabIndex = 8;
            // 
            // sendPathBtn
            // 
            this.sendPathBtn.Location = new System.Drawing.Point(459, 294);
            this.sendPathBtn.Name = "sendPathBtn";
            this.sendPathBtn.Size = new System.Drawing.Size(33, 23);
            this.sendPathBtn.TabIndex = 9;
            this.sendPathBtn.Text = "...";
            this.sendPathBtn.UseVisualStyleBackColor = true;
            this.sendPathBtn.Click += new System.EventHandler(this.sendPathBtn_Click);
            // 
            // sendBtn
            // 
            this.sendBtn.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.sendBtn.Location = new System.Drawing.Point(459, 203);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(33, 70);
            this.sendBtn.TabIndex = 12;
            this.sendBtn.Text = "发\r\n送";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "运行和接收信息显示:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(234, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "已连接客户端:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 204);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 12);
            this.label6.TabIndex = 15;
            // 
            // inputHint
            // 
            this.inputHint.AutoSize = true;
            this.inputHint.BackColor = System.Drawing.Color.White;
            this.inputHint.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.inputHint.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.inputHint.Location = new System.Drawing.Point(22, 215);
            this.inputHint.Name = "inputHint";
            this.inputHint.Size = new System.Drawing.Size(192, 16);
            this.inputHint.TabIndex = 16;
            this.inputHint.Text = "请输入需要发送的内容...";
            this.inputHint.Click += new System.EventHandler(this.inputHint_Click);
            // 
            // recvPath
            // 
            this.recvPath.Location = new System.Drawing.Point(12, 341);
            this.recvPath.Name = "recvPath";
            this.recvPath.Size = new System.Drawing.Size(441, 21);
            this.recvPath.TabIndex = 8;
            // 
            // recvPathBtn
            // 
            this.recvPathBtn.Location = new System.Drawing.Point(459, 339);
            this.recvPathBtn.Name = "recvPathBtn";
            this.recvPathBtn.Size = new System.Drawing.Size(33, 23);
            this.recvPathBtn.TabIndex = 9;
            this.recvPathBtn.Text = "...";
            this.recvPathBtn.UseVisualStyleBackColor = true;
            this.recvPathBtn.Click += new System.EventHandler(this.recvPathBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 278);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 17;
            this.label3.Text = "发送路径:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 326);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 17;
            this.label8.Text = "接收路径:";
            // 
            // getSelfIpBtn
            // 
            this.getSelfIpBtn.Location = new System.Drawing.Point(163, 9);
            this.getSelfIpBtn.Name = "getSelfIpBtn";
            this.getSelfIpBtn.Size = new System.Drawing.Size(75, 23);
            this.getSelfIpBtn.TabIndex = 18;
            this.getSelfIpBtn.Text = "获取本机ip";
            this.getSelfIpBtn.UseVisualStyleBackColor = true;
            this.getSelfIpBtn.Click += new System.EventHandler(this.getSelfIpBtn_Click);
            // 
            // server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 374);
            this.Controls.Add(this.getSelfIpBtn);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.inputHint);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sendBtn);
            this.Controls.Add(this.recvPathBtn);
            this.Controls.Add(this.sendPathBtn);
            this.Controls.Add(this.recvPath);
            this.Controls.Add(this.sendPath);
            this.Controls.Add(this.sendEdit);
            this.Controls.Add(this.recvShow);
            this.Controls.Add(this.connClient);
            this.Controls.Add(this.startServer);
            this.Controls.Add(this.ServerPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ServerIp);
            this.Controls.Add(this.label1);
            this.Name = "server";
            this.Text = "服务端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox ServerIp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox ServerPort;
        private System.Windows.Forms.Button startServer;
        private System.Windows.Forms.ComboBox connClient;
        private System.Windows.Forms.TextBox recvShow;
        private System.Windows.Forms.TextBox sendEdit;
        private System.Windows.Forms.TextBox sendPath;
        private System.Windows.Forms.Button sendPathBtn;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label inputHint;
        private System.Windows.Forms.TextBox recvPath;
        private System.Windows.Forms.Button recvPathBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button getSelfIpBtn;
    }
}

