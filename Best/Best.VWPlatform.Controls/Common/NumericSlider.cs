using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls.Common
{
    public class SurfaceSlider : Slider
    {
        static SurfaceSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SurfaceSlider), new FrameworkPropertyMetadata(typeof(SurfaceSlider)));
        }
    }

    public class NumericSlider : Slider
    {
        static NumericSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericSlider), new FrameworkPropertyMetadata(typeof(NumericSlider)));

            MinimumProperty.OverrideMetadata(typeof(NumericSlider), new FrameworkPropertyMetadata(MinOrMaxPropertyChanged));
            MaximumProperty.OverrideMetadata(typeof(NumericSlider), new FrameworkPropertyMetadata(MinOrMaxPropertyChanged));
        }

        private static void MinOrMaxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = d as NumericSlider;
            if (slider != null)
            {
                slider.SetArrowPostion();
            }
        }

        public string UnitText
        {
            get { return (string)GetValue(UnitTextProperty); }
            set { SetValue(UnitTextProperty, value); }
        }

        public static readonly DependencyProperty UnitTextProperty =
            DependencyProperty.Register("UnitText", typeof(string), typeof(NumericSlider), new PropertyMetadata(""));

        private Grid _valueTip;
        private NumericTextBox _tipTextBox;
        private Thumb _thumb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _valueTip = GetTemplateChild("x_valueTip") as Grid;
            _tipTextBox = GetTemplateChild("x_tipTextBox") as NumericTextBox;
            _thumb = GetTemplateChild("HorizontalThumb") as Thumb;

            if (_valueTip == null || _tipTextBox == null)
            {
                return;
            }

            _tipTextBox.TextChanged += _tipTextBox_TextChanged;
            _tipTextBox.MouseEnter += _tipTextBox_MouseEnter;
            _tipTextBox.MouseLeave += _tipTextBox_MouseLeave;
            _tipTextBox.GotFocus += _tipTextBox_GotFocus;
            _tipTextBox.LostFocus += _tipTextBox_LostFocus;

            Binding binding = new Binding("Value"){Source = this};
            binding.Converter = new NumericFormatConverter();
            binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            _tipTextBox.SetBinding(NumericTextBox.TextProperty, binding);

            Binding b1 = new Binding("UnitText"){Source = this};
            _tipTextBox.SetBinding(NumericTextBox.UnitTextProperty, b1);

            this.SizeChanged += NumericSlider_SizeChanged;
        }

        void NumericSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetArrowPostion();
        }

        void _tipTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetArrowPostion();
        }

        void _tipTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            _thumb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B000000"));
            _thumb.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B000000"));
        }

        void _tipTextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            _thumb.Background = Brushes.Black;
            _thumb.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F14B1FB"));
        }

        void _tipTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _thumb.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B000000"));
            _thumb.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B000000"));
            _tipTextBox.MouseLeave += _tipTextBox_MouseLeave;
        }

        void _tipTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _thumb.Background = Brushes.Black;
            _thumb.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F14B1FB"));
            _tipTextBox.MouseLeave -= _tipTextBox_MouseLeave;
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            SetArrowPostion();
        }

        private void SetArrowPostion()
        {
            if (_tipTextBox == null ||
                _valueTip == null)
                return;

            var i = Minimum;
            var j = Maximum;
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
            var pos = this.ActualWidth * (v / j);

            if (_tipTextBox.ActualWidth > this.ActualWidth)
            {
                _tipTextBox.Width = this.ActualWidth;
                _valueTip.HorizontalAlignment = HorizontalAlignment.Right;
                _valueTip.Margin = new Thickness(0, 0, 0, 0);
            }
            else if (pos < _tipTextBox.ActualWidth - 20)
            {
                _tipTextBox.Width = double.NaN;
                _valueTip.HorizontalAlignment = HorizontalAlignment.Left;
                _valueTip.Margin = new Thickness(0, 0, 0, 0);
            }
            else if (pos - 20 + _tipTextBox.ActualWidth >= this.ActualWidth)
            {
                _tipTextBox.Width = double.NaN;
                _valueTip.HorizontalAlignment = HorizontalAlignment.Right;
                _valueTip.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                _tipTextBox.Width = double.NaN;
                _valueTip.HorizontalAlignment = HorizontalAlignment.Left;
                double left = pos - _tipTextBox.ActualWidth / 2;
                if (!double.IsNaN(left))
                    _valueTip.Margin = new Thickness(left, 0, 0, 0);
            }
        }
    }

    public class NumericFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var re = string.Format("{0:N0}", value);
            return re;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class CustomThumb : Thumb
    {
        private TouchDevice currentDevice = null;

        protected override void OnPreviewTouchDown(TouchEventArgs e)
        {
            // Release any previous capture
            ReleaseCurrentDevice();
            // Capture the new touch
            CaptureCurrentDevice(e);
        }

        protected override void OnPreviewTouchUp(TouchEventArgs e)
        {
            ReleaseCurrentDevice();
        }

        protected override void OnLostTouchCapture(TouchEventArgs e)
        {
            // Only re-capture if the reference is not null
            // This way we avoid re-capturing after calling ReleaseCurrentDevice()
            if (currentDevice != null)
            {
                CaptureCurrentDevice(e);
            }
        }

        private void ReleaseCurrentDevice()
        {
            if (currentDevice != null)
            {
                // Set the reference to null so that we don't re-capture in the OnLostTouchCapture() method
                var temp = currentDevice;
                currentDevice = null;
                ReleaseTouchCapture(temp);
            }
        }

        private void CaptureCurrentDevice(TouchEventArgs e)
        {
            bool gotTouch = CaptureTouch(e.TouchDevice);
            if (gotTouch)
            {
                currentDevice = e.TouchDevice;
            }
        }
    }
}
