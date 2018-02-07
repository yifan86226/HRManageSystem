using BeiLiNu.Ui.Controls.WPF.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls.MessageBox
{
    /// <summary>
    /// CustomAlertDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CustomAlertDialog : XWindowBase
    {
        public CustomAlertDialog(string pMessage, params string[] ps)
        {
            InitializeComponent();

            if (ps != null)
            {
                xAlertMsg.Text = string.Format(pMessage, ps);
            }
            else
            {
                xAlertMsg.Text = pMessage;
            }

            this.Owner = Application.Current.MainWindow;            
        }

        private void xOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void xCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
