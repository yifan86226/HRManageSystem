using AT_BC.Common;
using AT_BC.Data;
using CO_IA.Data;
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


namespace CO_IA.UI.Setting
{
    /// <summary>
    /// LocationFreqPlanControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentClassFreqPlanningSelectWindow : AT_BC.Common.CheckableWindow
    {
        public EquipmentClassFreqPlanningSelectWindow()
        {
            InitializeComponent();
        }

        private CheckBox checkBoxAll;
        private void checkBoxAll_Loaded(object sender, RoutedEventArgs e)
        {
            this.checkBoxAll=sender as CheckBox;
        }

        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckableCheckBoxHelper.Checked(sender, e, this.checkBoxAll);
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public EquipmentClassFreqRange[] GetCheckedFreqPlans()
        {
            var dataList=this.DataContext as IEnumerable<EquipmentClassFreqRange>;
            if (dataList != null)
            {
                return dataList.GetCheckedItems().ToArray();
            }
            return new EquipmentClassFreqRange[0];
        }
    }
}
