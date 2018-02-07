using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CO_IA.Data;
using CO_IA.Data.Monitor;
using AT_BC.Types;
using CO_IA.Data.MonitorPlan;
using AT_BC.Data;

namespace CO_IA.UI.MonitorPlan.MonitorDialog
{
    
    /// <summary>
    /// TaskDetailDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TaskDetailDialog : Window
    {
        #region 属性
        string activityId = "";
        List<Sealed_PP_OrgInfo> sealed_pp_org = new List<Sealed_PP_OrgInfo>();
        public event Action<DetailMonitorPlan> AfterOkButtonClick;
        public event Action AfterViewLoaded;
        private DetailMonitorPlan _currentTask = new DetailMonitorPlan();
        List<FreqRange> _rangeListSource = new List<FreqRange>();
        //位置信息
        ActivityPlaceInfo _currentPlaceInfo = new ActivityPlaceInfo();
        List<string> getFrequencyRange = new List<string>();
        List<FreqPlanInfo> _currentfreqPlanSource = new List<FreqPlanInfo>();
        #endregion

        #region 构造函数
        //public TaskDetailDialog(DetailMonitorPlan p_monitorTask, List<FreqPlanInfo> p_freqPlaninfo)
        //{
        //    InitializeComponent();
        //    this._currentTask = p_monitorTask;
        //    this._freqPlanSource = p_freqPlaninfo;
        //    ItemsSourceLoad();
        //}
        public TaskDetailDialog(DetailMonitorPlan p_monitorTask, ActivityPlaceInfo p_placeinfo, List<FreqPlanInfo> _freqPlanSource)
        {
            InitializeComponent();
            activityId = p_monitorTask.ACTIVITY_GUID;
            this._currentTask = p_monitorTask;
            this._currentPlaceInfo = p_placeinfo;
            this._currentfreqPlanSource = _freqPlanSource;
            ItemsSourceLoad();
            ItemsPontsLoad(p_monitorTask);
            getFrequencyRange = p_monitorTask.FrequencyRange;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 加载频率信息
        /// </summary>
        private void ItemsSourceLoad()
        {
            ActivityPlace[] getActivityPlace = PrototypeDatas.GetPlaces(activityId);
            _addressComBox.ItemsSource = getActivityPlace.ToList();
            //加载 频段信息
            if (_currentTask.FrequencyRange.Count > 0)
            {
                foreach (var strfreq in _currentTask.FrequencyRange)
                {
                    FreqRange _freq = new FreqRange();
                    _freq.FreqFrom = Convert.ToDouble(strfreq.Split('-')[0]);
                    _freq.FreqTo = Convert.ToDouble(strfreq.Split('-')[1]);
                    _rangeListSource.Add(_freq);
                }
                _freqRangeLBox.ItemsSource = _rangeListSource;
            }
            else
            {
                _freqRangeLBox.ItemsSource = null;
            }
            this.DataContext = _currentTask;
            if (AfterViewLoaded != null)
            {
                AfterViewLoaded();
            }
        }
        /// <summary>
        /// 加载频点信息
        /// </summary>
        /// <param name="p_monitorTask"></param>
        private void ItemsPontsLoad(DetailMonitorPlan p_monitorTask)
        {
             //读取频率预案列表
            List<FreqPlanActivity> freqSegment = PrototypeDatas.GetActivityInfo(_currentPlaceInfo.Guid);
            if (freqSegment.Count > 0)
            {
                if (_currentTask.FreqMAINGUIDs.Count > 0)
                {
                    foreach (var freqid in _currentTask.FreqMAINGUIDs)
                    {

                    }
                }
            }
        }
        public void SetToolBarVisible(bool isVisible = true)
        {
            _toolBarGrid.Visibility = isVisible == true ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion



        #region 事件
        /// <summary>
        /// 选择，监测组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            //所得所有已存的人员组织结构
            List<PP_OrgInfo> list = new List<PP_OrgInfo>();
            List<PP_OrgInfo> listOrgType = new List<PP_OrgInfo>();
            List<Sealed_PP_OrgInfo> sealedList = new List<Sealed_PP_OrgInfo>();
            List<Sealed_PP_OrgInfo> sealed_pp_org = new List<Sealed_PP_OrgInfo>();
          // string  activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;

            //新建还是读取
            if (string.IsNullOrEmpty(activityId) == true)
            {
                MessageBox.Show("请先创建相关活动！");
                return;
            }
            else
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    list = channel.GetPP_OrgInfos(activityId);
                });
                if (list.Count() > 0)
                {
                    listOrgType = list.Where(p => 1 == 1).ToList();
                    foreach (PP_OrgInfo orginfo in listOrgType)
                    {
                        Sealed_PP_OrgInfo currentinfo = new Sealed_PP_OrgInfo();
                        currentinfo.Children = orginfo.Children;
                        currentinfo.GUID = orginfo.GUID;
                        currentinfo.ACTIVITY_GUID = orginfo.ACTIVITY_GUID;
                        
                        currentinfo.NAME = orginfo.NAME;
                        currentinfo.PARENT_GUID = orginfo.PARENT_GUID;
                        
                        currentinfo.Persons = PrototypeDatas.GetOrgOfPerson(orginfo.GUID);
                        sealed_pp_org.Add(currentinfo);
                    }
                }

                var workerDialog = new WorkerSelectedDialog(sealed_pp_org);
                workerDialog.OKButtonClick += (personGroupList) =>
                {
                    string personDisplay = string.Empty;
                    if (personGroupList != null)
                    {
                        foreach (Sealed_PP_OrgInfo ppinfo in personGroupList)
                        {
                            personDisplay = ppinfo.NAME;//监测组名
                            if (ppinfo.Persons.Count() > 0)
                            {
                                personDisplay += "{";
                                foreach (PP_PersonInfo personInfo in ppinfo.Persons)
                                {
                                    personDisplay += personInfo.NAME + ",";
                                }
                                personDisplay = personDisplay.Substring(0, personDisplay.Length - 1);
                                personDisplay += "}";
                            }
                            _workerTBox.Text = personDisplay;
                            _workerTBox.Tag = personGroupList;
                            _currentTask.SENDGROUPIDS = ppinfo.GUID;//监测组id
                            _currentTask.SENDGROUPS = personDisplay;
                        }
                    }
                };
                workerDialog.Show();
            }
        }
        /// <summary>
        /// 确定 按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (DateTime.Compare(Convert.ToDateTime(_beginDate.SelectedDate), Convert.ToDateTime(_endDate.SelectedDate)) > 0)
            {
                MessageBox.Show("开始时间不能大于结束时间！");
                return;
            }
            #region 判断监测小组在指定日期内是否已生成过监测任务
            bool ischeckfre = false;
            DetailMonitorPlan[] detailSoure = PrototypeDatas.GetDetailMonitorPlan(_currentPlaceInfo.Guid, _currentTask.ACTIVITY_GUID);
            //手动或自动 先判断数据库中是否已经存在任务
            var fresquery = detailSoure.Where(p => p.SENDGROUPIDS == _currentTask.SENDGROUPIDS).ToList();
            if (fresquery.Count() > 0)
            {
                foreach (var dtquery in fresquery)
                {
                    bool isStartDate = PrototypeDatas.IsInDate(_currentTask.STARTTASKDATE, dtquery.STARTTASKDATE, dtquery.ENDTASKDATE);
                    bool isEndDate = PrototypeDatas.IsInDate(_currentTask.ENDTASKDATE, dtquery.STARTTASKDATE, dtquery.ENDTASKDATE);
                    if (isStartDate == true || isEndDate == true)
                    {
                        PP_OrgInfo getOrginfo = PrototypeDatas.GetPP_OrgInfo(dtquery.SENDGROUPIDS.ToString());
                        MessageBox.Show(getOrginfo.NAME + ",设置的指定日期内已经生成过监测任务！请重新设置日期！");
                        ischeckfre = true;
                    }
                }
            }
            if (ischeckfre == true)
                return;
            #endregion
            if (AfterOkButtonClick != null)
            {
                string currentMainIds = "";
                //根据频率段的选择，更新频段对应的频率主键id值
                List<string> currentFreqIds = new List<string>();
                if (_freqRangeLBox.ItemsSource != null)
                {
                    //根据选择的频率段值，更新存储的频率段集合
                    List<FreqRange> rst = _freqRangeLBox.ItemsSource as List<FreqRange>;
                    string currentFreqRange = "";
                    List<string> freqList = new List<string>();
                    foreach (FreqRange range in rst)
                    {
                        string _range = range.FreqFrom.ToString() + '-' + range.FreqTo.ToString();
                        freqList.Add(_range);
                        currentFreqRange += _range + ",";
                    }
                    currentFreqRange = currentFreqRange.TrimEnd(',');

                    //根据选择的频率段集合，更新存储的频率预案信息id集合
                    List<FreqPlanActivity> originalFreq = PrototypeDatas.GetActivityInfo(_currentPlaceInfo.Guid);
                    if (_currentTask.FreqMAINGUIDs.Count > 0)
                    {
                        foreach (var freqid in _currentTask.FreqMAINGUIDs)
                        {
                            Range<double> originalFreqRange = originalFreq.Where(p => p.Guid == freqid).FirstOrDefault().FreqValue;
                            string orgfreq = (originalFreqRange.Little / 1000000).ToString() + "-" + (originalFreqRange.Great / 1000000).ToString();
                            if (currentFreqRange.Contains(orgfreq) == true)
                            {
                                currentFreqIds.Add(freqid);
                                currentMainIds += freqid + ",";
                            }
                        }
                    }
                    _currentTask.FrequencyRange = freqList;
                    _currentTask.IMPORTFREQUENCYRANGE = currentFreqRange;
                    _currentTask.FreqMAINGUIDs = currentFreqIds;
                    _currentTask.MAINGUID = currentMainIds.TrimEnd(',').ToString();
                }
                AfterOkButtonClick(_currentTask);
            }
            this.Close();
        }
        private void SaveDetailMonitor(DetailMonitorPlan p_monitor)
        {
           
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 选择，频率段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FreqRange_Selected(object sender, RoutedEventArgs e)
        {
            FreqSelectDialog dialog = new FreqSelectDialog(_currentTask, getFrequencyRange);
            dialog.AfterOKButtonClick += (rst) =>
            {
                if (rst is List<FreqRange>)
                {
                    _freqRangeLBox.ItemsSource = null;
                    _freqRangeLBox.ItemsSource = rst as List<FreqRange>;
                }
            };
            dialog.Show();
        }

        private void FreqPoint_Selected(object sender, RoutedEventArgs e)
        {//应该从频率预案中选择
            //List<double> list = new List<double>();
            //list.AddRange(PrototypeDatas.GetFreqPoints1());
            //list.AddRange(PrototypeDatas.GetFreqPoints2());
            //FreqSelectDialog dialog = new FreqSelectDialog(list, _currentTask.ProtectFreqPoints);
            //dialog.AfterOKButtonClick += (rst) =>
            //{
            //    if (rst is List<double>)
            //    {
            //        _freqPointLBox.ItemsSource = null;
            //        _freqPointLBox.ItemsSource = rst as List<double>;
            //        _currentTask.ProtectFreqPoints = rst as List<double>; 
            //    }
            //};
            //dialog.Show();
        }

        /// <summary>
        /// 选择,位置信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _placeTBox_Click(object sender, RoutedEventArgs e)
        {
            ActivityListDialog positionDialog = new ActivityListDialog(_currentPlaceInfo);
            positionDialog.LocationSelectionChanged += positionDialog_LocationSelectionChanged;
            positionDialog.ShowDialog(this);
        }

        void positionDialog_LocationSelectionChanged(ActivityPlaceLocation p_position)
        {
            _placeTBox.Text = p_position.LocationName.ToString();
            _currentTask.POSITIONID = p_position.GUID;//选择的位置id
            _currentTask.POSITIONINFO = p_position.LocationName.ToString();
        }
        /// <summary>
        /// 具体频点信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _freqRangeLBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            FreqRange freqRange = _freqRangeLBox.SelectedItem as FreqRange;
            string selectFreq = freqRange.FreqFrom + "-" + freqRange.FreqTo;
            List<string> getPonits = new List<string>();
            if (_currentTask.IMPORTFREQUENCYRANGE.Contains(selectFreq) == true)
            {
                if (_currentTask.FreqMAINGUIDs.Count > 0)
                {
                    foreach (var freqid in _currentTask.FreqMAINGUIDs)
                    {
                        var queryFreq = _currentfreqPlanSource.Where(p => p.FreqIdForMonitor == freqid);
                        foreach (var freq in queryFreq)
                        {
                            if (freq.FreqValue.Little == freqRange.FreqFrom * 1000000 && freq.FreqValue.Great == freqRange.FreqTo * 1000000)
                            {
                                _currentTask.Points = freq.Points;
                            }
                        }
                    }
                    if (_currentTask.Points.Count() > 0)
                    {
                        foreach (var point in _currentTask.Points)
                        {
                            string strPonit = point.X.ToString() + "-" + point.Y.ToString();
                            getPonits.Add(strPonit);
                        }
                    }
                    _freqPointLBox.ItemsSource = getPonits;
                }
            }
        }
        #endregion

       
       
    }
}
