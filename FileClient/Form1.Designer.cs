namespace FileClient
{
    partial class client
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
            this.ClientIp = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ClientPort = new System.Windows.Forms.MaskedTextBox();
            this.ClientStart = new System.Windows.Forms.Button();
            this.ClientSpot = new System.Windows.Forms.Button();
            this.recvShow = new System.Windows.Forms.TextBox();
            this.sendEdit = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.inputHint = new System.Windows.Forms.Label();
            this.getFileBtn = new System.Windows.Forms.Button();
            this.sendBtn = new System.Windows.Forms.Button();
            this.getPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.getPathBtn = new System.Windows.Forms.Button();
            this.setFileBtn = new System.Windows.Forms.Button();
            this.setPath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.setPathBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ClientIp
            // 
            this.ClientIp.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ClientIp.Location = new System.Drawing.Point(62, 9);
            this.ClientIp.Mask = "999.999.999.999";
            this.ClientIp.Name = "ClientIp";
            this.ClientIp.Size = new System.Drawing.Size(113, 23);
            this.ClientIp.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "连接IP:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(180, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "连接端口号:";
            // 
            // ClientPort
            // 
            this.ClientPort.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ClientPort.Location = new System.Drawing.Point(251, 10);
            this.ClientPort.Name = "ClientPort";
            this.ClientPort.Size = new System.Drawing.Size(60, 23);
            this.ClientPort.TabIndex = 3;
            // 
            // ClientStart
            // 
            this.ClientStart.Location = new System.Drawing.Point(350, 10);
            this.ClientStart.Name = "ClientStart";
            this.ClientStart.Size = new System.Drawing.Size(66, 23);
            this.ClientStart.TabIndex = 4;
            this.ClientStart.Text = "连 接";
            this.ClientStart.UseVisualStyleBackColor = true;
            this.ClientStart.Click += new System.EventHandler(this.ClientStart_Click);
            // 
            // ClientSpot
            // 
            this.ClientSpot.Location = new System.Drawing.Point(426, 10);
            this.ClientSpot.Name = "ClientSpot";
            this.ClientSpot.Size = new System.Drawing.Size(66, 23);
            this.ClientSpot.TabIndex = 4;
            this.ClientSpot.Text = "断 开";
            this.ClientSpot.UseVisualStyleBackColor = true;
            this.ClientSpot.Click += new System.EventHandler(this.ClientSpot_Click);
            // 
            // recvShow
            // 
            this.recvShow.Location = new System.Drawing.Point(12, 59);
            this.recvShow.Multiline = true;
            this.recvShow.Name = "recvShow";
            this.recvShow.ReadOnly = true;
            this.recvShow.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.recvShow.Size = new System.Drawing.Size(480, 115);
            this.recvShow.TabIndex = 5;
            this.recvShow.TabStop = false;
            this.recvShow.TextChanged += new System.EventHandler(this.recvShow_TextChanged);
            // 
            // sendEdit
            // 
            this.sendEdit.Location = new System.Drawing.Point(12, 182);
            this.sendEdit.Multiline = true;
            this.sendEdit.Name = "sendEdit";
            this.sendEdit.Size = new System.Drawing.Size(437, 70);
            this.sendEdit.TabIndex = 6;
            this.sendEdit.Enter += new System.EventHandler(this.sendEdit_Enter);
            this.sendEdit.Leave += new System.EventHandler(this.sendEdit_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "运行和接收信息显示:";
            // 
            // inputHint
            // 
            this.inputHint.AutoSize = true;
            this.inputHint.BackColor = System.Drawing.Color.White;
            this.inputHint.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.inputHint.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.inputHint.Location = new System.Drawing.Point(24, 193);
            this.inputHint.Name = "inputHint";
            this.inputHint.Size = new System.Drawing.Size(192, 16);
            this.inputHint.TabIndex = 8;
            this.inputHint.Text = "请输入需要发送的内容...";
            this.inputHint.Click += new System.EventHandler(this.inputHint_Click);
            // 
            // getFileBtn
            // 
            this.getFileBtn.Location = new System.Drawing.Point(419, 276);
            this.getFileBtn.Name = "getFileBtn";
            this.getFileBtn.Size = new System.Drawing.Size(73, 23);
            this.getFileBtn.TabIndex = 9;
            this.getFileBtn.Text = "获取文件";
            this.getFileBtn.UseVisualStyleBackColor = true;
            this.getFileBtn.Click += new System.EventHandler(this.getFileBtn_Click);
            // 
            // sendBtn
            // 
            this.sendBtn.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.sendBtn.Location = new System.Drawing.Point(455, 182);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(37, 70);
            this.sendBtn.TabIndex = 9;
            this.sendBtn.Text = "发送消息";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // getPath
            // 
            this.getPath.Location = new System.Drawing.Point(12, 277);
            this.getPath.Name = "getPath";
            this.getPath.Size = new System.Drawing.Size(369, 21);
            this.getPath.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 262);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "接收目录:";
            // 
            // getPathBtn
            // 
            this.getPathBtn.Location = new System.Drawing.Point(380, 276);
            this.getPathBtn.Name = "getPathBtn";
            this.getPathBtn.Size = new System.Drawing.Size(35, 23);
            this.getPathBtn.TabIndex = 12;
            this.getPathBtn.Text = "...";
            this.getPathBtn.UseVisualStyleBackColor = true;
            this.getPathBtn.Click += new System.EventHandler(this.getPathBtn_Click);
            // 
            // setFileBtn
            // 
            this.setFileBtn.Location = new System.Drawing.Point(419, 318);
            this.setFileBtn.Name = "setFileBtn";
            this.setFileBtn.Size = new System.Drawing.Size(73, 23);
            this.setFileBtn.TabIndex = 9;
            this.setFileBtn.Text = "发送文件";
            this.setFileBtn.UseVisualStyleBackColor = true;
            this.setFileBtn.Click += new System.EventHandler(this.setFileBtn_Click);
            // 
            // setPath
            // 
            this.setPath.Location = new System.Drawing.Point(12, 319);
            this.setPath.Name = "setPath";
            this.setPath.Size = new System.Drawing.Size(369, 21);
            this.setPath.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 304);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "发送目录:";
            // 
            // setPathBtn
            // 
            this.setPathBtn.Location = new System.Drawing.Point(380, 318);
            this.setPathBtn.Name = "setPathBtn";
            this.setPathBtn.Size = new System.Drawing.Size(35, 23);
            this.setPathBtn.TabIndex = 12;
            this.setPathBtn.Text = "...";
            this.setPathBtn.UseVisualStyleBackColor = true;
            this.setPathBtn.Click += new System.EventHandler(this.setPathBtn_Click);
            // 
            // client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 350);
            this.Controls.Add(this.setPathBtn);
            this.Controls.Add(this.getPathBtn);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.setPath);
            this.Controls.Add(this.getPath);
            this.Controls.Add(this.setFileBtn);
            this.Controls.Add(this.sendBtn);
            this.Controls.Add(this.getFileBtn);
            this.Controls.Add(this.inputHint);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sendEdit);
            this.Controls.Add(this.recvShow);
            this.Controls.Add(this.ClientSpot);
            this.Controls.Add(this.ClientStart);
            this.Controls.Add(this.ClientPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ClientIp);
            this.Name = "client";
            this.Text = "客户端";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox ClientIp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox ClientPort;
        private System.Windows.Forms.Button ClientStart;
        private System.Windows.Forms.Button ClientSpot;
        private System.Windows.Forms.TextBox recvShow;
        private System.Windows.Forms.TextBox sendEdit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label inputHint;
        private System.Windows.Forms.Button getFileBtn;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.TextBox getPath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button getPathBtn;
        private System.Windows.Forms.Button setFileBtn;
        private System.Windows.Forms.TextBox setPath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button setPathBtn;
    }
}

