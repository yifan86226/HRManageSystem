#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：频率指配列表
 * 日  期：2016-09-01
 * ********************************************************************************/
#endregion
using CO_IA.Data;
using System;
using System.Windows;
using System.Collections.Generic;
using CO_IA_Data;

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// FreqAssignDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FreqAssignDialog : Window
    {
        /// <summary>
        /// 建议频率
        /// </summary>
        private FreqUse AssingFreq
        {
            get { return _freqAssignList_Control.SelectedFreq; }
            set { _freqAssignList_Control.SelectedFreq = value; }
        }

        public bool isInitOk = true;

        public event Action<double> ConfirmEvent;

        public FreqAssignDialog(ActivityEquipmentInfo equ, List<RoundStationInfo> aroundstations, string placeGuid)
        //public FreqAssignDialog(ActivityEquipmentInfo equ, List<ActivityEquipmentInfo> aroundstations)
        {
            InitializeComponent();
            _freqAssignList_Control.EquInfo = equ;
            _freqAssignList_Control.AroundStations = aroundstations;
            _freqAssignList_Control.PlaceGuid = placeGuid;
            _freqAssignList_Control.BusinessCode = equ.BusinessCode;
            isInitOk = _freqAssignList_Control.InitPage();
        }

        public FreqAssignDialog(string placeGuid, string businessCode)
        {
            InitializeComponent();
            _freqAssignList_Control.PlaceGuid = placeGuid;
            _freqAssignList_Control.BusinessCode = businessCode;
            isInitOk = _freqAssignList_Control.InitPage(placeGuid, businessCode);
            this.Title = "频率占用";
            _freqAssignList_Control.titleselectfreq.Visibility = Visibility.Hidden;
            _freqAssignList_Control.tbselectfreq.Visibility = Visibility.Hidden;
            _freqAssignList_Control.ShowInterfere = false;
            BtnOK.Visibility = Visibility.Hidden;
            BtnCancel.Content = "关闭";
        }

        public FreqAssignDialog(FreqPlanSegment p_freqInfo)
        {
            InitializeComponent();
            List<FreqPlanSegment> list = new List<FreqPlanSegment>();
            list.Add(p_freqInfo);
            isInitOk = _freqAssignList_Control.InitPage2(list);
            this.Title = "频率占用";
            _freqAssignList_Control.titleselectfreq.Visibility = Visibility.Hidden;
            _freqAssignList_Control.tbselectfreq.Visibility = Visibility.Hidden;
            _freqAssignList_Control.ShowInterfere = false;
            BtnOK.Visibility = Visibility.Hidden;
            BtnCancel.Content = "关闭";
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (AssingFreq == null)
            {
                MessageBox.Show("请先选择建议频率");
                return;
            }
            if (ConfirmEvent != null)
            {
                ConfirmEvent(AssingFreq.Freq);
            }
            this.Close();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
