using CO_IA.Data;
using CO_IA.Data.Collection;
using CO_IA.UI.StationPlan.Converter;
using CO_IA.InterferenceAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using EMCS.Types;
using CO_IA_Data;

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// FreqAssignList_Control.xaml 的交互逻辑
    /// </summary>
    public partial class FreqAssignListControl : UserControl
    {

        //Application..Resources["keyBoolToVisibility"];

        /// <summary>
        /// 开始频率
        /// </summary>
        public double FreqBegin
        {
            get;
            set;
        }

        /// <summary>
        /// 结束频率
        /// </summary>
        public double FreqEnd
        {
            get;
            set;
        }

        /// <summary>
        /// 第一个频率
        /// </summary>
        public double FirstFreq
        {
            get;
            set;
        }

        /// <summary>
        /// 带宽
        /// </summary>
        public double Band
        {
            get;
            set;
        }

        /// <summary>
        /// 设备信息
        /// </summary>
        public ActivityEquipmentInfo EquInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<RoundStationInfo> AroundStations
        {
            get;
            set;
        }

        private bool showInterfere = true;
        public bool ShowInterfere
        {
            get
            {
                return showInterfere;
            }
            set
            {
                showInterfere = value;
                if (showInterfere)
                {
                    gridInterfere.Visibility = Visibility.Visible;
                }
                else
                {
                    gridInterfere.Visibility = Visibility.Collapsed;
                }
            }
        }

        private FreqUse selectedfreq;
        public FreqUse SelectedFreq
        {
            get 
            {
                return selectedfreq;
            }
            set
            {
                selectedfreq = value;
                tbselectfreq.Text = selectedfreq.Freq.ToString();
            }
        }

        public string PlaceGuid
        {
            get;
            set;
        }

        public string BusinessCode
        {
            get;
            set;
        }

        private List<ActivityEquipmentInfo> placeEqulist;
        private List<AnalysisResult> analysisRequltList;
        private List<EmeClearInfo> emeClearInfoList;

        public FreqAssignListControl()
        {
            InitializeComponent();
            freqContainer.AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonClick));
        }

        private void GetPlaceEqus()
        {
            EquipmentQueryCondition querycondition = new EquipmentQueryCondition();
            querycondition.ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            querycondition.PlaceGuid = this.PlaceGuid;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                placeEqulist = channel.GetEquipmentInfos(querycondition);
            });
        }

        private void GetEmeClearInfos()
        {
            EmeClearQueryCondition queryCondition = new EmeClearQueryCondition();
            queryCondition.PlaceGuid = this.PlaceGuid;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                emeClearInfoList = channel.GetEmeClearHandleInfoList(queryCondition);
            });
        }

        private void GetAnalysisResult()
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                analysisRequltList = channel.GetAnalysisResultListByCode(this.PlaceGuid, this.BusinessCode);
            });
        }

        public bool InitPage()
        {
            if (EquInfo.BusinessCode == "")
            {
                MessageBox.Show("设备没有设置业务类型！", "提示", MessageBoxButton.OK);
                return false;
            }
            else if (EquInfo.BusinessCode.Substring(0, 2) == "GD")
            {
                MessageBox.Show("设备业务类型为微波，无法给出建议频率，请自行指定。", "提示", MessageBoxButton.OK);
                return false;
            }
            List<FreqPlanSegment> segmentList = GetFreqPlanSegment(this.EquInfo.BusinessCode);
            if (segmentList.Count == 0)
            {
                MessageBox.Show("业务“" + this.EquInfo.BusinessCode + "”没有规划，无法给出建议频率！", "提示", MessageBoxButton.OK);
                return false;
            }
            //FreqBegin = 10;
            //FreqEnd = 500;
            //Band = 2;
            //FirstFreq = 12;
            GetPlaceEqus();
            GetAnalysisResult();
            GetEmeClearInfos();
            foreach (FreqPlanSegment segment in segmentList)
            {
                FreqBegin = segment.FreqValue.Little;
                FreqEnd = segment.FreqValue.Great;
                if (segment.FreqBand.Split('/').Length > 0)
                {
                    Band = double.Parse(segment.FreqBand.Split('/')[0]);
                }
                else
                {
                    Band = double.Parse(segment.FreqBand);
                }
                CreateFreqTable(GetFreqUses());
            }
            this.txtFreqs.Text = string.Format("{0}-{1}", segmentList[0].FreqValue.Little / (double)1000000, segmentList[segmentList.Count - 1].FreqValue.Great / (double)1000000);
            return true;
        }

        public bool InitPage(string placeGuid, string businessCode)
        {
            if (businessCode == "")
            {
                MessageBox.Show("业务类型为空，无法显示频率占用界面！", "提示", MessageBoxButton.OK);
                return false;
            }
            else if (businessCode == "GD")
            {
                MessageBox.Show("设备业务类型为微波，无法显示频率占用界面！", "提示", MessageBoxButton.OK);
                return false;
            }
            List<FreqPlanSegment> segmentList = GetFreqPlanSegment(businessCode);
            if (segmentList.Count == 0)
            {
                MessageBox.Show("业务“" + businessCode + "”没有规划，无法显示频率占用界面！", "提示", MessageBoxButton.OK);
                return false;
            }
            //FreqBegin = 10;
            //FreqEnd = 500;
            //Band = 2;
            //FirstFreq = 12;
            LoadTxtFreqs(segmentList);
            return true;
        }
        public bool InitPage2(List<FreqPlanSegment> p_list)
        {
            LoadTxtFreqs(p_list);
            return true;
        }
        private void LoadTxtFreqs(List<FreqPlanSegment> segmentList)
        {
            GetPlaceEqus();
            GetAnalysisResult();
            GetEmeClearInfos();
            foreach (FreqPlanSegment segment in segmentList)
            {
                FreqBegin = segment.FreqValue.Little;
                FreqEnd = segment.FreqValue.Great;
                if (segment.FreqBand.Split('/').Length > 0)
                {
                    Band = double.Parse(segment.FreqBand.Split('/')[0]);
                }
                else
                {
                    Band = double.Parse(segment.FreqBand);
                }
                CreateFreqTable(GetFreqUses());
            }
            this.txtFreqs.Text = string.Format("{0}-{1}", segmentList[0].FreqValue.Little / (double)1000000, segmentList[segmentList.Count - 1].FreqValue.Great / (double)1000000);
        }

        private List<FreqPlanSegment> GetFreqPlanSegment(string businessCode)
        {
            List<FreqPlanSegment> result = new List<FreqPlanSegment>();
            List<CO_IA.Data.FreqPlanSegment> segmentList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<CO_IA.Data.FreqPlanSegment>>(
                channel =>
                {
                    return channel.GetFreqPlanInfo();
                });
            if (!string.IsNullOrEmpty(businessCode))
            {
                foreach (FreqPlanSegment freqPlanSegment in segmentList)
                {
                    //if (freqPlanSegment.FreqValue.Little / (double)1000000 < this.EquInfo.SendFreqStart.Value
                    //    && freqPlanSegment.FreqValue.Great / (double)1000000 > this.EquInfo.SendFreqEnd.Value)
                    //{
                    //    segment = freqPlanSegment;
                    //    break;
                    //}
                    if (freqPlanSegment.ClassCode == businessCode)
                    {
                        result.Add(freqPlanSegment);
                    }
                }
            }
            else
            {
                result = segmentList;
            }
            result = result.OrderBy(r => r.FreqValue.Little).ToList();
            return result;
        }

        private List<FreqUse> GetFreqUses()
        {
            List<FreqUse> freqs = new List<FreqUse>();
            double dfreq = FirstFreq == 0.0 ? FreqBegin : FirstFreq;
            FreqUse frequse;
            for (double freq = dfreq; freq <= FreqEnd; )
            {
                frequse = new FreqUse();
                frequse.Freq = freq / (double)1000000;
                frequse.Use = Usage.None;
                freqs.Add(frequse);

                freq = freq + Band;
                CheckFreq(frequse);
                //if (freq % 5 == 0)
                //{
                //    frequse.Use = Usage.Applied;
                //}
                //if (freq % 5 == 2)
                //{
                //    frequse.Use = Usage.Lawful;
                //}
                //if (freq % 5 == 4)
                //{
                //    frequse.Use = Usage.UnLawful;
                //}

            }
            return freqs;
        }

        private void CreateFreqTable(List<FreqUse> lst)
        {
            foreach (FreqUse item in lst)
            {
                Button button = new Button();
                button.DataContext = item;
                Border border=new Border{ Child=new TextBlock{Text=item.Freq.ToString(), HorizontalAlignment= System.Windows.HorizontalAlignment.Center}};
                button.Content = border;
                button.Tag = item.Use.ToString();
                button.ToolTip = item.ToolTip;
                border.Background = new SolidColorBrush(Colors.White);
                //if (item.Use == Usage.None)
                //{
                //    button.IsEnabled = true;
                //}
                //else
                //{
                //    button.IsEnabled = false;
                //}
                FreqUseConverter useconverter = new FreqUseConverter();
                Binding binding = new Binding()
                {
                    Source = item,
                    Path = new PropertyPath("Use"),
                    Converter = useconverter
                };
                border.SetBinding(Border.BackgroundProperty, binding);
                freqContainer.Children.Add(button);
            }
        }

        private void CheckFreq(FreqUse freqUse)
        {
            string toolTip = "空闲";
            Usage usage = Usage.None;
            //第一个查AnalysisResult，进行预指
            foreach (AnalysisResult analysisReqult in analysisRequltList)
            {
                if (analysisReqult.Frequency == freqUse.Freq)
                {
                    if (analysisReqult.FreqType == SignalTypeEnum.已占)
                    {
                        if (string.IsNullOrEmpty(analysisReqult.FreqGuid))
                        {
                            usage = Usage.UnLawful;
                            toolTip = "非法占用";
                        }
                        else
                        {
                            usage = Usage.Lawful;
                            toolTip = "台站：" + analysisReqult.StationName + " 占用";
                        }
                    }
                    break;
                }
            }
            //第二个查周围台站清理情况
            foreach (EmeClearInfo emeInfo in emeClearInfoList)
            {
                //0 未作处理；1 清理成功；2 清理失败
                if (emeInfo.ResultIsClear != "1")
                {
                    if (emeInfo.FREQ_EC != null && emeInfo.FREQ_BAND != null)
                    {
                        double freq = (double)emeInfo.FREQ_EC;
                        double band = (double)emeInfo.FREQ_BAND;
                        double freqStart = freq - band / 2;
                        double freqEnd = freq + band / 2;
                        //先判断起点是否落在里面
                        if (freqUse.Freq <= freqStart && freqStart <= (freqUse.Freq + Band/(double)1000000))
                        {
                            usage = Usage.Lawful;
                            toolTip = "台站：" + emeInfo.StationName + " 占用";
                            break;
                        }
                        //再判断终点是否落在里面
                        if (freqUse.Freq <= freqEnd && freqEnd <= (freqUse.Freq + Band/(double)1000000))
                        {
                            usage = Usage.Lawful;
                            toolTip = "台站：" + emeInfo.StationName + " 占用";
                            break;
                        }
                        //最后判断是否覆盖
                        if (freqStart <= freqUse.Freq && (freqUse.Freq + Band/(double)1000000) <= freqEnd)
                        {
                            usage = Usage.Lawful;
                            toolTip = "台站：" + emeInfo.StationName + " 占用";
                            break;
                        }
                    }
                }
            }
            //检查频率是否为申请频率
            foreach (ActivityEquipmentInfo equ in placeEqulist)
            {
                //如果是指配建议调用此页面时，排除要指配的这个设备。
                if (this.EquInfo != null && equ.GUID == this.EquInfo.GUID)
                {
                    continue;
                }
                if (freqUse.Freq == equ.AssignFreq)
                {
                    usage = Usage.Applied;
                    toolTip = "设备：" + equ.Name + " 申请";
                    break;
                }
            }
            freqUse.Use = usage;
            freqUse.ToolTip = toolTip;
        }

        private void GetInterfereInfo()
        {
            ActivityEquipmentInfo equInfo = CopyEqu(this.EquInfo);

            if (this.SelectedFreq != null)
            {
                equInfo.AssignFreq = this.SelectedFreq.Freq;
            }
            else
            {
                MessageBox.Show("请先选定频率再进行干扰分析。", "提示", MessageBoxButton.OK);
                return;
            }
            if (equInfo.BusinessCode.Substring(0, 4) == "LY01")
            {
                MessageBox.Show("公众移动通信系统的设备不进行干扰计算。", "提示", MessageBoxButton.OK);
                chkIterfere.IsChecked = false;
                gridInterfere.Visibility = Visibility.Collapsed;
                return;
            }
            
            //ComparableFreq CalcFreq = new ComparableFreq((double)EquInfo.AssignFreq, (double)EquInfo.Band, EquInfo.GUID);
            //ComparableFreq[] CalcFreqs = { CalcFreq };
            //ComparableFreq[] CompFreq = ConverterToComparableFreq(this.AroundStations);
            List<RoundStationInfo> aroundStations = new List<RoundStationInfo>(this.AroundStations.ToArray());
            List<ActivityEquipmentInfo> equs = new List<ActivityEquipmentInfo>();
            equs.Add(equInfo);
            //Transmitter[] Transmitters = ConverterToTransmitter(equs, aroundStations);
            //Receiver[] Receivers = ConverterToReceiver(equs, aroundStations);
            try
            {
                AnalysisType type = AnalysisType.SameFreq | AnalysisType.ADJFreq | AnalysisType.IM;
                InterferenceAnalysisResult InterfResult = InterfAnalysis.Calculator(equs, aroundStations, type);
                //InterferenceAnalysisResult InterfAnalysisResult = InterfAnalysis.Calculator(type, CalcFreqs, CompFreq,calctransmitters,calcreceivers, Transmitters, Receivers);
                gridInterfereResult.DataContext = InterfResult;
                int total = InterfResult.Total;
            }
            catch (Exception ex)
            {
                MessageBox.Show("干扰分析出错，请检查设备频率。", "提示", MessageBoxButton.OK);
                return;
            }

        }

        private ActivityEquipmentInfo CopyEqu(ActivityEquipmentInfo equ)
        {
            ActivityEquipmentInfo equInfo = new ActivityEquipmentInfo();
            equInfo.GUID = equ.GUID;
            equInfo.OrgInfo = equ.OrgInfo;
            equInfo.ORG = equ.ORG;
            equInfo.IsChecked = equ.IsChecked;
            equInfo.ORGGuid = equ.ORGGuid;
            equInfo.ActivityGuid = equ.ActivityGuid;
            equInfo.PlaceGuid = equ.PlaceGuid;
            equInfo.Name = equ.Name;
            equInfo.EQUCount = equ.EQUCount;
            equInfo.Remark = equ.Remark;
            equInfo.IsStation = equ.IsStation;
            equInfo.StationName = equ.StationName;
            equInfo.IsMobile = equ.IsMobile;
            equInfo.SendFreq = equ.SendFreq;
            equInfo.ReceiveFreq = equ.ReceiveFreq;
            equInfo.IsTunAble = equ.IsTunAble;
            equInfo.SendFreqStart = equ.SendFreqStart;
            equInfo.SendFreqEnd = equ.SendFreqEnd;
            equInfo.MaxPower = equ.MaxPower;
            equInfo.Band = equ.Band;
            equInfo.ChannelBand = equ.ChannelBand;
            equInfo.Leakage = equ.Leakage;
            equInfo.ModulateMode = equ.ModulateMode;
            equInfo.RecvFreqStart = equ.RecvFreqStart;
            equInfo.RecvFreqEnd = equ.RecvFreqEnd;
            equInfo.Sensitivity = equ.Sensitivity;
            equInfo.SensitivityUnit = equ.SensitivityUnit;
            equInfo.ADJChannelInh = equ.ADJChannelInh;
            equInfo.SignalNoise = equ.SignalNoise;
            equInfo.CoChnPro = equ.CoChnPro;
            equInfo.RunningFrom = equ.RunningFrom;
            equInfo.RunningTo = equ.RunningTo;
            equInfo.Origin = equ.Origin;
            equInfo.EquModel = equ.EquModel;
            equInfo.EquNo = equ.EquNo;
            equInfo.AssignFreq = equ.AssignFreq;
            equInfo.RecvAntModel = equ.RecvAntModel;
            equInfo.RecvAntGain = equ.RecvAntGain;
            equInfo.RecvAntElevation = equ.RecvAntElevation;
            equInfo.RecvAntAzimuth = equ.RecvAntAzimuth;
            equInfo.RecvAntPolar = equ.RecvAntPolar;
            equInfo.RecvAntHeight = equ.RecvAntHeight;
            equInfo.RecvAntFeedLength = equ.RecvAntFeedLength;
            equInfo.RecvAntFeedLoss = equ.RecvAntFeedLoss;
            equInfo.SendAntModel = equ.SendAntModel;
            equInfo.SendAntGain = equ.SendAntGain;
            equInfo.SendAntElevation = equ.SendAntElevation;
            equInfo.SendAntAzimuth = equ.SendAntAzimuth;
            equInfo.SendAntPolar = equ.SendAntPolar;
            equInfo.SendAntHeight = equ.SendAntHeight;
            equInfo.SendAntFeedLength = equ.SendAntFeedLength;
            equInfo.SendAntFeedLoss = equ.SendAntFeedLoss;
            equInfo.BusinessCode = equ.BusinessCode;
            return equInfo;
        }

        /// <summary>
        /// Button路由事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            FreqUse selectedFreq = button.DataContext as FreqUse;
            if (selectedFreq.Use != Usage.None)
            {
                button.Focusable = false;
            }
            else
            {
                SelectedFreq = selectedFreq;
                if (this.ShowInterfere && (bool)chkIterfere.IsChecked)
                {
                    GetInterfereInfo();
                }
            }
        }
    }
}
