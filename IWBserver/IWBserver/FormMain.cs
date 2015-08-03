using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;


namespace IWBserver
{
    
    public partial class FormMain : Form
    {
        /**
         * 编写参考 http://blog.csdn.net/wuyazhe/article/details/5627253
         *  IAPer
         *  更改串口收取数据方式：原来是直接全部接受，要改进为 “按照前两个byte指明的数据包长度 来接收缓存中的数据”
         *  导入文件：1、限制只能.bin格式文件 --2015-08-02-ok  2、根据文件名 截取出版本号
            *重置内容，或根据请求包进行重置，关闭；-2015-8-2-OK
            *com端口数  -2015-8-1 OK
            重复发request 或者 server 发激活包 
            CRC校验
         * */
        //定义成员变量
        private SerialPort comm = new SerialPort();  
        private long received_count = 0;//接收计数  
        private long send_count = 0;//发送计数  
        

        //从 固件更新请求包 中 截取出 设备型号Product_ID
        byte[] byte2_Client_Product_ID = new byte[2] { 0x00, 0x01 };
        //从 固件更新请求包 中 截取出 设备的硬件版本Hardware_Rev,和Constant中的Server Hardware_Rev进行比较
        byte byte1_Client_Hardware_Rev;
        //从 固件更新请求包 中 截取出 设备的当前固件版本Firmwire_Rev 
        byte[] byte2_Client_Firmwire_Rev = new byte[2];
        //待发送的 固件更新初始包 
        byte[] byte14_Transmit_Preamble = new byte[14];

        //用于导入文件的全局变量
        string fileName = null;
        FileStream inputFileStream = null;
        //存放文件的byte数组
        byte[] byteFileData = null;
        //当前文件的分片个数，对应了IWB协议中的“固件更新初始包”的“Total_Num”参数
        //计算方式sliceNumber= byteData.length/SLICE_SIZE 再上取整
        int sliceNumber = 0;
        //固件更新初始包 中的Pad_Num ，最后一个固件更新数据包中的 有效字节数
        int preamble_Pad_Num = 0;
        //分片记数，用于count当前收到的是第几个 数据回应包，即确定了 前sliceReceiveCount个数据包已经被接收
        //再根据分片来计算出下一个 待发送的数据包分片的位置
        int sliceReceiveCount = 0;
        //固件更新 数据包的整体长度，用于构建comm串口的发送数据
        byte[] byteTransmitData = new byte[Constants.FW_DATA_PKT_TRANSMIT_TOTAL_LEN];
        //是否为第一次发送
        bool isfirstTransmit = true;
        //是否正在传送app.bin数据包
        bool isTransmitting = false;

        public FormMain()
        {
            InitializeComponent();
        }

        //窗体初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            
            //默认波特率选择 115200
            baudRate.SelectedIndex = baudRate.Items.IndexOf("115200");
            parity.SelectedIndex = parity.Items.IndexOf("None");
            dataBits.SelectedIndex = dataBits.Items.IndexOf("8");
            stopBits.SelectedIndex = stopBits.Items.IndexOf("One");

            //初始化SerialPort对象  
            comm.NewLine = "/r/n";
            //在串行通信中是否启用请求发送 (RTS) 信号？
            //comm.RtsEnable = true;
            comm.ReceivedBytesThreshold = 1;
            //添加事件注册  
            comm.DataReceived += comm_DataReceived;

        }

        //portNumber点击事件，每次点击，都重新初始化下拉串口名称列表框
        private void portNumber_Click(object sender, EventArgs e)
        {
            //初始化下拉串口名称列表框  
            string[] ports = SerialPort.GetPortNames();
            ports.Distinct();
            Array.Sort(ports);
            portNumber.Items.Clear();
            portNumber.Items.AddRange(ports);
            /*
            for (int ii = 0; ii < ports.Length;ii++ )
            {
                textBox_msgInfo.Text += "可选串口" + ports[ii] + "" + Environment.NewLine;
            }*/

            
            //默认选择下拉列表中的第一个串口 
            portNumber.SelectedIndex = 0;
        }

        /**
         * 响应事件处理
         * */
        void comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "收到数据" + Environment.NewLine;
           
            try
            {     
                int n = comm.BytesToRead;//收到的数据包长度  
                byte[] buf = new byte[n];//声明一个临时数组存储当前来的串口数据  
                received_count += n;//增加接收计数  
                comm.Read(buf, 0, n);//读取缓冲数据 
                //先在控制台中打印出来看看
                //textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "收到数据内容:" + buf[0] + "-" + buf[1] + "-" + buf[2] + "-" + buf[3] + Environment.NewLine;
                //<1、验证包的长度是否合乎协议的定义，不符合规格则丢弃>
                    //字符byte[]和int的转换 http://blog.csdn.net/brantyou/article/details/18698647
                //2个byte转成1个int
                int packLength = buf[0];
                packLength = packLength<<8;
                packLength += buf[1];
                /*if (packLength+2 !=n)//数据包长度非法
                {
                    //清空接收缓存
                    comm.DiscardInBuffer();
                    return;
                }*/
                //按照协议IWB规定，buf[2]中内容为Packet_Type
                //<2、判断 Packet_Type，本次接收到的是什么类型的数据包？>
                    //<3、按照Packet_Type对应的处理方法,解析包中数据，并作出回应>
                        //<4、执行完本次解析操作后，保留相应记录、清楚buf缓存>
                switch(buf[2])
                {
                 //收到 固件更新请求包，Sever端应该回应 固件更新初始包
                    case Constants.FW_REQUEST_PKT:

                        textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "收到固件更新请求包:" + byteArray2String0x(buf) + Environment.NewLine;
                        //异常处理：当前服务端正处于和设备进行 更新文件数据包的传输过程，趟若再收到 固件更新初始包，说明之前的传输无效，需要重新清零，重发
                        if (isTransmitting)
                        {
                            //接收记数清零
                            sliceReceiveCount = 0;
                            //清空接收缓存
                            comm.DiscardInBuffer();
                            
                        }
                        //尚待完成 异或校验
                        //1、解析 固件更新请求包，为全局变量配置参数，如硬件版本、当前固件版本等,
                        //并比较 设备的固件版本 是否低于 server端的固件版本（Constant类里面要定义），若低于，则需要更新，否则，不必更新
                        //并比较 设备和server的硬件版本 


                        //2、构建 固件更新初始包,共8个参数  例子{0x00, 0x0C, 0xF1, 0x00, 0x01, 0xA2, 0x00, 0x11, 0x00, 0x0C, 0x00, 0x00, 0xFF, 0x0D}
                        //2.1、Packet_Length=00 0C, 包后面的字节数s(12)
                        byte14_Transmit_Preamble[0] = 0x00;
                        byte14_Transmit_Preamble[1] = 0x0C;
                        //2.2、Packet_Type = 0xF1,
                        byte14_Transmit_Preamble[2] = Constants.FW_PREAMBLE_PKT;
                        //2.3、Product_ID = 设备 ID
                        byte14_Transmit_Preamble[3] = byte2_Client_Product_ID[0];//例子中0x00, 在此尚未按照设备号来写
                        byte14_Transmit_Preamble[4] = byte2_Client_Product_ID[1];//例子中0x01,
                        //2.4、Hardware_Rev  Server的硬件版本，在此先写"死"为0xA2
                        byte14_Transmit_Preamble[5] = 0xA2;
                        //2.5、Firmwire_Rev 导入文件的文件名中声明的固件版本号
                        byte14_Transmit_Preamble[6] = 0x00;
                        byte14_Transmit_Preamble[7] = 0x11;
                        //2.6、Total_Num 固件更新数据包的总个数
                        byte14_Transmit_Preamble[8] = (byte)(sliceNumber / 256);
                        byte14_Transmit_Preamble[9] = (byte)(sliceNumber % 256);
                        //2.7、Pad_Num 最后一个固件更新数据包中有效字节数
                        //Pad_Num对应全局变量preamble_Pad_Num
                            //1个int转成2个byte
                        byte14_Transmit_Preamble[10] = (byte)(preamble_Pad_Num / 256);
                        byte14_Transmit_Preamble[11] = (byte)(preamble_Pad_Num % 256);
                        //2.8、ACK_Request 是否需要回应？ 0xFF需要回应 
                        byte14_Transmit_Preamble[12] = 0xFF;
                        //2.8、Check_Sum 前若干字节异或校验值  目前默认不做校验
                        byte14_Transmit_Preamble[13] = 0x0D;
                        //固件更新初始包 构建完成

                        
                        //3、清空接收缓存
                        comm.DiscardInBuffer();
                        //4、向串口comm写数据，发送
                        //byte[] premble = new byte[]{0x00, 0x0C, 0xF1, 0x00, 0x01, 0xA2, 0x00, 0x11, 0x00, 0x0C, 0x00, 0x00, 0xFF, 0x0D};
                        comm.Write(byte14_Transmit_Preamble, 0, byte14_Transmit_Preamble.Length);
                        textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "发送premble:" + byteArray2String0x(byte14_Transmit_Preamble) + Environment.NewLine;

                        //当前状态置为“正在传输”
                        isTransmitting = true;
                        return;                        
                        break;


                    //收到 固件更新回应包，Sever端应该回应 固件更新数据包
                    case Constants.FW_RESPONSE_PKT:
                        textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "收到固件更新回应包（不做验证）" + buf[4] + Environment.NewLine;
                        //1、校验检查数据包
                        //1.1、尚待完成 异或校验
                        //1.2、获取 设备发回的固件更新回应包 中的Packet_Cnt，得知当前已经成功发送了几个包
                        int Packet_Cnt = buf[3];
                        Packet_Cnt = Packet_Cnt << 8;
                        Packet_Cnt += buf[4];
                        //Resonse_Content 设备是否成功接收上一个数据包？
                        if (buf[5] == 0x00)//成功接收上一个数据包
                        {
                            textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "收到固件更新回应包" + Packet_Cnt + "/" + sliceNumber + "内容" + byteArray2String0x(buf)+ Environment.NewLine;
                            sliceReceiveCount++;                            
                            
                            //在此要根据发送的分片序号记数，顺序发送每个分片                                               
                            
                            if (sliceReceiveCount < sliceNumber )
                            {
                                //2、构建 固件更新数据包
                                //2.1、构建固件更新数据包中的Packet_Length值，此处为 将数字1029转化为两个byte                       
                                byteTransmitData[0] = (byte)(Constants.FW_DATA_PKT_LEN_EXCEPT_HEAD / 256);
                                byteTransmitData[1] = (byte)(Constants.FW_DATA_PKT_LEN_EXCEPT_HEAD % 256);
                                //2.2、构建固件更新数据包中的Packet_Type值，此处为0xF2
                                byteTransmitData[2] = Constants.FW_DATA_PKT;
                                //2.3、构建固件更新数据包中的Packet_Index值，即 当前数据包为第几个分片，起始记数为1
                                byteTransmitData[3] = (byte)(sliceReceiveCount / 256);
                                byteTransmitData[4] = (byte)(sliceReceiveCount % 256);
                                //2.4、构建固件更新数据包中的Firmware_Data值,即为 发送的分片内容
                                //copy参数依次为_sourceBytes, sourceIndex, bytesNew, destinationIndex（前5个byte是HEAD）, dataLength(1024个byte)
                                Array.Copy(byteFileData, (sliceReceiveCount - 1) * Constants.SLICE_SIZE, byteTransmitData, 5, Constants.SLICE_SIZE);
                                //2.5、构建固件更新数据包中的ACK_Request值,"需要回应(0xFF) "
                                byteTransmitData[1029] = Constants.FW_DATA_PKT_ACK_REQUEST;
                                //2.6、构建固件更新数据包中的Check_Sum值,Check_Sum 默认为0x0D(目前还不做校验),
                                byteTransmitData[1030] = Constants.FW_DATA_PKT_CHECK_SUM;
                                //至此，固件更新数据包构建完成

                                //3、发送 固件更新数据包
                                //3.1、清空接收缓存
                                comm.DiscardInBuffer();
                                //3.2、向串口comm写数据，发送
                                comm.Write(byteTransmitData, 0, byteTransmitData.Length);
                                textBox_msgInfo.Text += "server发送数据内容" + byteArray2String0x(byteTransmitData) + Environment.NewLine;
                                return;
                                break;
                            }
                            else if (sliceReceiveCount == sliceNumber )//当前数据包是 所有分片中的最后一个
                            {
                                int Packet_Length = 5 + preamble_Pad_Num;
                                textBox_msgInfo.Text += "最后一个数据包 Packet_Length=" + Packet_Length + Environment.NewLine;
                                //2、构建 固件更新数据包
                                //2.1、构建固件更新数据包中的Packet_Length值，此处为 将数字1029转化为两个byte                       
                                byteTransmitData[0] = (byte)(Packet_Length / 256);
                                byteTransmitData[1] = (byte)(Packet_Length % 256);
                                //2.2、构建固件更新数据包中的Packet_Type值，此处为0xF2
                                byteTransmitData[2] = Constants.FW_DATA_PKT;
                                //2.3、构建固件更新数据包中的Packet_Index值，即 当前数据包为第几个分片，起始记数为1
                                byteTransmitData[3] = (byte)(sliceReceiveCount / 256);
                                byteTransmitData[4] = (byte)(sliceReceiveCount % 256);
                                //2.4、构建固件更新数据包中的Firmware_Data值,即为 发送的分片内容
                                //copy参数依次为_sourceBytes, sourceIndex, bytesNew, destinationIndex（前5个byte是HEAD）, dataLength(1024个byte)
                                Array.Copy(byteFileData, (sliceReceiveCount - 1) * Constants.SLICE_SIZE, byteTransmitData, 5, preamble_Pad_Num);

                                //2.5、构建固件更新数据包中的ACK_Request值,"需要回应(0xFF) "
                                int ACK_Request_index = 5 + preamble_Pad_Num;
                                byteTransmitData[ACK_Request_index] = Constants.FW_DATA_PKT_ACK_REQUEST;
                                //2.6、构建固件更新数据包中的Check_Sum值,Check_Sum 默认为0x0D(目前还不做校验),
                                int Check_Sum_index = ACK_Request_index + 1;
                                byteTransmitData[Check_Sum_index] = Constants.FW_DATA_PKT_CHECK_SUM;
                                //至此，固件更新数据包构建完成

                                //3、发送 固件更新数据包
                                //3.1、清空接收缓存
                                comm.DiscardInBuffer();
                                //3.2、向串口comm写数据，发送
                                comm.Write(byteTransmitData, 0, Check_Sum_index+1);
                                textBox_msgInfo.Text += "server发送最后一个数据包 内容" + byteArray2String0x(byteTransmitData, 0, Check_Sum_index + 1) + Environment.NewLine;
                                return;
                                break;
                            }                          

                            
                        }
                        else if (buf[5] == 0xF0)//异常情况 “硬件版本错误”
                        {
                            textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "ERROR硬件版本错误" + Packet_Cnt + "/" + sliceNumber + Environment.NewLine;
                            //清空接收缓存
                            comm.DiscardInBuffer();
                            return;
                        }
                        else if (buf[5] == 0xF1)//异常情况 “固件重复更新”
                        {
                            textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "ERROR固件重复更新" + Packet_Cnt + "/" + sliceNumber + Environment.NewLine;
                            //清空接收缓存
                            comm.DiscardInBuffer();
                            return;
                        }
                        else if (buf[5] == 0xF2)//异常情况 “当前包错误 需重传”
                        {
                            textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "ERROR当前包错误需重传" + Packet_Cnt + "/" + sliceNumber + Environment.NewLine;
                            //清空接收缓存
                            comm.DiscardInBuffer();
                            //直接沿用上一次生成的发送数据  3.2、向串口comm写数据，发送
                            comm.Write(byteTransmitData, 0, byteTransmitData.Length);
                            return;
                        }  

                        return;
                        break;


                    //收到 固件更新成功包，Sever端应该清除临时数据，进入下一个等待循环
                    case Constants.FW_SUCCESS_PKT:
                        textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "收到固件更新成功包" + Environment.NewLine;
                        //接收记数清零
                        sliceReceiveCount = 0;
                        //清空接收缓存
                        comm.DiscardInBuffer();
                        //当前状态置为“尚未传输”
                        isTransmitting = false;
                        return;
                        break;

                    default: 
                        textBox_msgInfo.Text += "ERROR：串口" + portNumber.SelectedItem.ToString() + "收到未能识别的数据包" + Environment.NewLine;
                        break;

                }              

            }
            catch(Exception)
            {
                Console.WriteLine("comm_DataReceived Exception");
            }
            finally
            {
                //清空接收缓存
                comm.DiscardInBuffer();
                
            }
        }
        

        //响应“打开串口”按钮的触发事件
        private void openPortBt_Click(object sender, EventArgs e)
        {
            //在打开串口之前需要先检查 是否已经导入所要传送的文件
            if (openPortBt.Text == "打开串口")
            {
                //检查文件是否准备好
                if (fileName == null)
                {
                    //尚未导入文件
                    MessageBox.Show("请先 导入待传输的文件");
                    return;
                }
                else
                {
                    try
                    {
                        //输入流
                        inputFileStream = File.OpenRead(fileName);                        
                        //byte数组
                        byteFileData = new byte[inputFileStream.Length];
                        //读入到byte数组中
                        if (inputFileStream.Read(byteFileData, 0, byteFileData.Length) > 0)
                        {
                            //计算分片个数
                            sliceNumber = byteFileData.Length % Constants.SLICE_SIZE == 0 ? byteFileData.Length / Constants.SLICE_SIZE : byteFileData.Length / Constants.SLICE_SIZE + 1;
                            //最后一个分片中的有效字节数
                            preamble_Pad_Num = byteFileData.Length % Constants.SLICE_SIZE;
                            textBox_msgInfo.Text += "文件：" + fileName + "已经打开" + Environment.NewLine;
                        }
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("文件读取出现异常");
                        return;
                    }
                    finally
                    {
                        inputFileStream.Close();
                    }
                }
                //打开串口
                try
                {
                    //设置串口号
                    comm.PortName = portNumber.SelectedItem.ToString();
                    //波特率
                    comm.BaudRate = Convert.ToInt32(baudRate.SelectedItem.ToString());
                    //校验位——获取或设置奇偶校验检查协议
                    switch (parity.SelectedIndex)
                    {
                        case 0:
                            comm.Parity = Parity.None;
                            break;
                        case 1:
                            comm.Parity = Parity.Odd;
                            break;
                        case 2:
                            comm.Parity = Parity.Even;
                            break;
                        case 3:
                            comm.Parity = Parity.Mark;
                            break;
                        case 4:
                            comm.Parity = Parity.Space;
                            break;
                    }
                    //数据位——
                    comm.DataBits = Convert.ToInt32(dataBits.SelectedItem.ToString());
                    //停止位
                    switch (stopBits.SelectedIndex)
                    {
                        case 0:
                            comm.StopBits = StopBits.One;
                            break;
                        case 1:
                            comm.StopBits = StopBits.Two;
                            break;
                        case 2:
                            comm.StopBits = StopBits.OnePointFive;
                            break;
                    }
                    //串口设置完成，此时可以打开串口了
                    comm.Open();

                    //串口打开，更改指示灯状态为 绿色
                    portStateColor.BackColor = Color.LawnGreen;
                    //串口配置中的下拉选框 状态置为 不可选
                    portNumber.Enabled = false;
                    baudRate.Enabled = false;
                    parity.Enabled = false;
                    dataBits.Enabled = false;
                    stopBits.Enabled = false;


                    //在msgInfo窗口输出信息
                    textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "已经打开" + Environment.NewLine;
                    openPortBt.Text = "关闭串口";
                }
                catch(Exception)
                {
                    MessageBox.Show("串口打开失败");
                }
            }
            else //当前串口已经是处于“打开状态”,再次点击按钮 将关闭串口
            {
                //关闭串口
                comm.Close();
                //串口状态指示灯颜色改为 红色
                portStateColor.BackColor = Color.Red;
                //使能 串口配置 中的下拉框
                portNumber.Enabled = true;
                baudRate.Enabled = true;
                parity.Enabled = true;
                dataBits.Enabled = true;
                stopBits.Enabled = true;
                //在msgInfo窗口输出信息
                textBox_msgInfo.Text += "串口" + portNumber.SelectedItem.ToString() + "已经关闭" + Environment.NewLine;
                openPortBt.Text = "打开串口";
            }
        }


        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        //导入文件 按钮 事件处理
        private void openFileBt_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "bin文件|*.bin";
            //openFile.ShowDialog(); 加上这行代码反而 会让 对话框弹出两次 =，=b
            //将文件路径赋值给 textBox_fileDir 以显示在面板上
            if(openFile.ShowDialog()==DialogResult.OK)
            {
                //将对话框得到的文件名赋值给 全局变量名 fileName
                fileName = openFile.FileName;
                textBox_fileDir.Text = openFile.FileName;                
            }
            

        }

        //将收到的byte数据转换成16进制的字符串
        public string byteArray2String0x(byte[] buf)
        {
            
            string receivedData = "";
            for (int i = 0; i < buf.Length; i++)
            {
                if (i == buf.Length - 1)
                {
                    receivedData += "0x" + Convert.ToString(buf[i], 16);
                    break;
                }
                receivedData += "0x" + Convert.ToString(buf[i], 16) + "-";
            }
            return receivedData;
        }
        public string byteArray2String0x(byte[] buf,int begin,int length)
        {

            string receivedData = "";
            for (int i = begin; i < length; i++)
            {
                if (i == length - 1)
                {
                    receivedData += "0x" + Convert.ToString(buf[i], 16);
                    break;
                }
                receivedData += "0x" + Convert.ToString(buf[i], 16) + "-";
            }
            return receivedData;
        }

        private void textBox_msgInfo_TextChanged(object sender, EventArgs e)
        {
            textBox_msgInfo.Focus();//获取焦点
            textBox_msgInfo.Select(textBox_msgInfo.Text.Length, 0);//光标定位到文本最后
            textBox_msgInfo.ScrollToCaret();//滚动到光标处
        }

        
    }

    //定义IWB协议所使用的常量
    public class Constants
    {
        //当前server的固件版本号，不同的导入文件也就对应着不同的固件版本，所以需要从导入的bin文件的文件名中 截取出来
        //public const byte[] Firmwire_Rev = new byte[2]{}; 这个定义在全局变量中
        //当前server的硬件版本号
        public const byte Hardware_Rev = 0xA2;
        
        //固件更新请求包
        public const byte FW_REQUEST_PKT = 0xF0;
        public const int FW_REQUEST_PKT_LEN = 0x000C;
        //固件更新初始包
        public const byte FW_PREAMBLE_PKT = 0xF1;
        //固件更新初始包例子{ 0x00, 0x0C,   0xF1,  0x00, 0x01,  0xA2,   0x00, 0x11,     0x00, 0x0C,     0x00, 0x00,    0xFF,   0x0D };
        


        //固件更新数据包
        public const byte FW_DATA_PKT = 0xF2;
        //固件更新数据包分片大小  对应协议中的Firmware_Data，为1024个byte
        public const int SLICE_SIZE = 1024;        
        //固件更新 包头Head所占字节数
        public const int FW_PKT_HEAD_LEN = 2;
        //固件更新数据包HEAD后面的长度 
        public const int FW_DATA_PKT_LEN_EXCEPT_HEAD = 1029;
        //经过包装的 分片数据包 总长度 = HEAD + HEAD后面的长度  即1031个byte
        public const int FW_DATA_PKT_TRANSMIT_TOTAL_LEN = FW_DATA_PKT_LEN_EXCEPT_HEAD + FW_PKT_HEAD_LEN;
        //固件更新数据包 ACK_Request 是否需要ACK回应
        public const byte FW_DATA_PKT_ACK_REQUEST = 0xFF;
        //固件更新数据包 Check_Sum 前若干字节异或校验值
        public const byte FW_DATA_PKT_CHECK_SUM = 0x0D;

        //固件更新回应包
        public const int FW_RESPONSE_PKT = 0xF3;
        public const int FW_RESPONSE_PKT_LEN = 0x06;        
        public const int RESPONSE_OK = 1;
        public const int HW_REV_ERROR = 0xF0;
        public const int FW_REV_SAME = 0xF1;
        //固件更新成功包
        public const int FW_SUCCESS_PKT = 0xF4;
        public const int FW_SUCCESS_PKT_LEN = 0x03;
        
    }
}
