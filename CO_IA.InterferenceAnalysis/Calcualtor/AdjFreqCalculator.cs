#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：邻频干扰计算
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
    /// 邻频生成器
    /// </summary>
    public class AdjFreqCalculator : FreqCalculator
    {
        /// <summary>
        /// 邻频比较结果字典
        /// </summary>
        private Dictionary<ComparableFreq, AdjFreqCompareResult> resultList = new Dictionary<ComparableFreq, AdjFreqCompareResult>();

        private Dictionary<ComparableFreq, List<AdjFreqCompareResult>> adjresultList = new Dictionary<ComparableFreq, List<AdjFreqCompareResult>>();


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="freqs">分析邻频的频率组</param>
        public AdjFreqCalculator(ComparableFreq[] freqs)
            : base(freqs)
        {
            this.GenerateSameMapping();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="calcfreqs">计算干扰的频率</param>
        /// <param name="comparefreqs">用于比较的频率</param>
        public AdjFreqCalculator(ComparableFreq[] calcfreqs, ComparableFreq[] comparefreqs)
            : base(calcfreqs, comparefreqs)
        {
            //this.InitSameMapping();
        }

        /// <summary>
        /// 计算频率的邻频比较结果
        /// 上邻频:比对频率是计算频率的上邻频（比对频率=计算频率-带宽）
        /// 下邻频:比对频率是计算频率的上邻频（比对频率=计算频率+带宽）
        /// </summary>
        /// <returns>邻频比较结果</returns>
        public AdjFreqCompareResult[] CalcAdjFreqInterference()
        {
            this.resultList.Clear();
            double diffUpperLimit = GetMaxBand() * 3 / 2;

            for (int i = 0; i < calcfreqPoints.Length; i++)
            {
                for (int j = 0; j < compfreqPoints.Length; j++)
                {
                    if (compfreqPoints[j].FreqID == calcfreqPoints[i].FreqID)
                        continue;

                    //设备主频vs(设备,周围台站)主频
                    if (compfreqPoints[j].Freq <= calcfreqPoints[i].Freq + diffUpperLimit)
                    {
                        if (calcfreqPoints[i].IsUpperAdjFreq(compfreqPoints[j], compfreqPoints[j].Freq, calcfreqPoints[i].Freq))
                        {
                            this.AddUpperAdjCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                string.Format("发射频率({0}MHz)干扰设备发射指配频率({1}MHz)", compfreqPoints[j].Freq, calcfreqPoints[i].Freq));
                        }
                    }

                    if (compfreqPoints[j].Freq >= calcfreqPoints[i].Freq - diffUpperLimit)
                    {
                        if (calcfreqPoints[i].IsLowerAdjFreq(compfreqPoints[j], compfreqPoints[j].Freq, calcfreqPoints[i].Freq))
                        {
                            this.AddLowerAdjCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                string.Format("发射频率({0}MHz)干扰设备发射指配频率({1}MHz)", compfreqPoints[j].Freq, calcfreqPoints[i].Freq));
                        }
                    }

                    //设备主频vs(设备,周围台站)备频
                    if (compfreqPoints[j].SpareFreq > 0)
                    {
                        if (compfreqPoints[j].SpareFreq <= calcfreqPoints[i].Freq + diffUpperLimit)
                        {
                            if (calcfreqPoints[i].IsUpperAdjFreq(compfreqPoints[j], compfreqPoints[j].SpareFreq, calcfreqPoints[i].Freq))
                            {
                                this.AddUpperAdjCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                    string.Format("备用频率({0}MHz)干扰设备发射指配频率({1}MHz)", compfreqPoints[j].SpareFreq, calcfreqPoints[i].Freq));
                            }
                        }

                        if (compfreqPoints[j].SpareFreq >= calcfreqPoints[i].Freq - diffUpperLimit)
                        {
                            if (calcfreqPoints[i].IsLowerAdjFreq(compfreqPoints[j], compfreqPoints[j].SpareFreq, calcfreqPoints[i].Freq))
                            {
                                this.AddLowerAdjCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                    string.Format("备用频率({0}MHz)干扰设备发射指配频率({1}MHz)", compfreqPoints[j].SpareFreq, calcfreqPoints[i].Freq));
                            }
                        }
                    }

                    //设备备频vs(设备,周围台站)主频
                    if (calcfreqPoints[i].SpareFreq > 0)
                    {
                        if (compfreqPoints[j].Freq <= calcfreqPoints[i].SpareFreq + diffUpperLimit)
                        {
                            if (calcfreqPoints[i].IsUpperAdjFreq(compfreqPoints[j], compfreqPoints[j].Freq, calcfreqPoints[i].SpareFreq))
                            {
                                this.AddUpperAdjCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                    string.Format("发射频率({0}MHz)干扰设备备用指配频率({1}MHz)", compfreqPoints[j].Freq, calcfreqPoints[i].SpareFreq));
                            }
                        }

                        if (compfreqPoints[j].Freq >= calcfreqPoints[i].SpareFreq - diffUpperLimit)
                        {
                            if (calcfreqPoints[i].IsLowerAdjFreq(compfreqPoints[j], compfreqPoints[j].Freq, calcfreqPoints[i].SpareFreq))
                            {
                                this.AddLowerAdjCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                    string.Format("发射频率({0}MHz)干扰设备备用指配频率({1}MHz)", compfreqPoints[j].Freq, calcfreqPoints[i].SpareFreq));
                            }
                        }
                    }

                    if (calcfreqPoints[i].SpareFreq > 0 && compfreqPoints[j].SpareFreq > 0)
                    {
                        //设备备频vs(设备,周围台站)备频
                        if (compfreqPoints[j].SpareFreq <= calcfreqPoints[i].SpareFreq + diffUpperLimit)
                        {
                            if (calcfreqPoints[i].IsUpperAdjFreq(compfreqPoints[j], compfreqPoints[j].SpareFreq, calcfreqPoints[i].SpareFreq))
                            {
                                this.AddUpperAdjCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                    string.Format("备用频率({0}MHz)干扰设备备用指配频率({1}MHz)", compfreqPoints[j].SpareFreq, calcfreqPoints[i].SpareFreq));
                            }

                        }

                        if (compfreqPoints[j].SpareFreq >= calcfreqPoints[i].SpareFreq - diffUpperLimit)
                        {
                            if (calcfreqPoints[i].IsLowerAdjFreq(compfreqPoints[j], compfreqPoints[j].SpareFreq, calcfreqPoints[i].SpareFreq))
                            {
                                this.AddLowerAdjCompareResult(calcfreqPoints[i], compfreqPoints[j],
                                    string.Format("备用频率({0}MHz),干扰设备备用指配频率({1}MHz)", compfreqPoints[j].SpareFreq, calcfreqPoints[i].SpareFreq));
                            }
                        }
                    }
                }
            }
            return resultList.Values.ToArray();
        }


        /// <summary>
        /// 向结果中增加上邻频信息
        /// </summary>
        /// <param name="freq">参考频率</param>
        /// <param name="disturbFreq">参考频率的上邻频频率</param>
        private void AddUpperAdjFreq(ComparableFreq freq, ComparableFreq disturbFreq)
        {
            AdjFreqCompareResult result;
            List<ComparableFreq> list;
            if (!this.resultList.TryGetValue(freq, out result))
            {
                if (this.dicFreqMapping.TryGetValue(freq, out list))
                {
                    ComparableFreq[] freqArray = new ComparableFreq[dicFreqMapping[freq].Count + 1];
                    freqArray[0] = freq;
                    dicFreqMapping[freq].CopyTo(freqArray, 1);
                    result = new AdjFreqCompareResult(freqArray);
                }
                else
                    result = new AdjFreqCompareResult(freq);
                this.resultList.Add(freq, result);
            }
            result.AddUpperAdjFreq(disturbFreq);
            if (this.dicFreqMapping.TryGetValue(disturbFreq, out list))
            {
                result.AddUpperAdjFreq(list.ToArray());
            }
        }


        /// <summary>
        /// 向结果中增加下邻频数据
        /// </summary>
        /// <param name="freq">参考频率</param>
        /// <param name="disturbFreq">参考频率的下邻频频率</param>
        private void AddLowerAdjFreq(ComparableFreq freq, ComparableFreq disturbFreq)
        {
            AdjFreqCompareResult result;
            List<ComparableFreq> list;
            if (!this.resultList.TryGetValue(freq, out result))
            {
                if (this.dicFreqMapping.TryGetValue(freq, out list))
                {
                    ComparableFreq[] freqArray = new ComparableFreq[dicFreqMapping[freq].Count + 1];
                    freqArray[0] = freq;
                    dicFreqMapping[freq].CopyTo(freqArray, 1);
                    result = new AdjFreqCompareResult(freqArray);
                }
                else
                    result = new AdjFreqCompareResult(freq);
                this.resultList.Add(freq, result);
            }
            result.AddLowerAdjFreq(disturbFreq);
            if (this.dicFreqMapping.TryGetValue(disturbFreq, out list))
            {
                result.AddLowerAdjFreq(list.ToArray());
            }
        }



        /// <summary>
        /// 向结果中增加上邻频信息
        /// </summary>
        /// <param name="freq">参考频率</param>
        /// <param name="disturbFreq">参考频率的上邻频频率</param>
        private void AddUpperAdjCompareResult(ComparableFreq freq, ComparableFreq disturbFreq, string interfdescribe)
        {
            disturbFreq.InterfereResult = interfdescribe;
            AdjFreqCompareResult result;
            List<ComparableFreq> list;
            if (!this.resultList.TryGetValue(freq, out result))
            {
                if (this.dicFreqMapping.TryGetValue(freq, out list))
                {
                    ComparableFreq[] freqArray = new ComparableFreq[dicFreqMapping[freq].Count + 1];
                    freqArray[0] = freq;
                    dicFreqMapping[freq].CopyTo(freqArray, 1);
                    result = new AdjFreqCompareResult(freqArray);
                }
                else
                    result = new AdjFreqCompareResult(freq);
                this.resultList.Add(freq, result);
            }
            result.AddUpperAdjFreq(disturbFreq);
            if (this.dicFreqMapping.TryGetValue(disturbFreq, out list))
            {
                result.AddUpperAdjFreq(list.ToArray());
            }
        }


        /// <summary>
        /// 向结果中增加上邻频信息
        /// </summary>
        /// <param name="freq">参考频率</param>
        /// <param name="disturbFreq">参考频率的上邻频频率</param>
        private void AddLowerAdjCompareResult(ComparableFreq freq, ComparableFreq disturbFreq, string interfdescribe)
        {
            disturbFreq.InterfereResult = interfdescribe;
            AdjFreqCompareResult result;
            List<ComparableFreq> list;
            if (!this.resultList.TryGetValue(freq, out result))
            {
                if (this.dicFreqMapping.TryGetValue(freq, out list))
                {
                    ComparableFreq[] freqArray = new ComparableFreq[dicFreqMapping[freq].Count + 1];
                    freqArray[0] = freq;
                    dicFreqMapping[freq].CopyTo(freqArray, 1);
                    result = new AdjFreqCompareResult(freqArray);
                }
                else
                    result = new AdjFreqCompareResult(freq);
                this.resultList.Add(freq, result);
            }
            result.AddLowerAdjFreq(disturbFreq);
            if (this.dicFreqMapping.TryGetValue(disturbFreq, out list))
            {
                result.AddLowerAdjFreq(list.ToArray());
            }
        }
    }
}
