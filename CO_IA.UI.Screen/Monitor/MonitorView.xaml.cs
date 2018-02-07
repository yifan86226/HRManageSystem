#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：历史监测
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion
using CO_IA.Data;
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
using CO_IA.Data.MonitorPlan;
using CO_IA.UI.Collection;
using CO_IA.UI.StationManage;

namespace CO_IA.UI.Screen.Monitor
{
    /// <summary>
    /// RecordHistory.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorView : Window
    {        

        RealTimeMonitorDataShowNew realTimeMonitorDataShow;

        /// <summary>
        /// 仅用于历史监测
        /// </summary>
        /// <param name="_activityPlaceInfo"></param>
        public MonitorView(ActivityPlaceInfo _activityPlaceInfo)
        {
            InitializeComponent();
            g_monitor.Width = 0;
            if (_activityPlaceInfo == null)
                return;

            DataAanalysis dataAanalysis = new Collection.DataAanalysis(_activityPlaceInfo);
            dataAanalysis.EnTryMode = CO_IA.Data.Collection.EnTryModeEnum.管理端;
            dataAanalysis.ControlDisplay();
            dataAanalysis.StationHyperlinkClick += dataAanalysis_StationHyperlinkClick;
            dataAanalysis.SetValue(Grid.ColumnProperty, 1);
            jc_fullpanel.Children.Add(dataAanalysis);

        }
        void dataAanalysis_StationHyperlinkClick(string p_stationGuid)
        {
            if (string.IsNullOrEmpty(p_stationGuid))
                return;

            StationDetailDialog dialog = new StationDetailDialog(p_stationGuid);
            dialog.ShowDialog(this);
        }
        /// <summary>
        /// 现场监测
        /// </summary>
        /// <param name="monitorPlan"></param>
        public MonitorView(string areaid,List<PP_OrgInfo> orgs)
        {
            InitializeComponent();
            g_monitor.Width = 150;

            MonitorPlanInfo[]  monitorPlan = GetMonitorFreqInfos(Obj.Activity.Guid, areaid);
            if (monitorPlan == null || monitorPlan.Length == 0)
            {
                MessageBox.Show("该区域的监测预案为空，没有监测数据！");
                return;
            }
            if (orgs == null || orgs.Count == 0)
            {
                MessageBox.Show("该区域没有小组信息，没有监测数据！");
                return;
            }
            lstMonitor.ItemsSource = orgs;

            realTimeMonitorDataShow = new RealTimeMonitorDataShowNew(monitorPlan.ToList());
            realTimeMonitorDataShow.SetValue(Grid.ColumnProperty, 1);
            jc_fullpanel.Children.Add(realTimeMonitorDataShow);

        }
       
        private MonitorPlanInfo[] GetMonitorFreqInfos(string p_activityGuid, string p_placeGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, MonitorPlanInfo[]>(channel =>
            {
                return channel.GetMonitorPlansByPlace(p_activityGuid, p_placeGuid);
            });
        }

        private void lstMonitor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstMonitor.SelectedItem == null)
                return;
            PP_OrgInfo orginfo = lstMonitor.SelectedItem as PP_OrgInfo;
            if (orginfo == null)
                return;

            g_monitor.Visibility = System.Windows.Visibility.Visible;

            //组ID+监测预案信息
            realTimeMonitorDataShow.SetCondition(orginfo.GUID);
        }

    }

}
