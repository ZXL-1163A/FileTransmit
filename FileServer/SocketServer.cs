using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace FileServer
{
    class SocketServer
    {
        //1、创建一个用于监听连接的Socket对像；
        //2、将服务器的字符串ip地址转换为网络地址(IPAddress),端口号为int；
        //3、将IP与端口绑定(IPEndPoint)；
        //4、用socket对像的Bind()方法绑定IPEndPoint；
        //5、用socket对像的Listen()方法设置同一时刻最大可连接多少个请求；
        //6、socket对像的Accept()方法等待客户端的连接，连接成功返回通信的socket；
        //7、用通信socket的send()方法发送消息给客户端；
        //8、用通信socket的Receive()方法接收客户端发送的消息；
        //9、关闭socket（服务器一般不使用关闭）;

        //委托执行运行和接收信息显示
        public delegate void DelegateSetUiValue(string value, string name);
        // 基于上面的委托定义事件
        public event DelegateSetUiValue EventSetUivalue;

        // 声明
        Thread AcceptThread;  // 监听连接线程
        Thread ReceiveThread;  // 收发消息线程
        Socket socketWatch;  // 用于监听的Socket
        

        //保存连接的客户端的IP地址和Socket的集合
        Dictionary<string, Socket> dicSocket = new Dictionary<string, Socket>();

        public string[] filePathList = new string[2];


        /// <summary>
        /// 创建监听Socket并启动线程监听
        /// </summary>
        /// <param name="net_ip"><本机ip/param>
        /// <param name="net_port"><端口号/param>
        public void CreateSocketServer(string net_ip, string net_port)
        {
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                IPAddress ip = IPAddress.Parse(net_ip);
                int port = Convert.ToInt32(net_port);
                IPEndPoint ipPort = new IPEndPoint(ip, port);  //ip与端口绑定
                socketWatch.Bind(ipPort); //socket绑定ip与端口号
                socketWatch.Listen(10);  //设置同一时刻最大可连接多少个请求
            }
            catch (FormatException)
            {
                EventSetUivalue("Ip或端口设置异常", "recvShow");
                EventSetUivalue("run", "enabled");  // 创建失败解除界面禁能控制
                return;
            }

            //创建监听连接线程
            AcceptThread = new Thread(new ThreadStart(AcceptConnect));
            AcceptThread.IsBackground = true;  // 设置为后台线程
            AcceptThread.Start();
            EventSetUivalue("开始监听...", "recvShow");
        }

        /// <summary>
        /// 监听连接线程函数
        /// </summary>
        /// <param name="obj"></param>
        private void AcceptConnect()
        {
            // 服务端可能存在多个客户端连接，所以通信socket不能使用类变量
            Socket socketObj;  
            while (true)
            {
                try
                {
                    socketObj = socketWatch.Accept();
                }
                    catch (Exception)
                {
                    EventSetUivalue("服务器停止服务...", "recvShow");
                    return;
                }

                //获取连接的ip地址和端口号
                string strIp = socketObj.RemoteEndPoint.ToString();
                dicSocket.Add(strIp, socketObj);
                EventSetUivalue(strIp, "AddClient");  // 添加已连接客户端至下拉框控件
                // 连接的信息添加至recvShow
                string strMsg = "连接: " + socketObj.RemoteEndPoint + "连接成功";
                EventSetUivalue(strMsg, "recvShow");

                //创建收消息线程，
                ReceiveThread = new Thread(new ParameterizedThreadStart(RecvMessage));
                ReceiveThread.IsBackground = true;
                ReceiveThread.Start(socketObj);
            }
        }


        #region 接收

        /// <summary>
        /// 线程socket接收数据函数
        /// </summary>
        private void RecvMessage(Object obj)
        {
            Socket socketObj = obj as Socket;

            string hint;  // 提示信息
            int count;  // 读取缓存区字节数
            string value;  // 接收信息内容
            bool mark = true;  //标记连接是否断开

            byte[] taskBuffer;  // 任务缓存区
            byte[] MessageBuffer; // 消息缓存区

            while (mark)
            {
                taskBuffer = new byte[1];
                count = this.SocketRecv(socketObj, taskBuffer);

                if (count ==0)
                    break;
                else
                {
                    switch (taskBuffer[0])
                    {
                        case 0:  // 接收到的为 文字消息
                            MessageBuffer = new byte[2048];
                            count = this.SocketRecv(socketObj, MessageBuffer);
                            if (count == 0)
                            {
                                mark = false;  // 接收到为空时需要退出 while 循环；
                                break;
                            }
                            value = Encoding.Default.GetString(MessageBuffer, 0, count);
                            hint = "接收：" + socketObj.RemoteEndPoint + "发送的消息:" + value;
                            EventSetUivalue(hint, "recvShow");
                            break;
                        case 1:  // 接收到的为 保存文件
                            EventSetUivalue("recv0", "enabled");  // 接收文件过程中保存目录相关参数禁止设置
                            this.saveFile(socketObj);
                            EventSetUivalue("recv1", "enabled");
                            break;
                        case 2:  // 接收到的为 获取文件
                            EventSetUivalue("send0", "enabled");  // 发送文件过程中发送目录相关参数禁止设置
                            this.readFile(socketObj);
                            EventSetUivalue("send1", "enabled");
                            break;
                    }
                }
            }

            // 只要退出接收循环就表示断开连接
            string socketIp = socketObj.RemoteEndPoint.ToString();
            hint = "断开: " + socketIp + "断开连接";
            EventSetUivalue(hint, "recvShow");
            EventSetUivalue(socketIp, "DelClient");  // 客户端连接下拉框删除已断开连接客户端
        }

        /// <summary>
        /// socket读取统一接口
        /// </summary>
        /// <param name="socketObj"><通信socket对象/param>
        /// <param name="buffer"><读取后保存的缓存空间/param>
        /// <returns><读取长度/returns>
        private int SocketRecv(Socket socketObj, byte[] buffer)
        {
            int sum;
            try
            {
                sum = socketObj.Receive(buffer);
            }
            catch (SocketException)
            {
                sum = 0;
            }
            return sum;
        }

        /// <summary>
        /// 接收文件
        /// </summary>
        /// <param name="socketObj"><通信socket对象/param>
        private void saveFile(Socket socketObj)
        {
            // 声明
            string hint;  // 提示信息
            byte[] DataBuffer; // 缓存文件信息空间
            byte[] fileSumBuffer;  //文件数量

            Tuple<string, string> fileTuple;  // 文件信息元组
            string fileName;  // 文件名称
            string fileSize;  // 文件大小

            string savePath;  // 包含路径的文件名
            int recvSumSize;  // 已读取文件大小
            int recvLen;  // 单次读取缓存大小
            bool mark;  // 标记文件是否保存完整
            int fileSum; // 需要接收文件数量
            int saveSum; // 已经保存数量

            // 接收文件数量
            fileSumBuffer = new byte[4];
            recvLen = this.SocketRecv(socketObj, fileSumBuffer);
            if (recvLen == 0)
            {
                hint = "接收文件数量异常断开连接";
                this.EventSetUivalue(hint, "recvShow");
                return;
            }
            fileSum = BitConverter.ToInt32(fileSumBuffer, 0);

            DataBuffer = new byte[1024];
            saveSum = 0;
            while (saveSum < fileSum)
            {
                mark = true;
                // 读取文件信息缓存
                recvLen = this.SocketRecv(socketObj, DataBuffer);
                if (recvLen == 0)
                {
                    hint = "接收文件信息过程中异常";
                    this.EventSetUivalue(hint, "recvShow");
                    break;
                }

                // 解析文件信息（名称和大小）
                fileTuple = this.FileParam(DataBuffer);
                fileName = fileTuple.Item1;
                fileSize = fileTuple.Item2;

                // 设置包含本地目录名称的文件名
                savePath = filePathList[1] + "\\" + fileName;

                //判断目标文件是否存在，存在则删除。
                if (File.Exists(savePath))
                    File.Delete(savePath);
                // 如果是空文件则只会发送文件信息
                FileStream fs = File.Create(savePath);
                fs.Close();

                byte[] buttf = new byte[1024 * 1024 * 5];
                recvSumSize = 0;
                while (recvSumSize < Convert.ToInt32(fileSize))
                {
                    recvLen = this.SocketRecv(socketObj, buttf);
                    if (recvLen == 0)
                    {
                        hint = "接收" + fileName + "文件过程中异常中止接收";
                        this.EventSetUivalue(hint, "recvShow");
                        mark = false;
                        break;
                    }
                    using (FileStream fscreat = new FileStream(savePath, FileMode.Append, FileAccess.Write))
                    {
                        fscreat.Write(buttf, 0, recvLen);
                    }
                    recvSumSize += recvLen;
                }

                if (mark)  // 非异常退出循环则表示单个文件接收完成
                {
                    hint = "文件 " + fileName + " 接收保存完成";
                    this.EventSetUivalue(hint, "recvShow");
                }
                saveSum += 1;
            }
        }

        /// <summary>
        /// 解析文件信息
        /// </summary>
        /// <param name="buffer"><文件信息字节数组/param>
        /// <returns><文件信息元组，item1文件名称，item2文件大小/returns>
        private Tuple<string, string> FileParam(byte[] buffer)
        {
            byte[] nameByteLenByte;  // 文件名称长度数组
            int nameByteLen;  // 文件名称长度
            byte[] nameByte;  // 文件名称数组
            string name;  // 文件名称

            byte[] sizeLenByte;  // 文件大小长度数组
            int sizeLen;  // 文件大小长度
            byte[] sizeByte;  // 文件大小数组
            string size;  // 文件大小

            int i = 0;  // 记录到达那个字节数

            // 解析文件名称长度 byte[]
            nameByteLenByte = new byte[4];
            Array.Copy(buffer, i, nameByteLenByte, 0, 4);
            i += 4;
            nameByteLen = BitConverter.ToInt32(nameByteLenByte, 0);

            // 解析文件名 byte[]
            nameByte = new byte[nameByteLen];
            Array.Copy(buffer, i, nameByte, 0, nameByteLen);
            i += nameByteLen;
            name = Encoding.Default.GetString(nameByte);

            // 解析文件大小长度 byte[]
            sizeLenByte = new byte[4];
            Array.Copy(buffer, i, sizeLenByte, 0, 4);
            i += 4;
            sizeLen = BitConverter.ToInt32(sizeLenByte, 0);

            // 解析文件大小 byte[]
            sizeByte = new byte[sizeLen];
            Array.Copy(buffer, i, sizeByte, 0, sizeLen);
            i += sizeLen;
            size = Encoding.Default.GetString(sizeByte);


            return new Tuple<string, string>(name, size);
        }

        #endregion


        #region 发送

        /// <summary>
        /// socket发送统一接口
        /// </summary>
        /// <param name="socketObj"><socket对象，使用字段多个客户端会出现异常/param>
        /// <param name="buffer"><需要发送的字节数组/param>
        /// <param name="name"><提示信息/param>
        /// <param name="mark"><发送成功是否提示/param>
        private void socketSendFunc(Socket socketObj, byte[] buffer, string name, bool mark)
        {
            string hint;
            try
            {
                //发送
                socketObj.Send(buffer);
                if (mark == true)  // 有些位置只发送了一部分数据
                {
                    hint = "发送: " + name + "完成";
                    this.EventSetUivalue(hint, "recvShow");
                }
            }
            catch (Exception)
            {
                hint = "发送: " + name + "异常";
                this.EventSetUivalue(hint, "recvShow");
            }

        }

        /// <summary>
        /// 指定连接客户端发送消息
        /// </summary>
        /// <param name="ip"><连接客户端ip地址/param>
        /// <param name="value"><发送的内容/param>
        public void SendMessage(string ip, string value)
        {
            // 声明
            string hint;  // 提示信息
            byte[] taskBuffer; // 任务类型
            byte[] MessageBuffer;  // 消息数组

            // 发送任务命令
            taskBuffer = new byte[1] { 0 };
            this.socketSendFunc(this.dicSocket[ip], taskBuffer, "消息命令", true);

            //发送消息
            hint = ip + " : " + value;
            MessageBuffer = Encoding.Default.GetBytes(value);
            this.socketSendFunc(this.dicSocket[ip], MessageBuffer, hint, true);
        }

        /// <summary>
        /// 客户端获取文件，发送文件
        /// </summary>
        /// <param name="buffer"><发送的通信socket对象/param>
        private void readFile(Socket socketObj)
        {
            // 声明
            #region  声明
            int readLen;  // 读取文件长度
            string hint;  // 提示信息
            string filename;  // 文件名称，不包含路径

            string[] nameList; // 文件名称列表
            string[] sizeList; // 文件大小

            byte[] taskBuffer;  //任务类型
            byte[] fileSumBuffer;  //文件数量
            byte[] DataBuffer; // 缓存文件信息空间
            byte[] fileBuffer;  // 缓存读取文件空间
            byte[] validBuffer;  // 有效缓存字节

            Tuple<byte[], byte[]> nameTuple;  // 文件名称元组，item1长度，item2名称
            Tuple<byte[], byte[]> sizeTuple;  // 文件大小元组

            List<byte> SendList;
            #endregion


            // 发送任务命令
            taskBuffer = new byte[1] {1};
            this.socketSendFunc(socketObj, taskBuffer, "发送文件命令", true);   // 发送命令

            // 获取发送目录下所有文件名称
            nameList = Directory.GetFiles(this.filePathList[0]);
            // 创建保存文件大小列表
            sizeList = new string[nameList.Length];

            // 发送文件数量
            fileSumBuffer = new byte[4]; // 固定int大小用于发送文件数量
            fileSumBuffer = BitConverter.GetBytes(nameList.Length);
            this.socketSendFunc(socketObj, fileSumBuffer, "发送文件数量", true);

            SendList = new List<byte>();

            // 遍历发送目录下文件列表
            for (int i=0; i< nameList.Length; i++)
            {
                SendList.Clear();
                // 获取文件大小
                sizeList[i] = new FileInfo(nameList[i]).Length.ToString();
                // 获取除去路径的文件名称
                filename = Path.GetFileName(nameList[i]);

                // 打包文件名称
                nameTuple = FileParam(filename);  
                SendList.AddRange(nameTuple.Item1);  //文件名称长度
                SendList.AddRange(nameTuple.Item2);  // 文件名称

                // 打包文件大小
                sizeTuple = FileParam(sizeList[i]);  
                SendList.AddRange(sizeTuple.Item1);  // 文件大小长度
                SendList.AddRange(sizeTuple.Item2);  // 文件大小

                // 发送文件信息，固定1024字节。
                DataBuffer = new byte[1024];
                Array.Copy(SendList.ToArray(), 0, DataBuffer, 0, SendList.ToArray().Length);
                this.socketSendFunc(socketObj, DataBuffer, filename+" 文件信息", true);   

                // 定义缓存空间，加载打开的文件
                fileBuffer = new byte[1024 * 1024 * 5];
                using (FileStream stream = new FileStream(nameList[i], FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    while (true)
                    {
                        // 读取文件至缓存空间
                        readLen = stream.Read(fileBuffer, 0, fileBuffer.Length);
                        //如果读取到的字节数为0，说明已到达文件结尾，则退出while循
                        if (readLen == 0)
                        {
                            hint = "发送 " + filename + " 文件完成";
                            this.EventSetUivalue(hint, "recvShow");
                            break;
                        }

                        // 发送文件内容，只发送有效缓存部分
                        if (readLen < fileBuffer.Length)
                        {
                            validBuffer = new byte[readLen];
                            Array.Copy(fileBuffer, 0, validBuffer, 0, readLen);
                            this.socketSendFunc(socketObj, validBuffer, "文件内容", false);   
                        }
                        else
                            this.socketSendFunc(socketObj, fileBuffer, "文件内容", false);
                    }
                }
            }
        }

        /// <summary>
        /// 获取字符串 长度byte[]和内容byte[]
        /// </summary>
        /// <param name="param"><字符串/param>
        /// <returns><元组(长度，字符串)/returns>
        private Tuple<byte[], byte[]> FileParam(string param)
        {
            byte[] value;
            byte[] len = new byte[4];

            value = Encoding.Default.GetBytes(param);
            len = BitConverter.GetBytes(value.Length);

            return new Tuple<byte[], byte[]>(len, value);
        }

        #endregion

    }



}
