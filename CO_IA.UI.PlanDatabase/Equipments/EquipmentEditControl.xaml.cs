using AT_BC.Client.Extensions;
using AT_BC.Data;
using CO_IA.Data;
using CO_IA.UI.MAP;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace CO_IA.UI.PlanDatabase.Equipments
{
    /// <summary>
    /// EqiupmentEditControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentEditControl : EditableUserControl, INotifyPropertyChanged
    {
        private Equipment currentequipment;
        private Range<double?> FreqRange = new Range<double?>();

        public Equipment CurrentEquipment
        {
            get
            {
                return currentequipment;
            }
            set
            {
                currentequipment = value;
                NotifyPropertyChanged("CurrentEquipment");
            }
        }

        public EquipmentEditControl()
        {
            InitializeComponent();
            this.DataContextChanged += EquipmentEditControl_DataContextChanged;
            comboxModulation.ItemsSource = Enum.GetValues(typeof(EMCS.Types.EMCModulationEnum));
            comboBoxClass.ItemsSource = CO_IA.Client.Utility.EquipmentClasses;
        }

        private void EquipmentEditControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.CurrentEquipment = this.DataContext as Equipment;
            this.FreqRange = this.CurrentEquipment.FreqRange;

            if (this.CurrentEquipment.IsMobile) //移动设备
            {
                rabmobile.IsChecked = true;
                rabfixed.IsChecked = false;
            }
            else
            {
                rabmobile.IsChecked = false;
                rabfixed.IsChecked = true;
            }

            if (this.CurrentEquipment.IsTunable)
            {
                ckboxIsTunable.IsChecked = true;
                textBoxFreqRangeFrom.IsReadOnly = false;
                textBoxFreqRangeTo.IsReadOnly = false;
            }
            else
            {
                ckboxIsTunable.IsChecked = false;
                textBoxFreqRangeFrom.IsReadOnly = true;
                textBoxFreqRangeTo.IsReadOnly = true;
            }
        }

        protected override bool UpdateIsReadOnly(DependencyObject obj, bool newValue)
        {
            if (!newValue)
            {
                if (obj == textBoxFreqRangeFrom || obj == textBoxFreqRangeTo)
                {
                    return true;
                }
            }

            if (obj == this.textBlockOrgName)
            {
                return true;
            }
            if (obj == this.groupBoxStation)
            {
                System.Windows.Controls.CheckBox chkbox = groupBoxStation.Header as System.Windows.Controls.CheckBox;
                if (chkbox != null)
                    chkbox.IsEnabled = !newValue;
            }
            if (obj == earthBtn)
            {
                earthBtn.IsEnabled = !newValue;
            }

            return base.UpdateIsReadOnly(obj, newValue);
        }

        /// <summary>
        /// 在地图上选取坐标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WinMapCoordinate winMap = new WinMapCoordinate(!IsReadOnly);
            winMap.ShowAreaGeo = false;
            winMap.mapcoor.selectPoint = new Point()
            {
                X = CurrentEquipment.Longitude == null ? 0 : CurrentEquipment.Longitude.Value,
                Y = CurrentEquipment.Latitude == null ? 0 : CurrentEquipment.Latitude.Value,
            };

            if (winMap.ShowDialog() == true)
            {
                if (IsReadOnly)
                    return;
                CurrentEquipment.Longitude = winMap.mapcoor.selectPoint.X;
                CurrentEquipment.Latitude = winMap.mapcoor.selectPoint.Y;
            }
        }

        private void ckboxIsStation_Unchecked(object sender, RoutedEventArgs e)
        {
            if (CurrentEquipment != null)
            {
                CurrentEquipment.StationName = string.Empty;
                CurrentEquipment.StationTDI = string.Empty;
            }
        }

        private void ckboxIsTunable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (CurrentEquipment != null)
            {
                CurrentEquipment.IsTunable = false;
                CurrentEquipment.FreqRange = null;
                textBoxFreqRangeFrom.IsReadOnly = true;
                textBoxFreqRangeTo.IsReadOnly = true;
            }
        }

        private void ckboxIsTunable_Checked(object sender, RoutedEventArgs e)
        {
            if (CurrentEquipment != null)
            {
                CurrentEquipment.IsTunable = true;
                CurrentEquipment.FreqRange = this.FreqRange;
                textBoxFreqRangeFrom.IsReadOnly = false;
                textBoxFreqRangeTo.IsReadOnly = false;
            }
        }

        /// <summary>
        /// 移动设备Checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rabmobile_Checked(object sender, RoutedEventArgs e)
        {
            if (CurrentEquipment != null)
            {
                CurrentEquipment.Longitude = null;
                CurrentEquipment.Latitude = null;
                CurrentEquipment.Address = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string proname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(proname));
            }
        }

    }
}
