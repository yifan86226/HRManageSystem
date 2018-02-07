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

namespace CO_IA.UI.PlanDatabase.Equipments
{
    /// <summary>
    /// EquipmentQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentQueryDialog : Window
    {
        private EquipmentLoadStrategy LoadStrategy;
        private string orgguid = string.Empty;

        public event Action<EquipmentLoadStrategy> OnQueryEvent;

        public bool ORGNameIsReadOnly 
        {
            set { this.orgname.IsReadOnly = value; }
        }

        public EquipmentQueryDialog(EquipmentLoadStrategy loadStrategy)
        {
            InitializeComponent();
            LoadStrategy = loadStrategy;
            this.DataContext = LoadStrategy;
            InitData();
            InitCondition(LoadStrategy);
        }

        private void InitData()
        {
            this.comboBoxClass.ItemsSource = CO_IA.Client.Utility.EquipmentClasses;
        }

        private void InitCondition(EquipmentLoadStrategy loadstrategy)
        {
            this.comboBoxClass.UnselectAllItems();
            if (loadstrategy.EquipClasses != null)
            {
                string[] classes = loadstrategy.EquipClasses;
                foreach (EquipmentClass equclass in comboBoxClass.ItemsSource as EquipmentClass[])
                {
                    if (classes.Contains(equclass.Guid))
                    {
                        comboBoxClass.SelectedItems.Add(equclass);
                    }
                }
            }
        }

        private EquipmentLoadStrategy GetEquipmentLoadStrategy()
        {
            EquipmentLoadStrategy loadstrategy = this.DataContext as EquipmentLoadStrategy;
            loadstrategy.EquipClasses = comboBoxClass.SelectedItems.Select(r => ((EquipmentClass)r).Guid).ToArray();
            return loadstrategy;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            EquipmentLoadStrategy strategy = GetEquipmentLoadStrategy();
            if (OnQueryEvent != null)
            {
                OnQueryEvent(strategy);
                this.Close();
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            EquipmentLoadStrategy loadstrategy = this.DataContext as EquipmentLoadStrategy;
            EquipmentLoadStrategy strategy = new EquipmentLoadStrategy();
            comboBoxClass.UnselectAllItems();
            this.DataContext = strategy;
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
