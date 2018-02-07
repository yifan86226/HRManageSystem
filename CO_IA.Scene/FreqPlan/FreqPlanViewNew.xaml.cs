using CO_IA.Data;
using CO_IA_Data;
using I_CO_IA.FreqStation;
using I_CO_IA.PlanDatabase;
using PT_BS_Service.Client.Framework;
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
using System.Windows.Shapes;

namespace CO_IA.Scene.FreqPlan
{
    /// <summary>
    /// FreqPlanViewNew.xaml 的交互逻辑
    /// </summary>
    public partial class FreqPlanViewNew : UserControl
    {
        public event Action<Equipment> DoubleClick;
        //public Organization[] Organizations { get; set; }
        
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
        public FreqPlanViewNew()
        {
            InitializeComponent();

            //LoadGridSource();
            LayoutGrid.DataContext = this;
            mapcontrol.CurrentPlaceInfo = SystemLoginService.CurrentActivityPlace;
            QuerySurroundStations();
        }
        
        private void stationdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mapcontrol.StationSelectionChanges(stationdatagrid.SelectedItem as ActivitySurroundStation);
        }
        //private void LoadGridSource()
        //{
        //    Organizations = DataOperator.GetORGSource(new OrgQueryCondition() { ActivityGuid =  SystemLoginService.CurrentActivity.Guid});
        //}
        //private void dataGridRowEquipment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ChangedButton == MouseButton.Left)
        //    {
        //        if (this.DoubleClick == null)
        //        {
        //            return;
        //        }
        //        DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
        //        if (dgr != null)
        //        {
        //            Equipment equipment = dgr.DataContext as Equipment;
        //            if (equipment != null)
        //            {
        //                this.DoubleClick(equipment);
        //            }
        //        }
        //    }
        //}

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
                return channel.GetActivitySurroundStations(SystemLoginService.CurrentActivity.Guid, SystemLoginService.CurrentActivityPlace.Guid, freqs);
            });
        }

        private void SetStationListItemsSource()
        {
            List<SurroundStationInfo> stations = new List<SurroundStationInfo>();
            this.mapcontrol.DrawStations(ActivityStationItemsSource.ToList());
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            QuerySurroundStations();
        }

    }
}
