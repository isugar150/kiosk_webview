using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace kiosk
{
    public partial class Form1 : Form
    {
        public static iniProperties IniProperties = new iniProperties();

        public Form1()
        {
            InitializeComponent();
        }

        // init
        private void Form1_Load(object sender, EventArgs e)
        { 
            bool disabledComPort = false;
            comboBox1.DataSource = SerialPort.GetPortNames();

            try
            {
                IniFile pairs = new IniFile();
                if (new FileInfo(Application.StartupPath + "/kiosk.ini").Exists)
                {
                    pairs.Load(Application.StartupPath + "/kiosk.ini");
                    IniProperties.serialPort = pairs["kiosk"]["serialPort"].ToString();
                    IniProperties.autoStart = pairs["kiosk"]["autoStart"].ToString();

                    appendRichText("설정파일을 불러왔습니다.");
                }
            }
            catch (Exception ex) { MessageBox.Show("설정 파일 초기화 중 오류가 발생하였습니다. " + ex.Message); }

            if(!String.IsNullOrEmpty(IniProperties.serialPort)) 
            {
                if (comboBox1.Items.IndexOf(IniProperties.serialPort) == -1)
                {
                    disabledComPort = true;
                    appendRichText("설정된 COM포트가 검색되지 않았습니다.");
                    comboBox1.SelectedIndex = 0;
                }
                else
                {
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(IniProperties.serialPort);
                }
            }
            else
            {
                comboBox1.SelectedIndex = 0;
            }
            if (!String.IsNullOrEmpty(IniProperties.autoStart) && IniProperties.autoStart.Equals("Y"))
            {
                checkBox1.Checked = true;
                if (disabledComPort)
                {
                    appendRichText("설정된 COM포트가 검색되지 않았으므로 자동실행 기능을 실행하지 않았습니다.");
                } else
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        Hide();
                    }));

                    button4.PerformClick();
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = 9600;
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

                serialPort1.Open();

                appendRichText(comboBox1.Text + "포트가 열렸습니다.");
                comboBox1.Enabled = false;
            }
            else
            {
                appendRichText(comboBox1.Text + "포트가 이미 열려 있습니다.");
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(10);
            this.Invoke(new EventHandler(MySerialReceived));
        }
        private void MySerialReceived(object s, EventArgs e)
        {
            byte[] data = new byte[serialPort1.BytesToRead];
            serialPort1.Read(data, 0, data.Length);
            string text = Encoding.Default.GetString(data).Trim();
            if (!String.IsNullOrEmpty(text))
            {
                appendRichText(text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();

                appendRichText(comboBox1.Text + "포트가 닫혔습니다.");
                comboBox1.Enabled = true;
            }
            else
            {
                appendRichText(comboBox1.Text + "포트가 이미 닫혀 있습니다.");
            }
        }
        private void appendRichText(byte[] data)
        {
            richTextBox1.Text = richTextBox1.Text + "Recive Data: " + Encoding.Default.GetString(data).Trim() + "\n";
        }

        private void appendRichText(string text)
        {
            richTextBox1.Text = richTextBox1.Text + text + "\n";
        }

        // 설정 저장 버튼
        private void button3_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen) button2.PerformClick();
            IniFile setting = new IniFile();

            setting["kiosk"]["serialPort"] = comboBox1.Text;
            setting["kiosk"]["autoStart"] = checkBox1.Checked ? "Y" : "n";

            setting.Save(Application.StartupPath + "/kiosk.ini");
            appendRichText("설정파일을 저장했습니다.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(serialPort1.IsOpen) button2.PerformClick();
            this.Hide();
            Form2 form2 = new Form2(this, comboBox1.Text);
            form2.Show();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            comboBox1.DataSource = SerialPort.GetPortNames();
            if (comboBox1.Items.IndexOf(IniProperties.serialPort) == -1)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(IniProperties.serialPort);
            }
            appendRichText("COM 포트 목록을 새로고침하였습니다.");
        }
    }
}
