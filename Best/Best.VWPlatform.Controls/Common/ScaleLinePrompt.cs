using Best.VWPlatform.Common.Types;
using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls.Common
{
    /// <summary>
    /// 值范围提示，在特定范围内显示出当前值位置，和历史最大值位置
    /// </summary>
    [TemplatePartAttribute(Name = "x_scales", Type = typeof(Grid))]
    [TemplatePartAttribute(Name = "x_content", Type = typeof(ContentPresenter))]
    [TemplatePartAttribute(Name = "x_rangePanel", Type = typeof(Grid))]
    [ContentProperty("Content")]
    public class ScaleLinePrompt : Control
    {
        #region 变量
        private Grid _rangePanel;
        private ContentPresenter _content;
        private Canvas _scaleCanvas;
        private const uint MaxExtendDec = 8;

        public static readonly DependencyProperty BeginValueProperty =
            DependencyProperty.Register("BeginValue", typeof(double), typeof(ScaleLinePrompt), new PropertyMetadata(ValueChangedCallback));

        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(ScaleLinePrompt), new PropertyMetadata(ValueChangedCallback));

        public static readonly DependencyProperty BeginValueFormatProperty =
            DependencyProperty.Register("BeginValueFormat", typeof(string), typeof(ScaleLinePrompt), new PropertyMetadata("{0}"));

        public static readonly DependencyProperty EndValueFormatProperty =
            DependencyProperty.Register("EndValueFormat", typeof(string), typeof(ScaleLinePrompt), new PropertyMetadata("{0}"));

        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register("ValueFormat", typeof(string), typeof(ScaleLinePrompt), null);

        public static readonly DependencyProperty BeginLabelStyleProperty =
            DependencyProperty.Register("BeginLabelStyle", typeof(Style), typeof(ScaleLinePrompt), null);

        public static readonly DependencyProperty EndLabelStyleProperty =
            DependencyProperty.Register("EndLabelStyle", typeof(Style), typeof(ScaleLinePrompt), null);

        public static readonly DependencyProperty ScaleLineColorProperty = DependencyProperty.Register(
            "ScaleLineColor", typeof(Brush), typeof(ScaleLinePrompt), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(ScaleLabelDirection), typeof(ScaleLinePrompt), null);

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ScaleLinePrompt), null);

        public static readonly DependencyProperty ScaleLabelCountProperty =
            DependencyProperty.Register("ScaleLabelCount", typeof(int), typeof(ScaleLinePrompt), new PropertyMetadata(ValueChangedCallback));

        public static readonly DependencyProperty DefaultDecLengthProperty =
            DependencyProperty.Register("DefaultDecLength", typeof(uint), typeof(ScaleLinePrompt), new PropertyMetadata((uint)0, ValueChangedCallback));

        #endregion

        #region 构造

        static ScaleLinePrompt()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScaleLinePrompt), new FrameworkPropertyMetadata(typeof(ScaleLinePrompt)));
        }
        public ScaleLinePrompt()
        {
            ScaleLabelCount = 5;
            DefaultDecLength = 2;
        }
        #endregion

        #region 依赖属性值变更回调
        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(e.OldValue))
                return;
            var prompt = d as ScaleLinePrompt;
            if (prompt != null) prompt.Update();
        }
        #endregion

        #region 属性
        public string BeginValueFormat
        {
            get { return (string)GetValue(BeginValueFormatProperty); }
            set { SetValue(BeginValueFormatProperty, value); }
        }

        public string EndValueFormat
        {
            get { return (string)GetValue(EndValueFormatProperty); }
            set { SetValue(EndValueFormatProperty, value); }
        }

        public Style LabelStyle { get; set; }

        public Style BeginLabelStyle
        {
            get { return (Style)GetValue(BeginLabelStyleProperty); }
            set { SetValue(BeginLabelStyleProperty, value); }
        }

        public Style EndLabelStyle
        {
            get { return (Style)GetValue(EndLabelStyleProperty); }
            set { SetValue(EndLabelStyleProperty, value); }
        }

        public Brush ScaleLineColor
        {
            get { return (Brush)GetValue(ScaleLineColorProperty); }
            set { SetValue(ScaleLineColorProperty, value); }
        }

        /// <summary>
        /// 获取或设置范围的开始值
        /// </summary>
        public double BeginValue
        {
            get { return (double)GetValue(BeginValueProperty); }
            set
            {
                SetValue(BeginValueProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置范围的结束值
        /// </summary>
        public double EndValue
        {
            get { return (double)GetValue(EndValueProperty); }
            set
            {
                SetValue(EndValueProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置刻度值格式化字符串
        /// </summary>
        public string ValueFormat
        {
            get { return (string)GetValue(ValueFormatProperty); }
            set
            {
                value = value.Replace("\\", string.Empty);
                SetValue(ValueFormatProperty, value);
            }
        }

        /// <summary>
        /// 值范围刻度值方向
        /// </summary>
        public ScaleLabelDirection Direction
        {
            get { return (ScaleLabelDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// 获取或设置值范围块内容
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// 获取或设置值刻度线段是否与范围块内容部分重叠
        /// </summary>
        public bool OverlapRangeBlock { get; set; }

        /// <summary>
        /// 获取或设置刻度标签数量，最小值3，默认值5
        /// </summary>
        [TypeConverter(typeof(IntTypeConverter))]
        public int ScaleLabelCount
        {
            get { return (int)GetValue(ScaleLabelCountProperty); }
            set
            {
                if (value < 3)
                    return;
                if (ScaleLabelCount == value)
                    return;
                SetValue(ScaleLabelCountProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置默认小数点长度，默认：2，最大8位小数
        /// </summary>
        [TypeConverter(typeof(UIntTypeConverter))]
        public uint DefaultDecLength
        {
            get { return (uint)GetValue(DefaultDecLengthProperty); }
            set
            {
                if (value > MaxExtendDec)
                    value = MaxExtendDec;
                SetValue(DefaultDecLengthProperty, value);
            }
        }

        /// <summary>
        /// 获取或设置自动扩展小数位数，默认：false
        /// </summary>
        /// <remarks>
        /// 如果标尺容器上有重复项时，自动扩充一位小数，最多扩充到8位小数
        /// </remarks>
        public bool AutoExtendDec { get; set; }

        #endregion

        #region 重写
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _content = GetTemplateChild("x_content") as ContentPresenter;
            _rangePanel = GetTemplateChild("x_rangePanel") as Grid;
            _scaleCanvas = GetTemplateChild("xScaleCanvas") as Canvas;

            _rangePanel.SizeChanged -= RangePanelSizeChanged;
            _rangePanel.SizeChanged += RangePanelSizeChanged;

            _rangePanel.Loaded -= _rangePanel_Loaded;
            _rangePanel.Loaded += _rangePanel_Loaded;

            if (_content == null)
                return;
            switch (Direction)
            {
                case ScaleLabelDirection.Left:
                    _rangePanel.RowDefinitions.Clear();
                    _rangePanel.ColumnDefinitions.Clear();
                    _rangePanel.ColumnDefinitions.Add(new ColumnDefinition());
                    _rangePanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) });
                    Grid.SetRow(_content, 0);
                    Grid.SetColumn(_content, 1);
                    Grid.SetColumn(_scaleCanvas, 0);
                    break;
                case ScaleLabelDirection.Top:
                    _rangePanel.RowDefinitions.Clear();
                    _rangePanel.ColumnDefinitions.Clear();
                    _rangePanel.RowDefinitions.Add(new RowDefinition());
                    _rangePanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
                    Grid.SetRow(_content, 1);
                    Grid.SetColumn(_content, 0);
                    Grid.SetRow(_scaleCanvas, 0);
                    break;
                case ScaleLabelDirection.Right:
                    _rangePanel.RowDefinitions.Clear();
                    _rangePanel.ColumnDefinitions.Clear();
                    _rangePanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) });
                    _rangePanel.ColumnDefinitions.Add(new ColumnDefinition());
                    Grid.SetRow(_content, 0);
                    Grid.SetColumn(_content, 0);
                    Grid.SetColumn(_scaleCanvas, 1);
                    break;
                case ScaleLabelDirection.Bottom:
                    _rangePanel.RowDefinitions.Clear();
                    _rangePanel.ColumnDefinitions.Clear();
                    _rangePanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
                    _rangePanel.RowDefinitions.Add(new RowDefinition());
                    Grid.SetRow(_content, 0);
                    Grid.SetColumn(_content, 0);
                    Grid.SetRow(_scaleCanvas, 1);
                    break;
            }
        }

        void _rangePanel_Loaded(object sender, RoutedEventArgs e)
        {
            int index = 0;
            for (int i = 0; i < _scaleCanvas.Children.Count; i++)
            {
                TextBlock tb = _scaleCanvas.Children[i] as TextBlock;
                if (tb == null)
                    continue;

                SetTextBlockProperty(Direction, tb, index, GetUnitSize());
                index++;
            }
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 更新瀑布图纵坐标时间
        /// </summary>
        /// <param name="pTimeFlows"></param>
        internal void UpdateTimeFlowScales(IList<DateTime> pTimeFlows)
        {
            if (_scaleCanvas == null)
                return;

            foreach (var el in _scaleCanvas.Children)
            {
                var tb = el as TextBlock;
                if (tb == null)
                    continue;
                var ts = ((TextBlock)el).Tag as double[];
                if (ts == null)
                    continue;
                int index = 0;
                switch (Direction)
                {
                    case ScaleLabelDirection.Right:
                        index = (int)ts[1];
                        if (index >= 0 && index < pTimeFlows.Count)
                        {
                            tb.Text = pTimeFlows[index].ToString("HH:mm:ss");
                        }
                        break;
                }
            }
        }

        void RangePanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }

        /// <summary>
        /// 布局更新
        /// </summary>
        internal void Update()
        {
            UpdateScales();
        }

        /// <summary>
        /// 刻度更新
        /// </summary>
        private void UpdateScales()
        {
            if (_rangePanel == null)
                return;
            uint defaultDec = DefaultDecLength;
            double vunit = GetUnitValue();
            double unit = GetUnitSize();
            var valStrs = new List<string>();

            if (BeginValue.Equals(EndValue))
            {
                for (int i = 0; i < ScaleLabelCount; i++)
                {
                    valStrs.Add("--:--:--");
                }
                UpdateScales2(Direction, valStrs, unit);
                return;
            }
            if (vunit.Equals(0) || unit.Equals(0))
                return;
        start:
            valStrs.Clear();

            for (int i = 0; i < ScaleLabelCount; i++)
            {
                double dv = Utile.MathNoRound(BeginValue + i * vunit, defaultDec);
                if (double.IsNaN(dv))
                    return;
                string v;
                if (string.IsNullOrWhiteSpace(ValueFormat))
                {
                    v = string.Format("{0:N" + defaultDec + "}", dv);
                    if (ScaleConvertValue != null)
                    {
                        var args = new ScaleValueConverterArgs(i, defaultDec, v);
                        ScaleConvertValue(args);
                        v = args.NewValue != null ? args.NewValue.ToString() : string.Empty;
                    }
                }
                else
                {
                    v = string.Format(ValueFormat, dv);
                }

                if (AutoExtendDec && defaultDec < MaxExtendDec)
                {
                    foreach (var obj in valStrs)
                    {
                        if (obj.StartsWith(v))
                        {
                            defaultDec += 1;
                            goto start;
                        }
                    }
                }
                valStrs.Add(v);
            }

            UpdateScales2(Direction, valStrs, unit);
        }

        private void UpdateScales2(ScaleLabelDirection pDirection, IList<string> valStrs, double unit)
        {
            if (_scaleCanvas.Children.Count == 0)
            {
                for (int i = 0; i < ScaleLabelCount; i++)
                {
                    var tb = new TextBlock { Text = valStrs[i], FontSize = 11};
                    _scaleCanvas.Children.Add(tb);

                    SetLineProperty(pDirection, i, unit);
                }               
            }
            else //因为wpf中控件没有render获取不到它的宽和高，所以每次刻度长度变化时有个小bug @?
            {
                var tl = new  List<TextBlock>();

                for (int i = 0; i < _scaleCanvas.Children.Count; i++)
                {
                    TextBlock tb = _scaleCanvas.Children[i] as TextBlock;
                    if (tb != null)
                    {
                        tl.Add(tb);
                    }
                }

                _scaleCanvas.Children.Clear();

                for (int i = 0; i < ScaleLabelCount; i++)
                {
                    tl[i].Text = valStrs[i];
                    _scaleCanvas.Children.Add(tl[i]);

                    SetLineProperty(pDirection, i, unit);
                    SetTextBlockProperty(Direction, tl[i], i, GetUnitSize());
                }
            }
        }

        /// <summary>
        /// 添加刻度的线
        /// </summary>
        /// <param name="pDirection"></param>
        /// <param name="i"></param>
        /// <param name="unit"></param>
        private void SetLineProperty(ScaleLabelDirection pDirection, int i, double unit)
        {
            switch (pDirection)
            {
                case ScaleLabelDirection.Left:
                    SetLeftLineProperty(i, unit);
                    break;
                case ScaleLabelDirection.Top:
                    SetTopLineProperty(i, unit);
                    break;
                case ScaleLabelDirection.Right:
                    SetRightLineProperty(i, unit);
                    break;
                case ScaleLabelDirection.Bottom:
                    SetBottomLineProperty(i, unit);
                    break;
            }
        }

        private void SetLeftLineProperty(int i, double unit)
        {
            if (i > 0 && i < ScaleLabelCount - 1)
            {
                var l = new Line
                {
                    X1 = 0,
                    Y1 = 0, 
                    X2 = _content.ActualWidth,
                    Y2 = 0, 
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = new TranslateTransform { X = 0.5, Y = 0.5 }
                };
                _scaleCanvas.Children.Add(l);
                Canvas.SetLeft(l, _scaleCanvas.ActualWidth);
                Canvas.SetTop(l, i * unit);
            }
        }
        private void SetTopLineProperty(int i, double unit)
        {
            if (i > 0 && i < ScaleLabelCount - 1)
            {
                var l = new Line
                {
                    X1 = 0,
                    X2 = 0,
                    Y1 = 0,
                    Y2 = _content.ActualHeight,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = new TranslateTransform { X = 0.5, Y = 0.5 }
                };
                _scaleCanvas.Children.Add(l);
                Canvas.SetLeft(l, i * unit);
                Canvas.SetTop(l, _scaleCanvas.ActualHeight);
            }
        }
        private void SetRightLineProperty(int i, double unit)
        {
            if (i > 0 && i < ScaleLabelCount - 1)
            {
                var l = new Line
                {
                    X1 = 0,
                    X2 = _content.ActualWidth,
                    Y1 = 0,
                    Y2 = 0,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = new TranslateTransform { X = 0.5, Y = 0.5 }
                };
                _scaleCanvas.Children.Add(l);
                Canvas.SetLeft(l, -_content.ActualWidth);
                Canvas.SetTop(l, i * unit);
            }
        }
        private void SetBottomLineProperty(int i, double unit)
        {
            if (i > 0 && i < ScaleLabelCount - 1)
            {
                var l = new Line
                {
                    X1 = 0,
                    X2 = 0,
                    Y1 = 0,
                    Y2 = _content.ActualHeight,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = new TranslateTransform { X = 0.5, Y = 0.5 }
                };
                _scaleCanvas.Children.Add(l);
                Canvas.SetLeft(l, i * unit);
                Canvas.SetTop(l, -_content.ActualHeight);
            }
        }

        private void SetTextBlockProperty(ScaleLabelDirection pDirection, TextBlock tb, int i, double unit)
        {
            double left = 0, top = 0;
            switch (pDirection)
            {
                case ScaleLabelDirection.Left:
                    tb.RenderTransform = new TranslateTransform { Y = -(tb.ActualHeight / 2) };
                    left = _scaleCanvas.ActualWidth - tb.ActualWidth - GetSpan();
                    top = i * unit;
                    Canvas.SetLeft(tb, left);
                    Canvas.SetTop(tb, top);
                    break;
                case ScaleLabelDirection.Top:
                    tb.RenderTransform = new TranslateTransform { X = -(tb.ActualWidth / 2) };
                    left = i * unit;
                    top = 0 - GetSpan();
                    Canvas.SetTop(tb, top);
                    Canvas.SetLeft(tb, left);
                    break;
                case ScaleLabelDirection.Right:
                    tb.RenderTransform = new TranslateTransform { Y = -(tb.ActualHeight / 2) };
                    left = 0 + GetSpan();
                    top = i * unit;
                    Canvas.SetLeft(tb, left);
                    Canvas.SetTop(tb, top);
                    break;
                case ScaleLabelDirection.Bottom:
                    tb.RenderTransform = new TranslateTransform { X = -(tb.ActualWidth / 2) };
                    left = i * unit;
                    top = 0 + GetSpan();
                    Canvas.SetTop(tb, top);
                    Canvas.SetLeft(tb, left);
                    break;
            }
            if (LabelStyle != null)
                tb.Style = LabelStyle;
            else
            {
                var bind = new Binding("Foreground") { Source = this };
                tb.SetBinding(TextBlock.ForegroundProperty, bind);
            }
            tb.Tag = new[] { left, top };
        }

        /// <summary>
        /// 刻度的宽或高
        /// </summary>
        /// <returns></returns>
        private double GetSpan()
        {
            if (OverlapRangeBlock)
                return 0;
            double span;
            if (Direction == ScaleLabelDirection.Top || Direction == ScaleLabelDirection.Bottom)
            {
                span = _content.ActualHeight;
            }
            else
            {
                span = _content.ActualWidth;
            }
            return span;
        }

        private double GetUnitValue()
        {
            return (EndValue - BeginValue) / (ScaleLabelCount - 1);
        }

        private double GetUnitSize()
        {
            if (Direction == ScaleLabelDirection.Top || Direction == ScaleLabelDirection.Bottom)
            {
                return _rangePanel.ActualWidth / (ScaleLabelCount - 1);
            }
            else
            {
                return _rangePanel.ActualHeight / (ScaleLabelCount - 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDirection"></param>
        /// <param name="vunit"></param>
        /// <param name="unit"></param>
        private void GetUnit(ScaleLabelDirection pDirection, out double vunit, out double unit)
        {
            vunit = (EndValue - BeginValue) / (ScaleLabelCount - 1);
            if (pDirection == ScaleLabelDirection.Top || pDirection == ScaleLabelDirection.Bottom)
            {
                unit = _rangePanel.ActualWidth / (ScaleLabelCount - 1);
            }
            else
            {
                unit = _rangePanel.ActualHeight / (ScaleLabelCount - 1);
            }
        }

        #endregion

        #region 公有

        public Func<double, double> ValueStringFormat;

        public event Action<ScaleValueConverterArgs> ScaleConvertValue;

        #endregion
    }

    public sealed class ScaleValueConverterArgs : EventArgs
    {
        public ScaleValueConverterArgs(int pScaleIndex, uint pDefaultDec, object pValue)
        {
            ScaleIndex = pScaleIndex;
            DefaultDec = pDefaultDec;
            Value = pValue;
        }
        public int ScaleIndex { get; private set; }
        public uint DefaultDec { get; private set; }
        public object Value { get; private set; }
        public object NewValue { get; set; }
    }

    internal class ScaleLinePromptValueConverter : IValueConverter
    {
        private readonly ScaleLinePrompt _prompt;
        public ScaleLinePromptValueConverter(ScaleLinePrompt pPrompt)
        {
            _prompt = pPrompt;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                if (parameter.ToString() == "b")
                {
                    if (!string.IsNullOrWhiteSpace(_prompt.BeginValueFormat))
                    {
                        double v;
                        if (double.TryParse(value.ToString(), out v) && _prompt.ValueStringFormat != null)
                            v = _prompt.ValueStringFormat(v);
                        return string.Format(_prompt.BeginValueFormat.Replace("\\", string.Empty), v);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(_prompt.EndValueFormat))
                    {
                        double v;
                        if (double.TryParse(value.ToString(), out v) && _prompt.ValueStringFormat != null)
                            v = _prompt.ValueStringFormat(v);
                        return string.Format(_prompt.EndValueFormat.Replace("\\", string.Empty), v);
                    }
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    /// <summary>
    /// 值范围刻度值连接线方向
    /// </summary>
    public enum ScaleLabelDirection
    {
        Left = 0,
        Top,
        Right,
        Bottom
    }

}
