#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：邻频干扰结果
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
    public class AdjFreqCompareResult
    {
        /// <summary>
        /// 关键字,邻频结果是基于该频率数组进行比较产生的,数组中频率应该严格相等
        /// </summary>
        private ComparableFreq[] keys;

        /// <summary>
        /// 上邻频列表
        /// </summary>
        private List<ComparableFreq> upperAdjFreqList = new List<ComparableFreq>();

        /// <summary>
        /// 下邻频列表
        /// </summary>
        private List<ComparableFreq> lowerAdjFreqList = new List<ComparableFreq>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="freq">计算邻频的频率</param>
        public AdjFreqCompareResult(ComparableFreq freq)
        {
            this.keys = new ComparableFreq[1];
            this.keys[0] = freq;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="freqs">计算邻频的频率组</param>
        public AdjFreqCompareResult(ComparableFreq[] freqs)
        {
            this.keys = freqs;
        }

        /// <summary>
        /// 上邻频存在标识
        /// </summary>
        public bool HasUpperAdjFreq
        {
            get
            {
                return this.upperAdjFreqList.Count > 0;
            }
        }

        /// <summary>
        /// 下邻频存在标识
        /// </summary>
        public bool HasLowerAdjFreq
        {
            get
            {
                return this.lowerAdjFreqList.Count > 0;
            }
        }

        /// <summary>
        /// 向结果中添加上邻频数据
        /// </summary>
        /// <param name="freqs">要添加的上邻频</param>
        public void AddUpperAdjFreq(params ComparableFreq[] freqs)
        {
            this.upperAdjFreqList.AddRange(freqs);
        }

        /// <summary>
        /// 向结果中添加下邻频数据
        /// </summary>
        /// <param name="freqs">要添加的下邻频</param>
        public void AddLowerAdjFreq(params ComparableFreq[] freqs)
        {
            this.lowerAdjFreqList.AddRange(freqs);
        }

        /// <summary>
        /// 获取上邻频结果
        /// </summary>
        public ComparableFreq[] UpperAdjFreqs
        {
            get
            {
                return this.upperAdjFreqList.ToArray();
            }
        }

        /// <summary>
        /// 获取下邻频结果
        /// </summary>
        public ComparableFreq[] LowerAdjFreqs
        {
            get
            {
                return this.lowerAdjFreqList.ToArray();
            }
        }

        /// <summary>
        /// 获取关键字频率
        /// </summary>
        public ComparableFreq[] Keys
        {
            get
            {
                return keys;
            }
        }

        //public string InterfereFreq
        //{
        //    get;
        //    set;
        //}
    }
}
