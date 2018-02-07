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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.ActivitySummarize
{
    /// <summary>
    /// HonorWall.xaml 的交互逻辑
    /// </summary>
    public partial class HonorWall : UserControl
    {
        public HonorWall()
        {
            InitializeComponent();
        }

        public HonorWall(int mode)
        {
            InitializeComponent();

            tb_Modify.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
