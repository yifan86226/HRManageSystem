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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.FreqPlan.StationPlan
{
    /// <summary>
    /// PlaceListControl.xaml 的交互逻辑
    /// </summary>
    public partial class PlaceListControl : UserControl
    {

        public event Action<int> PlaceSelectionChanged;

        public PlaceListControl()
        {
            InitializeComponent();
            this.listBoxPlace.ItemsSource = new string[] { "西安塔", "创意园" };
        }
        private void listBoxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaceSelectionChanged != null)
            {
                PlaceSelectionChanged(this.listBoxPlace.SelectedIndex);
            }

            //if (borderContent.Visibility == Visibility.Visible)
            //{
            //    if (companyControl != null)
            //    {
            //        int index = listBoxPlace.SelectedIndex;
            //        companyControl.UpdateSource(index);
            //    }
            //}
            //else
            //{
            //    ObservableCollection<EquipmentInfo> collection = new ObservableCollection<EquipmentInfo>();
            //    if (listBoxPlace.SelectedIndex == 0)
            //    {
            //        collection = new ObservableCollection<EquipmentInfo>
            //          (EquipmentItemsSource.Where(r => r.Address == "西安塔").ToList()) { };
            //    }
            //    else
            //    {
            //        collection = new ObservableCollection<EquipmentInfo>(EquipmentItemsSource.Where(r => r.Address == "创意园").ToList()) { };
            //    }
            //    equipmentListControl.EquipmentItemsSource = collection;
            //    //InitData();
            //}
        }
    }
}
