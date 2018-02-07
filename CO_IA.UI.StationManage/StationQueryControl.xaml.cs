using CO_IA.Data;
using CO_IA_Data;
using I_CO_IA.StationManage;
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

namespace CO_IA.UI.StationManage
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class StationQueryControl : UserControl
    {
        public Action<List<StationInfo>> GetStationFinished;
        public Action<StationInfo> LocationStation;
        public StationQueryControl()
        {
            InitializeComponent();
            
        }
        public List<StationInfo> StationItemsSource
        {
            get
            {
                if (stationdatagrid.ItemsSource == null)
                {
                    return null;
                }
                else
                {
                    return stationdatagrid.ItemsSource as List<StationInfo>;
                }
            }
            set { stationdatagrid.ItemsSource = value; }
        }
        public StationQueryControl(Point[] pList, Point pLeftTop, Point pRightBottom,string netSvn)
        {
            InitializeComponent();
            //调用服务
            GetStationQuery(pList, pLeftTop, pRightBottom, netSvn);
        }
        public void GetStationQuery(Point[] pList, Point pLeftTop, Point pRightBottom, string netSvn)
        {
            List<StationInfo> statinfoLs = new List<StationInfo>();
            List<QUERY_PARAMS> lsParams = new List<QUERY_PARAMS>();
            QUERY_PARAMS param = null;
            param = new QUERY_PARAMS();
            param.PARAMS_NAME = "通信业务";
            param.PARAMS_RELATION = "";
            param.PARAMS_VALUE = netSvn;
            param.PARAMS_UNIT = "";
            param.PARAMS_TYPE = "varchar";
            lsParams.Add(param);
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_StationManage>(channel =>
            {
                statinfoLs = channel.GetStationBaseInfo(lsParams, pList, pLeftTop, pRightBottom);
                stationdatagrid.ItemsSource = statinfoLs;
                if (GetStationFinished != null)
                {
                    GetStationFinished(statinfoLs);
                }
            });
        }
        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stationdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //查询结果列表中台站guid
            StationInfo selectstation = this.stationdatagrid.SelectedItem as StationInfo;
            StationDetailDialog dialog = new StationDetailDialog(selectstation.STATGUID);
            dialog.ShowDialog(this);
        }
        private void DrawStation(List<StationInfo> stationList)
        {
            if (stationList != null && stationList.Count > 0)
            {
                for (int i = 0; i < stationList.Count; i++)
                {
                    StationInfo info = stationList[i];
                }
            }
        }
    }

}
