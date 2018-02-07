using CO_IA_Data;
using I_CO_IA.FreqStation;
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

namespace CO_IA.UI.FreqStation.FreqPlan.SurroundStation
{
    /// <summary>
    /// SurroundStationSelectorDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SurroundStationSelectorDialog  : AT_BC.Common.CheckableWindow
    {
        public event Action<bool> SaveCallbcak;

        public SurroundStationSelectorDialog(ObservableCollection<ActivitySurroundStation> stations)
        {
            InitializeComponent();
            this.DataContext = stations;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (surroundStationListControl.SelectedSurroundStation != null)
            {
                List<ActivitySurroundStation> selectstation = surroundStationListControl.SelectedSurroundStation.ToList();
                bool saveresult = true;
                if (selectstation.Count == 0)
                {
                    MessageBox.Show("请选择要保存的周围台站");
                }
                else
                {
                    try
                    {
                        busyIndicator.IsBusy = true;
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                        {
                            channel.SaveActivitySurroundStation(selectstation);
                        });
                        busyIndicator.IsBusy = false;
                        saveresult = true;
                        MessageBox.Show("保存成功");
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        busyIndicator.IsBusy = false;
                        saveresult = false;
                        MessageBox.Show(ex.GetExceptionMessage(), "保存失败");
                    }
                    if (SaveCallbcak != null)
                    {
                        SaveCallbcak(saveresult);
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
