using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.DataAnalysis
{
    /// <summary>
    /// 多频段谱图数据管理
    /// </summary>
    internal class MultibandSpectrumDataManage
    {
        private readonly List<MultibandSpectrumData> _multibandSpectrumDataList = new List<MultibandSpectrumData>();
        private readonly List<SpectrumFreqPointData> _spectrumFreqPointList = new List<SpectrumFreqPointData>();

        public double StartFreq { get; private set; }

        public double StopFreq { get; private set; }

        public void Add(MultibandSpectrumData pMultibandSpectrumData)
        {
            _multibandSpectrumDataList.Add(pMultibandSpectrumData);
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            StartFreq = 0;
            StopFreq = 0;
            _spectrumFreqPointList.Clear();
            _multibandSpectrumDataList.Clear();
        }

        /// <summary>
        /// 根据索引获取谱图频点数据
        /// </summary>
        /// <param name="pIndex"></param>
        /// <returns></returns>
        public SpectrumFreqPointData GetData(int pIndex)
        {
            if (pIndex < 0)
                return null;
            return pIndex >= _spectrumFreqPointList.Count ? _spectrumFreqPointList.LastOrDefault() : _spectrumFreqPointList[pIndex];
        }

        /// <summary>
        /// 获取频率值在频点数据集合中的近似频率
        /// </summary>
        /// <param name="pFrequency">近似频率值</param>
        /// <returns>在缓存中的实际频率值</returns>
        public double GetFrequency(double pFrequency)
        {
            if (_spectrumFreqPointList.Count == 0)
                return 0;

            int low = 0;
            int high = _spectrumFreqPointList.Count - 1;
            var startFreq = _spectrumFreqPointList[low].Freq;
            var stopFreq = _spectrumFreqPointList[high].Freq;
            if (pFrequency <= startFreq)
                return startFreq;

            if (pFrequency >= stopFreq)
                return stopFreq;

            while (startFreq <= stopFreq)
            {
                int midIndex = (high + low) / 2;
                var midFreq = _spectrumFreqPointList[midIndex].Freq;
                if (midFreq.Equals(pFrequency))
                    return pFrequency;

                if (pFrequency > midFreq)
                    low = midIndex + 1;
                else
                    high = midIndex - 1;
                startFreq = _spectrumFreqPointList[low].Freq;
                stopFreq = _spectrumFreqPointList[high].Freq;
            }
            if (low >= _spectrumFreqPointList.Count)
                low = low - 1;
            var f1 = _spectrumFreqPointList[low].Freq;
            var f2 = _spectrumFreqPointList[high].Freq;
            if (Math.Abs(f1 - pFrequency) < Math.Abs(f2 - pFrequency))
                return f1;
            return f2;
        }

        /// <summary>
        /// 获取频率值在频点数据集合中的索引
        /// </summary>
        /// <param name="pFrequency">频率值</param>
        /// <returns>索引</returns>
        public int GetFrequencyIndex(double pFrequency)
        {
            if (_spectrumFreqPointList.Count == 0)
                return 0;

            int low = 0;
            int high = _spectrumFreqPointList.Count - 1;
            var startFreq = _spectrumFreqPointList[low].Freq;
            var stopFreq = _spectrumFreqPointList[high].Freq;
            if (pFrequency <= startFreq)
                return low;

            if (pFrequency >= stopFreq)
                return high;

            while (startFreq <= stopFreq)
            {
                int midIndex = (high + low) / 2;
                var midFreq = _spectrumFreqPointList[midIndex].Freq;
                if (midFreq.Equals(pFrequency))
                    return midIndex;

                if (pFrequency > midFreq)
                    low = midIndex + 1;
                else
                    high = midIndex - 1;
                startFreq = _spectrumFreqPointList[low].Freq;
                stopFreq = _spectrumFreqPointList[high].Freq;
            }
            return high;
        }

        /// <summary>
        /// 获取信号集合
        /// </summary>
        /// <returns></returns>
        public List<ElecEnvFlexGridData> GetSignalList()
        {
            var signalList = new List<ElecEnvFlexGridData>();
            foreach (var multibandSpectrumData in _multibandSpectrumDataList)
            {
                signalList.AddRange(multibandSpectrumData.SignalList);
            }
            return signalList;
        }

        /// <summary>
        /// 更新谱图数据
        /// </summary>
        public void UpdateData()
        {
            if (_multibandSpectrumDataList.Count == 0)
                return;
            _spectrumFreqPointList.Clear();
            //取融合后频段的起始频率和终止频率
            StartFreq = _multibandSpectrumDataList.Min(t => t.StartFreq);
            StopFreq = _multibandSpectrumDataList.Max(t => t.StopFreq);
            foreach (var multibandSpectrumData in _multibandSpectrumDataList)
            {
                if (multibandSpectrumData.Count == 0)
                    continue;
                //在标准频段分析中，这三组数据由三个服务而来，但各自对应的频点有微小差异。
                //若要针对重构，涉及数据缓存、管理、谱图画法，比较繁琐。此处和服务组商议能否修改服务，若不能再重构。
                var countList = new[]
                {
                    multibandSpectrumData.MaxDbuvs.Count,
                    multibandSpectrumData.MedianDbuvs.Count,
                    multibandSpectrumData.NoiseDbuvs.Count
                };
                //若有0项，说明未取到该项数据，排除掉
                var count = countList.Except(new List<int>
                {
                    0
                });
                for (int j = 0; j < count.Min(); j++)
                {
                    var spectrumFreqPointData = new SpectrumFreqPointData
                    {
                        Freq = multibandSpectrumData.FreqValues.Count > j ? multibandSpectrumData.FreqValues[j] : new short(),
                        MaxDbuv = multibandSpectrumData.MaxDbuvs.Count > j ? multibandSpectrumData.MaxDbuvs[j] : new short(),
                        Dbuv = multibandSpectrumData.MedianDbuvs.Count > j ? multibandSpectrumData.MedianDbuvs[j] : new short(),
                        Noise = multibandSpectrumData.NoiseDbuvs.Count > j ? multibandSpectrumData.NoiseDbuvs[j] : new short(),
                        Index = j
                    };
                    _spectrumFreqPointList.Add(spectrumFreqPointData);
                }
            }
            _spectrumFreqPointList.Sort(CompareSpectrumData);
        }

        /// <summary>
        /// 根据频点的RecordId更新信号
        /// </summary>
        /// <param name="pEefgd"></param>
        public void UpdateSignal(ElecEnvFlexGridData pEefgd)
        {
            var signalList = GetSignalList();
            for (int i = 0; i < signalList.Count; i++)
            {
                if (signalList[i].RecordId == pEefgd.RecordId)
                    signalList[i] = pEefgd;
            }
        }

        /// <summary>
        /// 获取频点幅度值的最大值和最小值
        /// </summary>
        /// <param name="min">最小值</param>
        /// <returns>最大值</returns>
        public double? GetDbuvExtremum(out double? min)
        {
            min = _spectrumFreqPointList.Min(t => t.Noise);
            return _spectrumFreqPointList.Max(t => t.MaxDbuv);
        }

        private int CompareSpectrumData(SpectrumFreqPointData pData1, SpectrumFreqPointData pData2)
        {
            if (pData1.Freq < pData2.Freq)
                return -1;
            if (pData1.Freq > pData2.Freq)
                return 1;
            return 0;
        }
    }
}
