using CO_IA.Data;
using CO_IA.Data.Collection;
using CO_IA_Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CO_IA.UI.Collection.DbEntity
{
    public partial class SQLiteDataService
    {
        //public static List<StationBaseExt> QueryAroundStationWithEmits(string p_areaGuid)
        //{
        //    //List<StationBase> statList = QueryData<StationBase>(LoadStationBaseRow, new Func<string, string>(SQLiteDataService.GetQueryStatByPlaceSql), new object[] { p_areaGuid });

        //    //List<StationBaseExt> list = new List<StationBaseExt>();
        //    //foreach (StationBase stat in statList)
        //    //{
        //    //    StationBaseExt statExt = new StationBaseExt(stat);
        //    //    statExt.EmitInfoList.AddRange(QueryEmitInfoByStatID(stat.Guid));
        //    //    list.Add(statExt);
        //    //}
        //    //return list;
        //    return null;
        //}

        /// <summary>
        /// 根据地点查询周围台站
        /// </summary>
        /// <param name="p_placeID"></param>
        /// <returns></returns>
        public static List<ActivitySurroundStation> QueryStatBaseByPlaceID(string p_placeID)
        {
            return QueryData<ActivitySurroundStation>(LoadStationBaseRow, new Func<string, string>(SQLiteDataService.GetQueryStatByPlaceSql), new object[] { p_placeID });
        }

        /// <summary>
        /// 台站表数据加载
        /// </summary>
        /// <param name="p_dataRow"></param>
        /// <returns></returns>
        //private static StationBaseExt LoadStationBaseRow(DataRow p_dataRow)
        //{
        //    StationBase stat = new StationBase();
        //    stat.AppCode = p_dataRow["APP_CODE"].ToString();
        //    stat.Guid = p_dataRow["STAT_GUID"].ToString();
        //    stat.NetSvn = p_dataRow["NET_SVN"].ToString();
        //    //stat.OrgGuid = p_dataRow["ORG_GUID"].ToString();
        //    //stat.OrgLinkPerson = p_dataRow["ORG_LINK_PERSON"].ToString();
        //    stat.OrgName = p_dataRow["ORG_NAME"].ToString();
        //    //stat.OrgPhone = p_dataRow["ORG_PHONE"].ToString();
        //    stat.PlaceGuid = p_dataRow["PLACE_GUID"].ToString();
        //    stat.StatAddr = p_dataRow["STAT_ADDR"].ToString();
        //    stat.StatAppType = p_dataRow["STAT_APP_TYPE"].ToString();
        //    double lg;
        //    double.TryParse(p_dataRow["STAT_LA"].ToString(), out lg);
        //    stat.StatLa = lg;
        //    double.TryParse(p_dataRow["STAT_LG"].ToString(), out lg);
        //    stat.StatLg = lg;
        //    stat.StatName = p_dataRow["STAT_NAME"].ToString();
        //    stat.StatTdi = p_dataRow["STAT_TDI"].ToString();

        //    stat.ActivityGuid = p_dataRow["ACTIVITY_GUID"].ToString();
        //    stat.ActivityFreqGuid = p_dataRow["ACTIVITY_FPLAN_GUID"].ToString();
        //    stat.AppGuid = p_dataRow["APPGUID"].ToString();

        //    return new StationBaseExt(stat);
        //}

        private static ActivitySurroundStation LoadStationBaseRow(DataRow p_dataRow)
        {
            ActivitySurroundStation station = new ActivitySurroundStation();
            station.ActivityId = p_dataRow["activity_guid"].ToString();
            station.PlaceId = p_dataRow["place_guid"].ToString();
            //station.FreqPlan = new PlaceFreqPlan()
            //{
            //    Key = dr["freqplan_guid"].ToString(),
            //    Name = dr["freqplan_name"].ToString()
            //};
            station.APPGUID = p_dataRow["appguid"].ToString();
            station.STATGUID = p_dataRow["stat_guid"].ToString();
            station.APP_CODE = p_dataRow["app_code"].ToString();
            station.ORG_NAME = p_dataRow["org_name"].ToString();
            station.STAT_TDI = p_dataRow["stat_tdi"].ToString();
            station.STAT_APP_TYPE = p_dataRow["stat_app_type"].ToString();
            station.STAT_NAME = p_dataRow["stat_name"].ToString();
            station.STAT_ADDR = p_dataRow["stat_addr"].ToString();
            station.NET_SVN = p_dataRow["net_svn"].ToString();
            double location = 0;
            if (string.IsNullOrEmpty(p_dataRow["stat_lg"].ToString()) && double.TryParse(p_dataRow["stat_lg"].ToString(), out location))
            {
                station.STAT_LG = location;
            }
            if (string.IsNullOrEmpty(p_dataRow["stat_la"].ToString()) && double.TryParse(p_dataRow["stat_la"].ToString(), out location))
            {
                station.STAT_LA = location;
            }
            station.EmitInfo = QueryEmitInfoByStatID(station.STATGUID);
            return station;
        }

        /// <summary>
        /// 台站表SQL查询语句
        /// </summary>
        /// <param name="p_activityID">地区ID</param>
        /// <returns></returns>
        private static string GetQueryStatByPlaceSql(string p_placeID)
        {
            return string.Format("select * from ACTIVITY_STATION_BASE  where PLACE_GUID = '{0}'", p_placeID);
        }

        //add by michael 17.07,19
        private static string GetQueryFreqPlanSql(string pPlaceId)
        {
            return string.Format("select freq_planning_id,name,freqfrom,freqto,khzband from activity_Freq_Plan t where place_id = '{0}'", pPlaceId);
        }

        private static FreqPlanSegment LoadFreqPlanRow(System.Data.DataRow p_dataRow)
        {
            FreqPlanSegment freqplan = new FreqPlanSegment();

            double dbBegin, dbEnd;
            freqplan.FreqId = p_dataRow["freq_planning_id"].ToString();
            freqplan.FreqPlanName = p_dataRow["name"].ToString();
            double.TryParse(p_dataRow["freqfrom"].ToString(), out dbBegin);
            double.TryParse(p_dataRow["freqto"].ToString(), out dbEnd);
            freqplan.FreqValue.Little = dbBegin;
            freqplan.FreqValue.Great = dbEnd;
            freqplan.FreqBand = p_dataRow["khzband"].ToString();

            return freqplan;
        }

        public static List<FreqPlanSegment> QueryFreqPlanByPlaceID(string p_placeID)
        {
            return QueryData<FreqPlanSegment>(LoadFreqPlanRow, new Func<string, string>(GetQueryFreqPlanSql), new object[] { p_placeID });
        }
        //end
    }
}
