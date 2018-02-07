#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：干扰分析返回结果
 * 日  期：2016-09-09
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{
    /// <summary>
    /// 干扰分析返回结果
    /// </summary>
    public class InterferenceAnalysisResult
    {
        /// <summary>
        /// 干扰总数
        /// </summary>
        public int Total
        {
            get { return SameFreqInterfResultCount + ADJFreqInterfResultCount + IMInterfResultCount; }
        }

        /// <summary>
        /// 同频干扰数量
        /// </summary>
        public int SameFreqInterfResultCount
        {
            get;
            set;
        }

        /// <summary>
        /// 邻频干扰数量
        /// </summary>
        public int ADJFreqInterfResultCount
        {
            get;
            set;
        }

        /// <summary>
        /// 互调干扰结果
        /// </summary>
        public int IMInterfResultCount
        {
            get;
            set;
        }

        /// <summary>
        /// 同频干扰结果
        /// </summary>
        public SameFreqCompareResult[] SameFreqInterfResult
        {
            get;
            set;
        }

        /// <summary>
        /// 邻频干扰结果
        /// </summary>
        public AdjFreqCompareResult[] ADJFreqInterfResult
        {
            get;
            set;
        }

        /// <summary>
        /// 互调干扰结果
        /// </summary>
        public IMAnalysisResult IMInterfResult
        {
            get;
            set;
        }
    }
}
