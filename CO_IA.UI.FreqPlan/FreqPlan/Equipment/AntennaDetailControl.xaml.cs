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

namespace CO_IA.UI.FreqPlan
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
    }
}
