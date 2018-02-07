using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Best.VWPlatform.Common.Utility;

namespace Best.VWPlatform.Controls.Common
{
    [TemplatePart(Name = "x_rect", Type = typeof(Border))]
    [TemplatePart(Name = "x_rectValue", Type = typeof(Rectangle))]
    [TemplatePart(Name = "x_rectMaxValueHistory", Type = typeof(Rectangle))]

    public class NumRangePrompt : Control
    {
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue",
                                                                                                 typeof(double),
                                                                                                 typeof(NumRangePrompt),
                                                                                                 null);
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue",
                                                                                                 typeof(double),
                                                                                                 typeof(NumRangePrompt),
                                                                                                 null);
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
                                                                                                 typeof(double),
                                                                                                 typeof(NumRangePrompt),
                                                                                                 new PropertyMetadata((double)0, new PropertyChangedCallback(OnValuePropertyChangedCallback)));

        public static readonly DependencyProperty MaxValueHistoryVisibilityProperty =
            DependencyProperty.Register("MaxValueHistoryVisibility",
                                        typeof(Visibility), typeof(NumRangePrompt),
                                        new PropertyMetadata(Visibility.Collapsed));

        private Rectangle _xRectValue;
        private Rectangle _xRectMaxValueHistory;
        private double _maxValueHistory;
        private Border _valueRangePanel;

        public NumRangePrompt()
        {
            DefaultStyleKey = typeof(NumRangePrompt);
            SizeChanged += OnNumRangePromptSizeChanged;
        }

        private static void OnValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumRangePrompt prompt = d as NumRangePrompt;
            if (prompt == null)
                return;
            //if ((double)e.NewValue < prompt.MinValue)
            //{
            //    prompt.MinValue = (double)e.NewValue;
            //}
            //if ((double)e.NewValue > prompt.MaxValue)
            //{
            //    prompt.MaxValue = (double)e.NewValue + 10;
            //}
            if ((double)e.NewValue > prompt._maxValueHistory)
            {
                prompt._maxValueHistory = (double)e.NewValue;
                if (prompt._xRectMaxValueHistory != null && prompt._xRectMaxValueHistory.Visibility == Visibility.Collapsed)
                    prompt._xRectMaxValueHistory.Visibility = Visibility.Visible;
            }
            prompt.UpdateValue();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _xRectValue = GetTemplateChild("x_rectValue") as Rectangle;
            _xRectMaxValueHistory = GetTemplateChild("x_rectMaxValueHistory") as Rectangle;
            _valueRangePanel = GetTemplateChild("x_rect") as Border;

            Debug.Assert(_xRectValue != null, "_xRectValue!=null");
            Debug.Assert(_xRectMaxValueHistory != null, "_xRectMaxValueHistory!=null");
            _xRectMaxValueHistory.Visibility = Visibility.Collapsed;
        }

        private void OnNumRangePromptSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Init();
        }

        private void UpdateValue()
        {
            if (_xRectValue == null || _xRectMaxValueHistory == null)
                return;

            int v = (int)WMonitorUtile.ViewToScreen(Value, ValueRangePanelWidth, 0, MaxValue, MinValue);
            _xRectValue.Width = Math.Abs(v);
            int m = (int)WMonitorUtile.ViewToScreen(_maxValueHistory, ValueRangePanelWidth, 0, MaxValue, MinValue);
            Canvas.SetLeft(_xRectMaxValueHistory, Math.Abs(m));
        }

        public void Init()
        {
            if (_xRectValue == null || _xRectMaxValueHistory == null)
                return;
            _xRectValue.Width = 0;
            _xRectMaxValueHistory.Visibility = Visibility.Collapsed;
            _maxValueHistory = -1000;
        }

        private bool _initMinValue;
        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set
            {
                SetValue(MinValueProperty, value);
                if (!_initMinValue)
                {
                    _initMinValue = true;
                    SetValue(ValueProperty, value);
                    _maxValueHistory = value;
                }
            }
        }
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double MaxValueHistory
        {
            get { return _maxValueHistory; }
            set { _maxValueHistory = value; }
        }

        public Orientation Orientation { get; set; }

        public Visibility MaxValueHistoryVisibility
        {
            get { return (Visibility)GetValue(MaxValueHistoryVisibilityProperty); }
            set { SetValue(MaxValueHistoryVisibilityProperty, value); }
        }

        private int ValueRangePanelWidth
        {
            get
            {
                return (int)_valueRangePanel.ActualWidth - 2;
            }
        }

        private int ValueRangePanelHeight
        {
            get { return (int)_valueRangePanel.ActualHeight - 2; }
        }

    }
}
