#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：工作统计
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion

using AT_BC.Common;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Types;
using CO_IA.UI.Statistic;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.ActivitySummarize
{
    /// <summary>
    /// WorkAchievement.xaml 的交互逻辑
    /// </summary>
    public partial class WorkAchievement : UserControl
    {
        public WorkAchievement()
        {
            InitializeComponent();
        }
        public WorkAchievement(int mode)
        {
            InitializeComponent();
            this.Loaded += WorkAchievement_Loaded;
            //this.gb_PersonAreaStatistics.DataContext = Canstant.GetInstance().PersonAreaStatistics;

            //this.gb_StaffOrgStatistics.DataContext = Canstant.GetInstance().StaffOrgStatistics;

            //this.gb_ActivityPEVStatistics.Content = Canstant.GetInstance().ActivityPEVStatistics;

            //this.gb_StationClearStatistics.DataContext = Canstant.GetInstance().StationClearStatistics;


            //this.gb_CallResourceStatistics.DataContext = Canstant.GetInstance().BasicSummarizeChartListForCallResource;

            //DrawingPiePoints(this.line_1, Canstant.GetInstance().SummarizeChartList[0].List);
            //line_1.DisplayName = Canstant.GetInstance().SummarizeChartList[0].Type;

            //DrawingPiePoints(this.line_2, Canstant.GetInstance().SummarizeChartList[1].List);
            //line_2.DisplayName = Canstant.GetInstance().SummarizeChartList[1].Type;

            //DrawingPiePoints(this.line_3, Canstant.GetInstance().SummarizeChartList[2].List);
            //line_3.DisplayName = Canstant.GetInstance().SummarizeChartList[2].Type;

        }

        void WorkAchievement_Loaded(object sender, RoutedEventArgs e)
        {
            AddItem();

            if (flowLayout.ItemsSource != null&&flowLayout.Children.Count!=0)
            {
                DevExpress.Xpf.LayoutControl.GroupBox groupbox = flowLayout.Children[0] as DevExpress.Xpf.LayoutControl.GroupBox;
                if (groupbox != null)
                    groupbox.State = GroupBoxState.Maximized;
            }
        }
        private void AddItem()
        {
            var arr = Enum.GetValues(typeof(StatisticTypes)) as StatisticTypes[];
            List<StaticItem> itemList = new List<StaticItem>();
            for (int i = 0; i < arr.Length; i++)
            {
                StaticItem item = new StaticItem();
                item.UC = GetUC(arr[i]);
                if (item.UC == null)
                    continue;
                item.Title = item.UC.Tag.ToString();
                itemList.Add(item);
                
            }
            flowLayout.ItemsSource = itemList;
        }

        public UserControl GetUC(StatisticTypes types)
        {
            UserControl UC= null;
            switch (types)
            { 
                case StatisticTypes.EquInspectionSticType:
                    if (!GetUI(ActivityStep.FreqPlanning))
                    {
                        return null;
                    }
                    EquInspectionStatisticControl _uc1 = new EquInspectionStatisticControl();
                    _uc1.ShowStatisticList = false;
                    _uc1.ShowLegend = false;
                    _uc1.StatisticSource = (StatisticHelper.StatisticEquInspection(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid) as IList<EquInspectionStatisticData>).ToList();                    
                    UC = _uc1;
                    UC.Tag = "设备检测统计";
                    break;
                case StatisticTypes.FreqAssignStatisticType:
                    if (!GetUI(ActivityStep.FreqPlanning))
                    {
                        return null;
                    }
                    FreqAssignStatisticControl _uc2 = new FreqAssignStatisticControl();
                    _uc2.ShowStatisticList = false;
                    _uc2.ShowLegend = false;
                    _uc2.StatisticSource = (StatisticHelper.StatisticFreqAssign(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid) as IList<FreqAssignStatisticData>).ToList();                    
                    UC = _uc2;
                    UC.Tag = "频率指配统计";
                    break;
                case StatisticTypes.FreqPartPlanStatisticType:
                    if (!GetUI(ActivityStep.FreqPlanning))
                    {
                        return null;
                    }
                    FreqPartPlanStatisticControl _uc3 = new FreqPartPlanStatisticControl();
                    _uc3.ShowStatisticList = false;
                    _uc3.ShowLegend = false;
                    _uc3.StatisticSource = (StatisticHelper.StatisticFreqPartPlan(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid) as IList<FreqPartPlanStatisticData>).ToList();
                    UC = _uc3;
                    UC.Tag = "频率保障方案统计";
                    break;
                case StatisticTypes.ORGAndEQUStatisticType:
                    if (!GetUI(ActivityStep.FreqPlanning))
                    {
                        return null;
                    }
                    ORGAndEQUStatisticControl _uc4 = new ORGAndEQUStatisticControl();
                    _uc4.ShowStatisticList = false;
                    _uc4.ShowLegend = false;
                    System.Collections.IList list = StatisticHelper.StatisticEquipment(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                    _uc4.StatisticSource = (StatisticHelper.StatisticEquipment(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid) as IList<EquStatisticData>).ToList();
                    UC = _uc4;
                    UC.Tag = "参保单位统计";
                    break;
                case StatisticTypes.SurroundStatStatisticType:
                    if (!GetUI(ActivityStep.StationPlanning))
                    {
                        return null;
                    }
                    RoundStatStatisticControl _uc5 = new RoundStatStatisticControl();
                    _uc5.ShowStatisticList = false;
                    _uc5.ShowLegend = false;
                    _uc5.StatisticSource = (StatisticHelper.StatisticSurroundStation(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid) as IList<SurroundStatStatisticData>).ToList();
                    UC = _uc5;
                    UC.Tag = "周围台站统计";
                    break;
                case StatisticTypes.PersonPlanStatisticType:
                    if (!GetUI(ActivityStep.StaffPlanning))
                    {
                        return null;
                    }
                    PersonPlanStatisticControl _uc6 = new PersonPlanStatisticControl();
                    _uc6.ShowStatisticList = false;
                    _uc6.ShowLegend = false;
                    IList _statisticSource = StatisticHelper.StatisticPersonPlan(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                    _uc6.PersonPlanStatisticSource = _statisticSource as IList<PersonPlanStatisticData>;
                    _uc6.StatisticSource = _statisticSource;   
                    UC = _uc6;
                    UC.Tag = "人员预案统计";
                    break;
                default:
                    return null;
            }
                    
            
            return UC;
        }
        private bool GetUI(ActivityStep step)
        {
            var uiFactory = Utility.GetUIFactory();
            if (uiFactory != null)
            {
                var uiBuilder = uiFactory.GetUIBuilder(RiasPortal.ModuleContainer.Activity.ActivityType);
                if (uiBuilder.CanBuildStep(step))
                {
                    return true;
                }
            }
            return false;

        }
        private void DrawingPiePoints(LineSeries2D p_bar, List<BasicSummarizeChart> p_list)
        {
            p_bar.Points.Clear();
            foreach (BasicSummarizeChart pac in p_list)
            {
                SeriesPoint sp = new SeriesPoint();
                sp.Argument = pac.Name;
                sp.Value = pac.Value;

                p_bar.Points.Add(sp);
            }
        }

        /// <summary>
        /// 放大和缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            var groupBox = VisualTreeHelperExtension.GetParentObject<DevExpress.Xpf.LayoutControl.GroupBox>(img);

            groupBox.State = groupBox.State == GroupBoxState.Normal ? GroupBoxState.Maximized : GroupBoxState.Normal;
            
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

  
        /// <summary>
        /// 环形图左键放开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChartControl chartName = sender as ChartControl;
            ChartHitInfo hitInfo = chartName.CalcHitInfo(e.GetPosition(chartName));
            if (hitInfo == null || hitInfo.SeriesPoint == null)
                return;
            double distance = PieSeries.GetExplodedDistance(hitInfo.SeriesPoint);
            Storyboard storyBoard = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300));
            animation.To = distance > 0 ? 0 : 0.3;
            storyBoard.Children.Add(animation);
            Storyboard.SetTarget(animation, hitInfo.SeriesPoint);
            Storyboard.SetTargetProperty(animation, new PropertyPath(PieSeries.ExplodedDistanceProperty));
            storyBoard.Begin();

            e.Handled = true;
        }

        private void GroupBox_StateChanged(object sender, DevExpress.Xpf.Core.ValueChangedEventArgs<GroupBoxState> e)
        {
            DevExpress.Xpf.LayoutControl.GroupBox group = sender as DevExpress.Xpf.LayoutControl.GroupBox;
            StaticItem item = group.DataContext as StaticItem;
            IStatisticObject uc = null;
            if (item != null && item.UC != null)
            {
                uc = item.UC as IStatisticObject;                
            }
            if (group.State == GroupBoxState.Maximized)
            {
                uc.ShowLegend = true;
            }
            if (group.State == GroupBoxState.Normal)
            {
                uc.ShowLegend = false;
            }
        }
    }
    public class StaticItem
    { 
        public string Title{get;set;}
        public UserControl UC { get; set; } 
    }
}
