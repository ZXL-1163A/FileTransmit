using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace FileClient
{
    class SocketClient
    {
        //1、建立一个Socket对像；
        //2、将需要连接的字符串ip地址转换为网络地址(IPAddress),端口号为int；
        //3、用socket对像的Connect()方法，向服务器发出连接请求；
        //4、如果连接成功，就用socket对像的Send()方法向服务器发送信息；
        //5、用socket对像的Receive()方法接受服务器发来的信息 ;
        //6、断开连接关闭socket；

        // 事件委托
        public delegate void DelegateSetUiValue(string value, string name);
        // 基于上面的委托定义事件
        public event DelegateSetUiValue EventSetUivalue;

        private static readonly object ObjLock = new object();

        // 声明
        Socket socketSend;  // 用于通信的Socket
        Thread ReceiveThread;  // 收消息线程
        Thread SendFileThread;  // 发送文件线程

        List<byte> sendList = new List<byte>();
        public string[] filePathList = new string[2];

        /// <summary>
        /// 创建Socket连接
        /// </summary>
        /// <param name="net_ip"><连接ip地址/param>
        /// <param name="net_port"><连接端口号/param>
        public void CreateSocketClient(string net_ip, string net_port)
        {
            IPAddress ip;  // 网络ip
            int port;  // 端口号
            string hint;  // 提示

            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                ip = IPAddress.Parse(net_ip);
                port = Convert.ToInt32(net_port);
                socketSend.Connect(ip, port);
                EventSetUivalue("连接成功...", "recvShow");  // 返回连接成功事件
            }
            catch (SocketException)
            {
                hint = "连接服务器未开或ip与端口号设置错误";
                EventSetUivalue(hint, "recvShow");
                EventSetUivalue("connClient", "enabled");  // 连接失败解除界面禁能控制
                return;
            }

            // 创建收消息线程
            ReceiveThread = new Thread(new ThreadStart(RecvMessage));
            ReceiveThread.IsBackground = true;  //设置为后台线程
            ReceiveThread.Start();
        }


        #region  接收

        /// <summary>
        /// 线程接收消息函数
        /// </summary>
        private void RecvMessage()
        {
            // 声明
            string hint;  //提示信息
            byte[] taskBuffer; //任务类型
            byte[] MessageBuffer; // 消息数组
            int count;  // 读取字节数
            string value;  // 接收的内容

            while (true)
            {
                taskBuffer = new byte[1];
                count = this.SocketRecv(taskBuffer);
                if (count == 0)
                    break;
                else
                {
                    if (taskBuffer[0] == 1)  // 接收文件
                    {
                        this.saveFile();
                        EventSetUivalue("finish", "enabled");  // 接收文件退出解除界面禁能控制
                    }
                        
                    else  // 接收消息 
                    {
                        MessageBuffer = new byte[2048];
                        count = this.SocketRecv(MessageBuffer);
                        if (count == 0)
                            break;
                        value = Encoding.Default.GetString(MessageBuffer, 0, count);
                        hint = "接收: " + socketSend.RemoteEndPoint + "发送的消息:" + value;
                        EventSetUivalue(hint, "recvShow");
                    }
                }
            }
            // 只要退出接收循环就表示断开连接
            hint = "断开连接";
            EventSetUivalue(hint, "recvShow");
            EventSetUivalue("connClient", "enabled");  // 解除界面禁能控制
        }

        /// <summary>
        /// socket读取统一接口
        /// </summary>
        /// <param name="buffer"><待读取的字节数组/param>
        /// <returns><读取长度/returns>
        private int SocketRecv(byte[] buffer)
        {
            int sum;
            try
            {
                sum = socketSend.Receive(buffer);
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
        private void saveFile()
        {
            // 声明
            string hint;  // 提示信息
            byte[] DataBuffer; // 文件信息
            byte[] fileSumBuffer;  //文件数量
            byte[] fileButtf;  //文件内容

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
            recvLen = this.SocketRecv(fileSumBuffer);
            if (recvLen == 0)
            {
                hint = "接收文件数量异常";
                this.EventSetUivalue(hint, "recvShow");
                return;
            }
            fileSum = BitConverter.ToInt32(fileSumBuffer, 0);


            DataBuffer = new byte[1024];
            saveSum = 0;
            while (saveSum < fileSum)
            {
                mark = true;
                // socket读取至缓存空间
                recvLen = this.SocketRecv(DataBuffer);
                if (recvLen == 0)
                {
                    hint = "接收文件信息异常";
                    this.EventSetUivalue(hint, "recvShow");
                    break;
                }
                
                // 解析文件信息（名称和大小）
                fileTuple = this.FileParam(DataBuffer);
                fileName = fileTuple.Item1;
                fileSize = fileTuple.Item2;

                // 远程文件名称添加本地路径
                savePath = filePathList[0] + "\\" + fileName;

                // 本地路径合法性在前面已经判断过了
                //判断本地文件是否存在，存在则删除。
                if (File.Exists(savePath)) 
                    File.Delete(savePath);
                // 如果是空文件服务端只发送文件信息，所以接收文件信息需要先创建文件
                FileStream fs = File.Create(savePath);
                fs.Close();

                fileButtf = new byte[1024 * 1024 * 5];
                recvSumSize = 0;
                while (recvSumSize < Convert.ToInt32(fileSize))
                {
                    recvLen = this.SocketRecv(fileButtf);
                    if (recvLen == 0)
                    {
                        hint = "接收 "+ fileName + " 文件过程中异常中止";
                        this.EventSetUivalue(hint, "recvShow");
                        mark = false;
                        break;
                    }
                    using (FileStream fscreat = new FileStream(savePath, FileMode.Append, FileAccess.Write))
                    {
                        fscreat.Write(fileButtf, 0, recvLen);
                    }
                    recvSumSize += recvLen;
                }

                
                if (mark)  // 非异常退出循环则表示文件接收完成
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


        #region  发送

        /// <summary>
        /// socket发送统一接口
        /// </summary>
        /// <param name="buffer"><发送字节数组/param>
        /// <param name="name"><提示信息/param>
        /// <param name="mark"><发送成功是否显示提示信息/param>
        private void socketSendFunc(byte[] buffer, string name, bool mark)
        {
            lock(ObjLock)
            {
                string hint;
                try
                {
                    this.socketSend.Send(buffer);
                    if (mark == true)
                    {
                        hint = "发送: " + name + " 完成";
                        this.EventSetUivalue(hint, "recvShow");
                    }
                }
                catch (Exception)
                {
                    hint = "发送: " + name + " 异常";
                    this.EventSetUivalue(hint, "recvShow");
                }
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="param"><消息内容/param>
        public void SendMes(string param)
        {
            byte[] taskBuffer = new byte[1] { 0 };
            this.socketSendFunc(taskBuffer, "消息命令", true);

            byte[] MessageBuffer = Encoding.Default.GetBytes(param);
            this.socketSendFunc(MessageBuffer, param, true);
        }

        /// <summary>
        /// 获取文件请求
        /// </summary>
        public void getFile()
        {
            byte[] Buffer = new byte[1] { 2 };
            this.socketSendFunc(Buffer, "获取文件请求", true);
        }

        /// <summary>
        /// 发送文件创建线程
        /// </summary>
        public void ThreadSetFile()
        {
            // 线程发送，防止发送大文件时界面假死
            SendFileThread = new Thread(new ThreadStart(setFile));
            SendFileThread.IsBackground = true;  //设置为后台线程
            SendFileThread.Start();
        }

        /// <summary>
        /// 线程发送文件
        /// </summary>
        /// <param name="buffer"><socket对象，使用字段多个客户端会出现异常/param>
        public void setFile()
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

            List<byte> SendList;  // 文件信息数组
            #endregion


            // 发送任务命令
            taskBuffer = new byte[1] { 1 };
            this.socketSendFunc(taskBuffer, "发送文件命令", true);

            // 获取发送目录下所有文件名称
            nameList = Directory.GetFiles(this.filePathList[1]);
            // 创建保存文件大小列表
            sizeList = new string[nameList.Length];

            fileSumBuffer = new byte[4]; // 固定int大小用于发送文件数量
            fileSumBuffer = BitConverter.GetBytes(nameList.Length);
            this.socketSendFunc(fileSumBuffer, "发送文件数量", true);

            SendList = new List<byte>();

            // 遍历设定目录下文件列表，需要用到与 sizeList 列表同步
            for (int i = 0; i < nameList.Length; i++)
            {
                SendList.Clear();
                // 获取文件大小
                sizeList[i] = new FileInfo(nameList[i]).Length.ToString();
                // 获取除去文件路径的文件名称
                filename = Path.GetFileName(nameList[i]);

                // 文件名称
                nameTuple = FileParam(filename);
                SendList.AddRange(nameTuple.Item1);  //文件名称长度
                SendList.AddRange(nameTuple.Item2);  // 文件名称
                // 文件大小
                sizeTuple = FileParam(sizeList[i]);
                SendList.AddRange(sizeTuple.Item1);  // 文件大小长度
                SendList.AddRange(sizeTuple.Item2);  // 文件大小

                // 新建一个1024字节的缓存， 将文件信息拷贝至字节数组
                DataBuffer = new byte[1024];
                Array.Copy(SendList.ToArray(), 0, DataBuffer, 0, SendList.ToArray().Length);

                // 发送文件信息
                hint = filename + " 文件信息";
                this.socketSendFunc(DataBuffer, hint, true); 

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

                        // 获取有效缓存内容发送
                        if (readLen < fileBuffer.Length)
                        {
                            validBuffer = new byte[readLen];
                            Array.Copy(fileBuffer, 0, validBuffer, 0, readLen);
                            this.socketSendFunc(validBuffer, "文件内容", false);   // 发送文件内容
                        }
                        else
                        {
                            this.socketSendFunc(fileBuffer, "文件内容", false);   // 发送文件内容
                        }
                    }
                }
            }

            // 发送文件完成解除界面禁能控制
            this.EventSetUivalue("finish", "enabled");
        }

        /// <summary>
        /// 获取字符串长度byte[]和字符串byte[]
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


        /// <summary>
        /// 断开连接
        /// </summary>
        public void closeClient()
        {
            if (socketSend != null)
                socketSend.Close();  //关闭socket
            if (ReceiveThread != null)
                ReceiveThread.Abort(); //终止线程
        }


    }
}
