#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：用于计算的比对频率
 * 日  期：2016-09-07
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{
    public struct ComparableFreq : IComparable<ComparableFreq>
    {
        public static readonly ComparableFreq InvalidFreq = new ComparableFreq(0, 0, 0, "-1");
        /// <summary>
        /// 0值上限,当小数值大于该值时认为其大于0
        /// </summary>
        public const double PositiveZero = 0.000001;

        /// <summary>
        /// 0值下限,当小数值小于该值时认为其小于0
        /// </summary>
        public const double NegativeZero = -0.000001;

        /// <summary>
        /// 中心频点/起始频点
        /// </summary>
        private double freq;

        /// <summary>
        /// 终止频点
        /// </summary>
        private double endFreq;

        /// <summary>
        /// 备用频率
        /// </summary>
        private double sparefreq;

        /// <summary>
        /// 频点标识
        /// </summary>
        private bool isFreqPoint;
      
        /// <summary>
        /// 频率标识
        /// </summary>
        private readonly string freqID;

        /// <summary>
        /// 带宽
        /// </summary>
        private double band;

        /// <summary>
        /// 
        /// </summary>
        private string interferereset;

        /// <summary>
        /// 构造函数计算单位（MHz）
        /// </summary>
        /// <param name="freq">中心频点</param>
        /// <param name="band">带宽</param>
        /// <param name="id">频率id</param>
        /// <param name="result">干扰结果,在最后取干扰结果时用到,初始化频率时,请传空</param>
        public ComparableFreq(double freq, double sparefreq, double band, string id, string result = null)
        {
            this.freq = Math.Round(freq, 4, MidpointRounding.AwayFromZero);
            this.sparefreq = Math.Round(sparefreq, 4, MidpointRounding.AwayFromZero);
            this.band = band;
            this.freqID = id;
            endFreq = 0;
            isFreqPoint = true;
            this.interferereset = result;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="from">频段起始频率</param>
        /// <param name="to">频段终止频率</param>
        /// <param name="band">频段内频率带宽</param>
        /// <param name="id">标识</param>
        public ComparableFreq(double from, double to, double backupfreq, double band, string id, string result = null)
        {
            this.freq = Math.Round(from, 4, MidpointRounding.AwayFromZero);
            this.endFreq = Math.Round(to, 4, MidpointRounding.AwayFromZero);
            this.sparefreq = Math.Round(backupfreq, 4, MidpointRounding.AwayFromZero);
            this.band = band;
            this.freqID = id;
            this.isFreqPoint = false;
            this.interferereset = result;
        }


        #region IComparableFreq 成员

        /// <summary>
        /// 对象表示的频率的中心频率值
        /// </summary>
        public double Freq
        {
            get
            {
                return this.freq;
            }
        }

        public double SpareFreq
        {
            get
            {
                return this.sparefreq;
            }
        }

        public double From
        {
            get
            {
                return this.freq;
            }
        }

        public double To
        {
            get
            {
                return this.endFreq;
            }
        }

        public bool IsFreqPoint
        {
            get
            {
                return this.isFreqPoint;
            }
        }

        /// <summary>
        /// 对象表示的频率的带宽
        /// </summary>
        public double Band
        {
            get
            {
                return this.band;
            }
        }

        /// <summary>
        /// 用于描述该频率的干扰结果
        /// </summary>
        public string InterfereResult
        {
            get
            {
                return this.interferereset;
            }
            set
            {
                interferereset = value;
            }
        }
        /// <summary>
        /// 判断同另一个频率是否为同频关系
        /// </summary>
        /// <param name="compareFreq">用于比较的另一个频率对象</param>
        /// <returns>比较结果,true表示参数对象和当前对象同频,false:不是同频</returns>
        public bool IsSameFreq(ComparableFreq compareFreq)
        {
            return compareFreq.IsSameFreq(freq, band);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compareFreq">比较频率</param>
        /// <param name="compfreq">比较频率值</param>
        /// <param name="calcfreq">计算频率值</param>
        /// <returns></returns>
        public bool IsSameSpareFreq(ComparableFreq compareFreq, double compfreq, double calcfreq)
        {
            return compareFreq.IsSameShareFreq(compfreq, calcfreq, band);
        }

        /// <summary>
        /// 内部同频比较方法
        /// </summary>
        /// <param name="freq">用于比较的中心频点</param>
        /// <param name="band">用于比较的频段</param>
        /// <returns>true:同频;false:不是同频</returns>
        private bool IsSameFreq(double freq, double band)
        {
            //频率差不能超过带宽差一半
            //return Math.Abs(freq - this.freq) <= Math.Abs((band - this.band) / 2);
            return Math.Abs((band - this.band) / 2) - Math.Abs(freq - this.freq) > NegativeZero;
        }

        /// <summary>
        /// 备用频率比对
        /// </summary>
        /// <param name="freq"></param>
        /// <param name="sparefreq"></param>
        /// <param name="band"></param>
        /// <returns></returns>
        private bool IsSameShareFreq(double comparefreq, double calcfreq, double band)
        {
            //频率差不能超过带宽差一半
            return Math.Abs((band - this.band) / 2) - Math.Abs(comparefreq - calcfreq) > NegativeZero;
        }


        /// <summary>
        /// 上邻频判断方法
        /// </summary>
        /// <param name="compareFreq">用于比较的频率</param>
        /// <returns>true:是上邻频关系;false:不是上邻频关系</returns>
        public bool IsUpperAdjFreq(ComparableFreq compareFreq)
        {
            return compareFreq.IsSameFreq(freq - band, band);
        }

        public bool IsUpperAdjFreq(ComparableFreq compareFreq, double compfreq, double calcfreq)
        {
            return compareFreq.IsSameSpareFreq(compareFreq, compfreq, calcfreq - band);
        }

        /// <summary>
        /// 下邻频判断方法
        /// </summary>
        /// <param name="compareFreq">用于比较的频率</param>
        /// <returns>true:是下邻频关系;false:不是下邻频关系</returns>
        public bool IsLowerAdjFreq(ComparableFreq compareFreq)
        {
            return compareFreq.IsSameFreq(freq + band, band);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="compareFreq"></param>
        /// <param name="compfreq"></param>
        /// <param name="calcfreq"></param>
        /// <returns></returns>
        public bool IsLowerAdjFreq(ComparableFreq compareFreq, double compfreq, double calcfreq)
        {
            return compareFreq.IsSameSpareFreq(compareFreq, compfreq, calcfreq + band);
        }

        /// <summary>
        /// 频率ID,用于区分值相同的频率
        /// </summary>
        public string FreqID
        {
            get
            {
                return this.freqID;
            }
        }

        #endregion
        /// <summary>
        /// 比较当前频率和同一类型的另一对象。
        /// </summary>
        /// <param name="other">与此频率进行比较的频率</param>
        /// <returns>一个值，指示要比较的对象的相对顺序。返回值的含义如下：值含义小于零此对象小于 other 参数。零此对象等于 other。大于零此对象大于 other。</returns>
        public int CompareTo(ComparableFreq other)
        {
            int result = this.freq.CompareTo(other.freq);
            if (result == 0)
                return this.band.CompareTo(other.band);
            return result;
        }

        /// <summary>
        /// 判断同另一个频率相比较是否具有相同的中心频点和带宽
        /// </summary>
        /// <param name="other">与此频率进行比较的频率</param>
        /// <returns>true:两个对象具有相同的中心频点和带宽;false:带宽或者中心频点存在不同</returns>
        public bool IsValueEquals(ComparableFreq other)
        {
            return this.freq == other.freq && this.band == other.band;
        }

        public bool IsValidFreq
        {
            get
            {
                return this.Equals(InvalidFreq);
            }
        }

        public bool ContainFreq(double freq)
        {
            double diff = Math.Round(band / 2, 4, MidpointRounding.AwayFromZero);
            return freq > this.freq - diff && freq < this.freq + diff;
        }
    }
}
