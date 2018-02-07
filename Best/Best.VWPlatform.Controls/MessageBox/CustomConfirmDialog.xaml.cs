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
using BeiLiNu.Ui.Controls.WPF.Windows;

namespace Best.VWPlatform.Controls.MessageBox
{
    /// <summary>
    /// CustomConfirmDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CustomConfirmDialog : XWindowBase
    {
        public EResultType ConfirmResult { get;private set; }
        public CustomConfirmDialog(string pMessage, params string[] ps)
        {
            InitializeComponent();

            if (ps != null)
            {
                xConfirmMsg.Text = string.Format(pMessage, ps);
            }
            else
            {
                xConfirmMsg.Text = pMessage;
            }
            this.Owner = Application.Current.MainWindow;
        }

        private void xOK_Click(object sender, RoutedEventArgs e)
        {
            ConfirmResult = EResultType.OK;
            this.Close();
        }

        private void xCancel_Click(object sender, RoutedEventArgs e)
        {
            ConfirmResult = EResultType.Cancel;
            this.Close();
        }

        private void xCloseButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmResult = EResultType.Cancel;
            this.Close();
        }
    }
}
