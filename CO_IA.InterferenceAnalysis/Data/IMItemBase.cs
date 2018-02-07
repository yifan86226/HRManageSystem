using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{
    public abstract class IMItemBase
    {
        /// <summary>
        /// 互调计算项目频率
        /// </summary>
        protected ComparableFreq[] imFreqs;

        /// <summary>
        /// 互调计算频率因子
        /// </summary>
        protected int[] factors;

        /// <summary>
        /// 受干扰频率组
        /// </summary>
        private ComparableFreq[] disturbedFreqs = new ComparableFreq[0];

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="freqs">计算公式用频</param>
        /// <param name="factors">计算公式用频因子</param>
        public IMItemBase(ComparableFreq[] freqs, int[] factors)
        {
            this.imFreqs = freqs;
            this.factors = factors;
        }

        /// <summary>
        /// 互调阶数
        /// </summary>
        public abstract IMOrder Order
        {
            get;
        }

        /// <summary>
        /// 受该频率组互调影响的的频率组
        /// </summary>
        public ComparableFreq[] DisturbedFreqs
        {
            get
            {
                return this.disturbedFreqs;
            }
            set
            {
                this.disturbedFreqs = value;
            }
        }

        /// <summary>
        /// 影响互调的频率组
        /// </summary>
        public ComparableFreq[] IMFreqs
        {
            get 
            {
                return this.imFreqs;
            }
            set
            {
                this.imFreqs = value;
            }
        }

        /// <summary>
        /// 获取互调公式
        /// </summary>
        /// <param name="key">公式中频率的替代字母</param>
        /// <returns>互调公式</returns>
        public virtual string GetFormula(string key)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            if (factors[0] == 1)
            {
                builder.AppendFormat("{0}{1}", key, 1);
            }
            else
            {
                builder.AppendFormat("{0}*{1}{2}", factors[0], key, 1);
            }
            for (int i = 1; i < factors.Length; i++)
            {
                if (factors[i] == 1)
                {
                    builder.AppendFormat("+{0}{1}", key, i + 1);
                }
                else if (factors[i] == -1)
                {
                    builder.AppendFormat("-{0}{1}", key, i + 1);
                }
                else if (factors[i] > 1)
                {
                    builder.AppendFormat("+{0}*{1}{2}", factors[i], key, i + 1);
                }
                else if (factors[i] < -1)
                {
                    builder.AppendFormat("{0}*{1}{2}", factors[i], key, i + 1);
                }
            }
            builder.AppendFormat("={0}{1}", key, factors.Length + 1);
            return builder.ToString();
        }

        /// <summary>
        /// 获取互调公式
        /// </summary>
        public virtual string Formula
        {
            get
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                if (factors[0] == 1)
                {
                    builder.AppendFormat("{0}", imFreqs[0].Freq);
                }
                else
                {
                    builder.AppendFormat("{0}*{1}", factors[0], imFreqs[0].Freq);
                }
                for (int i = 1; i < factors.Length; i++)
                {
                    if (factors[i] == 1)
                    {
                        builder.AppendFormat("+{0}", imFreqs[i].Freq);
                    }
                    else if (factors[i] == -1)
                    {
                        builder.AppendFormat("-{0}", imFreqs[i].Freq);
                    }
                    else if (factors[i] > 1)
                    {
                        builder.AppendFormat("+{0}*{1}", factors[i], imFreqs[i].Freq);
                    }
                    else if (factors[i] < -1)
                    {
                        builder.AppendFormat("{0}*{1}", factors[i], imFreqs[i].Freq);
                    }
                }
                builder.AppendFormat("={0}", this.DisturbedStandardFreq.Freq);
                return builder.ToString();
            }
        }

        /// <summary>
        /// 互调标准频率,该频率是根据互调公式计算出来的频率
        /// </summary>
        public ComparableFreq DisturbedStandardFreq
        {
            get
            {
                double freq = 0;
                double band = double.MaxValue;
                for (int i = 0; i < factors.Length; i++)
                {
                    freq += factors[i] * imFreqs[i].Freq;
                    if (imFreqs[i].Band < band)
                    {
                        band = imFreqs[i].Band;
                    }
                }
                return new ComparableFreq(freq,0, band, "0");
            }
        }

        /// <summary>
        /// 获取存在受干扰频率标识
        /// </summary>
        public bool ExistDisturbedFreq
        {
            get
            {
                return this.disturbedFreqs.Length > 0;
            }
        }

        /// <summary>
        /// 验证互调公式的有效性
        /// </summary>
        /// <returns>计算结果:true,互调公式有效;false,无效</returns>
        public virtual bool ValidatingIM()
        {
            return this.ExistDisturbedFreq;
        }

        /// <summary>
        /// 按照索引获取公式中频率
        /// </summary>
        /// <param name="i">索引号</param>
        /// <returns>该索引指向的公式中用频</returns>
        public ComparableFreq this[int i]
        {
            get
            {
                return this.imFreqs[i];
            }
        }

        /// <summary>
        /// 公式中频率数量
        /// </summary>
        public int CalcFreqCount
        {
            get
            {
                return this.imFreqs.Length;
            }
        }

        /// <summary>
        /// 判断干扰公式是否涉及参数指定频率
        /// </summary>
        /// <param name="freq">判定频率</param>
        /// <returns>判断结果:true,公式中使用的该频率;false,公式中未使用该频率</returns>
        public bool ReferFreq(ComparableFreq freq)
        {
            foreach (ComparableFreq compareFreq in this.imFreqs)
            {
                if (freq.Equals(compareFreq))
                    return true;
            }
            foreach (ComparableFreq compareFreq in this.disturbedFreqs)
            {
                if (freq.Equals(compareFreq))
                    return true;
            }
            return false;
        }
    }
    /// <summary>
    /// 互调阶数
    /// </summary>
    [Flags]
    public enum IMOrder : short
    {
        None = 0,
        Second = 4,
        Third = 8,
        Fifth = 32
    }
}
