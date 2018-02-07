using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using CO_IA.UI.Collection.Model;

namespace CO_IA.UI.Collection
{
    /// <summary>
    /// CollectionIndexManage.xaml 的交互逻辑
    /// </summary>
    public partial class CollectionIndexManage : Window
    {
        public CollectionDataSave collectionDataSave;
        public SetCollectionLabelDelegate setCollectionLabel;
        public InitComboxDataDelegate initComboxDataDelegate;
        public CollectionIndexManage()
        {
            InitializeComponent();
        }

        public void initGridCtrlData(ObservableCollection<FreqCollectionIndex> data) 
        {
            grid_collectionData.ItemsSource = data;
        }

        private void deleteone_Click(object sender, RoutedEventArgs e)
        {
            FreqCollectionIndex fci = (FreqCollectionIndex)grid_collectionData.GetFocusedRow();
            collectionDataSave.deleteByMeasureId(fci.MeasureID);
            grid_collectionData.ItemsSource = collectionDataSave.getMeasureIds();
            initComboxDataDelegate();
        }

        private void applyone_Click(object sender, RoutedEventArgs e)
        {
            FreqCollectionIndex fci = (FreqCollectionIndex)grid_collectionData.GetFocusedRow();
            setCollectionLabel(fci);
            this.Close();
        }
    }
}
