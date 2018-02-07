using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Best.VWPlatform.Controls.Common
{
    public class TextEditWithUnit : TextEdit
    {
        private string _unitText;

        static TextEditWithUnit()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextEditWithUnit), new FrameworkPropertyMetadata(typeof(TextEditWithUnit)));
        }

        public TextEditWithUnit()
        {
            //this.DefaultStyleKey = typeof(TextEditWithUnit);
            MaxValue = double.MaxValue;
            MinValue = double.MinValue;
        }

        #region 属性

        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(TextEditWithUnit), new PropertyMetadata(double.NaN));


        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(TextEditWithUnit), new PropertyMetadata(0.00));


        /// <summary>
        /// 单位
        /// </summary>
        public string UnitText
        {
            get { return (string)GetValue(UnitTextProperty); }
            set { SetValue(UnitTextProperty, value); }
        }

        public static readonly DependencyProperty UnitTextProperty =
            DependencyProperty.Register("UnitText", typeof(string), typeof(TextEditWithUnit), new PropertyMetadata("Unit", UnitTextPropertyCallback));

        #endregion

        #region 内部方法

        private static void UnitTextPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var edit = (TextEditWithUnit)d;

            if (e.NewValue != null)
            {
                edit._unitText = e.NewValue.ToString();

                if (edit._tbUint != null) edit._tbUint.Text = edit._unitText;
            }
        }

        void TextEditWithUnit_Validate(object sender, ValidationEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value.ToString()))
                return;

            double v = 0.0;
            if (e.Value.ToString() != ".") 
                v = double.Parse(e.Value.ToString());

            e.ErrorContent = string.Format("值超出了范围（{0}-{1}）", MinValue, MaxValue);
            e.IsValid = !(v < MinValue || v > MaxValue);
        }

        #endregion

        #region 重写

        private TextBlock _tbUint;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _tbUint = GetTemplateChild("x_tbUint") as TextBlock;
            if (_tbUint != null && _unitText != null) _tbUint.Text = _unitText;

            this.Validate -= TextEditWithUnit_Validate;
            this.Validate += TextEditWithUnit_Validate;
        }

        #endregion

        #region 内部方法

        #endregion
    }

    public class TextBoxHelper
    {
        public static readonly DependencyProperty AutoSelectAllProperty =
            DependencyProperty.RegisterAttached("AutoSelectAll", typeof(bool), typeof(TextBoxHelper),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnAutoSelectAllChanged)));

        public static bool GetAutoSelectAll(TextBoxBase d)
        {
            return (bool)d.GetValue(AutoSelectAllProperty);
        }

        public static void SetAutoSelectAll(TextBoxBase d, bool value)
        {
            d.SetValue(AutoSelectAllProperty, value);
        }

        private static void OnAutoSelectAllChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBoxBase;
            if (textBox != null)
            {
                var flag = (bool)e.NewValue;
                if (flag)
                {
                    textBox.GotFocus += TextBoxOnGotFocus;
                    textBox.MouseDoubleClick += textBox_MouseDoubleClick;
                    
                }
                else
                {
                    textBox.MouseDoubleClick += textBox_MouseDoubleClick;
                    textBox.GotFocus -= TextBoxOnGotFocus;
                }
            }
        }

        static void textBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var textBox = sender as TextBoxBase;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        private static void TextBoxOnGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBoxBase;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }
    }
}
