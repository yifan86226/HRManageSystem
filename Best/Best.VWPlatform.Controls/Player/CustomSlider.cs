using System;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Best.VWPlatform.Common.Core;
using Best.VWPlatform.Common.Utility;

namespace Best.VWPlatform.Controls.Player
{
    public class CustomSlider : Slider
    {
        private Thumb _horizontalThumb;
        private FrameworkElement _left;
        private FrameworkElement _right;
        private FrameworkElement _track;
        private TextBlock _textBlock;
        private Border _border;
        private volatile bool _isDragCompleted = true;

        public static readonly DependencyProperty CurrTimeProperty =
            DependencyProperty.Register("CurrTime", typeof(int), typeof(CustomSlider), new PropertyMetadata(default(int)));

        public int CurrTime
        {
            get { return (int)GetValue(CurrTimeProperty); }
            set
            {
                SetValue(CurrTimeProperty, value);
            }
        }

        public static readonly DependencyProperty TotalTimeProperty =
            DependencyProperty.Register("TotalTime", typeof(int), typeof(CustomSlider), new PropertyMetadata(default(int)));

        public int TotalTime
        {
            get { return (int)GetValue(TotalTimeProperty); }
            set { SetValue(TotalTimeProperty, value); }
        }

        public static readonly DependencyProperty ValueChangedCommandProperty =
            DependencyProperty.Register("ValueChangedCommand", typeof(DelegateCommand), typeof(CustomSlider), new PropertyMetadata(default(DelegateCommand)));

        public DelegateCommand ValueChangedCommand
        {
            get { return (DelegateCommand)GetValue(ValueChangedCommandProperty); }
            set { SetValue(ValueChangedCommandProperty, value); }
        }
        static CustomSlider()
        {
            //base.DefaultStyleKey = typeof(CustomSlider);
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomSlider), new FrameworkPropertyMetadata(typeof(CustomSlider)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _horizontalThumb = GetTemplateChild("Thumb") as Thumb;
            _track = GetTemplateChild("PART_Track") as FrameworkElement;
            _left = GetTemplateChild("LeftTrack") as FrameworkElement;
            _right = GetTemplateChild("RightTrack") as FrameworkElement;
            _textBlock = GetTemplateChild("tool") as TextBlock;
            _border = GetTemplateChild("toolBoder") as Border;

            if (_track != null) this.MouseMove += track_MouseMove;
            if (_track != null) this.MouseEnter += track_MouseEnter;
            if (_track != null) this.MouseLeave += CustomSlider_MouseLeave;

            //this.ValueChanged += CustomSlider_ValueChanged;
        }

        void CustomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ValueChangedCommand != null)
            {
                ValueChangedCommand.Execute((int)Value);
            }
            CurrTime = (int)Value;
        }

        public void MovingSliderThumb(int pPos)
        {
            if (_isDragCompleted)
            {
                CurrTime = pPos;
                Value = pPos;
            }
        }

        void CustomSlider_MouseLeave(object sender, MouseEventArgs e)
        {
            _border.Visibility = Visibility.Collapsed;
        }

        void track_MouseEnter(object sender, MouseEventArgs e)
        {
            _border.Visibility = Visibility.Visible;
            Point p = e.GetPosition(_track);
            if (p.X + _border.Width / 2 < _track.ActualWidth)
                _border.Margin = new Thickness(p.X - _border.Width / 2, 0, 0, 0);
            else
                _border.Margin = new Thickness(_track.ActualWidth - _border.Width, 0, 0, 0);
        }

        void track_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragCompleted)
                _border.Visibility = Visibility.Visible;

            Point p = e.GetPosition(_track);
            if (p.X + _border.Width / 2 < _track.ActualWidth)
                _border.Margin = new Thickness(p.X - _border.Width / 2, 0, 0, 0);
            else
                _border.Margin = new Thickness(_track.ActualWidth - _border.Width, 0, 0, 0);

            double s = _textBlock.ActualWidth;

            double scale = p.X / _track.ActualWidth;
            int dynamicValueTime = (int)(scale * TotalTime);
            string dynamicValueTimeStr = Utile.SecondToMinute(dynamicValueTime);
            _textBlock.Text = dynamicValueTimeStr;
        }

        public Storyboard ProgressStoryboard { get { return (GetTemplateChild("ProgressStoryboard") as Storyboard); } }

        public Rectangle ProgressBar { get { return (GetTemplateChild("Progress") as Rectangle); } }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size s = base.ArrangeOverride(finalSize);
            if (_horizontalThumb == null)
                return s;
            if (double.IsNaN(_horizontalThumb.Width) && (_horizontalThumb.ActualWidth != 0))
            {
                _horizontalThumb.Width = _horizontalThumb.ActualWidth;
            }

            if (double.IsNaN(_horizontalThumb.Height) && (_horizontalThumb.ActualHeight != 0))
            {
                _horizontalThumb.Height = _horizontalThumb.ActualHeight;
            }

            if (double.IsNaN(_horizontalThumb.Width)) _horizontalThumb.Width = _horizontalThumb.Height;
            if (double.IsNaN(_horizontalThumb.Height)) _horizontalThumb.Height = _horizontalThumb.Width;

            return (s);
        }

        private ToolTip _autoToolTip;
        private string _autoToolTipFormat;

        public string AutoToolTipFormat
        {
            get { return _autoToolTipFormat; }
            set { _autoToolTipFormat = value; }
        }

        protected override void OnThumbDragStarted(DragStartedEventArgs e)
        {
            base.OnThumbDragStarted(e);
            this.FormatAutoToolTipContent();
            _border.Visibility = Visibility.Collapsed;
            _isDragCompleted = false;
        }

        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            base.OnThumbDragDelta(e);
            this.FormatAutoToolTipContent();
        }

        protected override void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            base.OnThumbDragCompleted(e);
            _isDragCompleted = true;

            if (ValueChangedCommand != null)
            {
                ValueChangedCommand.Execute((int)Value);
            }

            CurrTime = (int)Value;
        }

        private void FormatAutoToolTipContent()
        {
            if (!string.IsNullOrEmpty(this.AutoToolTipFormat))
            {
                this.AutoToolTip.Content = string.Format(
                    this.AutoToolTipFormat,
                    Utile.SecondToMinute((int)Value));
                CurrTime = (int)(Value);
            }
        }

        private ToolTip AutoToolTip
        {
            get
            {
                if (_autoToolTip == null)
                {
                    FieldInfo field = typeof(Slider).GetField(
                        "_autoToolTip",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    _autoToolTip = field.GetValue(this) as ToolTip;
                    _autoToolTip.Style = Application.Current.Resources["x_TimeToolTip"] as Style;
                }
                return _autoToolTip;
            }
        }
    }

}
