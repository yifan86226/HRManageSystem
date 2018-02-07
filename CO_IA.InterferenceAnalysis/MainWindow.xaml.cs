using EMCS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.InterferenceAnalysis
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ComparableFreq[] compareFreqs;
        ComparableFreq[] calcFreqs;

        Receiver[] receivers;
        Transmitter[] transmitters;

        public MainWindow()
        {
            InitializeComponent();
            //InitData();
        }

        //private void InitData()
        //{
        //    compareFreqs = new ComparableFreq[6];
        //    compareFreqs[0] = new ComparableFreq(18, 0, 2, "1");
        //    compareFreqs[1] = new ComparableFreq(19, 0, 2, "2");
        //    compareFreqs[2] = new ComparableFreq(20, 0, 2, "3");
        //    compareFreqs[3] = new ComparableFreq(20, 0, 4, "4");
        //    compareFreqs[4] = new ComparableFreq(21, 0, 2, "5");
        //    compareFreqs[5] = new ComparableFreq(22, 0, 4, "6");
        //    datagrid.ItemsSource = compareFreqs;

        //    calcFreqs = new ComparableFreq[1];
        //    calcFreqs[0] = new ComparableFreq(20, 0, 2, "1");

        //    //互调干扰数据源
        //    //接收  new EMCGeographyCoordinate(109, 36.01)
        //    receivers = new Receiver[1];
        //    receivers[0] = new Receiver("1",
        //        new ComparableFreq(30, 0,2, "1"),
        //        new ReceiverParams()
        //        {
        //            TuningRangeStart = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 8),
        //            TuningRangeEnd = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 32),
        //            IFBand = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 4),
        //            Sensitivity = -1000,
        //            SensitivityUnit = SensitivityUnitEnum.dBm

        //        }, null );

        //    //发射 new EMCGeographyCoordinate(109, 36)
        //    transmitters = new Transmitter[4];

        //    transmitters[0] = new Transmitter("4", new ComparableFreq(10, 2, "2"),
        //        new TransmitterParams()
        //        {
        //            TuningRangeStart = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 9),
        //            TuningRangeEnd = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 15),
        //            Power = new EMCPowerValue(EMCPowerUnitEnum.W, 10)
        //        }, null
        //        );

        //    transmitters[1] = new Transmitter("5", new ComparableFreq(15, 2, "1"),
        //        new TransmitterParams()
        //        {
        //            TuningRangeStart = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 10),
        //            TuningRangeEnd = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 18),
        //            Power = new EMCPowerValue(EMCPowerUnitEnum.W, 10)
        //        }, null);

        //    transmitters[2] = new Transmitter("6", new ComparableFreq(20, 2, "1"),
        //        new TransmitterParams()
        //        {
        //            TuningRangeStart = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 19),
        //            TuningRangeEnd = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 22),
        //            Power = new EMCPowerValue(EMCPowerUnitEnum.W, 10)
        //        },
        //       null);

        //    transmitters[3] = new Transmitter("7", new ComparableFreq(40, 2, "2"),
        //       new TransmitterParams()
        //       {
        //           TuningRangeStart = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 35),
        //           TuningRangeEnd = new EMCFreqValue(EMCFreqUnitEnum.MHZ, 45),
        //           Power = new EMCPowerValue(EMCPowerUnitEnum.W, 10)
        //       },
        //       null);

        //    transdatagrid.ItemsSource = transmitters;

        //}

        /// <summary>
        /// 同频计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSameFreq_Click(object sender, RoutedEventArgs e)
        {
            //AnalysisType analysisType = AnalysisType.None ;
            //InterferenceAnalysisResult result = InterferenceAnalysis.Calculator(analysisType,compareFreqs, transmitters, receivers);

            this.samefreqresult.Text = null;
            SameFreqCalculator samefreqCalc = new SameFreqCalculator(calcFreqs, compareFreqs);
            SameFreqCompareResult[] samecompareResult = samefreqCalc.CalcSameFreqInterference();

            StringBuilder strmsg = new StringBuilder();
            if (samecompareResult != null && samecompareResult.Length > 0)
            {
                for (int i = 0; i < samecompareResult.Length; i++)
                {
                    SameFreqCompareResult sameresult = samecompareResult[i];
                    ComparableFreq ifreq = sameresult.Keys[0];
                    strmsg.AppendFormat("{0}:受干扰频率:ID:{1},频率:{2},带宽:{3} \r", i + 1, ifreq.FreqID, ifreq.Freq, ifreq.Band);
                    strmsg.Append("  干扰频率:");
                    string disfreq = string.Empty;
                    foreach (ComparableFreq item in sameresult.Values)
                    {
                        strmsg.AppendFormat("干扰频率:ID:{0},频率:{1},带宽:{2}\r", item.FreqID, item.Freq, item.Band);
                    }
                    strmsg.Append("\r");
                }
            }
            this.samefreqresult.Text = strmsg.ToString();
        }

        /// <summary>
        /// 领频计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdjFreq_Click(object sender, RoutedEventArgs e)
        {
            adjfreqresult.Text = null;
            AdjFreqCalculator adjfreqCalc = new AdjFreqCalculator(calcFreqs, compareFreqs);
            AdjFreqCompareResult[] adjcompareResult = adjfreqCalc.CalcAdjFreqInterference();
            StringBuilder strmsg = new StringBuilder();
            if (adjcompareResult != null && adjcompareResult.Length > 0)
            {
                for (int i = 0; i < adjcompareResult.Length; i++)
                {
                    AdjFreqCompareResult adjresult = adjcompareResult[i];
                    ComparableFreq ifreq = adjresult.Keys[0];
                    strmsg.AppendFormat("{0}:受干扰频率:ID:{1},频率:{2},带宽:{3} \r", i + 1, ifreq.FreqID, ifreq.Freq, ifreq.Band);
                    //上邻频干扰
                    if (adjresult.HasUpperAdjFreq)
                    {
                        foreach (ComparableFreq item in adjresult.UpperAdjFreqs)
                        {
                            strmsg.AppendFormat("上邻频干扰:ID:{0},频率:{1},带宽:{2} \r", item.FreqID, item.Freq, item.Band);
                        }
                    }

                    //下邻频干扰
                    if (adjresult.HasLowerAdjFreq)
                    {
                        foreach (ComparableFreq item in adjresult.LowerAdjFreqs)
                        {
                            strmsg.AppendFormat("下邻频干扰:ID:{0},频率:{1},带宽:{2} \r", item.FreqID, item.Freq, item.Band);
                        }
                    }
                    strmsg.Append("\r");
                }
            }
            adjfreqresult.Text = strmsg.ToString();
        }

        /// <summary>
        /// 互调干扰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnIM_Click(object sender, RoutedEventArgs e)
        {
            imresult.Text = null;
            IMOrder order = IMOrder.Second | IMOrder.Third | IMOrder.Fifth;
            IMAnalysisResult result = new IMAnalysisResult();
            IMCalculator calculator = new IMCalculator(transmitters, receivers, transmitters, receivers);
            result.SetReceiverImResult(calculator.ReceiverIM(order));
            result.SetTransmitterImResult(calculator.TransmitterIM());

            StringBuilder strmsg = new StringBuilder();

            int index = 0;
            //接收互调
            IMCompareResult receiverResult = result.GetReceiverImResult();
            if (receiverResult != null)
            {
                strmsg.Append("接收机互调干扰: \r");
                IMItemBase imBase = null;

                for (int i = 0; i < receiverResult.Values.Length; i++)
                {
                    imBase = receiverResult.Values[i];
                    for (int j = 0; j < imBase.DisturbedFreqs.Length; j++)
                    {
                        index++;
                        ComparableFreq disfreq = imBase.DisturbedFreqs[j];
                        strmsg.AppendFormat("{0}:受干扰接收机: 频率ID:{1},频率:{2},带宽:{3} \r", index, disfreq.FreqID, disfreq.Freq, disfreq.Band);
                        strmsg.AppendFormat("干扰阶数:{0},干扰公式:{1} \r", GetIMOrder(imBase.Order), imBase.Formula);
                        for (int k = 0; k < imBase.IMFreqs.Length; k++)
                        {
                            strmsg.AppendFormat("干扰频率{0}:频率:{1},带宽:{2} \r", k + 1, imBase.IMFreqs[k].Freq, imBase.IMFreqs[k].Band);
                        }
                        strmsg.Append("\r");
                    }
                }
            }

            //发射机互调
            IMCompareResult transResult = result.GetTransmitterImResult();
            if (transResult != null && transResult.Values.Length > 0)
            {
                strmsg.Append("发射机互调干扰: \r");
                IMItemBase imBase = null;
                index = 0;
                for (int i = 0; i < transResult.Values.Length; i++)
                {
                    imBase = transResult.Values[i];
                    for (int j = 0; j < imBase.DisturbedFreqs.Length; j++)
                    {
                        index++;
                        ComparableFreq disfreq = imBase.DisturbedFreqs[j];
                        strmsg.AppendFormat("{0}:受干扰发射机: 频率ID:{1},频率:{2},带宽:{3} \r", index, disfreq.FreqID, disfreq.Freq, disfreq.Band);
                        strmsg.AppendFormat("干扰阶数:{0},干扰公式:{1} \r", GetIMOrder(imBase.Order), imBase.Formula);
                        for (int k = 0; k < imBase.IMFreqs.Length; k++)
                        {
                            strmsg.AppendFormat("干扰频率{0}:频率:{1},带宽:{2} \r", k + 1, imBase.IMFreqs[k].Freq, imBase.IMFreqs[k].Band);
                        }
                        strmsg.Append("\r");
                    }
                }

            }


            imresult.Text = strmsg.ToString();
        }

        private string GetIMOrder(IMOrder order)
        {
            switch (order)
            {
                case IMOrder.Second:
                    return "二阶干扰";
                case IMOrder.Third:
                    return "二阶干扰";
                case IMOrder.Fifth:
                    return "二阶干扰";
                default: return null;
            }
            return null;
        }
    }
}
