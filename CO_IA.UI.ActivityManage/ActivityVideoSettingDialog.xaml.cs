using CO_IA.Client;
using CO_IA.Data;
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

namespace CO_IA.UI.ActivityManage
{
    /// <summary>
    /// ActivityManageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityVideoSettingDialog : Window
    {
        public ActivityVideoSettingDialog()
        {
            InitializeComponent();
            this.Loaded += ActivityVideoSettingDialog_Loaded;
        }

        private void ActivityVideoSettingDialog_Loaded(object sender, RoutedEventArgs e)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(channel =>
                {
                    this.Places = channel.GetPlaces(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                    var videoSettings=channel.GetVideoSettings(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                    if (videoSettings != null && videoSettings.Length>0)
                    {
                        this.dataGridVideoSetting.ItemsSource = new ObservableCollection<VideoSetting>(videoSettings);
                    }
                    else
                    {
                        this.dataGridVideoSetting.ItemsSource = new ObservableCollection<VideoSetting>(videoSettings);
                    }
                    //this.dataGridVideoSetting.ItemsSource = videoSettings;
                });
        }

        private void dataGridRowVideoSetting_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    var setting = dgr.DataContext as VideoSetting;
                    if (setting != null)
                    {
                        VideoSettingEditDialog dialog = new VideoSettingEditDialog();
                        dialog.EditVideoSetting(setting, this.Places);
                        dialog.ShowDialog(this);
                    }
                }
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            var setting = this.NewVideoSetting();
            VideoSettingEditDialog dialog = new VideoSettingEditDialog();
            dialog.EditVideoSetting(setting, this.Places);
            if (dialog.ShowDialog(this) == true)
            {
                (this.dataGridVideoSetting.ItemsSource as ObservableCollection<VideoSetting>).Add(setting);
            }
        }

        private VideoSetting NewVideoSetting()
        {
            VideoSetting setting = new VideoSetting { ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, Key = Utility.NewGuid() };
            setting.OwnerType = "Place";
            return setting;
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var setting=this.dataGridVideoSetting.SelectedValue as VideoSetting;
            if (setting != null)
            {
                if (MessageBox.Show(this, "确实要删除选中的监测设备吗?", "删除提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(channel =>
                        {
                            channel.DeleteVideoSetting(setting.Key);
                        });
                    (this.dataGridVideoSetting.ItemsSource as ObservableCollection<VideoSetting>).Remove(setting);
                }
            }
            else
            {
                MessageBox.Show("请先选择要删除的监控设备");
            }
        }

        public ActivityPlace[] Places
        {
            get;
            private set;
        }
    }

    public class PlaceNameMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length > 1)
            {
                if (values[1] is IList<ActivityPlace>)
                {
                    IList<ActivityPlace> list = values[1] as IList<ActivityPlace>;
                    string code = values[0] as string;
                    foreach (var place in list)
                    {
                        if (place.Guid == code)
                        {
                            return place.Name;
                        }
                    }
                    return code;
                }
            }
            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }   
}
