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
using BeiLiNu.Ui.Controls.WPF.Windows;

namespace Best.VWPlatform.Assist
{
    /// <summary>
    /// AssistWnd.xaml 的交互逻辑
    /// </summary>
    public partial class AssistWnd : XWindowBase
    {
        public AssistWnd()
        {
            InitializeComponent();
            this.Loaded += AssistWnd_Loaded;
        }

        void AssistWnd_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void xCloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public void WriteLine(string pStr)
        {
            xTbOutput.AppendText(pStr + "\n");
        }

        private void xBtnClear_Click(object sender, RoutedEventArgs e)
        {
            xTbOutput.Clear();
        }
    }
}
