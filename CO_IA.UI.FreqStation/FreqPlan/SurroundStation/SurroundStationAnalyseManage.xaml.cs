using CO_IA.Data;
using CO_IA_Data;
using I_CO_IA.FreqStation;
using I_CO_IA.StationManage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using AT_BC.Data;
using System.Threading;
using System.ComponentModel;
using CO_IA.Client;
using System.Data;
using System.Linq;
using System.Reflection;
using System.IO;
using CO_IA.UI.Setting;

namespace CO_IA.UI.FreqStation.FreqPlan.SurroundStation
{
    /// <summary>
    /// AroundStationAnalysisManage.xaml 的交互逻辑
    /// </summary>
    public partial class SurroundStationAnalyseManage : UserControl
    {
        private static object locked = new object();

        private ActivityPlaceInfo CurrentActivityPlace
        {
            get;
            set;
        }

        private List<StationInfo> stationitemsource = new List<StationInfo>();
        public List<StationInfo> StationItemsSource
        {
            get
            {
                return stationitemsource;
            }
            set
            {
                stationitemsource = value;
            }
        }


        private ObservableCollection<ActivitySurroundStation> activitystationitemssource = new ObservableCollection<ActivitySurroundStation>();

        public ObservableCollection<ActivitySurroundStation> ActivityStationItemsSource
        {
            get
            {
                return activitystationitemssource;
            }
            set
            {
                activitystationitemssource = value;
            }
        }

        public SurroundStationAnalyseManage(ActivityPlaceInfo acvtivityplace)
        {
            InitializeComponent();
            CurrentActivityPlace = acvtivityplace;
            mapcontrol.CurrentPlaceInfo = CurrentActivityPlace;
            QuerySurroundStations();

            surroundStationListControl.StationSelectionChanged += surroundStationListControl_StationSelectionChanged;
        }

        /// <summary>
        /// 在地图上选中台站
        /// </summary>
        /// <param name="obj"></param>
        private void surroundStationListControl_StationSelectionChanged(ActivitySurroundStation obj)
        {
            mapcontrol.StationSelectionChanges(obj);
        }

        private void xbtnQuery_Click(object sender, RoutedEventArgs e)
        {
            PlaceFreqPlan[] placefreqplans = this.GetPlaceFreqPlans(CurrentActivityPlace.Guid);
            FreqPlanningSelectDialog freqselectdialog = new FreqPlanningSelectDialog();
            freqselectdialog.DataContext = placefreqplans;
            freqselectdialog.CurrentActivityPlace = CurrentActivityPlace;
            freqselectdialog.RefreshItemsSource += () =>
            {
                QuerySurroundStations();
            };
            freqselectdialog.ShowDialog();
        }

        private void xbtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.surroundStationListControl.SurroundStationItemsSource != null && this.surroundStationListControl.SurroundStationItemsSource.Count > 0)
            {
                this.Busy("正在保存,请稍后...");
                try
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                    {
                        channel.SaveActivitySurroundStation(this.surroundStationListControl.SurroundStationItemsSource.ToList());
                        this.Idle();
                    });
                    MessageBox.Show("保存成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage(), "保存失败");
                }
            }
            else
            {
                MessageBox.Show("请先查询周围台站");
            }
        }

        private void xbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<ActivitySurroundStation> surstation = surroundStationListControl.SurroundStationItemsSource.Where(r => r.IsChecked == true).ToList();
            if (surstation.Count == 0)
            {
                MessageBox.Show("请选择要删除的周围台站");
            }
            else
            {
                try
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                    {
                        channel.DeleteActivitySurroundStation(surstation);
                    });
                    this.surroundStationListControl.UnChecked();
                    QuerySurroundStations();
                    MessageBox.Show("删除成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "删除失败");
                    throw;
                }
            }
        }

        /// <summary>
        /// 查询周围台站
        /// </summary>
        private void QuerySurroundStations()
        {
            List<ActivitySurroundStation> stations = QuerySurroundStationFromDB(null);
            foreach (ActivitySurroundStation item in stations)
            {
                ActivityStationItemsSource.Add(item);
            }

            SetStationListItemsSource();
        }

        private List<ActivitySurroundStation> QuerySurroundStationFromDB(List<string> freqs)
        {
            ActivityStationItemsSource.Clear();

            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, List<ActivitySurroundStation>>(channel =>
            {
                return channel.GetActivitySurroundStations(this.CurrentActivityPlace.ActivityGuid, this.CurrentActivityPlace.Guid, freqs);
            });
        }

        private void SetStationListItemsSource()
        {
            surroundStationListControl.DataContext = ActivityStationItemsSource;
            List<SurroundStationInfo> stations = new List<SurroundStationInfo>();
            this.mapcontrol.DrawStations(ActivityStationItemsSource.ToList());
        }

        private PlaceFreqPlan[] GetPlaceFreqPlans(string placeguid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, PlaceFreqPlan[]>(channel =>
            {
                return channel.GetPlaceFreqPlans(placeguid);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
