using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace CO_IA.MonitoringCollecting.Views
{
    public partial class AroundStationView : UserControl
    {
        public AroundStationView()
        {
            InitializeComponent();
            LoadAroundStation();
        }

        private void LoadAroundStation()
        {
            //List<StationBase> stationList = SQLiteDataService.QueryStatBaseByPlaceID(SystemLoginService.CurrentActivityPlace.Guid);
            //List<StationInfo> stationInfoList = new List<StationInfo>();
            //foreach (StationBase statB in stationList)
            //{
            //    stationInfoList.Add(ToStationInfo(statB));
            //}
            //RoundStatAnalyse_Control roundStat = new RoundStatAnalyse_Control();
            //roundStat.StationItemsSource = stationInfoList;
            //MapLoad_Control mapLoad = new MapLoad_Control(roundStat);
            //LayOutRoot.Children.Clear();
            //LayOutRoot.Children.Add(mapLoad);

        }

        //private StationInfo ToStationInfo(StationBase p_statBase)
        //{
        //    StationInfo statInfo = new StationInfo();
        //    statInfo.APP_CODE = p_statBase.AppCode;
        //    //statInfo.APPGUID = 
        //    //statInfo.FREQ_LC
        //    //statInfo.FREQ_UC
        //    statInfo.NET_SVN = p_statBase.NetSvn;
        //    statInfo.ORG_NAME = p_statBase.OrgName;
        //    //statInfo.ORGSYSCODE 
        //    //statInfo.Power
        //    statInfo.STAT_ADDR = p_statBase.StatAddr;
        //    statInfo.STAT_APP_TYPE = p_statBase.StatAppType;
        //    statInfo.STAT_NAME = p_statBase.StatName;
        //    statInfo.STAT_TDI = p_statBase.StatTdi;
        //    //statInfo.STATGUID 

        //    //缺少 经度，纬度，地点GUID
        //    return statInfo;
        //}

    }
}
