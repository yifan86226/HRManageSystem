using CO_IA_Data;
using I_CO_IA.StationManage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO_IA.UI.Screen.Dialog
{
    /// <summary>
    /// StationListDialog.xaml 的交互逻辑
    /// </summary>
    public partial class StationListDialog : Window
    {
        double[] Range;
        private ObservableCollection<ActivitySurroundStation> activitystationitemssource = new ObservableCollection<ActivitySurroundStation>();

        public ObservableCollection<ActivitySurroundStation> ActivityStationItemsSource
        {
            get
            {
                return activitystationitemssource;
            }
            set
            {
                activitystationitemssource = value;
            }
        }
        public StationListDialog(string[] range)
        {
            InitializeComponent();
            if(range==null&&range.Length!=4)
                return;
            Range = new double[range.Length];
            for (int i = 0; i < range.Length;i++ )
            {
                Range[i] = double.Parse(range[i]);
            }
            this.Loaded += StationListDialog_Loaded;
        }

        void StationListDialog_Loaded(object sender, RoutedEventArgs e)
        {
            GetStation();
        }
        private void GetStation()
        {
            if (Range == null)
                return;
            List<StationInfo> statinfoLs = new List<StationInfo>();
            List<QUERY_PARAMS> lsParams = new List<QUERY_PARAMS>();
            QUERY_PARAMS param = null;
            param = new QUERY_PARAMS();
            param.PARAMS_NAME = "rownum";
            param.PARAMS_RELATION = "";
            param.PARAMS_VALUE = "rownum<=100";
            param.PARAMS_UNIT = "";
            param.PARAMS_TYPE = "varchar";
            lsParams.Add(param);
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_StationManage>(channel =>
            {
                statinfoLs = channel.GetStationBaseInfo(lsParams, new Point[] { new Point(Range[0], Range[1]), new Point(Range[2], Range[1]), new Point(Range[2], Range[3]), new Point(Range[0], Range[3]) }, new Point(Range[0], Range[1]), new Point(Range[2], Range[3]));
                CreateSurroundStation(statinfoLs);
                surroundStationListControl.DataContext = ActivityStationItemsSource;
            });
        }

        /// <summary>
        /// 创建周围台站
        /// </summary>
        /// <returns></returns>
        private void CreateSurroundStation(List<StationInfo> stations)
        {
            foreach (StationInfo station in stations)
            {
                 //去掉重复
                int count = ActivityStationItemsSource.Count(r => r.STATGUID == station.STATGUID);
                if (count == 0)
                {
                    string statguid = station.STATGUID;
                    string stattype = station.STAT_APP_TYPE;
                    //List<StationEmitInfo> stationEmit = GetStationEmitInfo(statguid, stattype);

                    StationInfo stat = new SurroundStationInfo();
                    StationClone(station, stat);
                    SurroundStationInfo surstat = (SurroundStationInfo)stat;
                    //surstat.EmitInfo = stationEmit;

                    SurroundStationInfo asurstat = new ActivitySurroundStation();
                    StationClone(surstat, asurstat);
                    ActivitySurroundStation activitystation = (ActivitySurroundStation)asurstat;
                    //activitystation.ActivityId = this.CurrentActivityPlace.ActivityGuid;
                    //activitystation.PlaceId = this.CurrentActivityPlace.Guid;

                    ActivityStationItemsSource.Add(activitystation);
                }
            }
        }

        public void StationClone<T>(T source, T taeget)
        {
            System.Reflection.PropertyInfo[] properties1 = source.GetType().GetProperties();
            System.Reflection.PropertyInfo[] properties2 = taeget.GetType().GetProperties();

            foreach (PropertyInfo sourceproperty in properties1)
            {
                PropertyInfo pro = properties2.FirstOrDefault(r => r.Name == sourceproperty.Name && r.PropertyType == sourceproperty.PropertyType);
                if (pro != null)
                {
                    object value = sourceproperty.GetValue(source, null);
                    pro.SetValue(taeget, value, null);
                }
            }
        }
        private List<StationEmitInfo> GetStationEmitInfo(string pStatId, string pStatAppType)
        {
            List<StationEmitInfo> stationEmitInfo = new List<StationEmitInfo>();
            string strStation = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_StationManage, string>(channel =>
            {
                return channel.SearchStationByGuid(pStatId);
            });
            if (!string.IsNullOrEmpty(strStation))
            {
                System.IO.StringReader xmlSR = new System.IO.StringReader(strStation);
                System.Data.DataSet CurrentResult = new System.Data.DataSet();
                MemoryStream ms = new MemoryStream(Convert.FromBase64String(strStation));
                CurrentResult.ReadXml(ms, XmlReadMode.Auto);
                if (CurrentResult.Tables.Count > 0)
                {
                    stationEmitInfo = CreateStationEmitInfo(CurrentResult, pStatId, pStatAppType);
                }
            }
            return stationEmitInfo;
        }
        private List<StationEmitInfo> CreateStationEmitInfo(DataSet ds, string pStatId, string pStatAppType)
        {
            List<StationEmitInfo> stationEmitInfos = null;
            if (ds == null || ds.Tables.Count == 0)
            {
                return stationEmitInfos;
            }

            #region 取数据
            if (ds.Tables.Count > 0)
            {
                //频率表
                if (ds.Tables["RSBT_FREQ"] != null && ds.Tables["RSBT_FREQ"].Rows.Count > 0)
                {
                    stationEmitInfos = FreqInfoToEmit(pStatId, ds.Tables["RSBT_FREQ"]);
                    stationEmitInfos = stationEmitInfos.OrderBy(p => p.Guid).ToList();
                }
                if (stationEmitInfos == null || stationEmitInfos.Count == 0) return stationEmitInfos;
                //台站资料表
                if (ds.Tables["RSBT_STATION"] != null && ds.Tables["RSBT_STATION"].Rows.Count > 0)
                {
                    StationInfoToEmit(ds.Tables["RSBT_STATION"], ref stationEmitInfos);
                }
                //eaf对应关系ccode赋值
                if (pStatAppType == "TF" || pStatAppType == "C" || pStatAppType == "E")
                {
                    //频率冗余表
                    if (ds.Tables["RSBT_FREQ_T"] != null && ds.Tables["RSBT_FREQ_T"].Rows.Count > 0)
                    {
                        DataView dv = ds.Tables["RSBT_FREQ_T"].DefaultView;
                        dv.Sort = "GUID";
                        Freq_TtoEmit(dv.ToTable(), ref stationEmitInfos, pStatAppType);
                    }
                }
                //天线信息赋值
                if (pStatAppType == "TF" || pStatAppType == "C")
                {
                    //TF及C表的天线信息
                    if (ds.Tables["RSBT_ANTFEED"] != null && ds.Tables["RSBT_ANTFEED"].Rows.Count > 0 &&
                        ds.Tables["RSBT_ANTFEED_T"] != null && ds.Tables["RSBT_ANTFEED_T"].Rows.Count > 0)
                    {
                        CAndTFAntToEmit(ds.Tables["RSBT_ANTFEED"], ds.Tables["RSBT_ANTFEED_T"], ref stationEmitInfos, pStatAppType);
                    }
                }
                else
                {
                    //其他表天线赋值
                    if (ds.Tables["RSBT_ANTFEED"] != null && ds.Tables["RSBT_ANTFEED"].Rows.Count > 0)
                    {
                        AntToEmit(ds.Tables["RSBT_ANTFEED"], ref stationEmitInfos, pStatAppType);
                    }
                }

                //设备功率信息赋值
                if (pStatAppType != "E")
                {
                    if (pStatAppType == "TF" || pStatAppType == "C")
                    {
                        //TF及C表设备功率信息
                        if (ds.Tables["RSBT_EQU"] != null && ds.Tables["RSBT_EQU"].Rows.Count > 0 &&
                            ds.Tables["RSBT_EQU_T"] != null && ds.Tables["RSBT_EQU_T"].Rows.Count > 0)
                        {
                            CAndTFEquToEmit(ds.Tables["RSBT_EQU"], ds.Tables["RSBT_EQU_T"], ref stationEmitInfos);
                        }
                    }
                    else
                    {
                        //其他表设备功率
                        if (ds.Tables["RSBT_EQU"] != null && ds.Tables["RSBT_EQU"].Rows.Count > 0)
                        {
                            EquToEmit(ds.Tables["RSBT_EQU"], ref stationEmitInfos);
                        }
                    }
                }

            }
            #endregion
            return stationEmitInfos;
        }

        #region 频率信息
        /// <summary>
        /// 频率信息转为发射信息
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private List<StationEmitInfo> FreqInfoToEmit(string pStatId, DataTable dataTable)
        {
            List<StationEmitInfo> stationEmits = new List<StationEmitInfo>();
            foreach (DataRow dr in dataTable.Rows)
            {
                try
                {
                    StationEmitInfo stationEmit = new StationEmitInfo();
                    stationEmit.StationGuid = pStatId;
                    //stationEmit.PlaceGuid = this.CurrentActivityPlace.Guid;
                    if (dataTable.Columns.Contains("GUID"))
                    {
                        stationEmit.Guid = dr["GUID"].ToString();
                    }

                    //频段 :发射FREQ_EFB、FREQ_EFE   接收FREQ_RFB、FREQ_RFE
                    //频点 :发射FREQ_UC、接收FREQ_LC
                    if (dataTable.Columns.Contains("FREQ_TYPE"))
                    {
                        if (dr["FREQ_TYPE"].ToString() == "1")
                        {
                            stationEmit.FreqType = FreqType.频段;

                            double freqEfb = 0;
                            if (dataTable.Columns.Contains("FREQ_EFB"))
                            {
                                double.TryParse(dr["FREQ_EFB"].ToString(), out freqEfb);
                                stationEmit.FreqEFB = freqEfb;
                            }

                            double freqEfe = 0;
                            if (dataTable.Columns.Contains("FREQ_EFE"))
                            {
                                double.TryParse(dr["FREQ_EFE"].ToString(), out freqEfe);
                                stationEmit.FreqEFE = freqEfe;
                            }

                            double freqRfb = 0;
                            if (dataTable.Columns.Contains("FREQ_RFB"))
                            {
                                double.TryParse(dr["FREQ_RFB"].ToString(), out freqRfb);
                                stationEmit.FreqRFB = freqRfb;
                            }

                            double freqRfe = 0;
                            if (dataTable.Columns.Contains("FREQ_RFE"))
                            {
                                double.TryParse(dr["FREQ_RFE"].ToString(), out freqRfe);
                                stationEmit.FreqRFE = freqRfe;
                            }
                        }
                        else
                        {
                            stationEmit.FreqType = FreqType.频点;
                            double freqUc = 0;
                            if (dataTable.Columns.Contains("FREQ_UC"))
                            {
                                double.TryParse(dr["FREQ_UC"].ToString(), out freqUc);
                                stationEmit.FreqEC = freqUc;
                            }

                            double freqLc = 0;
                            if (dataTable.Columns.Contains("FREQ_LC"))
                            {
                                double.TryParse(dr["FREQ_LC"].ToString(), out freqLc);
                                stationEmit.FreqRC = freqLc;
                            }
                            stationEmit.FreqEFB = null;
                            stationEmit.FreqEFE = null;
                            stationEmit.FreqRFB = null;
                            stationEmit.FreqRFE = null;
                        }
                    }
                    else
                    {
                        //stationEmit.FreqType = null;
                    }

                    #region 注释以前获取频率的方法

                    #region 频率发射
                    //stationEmit.FreqEC = 0;
                    //if (dataTable.Columns.Contains("FREQ_UC"))
                    //{
                    //    double freqUc = 0;
                    //    double.TryParse(dr["FREQ_UC"].ToString(), out freqUc);
                    //    if (freqUc == 0 || double.IsNaN(freqUc))
                    //    {
                    //        if (dataTable.Columns.Contains("FREQ_EFB"))
                    //        {
                    //            freqUc = 0;

                    //          
                    //            double.TryParse(dr["FREQ_EFB"].ToString(), out freqUc);
                    //            if (freqUc == 0 || double.IsNaN(freqUc))
                    //            {
                    //                if (dataTable.Columns.Contains("FREQ_EFE"))
                    //                {
                    //                    freqUc = 0;
                    //                    double.TryParse(dr["FREQ_EFE"].ToString(), out freqUc);
                    //                    if (!double.IsNaN(freqUc))
                    //                    {
                    //                        stationEmit.FreqEC = freqUc;
                    //                    }
                    //                }
                    //            }
                    //            else
                    //                stationEmit.FreqEC = freqUc;
                    //        }
                    //    }
                    //    else
                    //        stationEmit.FreqEC = freqUc;
                    //}
                    //else if (dataTable.Columns.Contains("FREQ_EFB"))
                    //{
                    //    double freqUc = 0;
                    //    double.TryParse(dr["FREQ_EFB"].ToString(), out freqUc);
                    //    if (freqUc == 0 || double.IsNaN(freqUc))
                    //    {
                    //        if (dataTable.Columns.Contains("FREQ_EFE"))
                    //        {
                    //            freqUc = 0;
                    //            double.TryParse(dr["FREQ_EFE"].ToString(), out freqUc);
                    //            if (!double.IsNaN(freqUc))
                    //            {
                    //                stationEmit.FreqEC = freqUc;
                    //            }
                    //        }
                    //    }
                    //    else
                    //        stationEmit.FreqEC = freqUc;
                    //}
                    //else if (dataTable.Columns.Contains("FREQ_EFE"))
                    //{
                    //    double freqUc = 0;
                    //    double.TryParse(dr["FREQ_EFE"].ToString(), out freqUc);
                    //    if (!double.IsNaN(freqUc))
                    //    {
                    //        stationEmit.FreqEC = freqUc;
                    //    }
                    //}
                    #endregion 频率发射结束

                    #region 频率接收
                    //stationEmit.FreqRC = 0;
                    //if (dataTable.Columns.Contains("FREQ_LC"))
                    //{
                    //    double freqUc = 0;
                    //    double.TryParse(dr["FREQ_LC"].ToString(), out freqUc);
                    //    if (freqUc == 0 || double.IsNaN(freqUc))
                    //    {
                    //        if (dataTable.Columns.Contains("FREQ_RFB"))
                    //        {
                    //            freqUc = 0;
                    //            double.TryParse(dr["FREQ_RFB"].ToString(), out freqUc);
                    //            if (freqUc == 0 || double.IsNaN(freqUc))
                    //            {
                    //                if (dataTable.Columns.Contains("FREQ_RFE"))
                    //                {
                    //                    freqUc = 0;
                    //                    double.TryParse(dr["FREQ_RFE"].ToString(), out freqUc);
                    //                    if (!double.IsNaN(freqUc))
                    //                    {
                    //                        stationEmit.FreqRC = freqUc;
                    //                    }
                    //                }
                    //            }
                    //            else
                    //                stationEmit.FreqRC = freqUc;
                    //        }
                    //    }
                    //    else
                    //        stationEmit.FreqRC = freqUc;
                    //}
                    //else if (dataTable.Columns.Contains("FREQ_RFB"))
                    //{
                    //    double freqUc = 0;
                    //    double.TryParse(dr["FREQ_RFB"].ToString(), out freqUc);
                    //    if (freqUc == 0 || double.IsNaN(freqUc))
                    //    {
                    //        if (dataTable.Columns.Contains("FREQ_RFE"))
                    //        {
                    //            freqUc = 0;
                    //            double.TryParse(dr["FREQ_RFE"].ToString(), out freqUc);
                    //            if (!double.IsNaN(freqUc))
                    //            {
                    //                stationEmit.FreqRC = freqUc;
                    //            }
                    //        }
                    //    }
                    //    else
                    //        stationEmit.FreqRC = freqUc;
                    //}
                    //else if (dataTable.Columns.Contains("FREQ_RFE"))
                    //{
                    //    double freqUc = 0;
                    //    double.TryParse(dr["FREQ_RFE"].ToString(), out freqUc);
                    //    if (!double.IsNaN(freqUc))
                    //    {
                    //        stationEmit.FreqRC = freqUc;
                    //    }
                    //}
                    #endregion 频率接收结束

                    #endregion

                    //频率带宽
                    stationEmit.FreqBand = 0;
                    if (dataTable.Columns.Contains("FREQ_E_BAND"))
                    {
                        double freqBand = 0;
                        double.TryParse(dr["FREQ_E_BAND"].ToString(), out freqBand);
                        stationEmit.FreqBand = freqBand;
                    }
                    //调制方式
                    if (dataTable.Columns.Contains("FREQ_MOD"))
                        stationEmit.FreqMod = dr["FREQ_MOD"].ToString();
                    if (stationEmits.FirstOrDefault(p => p.FreqEC == stationEmit.FreqEC) == null)   //过滤重复的发射频率
                        stationEmits.Add(stationEmit);
                }
                catch
                {

                }
            }
            return stationEmits;
        }

        #endregion

        #region 台站信息
        /// <summary>
        /// 台站信息
        /// </summary>
        /// <param name="dataTable"></param>
        private void StationInfoToEmit(DataTable dataTable, ref List<StationEmitInfo> stationEmitlist)
        {
            foreach (var emit in stationEmitlist)
            {
                double statAt = 0;
                emit.StatAT = 0;
                if (dataTable.Columns.Contains("STAT_AT") &&
                    double.TryParse(dataTable.Rows[0]["STAT_AT"].ToString(), out statAt))
                    //海拔高度
                    emit.StatAT = statAt;
            }
        }
        #endregion

        #region C表、TF表以外的天线信息
        /// <summary>
        /// 天线信息
        /// </summary>
        /// <param name="dataTable"></param>
        private void AntToEmit(DataTable dataTable, ref List<StationEmitInfo> freqEmitlist, string pStatAppType)
        {
            foreach (var emit in freqEmitlist)
            {
                double antValue = 0;
                //天线高度
                emit.AntHight = 0;
                //接收天线高度
                emit.RCVAntHight = 0;
                if (dataTable.Columns.Contains("ANT_HIGHT") &&
                double.TryParse(dataTable.Rows[0]["ANT_HIGHT"].ToString(), out antValue))
                {
                    emit.AntHight = antValue;
                    emit.RCVAntHight = antValue;
                }
                //天线增益
                antValue = 0;
                if (dataTable.Columns.Contains("ANT_EGAIN") && double.TryParse(dataTable.Rows[0]["ANT_EGAIN"].ToString(), out antValue))
                    emit.AntEgain = antValue;
                else if (dataTable.Columns.Contains("ANT_GAIN"))
                {
                    double.TryParse(dataTable.Rows[0]["ANT_GAIN"].ToString(), out antValue);
                    emit.AntEgain = antValue;
                }
                //馈线损耗
                emit.FeedLose = 0;
                //接收天线损耗
                emit.RCVFeedLose = 0;
                antValue = 0;
                if (dataTable.Columns.Contains("FEED_LOSE") &&
                    double.TryParse(dataTable.Rows[0]["FEED_LOSE"].ToString(), out antValue))
                {
                    emit.FeedLose = antValue;
                    //接收天线损耗
                    emit.RCVFeedLose = antValue;
                }
                //接收天线增益
                antValue = 0;
                emit.RCVAntEgain = 0;
                if (dataTable.Columns.Contains("ANT_RGAIN") && double.TryParse(dataTable.Rows[0]["ANT_RGAIN"].ToString(), out antValue))
                    emit.RCVAntEgain = antValue;
                if (pStatAppType != "TF" && dataTable.Columns.Contains("ANT_POLE"))
                    emit.AntPole = dataTable.Rows[0]["ANT_POLE"].ToString();
            }
        }
        #endregion

        #region C表及TF表天线信息
        /// <summary>
        /// C表及TF表天线信息
        /// </summary>
        /// <param name="dataTable"></param>
        private void CAndTFAntToEmit(DataTable dataTable, DataTable dataTable_T, ref List<StationEmitInfo> freqEmitlist, string pStatAppType)
        {
            foreach (var emit in freqEmitlist)
            {
                if (dataTable_T.Columns.Contains("AT_CCODE"))
                {
                    var enumTable = (from a in dataTable.AsEnumerable()
                                     join b in dataTable_T.AsEnumerable()
                                     on a.Field<string>("GUID") equals
                                         b.Field<string>("GUID")
                                     where b["AT_CCODE"].ToString() == emit.Ccode     //.Field<string>(
                                     select new
                                     {
                                         AntHight = dataTable.Columns.Contains("ANT_HIGHT") ? (a["ANT_HIGHT"] != DBNull.Value ? a["ANT_HIGHT"].ToString() : "0") : "0",
                                         AntGain = dataTable.Columns.Contains("ANT_GAIN") ? (a["ANT_GAIN"] != DBNull.Value ? a["ANT_GAIN"].ToString() : "0") : "0",
                                         AntEGain = dataTable.Columns.Contains("ANT_EGAIN") ? (a["ANT_EGAIN"] != DBNull.Value ? a["ANT_EGAIN"].ToString() : "0") : "0",
                                         AntLose = dataTable.Columns.Contains("FEED_LOSE") ? (a["FEED_LOSE"] != DBNull.Value ? a["FEED_LOSE"].ToString() : "0") : "0",
                                         AntRGain = dataTable.Columns.Contains("ANT_RGAIN") ? (a["ANT_RGAIN"] != DBNull.Value ? a["ANT_RGAIN"].ToString() : "0") : "0",
                                         AntPole = dataTable.Columns.Contains("ANT_POLE") ? a["ANT_POLE"].ToString() : ""
                                     }).ToArray();
                    if (enumTable != null && enumTable.Length > 0)
                    {
                        //天线高度
                        emit.AntHight = 0;
                        //接收天线高度
                        emit.RCVAntHight = 0;
                        //天线增益
                        emit.AntEgain = 0;
                        //馈线损耗
                        emit.FeedLose = 0;
                        //接收天线损耗
                        emit.RCVFeedLose = 0;
                        //接收天线增益
                        emit.RCVAntEgain = 0;
                        /////////////////////////////////////////////////////
                        double antValue = 0;
                        //天线高度 //接收天线高度
                        if (double.TryParse(enumTable[0].AntHight, out antValue))
                        {
                            emit.AntHight = antValue;
                            emit.RCVAntHight = antValue;
                        }
                        //天线增益
                        if (double.TryParse(enumTable[0].AntEGain, out antValue))
                        {
                            if (antValue == 0)
                            {
                                if (double.TryParse(enumTable[0].AntGain, out antValue))
                                    emit.AntEgain = antValue;
                            }
                            else
                                emit.AntEgain = antValue;

                        }
                        if (double.TryParse(enumTable[0].AntLose, out antValue))
                        {
                            //馈线损耗
                            emit.FeedLose = antValue;
                            //接收天线损耗
                            emit.RCVFeedLose = antValue;
                        }
                        //接收天线增益
                        if (double.TryParse(enumTable[0].AntRGain, out antValue))
                            emit.RCVAntEgain = antValue;
                        //极化方式
                        if (pStatAppType == "C")
                            emit.AntPole = enumTable[0].AntPole;
                    }
                }
            }
        }
        #endregion

        #region C表、TF表及E表以外的设备功率信息
        /// <summary>
        /// 设备功率信息
        /// </summary>
        /// <param name="dataTable"></param>
        private void EquToEmit(DataTable dataTable, ref List<StationEmitInfo> freqEmitlist)
        {
            foreach (var emit in freqEmitlist)
            {
                double equPow = 0;
                emit.EquPow = 0;
                if (dataTable.Columns.Contains("EQU_POW") &&
                    double.TryParse(dataTable.Rows[0]["EQU_POW"].ToString(), out equPow))
                    //功率
                    emit.EquPow = equPow;
            }
        }
        #endregion

        #region C表及TF表设备功率信息
        /// <summary>
        /// C表及TF表设备功率信息
        /// </summary>
        /// <param name="dataTable"></param>
        private void CAndTFEquToEmit(DataTable dataTable, DataTable dataTable_T, ref List<StationEmitInfo> freqEmitlist)
        {
            foreach (var emit in freqEmitlist)
            {
                emit.EquPow = 0;
                if (dataTable_T.Columns.Contains("ET_EQU_CCODE"))
                {
                    var ePows = (from a in dataTable.AsEnumerable()
                                 join b in dataTable_T.AsEnumerable()
                                 on a.Field<string>("GUID") equals
                                     b.Field<string>("GUID")
                                 where b["ET_EQU_CCODE"].ToString() == emit.Ccode
                                 select new
                                 {
                                     EPow = dataTable.Columns.Contains("EQU_POW") ? (a["EQU_POW"] != DBNull.Value ? a["EQU_POW"].ToString() : "0") : "0"
                                 }).ToArray();
                    if (ePows != null && ePows.Length > 0)
                    {
                        //功率
                        emit.EquPow = 0;
                        double equPow = 0;
                        if (double.TryParse(ePows[0].EPow, out equPow))
                            emit.EquPow = equPow;
                    }
                }
            }
        }
        #endregion

        #region 频率冗余数据及E表功率信息查询
        /// <summary>
        /// 频率冗余
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="freqEmitlist"></param>
        private void Freq_TtoEmit(DataTable dataTable, ref List<StationEmitInfo> freqEmitlist, string pStatAppType)
        {
            if (pStatAppType == "C")
            {
                int i = 0;
                foreach (var emit in freqEmitlist)
                {
                    for (; i < dataTable.Rows.Count; )
                    {
                        int result = emit.Guid.CompareTo(dataTable.Rows[i]["GUID"].ToString());
                        if (result == 0)
                        {
                            emit.Ccode = dataTable.Columns.Contains("FT_FREQ_CCODE") ? dataTable.Rows[i]["FT_FREQ_CCODE"].ToString() : i.ToString();
                            i++;
                            break;
                        }
                        else if (result > 0)
                        {
                            i++;
                        }
                        else
                            break;
                    }
                }
            }
            else if (pStatAppType == "TF")
            {
                int i = 0;
                foreach (var emit in freqEmitlist)
                {
                    for (; i < dataTable.Rows.Count; )
                    {
                        int result = emit.Guid.CompareTo(dataTable.Rows[i]["GUID"].ToString());
                        if (result == 0)
                        {
                            emit.Ccode = dataTable.Columns.Contains("FT_FREQ_CCODE") ? dataTable.Rows[i]["FT_FREQ_CCODE"].ToString() : i.ToString();
                            emit.AntPole = dataTable.Columns.Contains("FT_FREQ_FEP") ? dataTable.Rows[i]["FT_FREQ_FEP"].ToString() : ""; ;
                            i++;
                            break;
                        }
                        else if (result > 0)
                        {
                            i++;
                        }
                        else
                            break;
                    }
                }
            }
            else if (pStatAppType == "E")
            {
                int i = 0;
                foreach (var emit in freqEmitlist)
                {
                    for (; i < dataTable.Rows.Count; )
                    {
                        int result = emit.Guid.CompareTo(dataTable.Rows[i]["GUID"].ToString());
                        if (result == 0)
                        {
                            double equPow = 0;
                            emit.EquPow = 0;
                            if (dataTable.Columns.Contains("FT_FREQ_EPOW") &&
                                double.TryParse(dataTable.Rows[i]["FT_FREQ_EPOW"].ToString(), out equPow))
                                //功率
                                emit.EquPow = equPow;
                            i++;
                            break;
                        }
                        else if (result > 0)
                        {
                            i++;
                        }
                        else
                            break;
                    }
                }
            }
        }
        #endregion
    }
}
