using I_CO_IA.Setting;
using PT_BS_Service.Client.Framework;
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

namespace CO_IA.UI.Setting
{
    /// <summary>
    /// MonitorStationTypeModule.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorEQUTypeModule : UserControl
    {
        public MonitorEQUTypeModule()
        {
            InitializeComponent();
            QueryType();
        }

        private void QueryType()
        {
            BeOperationInvoker.Invoke<I_CO_IA_Setting, List<string>>(channel =>
           {
               List<string> types =
                channel.GetMonitorEQUType();
               typegrid.ItemsSource = types;
               return types;

           });
        }
    }
}
