using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Bars;

namespace Best.VWPlatform.Controls.Common
{
    public class NumericTextBoxBase : TextBox
    {
        private NumericRangeValidationRule _rule = new NumericRangeValidationRule();

        public NumericTextBoxBase()
        {
            Binding binding = new Binding("NumericValue") { Source = this };
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            binding.ValidationRules.Add(_rule);
            this.SetBinding(TextProperty, binding);

            InputMethod.SetIsInputMethodEnabled(this, false);

            this.Loaded += NumericTextBoxBase_Loaded;
        }

        void NumericTextBoxBase_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }
            else if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToUpper().Equals("DEVENV"))
            {
                return;
            }
#endif
            
            TouchScreenKeyboard.SetIsShowKeyboard(this, ConfigurationManager.AppSettings["IsTouchModel"].Equals("1"));

            PreviewMouseDown += NumericTextBoxBase_PreviewMouseDown;
        }

        public static readonly DependencyProperty IsNullableProperty = DependencyProperty.Register(
            "IsNullable", typeof (bool), typeof (NumericTextBoxBase), new PropertyMetadata(default(bool), IsNullablePropertyChangedCallback));

        private static void IsNullablePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (NumericTextBoxBase)d;
            v._rule.UpdateValidateRules(null, null, (bool)e.NewValue);
        }

        public bool IsNullable
        {
            get { return (bool) GetValue(IsNullableProperty); }
            set { SetValue(IsNullableProperty, value); }
        }

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericTextBoxBase), new PropertyMetadata(double.MaxValue, MaxValueChangedCallback));

        private static void MaxValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (NumericTextBoxBase) d;
            v._rule.UpdateValidateRules((double) e.NewValue, null, null);
        }

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(NumericTextBoxBase), new PropertyMetadata(double.MinValue, MinValueChangedCallback));

        private static void MinValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (NumericTextBoxBase)d;
            v._rule.UpdateValidateRules(null, (double) e.NewValue, null);
        }

        public string NumericValue
        {
            get
            {
                return (string)GetValue(NumericValueProperty);
            }
            set { SetValue(NumericValueProperty, value); }
        }

        public static readonly DependencyProperty NumericValueProperty =
            DependencyProperty.Register("NumericValue", typeof(string), typeof(NumericTextBoxBase), new PropertyMetadata(""));


        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            //屏蔽中文输入和非法字符粘贴输入
            var textBox = e.Source as TextBox;

            if (textBox.Text.Equals("-"))
            {
                return;
            }

            TextChange[] change = new TextChange[e.Changes.Count];
            e.Changes.CopyTo(change, 0);

            double num = MinValue;

            int offset = change[0].Offset;
            if (change[0].AddedLength > 0)
            {
                if (!Double.TryParse(textBox.Text, out num))
                {
                    textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                    textBox.Select(offset, 0);
                }
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.SelectAll();

            this.PreviewMouseDown -= NumericTextBoxBase_PreviewMouseDown;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            this.PreviewMouseDown += NumericTextBoxBase_PreviewMouseDown;
        }

        void NumericTextBoxBase_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
            e.Handled = true;
        }
    }

    public class NumericRangeValidationRule : ValidationRule
    {
        private double _max = double.MaxValue;
        private double _min = double.MinValue;
        private bool _isNullable = false;
        public NumericRangeValidationRule()
        {
        }

        internal void UpdateValidateRules(double? pMax, double? pMin, bool? pIsNullable)
        {
            if (pMax != null)
            {
                _max = (double)pMax;
            }

            if (pMin != null)
            {
                _min = (double) pMin;
            }

            if (pIsNullable != null)
            {
                _isNullable = (bool) pIsNullable;
            }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (_isNullable && string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(true, null);
            }

            double d = 0;
            if (double.TryParse(value.ToString(), out d))
            {
                if (d >= _min && d <= _max)
                {
                    return new ValidationResult(true, null);
                }
            }

            return new ValidationResult(false, "超出" + _min + "-" + _max + "范围!");
        }
    }
}
