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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO.IA.UI.TaskManage.Rules
{
    /// <summary>
    /// PleaseWait.xaml 的交互逻辑
    /// </summary>
    public partial class PleaseWait : Window
    {
        public PleaseWait()
        {
            InitializeComponent();
           // Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 20 });
        }
    }
}
