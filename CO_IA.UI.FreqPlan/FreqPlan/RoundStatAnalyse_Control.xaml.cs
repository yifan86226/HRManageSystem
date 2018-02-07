using CO_IA.Data;
using CO_IA.UI.FreqPlan.StationPlan;
using CO_IA.UI.Setting;
using CO_IA_Data;
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
using CO_IA.UI.StationManage;
using System.Collections.ObjectModel;
using I_CO_IA.StationManage;
using System.Data;
using CO_IA.Client;
using System.Threading;

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// RoundStatAnalyse_Control.xaml 的交互逻辑
    /// </summary>
    public partial class RoundStatAnalyse_Control : UserControl
    {
        private string _activityPlaceId;
        public event Action<List<StationInfo>> OnDrawStationToMap;
        public RoundStatAnalyse_Control()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public string ActivityPlaceId
        {
            get { return _activityPlaceId; }
            set
            {
                _activityPlaceId = value;
                StationItemsSource = GetSavedRoundStations(value);
                //绘制台站
                if (OnDrawStationToMap != null && StationItemsSource != null)
                {
                    List<StationInfo> stationlist = new List<StationInfo>();
                    for (int i = 0; i < StationItemsSource.Count; i++)
                        stationlist.Add(StationItemsSource[i]);
                    OnDrawStationToMap(stationlist);
                }
            }
        }

        public List<RoundStationInfo> StationItemsSource
        {
            get { return (List<RoundStationInfo>)GetValue(StationItemsSourceProperty); }
            set { SetValue(StationItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StationItemsSourceProperty =
            DependencyProperty.Register("StationItemsSource", typeof(List<RoundStationInfo>), typeof(RoundStatAnalyse_Control), new PropertyMetadata(new PropertyChangedCallback(callback )));

        private static void callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             
        }

        private void xbtnQuery_Click(object sender, RoutedEventArgs e)
        {
            QueryFreqPartPlanDialog queryDialog = new QueryFreqPartPlanDialog(_activityPlaceId);
            queryDialog.OnSelectList += queryDialog_OnSelectList;
            queryDialog.ShowDialog(this);
        }
        //循环查询周围台站
        void queryDialog_OnSelectList(List<FreqPlanActivity> obj)
        {
            busyIndicator.IsBusy = true;
            ThreadArg arg = new ThreadArg();
            arg.FreqActivitys = obj;
            arg.OldRoundStations = StationItemsSource;
            arg.SynchContext = System.Threading.SynchronizationContext.Current;
            arg.OnNotifyRoundStations += p =>
                {
                    busyIndicator.IsBusy = false;
                    //更新数据源
                    StationItemsSource = p;
                    //绘制台站
                    if (OnDrawStationToMap != null && StationItemsSource !=null)
                    {
                        List<StationInfo> stationlist = new List<StationInfo>();
                        for (int i = 0; i < StationItemsSource.Count; i++)
                            stationlist.Add(StationItemsSource[i]);
                        OnDrawStationToMap(stationlist);
                    }
                        
                };
            Thread thread = new Thread(ThreadExec);
            thread.IsBackground = true;
            thread.Start(arg);
        }

        #region 查询远程台站信息线程结构
        private void ThreadExec(object pArg)
        {
            ThreadArg arg = pArg as ThreadArg;
            List<RoundStationInfo> roundStats = GetRemoteRoundStations(arg.FreqActivitys, arg.OldRoundStations);
            arg.ReturnStationList(roundStats);
        }
        private class ThreadArg
        {
            public event Action<List<RoundStationInfo>> OnNotifyRoundStations;
            public List<FreqPlanActivity> FreqActivitys
            {
                get;
                set;
            }
            public List<RoundStationInfo> OldRoundStations
            {
                get;
                set;
            }
            public System.Threading.SynchronizationContext SynchContext
            {
                get;
                set;
            }

            public void ReturnStationList(List<RoundStationInfo> pRStations)
            {
                if (OnNotifyRoundStations != null)
                {
                    this.SynchContext.Send(p =>
                    {
                        this.OnNotifyRoundStations(pRStations);
                    }, null);
                }
            }
        }

        #endregion

        #region 查询远程台站信息及信息处理
        private List<RoundStationInfo> GetRemoteRoundStations(List<FreqPlanActivity> obj,List<RoundStationInfo> pOldRoundStations)
        {
            List<RoundStationInfo> roundStations = new List<RoundStationInfo>();
            Dictionary<string, FreqPlanSegment> statFreqDic = new Dictionary<string, FreqPlanSegment>();
            List<StationInfo> stationInfos = new List<StationInfo>();
            for (int i = 0; i < obj.Count; i++)
            {
//#warning 以下注释信息为实际代码，当前用的是测试代码
                Point[] points = new Point[obj[i].Points.Length];
                for (int j = 0; j < obj[i].Points.Length; j++)
                {
                    points[j] = new Point();
                    points[j].X = obj[i].Points[j].X;
                    points[j].Y = obj[i].Points[j].Y;
                }
                Point startPoint = new Point();
                startPoint.X = obj[i].StartPoint.X;
                startPoint.Y = obj[i].StartPoint.Y;
                Point endPoint = new Point();
                endPoint.X = obj[i].EndPoint.X;
                endPoint.Y = obj[i].EndPoint.Y;
                //stationInfos.AddRange(xStationQueryControl.QueryStationData(points, startPoint, endPoint, obj[i].ClassCode.Length >= 4 ? obj[i].ClassCode.Substring(0, 4) : ""));
                ////测试数据
                //Point[] points = new Point[10];
                //points[0] = new Point();
                //points[0].X = 114.4401000;
                //points[0].Y = 36.6189000;

                //points[1] = new Point();
                //points[1].X = 114.4401100;
                //points[1].Y = 36.6189100;

                //points[2] = new Point();
                //points[2].X = 114.4402778;
                //points[2].Y = 36.6105556;

                //points[3] = new Point();
                //points[3].X = 114.4402778;
                //points[3].Y = 36.6283333;

                //points[4] = new Point();
                //points[4].X = 114.4403700;
                //points[4].Y = 36.6283800;

                //points[5] = new Point();
                //points[5].X = 114.4404240;
                //points[5].Y = 36.6283800;

                //points[6] = new Point();
                //points[6].X = 114.4404560;
                //points[6].Y = 336.6283800;

                //points[7] = new Point();
                //points[7].X = 114.4405100;
                //points[7].Y = 36.6283800;

                //points[8] = new Point();
                //points[8].X = 114.4407300;
                //points[8].Y = 36.6283800;

                //points[9] = new Point();
                //points[9].X = 114.4408;
                //points[9].Y = 36.63;

                //Point startPoint = new Point();
                //startPoint.X = 114.4398;
                //startPoint.Y = 36.61;
                //Point endPoint = new Point();
                //endPoint.X = 114.4408;
                //endPoint.Y = 36.63;
                string freqRange = (obj[i].FreqValue.Little/1000000).ToString()+";"+(obj[i].FreqValue.Great/1000000).ToString();
                //string freqRange = "";
                List<StationInfo> tempStatInfos = QueryStationData(points, startPoint, endPoint, freqRange);
                if (tempStatInfos != null && tempStatInfos.Count > 0)
                {
                    for (int j = 0; j < tempStatInfos.Count; j++)
                    {
                        if (!statFreqDic.Keys.Contains(tempStatInfos[j].STATGUID) &&  (string.IsNullOrEmpty(tempStatInfos[j].NET_SVN) ||
                            (tempStatInfos[j].NET_SVN.Length >=4 && obj[i].ClassCode.Length >=4 &&
                            tempStatInfos[j].NET_SVN.Substring(0, 4) == obj[i].ClassCode.Substring(0, 4))))
                        {
                            FreqPlanSegment freqSegment = obj[i];
                            statFreqDic.Add(tempStatInfos[j].STATGUID, freqSegment);
                            stationInfos.Add(tempStatInfos[j]);
                        }
                    }
                }
            }
            //过滤台站,并赋数据源
            List<StationInfo> statlist = stationInfos.Where((x, i) => stationInfos.FindIndex(z => z.STATGUID == x.STATGUID) == i).ToList();
            
            //数据结构赋值
            List<RoundStationInfo> tempRoundStats = pOldRoundStations == null ? new List<RoundStationInfo>() : pOldRoundStations;
            if (statlist != null && statlist.Count > 0) 
                tempRoundStats.AddRange(StationInfoConvert(statlist, obj, statFreqDic));
            //更新数据源
            roundStations = tempRoundStats.Where((x, i) => tempRoundStats.FindIndex(z => z.STATGUID == x.STATGUID) == i).ToList();
            return roundStations;
        }
              
        /// <summary>
        /// 台站信息转周围台站信息
        /// </summary>
        /// <param name="pStationInfos"></param>
        /// <param name="pFreqPlans"></param>
        /// <param name="pStatFreqDic"></param>
        /// <returns></returns>
        private List<RoundStationInfo> StationInfoConvert(List<StationInfo> pStationInfos, List<FreqPlanActivity> pFreqPlans, Dictionary<string, FreqPlanSegment> pStatFreqDic)
        {
            List<RoundStationInfo> roundStations = new List<RoundStationInfo>(pStationInfos.Count);
            for (int i = 0; i < pStationInfos.Count; i++)
            {
                StationInfo stationInfo = new RoundStationInfo();
                StructClone.ClassClone(pStationInfos[i], ref stationInfo);
                ((RoundStationInfo)stationInfo).ActivityId = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
                ((RoundStationInfo)stationInfo).PlaceId = _activityPlaceId;
                ((RoundStationInfo)stationInfo).FreqActivityGuid = _activityPlaceId;
                //获取台站发射信息
                List<FreqEmitInfo> emitInfos = GetStationSystemEmitInfo(pStationInfos[i].STATGUID, pStationInfos[i].STAT_APP_TYPE);
                if (emitInfos != null && emitInfos.Count > 0)
                {
                    foreach (var emit in emitInfos)
                    {
                        emit.StationGuid = pStationInfos[i].STATGUID;
                        emit.PlaceGuid = _activityPlaceId;
                        emit.NeedClear = NeedClearEunm.NotNeedClear;
                        emit.ClearResult = ClearResultEnum.NotClear;
                    }
                }
                ((RoundStationInfo)stationInfo).EmitInfos = emitInfos;
                FreqPlanSegment freqSegment = null;
                if (pStatFreqDic.TryGetValue(((RoundStationInfo)stationInfo).STATGUID, out freqSegment))
                {
                    if (freqSegment is FreqPlanActivity)
                        ((RoundStationInfo)stationInfo).FreqActivityGuid = (freqSegment as FreqPlanActivity).Guid;
                    ((RoundStationInfo)stationInfo).FreqPart = freqSegment;
                }
                roundStations.Add((RoundStationInfo)stationInfo);
            }
            return roundStations;
        }

        private List<FreqEmitInfo> GetStationSystemEmitInfo(string pStatId, string pStatAppType)
        {
            List<FreqEmitInfo> freqEmitInfo = new List<FreqEmitInfo>();
            string strStation = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_StationManage, string>(channel =>
            {
                return channel.SearchStationByGuid(pStatId);
            });
            if (!string.IsNullOrEmpty(strStation))
            {
                System.IO.StringReader xmlSR = new System.IO.StringReader(strStation);
                System.Data.DataSet CurrentResult = new System.Data.DataSet();
                CurrentResult.ReadXml(xmlSR, XmlReadMode.InferTypedSchema); //忽视任何内联架构，从数据推断出强类型架构并加载数据。如果无法推断，则解释成字符串数据
                if (CurrentResult.Tables.Count > 0)
                {
                    freqEmitInfo = ShowDetail(CurrentResult, pStatAppType);
                }
            }
            return freqEmitInfo;
        }

        #region 获取发射信息
        private List<FreqEmitInfo> ShowDetail(DataSet ds, string pStatAppType)
        {
            List<FreqEmitInfo> freqEmitInfos = null;
            if (ds == null || ds.Tables.Count == 0)
            {
                return freqEmitInfos;
            }

            #region 取数据
            if (ds.Tables.Count > 0)
            {
                //频率表
                if (ds.Tables["RSBT_FREQ"] != null && ds.Tables["RSBT_FREQ"].Rows.Count > 0)
                {
                    freqEmitInfos = FreqInfoToEmit(ds.Tables["RSBT_FREQ"]);
                    freqEmitInfos = freqEmitInfos.OrderBy(p => p.Guid).ToList();
                }
                if (freqEmitInfos == null || freqEmitInfos.Count == 0) return freqEmitInfos;
                //台站资料表
                if (ds.Tables["RSBT_STATION"] != null && ds.Tables["RSBT_STATION"].Rows.Count > 0)
                {
                    StationInfoToEmit(ds.Tables["RSBT_STATION"], ref freqEmitInfos);
                }
                //eaf对应关系ccode赋值
                if (pStatAppType == "TF" || pStatAppType == "C" || pStatAppType == "E")
                {
                    //频率冗余表
                    if (ds.Tables["RSBT_FREQ_T"] != null && ds.Tables["RSBT_FREQ_T"].Rows.Count > 0)
                    {
                        DataView dv = ds.Tables["RSBT_FREQ_T"].DefaultView;
                        dv.Sort = "GUID";
                        Freq_TtoEmit(dv.ToTable(), ref freqEmitInfos, pStatAppType);
                    }
                }
                //天线信息赋值
                if (pStatAppType == "TF" || pStatAppType == "C")
                {
                    //TF及C表的天线信息
                    if (ds.Tables["RSBT_ANTFEED"] != null && ds.Tables["RSBT_ANTFEED"].Rows.Count > 0 &&
                        ds.Tables["RSBT_ANTFEED_T"] != null && ds.Tables["RSBT_ANTFEED_T"].Rows.Count > 0)
                    {
                        CAndTFAntToEmit(ds.Tables["RSBT_ANTFEED"], ds.Tables["RSBT_ANTFEED_T"], ref freqEmitInfos, pStatAppType);
                    }
                }
                else
                {
                    //其他表天线赋值
                    if (ds.Tables["RSBT_ANTFEED"] != null && ds.Tables["RSBT_ANTFEED"].Rows.Count > 0)
                    {
                        AntToEmit(ds.Tables["RSBT_ANTFEED"], ref freqEmitInfos, pStatAppType);
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
                            CAndTFEquToEmit(ds.Tables["RSBT_EQU"], ds.Tables["RSBT_EQU_T"], ref freqEmitInfos);
                        }
                    }
                    else
                    {
                        //其他表设备功率
                        if (ds.Tables["RSBT_EQU"] != null && ds.Tables["RSBT_EQU"].Rows.Count > 0)
                        {
                            EquToEmit(ds.Tables["RSBT_EQU"], ref freqEmitInfos);
                        }
                    }
                }

            }
            #endregion
            return freqEmitInfos;
        }
        #endregion

        #region 频率信息
        /// <summary>
        /// 频率信息转为发射信息
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private List<FreqEmitInfo> FreqInfoToEmit(DataTable dataTable)
        {
            List<FreqEmitInfo> freqEmits = new List<FreqEmitInfo>();
            foreach (DataRow dr in dataTable.Rows)
            {
                try
                {
                    FreqEmitInfo freqEmit = new FreqEmitInfo();
                    if (dataTable.Columns.Contains("GUID"))
                    {
                        freqEmit.Guid = dr["GUID"].ToString();
                    }
                    else
                    {
                        freqEmit.Guid = "";
                    }

                    #region 频率发射
                    freqEmit.FreqEC = 0;
                    if (dataTable.Columns.Contains("FREQ_UC"))
                    {
                        double freqUc = 0;
                        double.TryParse(dr["FREQ_UC"].ToString(), out freqUc);
                        if (freqUc == 0 || double.IsNaN(freqUc))
                        {
                            if (dataTable.Columns.Contains("FREQ_EFB"))
                            {
                                freqUc = 0;
                                double.TryParse(dr["FREQ_EFB"].ToString(), out freqUc);
                                if (freqUc == 0 || double.IsNaN(freqUc))
                                {
                                    if (dataTable.Columns.Contains("FREQ_EFE"))
                                    {
                                        freqUc = 0;
                                        double.TryParse(dr["FREQ_EFE"].ToString(), out freqUc);
                                        if (!double.IsNaN(freqUc))
                                        {
                                            freqEmit.FreqEC = freqUc;
                                        }
                                    }
                                }
                                else
                                    freqEmit.FreqEC = freqUc;
                            }
                        }
                        else
                            freqEmit.FreqEC = freqUc;
                    }
                    else if (dataTable.Columns.Contains("FREQ_EFB"))
                    {
                        double freqUc = 0;
                        double.TryParse(dr["FREQ_EFB"].ToString(), out freqUc);
                        if (freqUc == 0 || double.IsNaN(freqUc))
                        {
                            if (dataTable.Columns.Contains("FREQ_EFE"))
                            {
                                freqUc = 0;
                                double.TryParse(dr["FREQ_EFE"].ToString(), out freqUc);
                                if (!double.IsNaN(freqUc))
                                {
                                    freqEmit.FreqEC = freqUc;
                                }
                            }
                        }
                        else
                            freqEmit.FreqEC = freqUc;
                    }
                    else if (dataTable.Columns.Contains("FREQ_EFE"))
                    {
                        double freqUc = 0;
                        double.TryParse(dr["FREQ_EFE"].ToString(), out freqUc);
                        if (!double.IsNaN(freqUc))
                        {
                            freqEmit.FreqEC = freqUc;
                        }
                    }
                    #endregion 频率发射结束

                    #region 频率接收
                    freqEmit.FreqRC = 0;
                    if (dataTable.Columns.Contains("FREQ_LC"))
                    {
                        double freqUc = 0;
                        double.TryParse(dr["FREQ_LC"].ToString(), out freqUc);
                        if (freqUc == 0 || double.IsNaN(freqUc))
                        {
                            if (dataTable.Columns.Contains("FREQ_RFB"))
                            {
                                freqUc = 0;
                                double.TryParse(dr["FREQ_RFB"].ToString(), out freqUc);
                                if (freqUc == 0 || double.IsNaN(freqUc))
                                {
                                    if (dataTable.Columns.Contains("FREQ_RFE"))
                                    {
                                        freqUc = 0;
                                        double.TryParse(dr["FREQ_RFE"].ToString(), out freqUc);
                                        if (!double.IsNaN(freqUc))
                                        {
                                            freqEmit.FreqRC = freqUc;
                                        }
                                    }
                                }
                                else
                                    freqEmit.FreqRC = freqUc;
                            }
                        }
                        else
                            freqEmit.FreqRC = freqUc;
                    }
                    else if (dataTable.Columns.Contains("FREQ_RFB"))
                    {
                        double freqUc = 0;
                        double.TryParse(dr["FREQ_RFB"].ToString(), out freqUc);
                        if (freqUc == 0 || double.IsNaN(freqUc))
                        {
                            if (dataTable.Columns.Contains("FREQ_RFE"))
                            {
                                freqUc = 0;
                                double.TryParse(dr["FREQ_RFE"].ToString(), out freqUc);
                                if (!double.IsNaN(freqUc))
                                {
                                    freqEmit.FreqRC = freqUc;
                                }
                            }
                        }
                        else
                            freqEmit.FreqRC = freqUc;
                    }
                    else if (dataTable.Columns.Contains("FREQ_RFE"))
                    {
                        double freqUc = 0;
                        double.TryParse(dr["FREQ_RFE"].ToString(), out freqUc);
                        if (!double.IsNaN(freqUc))
                        {
                            freqEmit.FreqRC = freqUc;
                        }
                    }
                    #endregion 频率接收结束

                    //频率带宽
                    freqEmit.FreqBand = 0;
                    if (dataTable.Columns.Contains("FREQ_E_BAND"))
                    {
                        double freqBand = 0;
                        double.TryParse(dr["FREQ_E_BAND"].ToString(), out freqBand);
                        freqEmit.FreqBand = freqBand;
                    }
                    //调制方式
                    if (dataTable.Columns.Contains("FREQ_MOD"))
                        freqEmit.FreqMod = dr["FREQ_MOD"].ToString();
                    if (freqEmits.FirstOrDefault(p=>p.FreqEC == freqEmit.FreqEC) == null)   //过滤重复的发射频率
                        freqEmits.Add(freqEmit);
                }
                catch
                {

                }
            }
            return freqEmits;
        }
        #endregion

        #region 台站信息
        /// <summary>
        /// 台站信息
        /// </summary>
        /// <param name="dataTable"></param>
        private void StationInfoToEmit(DataTable dataTable, ref List<FreqEmitInfo> freqEmitlist)
        {
            foreach (var emit in freqEmitlist)
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
        private void AntToEmit(DataTable dataTable, ref List<FreqEmitInfo> freqEmitlist, string pStatAppType)
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
        private void CAndTFAntToEmit(DataTable dataTable, DataTable dataTable_T, ref List<FreqEmitInfo> freqEmitlist, string pStatAppType)
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
        private void EquToEmit(DataTable dataTable, ref List<FreqEmitInfo> freqEmitlist)
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
        private void CAndTFEquToEmit(DataTable dataTable, DataTable dataTable_T, ref List<FreqEmitInfo> freqEmitlist)
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
        private void Freq_TtoEmit(DataTable dataTable, ref List<FreqEmitInfo> freqEmitlist, string pStatAppType)
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

        #endregion


        private void xbtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveRoundStationInfos(StationItemsSource);
        }
        /// <summary>
        /// 保存更新周围台站信息
        /// </summary>
        /// <param name="pRoundStations"></param>
        private void SaveRoundStationInfos(List<RoundStationInfo> pRoundStations)
        {

            try
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                {
                    channel.SaveRoundStationInfos(pRoundStations);
                });
                MessageBox.Show("保存成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
            }
        }
        /// <summary>
        /// 查询周围台站信息
        /// </summary>
        /// <returns></returns>
        private List<RoundStationInfo> QueryRouindStations(string pPlaceId)
        {
            List<RoundStationInfo> roundStations = new List<RoundStationInfo>();
            return roundStations;
        }

        private List<StationInfo> QueryStationData(Point[] pList, Point pLeftTop, Point pRightBottom, string pFreqRange)
        {
            List<StationInfo> statinfoLs = new List<StationInfo>();
            List<QUERY_PARAMS> lsParams = new List<QUERY_PARAMS>();
            if (!string.IsNullOrEmpty(pFreqRange))
            {
                QUERY_PARAMS param = null;
                param = new QUERY_PARAMS();
                param.PARAMS_NAME = "频率范围";
                param.PARAMS_RELATION = "";
                param.PARAMS_VALUE = pFreqRange;
                param.PARAMS_UNIT = "";
                param.PARAMS_TYPE = "varchar";
                lsParams.Add(param);
            }
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_StationManage>(channel =>
            {
                statinfoLs = channel.GetStationBaseInfo(lsParams, pList, pLeftTop, pRightBottom);
            });
            return statinfoLs;
        }
        

        private List<RoundStationInfo> GetSavedRoundStations(string pPlaceId)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<RoundStationInfo>>(channel =>
            {
                return channel.QueryRoundStationsByPlace(pPlaceId);
            });
        }
    }
}
