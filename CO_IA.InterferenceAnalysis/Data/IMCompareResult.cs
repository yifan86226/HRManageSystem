#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：互调比对结果
 * 日  期：2016-09-07
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{
    public class IMCompareResult
    {
        /// <summary>
        /// 互调项目列表
        /// </summary>
        private List<IMItemBase> imList = new List<IMItemBase>();

        /// <summary>
        /// 同频映射表,记录同频信息(中心频点和带宽完全一致)
        /// </summary>
        private Dictionary<ComparableFreq, List<ComparableFreq>> sameFreqMapping = new Dictionary<ComparableFreq, List<ComparableFreq>>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sameFreqMapping">同频映射表</param>
        public IMCompareResult(Dictionary<ComparableFreq, List<ComparableFreq>> sameFreqMapping)
        {
            this.sameFreqMapping = sameFreqMapping;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IMCompareResult()
        {
        }
        /// <summary>
        /// 向结果中注册互调项目
        /// </summary>
        /// <param name="item">互调项目</param>
        public void RegisterIMItem(IMItemBase item)
        {
            imList.Add(item);
        }

        /// <summary>
        /// 互调分析结果
        /// </summary>
        public IMItemBase[] Values
        {
            get
            {
                return this.imList.ToArray();
            }
        }

        /// <summary>
        /// 获取同频映射表
        /// </summary>
        /// <param name="freq">用于比较的频率</param>
        /// <returns>和当前参数指定频率完全一直的频率组</returns>
        public ComparableFreq[] GetSameFreqMapping(ComparableFreq freq)
        {
            List<ComparableFreq> listFreq;
            if (this.sameFreqMapping.TryGetValue(freq, out listFreq))
            {
                return listFreq.ToArray();
            }
            return new ComparableFreq[0];
        }

        /// <summary>
        /// 获取参数指定频率参与的互调公式
        /// </summary>
        /// <param name="freq">参考频率</param>
        /// <returns>互调结果</returns>
        public IMCompareResult GetIMCompareResult(ComparableFreq freq)
        {
            IMCompareResult result = new IMCompareResult(this.sameFreqMapping);
            ComparableFreq keyFreq = freq;
            foreach (ComparableFreq key in sameFreqMapping.Keys)
            {
                if (freq.IsValueEquals(key))
                {
                    keyFreq = key;
                    break;
                }
            }
            foreach (IMItemBase item in this.imList)
            {
                if (item.ReferFreq(freq))
                    result.RegisterIMItem(item);
            }
            return result;
        }

        /// <summary>
        /// 清理重复干扰结果
        /// </summary>
        public void CleanRepeatDisturbResult()
        {
            for (int i = 0; i < this.imList.Count; i++)
            {
                if (imList[i] != null && imList[i].DisturbedFreqs.Length == 1)
                {
                    IMOrder order = this.imList[i].Order;
                    ComparableFreq[] freqs = this.GetCalcFreqs(imList[i]);
                    Array.Sort(freqs);
                    for (int j = i + 1; j < this.imList.Count; j++)
                    {
                        if (this.imList[j] != null && this.imList[j].Order == order
                            && this.imList[j].DisturbedFreqs.Length == 1)
                        {
                            ComparableFreq[] refFreqs = this.GetCalcFreqs(imList[j]);
                            Array.Sort(refFreqs);
                            if (CompareFreqsArray(freqs, refFreqs))
                            {
                                this.imList[j] = null;
                            }
                        }
                    }
                }
            }
            List<IMItemBase> list = new List<IMItemBase>(imList.Count);
            foreach (IMItemBase item in imList)
            {
                if (item != null)
                    list.Add(item);
            }
            imList = list;
        }

        /// <summary>
        /// 判断参数指定两个频率数组是否相等
        /// </summary>
        /// <param name="one">一个用于比较的频率数组</param>
        /// <param name="other">另一个用于比较的频率数组</param>
        /// <returns>比较结果:true,频率数组相等;false,频率数组不等</returns>
        private bool CompareFreqsArray(ComparableFreq[] one, ComparableFreq[] other)
        {
            if (one.Length == other.Length)
            {
                for (int i = 0; i < one.Length; i++)
                {
                    if (!one[i].Equals(other[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取所有参与计算的频率
        /// </summary>
        /// <param name="item">要计算的互调项目</param>
        /// <returns>参与计算的频率</returns>
        private ComparableFreq[] GetCalcFreqs(IMItemBase item)
        {
            ComparableFreq[] resultFreqs = new ComparableFreq[item.CalcFreqCount + 1];
            for (int i = 0; i < item.CalcFreqCount; i++)
            {
                resultFreqs[i] = item[i];
            }
            resultFreqs[item.CalcFreqCount] = item.DisturbedFreqs[0];
            return resultFreqs;
        }
    }

    /// <summary>
    /// 二阶互调结果
    /// </summary>
    [Serializable]
    public class SecondOrderIMItem : IMItemBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="freqs">组成互调公式的频率</param>
        /// <param name="factors">组成互调公式的频率的计算因子</param>
        public SecondOrderIMItem(ComparableFreq[] freqs, int[] factors)
            : base(freqs, factors)
        {
        }

        /// <summary>
        /// 互调阶数
        /// </summary>
        public override IMOrder Order
        {
            get
            {
                return IMOrder.Second;
            }
        }

        /// <summary>
        /// 验证互调有效性
        /// </summary>
        /// <returns>验证结果:true,有效;false:无效</returns>
        public override bool ValidatingIM()
        {
            if (this.imFreqs.Length == 2 && this.factors.Length == 2 && this.factors[0] == 1 && (this.factors[1] == 1 || this.factors[1] == -1))
            {
                return base.ValidatingIM();
            }
            return false;
        }
    }

    /// <summary>
    /// 三阶互调结果
    /// </summary>
    [Serializable]
    public class ThirdOrderIMItem : IMItemBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="imFreqs">构成互调公式的频率组</param>
        /// <param name="factors">频率组中频率的计算因子</param>
        public ThirdOrderIMItem(ComparableFreq[] imFreqs, int[] factors)
            : base(imFreqs, factors)
        {
        }

        /// <summary>
        /// 互调阶数
        /// </summary>
        public override IMOrder Order
        {
            get
            {
                return IMOrder.Third;
            }
        }

        /// <summary>
        /// 验证互调项目的有效性
        /// </summary>
        /// <returns>互调有效性:true,有效;false:无效</returns>
        public override bool ValidatingIM()
        {
            if (this.factors.Length != this.imFreqs.Length)
            {
                return false;
            }
            int result = 0;
            foreach (int factor in this.factors)
            {
                result += Math.Abs(factor);
            }
            if (result == 3)
                return base.ValidatingIM();
            return false;
        }
    }

    /// <summary>
    /// 五阶互调结果
    /// </summary>
    [Serializable]
    public class FifthIMItem : IMItemBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="imFreqs">构成互调公式的频率组</param>
        /// <param name="factors">频率组中频率的计算因子</param>
        public FifthIMItem(ComparableFreq[] imFreqs, int[] factors)
            : base(imFreqs, factors)
        {
        }

        /// <summary>
        /// 互调阶数
        /// </summary>
        public override IMOrder Order
        {
            get
            {
                return IMOrder.Fifth;
            }
        }

        /// <summary>
        /// 验证互调项目的有效性
        /// </summary>
        /// <returns>互调有效性:true,有效;false:无效</returns>
        public override bool ValidatingIM()
        {
            if (this.factors.Length != this.imFreqs.Length)
            {
                return false;
            }
            int result = 0;
            foreach (int factor in this.factors)
            {
                result += Math.Abs(factor);
            }
            if (result == 5)
                return base.ValidatingIM();
            return false;
        }
    }
}
