using CO_IA.Data;
using EMCS.Types;
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

namespace CO_IA.UI.Setting.Equipment
{
    /// <summary>
    /// EquipmentDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentDetailControl : UserControl
    {
        public EquipmentInfo CurrentEquipmentInfo
        {
            get { return (EquipmentInfo)GetValue(CurrentEquipmentInfoProperty); }
            set { SetValue(CurrentEquipmentInfoProperty, value); }
        }

        public static readonly DependencyProperty CurrentEquipmentInfoProperty =
            DependencyProperty.Register("CurrentEquipmentInfo", typeof(EquipmentInfo), typeof(EquipmentDetailControl), new PropertyMetadata(new PropertyChangedCallback(EquipmentInfochanged)));

        private static void EquipmentInfochanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public EquipmentInfo OriginalEquipment { get; set; }

        private static void OriginalEquipmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as EquipmentDetailControl).CurrentEquipmentInfo = SettingHelper.Clone<EquipmentInfo>(e.NewValue as EquipmentInfo);
        }

        public EquipmentDetailControl()
        {
            InitializeComponent();
            this.DataContextChanged += EquipmentDetailControl_DataContextChanged;
            List<EMCModulationEnum> modulation = new List<EMCModulationEnum>();
            foreach (string item in Enum.GetNames(typeof(EMCModulationEnum)))
            {
                EMCModulationEnum modulate = new EMCModulationEnum();
                if (Enum.TryParse(item, out modulate))
                {
                    modulation.Add(modulate);
                }
            }
            combModulate.ItemsSource = modulation;
            comboxBusinesstype.ItemsSource = CO_IA.Client.Utility.BusinessTypes;
        }

        private void EquipmentDetailControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CurrentEquipmentInfo = e.NewValue as EquipmentInfo;
            OriginalEquipment = SettingHelper.Clone<EquipmentInfo>(OriginalEquipment);
        }

        private void ChkSendPara_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox chksend = sender as CheckBox;
            if (CurrentEquipmentInfo.SendPara == null)
            {
                chksend.IsChecked = false;
            }
            else
            {
                chksend.IsChecked = true;
            }
        }

        private void ChkSendPara_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chksend = sender as CheckBox;
            if (chksend.IsChecked.Value)
            {
                CurrentEquipmentInfo.SendPara = new SendParameter();
            }
            else
            {
                CurrentEquipmentInfo.SendPara = null;
            }
        }

        private void ChkRecivePara_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox chkrecive = sender as CheckBox;
            if (CurrentEquipmentInfo.RecivePara == null)
            {
                chkrecive.IsChecked = false;
            }
            else
            {
                chkrecive.IsChecked = true;
            }
        }

        private void ChkRecivePara_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkrecive = sender as CheckBox;
            if (chkrecive.IsChecked.Value)
            {
                CurrentEquipmentInfo.RecivePara = new ReceiveParameter();
            }
            else
            {
                CurrentEquipmentInfo.RecivePara = null;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("[^0-9.]");
            e.Handled = re.IsMatch(e.Text);
        }

        private void EQUCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("[^0-9]");
            e.Handled = re.IsMatch(e.Text);
        }
    }
}
