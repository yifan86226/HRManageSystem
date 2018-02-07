using CO_IA.Data.PlanDatabase;
using CO_IA.Data.Setting;
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

namespace CO_IA.UI.ActivityManage
{
    /// <summary>
    /// SelectExamPlaceDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectExamPlaceDialog : Window
    {
        public event Action<ExamPlace[]> OnGetDataEvent;

        private ExamCondition _ExamPlace = new ExamCondition();
        private ExamCondition ExamPlaceCondition
        {
            get
            {
                return _ExamPlace;
            }
            set
            {
                _ExamPlace = value;
            }
        }
        public SelectExamPlaceDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            cbarea.ItemsSource = CO_IA.Client.Utility.GetProvinceAreaCode(); //Modify nxt 2017-8-16
            examPlaceDataContext = this.GetExamPlace();
        }
        public SelectExamPlaceDialog(string _examPlaceGuid)
        {
            InitializeComponent();
            this.DataContext = this;
            cbarea.ItemsSource = CO_IA.Client.Utility.GetProvinceAreaCode(); //Modify nxt 2017-8-16
            if (_examPlaceGuid != null && _examPlaceGuid != "")
            {
                this.ExamPlaceCondition.Guid = _examPlaceGuid;
            }
            examPlaceDataContext = this.GetExamPlace();
        }
        private ExamPlace[] GetExamPlace()
        {
            //dataGridExamPlace.ItemsSource = CO_IA.Client.Utility.GetExamPlace();
            ExamPlaceItemsSource = CO_IA.Client.Utility.GetExamPlace(this.ExamPlaceCondition);
            return ExamPlaceItemsSource;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int checkcount = ExamPlaceItemsSource.Count(r => r.IsChecked == true);
            if (checkcount == 0)
            {
                MessageBox.Show("请选择考点", "消息提示", MessageBoxButton.OK);
                return;
            }
            ExamPlace[] selectExamPlaces = ExamPlaceItemsSource.Where(o => o.IsChecked == true).ToArray();
            if (OnGetDataEvent != null)
            {
                OnGetDataEvent(selectExamPlaces);
            }
            this.Close();
        }
        #region 全选
        private CheckBox chkAll;
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            if (ExamPlaceItemsSource != null)
            {
                CheckBox chk = sender as CheckBox;
                bool ischecked = chk.IsChecked.Value;

                foreach (ExamPlace item in ExamPlaceItemsSource)
                {
                    item.IsChecked = ischecked;
                }
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (this.ExamPlaceItemsSource != null)
            {
                chkAll.IsChecked = ExamPlaceItemsSource.Any(item => item.IsChecked);
            }
        }

        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            if (ExamPlaceItemsSource != null)
            {
                int checkcount = ExamPlaceItemsSource.Count(r => r.IsChecked == true);
                if (checkcount == ExamPlaceItemsSource.Length)
                {
                    chkAll.IsChecked = true;
                }
                else if (checkcount == 0)
                {
                    chkAll.IsChecked = false;
                }
                else
                {
                    chkAll.IsChecked = null;
                }
            }
        }
        /// <summary>
        /// 地区选择行改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbarea_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {

            FilterFixedStationItemSource();
        }
        private void FilterFixedStationItemSource()
        {
            IList<object> items = cbarea.SelectedItems as IList<object>;
            List<string> keys = items.Select(r => ((KeyValuePair<string, string>)r).Key).ToList();
            ExamPlaceCondition.AreaCodes = keys;

            GetExamPlace();
        }

        public ExamPlace[] ExamPlaceItemsSource
        {
            get { return (ExamPlace[])GetValue(ExamPlaceItemsSourceProperty); }
            set { SetValue(ExamPlaceItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ExamPlaceItemsSourceProperty =
    DependencyProperty.Register("ExamPlaceItemsSource", typeof(ExamPlace[]), typeof(SelectExamPlaceDialog), new PropertyMetadata(null, null));

        private ExamPlace[] examPlaceDataContext
        {
            get;
            set;
        }
        #endregion
    }
}
