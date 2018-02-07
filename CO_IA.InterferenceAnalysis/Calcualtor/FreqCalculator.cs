#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：频率计算
 * 日  期：2016-09-07
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{
    public abstract class FreqCalculator
    {
        /// <summary>
        /// 用于计算其间关系的频段数组
        /// </summary>
        protected ComparableFreq[] freqRanges;


        /// <summary>
        /// 用于计算其间关系的频点数组
        /// </summary>
        protected ComparableFreq[] compfreqPoints;

        /// <summary>
        /// 用于计算的频点
        /// </summary>
        protected ComparableFreq[] calcfreqPoints;

        ///// <summary>
        ///// 用于计算其间关系的频段数组
        ///// </summary>
        //protected ComparableFreq[] freqRanges;

        /// <summary>
        /// 相同频率映射表,表中数据频率带宽完全一致
        /// </summary>
        protected Dictionary<ComparableFreq, List<ComparableFreq>> dicFreqMapping = new Dictionary<ComparableFreq, List<ComparableFreq>>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="freqs">该对象处理的频率组</param>
        public FreqCalculator(ComparableFreq[] freqs)
        {
            List<ComparableFreq> listFreqPoint = new List<ComparableFreq>();
            List<ComparableFreq> listFreqRange = new List<ComparableFreq>();
            foreach (ComparableFreq freq in freqs)
            {
                //listFreqPoint.Add(freq);
                if (freq.IsFreqPoint)
                {
                    listFreqPoint.Add(freq);
                }
                else
                {
                    listFreqRange.Add(freq);
                }
            }
            this.compfreqPoints = listFreqPoint.ToArray();
            this.freqRanges = listFreqRange.ToArray();
        }
        public FreqCalculator(ComparableFreq[] calcfreqs, ComparableFreq[] comparefreqs)
        {
            List<ComparableFreq> listCalcFreqPoint = new List<ComparableFreq>();
            List<ComparableFreq> listComFreqPoint = new List<ComparableFreq>();
            foreach (ComparableFreq item in calcfreqs)
            {
                listCalcFreqPoint.Add(item);
            }
            foreach (ComparableFreq freq in comparefreqs)
            {
                listComFreqPoint.Add(freq);
            }
            this.calcfreqPoints = listCalcFreqPoint.ToArray();
            this.compfreqPoints = listComFreqPoint.ToArray();

            Array.Sort(this.calcfreqPoints);
            Array.Sort(this.compfreqPoints);
        }

        protected void GenerateSameMapping()
        {
            Array.Sort(compfreqPoints);
            List<ComparableFreq> resultList = new List<ComparableFreq>(compfreqPoints.Length);
            for (int i = 0; i < compfreqPoints.Length; )
            {
                resultList.Add(compfreqPoints[i]);
                int j = i + 1;
                for (; j < compfreqPoints.Length; j++)
                {
                    if (compfreqPoints[j].IsValueEquals(compfreqPoints[i]))
                    {
                        List<ComparableFreq> list;
                        if (!dicFreqMapping.TryGetValue(compfreqPoints[i], out list))
                        {
                            list = new List<ComparableFreq>();
                            dicFreqMapping.Add(compfreqPoints[i], list);
                        }
                        list.Add(compfreqPoints[j]);
                    }
                    else
                        break;
                }
                i = j;
            }
            compfreqPoints = resultList.ToArray();
        }

        protected void InitSameMapping()
        {
            Array.Sort(calcfreqPoints);
            List<ComparableFreq> resultList = new List<ComparableFreq>(calcfreqPoints.Length);
            for (int i = 0; i < calcfreqPoints.Length; )
            {
                resultList.Add(calcfreqPoints[i]);
                int j = i + 1;
                for (; j < calcfreqPoints.Length; j++)
                {
                    if (calcfreqPoints[j].IsValueEquals(calcfreqPoints[i]))
                    {
                        List<ComparableFreq> list;
                        if (!dicFreqMapping.TryGetValue(calcfreqPoints[i], out list))
                        {
                            list = new List<ComparableFreq>();
                            dicFreqMapping.Add(calcfreqPoints[i], list);
                        }
                        list.Add(calcfreqPoints[j]);
                    }
                    else
                        break;
                }
                i = j;
            }
            calcfreqPoints = resultList.ToArray();
        }


        /// <summary>
        /// 获取最大带宽值
        /// </summary>
        /// <returns>频率组中的最大带宽值</returns>
        protected double GetMaxBand()
        {
            double max = 0;
            foreach (ComparableFreq freq in compfreqPoints)
            {
                if (freq.Band > max)
                {
                    max = freq.Band;
                }
            }
            return max;
        }
    }
}
