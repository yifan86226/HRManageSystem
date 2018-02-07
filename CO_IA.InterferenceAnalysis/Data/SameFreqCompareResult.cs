#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：同频比对结果
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
    public class SameFreqCompareResult
    {
        /// <summary>
        /// 用于比较的频率关键字
        /// </summary>
        private ComparableFreq[] keys;

        /// <summary>
        /// 结果频率列表
        /// </summary>
        private List<ComparableFreq> list = new List<ComparableFreq>();


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="keys">用于获取结果的一组关键字,该组关键字应该具有相同的频率值和带宽</param>
        public SameFreqCompareResult(ComparableFreq[] keys)
        {
            this.keys = keys;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">用于获取结果的频率</param>
        public SameFreqCompareResult(ComparableFreq key)
        {
            this.keys = new ComparableFreq[1];
            this.keys[0] = key;
        }

        /// <summary>
        /// 向结果中添加频率
        /// </summary>
        /// <param name="freq">待添加频率</param>
        public void AddFreq(ComparableFreq freq)
        {
            this.list.Add(freq);
        }

        /// <summary>
        /// 想结果中添加一组频率
        /// </summary>
        /// <param name="freqs">待添加的一组频率</param>
        public void AddFreqRange(ComparableFreq[] freqs)
        {
            this.list.AddRange(freqs);
        }

        /// <summary>
        /// 频率关键字,结果数组中的每一个频率都和关键字中任一个频率存在指定关系
        /// </summary>
        public ComparableFreq[] Keys
        {
            get
            {
                return this.keys;
            }
        }

        /// <summary>
        /// 结果数组
        /// </summary>
        public ComparableFreq[] Values
        {
            get
            {
                return this.list.ToArray();
            }
        }

        /// <summary>
        /// 干扰物设备/台站
        /// </summary>
        public InterfereObjectEnum InterfObject
        {
            get;
            set;
        }
    }
}
