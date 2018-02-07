using CO_IA.UI.Collection;
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

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// EmeMonitorAnalyse_Control.xaml 的交互逻辑
    /// </summary>
    public partial class EmeMonitorAnalyse_Control : UserControl
    {
        private CO_IA.Data.ActivityPlaceInfo activityPlaceInfo;

        public EmeMonitorAnalyse_Control()
        {
            InitializeComponent();
        }

        public EmeMonitorAnalyse_Control(CO_IA.Data.ActivityPlaceInfo activityPlaceInfo)
        {
            InitializeComponent();
            this.activityPlaceInfo = activityPlaceInfo;
            DataAanalysis dataAanalysis = new Collection.DataAanalysis(activityPlaceInfo);
            dataAanalysis.EnTryMode = CO_IA.Data.Collection.EnTryModeEnum.管理端;
            dataAanalysis.ControlDisplay();
            tbItem_DataAanalysis.Content = dataAanalysis;
        }
    }
}
