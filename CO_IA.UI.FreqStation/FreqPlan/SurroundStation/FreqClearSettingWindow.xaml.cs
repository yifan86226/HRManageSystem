using CO_IA_Data;
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

namespace CO_IA.UI.FreqStation.FreqPlan.SurroundStation
{
    /// <summary>
    /// FreqClearSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FreqClearSettingWindow : Window
    {
        public FreqClearSettingWindow()
        {
            InitializeComponent();
            this.DataContextChanged += FreqClearSettingWindow_DataContextChanged;
        }

        private void FreqClearSettingWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            IEnumerable<ActivitySurroundStation> surroundStations = e.NewValue as IEnumerable<ActivitySurroundStation>;
            if (surroundStations != null)
            {
                this.dataGridSurroundStation.ItemsSource = surroundStations;
            }
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<ActivitySurroundStation> surroundStations = this.DataContext as IEnumerable<ActivitySurroundStation>;
            if (surroundStations != null)
            {
                List<StationEmitInfo> list=new List<StationEmitInfo>();
                foreach (var station in surroundStations)
                {
                    if (station.EmitInfo != null)
                    {
                        foreach (var emitInfo in station.EmitInfo)
                        {
                            if (emitInfo.NeedClear == Data.NeedClearEunm.NeedClear)
                            {
                                emitInfo.ClearResult = Data.ClearResultEnum.ClearSucceed;
                                list.Add(emitInfo);
                            }
                        }
                    }
                }
                if (list.Count > 0)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation>(channel =>
                        {
                            channel.SaveStationEmits(list);
                        });
                }
            }
            this.DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void dataGridSurroundStation_LayoutUpdated(object sender, EventArgs e)
        {
            this.dataGridSurroundStation.RowHeight = Double.NaN;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridSurroundStation_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (SelectedActivitySurroundStation != null)
            //{
            //    StationDetailDialog dialog = new StationDetailDialog(SelectedActivitySurroundStation.STATGUID);
            //    dialog.ShowDialog(this);
            //}
        }
    }
}
