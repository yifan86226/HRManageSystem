using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


namespace CO_IA.UI.FreqPlan.FreqPlan
{
    public class AssignFreqDataHelper
    {
        /// <summary>
        /// 电磁环境清理统计数据源
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<EmeClearStatisticData> CreateEmeClearStatisticDatas()
        {
            ObservableCollection<EmeClearStatisticData> statisticDatas = new ObservableCollection<EmeClearStatisticData>();
            EmeClearStatisticData statisticData = new EmeClearStatisticData();
            statisticData.Address = "西安塔";
            statisticData.SignalSource = "已建";
            statisticData.Count = 12;
            statisticDatas.Add(statisticData);

            statisticData = new EmeClearStatisticData();
            statisticData.Address = "西安塔";
            statisticData.SignalSource = "未知";
            statisticData.Count = 15;
            statisticDatas.Add(statisticData);

            statisticData = new EmeClearStatisticData();
            statisticData.Address = "创意园";
            statisticData.SignalSource = "已建";
            statisticData.Count = 8;
            statisticDatas.Add(statisticData);

            statisticData = new EmeClearStatisticData();
            statisticData.Address = "创意园";
            statisticData.SignalSource = "未知";
            statisticData.Count = 9;
            statisticDatas.Add(statisticData);

            return statisticDatas;
        }
    }
}
