using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Editors;
using System.Globalization;
using System.Windows.Input;

namespace Best.VWPlatform.Controls.Common
{
    /// <summary>
    /// 数值范围编辑器 用DX控件的
    /// </summary>
    public class NumericEditDX : Control
    {
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericEditDX), new PropertyMetadata(1000000.0d, MaxValuePropertyChangedCallback));

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(NumericEditDX), new PropertyMetadata(-10000.000d, MinValuePropertyChangedCallback));

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(NumericEditDX), new PropertyMetadata(string.Empty, UnitPropertyChangedCallback));

        public static readonly DependencyProperty UnitsProperty =
            DependencyProperty.Register("Units", typeof(string), typeof(NumericEditDX), new PropertyMetadata("GHz,MHz,kHz,Hz", UnitsPropertyChangedCallback));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericEditDX), new PropertyMetadata(0.000d, OnValueChanged));

        public static readonly DependencyProperty DecimalsProperty =
            DependencyProperty.Register("Decimals", typeof(int), typeof(NumericEditDX), new PropertyMetadata(3, DecimalsPropertyChangedCallback));

        public static readonly DependencyProperty IsOnlyShowTextProperty =
            DependencyProperty.Register("IsOnlyShowText", typeof(bool), typeof(NumericEditDX), null);

        private bool _reverse;
        private Slider _slider;
        private Storyboard _storyboard1;
        private Storyboard _storyboard2;
        private TextEditWithUnit _textEdit;
        private TextEdit _editTip;
        private Grid _valueTip;
        private ComboBox _comboUnits;
        private bool _isLayoutUpdated;
        private double _validWidth;
        private string _decimalFormat = "{0:0.000}";

        public NumericEditDX()
        {
            DefaultStyleKey = typeof(NumericEditDX);

            LayoutUpdated += delegate
            {
                if (_isLayoutUpdated) return;
                _isLayoutUpdated = true;
                SetArrowPostion();
            };
        }

        private static void MaxValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var edit = (NumericEditDX)d;

            if (edit != null && e.NewValue != null)
            {
                if (edit.InternalTextEdit != null)
                    edit.InternalTextEdit.MaxValue = (double)e.NewValue;
                edit.UpdateEditMask(edit.Decimals, (double)e.NewValue, edit.MinValue);

                edit.SetArrowPostion();
            }
        }

        private static void MinValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var edit = (NumericEditDX)d;

            if (edit != null && e.NewValue != null)
            {
                if (edit.InternalTextEdit != null)
                    edit.InternalTextEdit.MinValue = (double)e.NewValue;
                edit.UpdateEditMask(edit.Decimals, edit.MaxValue, (double)e.NewValue);

                edit.SetArrowPostion();
            }
        }

        private static void DecimalsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var edit = (NumericEditDX)d;

            if (edit != null && e.NewValue != null)
            {
                if ((int)e.NewValue < 0) edit.Decimals = 0;
                edit.UpdateEditMask((int)e.NewValue, edit.MaxValue, edit.MinValue);

                edit.SetArrowPostion();
            }

        }

        private static void UnitsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var edit = (NumericEditDX)d;

            if (edit._comboUnits != null && e.NewValue != null)
            {
                edit._comboUnits.ItemsSource = e.NewValue.ToString().Split(',');

                edit.SetArrowPostion();
            }
        }

        private static void UnitPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var edit = (NumericEditDX)d;

            var unit = string.IsNullOrWhiteSpace(edit.Unit) ? "mhz" : edit.Unit.ToLower();
            switch (unit)
            {
                case "ghz":
                    edit.Decimals = 9;
                    break;
                case "mhz":
                    edit.Decimals = 6;
                    break;
                case "khz":
                    edit.Decimals = 3;
                    break;
                case "hz":
                    edit.Decimals = 0;
                    break;
            }

            if (edit.UnitChanged != null)
            {
                var uca = new UnitsChangedArgs(e.OldValue == null ? string.Empty : e.OldValue.ToString(),
                    e.NewValue == null ? string.Empty : e.NewValue.ToString());
                edit.UnitChanged(edit, uca);
                edit.SetArrowPostion();
            }
            else if (e.NewValue != null)
            {
                if (edit._comboUnits != null) edit._comboUnits.SelectedItem = e.NewValue;
                if (edit.InternalTextEdit != null) edit.InternalTextEdit.UnitText = e.NewValue.ToString();

                edit.SetArrowPostion();
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var edit = (NumericEditDX)d;

            if (e.NewValue != null && !e.NewValue.Equals(e.OldValue))
            {
                edit.SetArrowPostion();

                if (edit.ValueChanged != null)
                    edit.ValueChanged((double)e.NewValue);
            }
        }

        #region 重写

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _storyboard1 = GetTemplateChild("x_storyboard1") as Storyboard;
            _storyboard2 = GetTemplateChild("x_storyboard2") as Storyboard;
            InternalTextEdit = GetTemplateChild("x_textBox") as TextEditWithUnit;
            InternalSlider = GetTemplateChild("x_slider") as Slider;
            _comboUnits = GetTemplateChild("x_cbbUnits") as ComboBox;
            _valueTip = GetTemplateChild("x_valueTip") as Grid;
            _editTip = GetTemplateChild("x_tipTextBox") as TextEdit;

            if (_editTip != null)
            {
                _editTip.Validate -= TextEditValidate;
                _editTip.EditValueChanged -= OnEditTipValueChanged;
                _editTip.KeyDown -= OnInternalTextEditKeyDown;

                UpdateEditMask(Decimals, MaxValue, MinValue);

                _editTip.EditValueChanged += OnEditTipValueChanged;
                _editTip.Validate += TextEditValidate;
                _editTip.KeyDown += OnInternalTextEditKeyDown;
            }

            if (_comboUnits != null)
            {
                _comboUnits.ItemsSource = Units.Split(',');
                _comboUnits.SelectedItem = Unit;
                _comboUnits.SelectionChanged += OnComboUnitsSelectionChanged;
            }

            if (InternalSlider != null)
            {
                InternalSlider.ValueChanged += OnInternalSliderValueChanged;
                InternalSlider.MouseLeftButtonUp += OnInternalSliderMouseLeftButtonUp;
            }

            if (InternalTextEdit != null)
            {
                InternalTextEdit.MinValue = MinValue;
                InternalTextEdit.MaxValue = MaxValue;

                UpdateEditMask(Decimals, MaxValue, MinValue);

                InternalTextEdit.EditValueChanged += OnInternalTextEditValueChanged;
            }

            OnlyShowText(IsOnlyShowText);
            SetArrowPostion();
        }

        private void OnInternalTextEditKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && TextEditEnter != null)
            {
                TextEditEnter();
            }
        }

        private void OnInternalSliderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SliderMouseLeftButtonUp != null)
                SliderMouseLeftButtonUp();
        }

        #endregion

        #region 内部方法

        private void SetArrowPostion()
        {
            if (_editTip == null ||
                _slider == null ||
                _valueTip == null ||
                _comboUnits == null ||
                IsOnlyShowText) return;

            var i = MinValue;
            var j = MaxValue;
            var v = Value;
            if (i < 0)
            {
                j += Math.Abs(i);
                v += Math.Abs(i);
            }
            else if (i > 0)
            {
                j -= i;
                v -= i;
            }
            var pos = _slider.ActualWidth * (v / j);

            if ((_editTip.ActualWidth + _comboUnits.ActualWidth + _validWidth) > _slider.ActualWidth)
            {
                var width = _slider.ActualWidth - _comboUnits.ActualWidth - _validWidth;
                _editTip.Width = width > 0 ? width : 0;
                _valueTip.HorizontalAlignment = HorizontalAlignment.Right;
                _valueTip.Margin = new Thickness(0, 0, 0, 0);
            }
            else if (pos < _editTip.ActualWidth + _comboUnits.ActualWidth + _validWidth - 10)
            {
                _editTip.Width = double.NaN;
                _valueTip.HorizontalAlignment = HorizontalAlignment.Left;
                _valueTip.Margin = new Thickness(0, 0, 0, 0);
            }
            else if (pos - 10 + (_editTip.ActualWidth + _comboUnits.ActualWidth + _validWidth) >= _slider.ActualWidth)
            {
                _editTip.Width = double.NaN;
                _valueTip.HorizontalAlignment = HorizontalAlignment.Right;
                _valueTip.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                _editTip.Width = double.NaN;
                _valueTip.HorizontalAlignment = HorizontalAlignment.Left;
                double left = pos - (_editTip.ActualWidth + _comboUnits.ActualWidth + _validWidth) / 2;
                //add by xgz，当left为 NaN值时，构造的 Thickness 无效
                if (!double.IsNaN(left))
                    _valueTip.Margin = new Thickness(left, 0, 0, 0);
            }
        }

        /// <summary>
        /// 更新mask的值
        /// </summary>
        /// <param name="decimals">小数位数</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        private void UpdateEditMask(int decimals, double maxValue, double minValue)
        {
            int minLength = Convert.ToInt32(minValue).ToString(CultureInfo.InvariantCulture).Length;
            int maxLength = Convert.ToInt64(maxValue).ToString(CultureInfo.InvariantCulture).Length;

            if (minValue >= 0)
            {
                if (decimals <= 0)
                {
                    _decimalFormat = "{0:0.}";
                    if (_editTip != null) _editTip.Mask = string.Format("[0-9]{{1,{0}}}", maxLength);
                    if (InternalTextEdit != null) InternalTextEdit.Mask = string.Format("[0-9]{{1,{0}}}", maxLength);
                }
                else
                {
                    string s = "";
                    for (int i = 1; i <= decimals; i++)
                    {
                        s = s + "0";
                    }

                    _decimalFormat = "{0:0." + s + "}";
                    if (_editTip != null) _editTip.Mask = string.Format("[0-9]{{0,{0}}}[.]{{1}}[0-9]{{0,{1}}}", maxLength, decimals);
                    if (InternalTextEdit != null) InternalTextEdit.Mask = string.Format("[0-9]{{0,{0}}}[.]{{1}}[0-9]{{0,{1}}}", maxLength, decimals);
                }
            }
            else if (maxValue <= 0)
            {
                if (decimals <= 0)
                {
                    _decimalFormat = "{0:0.}";
                    if (_editTip != null) _editTip.Mask = string.Format("-?[0-9]{{1,{0}}}", minLength);
                    if (InternalTextEdit != null) InternalTextEdit.Mask = string.Format("-?[0-9]{{1,{0}}}", minLength);
                }
                else
                {
                    string s = "";
                    for (int i = 1; i <= decimals; i++)
                    {
                        s = s + "0";
                    }

                    _decimalFormat = "{0:0." + s + "}";
                    if (_editTip != null) _editTip.Mask = string.Format("-?[0-9]{{0,{0}}}[.]{{1}}[0-9]{{0,{1}}}", minLength, decimals);
                    if (InternalTextEdit != null) InternalTextEdit.Mask = string.Format("-?[0-9]{{0,{0}}}[.]{{1}}[0-9]{{0,{1}}}", minLength, decimals);
                }
            }
            else if (minValue < 0 && maxValue > 0)
            {
                if (decimals <= 0)
                {
                    _decimalFormat = "{0:0.}";
                    if (_editTip != null) _editTip.Mask = string.Format("[0-9]{{1,{0}}}|-[0-9]{{1,{1}}}", maxLength, minLength);
                    if (InternalTextEdit != null) InternalTextEdit.Mask = string.Format("[0-9]{{1,{0}}}|-[0-9]{{1,{1}}}", maxLength, minLength);
                }
                else
                {
                    string s = "";
                    for (int i = 1; i <= decimals; i++)
                    {
                        s = s + "0";
                    }

                    _decimalFormat = "{0:0." + s + "}";
                    if (_editTip != null) _editTip.Mask = string.Format("-[0-9]{{0,{0}}}[.]{{1}}[0-9]{{0,{2}}}|[0-9]{{0,{1}}}[.]{{1}}[0-9]{{0,{2}}}", minLength, maxLength, decimals);
                    if (InternalTextEdit != null) InternalTextEdit.Mask = string.Format("-[0-9]{{0,{0}}}[.]{{1}}[0-9]{{0,{2}}}|[0-9]{{0,{1}}}[.]{{1}}[0-9]{{0,{2}}}", minLength, maxLength, decimals);
                }
            }
        }

        //更新所有和value有关的值
        private void UpdateEditValue(object value)
        {
            //if (!string.IsNullOrEmpty(_decimalFormat) && value != null)
            //{
            //    if (string.IsNullOrEmpty(value.ToString()) || value.ToString() == "." || value.ToString() == "-.") value = 0;

                //string s = string.Format(_decimalFormat, Convert.ToDouble(value));
                //Value = double.Parse(string.Format(_decimalFormat, Convert.ToDouble(value)));
                //if (_editTip != null) _editTip.EditValue = s;
                //if (InternalTextEdit != null)
                //{
                //    InternalTextEdit.EditValue = s;
                //}
           // }
        }

        void OnInternalTextEditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (InternalTextEdit != null && InternalTextEdit.EditValue != null)
            {
                //UpdateEditValue(InternalTextEdit.EditValue);
                SetArrowPostion();
            }
        }

        void OnEditTipValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (_editTip != null && _editTip.EditValue != null)
            {
                //UpdateEditValue(_editTip.EditValue);
                SetArrowPostion();
            }
        }

        void OnInternalSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetArrowPostion();
        }

        void OnComboUnitsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_comboUnits != null) _comboUnits.IsDropDownOpen = false;

            SetArrowPostion();
        }

        private void TextEditValidate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null && !string.IsNullOrEmpty(e.Value.ToString()))
            {
                e.ErrorContent = string.Format("值超出了范围（{0}-{1}）", MinValue, MaxValue);

                double v = 0.0;
                //if (e.Value.ToString() != ".") v = double.Parse(e.Value.ToString());
                if (e.Value.ToString() != "." && e.Value.ToString() != "-.") v = double.Parse(e.Value.ToString());
                e.IsValid = !(v < MinValue || v > MaxValue);
                _validWidth = e.IsValid ? 0 : 5;
                SetArrowPostion();
            }
        }

        /// <summary>
        /// 是否只显示TextEdit控件
        /// </summary>
        /// <param name="isOnlyShowText">为true只显示TextEdit控件</param>
        private void OnlyShowText(bool isOnlyShowText)
        {
            if (InternalSlider != null)
                InternalSlider.Visibility = isOnlyShowText ? Visibility.Collapsed : Visibility.Visible;

            if (InternalTextEdit != null)
                InternalTextEdit.Visibility = isOnlyShowText ? Visibility.Collapsed : Visibility.Visible;

            if (_valueTip != null)
            {
                if (isOnlyShowText)
                {
                    _valueTip.HorizontalAlignment = HorizontalAlignment.Stretch;
                }
                else
                {
                    SetArrowPostion();
                }
            }
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 切换输入面板，文本编辑器和滑块
        /// </summary>
        public void ChangeEditor()
        {
            if (_storyboard1 == null || _storyboard2 == null || IsOnlyShowText)
                return;

            if (_reverse)
            {
                _storyboard2.Begin();
                _reverse = false;
            }
            else
            {
                _storyboard1.Begin();
                _reverse = true;
            }
        }

        #endregion

        #region 属性

        private Slider InternalSlider
        {
            get { return _slider; }
            set
            {
                _slider = value;
                if (_slider != null)
                {
                    _slider.Minimum = MinValue;
                    _slider.Maximum = MaxValue;
                    if (Value < MinValue || Value > MaxValue)
                    {
                        Value = MinValue;
                    }
                }
            }
        }

        private TextEditWithUnit InternalTextEdit
        {
            get { return _textEdit; }
            set
            {
                if (_textEdit != null)
                {
                    _textEdit.Validate -= TextEditValidate;
                }
                _textEdit = value;
                if (_textEdit != null)
                {
                    _textEdit.Validate += TextEditValidate;
                }
            }
        }

        public string Units
        {
            get { return (string)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }

        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        /// <summary>
        /// 值
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 小数位数 
        /// </summary>
        public int Decimals
        {
            get { return (int)GetValue(DecimalsProperty); }
            set { SetValue(DecimalsProperty, value); }
        }

        /// <summary>
        /// 获取或设置允许输入的最小值，默认为零；当MinValue小于零时，本控件的Decimals属性自动禁用，需要设置Mask属性（正则表达式）来控制Value值
        /// </summary>
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set
            {
                if (!string.IsNullOrWhiteSpace(_decimalFormat))
                    SetValue(MinValueProperty, value);

                if (Value < MinValue || Value > MaxValue)
                {
                    Value = MinValue;
                }
                if (InternalSlider != null)
                {
                    InternalSlider.Minimum = MinValue;
                }
            }
        }

        /// <summary>
        /// 获取或设置允许输入的最大值，默认值：1000
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set
            {
                if (!string.IsNullOrWhiteSpace(_decimalFormat))
                    SetValue(MaxValueProperty, value);

                if (Value < MinValue || Value > MaxValue)
                {
                    Value = MinValue;
                }
                if (InternalSlider != null)
                {
                    InternalSlider.Maximum = MaxValue;
                }
            }
        }

        /// <summary>
        /// 是否只显示TextEdit控件
        /// </summary>
        public bool IsOnlyShowText
        {
            get { return (bool)GetValue(IsOnlyShowTextProperty); }
            set
            {
                SetValue(IsOnlyShowTextProperty, value);
                OnlyShowText(value);
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event Action<double> ValueChanged;

        /// <summary>
        /// 单位改变引发该事件
        /// arg1 旧单位 arg2 新单位
        /// obj1
        /// </summary>
        public event Action<object, UnitsChangedArgs> UnitChanged;

        public event Action SliderMouseLeftButtonUp;

        public event Action TextEditEnter;

        #endregion
    }

    public class UnitsChangedArgs : EventArgs
    {
        public UnitsChangedArgs(string oldUnit, string newUnit)
        {
            OldUnit = oldUnit;
            NewUnit = newUnit;
        }
        public string OldUnit { get; set; }

        public string NewUnit { get; set; }
    }
}
