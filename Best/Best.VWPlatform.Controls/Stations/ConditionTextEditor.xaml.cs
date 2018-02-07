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

namespace Best.VWPlatform.Controls.Stations
{
    /// <summary>
    /// ConditionTextEditor.xaml 的交互逻辑
    /// </summary>
    public partial class ConditionTextEditor : UserControl
    {
        public ConditionTextEditor()
        {
            InitializeComponent();
            DataContext = this;
        }

        public int GetCompareSign()
        {
            return XCb.SelectedIndex;
        }

        public string GetInputValue()
        {
            return XTbValue.Text;
        }

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(ConditionTextEditor), new PropertyMetadata(double.MaxValue));


        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(ConditionTextEditor), new PropertyMetadata(double.MinValue));


        ///// <summary>
        ///// 单位
        ///// </summary>
        public string UnitText
        {
            get { return (string)GetValue(UnitTextProperty); }
            set { SetValue(UnitTextProperty, value); }
        }

        public static readonly DependencyProperty UnitTextProperty =
            DependencyProperty.Register("UnitText", typeof(string), typeof(ConditionTextEditor), new PropertyMetadata(""));

        public bool HasValidationError()
        {
            return Validation.GetHasError(XTbValue);
        }
    }
}
