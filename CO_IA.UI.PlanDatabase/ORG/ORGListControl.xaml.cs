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

namespace CO_IA.UI.PlanDatabase.ORG
{
    /// <summary>
    /// ORGListControl.xaml 的交互逻辑
    /// </summary>
    public partial class ORGListControl : UserControl
    {
        public event Action<Organization> SelectionChanged;

        public Organization SelectedORG
        {
            get { return orgdatagrid.SelectedItem as Organization; }
            set
            {
                orgdatagrid.SelectedItem = value;
            }
        }

        public Organization[] ORGItemsSource
        {
            get;
            set;
        }

        public ORGListControl()
        {
            InitializeComponent();
            this.DataContextChanged += ORGListControl_DataContextChanged;
        }

        private void ORGListControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ORGItemsSource = this.DataContext as Organization[];
            if (ORGItemsSource != null && ORGItemsSource.Length > 0)
            {
                orgdatagrid.SelectedIndex = 0;
            }
        }

        private void orgdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedORG != null)
            {
                if (SelectionChanged != null)
                {
                    SelectionChanged(SelectedORG);
                }
            }
        }
    }
}
