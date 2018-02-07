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

namespace CO_IA.UI.Scene
{
    /// <summary>
    /// ConnectEquipment.xaml 的交互逻辑
    /// </summary>
    public partial class RealTimeMonitoring : UserControl
    {
        public RealTimeMonitoring()
        {
            InitializeComponent();
            xBtnBack.Click += xBtnBack_Click;
        }

        private void xBtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.GoBack != null)
            {
                GoBack();
            }
        }

        public Action GoBack;
    }
}
