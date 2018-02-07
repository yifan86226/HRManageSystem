#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：同频干扰计算
 * 日  期：2016-09-07
 * ********************************************************************************/
#endregion
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{
    /// <summary>
    /// 同频比较计算器
    /// </summary>
    public class SameFreqCalculator : FreqCalculator
    {
        /// <summary>
        /// 同频比较结果字典
        /// </summary>
        private Dictionary<ComparableFreq, SameFreqCompareResult> resultList = new Dictionary<ComparableFreq, SameFreqCompareResult>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="freqs">用于计算同频干扰的频率组</param>
        public SameFreqCalculator(ComparableFreq[] freqs)
            : base(freqs)
        {
            this.GenerateSameMapping();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calcfreqs">计算干扰的频率</param>
        /// <param name="comparefreqs">用于比较的频率</param>
        public SameFreqCalculator(ComparableFreq[] calcfreqs, ComparableFreq[] comparefreqs)
            : base(calcfreqs, comparefreqs)
        {
            //InitSameMapping();
        }


        /// <summary>
        /// 向结果中添加同频结果
        /// </summary>
        /// <param name="freq">检索同频干扰关键字</param>
        /// <param name="disturbFreq">同频干扰频率 </param>
        private void Add(ComparableFreq freq, ComparableFreq disturbFreq)
        {
            SameFreqCompareResult result;
            List<ComparableFreq> list;
            if (!this.resultList.TryGetValue(freq, out result))
            {
                if (this.dicFreqMapping.TryGetValue(freq, out list))
                {
                    ComparableFreq[] freqArray = new ComparableFreq[dicFreqMapping[freq].Count + 1];
                    freqArray[0] = freq;
                    dicFreqMapping[freq].CopyTo(freqArray, 1);
                    result = new SameFreqCompareResult(freqArray);
                }
                else
                    result = new SameFreqCompareResult(freq);
                this.resultList.Add(freq, result);
            }
            result.AddFreq(disturbFreq);
            if (this.dicFreqMapping.TryGetValue(disturbFreq, out list))
            {
                result.AddFreqRange(list.ToArray());
            }
        }

        /// <summary>
        /// 向结果中添加同频结果
        /// </summary>
        /// <param name="freq">检索同频干扰关键字</param>
        /// <param name="disturbFreq">同频干扰频率 </param>
        private void AddSameFreqCompareResult(ComparableFreq freq, ComparableFreq disturbFreq, string interdescribe)
        {
            SameFreqCompareResult result;
            List<ComparableFreq> list;
            disturbFreq.InterfereResult = interdescribe;
            if (!this.resultList.TryGetValue(freq, out result))
            {
                if (this.dicFreqMapping.TryGetValue(freq, out list))
                {
                    ComparableFreq[] freqArray = new ComparableFreq[dicFreqMapping[freq].Count + 1];
                    freqArray[0] = freq;
                    dicFreqMapping[freq].CopyTo(freqArray, 1);
                    result = new SameFreqCompareResult(freqArray);
                }
                else
                {
                    result = new SameFreqCompareResult(freq);
                }

                this.resultList.Add(freq, result);
            }
            result.AddFreq(disturbFreq);
            if (this.dicFreqMapping.TryGetValue(disturbFreq, out list))
            {
                result.AddFreqRange(list.ToArray());
            }
        }

        /// <summary>
        /// 计划同频干扰
        /// </summary>
        /// <param name="calcfreqs"></param>
        /// <returns></returns>
        public SameFreqCompareResult[] CalcSameFreqInterference()
        {
            this.resultList.Clear();
            double maxHalfBand = GetMaxBand() / 2;

            for (int i = 0; i < calcfreqPoints.Length; i++)
            {
                for (int j = 0; j < compfreqPoints.Length; j++)
                {
                    //设备主频VS（设备、台站）主频
                    if (compfreqPoints[j].FreqID == calcfreqPoints[i].FreqID)
                        continue;
                    if (compfreqPoints[j].Freq - calcfreqPoints[i].Freq <= maxHalfBand)
                    {
                        if (calcfreqPoints[i].IsSameFreq(compfreqPoints[j]))
                        {
                            this.AddSameFreqCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                 string.Format("发射频率({0}MHz)干扰设备发射指配频率({1}MHz)", compfreqPoints[j].Freq, calcfreqPoints[i].Freq)
                             );
                        }
                    }

                    //设备主频VS（设备、台站）备频
                    if (compfreqPoints[j].SpareFreq > 0)
                    {
                        if (compfreqPoints[j].SpareFreq - calcfreqPoints[i].Freq <= maxHalfBand)
                        {
                            if (calcfreqPoints[i].IsSameSpareFreq(compfreqPoints[j], compfreqPoints[j].SpareFreq, calcfreqPoints[i].Freq))
                            {
                                this.AddSameFreqCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                    string.Format("备用频率({0}MHz)干扰设备发射指配频率({1}MHz)", compfreqPoints[j].SpareFreq, calcfreqPoints[i].Freq));
                            }
                        }
                    }

                    if (calcfreqPoints[i].SpareFreq > 0)
                    {
                        //设备备频VS（设备、台站）主频
                        if (compfreqPoints[j].Freq - calcfreqPoints[i].SpareFreq <= maxHalfBand)
                        {
                            if (calcfreqPoints[i].IsSameSpareFreq(compfreqPoints[j], compfreqPoints[j].Freq, calcfreqPoints[i].SpareFreq))
                            {
                                this.AddSameFreqCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                    string.Format("发射频率({0}MHz)干扰设备备用指配频率({1}MHz)", compfreqPoints[j].Freq, calcfreqPoints[i].SpareFreq));
                            }
                        }
                    }

                    if (calcfreqPoints[i].SpareFreq > 0)
                    {
                        //设备备频VS（设备、台站）备频
                        if (compfreqPoints[j].SpareFreq - calcfreqPoints[i].SpareFreq <= maxHalfBand)
                        {
                            if (calcfreqPoints[i].IsSameSpareFreq(compfreqPoints[j], compfreqPoints[j].SpareFreq, calcfreqPoints[i].SpareFreq))
                            {
                                this.AddSameFreqCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                    string.Format("备用频率({0}MHz)干扰设备备用指配频率({1}MHz)", compfreqPoints[j].SpareFreq, calcfreqPoints[i].SpareFreq));
                            }
                        }
                    }
                }
            }
            return this.resultList.Values.ToArray();
        }
    }
}
