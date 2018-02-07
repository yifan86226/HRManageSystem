//#region 文件描述
///**********************************************************************************
// * 创建人：Niext
// * 摘  要：干扰分析计算类
// * 日  期：2016-09-09
// * ********************************************************************************/
//#endregion
//using CO_IA.Data;
//using CO_IA.Data.Collection;
//using CO_IA_Data;
//using EMCS.Types;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace CO_IA.InterferenceAnalysis
//{
//    public class InterfAnalysis
//    {
//        /// <summary>
//        /// 设备频率
//        /// </summary>
//        private static ComparableFreq[] EquFreqs
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 周围台站频率
//        /// </summary>
//        private static ComparableFreq[] StationFreqs
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 设备的接收信息
//        /// </summary>
//        private static Receiver[] EquReceivers
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 设备的发射信息
//        /// </summary>
//        private static Transmitter[] EquTransmitters
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 周围台站的发射信息
//        /// </summary>
//        private static Transmitter[] StationTransmitters
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 周围台站的接收信息
//        /// </summary>
//        private static Receiver[] StationReceivers
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="equs">设备集合</param>
//        /// <param name="aroundstation">周围台站集合</param>
//        /// <param name="analysisType">非法信息</param>
//        /// <returns></returns>
//        public static InterferenceAnalysisResult Calculator(List<ActivityEquipmentInfo> equs, List<RoundStationInfo> aroundstation,
//            AnalysisType analysisType, List<AnalysisResult> p_IllegalSignal = null)
//        {
//            ConvertToCalcCondition(equs, aroundstation, p_IllegalSignal);

//            InterferenceAnalysisResult Interfresult = new InterferenceAnalysisResult();
//            if ((analysisType & AnalysisType.SameFreq) == AnalysisType.SameFreq)
//            {
//                //同频干扰计算
//                SameFreqCalculator samefreqCalc = new SameFreqCalculator(EquFreqs, StationFreqs);
//                SameFreqCompareResult[] sameInterfResult = samefreqCalc.CalcSameFreqInterference();
//                if (sameInterfResult != null && sameInterfResult.Length > 0)
//                {
//                    Interfresult.SameFreqInterfResult = sameInterfResult;
//                    Interfresult.SameFreqInterfResultCount = sameInterfResult.Length;
//                }
//            }

//            if ((analysisType & AnalysisType.ADJFreq) == AnalysisType.ADJFreq)
//            {
//                //邻频干扰计算
//                AdjFreqCalculator adjfreqCalc = new AdjFreqCalculator(EquFreqs, StationFreqs);
//                AdjFreqCompareResult[] adjInterfResult = adjfreqCalc.CalcAdjFreqInterference();
//                if (adjInterfResult != null && adjInterfResult.Length > 0)
//                {
//                    Interfresult.ADJFreqInterfResult = adjInterfResult;
//                    Interfresult.ADJFreqInterfResultCount = adjInterfResult.Length;
//                }
//            }

//            if ((analysisType & AnalysisType.IM) == AnalysisType.IM)
//            {
//                //互调干扰计算
//                IMOrder order = IMOrder.Second | IMOrder.Third | IMOrder.Fifth;
//                IMAnalysisResult imresult = new IMAnalysisResult();
//                IMCalculator calculator = new IMCalculator(EquTransmitters, EquReceivers, StationTransmitters, StationReceivers);
//                imresult.SetReceiverImResult(calculator.CalcReceiverIMInterference(order));
//                //imresult.SetTransmitterImResult(calculator.CalcTransmitterIMInterference());
//                if (imresult != null)
//                {
//                    Interfresult.IMInterfResult = imresult;
//                    int transcount = 0;
//                    int receivcount = 0;
//                    if (imresult.GetReceiverImResult() != null && imresult.GetReceiverImResult().Values.Length > 0)
//                    {
//                        receivcount = imresult.GetReceiverImResult().Values.Length;
//                    }
//                    //if (imresult.GetTransmitterImResult() != null && imresult.GetTransmitterImResult().Values.Length > 0)
//                    //{
//                    //    transcount = imresult.GetTransmitterImResult().Values.Length;
//                    //}
//                    Interfresult.IMInterfResultCount = transcount + receivcount;
//                }
//            }
//            return Interfresult;
//        }

//        private static void ConvertToCalcCondition(List<ActivityEquipmentInfo> equs, List<RoundStationInfo> aroundstation, List<AnalysisResult> p_IllegalSignal = null)
//        {
//            EquFreqs = GetEquipmentFreqs(equs);
//            List<ComparableFreq> stationfreqs = GetStationFreqs(aroundstation).ToList();
//            stationfreqs.AddRange(EquFreqs);//设备与设备之间也需要进行干扰计算

//            //非法信息，计算邻频干扰和同频干扰
//            if (p_IllegalSignal != null)
//            {
//                ComparableFreq[] illegalsignal = GetIllegalSignal(p_IllegalSignal);
//                stationfreqs.AddRange(illegalsignal);
//            }

//            StationFreqs = stationfreqs.ToArray();

//            EquTransmitters = GetEquTransmitters(equs);
//            EquReceivers = GetEquReceivers(equs);

//            StationTransmitters = GetStationTransmitters(aroundstation);
//            StationReceivers = GetStationReceivers(aroundstation);
//        }

//        /// <summary>
//        /// 取设备的频率，转成ComparableFreq
//        /// </summary>
//        /// <param name="equs"></param>
//        /// <returns></returns>
//        private static ComparableFreq[] GetEquipmentFreqs(List<ActivityEquipmentInfo> equs)
//        {
//            List<ComparableFreq> freqs = new List<ComparableFreq>();
//            if (equs != null && equs.Count > 0)
//            {
//                foreach (ActivityEquipmentInfo equ in equs)
//                {
//                    double assignFreq = 0;
//                    if (equ.AssignFreq != null)
//                    {
//                        assignFreq = equ.AssignFreq.Value;
//                    }

//                    double band = 0;
//                    if (equ.Band != null)
//                    {
//                        band = equ.Band.Value;
//                    }
                   
//                    freqs.Add(new ComparableFreq(assignFreq,0, band / 1000, equ.GUID));
//                }
//            }
//            return freqs.ToArray();
//        }

//        /// <summary>
//        /// 取台站的频率，转成ComparableFreq
//        /// </summary>
//        /// <param name="aroundstations"></param>
//        /// <returns></returns>
//        private static ComparableFreq[] GetStationFreqs(List<RoundStationInfo> stations)
//        {
//            List<ComparableFreq> freqs = new List<ComparableFreq>();
//            if (stations != null)
//            {
//                foreach (RoundStationInfo station in stations)
//                {
//                    if (station.EmitInfos != null)
//                    {
//                        foreach (FreqEmitInfo freqem in station.EmitInfos)
//                        {
//                            double freqEC = freqem.FreqEC;
//                            double band = freqem.FreqBand;
//                            ComparableFreq freq = new ComparableFreq(freqEC,0, band / 1000, station.STATGUID);

//                            if (!freqs.Contains(freq))
//                            {
//                                freqs.Add(freq);
//                            }
//                        }
//                    }

//                }
//            }
//            return freqs.ToArray();
//        }

//        /// <summary>
//        /// 取设备的Transmitter
//        /// </summary>
//        /// <param name="equs"></param>
//        /// <returns></returns>
//        private static Transmitter[] GetEquTransmitters(List<ActivityEquipmentInfo> equs)
//        {
//            List<Transmitter> transList = new List<Transmitter>();

//            if (equs != null && equs.Count > 0)
//            {
//                ActivityEquipmentInfo equ = null;
//                for (int i = 0; i < equs.Count; i++)
//                {
//                    equ = equs[i];

//                    double freqsend = 0;
//                    if (equ.SendFreq != null)
//                    {
//                        freqsend = equ.SendFreq.Value;
//                    }

//                    double band = 0;
//                    if (equ.Band != null)
//                    {
//                        band = equ.Band.Value;
//                    }

//                    double sendFreqStart = 0;
//                    if (equ.SendFreqStart != null)
//                    {
//                        sendFreqStart = equ.SendFreqStart.Value;
//                    }

//                    double sendFreqEnd = 0;
//                    if (equ.SendFreqEnd != null)
//                    {
//                        sendFreqEnd = equ.SendFreqEnd.Value;
//                    }

//                    double maxPower = 0;
//                    if (equ.MaxPower != null)
//                    {
//                        maxPower = equ.MaxPower.Value;
//                    }

//                    Transmitter transmitter = new Transmitter(equ.GUID,
//                        new ComparableFreq(freqsend, 0, band / 1000, equ.GUID),
//                        new TransmitterParams()
//                        {
//                            TuningRangeStart = new EMCFreqValue(EMCFreqUnitEnum.MHZ, sendFreqStart),
//                            TuningRangeEnd = new EMCFreqValue(EMCFreqUnitEnum.MHZ, sendFreqEnd),
//                            Power = new EMCPowerValue(EMCPowerUnitEnum.W, maxPower)
//                        }, null);
//                    transList.Add(transmitter);

//                }
//            }
//            return transList.ToArray();
//        }

//        /// <summary>
//        /// 取台站的Transmitter
//        /// </summary>
//        /// <param name="aroundstations"></param>
//        /// <returns></returns>
//        private static Transmitter[] GetStationTransmitters(List<RoundStationInfo> aroundstations)
//        {
//            List<Transmitter> transList = new List<Transmitter>();
//            if (aroundstations != null && aroundstations.Count > 0)
//            {
//                foreach (RoundStationInfo station in aroundstations)
//                {
//                    if (station.EmitInfos != null)
//                    {
//                        foreach (FreqEmitInfo freqem in station.EmitInfos)
//                        {
//                            double freqec = freqem.FreqEC;
//                            double band = freqem.FreqBand;

//                            //排除同一个台站，多个相同发射频率的问题   不比对功率（r.Erp == freqem.EquPow）
//                            int count = transList.Count(r => r.EquipID == station.STATGUID &&
//                                r.ComparableFreq.Freq == freqec &&
//                                r.ComparableFreq.Band == band);

//                            if (count == 0)
//                            {
//                                Transmitter transmitter = new Transmitter(station.STATGUID,
//                                    new ComparableFreq(freqec, 0, band / 1000, station.STATGUID),
//                                    new TransmitterParams()
//                                    {
//                                        Power = new EMCPowerValue(EMCPowerUnitEnum.W, freqem.EquPow)
//                                    }, null);
//                                transList.Add(transmitter);
//                            }
//                        }
//                    }
//                }
//            }
//            return transList.ToArray();
//        }

//        /// <summary>
//        /// 取设备的Receiver
//        /// </summary>
//        /// <param name="equs"></param>
//        /// <returns></returns>
//        private static Receiver[] GetEquReceivers(List<ActivityEquipmentInfo> equs)
//        {
//            List<Receiver> receiverList = new List<Receiver>();

//            if (equs != null && equs.Count > 0)
//            {
//                ActivityEquipmentInfo equ = null;
//                for (int i = 0; i < equs.Count; i++)
//                {
//                    equ = equs[i];

//                    double recvFreq = 0;
//                    if (equ.ReceiveFreq != null)
//                    {
//                        recvFreq = equ.ReceiveFreq.Value;
//                    }

//                    double band = 0;
//                    if (equ.Band != null)
//                    {
//                        band = equ.Band.Value;
//                    }

//                    double recvFreqStart = 0;
//                    if (equ.RecvFreqStart != null)
//                    {
//                        recvFreqStart = equ.RecvFreqStart.Value;
//                    }

//                    double recvFreqEnd = 0;
//                    if (equ.RecvFreqEnd != null)
//                    {
//                        recvFreqEnd = equ.RecvFreqEnd.Value;
//                    }

//                    double sensitivity = 0;
//                    if (equ.Sensitivity != null)
//                    {
//                        sensitivity = equ.Sensitivity.Value;
//                    }

//                    SensitivityUnitEnum sensitivityUnit = SensitivityUnitEnum.None;
//                    if (equ.SensitivityUnit != null)
//                    {
//                        sensitivityUnit = (SensitivityUnitEnum)equ.SensitivityUnit;
//                    }
//                    Receiver receiver = new Receiver(equ.GUID,
//                        new ComparableFreq(recvFreq, 0, (double)(band / 1000), equ.GUID),
//                        new ReceiverParams()
//                        {
//                            Sensitivity = sensitivity,
//                            SensitivityUnit = sensitivityUnit,
//                            TuningRangeStart = new EMCFreqValue(EMCFreqUnitEnum.MHZ, recvFreqStart),
//                            TuningRangeEnd = new EMCFreqValue(EMCFreqUnitEnum.MHZ, recvFreqEnd),
//                            IFBand = new EMCFreqValue(EMCFreqUnitEnum.KHz, (double)(band / 1000))
//                        }, null);
//                    receiverList.Add(receiver);
//                }
//            }
//            return receiverList.ToArray(); ;
//        }

//        /// <summary>
//        /// 取台站的Receiver
//        /// </summary>
//        /// <param name="aroundstations"></param>
//        /// <returns></returns>
//        private static Receiver[] GetStationReceivers(List<RoundStationInfo> aroundstations)
//        {
//            List<Receiver> receiverList = new List<Receiver>();
//            if (aroundstations != null && aroundstations.Count > 0)
//            {
//                foreach (RoundStationInfo station in aroundstations)
//                {
//                    if (station.EmitInfos != null)
//                    {
//                        foreach (FreqEmitInfo freqem in station.EmitInfos)
//                        {
//                            if (freqem.FreqEC > 0 && freqem.FreqBand > 0)
//                            {
//                                //排除同一个台站，多个相同发射频率的问题
//                                int count = receiverList.Count(r => r.EquipID == station.STATGUID &&
//                                    r.ComparableFreq.Freq == freqem.FreqRC &&
//                                    r.ComparableFreq.Band == freqem.FreqBand);
//                                if (count == 0)
//                                {
//                                    Receiver receiver = new Receiver(station.STATGUID,
//                                        new ComparableFreq((double)freqem.FreqRC, 0, (double)(freqem.FreqBand / 1000), station.STATGUID),
//                                        null, null);
//                                    receiverList.Add(receiver);
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            return receiverList.ToArray();

//            //for (int i = 0; i < aroundstations.Count; i++)
//            //{
//            //    aroundstation = aroundstations[i];

//            //    if (aroundstation.SendFreq != null && aroundstation.Band != null)
//            //    {
//            //        double recvFreqStart = 0;
//            //        if (aroundstation.RecvFreqStart != null)
//            //        {
//            //            recvFreqStart = (double)aroundstation.RecvFreqStart;
//            //        }
//            //        double recvFreqEnd = 0;
//            //        if (aroundstation.RecvFreqEnd != null)
//            //        {
//            //            recvFreqEnd = (double)aroundstation.RecvFreqEnd;
//            //        }
//            //        double sensitivity = 0;
//            //        if (aroundstation.Sensitivity != null)
//            //        {
//            //            sensitivity = (double)aroundstation.Sensitivity;
//            //        }
//            //        SensitivityUnitEnum sensitivityUnit = SensitivityUnitEnum.None;
//            //        if (aroundstation.SensitivityUnit != null)
//            //        {
//            //            sensitivityUnit = (SensitivityUnitEnum)aroundstation.SensitivityUnit;
//            //        }
//            //        Receiver receiver = new Receiver(aroundstation.GUID,
//            //            new ComparableFreq((double)aroundstation.ReceiveFreq, (double)aroundstation.Band, aroundstation.GUID),
//            //            new ReceiverParams()
//            //            {
//            //                TuningRangeStart = new EMCFreqValue(EMCFreqUnitEnum.MHZ, recvFreqStart),
//            //                TuningRangeEnd = new EMCFreqValue(EMCFreqUnitEnum.MHZ, recvFreqEnd),
//            //                IFBand = new EMCFreqValue(EMCFreqUnitEnum.MHZ, (double)aroundstation.Band),
//            //                Sensitivity = sensitivity,
//            //                SensitivityUnit = sensitivityUnit
//            //            }, null);
//            //        receiverList.Add(receiver);
//            //    }
//            //}
//            //receivers = receiverList.ToArray();

//        }

//        /// <summary>
//        ///
//        /// </summary>
//        /// <returns></returns>
//        private static ComparableFreq[] GetIllegalSignal(List<AnalysisResult> p_IllegalSignal)
//        {
//            ComparableFreq[] freqs = new ComparableFreq[p_IllegalSignal.Count] ;

//            for (int i = 0; i < p_IllegalSignal.Count; i++)
//            {
//                AnalysisResult result = p_IllegalSignal[i];
//                freqs[i] = new ComparableFreq(result.Frequency,0, result.BandWidth / 1000, result.Id) { };
//            }

//            return freqs;
//        }

//    }
//}
