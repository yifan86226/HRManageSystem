using System;
using System.Collections.Generic;
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

namespace CO_IA.UI.RemoteScreenCapturer.Tools
{
    /// <summary>
    /// CapSeleZone.xaml 的交互逻辑
    /// </summary>
    public partial class CapSeleZone : Window
    {

        public delegate void GetPosition(Point p,double width,double height );
        public GetPosition getPosition;

        public CapSeleZone()
        {
            InitializeComponent();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)//双击退出
            {
                //this.Hide();
                //MessageBox.Show(this.Top.ToString()+"-"+this.Left.ToString());
                //记录抓屏区域到配置文件
                this.Close();
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (getPosition != null)
            {
                Point point = new Point(Left, Top);
                getPosition(point, Width, Height);
            }
        }
    }
}
