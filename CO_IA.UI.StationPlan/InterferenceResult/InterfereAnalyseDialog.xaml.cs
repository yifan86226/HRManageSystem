using CO_IA.Data;
using CO_IA.InterferenceAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CO_IA.UI.FreqPlan;
using CO_IA_Data;

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// SameFreqResultControl.xaml 的交互逻辑
    /// </summary>
    public partial class InterfereAnalyseDialog : Window
    {
        //干扰集合
        Dictionary<ActivityEquipmentInfo, List<InterfereResult>> dicInterfereResult = new Dictionary<ActivityEquipmentInfo, List<InterfereResult>>();
        //互调干扰集合
        Dictionary<ActivityEquipmentInfo, List<IMInterfereResult>> dicIMInterfereResult = new Dictionary<ActivityEquipmentInfo, List<IMInterfereResult>>();
        //干扰统计
        Dictionary<ActivityEquipmentInfo, InterferenceAnalysisResult> dicInterfereCount = new Dictionary<ActivityEquipmentInfo, InterferenceAnalysisResult>();
        InterferedCalculateManage _interferedCalculateManage = null;
        private List<ActivityEquipmentInfo> Equipments
        {
            get;
            set;
        }

        private List<RoundStationInfo> AroundStation
        {
            get;
            set;
        }

        public ActivityEquipmentInfo InterfereEquipment
        {
            get
            {
                return InterferedResult.SelectedItem == null ? null : (InterferedResult.SelectedItem as ActivityEquipmentInfo);
            }
        }

        public InterferenceAnalysisResult SelectedInterfResult
        {
            get { return (InterferenceAnalysisResult)GetValue(SelectedInterfResultProperty); }
            set { SetValue(SelectedInterfResultProperty, value); }
        }

        public static readonly DependencyProperty SelectedInterfResultProperty =
            DependencyProperty.Register("SelectedInterfResult", typeof(InterferenceAnalysisResult), typeof(InterfereAnalyseDialog),
            new PropertyMetadata(new PropertyChangedCallback(ResultPropertyChangedCallback)));

        private static void ResultPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public InterfereAnalyseDialog(List<ActivityEquipmentInfo> equs, List<RoundStationInfo> aroundstation)
        {
            InitializeComponent();

            busyIndicator.IsBusy = true;
            this.DataContext = this;
            this.Equipments = equs;
            this.AroundStation = aroundstation;

            Func<InterferedCalculateManage> func = this.CreateCalculator;
            func.BeginInvoke(async =>
                {
                    _interferedCalculateManage = func.EndInvoke(async);
                    this.Dispatcher.Invoke(new Action(() =>
                        {
                            busyIndicator.IsBusy = false;
                            InterferedResult.ItemsSource = _interferedCalculateManage.GetInterferedAllEquipments();
                            this.dicInterfereResult = _interferedCalculateManage.DicInterfereResult;
                            this.dicIMInterfereResult = _interferedCalculateManage.DicIMInterfereResult;
                        }));
                }, null);
        }

        private InterferedCalculateManage CreateCalculator()
        {
            return new InterferedCalculateManage(this.Equipments, this.AroundStation, AnalysisType.SameFreq | AnalysisType.ADJFreq | AnalysisType.IM);
        }

        /// <summary>
        /// DataGrid选择行改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterferedResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InterfereEquipment != null)
            {
                List<InterfereResult> list = null;
                int samecount = 0;
                int adjcount = 0;
                int imtcount = 0;
                int imrcount = 0;
                if (this.dicInterfereResult.Keys.Contains(InterfereEquipment))
                {
                    list = this.dicInterfereResult[InterfereEquipment];
                    _baseInterfereResultControl.InterfItemsSource = list;

                    //获取干扰统计信息
                    samecount = list.Count(r => r.InterfType == InterfereTypeEnum.同频干扰);
                    adjcount = list.Count(r => r.InterfType == InterfereTypeEnum.邻频干扰);
                }

                List<IMInterfereResult> imlist = null;
                if (dicIMInterfereResult.Keys.Contains(InterfereEquipment))
                {
                    imlist = this.dicIMInterfereResult[InterfereEquipment];
                    _iMInterfereResultControl.IMInterfItemsSource = imlist;
                    imtcount = imlist.Count(r => r.InterfType == InterfereTypeEnum.发射机互调干扰);
                    imrcount = imlist.Count(r => r.InterfType == InterfereTypeEnum.接收机互调干扰);
                }

                InterferenceAnalysisResult result = new InterferenceAnalysisResult();
                result.SameFreqInterfResultCount = samecount;
                result.ADJFreqInterfResultCount = adjcount;
                result.IMInterfResultCount = imtcount + imrcount;
                SelectedInterfResult = result;
            }
        }

        /// <summary>
        /// DataGrid行双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterferedResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (InterfereEquipment != null)
            {
                EquipmentDetailDialog dialog = new EquipmentDetailDialog(InterfereEquipment);
                dialog.IsEnabled = false;
                dialog.WindowTitle = "设备-详细信息";
                dialog.ShowDialog(this);
            }
        }

        #region 注释
        //private void CalcInterfereAnalyse()
        //{
        //    dicInterfereResult.Clear();
        //    dicIMInterfereResult.Clear();

        //    AnalysisType type = AnalysisType.SameFreq | AnalysisType.ADJFreq | AnalysisType.IM;
        //    InterferenceAnalysisResult InterfResult = InterfAnalysis.Calculator(this.Equipments, this.AroundStation, type);
        //    List<InterfereResult> interfereresult = new List<InterfereResult>();

        //    #region 获取同频干扰结果

        //    if (InterfResult.SameFreqInterfResult != null)
        //    {
        //        List<InterfereResult> sameInterferresult = GetSameFreqInterfResult(InterfResult.SameFreqInterfResult);
        //        interfereresult.AddRange(sameInterferresult);
        //    }

        //    #endregion

        //    #region 获取邻频干扰结果

        //    if (InterfResult.ADJFreqInterfResult != null)
        //    {
        //        List<InterfereResult> adjInterferresult = GetADJFreqInterfResult(InterfResult.ADJFreqInterfResult);
        //        interfereresult.AddRange(adjInterferresult);
        //    }

        //    #endregion

        //    #region 获取互调干扰结果

        //    if (InterfResult.IMInterfResult != null)
        //    {
        //        List<IMInterfereResult> imInterferresult = GetIMInterfResult(InterfResult.IMInterfResult);
        //        interfereresult.AddRange(imInterferresult);
        //    }

        //    #endregion

        //    //取两个集合的并集作为数据源
        //    List<ActivityEquipmentInfo> itemsource = dicInterfereResult.Keys.Union(dicIMInterfereResult.Keys).ToList();
        //    InterferedResult.ItemsSource = itemsource;
        //}

        ///// <summary>
        ///// 获取同频干扰结果
        ///// </summary>
        ///// <param name="result"></param>
        ///// <returns></returns>
        //private List<InterfereResult> GetSameFreqInterfResult(SameFreqCompareResult[] result)
        //{
        //    List<InterfereResult> interfresultlst = new List<InterfereResult>(); ;
        //    foreach (SameFreqCompareResult item in result)
        //    {
        //        ActivityEquipmentInfo equ = new ActivityEquipmentInfo();
        //        equ = Equipments.FirstOrDefault(r => r.GUID == item.Keys[0].FreqID);

        //        bool isnewInterfere = true; //新的干扰,不存在dicInterfereResult字典中
        //        if (this.dicInterfereResult.TryGetValue(equ, out interfresultlst))
        //        {
        //            isnewInterfere = false;
        //        }
        //        else
        //        {
        //            isnewInterfere = true;
        //            interfresultlst = new List<InterfereResult>();
        //        }
        //        InterfereResult sameinterfInfo = new InterfereResult();
        //        sameinterfInfo.InterfType = InterfereTypeEnum.同频干扰;
        //        sameinterfInfo.InterfOrder = InterfereOrderEnum.同频;
        //        sameinterfInfo.InterfObject = new List<InterfereObject>();

        //        #region 遍历干扰列表，取干扰设备或者干扰物

        //        foreach (ComparableFreq freq in item.Values)
        //        {
        //            //先在设备列表中查找，如果有，则认为干扰物为设备
        //            ActivityEquipmentInfo Interfequ = Equipments.FirstOrDefault(r => r.GUID == freq.FreqID);
        //            InterfereObject interfobject = null;
        //            if (Interfequ != null)
        //            {
        //                interfobject = CreateInterfObjectFromEqu(Interfequ);
        //            }
        //            else
        //            {
        //                //如设备列表中没有，则在周围台站列表中查找，如果有，则认为干扰物为设备
        //                RoundStationInfo Interfstation = AroundStation.FirstOrDefault(r => r.STATGUID == freq.FreqID);
        //                if (Interfstation != null)
        //                {
        //                    interfobject = CreateInterfObjectFromStation(Interfstation, freq);
        //                }
        //                else
        //                {
        //                    //如设备列表和周围台站列表中都没有此干扰物，则认为干扰物为其他（这种情况应该不会出现？？）
        //                    interfobject = CreateInterfObjectFromOther(freq);
        //                }
        //            }
        //            sameinterfInfo.InterfObject.Add(interfobject);
        //        }

        //        #endregion

        //        interfresultlst.Add(sameinterfInfo);
        //        //将新的干扰结果加到字典中
        //        if (isnewInterfere)
        //        {
        //            this.dicInterfereResult.Add(equ, interfresultlst);
        //        }
        //    }
        //    return interfresultlst;
        //}

        ///// <summary>
        ///// 获取邻频干扰结果
        ///// </summary>
        ///// <param name="result"></param>
        ///// <returns></returns>
        //private List<InterfereResult> GetADJFreqInterfResult(AdjFreqCompareResult[] result)
        //{
        //    List<InterfereResult> adjinterfresultlst = new List<InterfereResult>();
        //    //干扰结果
        //    foreach (AdjFreqCompareResult item in result)
        //    {
        //        ActivityEquipmentInfo equ = new ActivityEquipmentInfo();
        //        equ = Equipments.FirstOrDefault(r => r.GUID == item.Keys[0].FreqID); //获取干扰设备

        //        bool isnewInterfere = true; //新的干扰,不存在dicInterfereResult字典中
        //        if (this.dicInterfereResult.TryGetValue(equ, out adjinterfresultlst))
        //        {
        //            isnewInterfere = false;
        //        }
        //        else
        //        {
        //            isnewInterfere = true;
        //            adjinterfresultlst = new List<InterfereResult>();
        //        }

        //        #region 上邻频干扰

        //        if (item.UpperAdjFreqs != null && item.UpperAdjFreqs.Length > 0)
        //        {
        //            InterfereResult uadinterfInfo = new InterfereResult();
        //            uadinterfInfo.InterfType = InterfereTypeEnum.邻频干扰;
        //            uadinterfInfo.InterfOrder = InterfereOrderEnum.上邻频;
        //            uadinterfInfo.InterfObject = new List<InterfereObject>();
        //            foreach (ComparableFreq uadjfreq in item.UpperAdjFreqs)
        //            {
        //                //先在设备列表中查找，如果有，则认为干扰物为设备
        //                ActivityEquipmentInfo Interfequ = Equipments.FirstOrDefault(r => r.GUID == uadjfreq.FreqID);
        //                InterfereObject interfobject = null;
        //                if (Interfequ != null)
        //                {
        //                    interfobject = CreateInterfObjectFromEqu(Interfequ);
        //                }
        //                else
        //                {
        //                    //如设备列表中没有，则在周围台站列表中查找，如果有，则认为干扰物为设备
        //                    RoundStationInfo Interfstation = AroundStation.FirstOrDefault(r => r.STATGUID == uadjfreq.FreqID);
        //                    if (Interfstation != null)
        //                    {
        //                        interfobject = CreateInterfObjectFromStation(Interfstation, uadjfreq);
        //                    }
        //                    else
        //                    {
        //                        //如设备列表和周围台站列表中都没有此干扰物，则认为干扰物为其他（这种情况应该不会出现？？）
        //                        interfobject = CreateInterfObjectFromOther(uadjfreq);
        //                    }
        //                }
        //                uadinterfInfo.InterfObject.Add(interfobject);
        //            }
        //            adjinterfresultlst.Add(uadinterfInfo);
        //        }

        //        #endregion

        //        #region 下邻频干扰

        //        if (item.LowerAdjFreqs != null && item.LowerAdjFreqs.Length > 0)
        //        {
        //            InterfereResult ladjinterfInfo = new InterfereResult();
        //            ladjinterfInfo.InterfType = InterfereTypeEnum.邻频干扰;
        //            ladjinterfInfo.InterfOrder = InterfereOrderEnum.下邻频;
        //            ladjinterfInfo.InterfObject = new List<InterfereObject>();

        //            foreach (ComparableFreq ladjfreq in item.LowerAdjFreqs)
        //            {
        //                //先在设备列表中查找，如果有，则认为干扰物为设备
        //                ActivityEquipmentInfo Interfequ = Equipments.FirstOrDefault(r => r.GUID == ladjfreq.FreqID);
        //                InterfereObject interfobject = null;
        //                if (Interfequ != null)
        //                {
        //                    interfobject = CreateInterfObjectFromEqu(Interfequ);
        //                }
        //                else
        //                {
        //                    //如设备列表中没有，则在周围台站列表中查找，如果有，则认为干扰物为设备
        //                    RoundStationInfo Interfstation = AroundStation.FirstOrDefault(r => r.STATGUID == ladjfreq.FreqID);
        //                    if (Interfstation != null)
        //                    {
        //                        interfobject = CreateInterfObjectFromStation(Interfstation, ladjfreq);
        //                    }
        //                    else
        //                    {
        //                        //如设备列表和周围台站列表中都没有此干扰物，则认为干扰物为其他（这种情况应该不会出现？？）
        //                        interfobject = CreateInterfObjectFromOther(ladjfreq);
        //                    }
        //                }
        //                ladjinterfInfo.InterfObject.Add(interfobject);
        //            }
        //            adjinterfresultlst.Add(ladjinterfInfo);
        //        }
        //        #endregion

        //        if (isnewInterfere)
        //        {
        //            this.dicInterfereResult.Add(equ, adjinterfresultlst);
        //        }
        //    }
        //    return adjinterfresultlst;
        //}

        ///// <summary>
        ///// 获取互调干扰结果
        ///// </summary>
        ///// <returns></returns>
        //private List<IMInterfereResult> GetIMInterfResult(IMAnalysisResult result)
        //{
        //    List<IMInterfereResult> imInterfresultlst = new List<IMInterfereResult>();
        //    List<IMInterfereResult> receiverImResult = GetReceiverImResult(result.GetReceiverImResult());
        //    imInterfresultlst.AddRange(receiverImResult);

        //    //List<IMInterfereResult> transImResult = GetTransmitterImResult(result.GetTransmitterImResult());
        //    //imInterfresultlst.AddRange(transImResult);

        //    return imInterfresultlst;
        //}

        ///// <summary>
        ///// 获取接收机互调
        ///// </summary>
        ///// <param name="receiverResult"></param>
        ///// <returns></returns>
        //private List<IMInterfereResult> GetReceiverImResult(IMCompareResult receiverResult)
        //{
        //    List<IMInterfereResult> imInterfresultlst = new List<IMInterfereResult>();

        //    if (receiverResult != null)
        //    {
        //        #region 获取干扰结果

        //        IMItemBase imBase = null;
        //        for (int i = 0; i < receiverResult.Values.Length; i++)
        //        {
        //            imBase = receiverResult.Values[i];
        //            ActivityEquipmentInfo equ = new ActivityEquipmentInfo();
        //            ComparableFreq disfreq = imBase.DisturbedFreqs[0];
        //            equ = Equipments.FirstOrDefault(r => r.GUID == disfreq.FreqID);

        //            bool isnewInterfere = true; //新的干扰,不存在dicIMInterfereResult字典中
        //            if (equ != null)
        //            {
        //                if (this.dicIMInterfereResult.TryGetValue(equ, out imInterfresultlst))
        //                {
        //                    isnewInterfere = false;
        //                }
        //                else
        //                {
        //                    isnewInterfere = true;
        //                    imInterfresultlst = new List<IMInterfereResult>();
        //                }

        //                IMInterfereResult iminterfinfo = new IMInterfereResult();
        //                iminterfinfo.InterfType = InterfereTypeEnum.接收机互调干扰;
        //                iminterfinfo.InterfOrder = GetIMInterfOrder(imBase.Order);
        //                iminterfinfo.Formual = imBase.Formula;
        //                iminterfinfo.InterfObject = new List<InterfereObject>();


        //                #region  获取干扰台站

        //                for (int k = 0; k < imBase.IMFreqs.Length; k++)
        //                {

        //                    ComparableFreq freq = imBase.IMFreqs[k];

        //                    //先在设备列表中查找，如果有，则认为干扰物为设备
        //                    ActivityEquipmentInfo Interfequ = Equipments.FirstOrDefault(r => r.GUID == freq.FreqID);
        //                    InterfereObject interfobject = null;
        //                    if (Interfequ != null)
        //                    {
        //                        interfobject = CreateInterfObjectFromEqu(Interfequ);
        //                    }
        //                    else
        //                    {
        //                        //如设备列表中没有，则在周围台站列表中查找，如果有，则认为干扰物为设备
        //                        RoundStationInfo Interfstation = AroundStation.FirstOrDefault(r => r.STATGUID == freq.FreqID);
        //                        if (Interfstation != null)
        //                        {
        //                            interfobject = CreateInterfObjectFromStation(Interfstation, freq);
        //                        }
        //                        else
        //                        {
        //                            //如设备列表和周围台站列表中都没有此干扰物，则认为干扰物为其他（这种情况应该不会出现？？）
        //                            interfobject = CreateInterfObjectFromOther(freq);
        //                        }
        //                    }
        //                    iminterfinfo.InterfObject.Add(interfobject);
        //                }

        //                #endregion

        //                imInterfresultlst.Add(iminterfinfo);

        //                if (isnewInterfere)
        //                {
        //                    this.dicIMInterfereResult.Add(equ, imInterfresultlst);
        //                }
        //            }
        //        }
        //        #endregion
        //    }

        //    return imInterfresultlst;
        //}

        ///// <summary>
        ///// 获取发射机互调
        ///// </summary>
        ///// <param name="transmResult"></param>
        ///// <returns></returns>
        //private List<IMInterfereResult> GetTransmitterImResult(IMCompareResult transmResult)
        //{
        //    List<IMInterfereResult> imInterfresultlst = new List<IMInterfereResult>();
        //    if (transmResult != null)
        //    {
        //        #region 获取干扰结果

        //        IMItemBase imBase = null;
        //        for (int i = 0; i < transmResult.Values.Length; i++)
        //        {
        //            imBase = transmResult.Values[i];
        //            ActivityEquipmentInfo equ = new ActivityEquipmentInfo();

        //            ComparableFreq imfreq = imBase.IMFreqs[0];//发射互调的干扰频率存在IMFreqs中
        //            equ = Equipments.FirstOrDefault(r => r.GUID == imfreq.FreqID);

        //            bool isnewInterfere = true; //新的干扰,不存在dicIMInterfereResult字典中
        //            if (equ != null)
        //            {
        //                if (this.dicIMInterfereResult.TryGetValue(equ, out imInterfresultlst))
        //                {
        //                    isnewInterfere = false;
        //                }
        //                else
        //                {
        //                    isnewInterfere = true;
        //                    imInterfresultlst = new List<IMInterfereResult>();
        //                }

        //                IMInterfereResult iminterfinfo = new IMInterfereResult();
        //                iminterfinfo.InterfType = InterfereTypeEnum.发射机互调干扰;
        //                iminterfinfo.InterfOrder = GetIMInterfOrder(imBase.Order);
        //                iminterfinfo.Formual = imBase.Formula;
        //                iminterfinfo.InterfObject = new List<InterfereObject>();


        //                #region  获取干扰台站


        //                ComparableFreq disfreq = imBase.DisturbedFreqs[0];

        //                //先在设备列表中查找，如果有，则认为干扰物为设备
        //                ActivityEquipmentInfo Interfequ1 = Equipments.FirstOrDefault(r => r.GUID == disfreq.FreqID);
        //                InterfereObject interfobject1 = null;
        //                if (Interfequ1 != null)
        //                {
        //                    interfobject1 = CreateInterfObjectFromEqu(Interfequ1);
        //                }
        //                else
        //                {
        //                    //如设备列表中没有，则在周围台站列表中查找，如果有，则认为干扰物为设备
        //                    RoundStationInfo Interfstation = AroundStation.FirstOrDefault(r => r.STATGUID == disfreq.FreqID);
        //                    if (Interfstation != null)
        //                    {
        //                        interfobject1 = CreateInterfObjectFromStation(Interfstation,disfreq);
        //                    }
        //                    else
        //                    {
        //                        //如设备列表和周围台站列表中都没有此干扰物，则认为干扰物为其他（这种情况应该不会出现？？）
        //                        interfobject1 = CreateInterfObjectFromOther(imBase.DisturbedFreqs[0]);
        //                    }
        //                }
        //                iminterfinfo.InterfObject.Add(interfobject1);

        //                //先在设备列表中查找，如果有，则认为干扰物为设备
        //                ActivityEquipmentInfo Interfequ2 = Equipments.FirstOrDefault(r => r.GUID == imBase.IMFreqs[1].FreqID);
        //                InterfereObject interfobject2 = null;
        //                if (Interfequ2 != null)
        //                {
        //                    interfobject2 = CreateInterfObjectFromEqu(Interfequ2);
        //                }
        //                else
        //                {
        //                    //如设备列表中没有，则在周围台站列表中查找，如果有，则认为干扰物为设备
        //                  RoundStationInfo  Interfstation = AroundStation.FirstOrDefault(r => r.STATGUID == imBase.IMFreqs[1].FreqID);
        //                  if (Interfstation != null)
        //                    {
        //                        interfobject2 = CreateInterfObjectFromStation(Interfstation, imBase.IMFreqs[1]);
        //                    }
        //                    else
        //                    {
        //                        //如设备列表和周围台站列表中都没有此干扰物，则认为干扰物为其他（这种情况应该不会出现？？）
        //                        interfobject2 = CreateInterfObjectFromOther(imBase.IMFreqs[1]);
        //                    }
        //                }
        //                iminterfinfo.InterfObject.Add(interfobject2);

        //                #endregion

        //                imInterfresultlst.Add(iminterfinfo);

        //                if (isnewInterfere)
        //                {
        //                    this.dicIMInterfereResult.Add(equ, imInterfresultlst);
        //                }
        //            }
        //        }
        //        #endregion
        //    }
        //    return imInterfresultlst;
        //}

        ///// <summary>
        ///// 获取互调干扰阶数
        ///// </summary>
        ///// <param name="order"></param>
        ///// <returns></returns>
        //private InterfereOrderEnum GetIMInterfOrder(IMOrder order)
        //{
        //    switch (order)
        //    {
        //        case IMOrder.Second:
        //            return InterfereOrderEnum.二阶互调;

        //        case IMOrder.Third:
        //            return InterfereOrderEnum.三阶互调;

        //        case IMOrder.Fifth:
        //            return InterfereOrderEnum.五阶互调;
        //    }
        //    return InterfereOrderEnum.干扰;
        //}

        ///// <summary>
        ///// 取设备干扰信息
        ///// </summary>
        ///// <param name="Interfequ"></param>
        ///// <returns></returns>
        //private InterfereObject CreateInterfObjectFromEqu(ActivityEquipmentInfo Interfequ)
        //{
        //    InterfereObject intobject = new InterfereObject();
        //    intobject.Guid = Interfequ.GUID;
        //    intobject.Name = Interfequ.Name;
        //    intobject.Type = InterfereObjectEnum.设备;
        //    intobject.Freq = Interfequ.SendFreq;
        //    intobject.Band = Interfequ.Band;

        //    return intobject;
        //}

        ///// <summary>
        ///// 取台站干扰信息
        ///// </summary>
        ///// <param name="Interfstation"></param>
        ///// <returns></returns>
        //private InterfereObject CreateInterfObjectFromStation(RoundStationInfo Interfstation, ComparableFreq freq)
        //{
        //    InterfereObject intobject = new InterfereObject();
        //    intobject.Guid = Interfstation.STATGUID;
        //    intobject.Name = Interfstation.STAT_NAME;
        //    intobject.Type = InterfereObjectEnum.周围台站;
        //    intobject.Freq = freq.Freq;
        //    intobject.Band = freq.Band;
        //    return intobject;
        //}

        ///// <summary>
        ///// 其他干扰信息
        ///// </summary>
        ///// <param name="Interfother"></param>
        ///// <returns></returns>
        //private InterfereObject CreateInterfObjectFromOther(ComparableFreq Interfother)
        //{
        //    InterfereObject intobject = new InterfereObject();
        //    intobject.Guid = Interfother.FreqID;
        //    intobject.Name = "其他干扰";
        //    intobject.Type = InterfereObjectEnum.其他;
        //    intobject.Freq = Interfother.Freq;
        //    intobject.Band = Interfother.Band;
        //    return intobject;
        //}
        #endregion

    }
}
