using CO_IA.Data;
using CO_IA_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO_IA.Data.Portable;
using EMCS.Types;

namespace CO_IA.InterferenceAnalysis
{
    public class InterferedAnalysis
    {
        ///
        /// 设备频率
        /// </summary>
        private static ComparableFreq[] EquFreqs
        {
            get;
            set;
        }

        /// <summary>
        /// 周围台站频率
        /// </summary>
        private static ComparableFreq[] StationFreqs
        {
            get;
            set;
        }

        /// <summary>
        /// 设备的接收信息
        /// </summary>
        private static Receiver[] EquReceivers
        {
            get;
            set;
        }

        /// <summary>
        /// 设备的发射信息
        /// </summary>
        private static Transmitter[] EquTransmitters
        {
            get;
            set;
        }

        /// <summary>
        /// 周围台站的发射信息
        /// </summary>
        private static Transmitter[] StationTransmitters
        {
            get;
            set;
        }

        /// <summary>
        /// 周围台站的接收信息
        /// </summary>
        private static Receiver[] StationReceivers
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equs">设备集合</param>
        /// <param name="aroundstation">周围台站集合</param>
        /// <param name="analysisType">非法信息</param>
        /// <returns></returns>
        public static InterferenceAnalysisResult Calculator(List<ActivityEquipment> equs, List<ActivitySurroundStation> aroundstation,
            AnalysisType analysisType)
        {
            ConvertToCalcCondition(equs, aroundstation);

            InterferenceAnalysisResult Interfresult = new InterferenceAnalysisResult();
            if ((analysisType & AnalysisType.SameFreq) == AnalysisType.SameFreq)
            {
                //同频干扰计算
                SameFreqCalculator samefreqCalc = new SameFreqCalculator(EquFreqs, StationFreqs);
                SameFreqCompareResult[] sameInterfResult = samefreqCalc.CalcSameFreqInterference();
                if (sameInterfResult != null && sameInterfResult.Length > 0)
                {
                    Interfresult.SameFreqInterfResult = sameInterfResult;
                    Interfresult.SameFreqInterfResultCount = sameInterfResult.Length;
                }
            }

            if ((analysisType & AnalysisType.ADJFreq) == AnalysisType.ADJFreq)
            {
                //邻频干扰计算
                AdjFreqCalculator adjfreqCalc = new AdjFreqCalculator(EquFreqs, StationFreqs);
                AdjFreqCompareResult[] adjInterfResult = adjfreqCalc.CalcAdjFreqInterference();
                if (adjInterfResult != null && adjInterfResult.Length > 0)
                {
                    Interfresult.ADJFreqInterfResult = adjInterfResult;
                    Interfresult.ADJFreqInterfResultCount = adjInterfResult.Length;
                }
            }

            if ((analysisType & AnalysisType.IM) == AnalysisType.IM)
            {
                //互调干扰计算
                IMOrder order = IMOrder.Second | IMOrder.Third | IMOrder.Fifth;
                IMAnalysisResult imresult = new IMAnalysisResult();
                IMCalculator calculator = new IMCalculator(EquTransmitters, EquReceivers, StationTransmitters, StationReceivers);
                imresult.SetReceiverImResult(calculator.CalcReceiverIMInterference(order));
                //imresult.SetTransmitterImResult(calculator.CalcTransmitterIMInterference());
                if (imresult != null)
                {
                    Interfresult.IMInterfResult = imresult;
                    int transcount = 0;
                    int receivcount = 0;
                    if (imresult.GetReceiverImResult() != null && imresult.GetReceiverImResult().Values.Length > 0)
                    {
                        receivcount = imresult.GetReceiverImResult().Values.Length;
                    }
                    //if (imresult.GetTransmitterImResult() != null && imresult.GetTransmitterImResult().Values.Length > 0)
                    //{
                    //    transcount = imresult.GetTransmitterImResult().Values.Length;
                    //}
                    Interfresult.IMInterfResultCount = transcount + receivcount;
                }
            }
            return Interfresult;
        }

        private static void ConvertToCalcCondition(List<ActivityEquipment> equs, List<ActivitySurroundStation> aroundstation)
        {
            EquFreqs = GetEquipmentFreqs(equs);
            List<ComparableFreq> stationfreqs = GetStationFreqs(aroundstation).ToList();
            stationfreqs.AddRange(EquFreqs);//设备与设备之间也需要进行干扰计算

            StationFreqs = stationfreqs.ToArray();
            EquTransmitters = GetEquTransmitters(equs); //包含备用频率发射信息
            EquReceivers = GetEquReceivers(equs);

            StationTransmitters = GetStationTransmitters(aroundstation);
            StationReceivers = GetStationReceivers(aroundstation);
        }

        /// <summary>
        /// 取设备的频率，转成ComparableFreq
        /// </summary>
        /// <param name="equs"></param>
        /// <returns></returns>
        private static ComparableFreq[] GetEquipmentFreqs(List<ActivityEquipment> equs)
        {
            List<ComparableFreq> freqs = new List<ComparableFreq>();
            if (equs != null && equs.Count > 0)
            {
                foreach (ActivityEquipment equ in equs)
                {
                    double assignFreq = 0;
                    if (equ.AssignSendFreq != null)
                    {
                        assignFreq = equ.AssignSendFreq.Value;
                    }

                    double sparefreq = 0;
                    if (equ.AssignSpareFreq != null)
                    {
                        sparefreq = equ.AssignSpareFreq.Value;
                    }

                    double band = 0;
                    if (equ.Band_kHz != null)
                    {
                        band = equ.Band_kHz.Value;
                    }
                    freqs.Add(new ComparableFreq(assignFreq, sparefreq, band / 1000, equ.Key));
                }
            }
            return freqs.ToArray();
        }

        /// <summary>
        /// 取台站的频率，转成ComparableFreq
        /// </summary>
        /// <param name="aroundstations"></param>
        /// <returns></returns>
        private static ComparableFreq[] GetStationFreqs(List<ActivitySurroundStation> stations)
        {
            List<ComparableFreq> freqs = new List<ComparableFreq>();
            if (stations != null)
            {
                foreach (ActivitySurroundStation station in stations)
                {
                    if (station.EmitInfo != null)
                    {
                        foreach (StationEmitInfo freqem in station.EmitInfo)
                        {
                            //只计算频点干扰,不计算频段干扰
                            if (freqem.FreqType == FreqType.频点)
                            {
                                double freqEC = freqem.FreqEC.Value;
                                double band = freqem.FreqBand;
                                ComparableFreq freq = new ComparableFreq(freqEC, 0, band, station.STATGUID);

                                if (!freqs.Contains(freq))
                                {
                                    freqs.Add(freq);
                                }
                            }
                        }
                    }

                }
            }
            return freqs.ToArray();
        }

        /// <summary>
        /// 取设备的Transmitter(包含备用频率发射信息)
        /// </summary>
        /// <param name="equs"></param>
        /// <returns></returns>
        private static Transmitter[] GetEquTransmitters(List<ActivityEquipment> equs)
        {
            List<Transmitter> transList = new List<Transmitter>();

            if (equs != null && equs.Count > 0)
            {
                ActivityEquipment equ = null;
                for (int i = 0; i < equs.Count; i++)
                {
                    equ = equs[i];

                    double freqsend = 0;
                    if (equ.AssignSendFreq != null)
                    {
                        freqsend = equ.AssignSendFreq.Value;
                    }


                    double band = 0;
                    if (equ.Band_kHz != null)
                    {
                        band = equ.Band_kHz.Value;
                    }

                    double sendFreqStart = 0;
                    if (equ.FreqRange.Little != null)
                    {
                        sendFreqStart = equ.FreqRange.Little.Value;
                    }

                    double sendFreqEnd = 0;
                    if (equ.FreqRange.Great != null)
                    {
                        sendFreqEnd = equ.FreqRange.Great.Value;
                    }

                    double maxPower = 0;
                    if (equ.Power_W != null)
                    {
                        maxPower = equ.Power_W.Value;
                    }

                    Transmitter transmitter = new Transmitter(equ.Key,
                        new ComparableFreq(freqsend, 0, band / 1000, equ.Key),
                        new TransmitterParams()
                        {
                            TuningRangeStart = new EMCFreqValue(EMCFreqUnitEnum.MHZ, sendFreqStart),
                            TuningRangeEnd = new EMCFreqValue(EMCFreqUnitEnum.MHZ, sendFreqEnd),
                            Power = new EMCPowerValue(EMCPowerUnitEnum.W, maxPower)
                        }, null);
                    transList.Add(transmitter);


                    //备用发射频率也作为一条发射
                    double sparefreq = 0;
                    if (equ.AssignSpareFreq != null)
                    {
                        sparefreq = equ.AssignSpareFreq.Value;
                        Transmitter sparetransmitter = new Transmitter(equ.Key,
                        new ComparableFreq(sparefreq, 0, band / 1000, equ.Key),
                        new TransmitterParams()
                        {
                            TuningRangeStart = new EMCFreqValue(EMCFreqUnitEnum.MHZ, sendFreqStart),
                            TuningRangeEnd = new EMCFreqValue(EMCFreqUnitEnum.MHZ, sendFreqEnd),
                            Power = new EMCPowerValue(EMCPowerUnitEnum.W, maxPower)
                        }, null);
                        transList.Add(sparetransmitter);
                    }
                }
            }
            return transList.ToArray();
        }

        /// <summary>
        /// 取台站的Transmitter
        /// </summary>
        /// <param name="aroundstations"></param>
        /// <returns></returns>
        private static Transmitter[] GetStationTransmitters(List<ActivitySurroundStation> aroundstations)
        {
            List<Transmitter> transList = new List<Transmitter>();
            if (aroundstations != null && aroundstations.Count > 0)
            {
                foreach (ActivitySurroundStation station in aroundstations)
                {
                    if (station.EmitInfo != null)
                    {
                        foreach (StationEmitInfo freqem in station.EmitInfo)
                        {
                            if (freqem.FreqType == FreqType.频点)
                            {
                                double freqec = freqem.FreqEC.Value;
                                double band = freqem.FreqBand;

                                //排除同一个台站，多个相同发射频率的问题   不比对功率（r.Erp == freqem.EquPow）
                                int count = transList.Count(r => r.EquipID == station.STATGUID &&
                                    r.ComparableFreq.Freq == freqec &&
                                    r.ComparableFreq.Band == band);

                                if (count == 0)
                                {
                                    Transmitter transmitter = new Transmitter(station.STATGUID,
                                        new ComparableFreq(freqec, 0, band / 1000, station.STATGUID),
                                        new TransmitterParams()
                                        {
                                            Power = new EMCPowerValue(EMCPowerUnitEnum.W, freqem.EquPow)
                                        }, null);
                                    transList.Add(transmitter);
                                }
                            }
                        }
                    }
                }
            }
            return transList.ToArray();
        }

        /// <summary>
        /// 取设备的Receiver
        /// </summary>
        /// <param name="equs"></param>
        /// <returns></returns>
        private static Receiver[] GetEquReceivers(List<ActivityEquipment> equs)
        {
            List<Receiver> receiverList = new List<Receiver>();

            if (equs != null && equs.Count > 0)
            {
                ActivityEquipment equ = null;
                for (int i = 0; i < equs.Count; i++)
                {
                    equ = equs[i];
                    if (equ.ReceiveFreq == null)
                        break;

                    double recvFreq = 0;
                    if (equ.ReceiveFreq != null)
                    {
                        recvFreq = equ.ReceiveFreq.Value;
                    }
                    double band = 0;
                    if (equ.Band_kHz != null)
                    {
                        band = equ.Band_kHz.Value;
                    }

                    double spareFreq = 0;
                    if (equ.SpareFreq != null)
                    {
                        spareFreq = equ.SpareFreq.Value;
                    }

                    Receiver receiv = new Receiver(equ.Key,
                        new ComparableFreq(recvFreq, spareFreq, band, equ.Key),
                        new ReceiverParams() { IFBand = new EMCFreqValue(EMCFreqUnitEnum.KHz, (double)(band / 1000)) },
                        null);

                    receiverList.Add(receiv);
                }
            }
            return receiverList.ToArray(); ;
        }

        /// <summary>
        /// 取台站的Receiver
        /// </summary>
        /// <param name="aroundstations"></param>
        /// <returns></returns>
        private static Receiver[] GetStationReceivers(List<ActivitySurroundStation> aroundstations)
        {
            List<Receiver> receiverList = new List<Receiver>();
            if (aroundstations != null && aroundstations.Count > 0)
            {
                foreach (ActivitySurroundStation station in aroundstations)
                {
                    if (station.EmitInfo != null)
                    {
                        foreach (StationEmitInfo freqem in station.EmitInfo)
                        {
                            if (freqem.FreqEC > 0 && freqem.FreqBand > 0)
                            {
                                //排除同一个台站，多个相同发射频率的问题
                                int count = receiverList.Count(r => r.EquipID == station.STATGUID &&
                                    r.ComparableFreq.Freq == freqem.FreqRC &&
                                    r.ComparableFreq.Band == freqem.FreqBand);
                                if (count == 0)
                                {
                                    Receiver receiver = new Receiver(station.STATGUID,
                                        new ComparableFreq((double)freqem.FreqRC, 0, (double)(freqem.FreqBand / 1000), station.STATGUID),
                                        null, null);
                                    receiverList.Add(receiver);
                                }
                            }
                        }
                    }
                }
            }
            return receiverList.ToArray();
        }
    }
}
