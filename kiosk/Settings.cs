using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kiosk
{
    internal interface iniproperties
    {
        string serialPort { get; set; }
        string autoStart { get; set; }
    }

    public class iniProperties : iniproperties
    {
        private string _serialPort;
        private string _autoStart;
        public string serialPort { get { return _serialPort; } set { _serialPort = value; } }
        public string autoStart { get { return _autoStart; } set { _autoStart = value; } }
    }

    public class Setting
    {
    }
}
