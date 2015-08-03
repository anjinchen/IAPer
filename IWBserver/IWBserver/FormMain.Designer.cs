namespace IWBserver
{
    partial class FormMain
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.portConfig = new System.Windows.Forms.GroupBox();
            this.portStateColor = new System.Windows.Forms.Panel();
            this.openPortBt = new System.Windows.Forms.Button();
            this.baudRate = new System.Windows.Forms.ComboBox();
            this.stopBits = new System.Windows.Forms.ComboBox();
            this.dataBits = new System.Windows.Forms.ComboBox();
            this.parity = new System.Windows.Forms.ComboBox();
            this.portNumber = new System.Windows.Forms.ComboBox();
            this.label_stopBits = new System.Windows.Forms.Label();
            this.label_dataBits = new System.Windows.Forms.Label();
            this.label_parity = new System.Windows.Forms.Label();
            this.label_baudRate = new System.Windows.Forms.Label();
            this.label_portNumber = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox_msgInfo = new System.Windows.Forms.GroupBox();
            this.textBox_msgInfo = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.openFileBt = new System.Windows.Forms.Button();
            this.textBox_fileDir = new System.Windows.Forms.TextBox();
            this.portConfig.SuspendLayout();
            this.groupBox_msgInfo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // portConfig
            // 
            this.portConfig.Controls.Add(this.portStateColor);
            this.portConfig.Controls.Add(this.openPortBt);
            this.portConfig.Controls.Add(this.baudRate);
            this.portConfig.Controls.Add(this.stopBits);
            this.portConfig.Controls.Add(this.dataBits);
            this.portConfig.Controls.Add(this.parity);
            this.portConfig.Controls.Add(this.portNumber);
            this.portConfig.Controls.Add(this.label_stopBits);
            this.portConfig.Controls.Add(this.label_dataBits);
            this.portConfig.Controls.Add(this.label_parity);
            this.portConfig.Controls.Add(this.label_baudRate);
            this.portConfig.Controls.Add(this.label_portNumber);
            this.portConfig.Location = new System.Drawing.Point(21, 33);
            this.portConfig.Name = "portConfig";
            this.portConfig.Size = new System.Drawing.Size(163, 245);
            this.portConfig.TabIndex = 2;
            this.portConfig.TabStop = false;
            this.portConfig.Text = "串口配置";
            // 
            // portStateColor
            // 
            this.portStateColor.BackColor = System.Drawing.Color.Red;
            this.portStateColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.portStateColor.Location = new System.Drawing.Point(26, 214);
            this.portStateColor.Name = "portStateColor";
            this.portStateColor.Size = new System.Drawing.Size(29, 19);
            this.portStateColor.TabIndex = 11;
            // 
            // openPortBt
            // 
            this.openPortBt.Location = new System.Drawing.Point(74, 211);
            this.openPortBt.Name = "openPortBt";
            this.openPortBt.Size = new System.Drawing.Size(75, 23);
            this.openPortBt.TabIndex = 10;
            this.openPortBt.Text = "打开串口";
            this.openPortBt.UseVisualStyleBackColor = true;
            this.openPortBt.Click += new System.EventHandler(this.openPortBt_Click);
            // 
            // baudRate
            // 
            this.baudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.baudRate.FormattingEnabled = true;
            this.baudRate.Items.AddRange(new object[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "28800",
            "38400",
            "56000",
            "57600",
            "115200",
            "128000",
            "256000"});
            this.baudRate.Location = new System.Drawing.Point(74, 61);
            this.baudRate.Name = "baudRate";
            this.baudRate.Size = new System.Drawing.Size(72, 20);
            this.baudRate.TabIndex = 4;
            // 
            // stopBits
            // 
            this.stopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stopBits.FormattingEnabled = true;
            this.stopBits.Items.AddRange(new object[] {
            "One",
            "Two",
            "OnePointFive"});
            this.stopBits.Location = new System.Drawing.Point(74, 169);
            this.stopBits.Name = "stopBits";
            this.stopBits.Size = new System.Drawing.Size(72, 20);
            this.stopBits.TabIndex = 9;
            // 
            // dataBits
            // 
            this.dataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dataBits.FormattingEnabled = true;
            this.dataBits.Items.AddRange(new object[] {
            "8",
            "7",
            "6",
            "5"});
            this.dataBits.Location = new System.Drawing.Point(74, 133);
            this.dataBits.Name = "dataBits";
            this.dataBits.Size = new System.Drawing.Size(72, 20);
            this.dataBits.TabIndex = 8;
            // 
            // parity
            // 
            this.parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parity.FormattingEnabled = true;
            this.parity.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.parity.Location = new System.Drawing.Point(74, 97);
            this.parity.Name = "parity";
            this.parity.Size = new System.Drawing.Size(72, 20);
            this.parity.TabIndex = 7;
            // 
            // portNumber
            // 
            this.portNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.portNumber.FormattingEnabled = true;
            this.portNumber.Items.AddRange(new object[] {
            "COM3",
            "COM1"});
            this.portNumber.Location = new System.Drawing.Point(74, 25);
            this.portNumber.Name = "portNumber";
            this.portNumber.Size = new System.Drawing.Size(72, 20);
            this.portNumber.TabIndex = 5;
            this.portNumber.Click += new System.EventHandler(this.portNumber_Click);
            // 
            // label_stopBits
            // 
            this.label_stopBits.AutoSize = true;
            this.label_stopBits.Location = new System.Drawing.Point(20, 174);
            this.label_stopBits.Name = "label_stopBits";
            this.label_stopBits.Size = new System.Drawing.Size(47, 12);
            this.label_stopBits.TabIndex = 4;
            this.label_stopBits.Text = "停止位:";
            // 
            // label_dataBits
            // 
            this.label_dataBits.AutoSize = true;
            this.label_dataBits.Location = new System.Drawing.Point(20, 138);
            this.label_dataBits.Name = "label_dataBits";
            this.label_dataBits.Size = new System.Drawing.Size(47, 12);
            this.label_dataBits.TabIndex = 3;
            this.label_dataBits.Text = "数据位:";
            // 
            // label_parity
            // 
            this.label_parity.AutoSize = true;
            this.label_parity.Location = new System.Drawing.Point(20, 102);
            this.label_parity.Name = "label_parity";
            this.label_parity.Size = new System.Drawing.Size(47, 12);
            this.label_parity.TabIndex = 2;
            this.label_parity.Text = "校验位:";
            // 
            // label_baudRate
            // 
            this.label_baudRate.AutoSize = true;
            this.label_baudRate.Location = new System.Drawing.Point(20, 66);
            this.label_baudRate.Name = "label_baudRate";
            this.label_baudRate.Size = new System.Drawing.Size(47, 12);
            this.label_baudRate.TabIndex = 1;
            this.label_baudRate.Text = "波特率:";
            // 
            // label_portNumber
            // 
            this.label_portNumber.AutoSize = true;
            this.label_portNumber.Location = new System.Drawing.Point(20, 30);
            this.label_portNumber.Name = "label_portNumber";
            this.label_portNumber.Size = new System.Drawing.Size(47, 12);
            this.label_portNumber.TabIndex = 0;
            this.label_portNumber.Text = "串口号:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(190, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 29);
            this.label1.TabIndex = 12;
            this.label1.Text = "IAPer";
            // 
            // groupBox_msgInfo
            // 
            this.groupBox_msgInfo.Controls.Add(this.textBox_msgInfo);
            this.groupBox_msgInfo.Location = new System.Drawing.Point(21, 283);
            this.groupBox_msgInfo.Name = "groupBox_msgInfo";
            this.groupBox_msgInfo.Size = new System.Drawing.Size(471, 219);
            this.groupBox_msgInfo.TabIndex = 13;
            this.groupBox_msgInfo.TabStop = false;
            this.groupBox_msgInfo.Text = "消息区";
            this.groupBox_msgInfo.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // textBox_msgInfo
            // 
            this.textBox_msgInfo.BackColor = System.Drawing.Color.White;
            this.textBox_msgInfo.Location = new System.Drawing.Point(6, 20);
            this.textBox_msgInfo.Multiline = true;
            this.textBox_msgInfo.Name = "textBox_msgInfo";
            this.textBox_msgInfo.ReadOnly = true;
            this.textBox_msgInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_msgInfo.Size = new System.Drawing.Size(459, 193);
            this.textBox_msgInfo.TabIndex = 0;
            this.textBox_msgInfo.TextChanged += new System.EventHandler(this.textBox_msgInfo_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.openFileBt);
            this.panel1.Controls.Add(this.textBox_fileDir);
            this.panel1.Location = new System.Drawing.Point(190, 50);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(296, 43);
            this.panel1.TabIndex = 14;
            // 
            // openFileBt
            // 
            this.openFileBt.Location = new System.Drawing.Point(215, 13);
            this.openFileBt.Name = "openFileBt";
            this.openFileBt.Size = new System.Drawing.Size(75, 23);
            this.openFileBt.TabIndex = 1;
            this.openFileBt.Text = "导入文件";
            this.openFileBt.UseVisualStyleBackColor = true;
            this.openFileBt.Click += new System.EventHandler(this.openFileBt_Click);
            // 
            // textBox_fileDir
            // 
            this.textBox_fileDir.Location = new System.Drawing.Point(13, 13);
            this.textBox_fileDir.Name = "textBox_fileDir";
            this.textBox_fileDir.ReadOnly = true;
            this.textBox_fileDir.Size = new System.Drawing.Size(195, 21);
            this.textBox_fileDir.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 514);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox_msgInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.portConfig);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.portConfig.ResumeLayout(false);
            this.portConfig.PerformLayout();
            this.groupBox_msgInfo.ResumeLayout(false);
            this.groupBox_msgInfo.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox portConfig;
        private System.Windows.Forms.Panel portStateColor;
        public System.Windows.Forms.Button openPortBt;
        private System.Windows.Forms.ComboBox baudRate;
        private System.Windows.Forms.ComboBox stopBits;
        private System.Windows.Forms.ComboBox dataBits;
        private System.Windows.Forms.ComboBox parity;
        private System.Windows.Forms.ComboBox portNumber;
        private System.Windows.Forms.Label label_stopBits;
        private System.Windows.Forms.Label label_dataBits;
        private System.Windows.Forms.Label label_parity;
        private System.Windows.Forms.Label label_baudRate;
        private System.Windows.Forms.Label label_portNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox_msgInfo;
        private System.Windows.Forms.TextBox textBox_msgInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button openFileBt;
        private System.Windows.Forms.TextBox textBox_fileDir;
    }
}

