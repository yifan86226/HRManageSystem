#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：互调干扰计算
 * 日  期：2016-09-07
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{/// <summary>
    /// 互调计算器
    /// </summary>
    public class IMCalculator
    {
        /// <summary>
        /// 参与计算的发射机
        /// </summary>
        private Transmitter[] stationtransmitters;

        /// <summary>
        /// 参与计算的接收机
        /// </summary>
        private Receiver[] stationreceivers;

        /// <summary>
        /// 用于计算的发射机
        /// </summary>
        private Transmitter[] equtransmitters;

        /// <summary>
        /// 用于计算的接收机
        /// </summary>
        private Receiver[] equreceivers;

        /// <summary>
        /// 当前计算的接收机
        /// </summary>
        private Receiver currentReceiver;

        /// <summary>
        /// 互调计算阶数
        /// </summary>
        private IMOrder imOrder = IMOrder.None;

        /// <summary>
        /// 发射机遍历结束位置
        /// </summary>
        private int endPosition;

        /// <summary>
        /// 计算接收机互调的方法
        /// </summary>
        private DisposeTransmitter CalculateReceiverIM;

        /// <summary>
        /// 互调结果
        /// </summary>
        private IMCompareResult imResult = new IMCompareResult(new Dictionary<ComparableFreq, List<ComparableFreq>>());

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stationtransmitters">参与计算的发射机</param>
        /// <param name="stationreceivers">参与计算的接收机</param>
        public IMCalculator(Transmitter[] equtransmitters, Receiver[] equreceivers, Transmitter[] stationtransmitters, Receiver[] stationreceivers)
        {
            this.equreceivers = equreceivers;
            this.equtransmitters = equtransmitters;
            this.stationtransmitters = stationtransmitters;
            this.stationreceivers = stationreceivers;

            Array.Sort(this.stationtransmitters);
            Array.Sort(this.stationreceivers);
            Array.Sort(this.equtransmitters);
            Array.Sort(this.equreceivers);
        }

        /// <summary>
        /// 初始化互调阶数
        /// </summary>
        /// <param name="order">互调阶数</param>
        private void InitializeImOrder(IMOrder order)
        {
            if (order != this.imOrder)
            {
                this.CalculateReceiverIM -= CalcSecondOrderIM;
                this.CalculateReceiverIM -= this.CalcThirdOrderIM;
                this.CalculateReceiverIM -= CalcFifthOrderIM;

                if ((order & IMOrder.Second) == IMOrder.Second)
                {
                    //this.CalculateReceiverIM += CalcSecondOrderIM;
                }
                if ((order & IMOrder.Third) == IMOrder.Third)
                {
                    this.CalculateReceiverIM += this.CalcThirdOrderIM;
                }
                if ((order & IMOrder.Fifth) == IMOrder.Fifth)
                {
                    this.CalculateReceiverIM += CalcFifthOrderIM;
                }
                this.imOrder = order;
            }
        }


        /// <summary>
        /// 计算接收机互调  
        /// </summary>
        internal IMCompareResult ReceiverIM(IMOrder order)
        {
            this.imResult = new IMCompareResult();
            this.InitializeImOrder(order);
            foreach (Receiver receiver in this.stationreceivers)
            {
                this.currentReceiver = receiver;
                int start = BiSearchTransmitter(receiver.TuningFreqFrom, 0, stationtransmitters.Length - 1);
                int end = BiSearchTransmitter(receiver.TuningRangeTo, start, stationtransmitters.Length - 1);
                this.endPosition = end;
                for (int i = start; i < end; i++)
                {
                    for (int j = i + 1; j <= end; j++)
                    {
                        if (this.currentReceiver.EquipID != this.stationtransmitters[i].EquipID
                            && this.currentReceiver.EquipID != this.stationtransmitters[j].EquipID)
                        {
                            CalculateReceiverIM(stationtransmitters[i], j);
                        }
                    }
                }
            }
            return imResult;
        }

        /// <summary>
        ///  计算接收机互调  
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public IMCompareResult CalcReceiverIMInterference(IMOrder order)
        {
            this.imResult = new IMCompareResult();
            //this.InitializeImOrder(order);

            //遍历设备接收参数
            foreach (Receiver receiver in this.equreceivers)
            {
                this.currentReceiver = receiver;
                int equstart = 0;
                int equend = equtransmitters.Length - 1;
                this.endPosition = equend;
                for (int e = equstart; e < equend; e++)
                {
                    for (int f = e + 1; f <= equend; f++)
                    {
                        if (this.currentReceiver.EquipID != this.equtransmitters[e].EquipID
                            && this.currentReceiver.EquipID != this.equtransmitters[f].EquipID)
                        {
                            //CalculateReceiverIM(equtransmitters[e], f);

                            if ((order & IMOrder.Second) == IMOrder.Second)
                            {
                                CalcEquSecondOrderIM(equtransmitters[e], f);
                            }
                            if ((order & IMOrder.Third) == IMOrder.Third)
                            {
                                CalcEquThirdOrderIM(equtransmitters[e], f);
                            }
                            if ((order & IMOrder.Fifth) == IMOrder.Fifth)
                            {
                                CalcEquFifthOrderIM(equtransmitters[e], f);
                            }
                        }
                    }
                }

                int stationstart = 0;
                int stationend = stationtransmitters.Length - 1;
                this.endPosition = stationend;
                for (int s = stationstart; s < stationend; s++)
                {
                    for (int k = s + 1; k <= stationend; k++)
                    {
                        if (this.currentReceiver.EquipID != this.stationtransmitters[s].EquipID
                            && this.currentReceiver.EquipID != this.stationtransmitters[k].EquipID)
                        {
                            //CalculateReceiverIM(equtransmitters[s], k);

                            if ((order & IMOrder.Second) == IMOrder.Second)
                            {
                                CalcStationSecondOrderIM(stationtransmitters[s], k);
                            }
                            if ((order & IMOrder.Third) == IMOrder.Third)
                            {
                                CalcStationThirdOrderIM(stationtransmitters[s], k);
                            }
                            if ((order & IMOrder.Fifth) == IMOrder.Fifth)
                            {
                                CalcStationFifthOrderIM(stationtransmitters[s], k);
                            }
                        }
                    }
                }
            }
            return imResult;
        }

        /// <summary>
        /// 计算发射机互调
        /// </summary>
        internal IMCompareResult TransmitterIM()
        {
            this.imResult = new IMCompareResult();
            int length = this.stationtransmitters.Length;
            for (int i = 0; i < length; i++)
            {
                //int start = this.BiSearchTransmitter(this.transmitters[i].TuningFreqFrom, 0, length - 1);
                //int end = this.BiSearchTransmitter(this.transmitters[i].TuningRangeTo, start, length - 1);
                int start = 0;
                int end = length - 1;
                double freqA = 2 * this.stationtransmitters[i].Freq;
                for (int j = start; j <= end; j++)
                {
                    if (i == j || this.stationtransmitters[i].EquipID == this.stationtransmitters[j].EquipID)
                    {
                        continue;
                    }
                    double generateFreq = freqA - this.stationtransmitters[j].Freq;
                    foreach (Receiver receiver in this.stationreceivers)
                    {
                        if (receiver.EquipID == this.stationtransmitters[i].EquipID || receiver.EquipID == this.stationtransmitters[j].EquipID)
                            continue;
                        if (receiver.IsValidatingFreq(generateFreq))
                        {
                            double distancef1Toreceiver = DistanceCalculator.GetKmDistance(this.stationtransmitters[i].Coordinate, receiver.Coordinate);
                            //double attenuation = 0;
                            //按照垂直计算AC
                            //double vAttenuation = 37.5 * Math.Log10() + 40.3 * Math.Log10(f2.Freq) - 63.87;
                            //按照水平计算AC
                            double attenuation = 20 * Math.Log10(distancef1Toreceiver * 1000) + 21 * Math.Log10(this.stationtransmitters[j].Freq) - 23.9;

                            //f2在f1发信机输入端的功率;
                            //Pin’(B)=ERPB-AC+GA-Lta
                            //Pin’(B)为TXB在TXA发信机输入端的功率;
                            double powerB = this.stationtransmitters[j].Erp - attenuation + this.stationtransmitters[i].AntGain - this.stationtransmitters[i].FeedLoss;
                            double deviation = Math.Round(Math.Abs(this.stationtransmitters[i].Freq - this.stationtransmitters[j].Freq), 4, MidpointRounding.AwayFromZero);
                            double refC = 57 * Math.Log10(deviation + 1.5) - 9.6 - 9 * Math.Log10(deviation);
                            double lb = GetTransLoss(this.stationtransmitters[i], distancef1Toreceiver);
                            //Pin’(B)+GA-LTA+GRX-LRX-Lb-C
                            double powerIn = powerB + this.stationtransmitters[i].AntGain - this.stationtransmitters[i].FeedLoss + receiver.AntGaindBi - receiver.FeedLoss - lb - refC;
                            if (receiver.IsValidatingSignal(powerIn))
                            {
                                //登记发射机互调
                                ThirdOrderIMItem item = new ThirdOrderIMItem(new ComparableFreq[] { this.stationtransmitters[i].ComparableFreq, this.stationtransmitters[j].ComparableFreq }, new int[] { 2, -1 });
                                item.DisturbedFreqs = new ComparableFreq[] { receiver.ComparableFreq };
                                this.imResult.RegisterIMItem(item);
                            }
                        }
                    }
                }
            }
            return this.imResult;
        }

        /// <summary>
        /// 计算发射机互调
        /// </summary>
        internal IMCompareResult CalcTransmitterIMInterference()
        {
            this.imResult = new IMCompareResult();
            int length = this.stationtransmitters.Length;

            for (int i = 0; i < this.equtransmitters.Length; i++)
            {
                int start = 0;
                int end = length - 1;
                double freqA = 2 * this.equtransmitters[i].Freq;

                for (int j = start; j <= end; j++)
                {
                    if (i == j || this.stationtransmitters[i].EquipID == this.stationtransmitters[j].EquipID)
                    {
                        continue;
                    }
                    double generateFreq = freqA - this.stationtransmitters[j].Freq;
                    foreach (Receiver receiver in this.stationreceivers)
                    {
                        if (receiver.EquipID == this.stationtransmitters[i].EquipID || receiver.EquipID == this.stationtransmitters[j].EquipID)
                            continue;
                        if (receiver.IsValidatingFreq(generateFreq))
                        {
                            double powerIn = CalcPowerIn(this.equtransmitters[i], this.equtransmitters[j], receiver);
                            if (receiver.IsValidatingSignal(powerIn))
                            {
                                //登记发射机互调
                                ThirdOrderIMItem item = new ThirdOrderIMItem(new ComparableFreq[] { this.stationtransmitters[i].ComparableFreq, this.stationtransmitters[j].ComparableFreq }, new int[] { 2, -1 });
                                item.DisturbedFreqs = new ComparableFreq[] { receiver.ComparableFreq };
                                this.imResult.RegisterIMItem(item);
                            }
                        }
                    }
                }
            }
            return this.imResult;



            //this.imResult = new IMCompareResult();
            //int calclength = this.calctransmitters.Length;
            //for (int i = 0; i < calclength; i++)
            //{
            //    #region 在设备中计算互调

            //    int equstart = 0;
            //    int equend = calclength - 1;

            //    double freqA = 2 * this.calctransmitters[i].Freq;

            //    for (int j = equstart; j <= equend; j++)
            //    {
            //        if (i == j || this.calctransmitters[i].EquipID == this.calctransmitters[j].EquipID)
            //        {
            //            continue;
            //        }

            //        double generateFreq = freqA - this.calctransmitters[j].Freq;
            //        foreach (Receiver receiver in this.calcreceivers)
            //        {
            //            if (receiver.EquipID == this.calctransmitters[i].EquipID || receiver.EquipID == this.calctransmitters[j].EquipID)
            //                continue;
            //            if (receiver.IsValidatingFreq(generateFreq))
            //            {
            //                double powerIn = CalcPowerIn(this.calctransmitters[i], this.calctransmitters[j], receiver);
            //                if (receiver.IsValidatingSignal(powerIn))
            //                {
            //                    //登记发射机互调 将计算频率存在IMFreqs中
            //                    ThirdOrderIMItem item = new ThirdOrderIMItem(new ComparableFreq[] { this.calctransmitters[i].ComparableFreq, this.calctransmitters[j].ComparableFreq }, new int[] { 2, -1 });
            //                    item.DisturbedFreqs = new ComparableFreq[] { receiver.ComparableFreq }; //将另一个干扰频率存在DisturbedFreqs中
            //                    this.imResult.RegisterIMItem(item);
            //                }
            //            }
            //        }
            //    }
            //    #endregion

            //    #region 在周围台站中计算互调

            //    int complength = this.comptransmitters.Length;
            //    int statstart = 0;
            //    int statend = complength - 1;
            //    for (int j = statstart; j <= statend; j++)
            //    {
            //        if (i == j || this.calctransmitters[i].EquipID == this.comptransmitters[j].EquipID)
            //        {
            //            continue;
            //        }
            //        double generateFreq = freqA - this.comptransmitters[j].Freq;
            //        foreach (Receiver receiver in this.compreceivers)
            //        {
            //            if (receiver.EquipID == this.calctransmitters[i].EquipID || receiver.EquipID == this.comptransmitters[j].EquipID)
            //                continue;
            //            if (receiver.IsValidatingFreq(generateFreq))
            //            {
            //                double powerIn = CalcPowerIn(this.calctransmitters[i], this.comptransmitters[j], receiver);
            //                if (receiver.IsValidatingSignal(powerIn))
            //                {
            //                    //登记发射机互调 将计算频率存在IMFreqs中
            //                    ThirdOrderIMItem item = new ThirdOrderIMItem(new ComparableFreq[] { this.calctransmitters[i].ComparableFreq, this.comptransmitters[j].ComparableFreq }, new int[] { 2, -1 });
            //                    item.DisturbedFreqs = new ComparableFreq[] { receiver.ComparableFreq }; //将另一个干扰频率存在DisturbedFreqs中
            //                    this.imResult.RegisterIMItem(item);
            //                }
            //            }
            //        }
            //    }

            //    #endregion

            //}
            //return this.imResult;
        }

        private double CalcPowerIn(Transmitter itransmitter, Transmitter jtransmitter, Receiver receiver)
        {
            double distancef1Toreceiver = DistanceCalculator.GetKmDistance(itransmitter.Coordinate, receiver.Coordinate);
            //double attenuation = 0;
            //按照垂直计算AC
            //double vAttenuation = 37.5 * Math.Log10() + 40.3 * Math.Log10(f2.Freq) - 63.87;
            //按照水平计算AC
            double attenuation = 20 * Math.Log10(distancef1Toreceiver * 1000) + 21 * Math.Log10(jtransmitter.Freq) - 23.9;

            //f2在f1发信机输入端的功率;
            //Pin’(B)=ERPB-AC+GA-Lta
            //Pin’(B)为TXB在TXA发信机输入端的功率;
            double powerB = jtransmitter.Erp - attenuation + itransmitter.AntGain - itransmitter.FeedLoss;
            double deviation = Math.Round(Math.Abs(itransmitter.Freq - jtransmitter.Freq), 4, MidpointRounding.AwayFromZero);
            double refC = 57 * Math.Log10(deviation + 1.5) - 9.6 - 9 * Math.Log10(deviation);
            double lb = GetTransLoss(itransmitter, distancef1Toreceiver);
            //Pin’(B)+GA-LTA+GRX-LRX-Lb-C
            double powerIn = powerB + itransmitter.AntGain - itransmitter.FeedLoss + receiver.AntGaindBi - receiver.FeedLoss - lb - refC;
            return powerIn;
        }

        /// <summary>
        /// 测试接收机互调
        /// </summary>
        /// <param name="receiver">被干扰接收机</param>
        /// <param name="order">互调阶数</param>
        /// <returns>互调结果</returns>
        public IMCompareResult TestReceiverIM(Receiver receiver, IMOrder order)
        {
            this.imResult = new IMCompareResult();
            this.InitializeImOrder(order);
            this.currentReceiver = receiver;
            int start = BiSearchTransmitter(receiver.TuningFreqFrom, 0, stationreceivers.Length - 1);
            int end = BiSearchTransmitter(receiver.TuningRangeTo, start, stationreceivers.Length - 1);
            this.endPosition = end;
            for (int i = start; i < end; i++)
            {
                for (int j = i + 1; j <= end; j++)
                {
                    CalculateReceiverIM(this.stationtransmitters[i], j);
                }
            }
            return imResult;
        }

        /// <summary>
        /// 测试接收机互调
        /// </summary>
        /// <param name="transmitter">参与计算的发射机</param>
        /// <param name="order">互调阶数</param>
        /// <returns>接收机互调结果</returns>
        public IMCompareResult TestReceiverIM(Transmitter transmitter, IMOrder order)
        {
            this.imResult = new IMCompareResult();
            this.InitializeImOrder(order);

            foreach (Receiver receiver in this.stationreceivers)
            {
                if (receiver.ReceivableFreq(transmitter.Freq))
                {
                    this.currentReceiver = receiver;
                    int start = BiSearchTransmitter(receiver.TuningFreqFrom, 0, stationreceivers.Length - 1);
                    int end = BiSearchTransmitter(receiver.TuningRangeTo, start, stationreceivers.Length - 1);
                    this.endPosition = end;
                    for (int i = start; i <= end; i++)
                    {
                        CalculateReceiverIM(transmitter, i);
                    }
                }
            }
            return this.imResult;
        }



        /// <summary>
        /// 测试发射机互调
        /// </summary>
        /// <param name="transmitter">要测试的发射机</param>
        /// <returns>发射机互调测试结果</returns>
        public IMCompareResult TestTransmitterIM(Transmitter transmitter)
        {
            this.imResult = new IMCompareResult();
            int length = this.stationtransmitters.Length;
            if (length > 0)
            {
                int start = this.BiSearchTransmitter(transmitter.TuningFreqFrom, 0, length - 1);
                int end = this.BiSearchTransmitter(transmitter.TuningRangeTo, start, length - 1);
                double freqA = 2 * transmitter.Freq;
                for (int j = start; j <= end; j++)
                {
                    double generateFreq = freqA - this.stationtransmitters[j].Freq;
                    foreach (Receiver receiver in this.stationreceivers)
                    {
                        if (receiver.IsValidatingFreq(generateFreq))
                        {
                            double distancef1Toreceiver = DistanceCalculator.GetKmDistance(transmitter.Coordinate, receiver.Coordinate);
                            //double attenuation = 0;
                            //按照垂直计算AC
                            //double vAttenuation = 37.5 * Math.Log10() + 40.3 * Math.Log10(f2.Freq) - 63.87;
                            //按照水平计算AC
                            double attenuation = 20 * Math.Log10(distancef1Toreceiver * 1000) + 21 * Math.Log10(this.stationtransmitters[j].Freq) - 23.9;

                            //f2在f1发信机输入端的功率;
                            //Pin’(B)=ERPB-AC+GA-Lta
                            //Pin’(B)为TXB在TXA发信机输入端的功率;
                            double powerB = this.stationtransmitters[j].Erp - attenuation + transmitter.AntGain - transmitter.FeedLoss;
                            double deviation = Math.Round(Math.Abs(transmitter.Freq - this.stationtransmitters[j].Freq), 4, MidpointRounding.AwayFromZero);
                            double refC = 57 * Math.Log10(deviation + 1.5) - 9.6 - 9 * Math.Log10(deviation);
                            double lb = GetTransLoss(transmitter, distancef1Toreceiver);
                            //Pin’(B)+GA-LTA+GRX-LRX-Lb-C
                            double powerIn = powerB + transmitter.AntGain - transmitter.FeedLoss + receiver.AntGaindBi - receiver.FeedLoss - lb - refC;
                            if (receiver.IsValidatingSignal(powerIn))
                            {
                                //登记发射机互调
                                ThirdOrderIMItem item = new ThirdOrderIMItem(new ComparableFreq[] { transmitter.ComparableFreq, this.stationtransmitters[j].ComparableFreq }, new int[] { 2, -1 });
                                item.DisturbedFreqs = new ComparableFreq[] { receiver.ComparableFreq };
                                this.imResult.RegisterIMItem(item);
                            }
                        }
                    }
                }
            }
            return this.imResult;
        }

        /// <summary>
        /// 计算二阶互调
        /// </summary>
        /// <param name="min">计算互调用较小频率在发射机组中位置</param>
        /// <param name="max">计算互调用较大频率在发射机组中位置</param>
        private void CalcSecondOrderIM(int min, int max)
        {
            if (this.currentReceiver.IsValidatingFreq(this.stationtransmitters[min].Freq + this.stationtransmitters[max].Freq))
            {
                SecondOrderIMItem item = new SecondOrderIMItem(new ComparableFreq[] { this.stationtransmitters[min].ComparableFreq, this.stationtransmitters[max].ComparableFreq }, new int[] { 1, 1 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
            else if (this.currentReceiver.IsValidatingFreq(this.stationtransmitters[max].Freq - this.stationtransmitters[min].Freq))
            {
                SecondOrderIMItem item = new SecondOrderIMItem(new ComparableFreq[] { this.stationtransmitters[max].ComparableFreq, this.stationtransmitters[min].ComparableFreq }, new int[] { 1, -1 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
        }

        /// <summary>
        /// 计算二阶互调
        /// </summary>
        /// <param name="transmitter">计算互调用发射机</param>
        /// <param name="max">计算互调用发射机在发射机组中位置</param>
        private void CalcSecondOrderIM(Transmitter transmitter, int max)
        {
            Transmitter minTransmitter, maxTransmitter;
            if (this.stationtransmitters[max].Freq > transmitter.Freq)
            {
                minTransmitter = transmitter;
                maxTransmitter = stationtransmitters[max];
            }
            else
            {
                minTransmitter = stationtransmitters[max];
                maxTransmitter = transmitter;
            }
            if (this.currentReceiver.IsValidatingFreq(minTransmitter.Freq + maxTransmitter.Freq))
            {
                SecondOrderIMItem item = new SecondOrderIMItem(new ComparableFreq[] { minTransmitter.ComparableFreq, maxTransmitter.ComparableFreq }, new int[] { 1, 1 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
            if (this.currentReceiver.IsValidatingFreq(maxTransmitter.Freq - minTransmitter.Freq))
            {
                SecondOrderIMItem item = new SecondOrderIMItem(new ComparableFreq[] { maxTransmitter.ComparableFreq, minTransmitter.ComparableFreq }, new int[] { 1, -1 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
        }

        /// <summary>
        /// 计算二阶互调
        /// </summary>
        /// <param name="transmitter">计算互调用发射机</param>
        /// <param name="max">计算互调用发射机在发射机组中位置</param>
        private void CalculateSecondOrderIM(Transmitter transmitter1, Transmitter transmitter2)
        {
            Transmitter minTransmitter, maxTransmitter;
            if (transmitter1.Freq > transmitter2.Freq)
            {
                minTransmitter = transmitter2;
                maxTransmitter = transmitter1;
            }
            else
            {
                minTransmitter = transmitter1;
                maxTransmitter = transmitter2;
            }
            if (this.currentReceiver.IsValidatingFreq(minTransmitter.Freq + maxTransmitter.Freq))
            {
                SecondOrderIMItem item = new SecondOrderIMItem(new ComparableFreq[] { minTransmitter.ComparableFreq, maxTransmitter.ComparableFreq }, new int[] { 1, 1 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
            if (this.currentReceiver.IsValidatingFreq(maxTransmitter.Freq - minTransmitter.Freq))
            {
                SecondOrderIMItem item = new SecondOrderIMItem(new ComparableFreq[] { maxTransmitter.ComparableFreq, minTransmitter.ComparableFreq }, new int[] { 1, -1 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
        }

        /// <summary>
        /// 计算二阶互调
        /// </summary>
        /// <param name="transmitter">计算互调用发射机</param>
        /// <param name="max">计算互调用发射机在发射机组中位置</param>
        private void CalcEquSecondOrderIM(Transmitter transmitter, int max)
        {
            Transmitter minTransmitter, maxTransmitter;

            if (this.equtransmitters[max].Freq > transmitter.Freq)
            {
                minTransmitter = transmitter;
                maxTransmitter = equtransmitters[max];
            }
            else
            {
                minTransmitter = equtransmitters[max];
                maxTransmitter = transmitter;
            }
            if (this.currentReceiver.IsValidatingFreq(minTransmitter.Freq + maxTransmitter.Freq))
            {
                SecondOrderIMItem item = new SecondOrderIMItem(new ComparableFreq[] { minTransmitter.ComparableFreq, maxTransmitter.ComparableFreq }, new int[] { 1, 1 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
            if (this.currentReceiver.IsValidatingFreq(maxTransmitter.Freq - minTransmitter.Freq))
            {
                SecondOrderIMItem item = new SecondOrderIMItem(new ComparableFreq[] { maxTransmitter.ComparableFreq, minTransmitter.ComparableFreq }, new int[] { 1, -1 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
        }

        private void CalcStationSecondOrderIM(Transmitter transmitter, int max)
        {
            Transmitter minTransmitter, maxTransmitter;
            if (this.stationtransmitters[max].Freq > transmitter.Freq)
            {
                minTransmitter = transmitter;
                maxTransmitter = stationtransmitters[max];
            }
            else
            {
                minTransmitter = stationtransmitters[max];
                maxTransmitter = transmitter;
            }
            if (this.currentReceiver.IsValidatingFreq(minTransmitter.Freq + maxTransmitter.Freq))
            {
                SecondOrderIMItem item = new SecondOrderIMItem(new ComparableFreq[] { minTransmitter.ComparableFreq, maxTransmitter.ComparableFreq }, new int[] { 1, 1 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
            if (this.currentReceiver.IsValidatingFreq(maxTransmitter.Freq - minTransmitter.Freq))
            {
                SecondOrderIMItem item = new SecondOrderIMItem(new ComparableFreq[] { maxTransmitter.ComparableFreq, minTransmitter.ComparableFreq }, new int[] { 1, -1 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
        }

        /// <summary>
        /// 计算三阶互调
        /// </summary>
        /// <param name="min">计算互调用较小频率在发射机组中位置</param>
        /// <param name="max">计算互调用较大频率在发射机组中位置</param>
        private void CalcThirdOrderIM(int min, int max)
        {
            RegisterThirdOrderIMItem(this.stationtransmitters[max], this.stationtransmitters[min]);
            RegisterThirdOrderIMItem(this.stationtransmitters[min], this.stationtransmitters[max]);
            for (int i = max + 1; i < this.endPosition; i++)
            {
                if (this.currentReceiver.EquipID != this.stationtransmitters[i].EquipID)
                {
                    RegisterThirdOrderIMItem(this.stationtransmitters[max], this.stationtransmitters[min], this.stationtransmitters[i]);
                    RegisterThirdOrderIMItem(this.stationtransmitters[i], this.stationtransmitters[max], this.stationtransmitters[min]);
                    RegisterThirdOrderIMItem(this.stationtransmitters[i], this.stationtransmitters[min], this.stationtransmitters[max]);
                }
            }
        }

        /// <summary>
        /// 计算三阶互调
        /// </summary>
        /// <param name="transmitter">计算互调用发射机</param>
        /// <param name="max">计算互调用发射机在发射机组中位置</param>
        private void CalcThirdOrderIM(Transmitter transmitter, int max)
        {
            RegisterThirdOrderIMItem(this.stationtransmitters[max], transmitter);
            RegisterThirdOrderIMItem(transmitter, this.stationtransmitters[max]);
            for (int i = max + 1; i <= this.endPosition; i++)
            {
                if (this.currentReceiver.EquipID != this.stationtransmitters[i].EquipID)
                {
                    RegisterThirdOrderIMItem(this.stationtransmitters[max], transmitter, this.stationtransmitters[i]);
                    RegisterThirdOrderIMItem(this.stationtransmitters[i], this.stationtransmitters[max], transmitter);
                    RegisterThirdOrderIMItem(this.stationtransmitters[i], transmitter, this.stationtransmitters[max]);
                }
            }
        }


        /// <summary>
        /// 计算与台站的三阶互调
        /// </summary>
        /// <param name="transmitter">计算互调用发射机</param>
        /// <param name="max">计算互调用发射机在发射机组中位置</param>
        private void CalcStationThirdOrderIM(Transmitter transmitter, int max)
        {
            RegisterThirdOrderIMItem(this.stationtransmitters[max], transmitter);
            RegisterThirdOrderIMItem(transmitter, this.stationtransmitters[max]);
            for (int i = max + 1; i <= this.endPosition; i++)
            {
                if (this.currentReceiver.EquipID != this.stationtransmitters[i].EquipID)
                {
                    RegisterThirdOrderIMItem(this.stationtransmitters[max], transmitter, this.stationtransmitters[i]);
                    RegisterThirdOrderIMItem(this.stationtransmitters[i], this.stationtransmitters[max], transmitter);
                    RegisterThirdOrderIMItem(this.stationtransmitters[i], transmitter, this.stationtransmitters[max]);
                }
            }
        }

        /// <summary>
        /// 计算与设备的三阶互调
        /// </summary>
        /// <param name="transmitter">计算互调用发射机</param>
        /// <param name="max">计算互调用发射机在发射机组中位置</param>
        private void CalcEquThirdOrderIM(Transmitter transmitter, int max)
        {
            RegisterThirdOrderIMItem(this.equtransmitters[max], transmitter);
            RegisterThirdOrderIMItem(transmitter, this.equtransmitters[max]);
            for (int i = max + 1; i <= this.endPosition; i++)
            {
                if (this.currentReceiver.EquipID != this.equtransmitters[i].EquipID)
                {
                    RegisterThirdOrderIMItem(this.equtransmitters[max], transmitter, this.equtransmitters[i]);
                    RegisterThirdOrderIMItem(this.equtransmitters[i], this.equtransmitters[max], transmitter);
                    RegisterThirdOrderIMItem(this.equtransmitters[i], transmitter, this.equtransmitters[max]);
                }
            }
        }



        /// <summary>
        /// 登记三阶互调
        /// </summary>
        /// <param name="one">计算互调用发射机</param>
        /// <param name="other">另一个计算互调用发射机</param>
        private void RegisterThirdOrderIMItem(Transmitter one, Transmitter other)
        {
            double freqValue = one.Freq * 2 - other.Freq;
            if (freqValue > 0 && this.currentReceiver.IsValidatingFreq(freqValue))
            {
                double pinA = PowerIn(one, this.currentReceiver);
                double pinB = PowerIn(other, this.currentReceiver);
                double deviation = Math.Round((Math.Abs(one.Freq - this.currentReceiver.Freq) + Math.Abs(other.Freq - this.currentReceiver.Freq)) / 2, 4, MidpointRounding.AwayFromZero);
                double powerIn = 2 * pinA + pinB + 10 - 60 * Math.Log10(deviation);
                if (this.currentReceiver.IsValidatingSignal(powerIn))
                {
                    ThirdOrderIMItem item = new ThirdOrderIMItem(new ComparableFreq[] { one.ComparableFreq, other.ComparableFreq }, new int[] { 2, -1 });
                    item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                    this.imResult.RegisterIMItem(item);
                }
            }
        }

        /// <summary>
        /// 登记三阶互调
        /// </summary>
        ///<param name="one">第一个发射机</param>
        ///<param name="two">第二个发射机</param>
        ///<param name="three">第三个发射机</param>
        private void RegisterThirdOrderIMItem(Transmitter one, Transmitter two, Transmitter three)
        {
            double freqValue = one.Freq + two.Freq - three.Freq;
            if (this.currentReceiver.IsValidatingFreq(freqValue))
            {
                double deviation = Math.Round((Math.Abs(one.Freq - this.currentReceiver.Freq) + Math.Abs(two.Freq - this.currentReceiver.Freq)) + Math.Abs(three.Freq - this.currentReceiver.Freq) / 3, 4, MidpointRounding.AwayFromZero);
                double pinA = PowerIn(one, this.currentReceiver);
                double pinB = PowerIn(two, this.currentReceiver);
                double pinC = PowerIn(three, this.currentReceiver);
                double powerIn = 2 * pinA + pinB + pinC - 81 * Math.Log10(deviation);
                if (this.currentReceiver.IsValidatingSignal(powerIn))
                {
                    ThirdOrderIMItem item = new ThirdOrderIMItem(new ComparableFreq[] { one.ComparableFreq, two.ComparableFreq, three.ComparableFreq }, new int[] { 1, 1, -1 });
                    item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                    this.imResult.RegisterIMItem(item);
                }
            }
        }

        /// <summary>
        /// 获取发射机在接收机位置功率
        /// </summary>
        /// <param name="transmitter">发射机</param>
        /// <param name="receiver">接收机</param>
        /// <returns>发射机在接收机位置产生的功率</returns>
        private double PowerIn(Transmitter transmitter, Receiver receiver)
        {
            //ERP-Lb-Ld-L0+GRX-LRX
            //发射机和接收机距离
            double kmDistance = DistanceCalculator.GetKmDistance(transmitter.Coordinate, receiver.Coordinate);

            //为发射机与接收机之间的基本传输损耗中值,单位:dB
            double lb = GetTransLoss(transmitter, kmDistance);

            //阻挡绕设损耗
            double ld = 0;

            //为环境修正因子
            double lo = 0;

            //发射机在接收机位置产生功率
            return transmitter.Erp - lb - ld - lo + receiver.AntGaindBi - receiver.FeedLoss;
        }

        /// <summary>
        /// 获取发射机在指定距离的传输损耗
        /// </summary>
        /// <param name="transmitter">要计算损耗的发射机</param>
        /// <param name="kmDistance">传输距离</param>
        /// <returns>传输损耗</returns>
        private double GetTransLoss(Transmitter transmitter, double kmDistance)
        {
            if (kmDistance <= 0.0001)
                kmDistance = 0.0001;
            return 32.4 + 20 * Math.Log10(transmitter.Freq) + 20 * Math.Log10(kmDistance);
        }

        /// <summary>
        /// 计算五阶互调
        /// </summary>
        /// <param name="min">计算互调用较小频率在发射机组中位置</param>
        /// <param name="max">计算互调用较大频率在发射机组中位置</param>
        private void CalcFifthOrderIM(int min, int max)
        {
            RegisterFifthOrderIMItem(this.stationtransmitters[max].ComparableFreq, this.stationtransmitters[min].ComparableFreq);
            RegisterFifthOrderIMItem(this.stationtransmitters[min].ComparableFreq, this.stationtransmitters[max].ComparableFreq);
        }

        /// <summary>
        /// 计算五阶互调
        /// </summary>
        /// <param name="transmitter">发射机</param>
        /// <param name="max">另一个发射机位置</param>
        private void CalcFifthOrderIM(Transmitter transmitter, int max)
        {
            RegisterFifthOrderIMItem(transmitter.ComparableFreq, this.stationtransmitters[max].ComparableFreq);
            RegisterFifthOrderIMItem(this.stationtransmitters[max].ComparableFreq, transmitter.ComparableFreq);
        }

        /// <summary>
        /// 计算五阶互调
        /// </summary>
        /// <param name="transmitter">发射机</param>
        /// <param name="max">另一个发射机位置</param>
        private void CalcEquFifthOrderIM(Transmitter transmitter, int max)
        {
            RegisterFifthOrderIMItem(transmitter.ComparableFreq, this.equtransmitters[max].ComparableFreq);
            RegisterFifthOrderIMItem(this.equtransmitters[max].ComparableFreq, transmitter.ComparableFreq);
        }

        /// <summary>
        /// 计算五阶互调
        /// </summary>
        /// <param name="transmitter">发射机</param>
        /// <param name="max">另一个发射机位置</param>
        private void CalcStationFifthOrderIM(Transmitter transmitter, int max)
        {
            RegisterFifthOrderIMItem(transmitter.ComparableFreq, this.stationtransmitters[max].ComparableFreq);
            RegisterFifthOrderIMItem(this.stationtransmitters[max].ComparableFreq, transmitter.ComparableFreq);
        }

        /// <summary>
        /// 登记五阶互调
        /// </summary>
        /// <param name="one">计算互调用频率</param>
        /// <param name="other">另一个计算互调用频率</param>
        private void RegisterFifthOrderIMItem(ComparableFreq one, ComparableFreq other)
        {
            double freqValue = one.Freq * 3 - other.Freq * 2;
            if (freqValue > 0 && this.currentReceiver.IsValidatingFreq(freqValue))
            {
                FifthIMItem item = new FifthIMItem(new ComparableFreq[] { one, other }, new int[] { 3, -2 });
                item.DisturbedFreqs = new ComparableFreq[] { this.currentReceiver.ComparableFreq };
                this.imResult.RegisterIMItem(item);
            }
        }

        /// <summary>
        /// 按照频率在频率数组中查找最接近的频率,使用折半查找法
        /// </summary>
        /// <param name="freqValue">查找频率</param>
        /// <param name="left">搜索起始位置</param>
        /// <param name="right">搜索结束位置</param>
        /// <returns>查找到的位置</returns>
        private int BiSearchTransmitter(double freqValue, int left, int right)
        {
            if (left >= right)
                return left;
            int index = (right + left) / 2;
            double povit = this.stationtransmitters[index].Freq - freqValue;
            if (povit > ComparableFreq.PositiveZero)
            {
                return BiSearchTransmitter(freqValue, left, index - 1);
            }
            else if (povit < ComparableFreq.NegativeZero)
            {
                return BiSearchTransmitter(freqValue, index + 1, right);
            }
            else
                return index;
        }

        /// <summary>
        /// 处理双频率数据代理
        /// </summary>
        /// <param name="min">要处理的较小频率位置</param>
        /// <param name="max">要处理的较大频率位置</param>
        private delegate void DisposeDoubleFreqs(int min, int max);

        /// <summary>
        /// 处理发射机数据代理
        /// </summary>
        /// <param name="transmitter">待处理的发射机</param>
        /// <param name="max">另一个待处理发射机在已登记发射机组中的位置</param>
        private delegate void DisposeTransmitter(Transmitter transmitter, int max);
    }
}
