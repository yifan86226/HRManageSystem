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
using System.Windows.Shapes;
using CO_IA.Data;
using CO_IA.InterferenceAnalysis;

namespace CO_IA.Scene.Monitor
{
    /// <summary>
    /// InterferedDetailInfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InterferedDetailInfoDialog : Window
    {
        ActivityEquipmentInfo _activityEquipmentInfo;
        //干扰集合
        Dictionary<ActivityEquipmentInfo, List<InterfereResult>> _dicInterfereResult = new Dictionary<ActivityEquipmentInfo, List<InterfereResult>>();
        //互调干扰集合
        Dictionary<ActivityEquipmentInfo, List<IMInterfereResult>> _dicIMInterfereResult = new Dictionary<ActivityEquipmentInfo, List<IMInterfereResult>>();

        public InterferedDetailInfoDialog(ActivityEquipmentInfo p_activityEquipmentInfo, Dictionary<ActivityEquipmentInfo, List<InterfereResult>> p_dicInterfereResult,Dictionary<ActivityEquipmentInfo, List<IMInterfereResult>> p_dicIMInterfereResult)
        {
            InitializeComponent();
            this._activityEquipmentInfo = p_activityEquipmentInfo;
            this._dicInterfereResult = p_dicInterfereResult;
            this._dicIMInterfereResult = p_dicIMInterfereResult;
            _interfereStatisticsGrid.DataContext = LoadAnalysisResult();
        }

        InterferenceAnalysisResult LoadAnalysisResult()
        {
            List<InterfereResult> list = null;
            int samecount = 0;
            int adjcount = 0;
            int imtcount = 0;
            int imrcount = 0;
            if (this._dicInterfereResult.Keys.Contains(_activityEquipmentInfo))
            {
                list = this._dicInterfereResult[_activityEquipmentInfo];
                //BaseInterfereResultControl baseInterfereResultControl = FindFirstVisualChild<BaseInterfereResultControl>(InterferedResult, "_baseInterfereResultControl");
                //if (_baseInterfereResultControl != null)
                //{
                //    _baseInterfereResultControl.InterfItemsSource = list;
                //}

                //获取干扰统计信息
                samecount = list.Count(r => r.InterfType == InterfereTypeEnum.同频干扰);
                adjcount = list.Count(r => r.InterfType == InterfereTypeEnum.邻频干扰);
            }

            List<IMInterfereResult> imlist = null;
            if (_dicIMInterfereResult.Keys.Contains(_activityEquipmentInfo))
            {
                imlist = this._dicIMInterfereResult[_activityEquipmentInfo];
                //IMInterfereResultControl iMInterfereResultControl = FindFirstVisualChild<IMInterfereResultControl>(InterferedResult, "_iMInterfereResultControl");
                //if (_iMInterfereResultControl != null)
                //    _iMInterfereResultControl.IMInterfItemsSource = imlist;
                imtcount = imlist.Count(r => r.InterfType == InterfereTypeEnum.发射机互调干扰);
                imrcount = imlist.Count(r => r.InterfType == InterfereTypeEnum.接收机互调干扰);
            }

            InterferenceAnalysisResult result = new InterferenceAnalysisResult();
            result.SameFreqInterfResultCount = samecount;
            result.ADJFreqInterfResultCount = adjcount;
            result.IMInterfResultCount = imtcount + imrcount;
            return result;
        }
    }
}
