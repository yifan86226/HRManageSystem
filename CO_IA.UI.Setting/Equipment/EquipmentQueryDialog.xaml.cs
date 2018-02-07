using CO_IA.Client;
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
using System.Windows.Shapes;

namespace CO_IA.UI.Setting.Equipment
{
    /// <summary>
    /// EquipmentQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentQueryDialog : Window
    {
        //private EquipmentQueryCondition QueryCondition;

        public EquipmentQueryCondition QueryCondition
        {
            get { return (EquipmentQueryCondition)GetValue(QueryConditionProperty); }
            set { SetValue(QueryConditionProperty, value); }
        }

        public static readonly DependencyProperty QueryConditionProperty =
            DependencyProperty.Register("QueryCondition", typeof(EquipmentQueryCondition), typeof(EquipmentQueryDialog), new PropertyMetadata(new PropertyChangedCallback(QueryConditionChanged)));

        private static void QueryConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public event Action<EquipmentQueryCondition> OnQueryEvent;

        public EquipmentQueryDialog(EquipmentQueryCondition queryCondition, bool isEnable=false)
        {
            InitializeComponent();
            comboxORG.ItemsSource = Utility.GetORGInfos();
            comboxORG.IsEnabled = isEnable;
            QueryCondition = queryCondition;
            this.DataContext = QueryCondition;
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (OnQueryEvent != null)
            {
                OnQueryEvent(QueryCondition);
            }
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 ||
                e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 ||
                e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 ||
                e.Key == Key.NumPad5 || e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 ||
                e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                //如果输入的是.
                if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
                {
                    TextBox txt = sender as TextBox;
                    if (txt.Text.Contains('.'))
                    {
                        e.Handled = true;
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
