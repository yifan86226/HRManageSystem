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
using CO_IA.Client;
using AT_BC.Data;

namespace CO_IA.UI.PlanDatabase.Equipments
{
    /// <summary>
    /// EquipmentListControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentListControl : UserControl
    {
        public static List<Organization> OrganizationList = new List<Organization>();
        private List<EquipmentClass> EquClassList = new List<EquipmentClass>();
        public event Action<Equipment> DoubleClick;

        private CheckBox chkAll;

        public Equipment SelectedEquipment
        {
            get
            {
                return equdatagrid.SelectedItem as Equipment;
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

        public Equipment[] EquipmentItemsSource
        {
            get
            {
                return equdatagrid.ItemsSource as Equipment[];
            }
        }

        public EquipmentListControl()
        {
            InitializeComponent();
            this.DataContextChanged += EquipmentListControl_DataContextChanged;
            this.Loaded += EquipmentListControl_Loaded;
        }

        private void EquipmentListControl_Loaded(object sender, RoutedEventArgs e)
        {
            var orgs = CO_IA.Client.Utility.GetORGInfos();
            if (orgs != null)
            {
                OrganizationList.AddRange(orgs);
            }
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
                    Equipment equipment = dgr.DataContext as Equipment;
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
            IEnumerable<Equipment> dataSource = this.DataContext as IEnumerable<Equipment>;
            if (dataSource != null)
            {
                foreach (Equipment result in dataSource)
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
            IEnumerable<Equipment> dataSource = this.DataContext as IEnumerable<Equipment>;
            if (dataSource != null)
            {
                foreach (Equipment result in dataSource)
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
            IEnumerable<Equipment> dataList = this.DataContext as IEnumerable<Equipment>;
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

        public object[,] SourceToObject()
        {
            if (EquipmentItemsSource != null && EquipmentItemsSource.Length > 0)
            {
                object[,] obj = null;
                int rows = EquipmentItemsSource.Length;
                DataGrid datagrid = new DataGrid();

                ObservableCollection<DataGridColumn> columns = equdatagrid.Columns;
                int cols = columns.Count;
                obj = new object[rows + 1, cols];

                //不要全选列
                for (int c = 1; c < cols; c++)
                {
                    DataGridColumn column = columns[c];
                    obj[0, c - 1] = column.Header;
                }
                for (int r = 0; r < rows; r++)
                {
                    Equipment equ = EquipmentItemsSource[r];
                    obj[r + 1, 0] = GetORGName(equ.OrgInfo.Guid);//单位
                    obj[r + 1, 1] = equ.Name;//设备名称
                    obj[r + 1, 2] = GetEquClassName(equ.EquipmentClassID);//业务类型
                    obj[r + 1, 3] = equ.SeriesNumber; //设备编号
                    obj[r + 1, 4] = equ.EQUCount; //设备数量
                    obj[r + 1, 5] = equ.SendFreq;//发射频率
                    obj[r + 1, 6] = equ.ReceiveFreq;//接收频率
                    obj[r + 1, 7] = equ.SpareFreq;//备用频率
                    obj[r + 1, 8] = equ.IsTunable == true ? "是" : "否";//频率可调
                    obj[r + 1, 9] = equ.Band_kHz;//带宽
                    obj[r + 1, 10] = equ.Power_W;//最大功率
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

        private string GetORGName(string orgguid)
        {
            Organization org = OrganizationList.FirstOrDefault(r => r.Guid == orgguid);
            return org.Name;
        }

        private string GetEquClassName(string classguid)
        {
            EquipmentClass equclass = EquClassList.FirstOrDefault(r => r.Guid == classguid);
            if (equclass != null)
            {
                return equclass.Name;
            }
            return null;
        }
    }
}
