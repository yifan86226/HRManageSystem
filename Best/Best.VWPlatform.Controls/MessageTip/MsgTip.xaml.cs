using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Best.VWPlatform.Controls.MessageTip
{
    /// <summary>
    /// MsgTip.xaml 的交互逻辑
    /// </summary>
    public partial class MsgTip : Window
    {
        DispatcherTimer _timer = null;
        public MsgTip(string pMsg)
        {
            InitializeComponent();
            xTipTextBlock.Text = pMsg;
            this.Loaded += MsgTip_Loaded;
        }

        void MsgTip_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (xTipTextBlock.Opacity > 0.7)
            {
                xTipTextBlock.Opacity = xTipTextBlock.Opacity - 0.1;
            }
            else if (xTipTextBlock.Opacity > 0.3)
            {
                xTipTextBlock.Opacity = xTipTextBlock.Opacity - 0.2;
            }
            else
            {
                this.Close();
            }        
        }
    }
}
