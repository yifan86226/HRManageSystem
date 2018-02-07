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

namespace Best.VWPlatform.Controls.Common
{
    public class NumericDataGridTextEdit : NumericTextBoxBase
    {
        static NumericDataGridTextEdit()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericDataGridTextEdit), new FrameworkPropertyMetadata(typeof(NumericDataGridTextEdit)));
        }
        ///// <summary>
        ///// 单位
        ///// </summary>
        public string UnitText
        {
            get { return (string)GetValue(UnitTextProperty); }
            set { SetValue(UnitTextProperty, value); }
        }

        public static readonly DependencyProperty UnitTextProperty =
            DependencyProperty.Register("UnitText", typeof(string), typeof(NumericDataGridTextEdit), new PropertyMetadata("", UnitChanged));

        private static void UnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (NumericDataGridTextEdit)d;

            var unit = v.GetTemplateChild("x_tbUint") as TextBlock;
            if (unit != null)
            {
                unit.Text = e.NewValue.ToString();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var unit = GetTemplateChild("x_tbUint") as TextBlock;
            if (unit != null)
            {
                unit.Text = UnitText;
            }
        }
    }
}
