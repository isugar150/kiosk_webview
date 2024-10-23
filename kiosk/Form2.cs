using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace kiosk
{
    public partial class Form2 : Form
    {
        Form super = null;
        string serialPort;
        public Form2(Form super, string serialPort)
        {
            InitializeComponent();
            this.super = super;
            this.serialPort = serialPort;
            this.Resize += new EventHandler(this.Form_Resize);
        }

        private void Form_Resize(object sender, EventArgs e)
        {
            webView.Size = this.ClientSize - new Size(webView.Location);
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            super.Show();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            await webView.EnsureCoreWebView2Async();

            if (webView != null && webView.CoreWebView2 != null)
            {
                //string text = System.IO.File.ReadAllText(@"./assets/index.html");
                //webView.CoreWebView2.NavigateToString(text);
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeFolder = Path.GetDirectoryName(exePath);
                string websiteFolder = Path.Combine(exeFolder, "assets");
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("kiosk", websiteFolder, CoreWebView2HostResourceAccessKind.DenyCors);
                //webView.CoreWebView2.Navigate("https://kiosk/index.html"); // 가상 호스팅 방식 - 속도 이슈로인해 파일방식으로 변경
                webView.CoreWebView2.Navigate("file:///" + websiteFolder + "/index.html"); // 파일 방식
            }

            serialPort1.PortName = serialPort;
            serialPort1.BaudRate = 9600;
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;
            serialPort1.Parity = Parity.None;
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

            serialPort1.Open();
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
                webView.CoreWebView2.PostWebMessageAsString(text);
            }
        }
    }
}
