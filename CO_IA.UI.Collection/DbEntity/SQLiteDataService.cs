#region 文件描述
/*************************************************************************
 * 创建人：王若兴
 * 摘  要：SQLite数据库连接和操作
 * 日  期：2016-09-08
 * ***********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using CO_IA.Data;
using CO_IA.Data.Collection;
using CO_IA.UI.Collection.DataAnalysis;
using I_CO_IA.ActivityManage;
using I_CO_IA.Collection;
using CO_IA_Data;
using System.Collections.ObjectModel;

namespace CO_IA.UI.Collection.DbEntity
{
    public partial class SQLiteDataService
    {
        private static SQLiteTransaction _transaction = null;
        //操作活动的名称
        public static string _operationActivityName = LoginService.CurrentActivity.Name;
        public static SQLiteTransaction Transaction
        {
            get { return SQLiteDataService._transaction; }
            set { SQLiteDataService._transaction = value; }
        }
      
        #region 公共服务方法

        #region 查询
        /// <summary>
        /// 查询活动列表
        /// </summary>
        /// <param name="p_activityID">活动记录ID，为空查询全部</param>
        /// <returns></returns>
        public static List<Activity> QueryActivity(string p_activityID = "")
        {
            return QueryData<Activity>(LoadActivityRow, new Func<string, string>(SQLiteDataService.GetQueryActivitySql), new object[] { p_activityID });
        }
        
        /// <summary>
        /// 查询分析结果列表
        /// </summary>
        /// <param name="p_activityID">地区GUID，为空查询全部</param>
        /// <returns></returns>
        public static List<AnalysisResult> QueryAnalysisResult(string p_placeID)
        {
            return QueryData<AnalysisResult>(LoadAnalysisResultRow, new Func<string, string>(SQLiteDataService.GetQueryAnalysisResultSql), new object[] { p_placeID });
        }

        /// <summary>
        /// 查询分析结果列表用测量ID
        /// </summary>
        /// <param name="measureID">分析结果ID，为空查询全部</param>
        /// <returns></returns>
        public static List<AnalysisResult> QueryAnalysisResultByMeasureID(string p_placeID, string measureID = "")
        {
            return QueryData<AnalysisResult>(LoadAnalysisResultRow, new Func<string, string, string>(SQLiteDataService.GetQueryAnalysisResultSqlByMeasureID), new object[] { p_placeID, measureID });
        }

        /// <summary>
        /// 查询分析结果列表用测量ID，开始频率，结束频率
        /// </summary>
        /// <param name="measureID">分析结果ID，为空查询全部</param>
        /// <returns></returns>
        public static List<AnalysisResult> QueryAnalysisResultByMeasureIDAndFreq(FreqNavBar freqNavBar)
        {
            return QueryData<AnalysisResult>(LoadAnalysisResultRow, new Func<FreqNavBar, string>(SQLiteDataService.GetQueryAnalysisResultSqlByMeasureIDAndFreq), new object[] { freqNavBar });
        }

        /// <summary>
        /// 查询发射信息列表
        /// </summary>
        /// <param name="p_activityID">发射信息ID，为空查询全部</param>
        /// <returns></returns>
        public static List<StationEmitInfo> QueryEmitInfo(string p_activityID, string p_placeID)
        {
            return QueryData<StationEmitInfo>(LoadEmitInfoRow, new Func<string,string, string>(SQLiteDataService.GetQueryEmitInfoSql), new object[] { p_activityID,p_placeID });
        }
        public static List<StationEmitInfo> QueryEmitInfoByStatID(string p_statID)
        {
            return QueryData<StationEmitInfo>(LoadEmitInfoRow, new Func<string, string>(SQLiteDataService.GetQueryEmitInfoByStatIDSql), new object[] { p_statID });
        }
        /// <summary>
        /// 查询地点列表
        /// </summary>
        /// <param name="p_activityID">地点ID，为空查询全部</param>
        /// <returns></returns>
        public static List<ActivityPlace> QueryPlace(string p_statID = "")
        {
            return QueryData<ActivityPlace>(LoadPlaceRow, new Func<string, string>(SQLiteDataService.GetQueryPlaceSql), new object[] { p_statID });
        }
        public static List<ActivityPlace> QueryPlaceByActivityGuid(string p_activityName, string p_activityGuid = "")
        {
            _operationActivityName = p_activityName; 
            return QueryData<ActivityPlace>(LoadPlaceRow, new Func<string, string>(SQLiteDataService.GetQueryPlaceByActivitySql), new object[] { p_activityGuid });
        }
        /// <summary>
        /// 查询地点坐标列表
        /// </summary>
        /// <param name="p_activityID">地点ID，为空查询全部</param>
        /// <returns></returns>
        public static List<ActivityPlaceLocation> QueryPlaceLocation(string p_statID = "")
        {
            return QueryData<ActivityPlaceLocation>(LoadPlaceLocationRow, new Func<string, string>(SQLiteDataService.GetQueryPlaceLocSql), new object[] { p_statID });
        }

        public static List<ActivitySurroundStation> QueryStationBaseInfo(string p_statID = "")
        {
            return QueryData<ActivitySurroundStation>(LoadStationBaseRow, new Func<string, string>(SQLiteDataService.GetQueryStatSql), new object[] { p_statID });
        }
        //private static ActivitySurroundStation LoadStationBase(DataRow p_dataRow)
        //{
        //    ActivitySurroundStation station = new ActivitySurroundStation();
        //    station.ActivityId = p_dataRow["activity_guid"].ToString();
        //    station.PlaceId = p_dataRow["place_guid"].ToString();
        //    //station.FreqPlan = new PlaceFreqPlan()
        //    //{
        //    //    Key = dr["freqplan_guid"].ToString(),
        //    //    Name = dr["freqplan_name"].ToString()
        //    //};
        //    station.APPGUID = p_dataRow["appguid"].ToString();
        //    station.STATGUID = p_dataRow["stat_guid"].ToString();
        //    station.APP_CODE = p_dataRow["app_code"].ToString();
        //    station.ORG_NAME = p_dataRow["org_name"].ToString();
        //    station.STAT_TDI = p_dataRow["stat_tdi"].ToString();
        //    station.STAT_APP_TYPE = p_dataRow["stat_app_type"].ToString();
        //    station.STAT_NAME = p_dataRow["stat_name"].ToString();
        //    station.STAT_ADDR = p_dataRow["stat_addr"].ToString();
        //    station.NET_SVN = p_dataRow["net_svn"].ToString();
        //    double location = 0;
        //    if (string.IsNullOrEmpty(p_dataRow["stat_lg"].ToString()) && double.TryParse(p_dataRow["stat_lg"].ToString(), out location))
        //    {
        //        station.STAT_LG = location;
        //    }
        //    if (string.IsNullOrEmpty(p_dataRow["stat_la"].ToString()) && double.TryParse(p_dataRow["stat_la"].ToString(), out location))
        //    {
        //        station.STAT_LA = location;
        //    }
        //    station.EmitInfo = QueryEmitInfoByStatID(station.STATGUID);
        //    return station;
        //}
        public static List<T> QueryData<T>(Func<DataRow, T> p_dataRowLoad, Delegate p_delegate, object[] pms)
        {
            string sql = (string)p_delegate.DynamicInvoke(pms);
            List<T> list = new List<T>();
            DataSet dt = SQLiteConnect.ExecuteDataSet(SQLiteConnect.ConnectToDatabase(_operationActivityName), sql, null);
            if (dt != null && dt.Tables.Count > 0)
            {
                foreach (DataRow dr in dt.Tables[0].Rows)
                {
                    list.Add(p_dataRowLoad(dr));
                }
            }
            return list;
        }
       
        private static List<T> QueryDataWithTrans<T>(Func<string, string> p_sqlMethod, Func<DataRow, T> p_dataRowLoad, string p_dataid, SQLiteTransaction p_transaction)
        {
            List<T> list = new List<T>();
            DataSet dt = SQLiteConnect.ExecuteDataSet2(p_transaction.Connection, p_sqlMethod(p_dataid), null);
            if (dt != null && dt.Tables.Count > 0)
            {
                foreach (DataRow dr in dt.Tables[0].Rows)
                {
                    list.Add(p_dataRowLoad(dr));
                }
            }
            return list;
        }
        
      
        #endregion

        #region 保存
        public static bool SaveOperaterLog(OperateLog p_logInfo)
        {
            return InsertLogInfo(p_logInfo, _transaction);
        }

        public static bool SaveStationBase(ActivitySurroundStation p_stationBase)
        {
            //先删除，后插入，管理端有删除的话，能去除垃圾数据
            //原来是根据主键，存在的更新，不存在的插入。这样写不能更新管理端删除的数据
            //现在的保存方法中，判断是否存在和更新方法是没用的。应该删除
           
            //保存周围台站关联的发射设备
            foreach(var emit in p_stationBase.EmitInfo)
            {
                SaveEmitInfo(emit);
            }

            return SaveData<ActivitySurroundStation>(p_stationBase, IsExsitStatBase, InsertStationBase, UpdateStationBase, p_stationBase.STATGUID + "@" + p_stationBase.PlaceId);
        }

        public static bool SaveActivity(Activity p_activity)
        {
            return SaveData<Activity>(p_activity, IsExsitActivity, InsertActivity, UpdateActivity, p_activity.Guid);
        }
        public static bool SaveActivityPlace(ActivityPlaceInfo p_activityPlace)
        {
            if (p_activityPlace.Locations != null && p_activityPlace.Locations.Length>0)
            {
                foreach (ActivityPlaceLocation loc in p_activityPlace.Locations)
                {
                    SaveActivityPlaceLocation(loc);
                }
            }
            return SaveData<ActivityPlaceInfo>(p_activityPlace, IsExsitActivityPlace, InsertPlaceInfo, UpdateActivityPlace, p_activityPlace.Guid); 
        }
        public static bool SaveActivityPlaceLocation(ActivityPlaceLocation p_placeLocation)
        {
            return SaveData<ActivityPlaceLocation>(p_placeLocation, IsExsitActivityPlaceLocation, InsertPlaceLocation, UpdateActivityPlaceLocation, p_placeLocation.GUID);
        }

        public static bool SaveFreqPlan(PlaceFreqPlan p_placeFreq)
        {

            return SaveData<PlaceFreqPlan>(p_placeFreq, IsExsitActivityFreqPlan, InsertActivityFreqPlan, UpdateActivityFreqPlan,p_placeFreq.PlaceGuid + "#" + p_placeFreq.Key);
        }
        /// <summary>
        /// 保存AnalysisResult表记录
        /// </summary>
        /// <param name="p_analysisResult"></param>
        /// <returns></returns>
        public static bool SaveAnalysisResult(AnalysisResult p_analysisResult)
        {
            //id格式 频率值(frequency)#测量ID(MeasureID)#地点ID(placeGUID)
            if (string.IsNullOrEmpty(p_analysisResult.MeasureId) || string.IsNullOrEmpty(p_analysisResult.PlaceGuid))
            {
                throw new Exception("测量ID或者地点ID为空");
            }
            string combinationalId = string.Format("{0}#{1}#{2}", p_analysisResult.Frequency, p_analysisResult.MeasureId, p_analysisResult.PlaceGuid);
            return SaveData<AnalysisResult>(p_analysisResult, IsExsitAnalysisResult, InsertAnalysisResult, UpdateAnalysisResult, combinationalId);
        }
        /// <summary>
        /// 保存EmitInfo表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        //public static bool SaveEmitInfo(EmitInfo p_emitInfo)
        //{
        //    return SaveData<EmitInfo>(p_emitInfo, IsExsitEmitInfo, InsertEmitInfo, UpdateEmitInfo, p_emitInfo.Guid);
        //}

        public static void SaveEmitInfo(StationEmitInfo p_emitInfo)
        {
             SaveData<StationEmitInfo>(p_emitInfo, IsExsitEmitInfo, InsertEmitInfo, UpdateEmitInfo, p_emitInfo.Guid);
        }

        private static bool SaveData<T>(T t, Func<string, SQLiteTransaction, bool> p_isExsitMethod, Func<T, SQLiteTransaction, bool> p_insertMethod, Func<T, SQLiteTransaction, bool> p_updateMethod, string p_guid)
        {
            if (_transaction == null)
                return false;
            bool isExsit;
            if (string.IsNullOrEmpty(p_guid))
            {
                isExsit = false;
            }
            else
            {
                isExsit = p_isExsitMethod(p_guid, _transaction);
            }
            bool isSuccess = false;
            if (!isExsit)
            {
                isSuccess = p_insertMethod(t, _transaction);
            }
            else
            {
                isSuccess = p_updateMethod(t, _transaction);
            }

            return isSuccess;
        }
        //private static bool SaveData<T>(T t, Func<string,string, SQLiteTransaction, bool> p_deleteMethod, Func<T, SQLiteTransaction, bool> p_insertMethod,  string p_guid1,string p_guid2)
        //{
        //    if (_transaction == null)
        //        return false;
        //    p_deleteMethod(p_guid1, p_guid2, _transaction);
        //    return p_insertMethod(t, _transaction);
        //}
        #endregion

        #region 删除

        /// <summary>
        /// 删除主键为p_id的分析结果记录
        /// </summary>
        /// <param name="p_guid">分析结果GUID</param>
        /// <param name="p_placeGuid">地点的GUID</param>
        /// <returns></returns>
        public bool DeleteAnalysisResult(string p_guid,string p_placeGuid)
        {
            int i = SQLiteConnect.ExecuteNonQuery(_transaction, DeleteAnalysisResultSql(p_guid, p_placeGuid), null);

            return i == -1 ? false : true;
        }

        /// <summary>
        /// 删除主键为p_id的发射信息记录
        /// </summary>
        /// <param name="p_statGuid">台站Guid</param>
        /// <param name="p_nullid">没用到，只是为了和其他方法一致，保持两个参数</param>
        /// <returns></returns>
        public static bool DeleteEmitInfo(string p_statGuid,string p_nullid)
        {
            int i = SQLiteConnect.ExecuteNonQuery(_transaction, DeleteEmitInfoSql(p_statGuid), null);

            return i == -1 ? false : true;
        }

        public static bool DeleteStationBase(string p_activityID, string p_placeGuid)
        {
            int i = SQLiteConnect.ExecuteNonQuery(_transaction, DeleteStationBaseSql(p_activityID, p_placeGuid), null);
            return i == -1 ? false : true;
        }
        public static bool DeleteFreqPlan(string p_activityID, string p_placeGuid)
        {
            int i = SQLiteConnect.ExecuteNonQuery(_transaction, DeleteFreqPlanSql(p_activityID, p_placeGuid), null);
            return i == -1 ? false : true;
        }
        public static bool DeletePlace(string p_activityGuid)
        {
            int i = SQLiteConnect.ExecuteNonQuery(_transaction, DeletePlaceSql(p_activityGuid), null);
            return i == -1 ? false : true;
        }
        public static bool DeletePlaceLocation(string p_placeGuid)
        {
            int i = SQLiteConnect.ExecuteNonQuery(_transaction, DeletePlaceLocationSql(p_placeGuid), null);
            return i == -1 ? false : true;
        }

        #endregion

        #endregion

        #region 实际操作

        /// <summary>
        /// AnalysisResult表中是否存在ID值为p_id的记录
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        private static bool IsExsitAnalysisResult(string p_combinationalId, SQLiteTransaction p_transaction)
        {
            List<bool> list = QueryDataWithTrans<bool>(GetAnalysisResultExsitSql, LoadDataExsitRow, p_combinationalId, p_transaction);
            return list.Count > 0 ? list[0] : false; ;
        }
        /// <summary>
        /// EmitInfo表中是否存在ID值为p_id的记录
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        private static bool IsExsitEmitInfo(string p_id, SQLiteTransaction p_transaction)
        {
            List<bool> list = QueryDataWithTrans<bool>(GetEmitInfoExsitSql, LoadDataExsitRow, p_id, p_transaction);
            return list.Count > 0 ? list[0] : false; ;
        }
        private static bool IsExsitEmitInfoNew(string p_id, SQLiteTransaction p_transaction)
        {
            List<bool> list = QueryDataWithTrans<bool>(GetEmitInfoExsitSql, LoadDataExsitRow, p_id, p_transaction);
            return list.Count > 0 ? list[0] : false; ;
        }
        /// <summary>
        /// ACTIVITY_STATION_BASE表中是否存在GUID值为p_id的记录
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        private static bool IsExsitStatBase(string p_id, SQLiteTransaction p_transaction)
        {
            List<bool> list = QueryDataWithTrans<bool>(GetStationBaseExsitSql, LoadDataExsitRow, p_id, p_transaction);
            return list.Count > 0 ? list[0] : false; ;
        }

        /// <summary>
        /// ACTIVITY表中是否存在GUID值为p_id的记录
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        private static bool IsExsitActivity(string p_id, SQLiteTransaction p_transaction)
        {
            List<bool> list = QueryDataWithTrans<bool>(GetActivityExsitSql, LoadDataExsitRow, p_id, p_transaction);
            return list.Count > 0 ? list[0] : false; ;
        }
        /// <summary>
        /// ACTIVITYPlace表中是否存在GUID值为p_id的记录
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        private static bool IsExsitActivityPlace(string p_id, SQLiteTransaction p_transaction)
        {
            List<bool> list = QueryDataWithTrans<bool>(GetActivityPlaceExsitSql, LoadDataExsitRow, p_id, p_transaction);
            return list.Count > 0 ? list[0] : false; ;
        }
        /// <summary>
        /// ACTIVITYPlaceLocation表中是否存在GUID值为p_id的记录
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        private static bool IsExsitActivityPlaceLocation(string p_id, SQLiteTransaction p_transaction)
        {
            List<bool> list = QueryDataWithTrans<bool>(GetActivityPlaceLocationExsitSql, LoadDataExsitRow, p_id, p_transaction);
            return list.Count > 0 ? list[0] : false; ;
        }
        private static bool IsExsitActivityFreqPlan(string p_id, SQLiteTransaction p_transaction)
        {
            List<bool> list = QueryDataWithTrans<bool>(GetActivityFreqPlanExsitSql, LoadDataExsitRow, p_id, p_transaction);
            return list.Count > 0 ? list[0] : false; ;
        }
        /// <summary>
        /// 插入AnalysisResult表记录
        /// </summary>
        /// <param name="p_analysisResult"></param>
        /// <returns></returns>
        private static bool InsertAnalysisResult(AnalysisResult p_analysisResult, SQLiteTransaction p_transaction)
        {
            int i = SQLiteConnect.ExecuteNonQuery(p_transaction, InsertAnalysisResultSql(p_analysisResult), null);
            if (i != -1)
            {
                string sql = string.Format(@"Update ACTIVITY_EMIT_INFO set CLEAR_RESULT=1 where  GUID ='{0}' ", p_analysisResult.FreqGuid);
                i = UpdateClearRusultForEmitInfo(p_analysisResult.FreqGuid, p_analysisResult.FreqType, p_transaction);
            }

            return i == -1 ? false : true;
        }

        /// <summary>
        /// 插入EmitInfo表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        private static bool InsertEmitInfo(StationEmitInfo p_emitInfo, SQLiteTransaction p_transaction)
        {
            int i = SQLiteConnect.ExecuteNonQuery(p_transaction, InsertEmitInfoSql(p_emitInfo), null);
            //if (i != -1)
            //{
            //    i = UpdateFreqTypeForAnalysisResult(p_emitInfo.Guid, (int)p_emitInfo.ClearResult, p_transaction);
            //}
            return i == -1 ? false : true;
        }

        /// <summary>
        /// 插入StationBase表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        private static bool InsertStationBase(ActivitySurroundStation p_stationBase, SQLiteTransaction p_transaction)
        {

            int i = SQLiteConnect.ExecuteNonQuery(p_transaction, InsertStatBaseSql(p_stationBase), null);
            return i == -1 ? false : true;
        }

        /// <summary>
        /// 插入Activity表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        private static bool InsertActivity(Activity p_activity, SQLiteTransaction p_transaction)
        {
            //int i = SQLiteConnect.ExecuteNonQuery(p_transaction, InsertActivitySql(p_activity), null);
            int i = InsertActivityByParam(p_transaction, p_activity);
            return i == -1 ? false : true;
        }
        private static bool InsertLogInfo(OperateLog p_logInfo, SQLiteTransaction p_transaction)
        {
            int i = InsertOperateLog(p_transaction, p_logInfo);
            return i == -1 ? false : true;
        }
        /// <summary>
        /// 插入Place表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        private static bool InsertPlaceInfo(ActivityPlaceInfo p_placeInfo, SQLiteTransaction p_transaction)
        {
            //int i = SQLiteConnect.ExecuteNonQuery(p_transaction, InsertActivityPlaceSql(p_placeInfo), null);
            int i = InsertActivityPlaceByParam(p_transaction, p_placeInfo);
            return i == -1 ? false : true;
        }
        /// <summary>
        /// 插入PlaceLocation表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        private static bool InsertPlaceLocation(ActivityPlaceLocation p_placeLocation, SQLiteTransaction p_transaction)
        {
            int i = SQLiteConnect.ExecuteNonQuery(p_transaction, InsertActivityPlaceLocationSql(p_placeLocation), null);

            return i == -1 ? false : true;
        }

        private static bool InsertActivityFreqPlan(PlaceFreqPlan p_placeFreqPlan, SQLiteTransaction p_transaction)
        {
            int i = SQLiteConnect.ExecuteNonQuery(p_transaction, InsertActivityFreqPlanSql(p_placeFreqPlan), null);

            return i == -1 ? false : true;
        }
        
        /// <summary>
        /// 更新AnalysisResult表记录
        /// </summary>
        /// <param name="p_analysisResult"></param>
        /// <returns></returns>
        private static bool UpdateAnalysisResult(AnalysisResult p_analysisResult, SQLiteTransaction p_transaction)
        {
            int i = SQLiteConnect.ExecuteNonQuery(p_transaction, UpdateAnalysisResultSql(p_analysisResult), null);
            if (i != -1)
            {
                i = UpdateClearRusultForEmitInfo(p_analysisResult.FreqGuid, p_analysisResult.FreqType, p_transaction);
            }
            return i == -1 ? false : true;
        }
        /// <summary>
        /// 更新EmitInfo表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        private static bool UpdateEmitInfo(StationEmitInfo p_emitInfo, SQLiteTransaction p_transaction)
        {
            int i = SQLiteConnect.ExecuteNonQuery(p_transaction, UpdateEmitInfoSql(p_emitInfo), null);
            //if (i != -1)
            //{
            //    i = UpdateFreqTypeForAnalysisResult(p_emitInfo.Guid, (int)p_emitInfo.ClearResult, p_transaction);
            //}
            return i == -1 ? false : true;
        }

        /// <summary>
        /// 更新StationBase表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        private static bool UpdateStationBase(ActivitySurroundStation p_stationBase, SQLiteTransaction p_transaction)
        {
            int i = SQLiteConnect.ExecuteNonQuery(p_transaction, UpdateStatBaseSql(p_stationBase), null);
            return i == -1 ? false : true;
        }

        /// <summary>
        /// 更新Activity表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        private static bool UpdateActivity(Activity p_activity, SQLiteTransaction p_transaction)
        {
            //int i = SQLiteConnect.ExecuteNonQuery(p_transaction, UpdateActivitySql(p_activity), null);

            int i = UpdateActivityByParam(p_transaction, p_activity);
            return i == -1 ? false : true;
        }

        /// <summary>
        /// 更新Place表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        private static bool UpdateActivityPlace(ActivityPlaceInfo p_activityPlace, SQLiteTransaction p_transaction)
        {
            //int i = SQLiteConnect.ExecuteNonQuery(p_transaction, UpdateActivityPlaceSql(p_activityPlace), null);
            int i = UpdateActivityPlaceByParam(p_transaction, p_activityPlace);
            return i == -1 ? false : true;
        }

        /// <summary>
        /// 更新Place表记录
        /// </summary>
        /// <param name="p_emitInfo"></param>
        /// <returns></returns>
        private static bool UpdateActivityPlaceLocation(ActivityPlaceLocation p_activityPlaceLocation, SQLiteTransaction p_transaction)
        {
            int i = SQLiteConnect.ExecuteNonQuery(p_transaction, UpdateActivityPlaceLocationSql(p_activityPlaceLocation), null);
            return i == -1 ? false : true;
        }
        private static bool UpdateActivityFreqPlan(PlaceFreqPlan p_activityFreqPlan, SQLiteTransaction p_transaction)
        {
            int i = SQLiteConnect.ExecuteNonQuery(p_transaction, UpdateActivityFreqPlanSql(p_activityFreqPlan), null);
            return i == -1 ? false : true;
        }
        /// <summary>
        /// 保存分析结果时更新设备信息表的清理结果字段
        /// </summary>
        /// <param name="p_freqGuid"></param>
        /// <param name="p_signalTypeEnum"></param>
        /// <param name="p_transaction"></param>
        /// <returns></returns>
        private static int UpdateClearRusultForEmitInfo(string p_freqGuid, SignalTypeEnum p_signalTypeEnum, SQLiteTransaction p_transaction)
        {
            int i = 0;
            if (p_signalTypeEnum == SignalTypeEnum.清理)
            {
                string sql = string.Format(@"Update ACTIVITY_EMIT_INFO set CLEAR_RESULT=1 where  GUID ='{0}' ", p_freqGuid);
                i = SQLiteConnect.ExecuteNonQuery(p_transaction, sql, null);
            }
            return i;
        }

        //private static int UpdateFreqTypeForAnalysisResult(string p_freqGuid, int p_clearResult, SQLiteTransaction p_transaction)
        //{
        //    int i = 0;
        //    if (p_clearResult == 1)
        //    {
        //        string sql = string.Format(@"Update ACTIVITY_ANALYSIS_RESULT set CLEAR_RESULT={0} where  FREQGUID ='{1}' ",p_clearResult, p_freqGuid);
        //        i = SQLiteConnect.ExecuteNonQuery(p_transaction, sql, null);
        //    }
        //    return i;
        //}
        #endregion

        #region 数据加载
        private static bool LoadDataExsitRow(DataRow p_dataRow)
        {
            int i;
            int.TryParse(p_dataRow["count"].ToString(), out i);

            return i == 0 ? false : true;
        }
        private static ActivitySurroundStation LoadStationBaseRowNew(DataRow p_dataRow)
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
            station.STATGUID = p_dataRow["station_guid"].ToString();
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
        /// 活动表数据加载
        /// </summary>
        /// <param name="p_dataRow"></param>
        /// <returns></returns>
        private static Activity LoadActivityRow(DataRow p_dataRow)
        {
            CO_IA.Types.ActivityStage stageEnum;
            //CO_IA.Types.ActivityType typeEnum;
            DateTime dtime;

            Activity activity = new Activity();
            Enum.TryParse(p_dataRow["stage"].ToString(), out stageEnum);
            activity.ActivityStage = stageEnum;
            //Enum.TryParse(p_dataRow["activitytype"].ToString(), out typeEnum);
            activity.ActivityType = p_dataRow["activitytype"].ToString();
            DateTime.TryParse(p_dataRow["createtime"].ToString(), out dtime);
            activity.CreateTime = dtime;
            activity.Creator = p_dataRow["creator"].ToString();
            DateTime.TryParse(p_dataRow["datefrom"].ToString(), out dtime);
            activity.DateFrom = dtime;
            DateTime.TryParse(p_dataRow["dateto"].ToString(), out dtime);
            activity.DateTo = dtime;
            activity.Description = p_dataRow["description"].ToString();
            activity.Guid = p_dataRow["guid"].ToString();
            if (p_dataRow["icon"] is byte[])
                activity.Icon = p_dataRow["icon"] as byte[];
            activity.Name = p_dataRow["name"].ToString();
            activity.Organizer = p_dataRow["organizer"].ToString();
            activity.ShortHand = p_dataRow["shorthand"].ToString();

            return activity;
        }
        ///// <summary>
        ///// 台站表数据加载
        ///// </summary>
        ///// <param name="p_dataRow"></param>
        ///// <returns></returns>
        //private static StationBase LoadStationBaseRow(DataRow p_dataRow)
        //{
        //    StationBase stat = new StationBase();
        //    stat.AppCode = p_dataRow["APP_CODE"].ToString();
        //    stat.Guid = p_dataRow["GUID"].ToString();
        //    stat.NetSvn = p_dataRow["NET_SVN"].ToString();
        //    stat.OrgGuid = p_dataRow["ORG_GUID"].ToString();
        //    stat.OrgLinkPerson = p_dataRow["ORG_LINK_PERSON"].ToString();
        //    stat.OrgName = p_dataRow["ORG_NAME"].ToString();
        //    stat.OrgPhone = p_dataRow["ORG_PHONE"].ToString();
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

        //    stat.ActivityGuid = p_dataRow["ACTIVITYID"].ToString();
        //    stat.ActivityFreqGuid = p_dataRow["ACTIVITYFREQID"].ToString();
        //    stat.AppGuid = p_dataRow["APPGUID"].ToString();

        //    return stat;
        //}
        /// <summary>
        /// 分析结果表数据加载
        /// </summary>
        /// <param name="p_dataRow"></param>
        /// <returns></returns>
        private static AnalysisResult LoadAnalysisResultRow(DataRow p_dataRow)
        {
            int i;
            double d;
            SignalTypeEnum signalEnum;
            AnalysisResult result = new AnalysisResult();
            int.TryParse(p_dataRow["AmplitudeMaxValue"].ToString(), out i);
            result.AmplitudeMaxValue = i;
            int.TryParse(p_dataRow["AmplitudeMidValue"].ToString(), out i);
            result.AmplitudeMidValue = i;
            double.TryParse(p_dataRow["BandWidth"].ToString(), out d);
            result.BandWidth = d;
            double.TryParse(p_dataRow["EndFreq"].ToString(), out d);
            result.EndFreq = d;
            Enum.TryParse(p_dataRow["FreqType"].ToString(), out signalEnum);
            result.FreqType = signalEnum;
            double.TryParse(p_dataRow["Frequency"].ToString(), out d);
            result.Frequency = d;
            if (!string.IsNullOrEmpty(p_dataRow["ID"].ToString()))
            {
                //int.TryParse(p_dataRow["ID"].ToString(), out i);
                result.Id = p_dataRow["ID"].ToString();
            }
            result.MeasureId = p_dataRow["MeasureID"].ToString();
            int.TryParse(p_dataRow["Occupy"].ToString(), out i);
            result.Occupy = i;
            double.TryParse(p_dataRow["StartFreq"].ToString(), out d);
            result.StartFreq = d;
            result.StationName = p_dataRow["StationName"].ToString();
            result.StationGuid = p_dataRow["StationGuid"].ToString();
            result.PlaceGuid = p_dataRow["place_guid"].ToString();
            result.StClassCode = p_dataRow["ST_CLASS_CODE"].ToString();
            result.FreqGuid = p_dataRow["FREQGUID"].ToString();
            string startDateStr = p_dataRow["MEASURESTARTTIME"].ToString();
            string endDateStr = p_dataRow["MEASUREENDTIME"].ToString();
            DateTime dt;
            if(!string.IsNullOrEmpty(startDateStr))
            {
                if(DateTime.TryParse(startDateStr,out dt))
                {
                    result.MeasureStartTime = dt;
                }
            }
            if (!string.IsNullOrEmpty(endDateStr))
            {
                if (DateTime.TryParse(endDateStr, out dt))
                {
                    result.MeasureStartTime = dt;
                }
            }
            return result;
        }
        /// <summary>
        /// 发射信息表数据加载
        /// </summary>
        /// <param name="p_dataRow"></param>
        /// <returns></returns>
        private static StationEmitInfo LoadEmitInfoRow(DataRow p_dataRow)
        {
            double d;
            int i;
            StationEmitInfo emitInfo = new StationEmitInfo();
            emitInfo.Guid = p_dataRow["GUID"].ToString();
            emitInfo.StationGuid = p_dataRow["STATION_GUID"].ToString();
            emitInfo.PlaceGuid = p_dataRow["PLACE_GUID"].ToString();
            double.TryParse(p_dataRow["ANT_HIGHT"].ToString(), out d);
            emitInfo.AntHight = d;
            double.TryParse(p_dataRow["EQU_POW"].ToString(), out d);
            emitInfo.EquPow = d;
            double.TryParse(p_dataRow["FREQ_EC"].ToString(), out d);
            emitInfo.FreqEC = d;
            double.TryParse(p_dataRow["FREQ_RC"].ToString(), out d);
            emitInfo.FreqRC = d;
            double.TryParse(p_dataRow["FREQ_BAND"].ToString(), out d);
            emitInfo.FreqBand = d;
            double.TryParse(p_dataRow["ANT_EGAIN"].ToString(), out d);
            emitInfo.AntEgain = d;
            double.TryParse(p_dataRow["FEED_LOSE"].ToString(), out d);
            emitInfo.FeedLose = d;
            emitInfo.FreqMod = p_dataRow["FREQ_MOD"].ToString();
            emitInfo.AntPole = p_dataRow["ANT_POLE"].ToString();
            double.TryParse(p_dataRow["STAT_AT"].ToString(), out d);
            emitInfo.StatAT = d;
            double.TryParse(p_dataRow["RCV_ANT_HIGHT"].ToString(), out d);
            emitInfo.RCVAntHight = d;
            double.TryParse(p_dataRow["RCV_ANT_EGAIN"].ToString(), out d);
            emitInfo.RCVAntEgain = d;
            double.TryParse(p_dataRow["RCV_FEED_LOSE"].ToString(), out d);
            emitInfo.RCVFeedLose = d;
           
           
            int.TryParse(p_dataRow["NEED_CLEAR"].ToString(), out i);
            emitInfo.NeedClear = (NeedClearEunm)i;
            int.TryParse(p_dataRow["CLEAR_RESULT"].ToString(), out i);
            emitInfo.ClearResult = (ClearResultEnum)i;
            int.TryParse(p_dataRow["FREQ_TYPE"].ToString(), out i);
            emitInfo.FreqType = (FreqType)i;
            double.TryParse(p_dataRow["FREQ_EFB"].ToString(), out d);
            if (!string.IsNullOrEmpty(p_dataRow["FREQ_EFB"].ToString()))
            {
                emitInfo.FreqEFB = d;
            }
            double.TryParse(p_dataRow["FREQ_EFE"].ToString(), out d);
            if (!string.IsNullOrEmpty(p_dataRow["FREQ_EFE"].ToString()))
            {
                emitInfo.FreqEFE = d;
            }

            double.TryParse(p_dataRow["FREQ_RFB"].ToString(), out d);
            if (!string.IsNullOrEmpty(p_dataRow["FREQ_RFB"].ToString()))
            {
                emitInfo.FreqRFB = d;
            }
            double.TryParse(p_dataRow["FREQ_RFE"].ToString(), out d);
            if (!string.IsNullOrEmpty(p_dataRow["FREQ_RFE"].ToString()))
            {
                emitInfo.FreqRFE = d;
            }

            return emitInfo;
        }
        /// <summary>
        /// 地区表数据加载
        /// </summary>
        /// <param name="p_dataRow"></param>
        /// <returns></returns>
        private static ActivityPlace LoadPlaceRow(DataRow p_dataRow)
        {
            ActivityPlaceInfo place = new ActivityPlaceInfo();
            place.ActivityGuid = p_dataRow["activity_guid"].ToString();
            place.Address = p_dataRow["address"].ToString();
            place.Contact = p_dataRow["contact"].ToString();
            place.Guid = p_dataRow["guid"].ToString();
            if (p_dataRow["image"] is byte[])
                place.Image = p_dataRow["image"] as byte[];
            place.Name = p_dataRow["name"].ToString();
            place.Phone = p_dataRow["phone"].ToString();
            place.Name = p_dataRow["name"].ToString();


            return place;
        }
        /// <summary>
        /// 地区坐标表数据加载
        /// </summary>
        /// <param name="p_dataRow"></param>
        /// <returns></returns>
        private static ActivityPlaceLocation LoadPlaceLocationRow(DataRow p_dataRow)
        {
            double d;
            ActivityPlaceLocation loc = new ActivityPlaceLocation();
            loc.ActivityPlaceGuid = p_dataRow["activity_place_guid"].ToString();
            loc.GUID = p_dataRow["guid"].ToString();
            double.TryParse(p_dataRow["location_la"].ToString(), out d);
            loc.LocationLA = d;
            double.TryParse(p_dataRow["location_lg"].ToString(), out d);
            loc.LocationLG = d;
            loc.LocationName = p_dataRow["location_name"].ToString();
            return loc;
        }

        #endregion

        #region SQL语句

        #region select
        private static string GetQueryStationSql(string p_emitID)
        {
            return string.Format(" select a.* from ACTIVITY_STATION_BASE a,ACTIVITY_EMIT_INFO b where a.STAT_GUID  = b.station_guid and b.guid='{0}'", p_emitID);;
        }
        /// <summary>
        /// 活动表SQL查询语句
        /// </summary>
        /// <param name="p_statID"></param>
        /// <returns></returns>
        private static string GetQueryActivitySql(string p_statID = "")
        {
            return string.Format("select Guid,name,shorthand,organizer,stage,description,icon,creator,activitytype,datetime(datefrom) as datefrom, datetime(dateto) as dateto,datetime(createtime) as createtime from RIAS_ACTIVITY {0}", string.IsNullOrEmpty(p_statID) ? "" : " where guid ='" + p_statID + "'");
        }
        /// <summary>
        /// 台站表SQL查询语句
        /// </summary>
        /// <param name="p_statID">台站ID</param>
        /// <returns></returns>
        private static string GetQueryStatSql(string p_statID = "")
        {
            return string.Format("select * from ACTIVITY_STATION_BASE {0}", string.IsNullOrEmpty(p_statID) ? "" : " where stat_guid = '" + p_statID + "'");
        }
        ///// <summary>
        ///// 台站表SQL查询语句
        ///// </summary>
        ///// <param name="p_activityID">地区ID</param>
        ///// <returns></returns>
        //private static string GetQueryStatByPlaceSql(string p_placeID)
        //{
        //    return string.Format("select * from ACTIVITY_STATION_BASE  where place_guid = '{0}'", p_placeID);
        //}
        /// <summary>
        /// 分析结果表SQL查询语句
        /// </summary>
        /// <param name="p_resultID"></param>
        /// <returns></returns>
        private static string GetQueryAnalysisResultSql(string p_placeGuid = "")
        {
            return string.Format(@"select * from ACTIVITY_ANALYSIS_RESULT {0}", string.IsNullOrEmpty(p_placeGuid) ? "" : " where place_guid ='" + p_placeGuid + "'");
        }

        /// <summary>
        /// 分析结果表SQL查询语句用测量ID
        /// </summary>
        /// <param name="measureID"></param>
        /// <returns></returns>
        private static string GetQueryAnalysisResultSqlByMeasureID(string p_placeID, string p_measureID)//measureID p_placeID + "@" + measureID;
        {
            string sql = string.Format(@"select * from ACTIVITY_ANALYSIS_RESULT where (1=1) and place_guid='{0}' and measureid='{1}' ", p_placeID, p_measureID);
            //if (string.IsNullOrEmpty(p_placeid) || p_placeid.Trim().Length == 1) return sql;
            //string[] idArray = p_id.Split('@');
            //if (!string.IsNullOrEmpty(idArray[0]))
            //{
            //    sql = sql + "and place_guid='" + idArray[0] + "'";
            //}
            //if (!string.IsNullOrEmpty(idArray[1]))
            //{
            //    sql = sql + "and measureid='" + idArray[1] + "'";
            //}
            return sql;
            //// return string.Format(@"select id, frequency, bandwidth, amplitudemidvalue, amplitudemaxvalue, occupy, 
            ////                       stationname, freqtype, measureid, startfreq, endfreq from ACTIVITY_ANALYSIS_RESULT {0}", string.IsNullOrEmpty(measureID) ? "" : " where measureid ='" + measureID+"'");
        }

        /// <summary>
        /// 分析结果表SQL查询语句用测量ID,起始频率
        /// </summary>
        /// <param name="measureID"></param>
        /// <returns></returns>
        private static string GetQueryAnalysisResultSqlByMeasureIDAndFreq(FreqNavBar freqNavBar)
        {
            if (string.IsNullOrEmpty(freqNavBar.PlaceGuid))
                return string.Empty;
            return string.Format(@"select * from ACTIVITY_ANALYSIS_RESULT  {0}", string.IsNullOrEmpty(freqNavBar.MeasureId) ? "" : " where measureid ='" + freqNavBar.MeasureId + "' and StartFreq=" + freqNavBar.FreqStart + " and EndFreq=" + freqNavBar.FreqStop + " and place_guid='" + freqNavBar.PlaceGuid + "'");
        }

        /// <summary>
        /// 发射信息表SQL查询语句
        /// </summary>
        /// <param name="p_emiID"></param>
        /// <returns></returns>
        private static string GetQueryEmitInfoSql(string p_activityGuid,string p_placeGuid)
        {
            
            return string.Format(@"select * from ACTIVITY_EMIT_INFO a,ACTIVITY_STATION_BASE b  where a.STATION_GUID = b.STAT_GUID and 
a.place_GUID ='{0}' and b.ACTIVITY_GUID ='{1}'", p_placeGuid, p_activityGuid);
        }
        private static string GetQueryEmitInfoByStatIDSql(string p_statID)
        {
            return string.Format("select * from ACTIVITY_EMIT_INFO where station_guid = '{0}'", p_statID);
        }

        /// <summary>
        /// 地点表SQL查询语句
        /// </summary>
        /// <param name="p_placeID"></param>
        /// <returns></returns>
        private static string GetQueryPlaceSql(string p_placeID = "")
        {
            return string.Format("select * from ACTIVITY_PLACE {0}", string.IsNullOrEmpty(p_placeID) ? "" : " where  GUID ='" + p_placeID + "'");
        }
        /// <summary>
        /// 地点表SQL查询语句 
        /// </summary>
        /// <param name="p_activityGuid"> 活动GUID</param>
        /// <returns></returns>
        private static string GetQueryPlaceByActivitySql(string p_activityGuid)
        {
            return string.Format("select * from ACTIVITY_PLACE {0}", " where  activity_guid ='" + p_activityGuid + "'");
        }

        /// <summary>
        /// 地点坐标表SQL查询语句
        /// </summary>
        /// <param name="p_emiID"></param>
        /// <returns></returns>
        private static string GetQueryPlaceLocSql(string p_placeID = "")
        {
            return string.Format("select * from ACTIVITY_PLACE_LOCATION {0}", string.IsNullOrEmpty(p_placeID) ? "" : " where GUID ='" + p_placeID + "'");
        }

        #region exsit
        /// <summary>
        /// 发射分析结果记录是否存在
        /// </summary>
        /// <param name="p_resultID"></param>
        /// <returns></returns>
        private static string GetAnalysisResultExsitSql(string p_combinationalId)
        {
            string[] idArray = p_combinationalId.Split('#');
            double freqValue = double.Parse(idArray[0]);
            return string.Format(@"select Count(*) as count from ACTIVITY_ANALYSIS_RESULT where Frequency={0} and MeasureID = '{1}' and PLACE_GUID = '{2}'", freqValue, idArray[1], idArray[2]);
        }
        /// <summary>
        /// 查询发射信息记录是否存在
        /// </summary>
        /// <param name="p_emitID"></param>
        /// <returns></returns>
        private static string GetEmitInfoExsitSql(string p_emitID)
        {
            return string.Format(@"select Count(*) as count from ACTIVITY_EMIT_INFO where guid='{0}'", p_emitID);
        }
        //private static string GetEmitInfoExsitSql(string p_emitID)
        //{
        //    return string.Format(@"select Count(*) as count from ACTIVITY_EMIT_INFO where guid='{0}'", p_emitID);
        //}

        /// <summary>
        /// 查询周围台站是否存在 联合主键
        /// </summary>
        /// <param name="p_statID">站id和地点GUID拼接传入，格式StatID@PlaceGUID</param>
        /// <returns></returns>
        private static string GetStationBaseExsitSql(string p_statID)//(string p_statID,string p_placeGuid)
        {
            string[] ids = p_statID.Split('@');
            if (ids.Length != 2) return null;
            return string.Format(@"select Count(*) as count from ACTIVITY_STATION_BASE where STAT_GUID='{0}' and place_guid = '{1}'", ids[0], ids[1]);
        }

        /// <summary>
        /// 查询活动是否存在 
        /// </summary>
        /// <param name="p_emitID"></param>
        /// <returns></returns>
        private static string GetActivityExsitSql(string p_activityID)
        {
            return string.Format(@"select Count(*) as count from RIAS_ACTIVITY where guid='{0}' ", p_activityID);
        }
        /// <summary>
        /// 查询活动地点是否存在 
        /// </summary>
        /// <param name="p_emitID"></param>
        /// <returns></returns>
        private static string GetActivityPlaceExsitSql(string p_placeID)
        {
            return string.Format(@"select Count(*) as count from ACTIVITY_PLACE where guid='{0}' ", p_placeID);
        }

        /// <summary>
        /// 查询活动地点是否存在 
        /// </summary>
        /// <param name="p_emitID"></param>
        /// <returns></returns>
        private static string GetActivityPlaceLocationExsitSql(string p_placeLocationID)
        {
            return string.Format(@"select Count(*) as count from ACTIVITY_PLACE_LOCATION where guid='{0}' ", p_placeLocationID);
        }
        private static string GetActivityFreqPlanExsitSql(string p_combinationalId)
        {
            string[] idArray = p_combinationalId.Split('#');
            return string.Format(@"select Count(*) as count from Activity_Freq_Plan where PLACE_ID='{0}' and freq_planning_id='{1}' ", idArray[0], idArray[1]);
        }
        #endregion

        #endregion

        #region insert
        /// <summary>
        /// 插入分析结果记录
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string InsertAnalysisResultSql(AnalysisResult p_result)
        {
            string[] columns = { "frequency", "bandwidth", "amplitudemidvalue", "amplitudemaxvalue", "occupy",
                                 "stationname", "freqtype", "measureid", "startfreq", "endfreq","place_guid","ST_CLASS_CODE","FREQGUID","ID","Analysis_Band" ,"MEASURESTARTTIME","MEASUREENDTIME","stationguid"};
            return string.Format(@"insert into ACTIVITY_ANALYSIS_RESULT({0}) values({1},{2},{3},{4},{5},'{6}',{7},'{8}'
                ,{9},{10},'{11}','{12}','{13}','{14}',{15},'{16}','{17}','{18}')", string.Join(",", columns), p_result.Frequency, p_result.BandWidth, p_result.AmplitudeMidValue,
                                p_result.AmplitudeMaxValue, p_result.Occupy, p_result.StationName, (int)p_result.FreqType, p_result.MeasureId,
                                p_result.StartFreq, p_result.EndFreq, p_result.PlaceGuid, p_result.StClassCode, p_result.FreqGuid, p_result.Id, p_result.AnalysisBandWidth, p_result.MeasureStartTime.ToString("s"), p_result.MeasureEndTime.ToString("s"),p_result.StationGuid);
        }
        /// <summary>
        /// 插入发射信息记录
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string InsertEmitInfoSql(StationEmitInfo p_result)
        {
            string[] columns = { "guid", "station_guid", "place_guid", "ant_hight", "equ_pow", "freq_ec", "freq_rc", "freq_band", "ant_egain",
                                   "feed_lose", "freq_mod", "ant_pole", "stat_at", "rcv_ant_hight", "rcv_ant_egain", "rcv_feed_lose" , "NEED_CLEAR" , "CLEAR_RESULT","FREQ_TYPE","FREQ_EFB","FREQ_EFE","FREQ_RFB","FREQ_RFE" };

            return string.Format(@"insert into activity_emit_info({0}) values('{1}','{2}','{3}',{4},{5},{6},{7},{8},
                {9},{10},'{11}','{12}',{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23})", string.Join(",", columns), p_result.Guid, p_result.StationGuid, p_result.PlaceGuid,
                                                        p_result.AntHight, p_result.EquPow, p_result.FreqEC == null ? "null" : p_result.FreqEC.ToString(), p_result.FreqRC == null ? "null" : p_result.FreqRC.ToString(), p_result.FreqBand,
                                                        p_result.AntEgain, p_result.FeedLose, p_result.FreqMod, p_result.AntPole, p_result.StatAT,
                                                        p_result.RCVAntHight, p_result.RCVAntEgain, p_result.RCVFeedLose, (int)p_result.NeedClear, (int)p_result.ClearResult, (int)p_result.FreqType, p_result.FreqEFB == null ? "null" : p_result.FreqEFB.ToString(), p_result.FreqEFE == null ? "null" : p_result.FreqEFE.ToString(), p_result.FreqRFB == null ? "null" : p_result.FreqRFB.ToString(), p_result.FreqRFE == null ? "null" : p_result.FreqRFE.ToString());
        }

        /// <summary>
        /// 插入周围台站信息
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string InsertStatBaseSql(ActivitySurroundStation p_result)
        {
            string[] columns = { "ACTIVITY_GUID", "PLACE_GUID", "ACTIVITY_FPLAN_GUID", "APPGUID", "STAT_GUID", "APP_CODE",
                                 "STAT_TDI", "STAT_APP_TYPE", "ORG_NAME", "STAT_NAME", "STAT_ADDR", "NET_SVN", "STAT_LG", "STAT_LA"};

            return string.Format(@"insert into ACTIVITY_STATION_BASE({0}) values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',
                '{9}','{10}','{11}','{12}',{13},{14})", string.Join(",", columns), p_result.ActivityId, p_result.PlaceId, p_result.FreqPlan.Key,
                                                        p_result.APPGUID, p_result.STATGUID, p_result.APP_CODE, p_result.STAT_TDI, p_result.STAT_APP_TYPE,
                                                        p_result.ORG_NAME, p_result.STAT_NAME, p_result.STAT_ADDR, p_result.NET_SVN, p_result.STAT_LG,
                                                        p_result.STAT_LA);
        }

        /// <summary>
        /// 插入活动信息
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string InsertActivitySql(Activity p_result)
        {
            string[] columns = { "guid", "name", "shorthand", "datefrom", "dateto", "icon",
                                 "organizer", "stage", "description", "creator", "createtime", "activitytype"};

            return string.Format(@"insert into rias_activity ({0}) values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',
                '{9}','{10}','{11}','{12}')", string.Join(",", columns),
                                        p_result.Guid, p_result.Name, p_result.ShortHand,
                                                        p_result.DateFrom.ToString("s"), p_result.DateTo.ToString("s"), p_result.Icon, p_result.Organizer, p_result.ActivityStage,
                                                        p_result.Description, p_result.Creator, p_result.CreateTime.ToString("s"), p_result.ActivityType);
        }
        private static int InsertOperateLog(SQLiteTransaction p_transaction, OperateLog p_result)
        {
            SQLiteParameter[] paramList = new SQLiteParameter[6];
            string[] columns = { "GUID", "Operater", "Operate_Date", "Operate_Type", "Operate_Tables", "Remark"};

            string sqlText = string.Format(@"insert into ACTIVITY_operate_LOG ({0}) 
                                             values('{1}','{2}','{3}','{4}','{5}','{6}') ", string.Join(",", columns), p_result.Guid,p_result.Operater,p_result.OperateDate.ToString("s"),p_result.OperateType,p_result.OperateTables,p_result.Remark); 
            return SQLiteConnect.ExecuteNonQuery(p_transaction, sqlText, null);
        }

        private static int InsertActivityByParam(SQLiteTransaction p_transaction, Activity p_result)
        {
            SQLiteParameter[] paramList = new SQLiteParameter[12];
            string[] columns = { "guid", "name", "shorthand", "datefrom", "dateto", "icon",
                                 "organizer", "stage", "description", "creator", "createtime", "activitytype"};
            string sqlText = string.Format(@"insert into rias_activity ({0}) values(:V_guid,:V_name,:V_shorthand,:V_datefrom,:V_dateto,
                             :V_icon,:V_organizer,:V_stage,:V_description,:V_creator,:V_createtime,:V_activitytype ) ", string.Join(",", columns));
            paramList[0] = new SQLiteParameter("V_guid", DbType.String) { Value = p_result.Guid };
            paramList[1] = new SQLiteParameter("V_name", DbType.String) { Value = p_result.Name };
            paramList[2] = new SQLiteParameter("V_shorthand", DbType.String) { Value = p_result.ShortHand };
            paramList[3] = new SQLiteParameter("V_datefrom", DbType.Date) { Value = p_result.DateFrom.ToString("s") };
            paramList[4] = new SQLiteParameter("V_dateto", DbType.Date) { Value = p_result.DateTo.ToString("s") };
            paramList[5] = new SQLiteParameter("V_icon", DbType.Binary) { Value = p_result.Icon };
            paramList[6] = new SQLiteParameter("V_organizer", DbType.String) { Value = p_result.Organizer };
            paramList[7] = new SQLiteParameter("V_stage", DbType.String) { Value = p_result.ActivityStage };
            paramList[8] = new SQLiteParameter("V_description", DbType.String) { Value = p_result.Description };
            paramList[9] = new SQLiteParameter("V_creator", DbType.String) { Value = p_result.Creator };
            paramList[10] = new SQLiteParameter("V_createtime", DbType.Date) { Value = p_result.CreateTime.ToString("s") };
            paramList[11] = new SQLiteParameter("V_activitytype", DbType.String) { Value = p_result.ActivityType };
            return SQLiteConnect.ExecuteNonQuery(p_transaction, sqlText, paramList);

        }

        /// <summary>
        /// 插入地点信息
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string InsertActivityPlaceSql(ActivityPlaceInfo p_result)
        {
            string[] columns = { "guid", "activity_guid", "name", "address", "contact", "phone", "image", "graphics" };

            return string.Format(@"insert into activity_place ({0}) values('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", string.Join(",", columns),
                                        p_result.Guid, p_result.ActivityGuid, p_result.Name,
                                                        p_result.Address, p_result.Contact, p_result.Phone, p_result.Image, p_result.Graphics);
        }
        private static int InsertActivityPlaceByParam(SQLiteTransaction p_transaction, ActivityPlaceInfo p_result)
        {
            SQLiteParameter[] paramList = new SQLiteParameter[8];
            string[] columns = { "guid", "activity_guid", "name", "address", "contact", "phone", "image", "graphics" };
            string sqlText = string.Format(@"insert into activity_place ({0}) values(:V_guid,:V_activity_guid,:V_name,:V_address,:V_contact,
                             :V_phone,:V_image,:V_graphics ) ", string.Join(",", columns));

            paramList[0] = new SQLiteParameter("V_guid", DbType.String) { Value = p_result.Guid };
            paramList[1] = new SQLiteParameter("V_activity_guid", DbType.String) { Value = p_result.ActivityGuid };
            paramList[2] = new SQLiteParameter("V_name", DbType.String) { Value = p_result.Name };
            paramList[3] = new SQLiteParameter("V_address", DbType.String) { Value = p_result.Address };
            paramList[4] = new SQLiteParameter("V_contact", DbType.String) { Value = p_result.Contact };
            paramList[5] = new SQLiteParameter("V_phone", DbType.String) { Value = p_result.Phone };
            paramList[6] = new SQLiteParameter("V_image", DbType.Binary) { Value = p_result.Image };
            paramList[7] = new SQLiteParameter("V_graphics", DbType.String) { Value = p_result.Graphics };

            return SQLiteConnect.ExecuteNonQuery(p_transaction, sqlText, paramList);
        }

        /// <summary>
        /// 插入地点坐标
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string InsertActivityPlaceLocationSql(ActivityPlaceLocation p_result)
        {
            string[] columns = { "guid", "activity_place_guid", "location_name", "location_lg", "location_la" };

            return string.Format(@"insert into activity_place_location ({0}) values('{1}','{2}','{3}',{4},{5})", string.Join(",", columns),
                                        p_result.GUID, p_result.ActivityPlaceGuid, p_result.LocationName,
                                                        p_result.LocationLG, p_result.LocationLA);
        }

        private static string InsertActivityFreqPlanSql(PlaceFreqPlan p_result)
        {
            string[] columns = { "freq_planning_id", "PLACE_ID", "ACTIVITY_ID", "MDISTANCETOPLACE", "LONGITUDEFROM", "LONGITUDETO", "LATITUDEFROM", "LATITUDETO", "POINTS", "NAME", "EQUIPMENTCLASS", "FREQFROM", "FREQTO", "KHZBAND" };

            return string.Format(@"insert into Activity_Freq_Plan ({0}) values('{1}','{2}','{3}',{4},{5},{6},{7},{8},{9},'{10}','{11}',{12},{13},{14})", string.Join(",", columns),
                                        p_result.Key, p_result.PlaceGuid, p_result.ActivityGuid,p_result.mDistanceToActivityPlace,p_result.LongitudeRange.Little,
                                        p_result.LongitudeRange.Great,p_result.LatitudeRange.Little,p_result.LatitudeRange.Great,"null",p_result.Name,p_result.EquipmentClassID,
                                                        p_result.MHzFreqFrom, p_result.MHzFreqTo, p_result.kHzBand);
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新分析结果记录
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string UpdateAnalysisResultSql(AnalysisResult p_result)
        {
            return string.Format(@"update ACTIVITY_ANALYSIS_RESULT set frequency={0},bandwidth={1},amplitudemidvalue={2},amplitudemaxvalue={3},
                                   occupy={4},stationname='{5}',freqtype='{6}',measureid='{7}',startfreq={8},endfreq={9},place_guid='{10}',ST_CLASS_CODE='{11}',FREQGUID='{12}',Analysis_Band={13},MEASURESTARTTIME='{14}',MEASURESTARTTIME='{15}',stationguid='{18}' where id ='{16}' and PLACE_GUID = '{17}'", p_result.Frequency, p_result.BandWidth, p_result.AmplitudeMidValue,
                                p_result.AmplitudeMaxValue, p_result.Occupy, p_result.StationName, (int)p_result.FreqType, p_result.MeasureId,
                                p_result.StartFreq, p_result.EndFreq, p_result.PlaceGuid, p_result.StClassCode, p_result.FreqGuid, p_result.AnalysisBandWidth,
                                p_result.MeasureStartTime.ToString("s"), p_result.MeasureEndTime.ToString("s"),
                                p_result.Id, p_result.PlaceGuid,p_result.StationGuid);
        }
        /// <summary>
        /// 更新发射信息记录
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string UpdateEmitInfoSql(StationEmitInfo p_result)
        {
            return string.Format(@"Update activity_emit_info set station_guid='{0}',place_guid='{1}',ant_hight={2},equ_pow={3},freq_ec={4},
                   freq_rc={5},freq_band={6},ant_egain={7},feed_lose={8},freq_mod='{9}',ant_pole='{10}',
                   stat_at={11},rcv_ant_hight={12},rcv_ant_egain={13},rcv_feed_lose={14},NEED_CLEAR={15},NEED_CLEAR={16},CLEAR_RESULT={17},FREQ_TYPE={18},FREQ_EFB={19},FREQ_EFE={20},FREQ_RFB={21},FREQ_RFE={22} where guid='{23}'",
                                    p_result.StationGuid, p_result.PlaceGuid,
                                  p_result.AntHight, p_result.EquPow, p_result.FreqEC, p_result.FreqRC, p_result.FreqBand,
                                  p_result.AntEgain, p_result.FeedLose, p_result.FreqMod, p_result.AntPole, p_result.StatAT,
                                  p_result.RCVAntHight, p_result.RCVAntEgain, p_result.RCVFeedLose, p_result.NeedClear, p_result.ClearResult, 
                                  p_result.FreqType,p_result.FreqEFB,p_result.FreqEFE,p_result.FreqRFB,p_result.FreqRFE,p_result.Guid);
        }
        /// <summary>
        /// 更新周围台站 联合主键
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string UpdateStatBaseSql(ActivitySurroundStation p_result)
        {
            return string.Format(@"Update ACTIVITY_STATION_BASE set ACTIVITY_GUID='{0}',ACTIVITY_FPLAN_GUID='{1}',APPGUID='{2}',APP_CODE='{3}',STAT_TDI='{4}',
                   STAT_APP_TYPE='{5}',ORG_NAME='{6}',STAT_NAME='{7}',STAT_ADDR='{8}',NET_SVN='{9}',STAT_LG={10},
                   STAT_LA={11} where STAT_GUID='{12}' and PLACE_GUID = '{13}'",
                                    p_result.ActivityId, p_result.FreqPlan.Key,p_result.APPGUID, p_result.APP_CODE, p_result.STAT_TDI, p_result.STAT_APP_TYPE, p_result.ORG_NAME,
                                  p_result.STAT_NAME, p_result.STAT_ADDR, p_result.NET_SVN, p_result.STAT_LG, p_result.STAT_LA,
                                  p_result.STATGUID, p_result.PlaceId);
        }

        /// <summary>
        /// 更新活动信息
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string UpdateActivitySql(Activity p_result)
        {
            return string.Format(@"Update rias_activity set name='{0}',shorthand='{1}',datefrom='{2}',dateto='{3}',icon='{4}',
                   organizer='{5}',stage='{6}',description='{7}',creator='{8}',createtime='{9}',activitytype='{10}'
                    where guid='{11}'",
                                    p_result.Name, p_result.ShortHand,
                                  p_result.DateFrom.ToString("s"), p_result.DateTo.ToString("s"), p_result.Icon, p_result.Organizer, p_result.ActivityStage,
                                  p_result.Description, p_result.Creator, p_result.CreateTime.ToString("s"), p_result.ActivityType,
                                  p_result.Guid);
        }
        private static int UpdateActivityByParam(SQLiteTransaction p_transaction, Activity p_result)
        {
            SQLiteParameter[] paramList = new SQLiteParameter[12];
            string sqlText = @"Update rias_activity set name=:V_name,shorthand=:V_shorthand,datefrom=:V_datefrom,dateto=:V_dateto,icon=:V_icon,
                   organizer=:V_organizer,stage=:V_stage,description=:V_description,creator=:V_creator,createtime=:V_createtime,activitytype=:V_activitytype 
                    where guid=:V_guid";

            paramList[0] = new SQLiteParameter("V_name", DbType.String) { Value = p_result.Name };
            paramList[1] = new SQLiteParameter("V_shorthand", DbType.String) { Value = p_result.ShortHand };
            paramList[2] = new SQLiteParameter("V_datefrom", DbType.Date) { Value = p_result.DateFrom.ToString("s") };
            paramList[3] = new SQLiteParameter("V_dateto", DbType.Date) { Value = p_result.DateTo.ToString("s") };
            paramList[4] = new SQLiteParameter("V_icon", DbType.Binary) { Value = p_result.Icon };
            paramList[5] = new SQLiteParameter("V_organizer", DbType.String) { Value = p_result.Organizer };
            paramList[6] = new SQLiteParameter("V_stage", DbType.String) { Value = p_result.ActivityStage };
            paramList[7] = new SQLiteParameter("V_description", DbType.String) { Value = p_result.Description };
            paramList[8] = new SQLiteParameter("V_creator", DbType.String) { Value = p_result.Creator };
            paramList[9] = new SQLiteParameter("V_createtime", DbType.Date) { Value = p_result.CreateTime.ToString("s") };
            paramList[10] = new SQLiteParameter("V_activitytype", DbType.String) { Value = p_result.ActivityType };
            paramList[11] = new SQLiteParameter("V_guid", DbType.String) { Value = p_result.Guid };
            return SQLiteConnect.ExecuteNonQuery(p_transaction, sqlText, paramList);
        }

        /// <summary>
        /// 更新地点信息
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string UpdateActivityPlaceSql(ActivityPlaceInfo p_result)
        {
            return string.Format(@"Update activity_place set activity_guid='{0}',name='{1}',address='{2}',contact='{3}',phone='{4}',
                   image='{5}',graphics='{6}'
                    where guid='{7}'",
                    p_result.ActivityGuid, p_result.Name, p_result.Address, p_result.Contact, p_result.Phone, p_result.Image,
                    p_result.Graphics, p_result.Guid);
        }
        private static int UpdateActivityPlaceByParam(SQLiteTransaction p_transaction, ActivityPlaceInfo p_result)
        {
            SQLiteParameter[] paramList = new SQLiteParameter[8];
            string sqlText = @"Update activity_place set activity_guid=:V_activity_guid,name=:V_name,address=:V_address,contact=:V_contact,phone=:V_phone,
                   image=:V_image,graphics=:V_graphics
                    where guid=:V_guid";

            paramList[0] = new SQLiteParameter("V_guid", DbType.String) { Value = p_result.Guid };
            paramList[1] = new SQLiteParameter("V_activity_guid", DbType.String) { Value = p_result.ActivityGuid };
            paramList[2] = new SQLiteParameter("V_name", DbType.String) { Value = p_result.Name };
            paramList[3] = new SQLiteParameter("V_address", DbType.String) { Value = p_result.Address };
            paramList[4] = new SQLiteParameter("V_contact", DbType.String) { Value = p_result.Contact };
            paramList[5] = new SQLiteParameter("V_phone", DbType.String) { Value = p_result.Phone };
            paramList[6] = new SQLiteParameter("V_image", DbType.Binary) { Value = p_result.Image };
            paramList[7] = new SQLiteParameter("V_graphics", DbType.String) { Value = p_result.Graphics };
            return SQLiteConnect.ExecuteNonQuery(p_transaction, sqlText, paramList);
        }
        /// <summary>
        /// 更新地点坐标信息
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        private static string UpdateActivityPlaceLocationSql(ActivityPlaceLocation p_result)
        {
            return string.Format(@"Update activity_place_location set activity_place_guid='{0}',location_name='{1}',
                                   location_lg={2},location_la={3} where guid='{4}'",
                    p_result.ActivityPlaceGuid, p_result.LocationName, p_result.LocationLG, p_result.LocationLA, p_result.GUID);
        }
        private static string UpdateActivityFreqPlanSql(PlaceFreqPlan p_activityFreqPlan)
        {
            return string.Format(@"Update Activity_Freq_Plan set ACTIVITY_ID='{0}',MDISTANCETOPLACE='{1}',
                                   LONGITUDEFROM={2},LONGITUDETO={3},LATITUDEFROM={4},LATITUDETO={5}
, POINTS={6},NAME='{7}',EQUIPMENTCLASS='{8}',FREQFROM={9},FREQTO={10}，KHZBAND={11} where PLACE_ID='{12}' and freq_planning_id='{13}'",
                    p_activityFreqPlan.ActivityGuid, p_activityFreqPlan.mDistanceToActivityPlace,p_activityFreqPlan.LongitudeRange.Little,
                    p_activityFreqPlan.LongitudeRange.Great,p_activityFreqPlan.LatitudeRange.Little,p_activityFreqPlan.LatitudeRange.Great,
                    null,p_activityFreqPlan.Name,p_activityFreqPlan.EquipmentClassID,p_activityFreqPlan.MHzFreqFrom,
                    p_activityFreqPlan.MHzFreqTo,p_activityFreqPlan.kHzBand, p_activityFreqPlan.PlaceGuid,p_activityFreqPlan.Key );
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除分析结果记录
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        private static string DeleteAnalysisResultSql(string p_id,string p_placeGuid)
        {
            return string.Format(@"delete from ACTIVITY_ANALYSIS_RESULT where id='{0}' and  PLACE_GUID ='{1}'", p_id, p_placeGuid);
        }
        /// <summary>
        /// 删除发射信息记录
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        private static string DeleteEmitInfoSql(string p_statGuid)
        {
            return string.Format(@"delete from ACTIVITY_EMIT_INFO where STATION_GUID='{0}'", p_statGuid);
        }

        private static string DeleteStationBaseSql(string p_activityGuid,string p_placeGuid)
        {
            return string.Format(@"delete from ACTIVITY_STATION_BASE where PLACE_GUID='{0}' and ACTIVITY_GUID='{1}'", p_placeGuid, p_activityGuid);
        }

        private static string DeleteFreqPlanSql(string p_activityID, string p_placeGuid)
        {
            return string.Format(@"delete from Activity_Freq_Plan where ACTIVITY_ID='{0}' and PLACE_ID='{1}'", p_activityID, p_placeGuid);
        }
        private static string DeletePlaceSql(string p_activityGuid)
        {
            return string.Format(@"delete from ACTIVITY_PLACE where activity_guid='{0}'", p_activityGuid);
        }
        private static string DeletePlaceLocationSql(string p_placeGuid)
        {
            return string.Format(@"delete from ACTIVITY_PLACE_LOCATION where activity_place_guid='{0}'", p_placeGuid);
        }
        #endregion

        #endregion

        #region SQLite数据库文件操作
        public static bool IsExsitThisActivityFile(string p_fileName)
        {
            List<string> fileNames = SQLiteConnect.GetFileNames();
            if (fileNames.Contains(p_fileName + ".db"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 查询本地Output\SqliteData\Activities目录下所有文件，读取活动表，返回活动列表
        /// </summary>
        /// <returns></returns>
        public static List<Activity> QueryActivitiesFromLocal()
        {
            //返回Output\SqliteData\Activities中所有活动的列表
            List<Activity> activityList = new List<Activity>();
            List<string> fileNames = SQLiteConnect.GetFileNames();
            foreach (string fileName in fileNames)
            {
                _operationActivityName = fileName;
                try
                {
                    List<Activity> activities = QueryActivity();
                    if (activities != null && activities.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(_operationActivityName) && _operationActivityName.Substring(0,_operationActivityName.Length -3) == activities.FirstOrDefault().Name)
                        {
                            activityList.AddRange(activities);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("调用QueryActivitiesFromLocal方法抛出异常提示:" + ex.Message);
                    continue;
                }
            }
            return activityList;
        }
        
        /// <summary>
        /// 根据活动ID，将活动写入本地
        /// </summary>
        /// <param name="p_activityGuid"></param>
        public static void WriteActivityToLocal(string p_activityGuid)
        {

            //活动GUID为空，返回
            if (string.IsNullOrEmpty(p_activityGuid)) return;
            Activity activity = GetActivity(p_activityGuid);
            string fileName = string.IsNullOrEmpty(activity.Name) ? p_activityGuid : activity.Name;
            SQLiteConnect.CopyDBFile(fileName);
            List<ActivityPlaceInfo> placeInfoList = GetActivityPlaces(p_activityGuid);
            List<ActivityPlaceLocation> placeLocationList = GetActivityPlaceLocations(p_activityGuid);
            _transaction = SQLiteConnect.GetSQLiteTransaction(fileName);

            SaveActivity(activity);
            foreach (ActivityPlaceInfo placeInfo in placeInfoList)
            {
                SaveActivityPlace(placeInfo);
            }
            foreach (ActivityPlaceLocation placeLocation in placeLocationList)
            {
                SaveActivityPlaceLocation(placeLocation);
            }
            SQLiteDataService.SaveOperaterLog(CreateLog());
            _transaction.Commit();
        }

        private static OperateLog CreateLog()
        {
            OperateLog log = new OperateLog();
            log.Guid = Guid.NewGuid().ToString();
            log.Operater = CO_IA.Client.RiasPortal.Current.UserSetting.UserName;
            log.OperateDate = DateTime.Now;
            log.OperateType = OperateTypeEnum.Initialize;
            log.OperateTables = "RIAS_ACTIVITY,ACTIVITY_PLACE,ACTIVITY_PLACE_LOCATION";
            return log;
        }

        private static List<ActivityPlaceInfo> GetActivityPlaces(string p_activityGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, List<ActivityPlaceInfo>>(channel =>
            {
                return channel.QueryActivityPlace(p_activityGuid);
            });
        }

        private static List<ActivityPlaceLocation> GetActivityPlaceLocations(string p_activityGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, List<ActivityPlaceLocation>>(channel =>
            {
                return channel.QueryPlaceLocation(p_activityGuid);
            });
        }
        private static Activity GetActivity(string p_activityGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_ActivityManage, Activity>(channel =>
            {
                return channel.GetActivity(p_activityGuid);
            });
        }
        #endregion


        #region 测试数据

        private AnalysisResult GetAnalysisResult()
        {
            AnalysisResult rst = new AnalysisResult();
            rst.AmplitudeMaxValue = 36;
            rst.AmplitudeMidValue = 24;
            rst.BandWidth = 25;
            rst.EndFreq = 700;
            rst.FreqType = SignalTypeEnum.空闲;
            rst.Frequency = 370;
            rst.Id = "234733";
            rst.MeasureId = "MeasureId8888888";
            rst.Occupy = 1;
            rst.StartFreq = 150;
            rst.StationName = "西安1号台";
            return rst;
        }

        private EmitInfo GetEmitInfo()
        {
            EmitInfo emitInfo = new EmitInfo();
            emitInfo.AntEgain = 1.45;
            emitInfo.AntHeight = 36;
            emitInfo.AntPole = PolarEnum.H;
            emitInfo.EquPower = 80;
            emitInfo.FeedLose = 0.93;
            emitInfo.FreqBand = 0.25;
            emitInfo.FreqEc = 55;
            emitInfo.FreqMod = "方式1";
            emitInfo.FreqRc = 57;
            emitInfo.Guid = "G2339U33429I39488D33235";
            emitInfo.PlaceGuid = "G2339U33429I39488D99999";
            emitInfo.RcvAntEgain = 34;
            emitInfo.RcvAntHeight = 128;
            emitInfo.RcvFeedLose = 0.85;
            emitInfo.StatAt = 20;
            emitInfo.StationGuid = "G2339U33429I39488D88888";
            return emitInfo;
        }

        #endregion
    }
}
