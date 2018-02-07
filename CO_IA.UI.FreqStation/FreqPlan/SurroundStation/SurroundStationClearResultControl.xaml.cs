using AT_BC.Common;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.StationManage;
using CO_IA_Data;
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

namespace CO_IA.UI.FreqStation.FreqPlan.SurroundStation
{
    /// <summary>
    /// SurroundStationClearResultControl.xaml 的交互逻辑
    /// </summary>
    public partial class SurroundStationClearResultControl : UserControl
    {
        private CheckBox checkBoxAll;
        public SurroundStationClearResultControl()
        {
            InitializeComponent();
            this.DataContextChanged += SurroundStationClearResultControl_DataContextChanged;
        }

        private ActivityPlaceInfo currentPlace;

        private void SurroundStationClearResultControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var activityPlace = e.NewValue as ActivityPlaceInfo;
            currentPlace = activityPlace;
            this.LoadStationClearResult();
        }

        private void LoadStationClearResult()
        {
            if (currentPlace != null)
            {
                var stationList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation, List<ActivitySurroundStation>>(channel =>
                {
                    return channel.GetClearSurroundStations(currentPlace.ActivityGuid, currentPlace.Guid);
                });
                if (stationList != null && stationList.Count > 0)
                {
                    this.dataGridSurroundStation.ItemsSource = new ObservableCollection<ActivitySurroundStation>(stationList);
                }
                else
                {
                    this.dataGridSurroundStation.ItemsSource = new ObservableCollection<ActivitySurroundStation>();
                }
            }
        }

        private void dataGridSurroundStation_LayoutUpdated(object sender, EventArgs e)
        {
            this.dataGridSurroundStation.RowHeight = Double.NaN;
        }

        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckableCheckBoxHelper.Checked(sender, e, this.checkBoxAll);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridSurroundStation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    ActivitySurroundStation surroundStation = dgr.DataContext as ActivitySurroundStation;
                    if (surroundStation != null)
                    {
                        StationDetailDialog dialog = new StationDetailDialog(surroundStation.STATGUID);
                        dialog.ShowDialog(this);
                    }
                }
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            checkBoxAll = sender as CheckBox;
        }

        private void menuItemRestore_Click(object sender, RoutedEventArgs e)
        {
            var stationEmitInfo = (sender as FrameworkElement).DataContext as StationEmitInfo;
            if (stationEmitInfo != null)
            {
                var oldResult = stationEmitInfo.ClearResult;
                stationEmitInfo.ClearResult = ClearResultEnum.NotClear;
                try
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation>(channel =>
                    {
                        List<StationEmitInfo> list = new List<StationEmitInfo>();
                        list.Add(stationEmitInfo);
                        channel.SaveStationEmits(list);
                    });
                }
                catch(Exception ex)
                {
                    stationEmitInfo.ClearResult = oldResult;
                    MessageBox.Show(ex.Message,"恢复清理信息错误");
                }
            }
        }

        private void menuItemClear_Click(object sender, RoutedEventArgs e)
        {
            var stationEmitInfo = (sender as FrameworkElement).DataContext as StationEmitInfo;
            if (stationEmitInfo != null)
            {
                var oldResult = stationEmitInfo.ClearResult;
                stationEmitInfo.ClearResult = ClearResultEnum.ClearSucceed;
                try
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation>(channel =>
                    {
                        List<StationEmitInfo> list = new List<StationEmitInfo>();
                        list.Add(stationEmitInfo);
                        channel.SaveStationEmits(list);
                    });
                }
                catch (Exception ex)
                {
                    stationEmitInfo.ClearResult = oldResult;
                    MessageBox.Show(ex.Message, "保存清理信息错误");
                }
            }
        }

        private void buttonAddClear_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentPlace != null)
            {
                var clearableSurroundList = this.GetClearableSurroundStationList();
                if (clearableSurroundList.Count > 0)
                {
                    FreqClearSettingWindow wnd = new FreqClearSettingWindow();
                    wnd.DataContext = clearableSurroundList;
                    if (wnd.ShowDialog(this) == true)
                    {
                        this.LoadStationClearResult();
                    }
                }
            }
        }

        private List<ActivitySurroundStation> GetClearableSurroundStationList()
        {
            var surroundStationList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation, List<ActivitySurroundStation>>(channel =>
            {
                return channel.GetActivitySurroundStations(this.currentPlace.ActivityGuid, this.currentPlace.Guid, null);
            });
            if (surroundStationList != null)
            {
                foreach (var station in surroundStationList)
                {
                    if (station.EmitInfo != null)
                    {
                        station.EmitInfo = (from emitInfo in station.EmitInfo where emitInfo.NeedClear== NeedClearEunm.NotNeedClear select emitInfo).ToList();
                    }
                }
                return (from station in surroundStationList where station.EmitInfo != null && station.EmitInfo.Count > 0 select station).ToList();
            }
            return new List<ActivitySurroundStation>();
        }
    }

    public class EmitFreqClearResultImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ClearResultEnum)
            {
                var clearResult = (ClearResultEnum)value;
                switch (clearResult)
                {
                    case ClearResultEnum.NotClear:
                        return "/CO_IA.UI.FreqStation;component/Images/UnCheck.png";
                    case ClearResultEnum.ClearSucceed:
                        return "/CO_IA.UI.FreqStation;component/Images/Qualified.png";
                    case Data.ClearResultEnum.ClearFail:
                        return "/CO_IA.UI.FreqStation;component/Images/UnQualified.png";
                }
            }
            return "/CO_IA.UI.FreqStation;component/Images/UnCheck.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RestoreMenuVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ClearResultEnum)
            {
                var clearResult = (ClearResultEnum)value;
                if (clearResult == ClearResultEnum.ClearSucceed)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ClearMenuVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ClearResultEnum)
            {
                var clearResult = (ClearResultEnum)value;
                if (clearResult != ClearResultEnum.ClearSucceed)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
