using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO_IA.Client
{
    /// <summary>
    /// SerialPortSettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortSettingDialog : Window
    {
        public SerialPortSettingDialog()
        {
            InitializeComponent();
            

        }
        private void Ini()
        {
            var portNames = System.IO.Ports.SerialPort.GetPortNames();
            this.comboBoxPortNames.ItemsSource = portNames;
            if (portNames.Length > 0)
            {
                this.comboBoxPortNames.SelectedIndex = 0;
            }
            this.comboBoxStopBits.ItemsSource = this.GetStopBitsDic();
            this.comboBoxParity.ItemsSource = this.GetParityDic();
            this.comboBoxStopBits.SelectedIndex = 0;
            this.comboBoxParity.SelectedIndex = 0;
        }
        private Dictionary<System.IO.Ports.StopBits, string> GetStopBitsDic()
        {
            Dictionary<System.IO.Ports.StopBits, string> dic = new Dictionary<System.IO.Ports.StopBits, string>();
            dic.Add(System.IO.Ports.StopBits.None, "无");
            dic.Add(System.IO.Ports.StopBits.One, "一个停止位");
            dic.Add(System.IO.Ports.StopBits.Two, "两个停止位");
            dic.Add(System.IO.Ports.StopBits.OnePointFive, "1.5 个停止位");
            return dic;
        }

        private Dictionary<System.IO.Ports.Parity, string> GetParityDic()
        {
            Dictionary<System.IO.Ports.Parity, string> dic = new Dictionary<System.IO.Ports.Parity, string>();
            dic.Add(System.IO.Ports.Parity.None, "不校验");
            dic.Add(System.IO.Ports.Parity.Odd, "奇校验");
            dic.Add(System.IO.Ports.Parity.Even, "偶校验");
            dic.Add(System.IO.Ports.Parity.Mark, "校验位为1");
            dic.Add(System.IO.Ports.Parity.Space, "校验位为0");
            return dic;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            var value = this.comboBoxParity.SelectedValue;
            DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public SerialPortSetting GetSetting()
        {
            SerialPortSetting setting = new SerialPortSetting();
            if (g_sensor.Visibility == System.Windows.Visibility.Visible)
            {
                setting.IP = textBoxIP.Text.Trim();
                int port = 0;
                if(int.TryParse(textBoxPort.Text.Trim(),out port))
                {
                
                }
                setting.Port = port;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(this.comboBoxPortNames.Text))
                {
                    throw new Exception("请选择使用的端口名称!");
                }
                setting.PortName = this.comboBoxPortNames.Text;
                int baudRate;
                if (!int.TryParse(this.textBoxBaudRate.Text, out baudRate) || baudRate <= 0)
                {
                    throw new Exception("波特率应该为大于0的整数");
                }
                setting.BaudRate = baudRate;
                setting.Parity = (Parity)this.comboBoxParity.SelectedValue;
                setting.DataBits = int.Parse(this.comboBoxDataBits.Text.ToString());
                setting.StopBits = (StopBits)this.comboBoxStopBits.SelectedValue;
            }
            return setting;
        }

        public void SetSetting(SerialPortSetting setting)
        {
            if (string.IsNullOrEmpty(setting.IP))
            {
                g_sensor.Visibility = System.Windows.Visibility.Collapsed;
                g_com.Visibility = System.Windows.Visibility.Visible;
                this.Height = 200;
                Ini();
                this.comboBoxPortNames.SelectedValue = setting.PortName;
                this.textBoxBaudRate.Text = setting.BaudRate.ToString();
                this.comboBoxStopBits.SelectedValue = setting.StopBits;
                this.comboBoxParity.SelectedValue = setting.Parity;
                this.comboBoxDataBits.Text = setting.DataBits.ToString();

            }
            else
            {
                this.Height = 150;
                g_com.Visibility = System.Windows.Visibility.Collapsed;
                g_sensor.Visibility = System.Windows.Visibility.Visible;
                textBoxIP.Text = setting.IP;
                textBoxPort.Text = setting.Port.ToString();
            }
        }

    }

    public class SerialPortSetting
    {
        public string IP
        {
            get;
            set;
        }
        public int Port
        {
            get;
            set;
        }
        public string PortName
        {
            get;
            set;
        }

        public int BaudRate
        {
            get;
            set;
        }

        public int DataBits 
        {
            get; 
            set;         
        }

        public Parity Parity 
        {
            get;
            set; 
        }

        public StopBits StopBits 
        {
            get;
            set; 
        }
    }
}
