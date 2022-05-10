using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace FileServer
{
    public partial class server : Form
    {
        private static readonly object ObjLock = new object();

        // 实例化socket通讯控制类
        private SocketServer SocketServer = new SocketServer();

        public List<string> RecvValue = new List<string> { };

        private string iniFilePath = @"./ServerConfig.ini";

        // 更新recvShow控件显示委托
        private delegate void DelegateUpdateUi(string value);
        private DelegateUpdateUi delegateUpdateUi;  // UI运行和接收信息显示刷新显示

        private delegate void DelegateConn(string name);
        private DelegateConn delegateAddConn;  // 新增客户端连接 connClient 控件添加
        private DelegateConn delegateDelConn;  // 断开客户端连接 connClient 控件减少

        private DelegateConn delegateEnabled;  //  UI禁能控制


        #region 初始化控制

        /// <summary>
        /// 构造函数
        /// </summary>
        public server()
        {
            InitializeComponent();

            this.setConfigParam();  // 初始化读取配置文件参数

            // 连接更新UI事件函数
            this.SocketServer.EventSetUivalue += new SocketServer.DelegateSetUiValue(SetUivalue);
            //this.SocketServer.EventSetUivalue += SetUivalue;

            // 创建委托实例
            delegateUpdateUi = new DelegateUpdateUi(this.UpdateRunLog);
            delegateAddConn = new DelegateConn(this.AddConnClientItem);
            delegateDelConn = new DelegateConn(this.DelConnClientItem);
            delegateEnabled = new DelegateConn(this.UiEnabled);

            this.UiEnabled(true);
        }

        /// <summary>
        /// 获取ini配置文件的参数更新至界面
        /// </summary>
        private void setConfigParam()
        {
            // 判断配置文件是否存在
            bool flag = File.Exists(iniFilePath);
            if (!flag)
            {
                string iniFileName = iniFilePath;
                FileStream fs = File.Create(iniFileName);
                fs.Close();
                return;
            }

            // 读取ip和端口号
            string netIp = "";
            string ip = ServerIni.Read("net", "netIp", "", iniFilePath);
            string port = ServerIni.Read("net", "netPort", "", iniFilePath);
            string[] ipList = ip.Split('.');
            for (int i=0;i<ipList.Length;i++)
            {
                while (ipList[i].Length < 3)
                {
                    ipList[i] += " ";
                }
                netIp += ipList[i];
            }

            this.ServerIp.Text = netIp.Trim();
            this.ServerPort.Text = port.Trim();

            // 读取路径
            string sendPath = ServerIni.Read("PATH", "send", "", iniFilePath);
            string recvPaht = ServerIni.Read("PATH", "recv", "", iniFilePath);
            this.sendPath.Text = sendPath;
            this.recvPath.Text = recvPaht;
            this.SocketServer.filePathList[0] = sendPath;
            this.SocketServer.filePathList[1] = recvPaht;
        }

        #endregion


        #region  输入框水印控制
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
        /// <param name="value"><值对象/param>
        /// <param name="name"><任务类型/param>
        private void SetUivalue(string value, string name)
        {
            // 广播机制分发消息
            switch (name)
            {
                case "recvShow":  // 更新recvShow控件显示
                    this.DelRecvValue();
                    value = DateTime.Now.ToString() + ": " + value;
                    this.RecvValue.Add(value);
                    this.updateUiValue();
                    break;
                case "AddClient":  // 添加已连接客户端
                    this.Invoke(delegateAddConn, value);
                    break;
                case "DelClient":  // 删除已断开客户端
                    this.Invoke(delegateDelConn, value);
                    break;
                case "enabled":  // UI禁能控制
                    this.Invoke(delegateEnabled, value);
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
        /// 更新UI控件 recvShow 显示
        /// </summary>
        private void updateUiValue()
        {
            // 加锁防止遍历列表过程中列表被修改
            lock (ObjLock)
            {
                string str = "";
                foreach (string value in this.RecvValue)
                {
                    str += value;
                    // str += "\r\n";
                    str += System.Environment.NewLine;
                }
                this.Invoke(delegateUpdateUi, str);
            }
        }

        /// <summary>
        /// 运行和接收信息显示 控件更新
        /// </summary>
        /// <param name="str"></param>
        private void UpdateRunLog(string str)
        {
            this.recvShow.Text = str;
        }

        /// <summary>
        /// SocketServer禁能相关控制
        /// </summary>
        /// <param name="name"><禁能类型/param>
        private void UiEnabled(string name)
        {
            switch(name)
            {
                case "run":  // 服务器开启
                    this.UiEnabled(true);
                    break;
                case "stop":  // 服务器停止
                    this.UiEnabled(false);
                    break;
                case "send0":  // 开始发送文件
                    this.sendPath.Enabled = false;
                    this.sendPathBtn.Enabled = false;
                    this.UiEnabled0(false);
                    break;
                case "send1":  // 结束发送文件
                    this.sendPath.Enabled = true;
                    this.sendPathBtn.Enabled = true;
                    this.UiEnabled0(true);
                    break;
                case "recv0":  // 开始接收文件
                    this.recvPath.Enabled = false;
                    this.recvPathBtn.Enabled = false;
                    this.UiEnabled0(false);
                    break;
                case "recv1":  // 结束接收文件
                    this.recvPath.Enabled = true;
                    this.recvPathBtn.Enabled = true;
                    this.UiEnabled0(true);
                    break;
            }
        }

        /// <summary>
        /// 文件传输过程禁能控制
        /// </summary>
        /// <param name="mark"></param>
        private void UiEnabled0(bool mark)
        {
            this.sendBtn.Enabled = mark;
            this.sendEdit.Enabled = mark;
        }

        /// <summary>
        /// 服务器开启后禁能控件
        /// </summary>
        /// <param name="mark"><状态/param>
        private void UiEnabled(bool mark)
        {
            this.startServer.Enabled = mark;
            this.ServerIp.Enabled = mark;
            this.getSelfIpBtn.Enabled = mark;
            this.ServerPort.Enabled = mark;
            this.sendBtn.Enabled = !mark;
        }

        /// <summary>
        /// 已连接客户端下拉控件添加元素
        /// </summary>
        /// <param name="item"><添加值/param>
        private void AddConnClientItem(string item)
        {
            this.connClient.Items.Add(item);
            this.connClient.SelectedIndex = 0;
        }

        /// <summary>
        /// 移除断开连接的客户端
        /// </summary>
        /// <param name="item"><删除值/param>
        private void DelConnClientItem(string item)
        {
            int index = this.connClient.Items.IndexOf(item);
            this.connClient.Items.RemoveAt(index);

            // 没有元素时需要置 "" 
            int sum = this.connClient.Items.Count;
            if (sum == 0)
                this.connClient.Text = "";
        }

        /// <summary>
        /// 开始监听按钮事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startServer_Click(object sender, EventArgs e)
        {
            this.UiEnabled(false);
            // 获取IP和端口号
            string ip = this.ServerIp.Text.Trim();
            string port = this.ServerPort.Text.Trim();
            string netIp = ip.Replace(" ", "");
            string netPort = port.Replace(" ", "");

            ServerIni.Write("net", "netIp", netIp, iniFilePath);
            ServerIni.Write("net", "netPort", netPort, iniFilePath);

            // 路径合法性验证
            string recvPath = this.recvPath.Text.Trim();
            string sendPath = this.sendPath.Text.Trim();

            if (this.PathExists(sendPath, "发送文件路径") &
                this.PathExists(recvPath, "保存文件路径") )
            {
                this.SocketServer.filePathList[1] = recvPath;
                this.SocketServer.filePathList[0] = sendPath;

                ServerIni.Write("PATH", "recv", recvPath, iniFilePath);
                ServerIni.Write("PATH", "send", sendPath, iniFilePath);

                this.SocketServer.CreateSocketServer(netIp, netPort);
            }
            else
                this.UiEnabled(true);
        }

        /// <summary>
        /// 判断路径合法性
        /// </summary>
        /// <param name="dir"><路径/param>
        /// <param name="name"><路径名称/param>
        /// <returns></returns>
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
        /// 发送消息按钮事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendBtn_Click(object sender, EventArgs e)
        {
            string value = this.sendEdit.Text.Trim();
            string ip = this.connClient.SelectedItem.ToString();
            this.SocketServer.SendMessage(ip, value);
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
        /// 设置发送目录按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendPathBtn_Click(object sender, EventArgs e)
        {
            string sendPath = selectPath(this.sendPath.Text);
            this.sendPath.Text = sendPath;
            this.SocketServer.filePathList[0] = sendPath;
            ServerIni.Write("PATH", "send", sendPath, iniFilePath);
        }

        /// <summary>
        /// 设置接收目录按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recvPathBtn_Click(object sender, EventArgs e)
        {
            string recvPath = selectPath(this.recvPath.Text);
            this.recvPath.Text = recvPath;
            this.SocketServer.filePathList[1] = recvPath;
            ServerIni.Write("PATH", "recv", recvPath, iniFilePath);
        }

        /// <summary>
        /// 弹窗选择目录
        /// </summary>
        /// <returns></returns>
        private string selectPath(string name)
        {
            string folderPath = name;
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "选择目录";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                folderPath = folder.SelectedPath;
            }
            return folderPath;
        }

        /// <summary>
        /// 获取本机IP按钮事件函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getSelfIpBtn_Click(object sender, EventArgs e)
        {
            string ip = "";
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress item in IpEntry.AddressList)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                        ip = item.ToString();
                }
            }

            // 不足三位的补足三位
            string netIp = "";
            string[] ipList = ip.Split('.');
            for (int i = 0; i < ipList.Length; i++)
            {
                while (ipList[i].Length < 3)
                {
                    ipList[i] += " ";
                }
                netIp += ipList[i];
            }

            this.ServerIp.Text = netIp.Trim();
        }

    }
}
