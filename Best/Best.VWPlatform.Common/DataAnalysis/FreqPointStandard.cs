using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Best.VWPlatform.Common.Types;

namespace Best.VWPlatform.Common.DataAnalysis
{

        [Serializable]
        public class FreqPointStandard : FreqPointMonitor
        {
            public FreqPointStandard()
            {
                freqPoint = 0;
                freqWidth = 0;
            }
            public FreqPointStandard(FreqPointMonitor freqMonitor)
            {
                freqPoint = freqMonitor.freqPoint;
                freqWidth = freqMonitor.freqWidth;
            }
            public int Upper;
            public string strGroup;
            public string strService;
            public string strRemark;
            public string strSysName;
            public string strEmergency;
            public string strSpecialness;
            public int ChannelNum;
        }
        /// <summary>
        /// 监测频点数据结构
        /// </summary>

        [Serializable]
        public class FreqPointMonitor
        {
            /// <summary>
            /// 获取或设置频点Id
            /// </summary>
            public int FreqId { get; set; }
            public double freqPoint;//MHz
            public double freqWidth;//kHz
            public FreqPointMonitor()
            {
                freqPoint = 0;
                freqWidth = 0;
            }

            public FreqPointMonitor(double fp, double fw)
            {
                freqPoint = fp;
                freqWidth = fw;
            }
        }

        /// <summary>
        /// 空域数据结构
        /// </summary>
        [Serializable]
        public class SpaceMonitor
        {
            public double longi;//经度
            public double lati;//纬度
            public bool canuse;//是否可用
            public SpaceMonitor()
            {
                longi = 0;
                lati = 0;
                canuse = false;
            }

            public SpaceMonitor(double lgi, double lai)
            {
                longi = lgi;
                lati = lai;
                canuse = true;
            }
        }


        /// <summary>
        /// 时域数据结构
        /// </summary>
        [Serializable]
        public class TimeMonitor
        {
            public string datetime;//信号发现时间
            public bool canuse;//是否可用
            public TimeMonitor()
            {
                datetime = "";
                canuse = false;
            }

            public TimeMonitor(string dt)
            {
                datetime = dt;
                canuse = true;
            }
        }


        /// <summary>
        /// 调制域数据结构
        /// </summary>
        [Serializable]
        public class ModMonitor
        {
            public string modulation;//调制样式
            public bool canuse;//是否可用
            public ModMonitor()
            {
                modulation = "31";
                canuse = false;
            }

            public ModMonitor(string mod)
            {
                modulation = mod;
                canuse = true;
            }
        }

        /// <summary>
        /// 能量域数据结构
        /// </summary>
        [Serializable]
        public class EnergyMonitor
        {
            public double energy;//幅度值
            public bool canuse;//是否可用
            public EnergyMonitor()
            {
                energy = 0;
                canuse = false;
            }

            public EnergyMonitor(double egy)
            {
                energy = egy;
                canuse = true;
            }
        }


        /// <summary>
        /// 判定时数据结构
        /// </summary>
        [Serializable]
        public class MaxDistanceMonitor
        {
            public double Distance;//允许的最大距离
            public bool canuse;//是否可用
            public MaxDistanceMonitor()
            {
                Distance = 1;
                canuse = false;
            }

            public MaxDistanceMonitor(double dist)
            {
                Distance = dist;
                canuse = true;
            }
        }

        /// <summary>
        /// 信号定性传入数据结构
        /// </summary>
        [Serializable]
        public class FreqsMonitor
        {
            public MaxDistanceMonitor maxdist;         //最大距离
            public FreqPointMonitor fpm;                //频域数据
            public TimeMonitor tm;                      //时域数据
            public SpaceMonitor sm;                     //空域数据
            public ModMonitor mm;                       //调制域数据
            public EnergyMonitor em;                    //能量域数据
            public FreqsMonitor()
            {

            }

        }


        [Serializable]
        public class ChannelEnvInfo
        {
            private FreqPointStandard freqStandard = new FreqPointStandard();
            public FreqPointMonitor freqInfoMonitor//监测频点信息
            {
                get
                {

                    return (FreqPointMonitor)freqStandard;

                }
                set
                {
                    freqStandard.freqPoint = value.freqPoint;
                    freqStandard.freqWidth = value.freqWidth;
                }
            }

            public FreqPointStandard freqInfoStandard//标准频点信息
            {
                get
                {
                    return freqStandard;
                }
                set
                {
                    freqStandard = value;
                }
            }
            public ChannelEnvInfo()
            {
            }
            public ChannelEnvInfo(double freqCenter, double freqWidth)
            {
                bStandard = false;
                freqInfoMonitor = new FreqPointMonitor(freqCenter, freqWidth);
            }
            public List<FreqElectroEnv> ElectroEnv = new List<FreqElectroEnv>();//各监测站测量的电磁环境结果
            public List<RadioStationFreq> listRS = new List<RadioStationFreq>();//相关发射台站
            public float maxValue = 0;//最大场强 dB(μv)/m
            public DateTime maxValueTime;//最大场强时间 dB(μv)/m
            public float aveValue = 0;//平均场强 dB(μv)/m
            public float noiseValue = -300;//统计噪声
            public float fOccupy = 0;//占用度 <=1
            public float fRecentMaxValue = 0;//最近最大场强
            public float fRecentOccupy = 0;//最近占用度
            public int Signal = 0;//信号有无
            public DateTime foremostTime = new DateTime(2900, 1, 1);//监测数据时间范围
            public DateTime latermostTime = new DateTime(1900, 1, 1);
            public int amountHours = 0;//总监测时间
            public int amountSamples = 0;//总采样次数 <=0时说明场强和占用度无数据
            public int amountNoiseSamples = 0;//噪声出现次数
            ///综合特征值
            public bool bDeviantFree = false;//已指配未使用
            public bool bMonstrousPower = false;//功率超标
            public bool bAbused = false;//未知占用
            public string strSproperty = ""; //信号性质
            public bool bLegalOccupied = false;//合法占用
            /// <summary>
            /// 是标准频点或是监测频点
            /// </summary>
            private bool bStandard = true;
            public bool IsStandardFreq
            {
                get { return bStandard; }
            }

            private void InitializeProperties()
            {
                maxValue = 0;
                aveValue = 0;
                noiseValue = -300;
                fOccupy = 0;
                fRecentMaxValue = 0;
                fRecentOccupy = 0;
                Signal = 0;
                foremostTime = new DateTime(2900, 1, 1);
                latermostTime = new DateTime(1900, 1, 1);
                amountHours = 0;
                amountSamples = 0;
                amountNoiseSamples = 0;
                ///综合特征值
                bDeviantFree = false;//已指配未使用
                bMonstrousPower = false;//功率超标
                bAbused = false;//未知占用
                strSproperty = ""; //信号性质
                bLegalOccupied = false;//合法占用
            }
            public ChannelEnvInfo Clone(bool bCloneWithEnvlist)
            {
                ChannelEnvInfo envCloned = new ChannelEnvInfo();
                envCloned.freqInfoMonitor = freqInfoMonitor;
                envCloned.maxValue = maxValue;
                envCloned.maxValueTime = maxValueTime;
                envCloned.aveValue = aveValue;
                envCloned.noiseValue = noiseValue;
                envCloned.fOccupy = fOccupy;
                envCloned.fRecentMaxValue = fRecentMaxValue;
                envCloned.fRecentOccupy = fRecentOccupy;
                envCloned.Signal = Signal;
                envCloned.foremostTime = foremostTime;
                envCloned.latermostTime = latermostTime;
                envCloned.amountHours = amountHours;
                envCloned.amountSamples = amountSamples;
                envCloned.amountNoiseSamples = amountNoiseSamples;
                envCloned.bDeviantFree = bDeviantFree;
                envCloned.bLegalOccupied = bLegalOccupied;
                envCloned.bAbused = bAbused;
                envCloned.strSproperty = strSproperty;
                envCloned.bMonstrousPower = bMonstrousPower;

                envCloned.bStandard = bStandard;
                if (bCloneWithEnvlist)
                {
                    foreach (FreqElectroEnv freqEnv in ElectroEnv)
                    {
                        envCloned.ElectroEnv.Add(freqEnv.Clone());
                    }
                }

                return envCloned;
            }

            /// <summary>
            ///  计算一天24小时内，起始点时段到终止时段间的平均场强值
            /// </summary>
            /// <param name="start">起始时段（第i个15分钟）</param>
            /// <param name="end">终止时段（第i个15分钟）</param>
            /// <returns>平均场强值 dB(μv)/m</returns>
            public float GetAvgValue(int start, int end)
            {
                if (start < 0 || start >= 96)
                {
                    return -1;
                }
                else if (end < 0 || end >= 96)
                {
                    return -1;
                }
                else if (start >= end)
                {
                    return -1;
                }
                int count = 0;
                float ret = 0;
                for (int i = start; i <= end; i++)
                {
                    count++;
                    ret += ElectroEnv[i].aveValue;
                }
                ret = ret / count;

                return ret;

            }

            /// <summary>
            /// 多监测站测量值合并
            /// </summary>
            /// <param name="listMonitorStations">监测站列表</param>
            
            //public void AggregateEnvValues(List<StationInfo> listMonitorStations)
            //{
            //    InitializeProperties();
            //    for (int i = 0; i < ElectroEnv.Count; i++)
            //    {
            //        if (listMonitorStations.Find(
            //              delegate(StationInfo value)
            //              {
            //                  return (ElectroEnv[i].MSID.CompareTo(value.Guid) == 0);
            //              }
            //          ) != null)
            //        {
            //            if (maxValue < ElectroEnv[i].maxValue)
            //            {
            //                maxValue = ElectroEnv[i].maxValue;
            //                maxValueTime = ElectroEnv[i].maxValueTime;
            //            }
            //            aveValue = (float)((dB2TrueValue(aveValue) * amountSamples + dB2TrueValue(ElectroEnv[i].aveValue) * ElectroEnv[i].amountSamples)
            //                    / (amountSamples + ElectroEnv[i].amountSamples));
            //            aveValue = (float)TrueValue2dB(aveValue);
            //            if (ElectroEnv[i].noiseValue > -300 && ElectroEnv[i].noiseValue < 300)
            //            {
            //                if (noiseValue > -300 && noiseValue < 300)
            //                {
            //                    noiseValue = (float)((dB2TrueValue(noiseValue) * amountNoiseSamples + dB2TrueValue(ElectroEnv[i].noiseValue) * ElectroEnv[i].noiseSamples)
            //                            / (amountNoiseSamples + ElectroEnv[i].noiseSamples));
            //                    noiseValue = (float)TrueValue2dB(noiseValue);
            //                }
            //                else
            //                {
            //                    noiseValue = ElectroEnv[i].noiseValue;
            //                }
            //                amountNoiseSamples += ElectroEnv[i].noiseSamples;
            //            }
            //            amountSamples += ElectroEnv[i].amountSamples;
            //            amountHours += ElectroEnv[i].amountHours;
            //            if (fOccupy < ElectroEnv[i].fOccupy)
            //                fOccupy = ElectroEnv[i].fOccupy;
            //            if (fRecentMaxValue < ElectroEnv[i].fRecentMaxValue)
            //                fRecentMaxValue = ElectroEnv[i].fRecentMaxValue;
            //            if (fRecentOccupy < ElectroEnv[i].fRecentOccupy)
            //                fRecentOccupy = ElectroEnv[i].fRecentOccupy;
            //            if (ElectroEnv[i].Signal == 1)
            //                Signal = 1;
            //            if (foremostTime > ElectroEnv[i].foremostTime)
            //                foremostTime = ElectroEnv[i].foremostTime;
            //            if (latermostTime < ElectroEnv[i].latermostTime)
            //                latermostTime = ElectroEnv[i].latermostTime;
            //        }
            //    }
            //}


            /// <summary>
            /// 相对值转绝对值
            /// </summary>
            /// <param name="fField">相对值</param>
            /// <returns>绝对值</returns>
            public static double dB2TrueValue(float fField)
            {
                return Math.Pow(10, fField / 10.0f);
            }
            /// <summary>
            /// 绝对值转相对值
            /// </summary>
            /// <param name="fLevel">绝对值</param>
            /// <returns>相对值</returns>
            public static double TrueValue2dB(float fLevel)
            {
                return 10 * Math.Log10(fLevel);
            }


        }
        /// <summary>
        /// 频点电磁环境特征数据结构
        /// </summary>
        [Serializable]
        public class FreqElectroEnv
        {
            public string MSID = "";
            public float maxValue;//最大场强 dB(μv)/m
            public DateTime maxValueTime;
            public float aveValue = 0;//平均场强
            public float noiseValue = 0;
            public float fOccupy;//占用度 <=1
            public float fRecentMaxValue;
            public float fRecentOccupy;//最近占用度
            public int Signal;
            public DateTime foremostTime;
            public DateTime latermostTime;
            public int amountHours;
            public int amountSamples;
            public int noiseSamples;

            public float order90 = 0;//dB(μv)/m
            public float order75 = 0;
            public float order65 = 0;
            public float order50 = 0;
            public float order35 = 0;
            public float order20 = 0;
            public float order10 = 0;


            public FreqElectroEnv Clone()
            {
                FreqElectroEnv envCloned = new FreqElectroEnv();
                envCloned.MSID = MSID;
                envCloned.maxValue = maxValue;
                envCloned.maxValueTime = maxValueTime;
                envCloned.aveValue = aveValue;
                envCloned.noiseValue = noiseValue;
                envCloned.fOccupy = fOccupy;
                envCloned.fRecentMaxValue = fRecentMaxValue;
                envCloned.fRecentOccupy = fRecentOccupy;
                envCloned.Signal = Signal;
                envCloned.foremostTime = foremostTime;
                envCloned.latermostTime = latermostTime;
                envCloned.amountHours = amountHours;
                envCloned.amountSamples = amountSamples;
                envCloned.noiseSamples = noiseSamples;

                envCloned.order10 = order10;
                envCloned.order50 = order50;
                envCloned.order90 = order90;
                return envCloned;
            }
        }

        /// <summary>
        /// 无线电台站发射频率数据结构
        /// </summary>
        [Serializable]
        public class RadioStationFreq
        {
            public double freqPoint;
            public double freqWidth;
            public float fDistance;
            public bool bPowerVerify = false;
            public bool bFreqVerify = false;
            public string strName = "";
            public string strOrganization = "";
            public string strAreaCode = "";
            public bool bMobile = false;
            public string strGUID;
            public string strSVN;//业务类型
            public string strTS;//技术体制

            public string m_strAPP_CODE;//申请表号

            public string m_strSTAT_ADDR;//台站地址
            public string m_strSTAT_APP_TYPE;//资料表类别
            public string m_strSTAT_TDI;//资料表号

            public double fLongi;
            public double fLati;
            /////发射相关参数
            public double fPower;//单位w
            public double fAntennaGain;//单位dBi
            public double fAntennaHeight;//m
            public double fFeederLoss;//dB
            /// <summary>
            /// 输出台站核查结果
            /// </summary>
            /// <returns>结果描述</returns>
            public string GetStationInfo()
            {
                string strCombinedName = strName;
                if (strCombinedName.Length < 6)
                    strCombinedName += (" " + strOrganization);
                string strInfo = string.Format("{0:S}(功率{1:S} 频率{2:S} 距离{3:F1}km)", strCombinedName,
                    bPowerVerify ? "超标" : "正常", (bFreqVerify && !bMobile) ? "未使用" : "在用", fDistance);
                if (bMobile)
                    strInfo += " 移动站";
                return strInfo;
            }
            public void CopyBasicInfo(RadioStationFreq rs)
            {
                this.strGUID = rs.strGUID;
                this.strName = rs.strName;
                this.strOrganization = rs.strOrganization;
                this.strSVN = rs.strSVN;
                this.strTS = rs.strTS;
                this.fLati = rs.fLati;
                this.fLongi = rs.fLongi;
                this.m_strAPP_CODE = rs.m_strAPP_CODE;
                this.m_strSTAT_ADDR = rs.m_strSTAT_ADDR;
                this.m_strSTAT_APP_TYPE = rs.m_strSTAT_APP_TYPE;
                this.m_strSTAT_TDI = rs.m_strSTAT_TDI;
                this.strAreaCode = rs.strAreaCode;
                this.fPower = rs.fPower;
                this.fFeederLoss = rs.fFeederLoss;
                this.fAntennaGain = rs.fAntennaGain;
                this.fAntennaHeight = rs.fAntennaHeight;
            }
        }


        [Serializable]
        ///频段列表
        public class CHANNELENVINFO_LIST : List<ChannelEnvInfo>
        {

        }

        [Serializable]
        public class ChannelOccupy
        {
            public double freqPoint = 0;//MHz
            /// <summary>
            /// 各时间点占用度 <=1 单位为百分比
            /// </summary>
            public Single[] listOccupy;
            public Single fOccupyAllTime = 0;
            public ChannelOccupy(double freq, int nOccupyNum)
            {
                freqPoint = freq;
                listOccupy = new float[nOccupyNum];
            }
        }

        [Serializable]
        ////监测日报统计结果
        public class DayReportResult
        {
            /// <summary>
            /// 频率范围 MHz
            /// </summary>
            public double freqLower;
            /// <summary>
            /// 频率范围 MHz
            /// </summary>
            public double freqUpper;
            /// <summary>
            /// 中频带宽、步长 kHz
            /// </summary>
            public double freqStep;
            /// <summary>
            /// 统计门限电平
            /// </summary>
            public double fThreshold = 6;
            /// <summary>
            /// 扫描速度,秒/次
            /// </summary>
            public int nScanSpeed = 2;
            /// <summary>
            /// 测量分辨率，即统计时间片，分钟/统计值
            /// </summary>
            public int nStatisTime = 60;
            /// <summary>
            /// 监测时间范围start
            /// </summary>
            public DateTime timeStart;
            public DateTime timeStop;
            public Single bandDayOccupy = 0;//频段天占用度，占用度<0是无数据
            public int nOccupyNum = 24;//占用度统计时间片个数
            public List<ChannelOccupy> listChannelsOccupy = new List<ChannelOccupy>();//如果频点值是0，表示频段占用度。，占用度<0是无数据
        }

        [Serializable]
        ///频点场强
        public class ChannelStrength
        {
            public double freqPoint = 0;//MHz
            public Single[] listValues;//各时间点场强值   dB(μv)/m
            public Single fStatisValue = -3000;///历史统计场强值 dB(μv)/m
            public int nAmountHours = 0;//监测时长，0 表示没有监测数据
            public ChannelStrength()
            {
            }
            public ChannelStrength(double freq, Single fValue, int hours)
            {
                freqPoint = freq;
                fStatisValue = fValue;
                nAmountHours = hours;
                listValues = null;
            }
            public ChannelStrength(double freq, Single fValue, int hours, int nStatisTimes)
            {
                freqPoint = freq;
                fStatisValue = fValue;
                nAmountHours = hours;
                listValues = new Single[nStatisTimes];
            }
            public bool IsDataExist
            {
                get
                {
                    return nAmountHours > 0;
                }
            }
        }

        [Serializable]
        ////频段背景噪声统计结
        public class BkgNoiseInBand
        {
            public string MSID = "";
            public double freqLower;//频率范围 MHz
            public double freqUpper;//频率范围 MHz
            public double freqStep;//步长 kHz

            public int nTimeSlice = 0;//时域统计分辨率，分钟，0表示不按时域统计
            public int nStatisTimes = 0;//统计时间片个数,0表示不按时域统计

            public Single fBandAveNoise = 0;//平均频段噪声场强值 dB(μv)/m

            //各频点噪声场强值
            public List<ChannelStrength> listChannelsNoise = new List<ChannelStrength>();
        }

        /// <summary>
        /// 多频段背景噪声统计结果
        /// </summary>
        [Serializable]
        public class BkgNoiseInMultiBand
        {
            /// <summary>
            /// 监测站编号
            /// </summary>
            public string MSID = "";
            /// <summary>
            /// 平均噪声值 dB(μv)/m 
            /// </summary>
            public Single fAveNoise = 0;
            /// <summary>
            /// 最大噪声值 dB(μv)/m 
            /// </summary>
            public Single fMaxNoise = 0;
            /// <summary>
            /// 最小噪声值 dB(μv)/m 
            /// </summary>
            public Single fMinNoise = 0;
            /// <summary>
            /// 频段详细列表
            /// </summary>
            public List<BkgNoiseInBand> BkgNoiseInBandList = new List<BkgNoiseInBand>();
        }
}
