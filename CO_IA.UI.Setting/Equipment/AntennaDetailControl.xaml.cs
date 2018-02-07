using EMCS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// AntennaDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class AntennaDetailControl : UserControl
    {
        public AntennaDetailControl()
        {
            InitializeComponent();
            List<EMCPolarisationEnum> Polars = new List<EMCPolarisationEnum>();
            foreach (string item in Enum.GetNames(typeof(EMCPolarisationEnum)))
            {
                EMCPolarisationEnum polar;
                if (Enum.TryParse(item, out polar))
                {
                    Polars.Add(polar);
                }

            }
            combAntPolar.ItemsSource = Polars;
        }



        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }
        
        //private void TextBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.D0 || e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 ||
        //        e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 ||
        //        e.Key == Key.NumPad0 || e.Key == Key.NumPad1 || e.Key == Key.NumPad2 || e.Key == Key.NumPad3 || e.Key == Key.NumPad4 ||
        //        e.Key == Key.NumPad5 || e.Key == Key.NumPad6 || e.Key == Key.NumPad7 || e.Key == Key.NumPad8 || e.Key == Key.NumPad9 ||
        //        e.Key == Key.OemPeriod || e.Key == Key.Decimal)
        //    {
        //        //如果输入的是.
        //        if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
        //        {
        //            TextBox txt = sender as TextBox;
        //            if (txt.Text.Contains('.'))
        //            {
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        e.Handled = true;
        //    }
        //}
    }
}
