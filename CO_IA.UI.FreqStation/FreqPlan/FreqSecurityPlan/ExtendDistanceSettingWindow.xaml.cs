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

namespace CO_IA.UI.FreqStation.FreqPlan
{
    /// <summary>
    /// DialogExtendDistance.xaml 的交互逻辑
    /// </summary>
    public partial class ExtendDistanceSettingWindow : Window
    {
        public event Action<double> OnSetDistance;
        public ExtendDistanceSettingWindow()
        {
            InitializeComponent();
        }

        private const string distanceTextPattern = @"^[1-9]\d{0,5}$";

        private void textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("[^0-9.]+");
            //System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"^[1-9]\d{0,5}$");
            string str;
            var textbox = sender as TextBox;
            str = textbox.Text.Substring(0, textbox.SelectionStart);
            str += e.Text;
            str += textbox.Text.Substring(textbox.SelectionStart + textbox.SelectionLength);
            e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(str, distanceTextPattern);
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            int distance = 0;
            if (!int.TryParse(this.xtextDistance.Text, out distance))
            {
                MessageBox.Show("输入不是有效的距离!");
                return;
            }
            this.DialogResult = true;
            //if (OnSetDistance != null)
            //{
            //    double distance = 0;
            //    double.TryParse(this.xtextDistance.Text, out distance);
            //    OnSetDistance(distance);
            //}
            //Close();
        }

        public int mDistance
        {
            get
            {
                int distance = 0;
                int.TryParse(this.xtextDistance.Text, out distance);
                return distance;
            }
            set
            {
                this.xtextDistance.Text = value.ToString();
            }
        }
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
