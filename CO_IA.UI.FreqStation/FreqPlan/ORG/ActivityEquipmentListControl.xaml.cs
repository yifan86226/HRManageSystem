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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.FreqStation.FreqPlan
{
    /// <summary>
    /// ActivityEquipmentListControl.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityEquipmentListControl : UserControl
    {
        public static List<Organization> OrganizationList = new List<Organization>();
        private List<EquipmentClass> EquClassList = new List<EquipmentClass>();
        public event Action<ActivityEquipment> DoubleClick;

        private CheckBox chkAll;

        public ActivityEquipment SelectedEquipment
        {
            get
            {
                return equdatagrid.SelectedItem as ActivityEquipment;
            }
            set
            {
                equdatagrid.SelectedItem = value;
            }
        }
        public bool _showCompany;

        public bool ShowCompany
        {
            get
            {
                return _showCompany;
            }
            set
            {
                _showCompany = value;
                if (_showCompany)
                {
                    columnCompany.Visibility = Visibility.Visible;
                }
                else
                {
                    columnCompany.Visibility = Visibility.Collapsed;
                }
            }
        }

        public ActivityEquipment[] EquipmentItemsSource
        {
            get
            {
                return equdatagrid.ItemsSource as ActivityEquipment[];
            }
        }

        public ActivityEquipmentListControl()
        {
            InitializeComponent();
            this.DataContextChanged += EquipmentListControl_DataContextChanged;
            OrganizationList = CO_IA.Client.Utility.GetORGInfos().ToList();
            EquClassList = CO_IA.Client.Utility.EquipmentClasses.ToList();
        }

        private void EquipmentListControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.chkAll != null)
            {
                this.UpdateCheckAllState();
            }
        }

        private void dataGridRowEquipment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (this.DoubleClick == null)
                {
                    return;
                }
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    ActivityEquipment equipment = dgr.DataContext as ActivityEquipment;
                    if (equipment != null)
                    {
                        this.DoubleClick(equipment);
                    }
                }
            }
        }

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;
            IEnumerable<ActivityEquipment> dataSource = this.DataContext as IEnumerable<ActivityEquipment>;
            if (dataSource != null)
            {
                foreach (ActivityEquipment result in dataSource)
                {
                    result.IsChecked = ischecked;
                }
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            this.UpdateCheckAllState();
        }

        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = (sender as CheckBox).IsChecked;
            if (!isChecked.HasValue)
            {
                return;
            }
            bool checkedState = isChecked.Value;
            IEnumerable<ActivityEquipment> dataSource = this.DataContext as IEnumerable<ActivityEquipment>;
            if (dataSource != null)
            {
                foreach (ActivityEquipment result in dataSource)
                {
                    if (result.IsChecked != checkedState)
                    {
                        this.chkAll.IsChecked = null;
                        return;
                    }
                }
            }

            chkAll.IsChecked = checkedState;
        }

        public void UpdateCheckAllState()
        {
            IEnumerable<ActivityEquipment> dataList = this.DataContext as IEnumerable<ActivityEquipment>;
            if (dataList != null && dataList.Count() > 0)
            {
                var enumerator = dataList.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    bool isChecked = enumerator.Current.IsChecked;
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.IsChecked != isChecked)
                        {
                            this.chkAll.IsChecked = null;
                            return;
                        }
                    }
                    this.chkAll.IsChecked = isChecked;
                }
            }
            else
            {
                this.chkAll.IsChecked = false;
            }
        }

        public object[,] SourceToObject(string activityname ,string placename)
        {
            if (EquipmentItemsSource != null && EquipmentItemsSource.Length > 0)
            {
                object[,] obj = null;
                int rows = EquipmentItemsSource.Length;
                DataGrid datagrid = new DataGrid();

                ObservableCollection<DataGridColumn> columns = equdatagrid.Columns;
                int cols = columns.Count;
                obj = new object[rows + 3, cols];

                obj[0, 0] = "活动名称:";
                obj[0, 1] = activityname;
                obj[1, 0] = "区域名称:";
                obj[1, 1] = placename;

                //不要全选列
                for (int c = 1; c < cols; c++)
                {
                    DataGridColumn column = columns[c];
                    obj[2, c - 1] = column.Header;
                }
                for (int r = 0; r < rows; r++)
                {
                    ActivityEquipment equ = EquipmentItemsSource[r];

                    obj[r + 3, 0] = equ.OrgInfo.Name;//单位
                    obj[r + 3, 1] = equ.Name;//设备名称
                    obj[r + 3, 2] = GetEquClassName(equ.EquipmentClassID);//业务类型
                    obj[r + 3, 3] = equ.EQUCount; //设备数量
                    obj[r + 3, 4] = equ.SendFreq;//发射频率
                    obj[r + 3, 5] = equ.ReceiveFreq;//接收频率
                    obj[r + 3, 6] = equ.IsTunable == true ? "是" : "否";//频率可调
                    obj[r + 3, 7] = equ.Band_kHz;//带宽
                    obj[r + 3, 8] = equ.Power_W;//最大功率
                }
                return obj;
            }
            else
            {
                return null;
            }
        }


        public object[,] EquSourceToObject()
        {
            if (EquipmentItemsSource != null && EquipmentItemsSource.Length > 0)
            {
                Equipment[] equs = EquipmentItemsSource.OrderBy(r => r.Name).ToArray();
                object[,] obj = null;
                int rows = equs.Length;
                int cols = 22;
                obj = new object[rows, cols];

                for (int r = 0; r < rows; r++)
                {
                    Equipment equ = equs[r];
                    obj[r, 0] = equ.Name;//设备名称
                    obj[r, 1] = equ.SeriesNumber;//设备编号
                    obj[r, 2] = GetEquClassName(equ.EquipmentClassID);//业务类型
                    obj[r, 3] = equ.EQUCount; //设备数量
                    obj[r, 4] = equ.EquModel;//设备型号
                    obj[r, 5] = equ.IsMobile == true ? "是" : "否";
                    obj[r, 6] = equ.Longitude;//经度
                    obj[r, 7] = equ.Latitude;//纬度
                    obj[r, 8] = equ.Address;//地点
                    obj[r, 9] = equ.IsStation == true ? "是" : "否";//已建站
                    obj[r, 10] = equ.StationName; //已建站名称
                    obj[r, 11] = equ.StationTDI; //台站编号
                    obj[r, 12] = equ.SendFreq; //发射频率
                    obj[r, 13] = equ.ReceiveFreq; //接收频率
                    obj[r, 14] = equ.SpareFreq; //备用频率
                    obj[r, 15] = equ.IsTunable == true ? "是" : "否"; //已建站名称
                    if (equ.FreqRange != null)
                    {
                        obj[r, 16] = equ.FreqRange.Little; //频率范围起始
                        obj[r, 17] = equ.FreqRange.Great; //频率范围终止
                    }
                    obj[r, 18] = equ.Band_kHz;//波道带宽
                    obj[r, 19] = equ.Power_W;//发射功率
                    obj[r, 20] = equ.Modulation.ToString();//调制方式
                    obj[r, 21] = equ.Remark; //备注
                }
                return obj;
            }
            else
            {
                return null;
            }
        }
        private string GetEquClassName(string classguid)
        {
            EquipmentClass equclass = EquClassList.FirstOrDefault(r => r.Guid == classguid);
            return equclass.Name;
        }
    }
}

