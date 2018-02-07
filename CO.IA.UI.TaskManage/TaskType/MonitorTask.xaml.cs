using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace CO.IA.UI.TaskManage.TaskType
{
    /// <summary>
    /// MonitorTask.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorTask : UserControl
    {
        ObservableCollection<listdateData> OListdateData = new ObservableCollection<listdateData>();
        ObservableCollection<listdate> Listdata { get; set; }
        public MonitorTask()
        {
            InitializeComponent();
        }
        protected void BindDatGrid()
        {
            Listdata = new ObservableCollection<listdate>{ 
            new listdate(){ ID=1, Name="150M",Formt="160M",Size="25K",IsChecked=false},
            new listdate(){ ID=2, Name="434.5M",Formt="120M",Size="50K",IsChecked=false},
            new listdate(){ ID=3, Name="1.2G",Formt="1.5G",Size="100M",IsChecked=false},
            new listdate(){ ID=4, Name="520M",Formt="210M",Size="10K",IsChecked=false}
            };
            foreach (var item in Listdata)
            {
                listdateData olist = new listdateData();
                olist.Listdate = item;
                olist.Count = Listdata.Count;
                olist.IsCheckedCount = Listdata.Count(p => p.IsChecked == true);
                OListdateData.Add(olist);
            }
            dgList.DataContext = OListdateData;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void cBox_All_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;
            foreach (var item in OListdateData)
            {
                item.Listdate.IsChecked = cb.IsChecked.Value;
                item.Count = Listdata.Count;
            }
            foreach (var item in OListdateData)
            {
                item.IsCheckedCount = Listdata.Count(p => item.Listdate.IsChecked == true);

            }
        }

        private void DgQuestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgList.SelectedItems.Count > 1)
            {
                foreach (var item in e.AddedItems)
                {
                    if (item is listdateData)
                    {
                        bool isChecked = (item as listdateData).Listdate.IsChecked;
                        (item as listdateData).Listdate.IsChecked = true;
                    }
                }
            }
            else
            {
                if (e.AddedItems.Count == 1)
                {
                    if (e.AddedItems[0] is listdateData)
                    {
                        bool isChecked = (e.AddedItems[0] as listdateData).Listdate.IsChecked;
                        if (isChecked)
                            (e.AddedItems[0] as listdateData).Listdate.IsChecked = false;
                        else
                            (e.AddedItems[0] as listdateData).Listdate.IsChecked = true;
                    }
                }
            }
            foreach (var item in OListdateData)
            {
                item.IsCheckedCount = Listdata.Count(p => item.Listdate.IsChecked == true);

            }


        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            foreach (var itm in OListdateData)
            {
                if (itm.Listdate.IsChecked == true)
                {
                    itm.Listdate.IsChecked = false;
                }
            }

            if (OListdateData.Count == 0)
     
            dgList.DataContext = OListdateData;

        }



        public class listdate : PropertyChangedBase
        {
            private int id;
            private string name;
            private string formt;
            private string size;

            public int ID
            {
                get { return this.id; }
                set { this.id = value; NotifyPropertyChanged("ID"); }
            }
            public string Name
            {
                get { return this.name; }
                set { this.name = value; NotifyPropertyChanged("Name"); }
            }
            public string Formt
            {
                get { return this.formt; }
                set { this.formt = value; NotifyPropertyChanged("Formt"); }
            }
            public string Size
            {
                get { return this.size; }
                set { this.size = value; NotifyPropertyChanged("Size"); }
            }

            public bool _IsChecked;
            /// <summary>
            /// 是否选中
            /// </summary>
            public bool IsChecked
            {
                get { return _IsChecked; }
                set { _IsChecked = value; NotifyPropertyChanged("IsChecked"); }
            }
        }
        public class listdateData : PropertyChangedBase
        {
            private listdate _listdate;
            public listdate Listdate
            {
                get { return _listdate; }
                set { _listdate = value; NotifyPropertyChanged("Listdate"); }
            }
            private int _Count = 0;
            /// <summary>
            /// 总条数
            /// </summary>
            public int Count
            {
                get { return _Count; }
                set { _Count = value; NotifyPropertyChanged("Count"); }
            }
            private int _IsCheckedCount = 0;
            /// <summary>
            ///  当前选中条数
            /// </summary>
            public int IsCheckedCount
            {
                get { return _IsCheckedCount; }
                set { _IsCheckedCount = value; NotifyPropertyChanged("IsCheckedCount"); }
            }
        }

        public class PropertyChangedBase : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

    }
}


