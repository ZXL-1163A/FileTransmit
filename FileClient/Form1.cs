using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace FileClient
{
    public partial class client : Form
    {
        private static readonly object ObjLock = new object();

        // 实例化socket通讯控制类
        private SocketClient SocketClient = new SocketClient();

        public List<string> RecvValue = new List<string> { };

        private string iniFilePath = "./ClientConfig.ini";

        // 定义声明委托，子线程需要代理操作界面
        private delegate void DelegateUpdateUi(string value);
        private DelegateUpdateUi delegateUpdateUi;  // 界面 运行和接收信息显示委托
        private DelegateUpdateUi delegateUiEnabled;  // 界面 禁能相关控制


        #region 初始化

        public client()
        {
            InitializeComponent();

            this.setConfigParam();  // 初始化读取配置文件

            // 事件、委托绑定函数
            //this.SocketClient.EventSetUivalue += new SocketClient.DelegateSetUiValue(SetUivalue);
            this.SocketClient.EventSetUivalue += this.SetUivalue;

            delegateUpdateUi = new DelegateUpdateUi(this.UpdateRunLog);
            delegateUiEnabled = new DelegateUpdateUi(this.UiEnabled);

            this.UiEnabled(true);  // 初始化禁能控制
        }

        /// <summary>
        /// 读取配置文件获取参数
        /// </summary>
        private void setConfigParam()
        {
            // 判断文件配置文件是否存在，不在则创建并退出函数
            bool flag = File.Exists(iniFilePath);
            if (!flag)
            {
                string iniFileName = iniFilePath;
                FileStream fs = File.Create(iniFileName);
                fs.Close();
                return;
            }

            // 获取ip和端口号， ip点之间不足三位的补足三位
            string netIp = "";
            string ip = ClientIni.Read("net", "netIp", "", iniFilePath);
            string port = ClientIni.Read("net", "netPort", "", iniFilePath);
            string[] ipList = ip.Split('.');
            for (int i = 0; i < ipList.Length; i++)
            {
                while (ipList[i].Length < 3)
                {
                    ipList[i] += " ";
                }
                netIp += ipList[i];
            }

            this.ClientIp.Text = netIp.Trim();
            this.ClientPort.Text = port.Trim();

            // 获取接收和发送文件路径
            string getPaht = ClientIni.Read("PATH", "recv", "", iniFilePath);
            string setPath = ClientIni.Read("PATH", "send", "", iniFilePath);
            this.getPath.Text = getPaht.Trim();
            this.setPath.Text = setPath.Trim();

            this.SocketClient.filePathList[0] = getPaht;
            this.SocketClient.filePathList[1] = setPath;
        }

        #endregion

        #region  发送内容输入框水印控制
        private void inputHint_Click(object sender, EventArgs e)
        {
            this.sendEdit.Focus();
        }

        private void sendEdit_Enter(object sender, EventArgs e)
        {
            this.inputHint.Visible = false;
        }

        private void sendEdit_Leave(object sender, EventArgs e)
        {
            if (this.sendEdit.Text == "")
                this.inputHint.Visible = true;
        }

        #endregion


        /// <summary>
        /// SocektServer事件响应函数，广播机制
        /// </summary>
        /// <param name="value"><内容/param>
        /// <param name="name"><任务类型/param>
        private void SetUivalue(string value, string name)
        {
            // 广播机制分发消息
            switch (name)
            {
                case "recvShow":  // 更新运行和接收信息显示控件
                    this.DelRecvValue();
                    value = DateTime.Now.ToString() + ": " + value;
                    this.RecvValue.Add(value);
                    this.updateUiValue();
                    break;
                case "enabled":
                    this.Invoke(delegateUiEnabled, value);
                    break;
            }
        }

        /// <summary>
        /// 控制显示信息数量不大于30条
        /// </summary>
        private void DelRecvValue()
        {
            while (this.RecvValue.Count >= 30)
            {
                this.RecvValue.RemoveAt(0);
            }
        }

        /// <summary>
        /// 调用委托更新UI控件 recvShow 显示
        /// </summary>
        private void updateUiValue()
        {
            // 加锁防止遍历列表过程中列表被修改
            lock(ObjLock)
            {
                string str = "";
                foreach (string value in this.RecvValue)
                {
                    str += value;
                    //str += "\r\n";
                    str += System.Environment.NewLine;
                }
                this.Invoke(delegateUpdateUi, str);
            }
            
        }

        /// <summary>
        /// 委托更新界面 运行和接收信息显示 控件
        /// </summary>
        /// <param name="str"><值/param>
        private void UpdateRunLog(string str)
        {
            this.recvShow.Text = str;
        }

        /// <summary>
        /// 界面禁能控制
        /// </summary>
        /// <param name="name"><任务类型/param>
        private void UiEnabled(string name)
        {
            switch (name)
            {
                case "connClient":  // 断开连接
                    this.UiEnabled(true);
                    break;
                case "finish":  // 文件传输完成
                    this.BtnEnabled(true);
                    break;
            }
        }

        /// <summary>
        /// 控件禁能
        /// </summary>
        /// <param name="mark"><false禁能, true解除/param>
        public void UiEnabled(bool mark)
        {
            this.ClientStart.Enabled = mark;  // 连接按钮
            this.ClientSpot.Enabled = !mark;  // 端口按钮
            this.ClientIp.Enabled = mark;  // ip输入框
            this.ClientPort.Enabled = mark;  // 端口输入框
            this.sendBtn.Enabled = !mark;  // 发送按钮
            this.getFileBtn.Enabled = !mark;  // 获取文件按钮
            this.setFileBtn.Enabled = !mark;  // 发送文件按钮
        }

        /// <summary>
        /// 判断路径合法性
        /// </summary>
        /// <param name="dir"><路径/param>
        /// <param name="name"><路径名称/param>
        /// <returns><合法返回true，否则返回false/returns>
        private bool PathExists(string dir, string name)
        {
            if (Directory.Exists(dir))
                return true;
            else
            {
                try
                {
                    Directory.CreateDirectory(dir);
                    return true;
                }
                catch (Exception)
                {
                    this.SetUivalue(name + " 非合法路径", "recvShow");
                    return false;
                }
            }
        }

        /// <summary>
        /// 连接按钮事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientStart_Click(object sender, EventArgs e)
        {
            this.UiEnabled(false);
            // 获取界面IP和端口号，并保存至配置文件
            string ip = this.ClientIp.Text.Trim();
            string port = this.ClientPort.Text.Trim();
            string netIp = ip.Replace(" ", "");
            string netPort = port.Replace(" ", "");

            ClientIni.Write("net", "netIp", netIp, iniFilePath);
            ClientIni.Write("net", "netPort", netPort, iniFilePath);

            // 路径合法性验证
            string getPath = this.getPath.Text.Trim();
            string setPath = this.setPath.Text.Trim();

            if (this.PathExists(setPath, "发送文件路径") &
                this.PathExists(getPath, "保存文件路径"))
            {
                this.SocketClient.filePathList[0] = getPath;
                this.SocketClient.filePathList[1] = setPath;

                ClientIni.Write("PATH", "recv", getPath, iniFilePath);
                ClientIni.Write("PATH", "send", setPath, iniFilePath);

                this.SocketClient.CreateSocketClient(netIp, netPort);
            }
            else
                this.UiEnabled(true);
        }

        /// <summary>
        /// 发送内容按钮事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendBtn_Click(object sender, EventArgs e)
        {
            string value = this.sendEdit.Text.Trim();
            this.SocketClient.SendMes(value);
        }

        /// <summary>
        /// 获取文件按钮事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getFileBtn_Click(object sender, EventArgs e)
        {
            this.BtnEnabled(false);
            // 判断路径的合法性
            string getPath = this.getPath.Text.Trim();

            if (this.PathExists(getPath, "保存文件路径"))
            {
                this.SocketClient.filePathList[0] = getPath;
                ClientIni.Write("PATH", "recv", getPath, iniFilePath);
                SocketClient.getFile();
            }
        }

        /// <summary>
        /// 接收文件过程禁能控制
        /// </summary>
        /// <param name="mark"><false禁能，true解除/param>
        private void BtnEnabled(bool mark)
        {
            this.getPath.Enabled = mark;
            this.getPathBtn.Enabled = mark;
            this.getFileBtn.Enabled = mark;
            this.setPath.Enabled = mark;
            this.setPathBtn.Enabled = mark;
            this.setFileBtn.Enabled = mark;
            this.sendBtn.Enabled = mark;
            this.sendEdit.Enabled = mark;
        }

        /// <summary>
        /// 发送文件按钮事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setFileBtn_Click(object sender, EventArgs e)
        {
            this.BtnEnabled(false);
            // 判断发送目录合法性
            string sendPath = this.setPath.Text.Trim();
            if (this.PathExists(sendPath, "发送文件路径"))
            {
                this.SocketClient.filePathList[1] = sendPath;
                ClientIni.Write("PATH", "send", sendPath, iniFilePath);
                this.SocketClient.ThreadSetFile();
            }
        }

        /// <summary>
        /// 断开连接按钮事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientSpot_Click(object sender, EventArgs e)
        {
            this.SocketClient.closeClient();
            this.UiEnabled(true);
        }

        /// <summary>
        /// 跟随最后一行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recvShow_TextChanged(object sender, EventArgs e)
        {
            this.recvShow.SelectionStart = this.recvShow.Text.Length;
            this.recvShow.SelectionLength = 0;
            this.recvShow.ScrollToCaret();
        }

        /// <summary>
        /// 设置保存目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getPathBtn_Click(object sender, EventArgs e)
        {
            string getPath = selectPath(this.getPath.Text);
            this.getPath.Text = getPath;
            this.SocketClient.filePathList[0] = getPath;
            ClientIni.Write("PATH", "recv", getPath, iniFilePath);
        }

        /// <summary>
        /// 设置发送目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setPathBtn_Click(object sender, EventArgs e)
        {
            string setPath = selectPath(this.setPath.Text);
            this.setPath.Text = setPath;
            this.SocketClient.filePathList[1] = setPath;
            ClientIni.Write("PATH", "send", setPath, iniFilePath);
        }

        /// <summary>
        /// 弹窗选择目录
        /// </summary>
        /// <returns></returns>
        private string selectPath(string name)
        {
            string folderPath = name;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "选择目录";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = dialog.SelectedPath;
            }
            return folderPath;
        }

    }
}
