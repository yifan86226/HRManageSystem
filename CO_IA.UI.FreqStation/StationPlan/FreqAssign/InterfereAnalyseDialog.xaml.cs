using CO_IA.Data;
using CO_IA.InterferenceAnalysis;
using CO_IA_Data;
using I_CO_IA.FreqStation;
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
using System.Windows.Shapes;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// InterfereAnalyseDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InterfereAnalyseDialog : Window
    {
        //干扰集合
        Dictionary<ActivityEquipment, List<InterfereResult>> dicInterfereResult = new Dictionary<ActivityEquipment, List<InterfereResult>>();
        //互调干扰集合
        Dictionary<ActivityEquipment, List<IMInterfereResult>> dicIMInterfereResult = new Dictionary<ActivityEquipment, List<IMInterfereResult>>();
        //干扰统计
        Dictionary<ActivityEquipment, InterferenceAnalysisResult> dicInterfereCount = new Dictionary<ActivityEquipment, InterferenceAnalysisResult>();
        InterferedCalculateManage _interferedCalculateManage = null;
        private List<ActivityEquipment> Equipments
        {
            get;
            set;
        }

        private List<ActivitySurroundStation> AroundStation
        {
            get;
            set;
        }

        public ActivityEquipment InterfereEquipment
        {
            get
            {
                return InterferedResult.SelectedItem == null ? null : (InterferedResult.SelectedItem as ActivityEquipment);
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

        public InterfereAnalyseDialog(List<ActivityEquipment> equs, List<ActivitySurroundStation> surroundstation)
        {
            InitializeComponent();

            busyIndicator.IsBusy = true;
            this.DataContext = this;
            this.Equipments = equs;
            this.AroundStation = surroundstation;

            Func<InterferedCalculateManage> func = this.CreateCalculator;
            func.BeginInvoke(async =>
            {
                _interferedCalculateManage = func.EndInvoke(async);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    busyIndicator.IsBusy = false;
                    this.dicInterfereResult = _interferedCalculateManage.DicInterfereResult;
                    this.dicIMInterfereResult = _interferedCalculateManage.DicIMInterfereResult;

                    List<ActivityEquipment> interequs = _interferedCalculateManage.GetInterferedAllEquipments();
                    InterferedResult.ItemsSource = interequs;
                    if (interequs == null || interequs.Count == 0)
                    {
                        MessageBox.Show("经过计算,没有干扰结果");
                        this.Close();
                    }
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
            _baseInterfereResultControl.DataContext = null;
            _iMInterfereResultControl.DataContext = null;

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
                    _baseInterfereResultControl.DataContext = list;

                    //获取干扰统计信息
                    samecount = list.Where(r => r.InterfType == InterfereTypeEnum.同频干扰).Sum(r => r.InterfObject.Count);
                    adjcount = list.Where(r => r.InterfType == InterfereTypeEnum.邻频干扰).Sum(r => r.InterfObject.Count);
                }

                List<IMInterfereResult> imlist = null;
                if (dicIMInterfereResult.Keys.Contains(InterfereEquipment))
                {
                    imlist = this.dicIMInterfereResult[InterfereEquipment];
                    _iMInterfereResultControl.DataContext = imlist.OrderBy(r => r.InterfOrder).ToList();
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
                //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                //{
                //    ActivityEquipment sources = channel.GetActivityEquipment(InterfereEquipment);
                //    freqAssignListControl.DataContext = sources;
                //});


                //EquipmentDetailDialog dialog = new EquipmentDetailDialog(InterfereEquipment);
                //dialog.IsEnabled = false;
                //dialog.WindowTitle = "设备-详细信息";
                //dialog.ShowDialog(this);
            }
        }

    }
}
