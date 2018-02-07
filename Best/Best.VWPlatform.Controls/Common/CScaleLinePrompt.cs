using Best.VWPlatform.Common.Types;
using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls.Common
{
    /// <summary>
    /// 值范围提示，在特定范围内显示出当前值位置，和历史最大值位置
    /// </summary>
    [TemplatePartAttribute(Name = "x_rangePanel", Type = typeof(Grid))]
    [TemplatePartAttribute(Name = "x_content", Type = typeof(ContentPresenter))]
    [TemplatePartAttribute(Name = "x_scalePanel", Type = typeof(Grid))]
    [ContentProperty("Content")]
    public class CScaleLinePrompt : Control
    {
        #region 变量
        private Grid _rangePanel;
        private ContentPresenter _content;
        private Grid _scalePanel;
        private const uint MaxExtendDec = 8;

        public static readonly DependencyProperty BeginValueProperty =
            DependencyProperty.Register("BeginValue", typeof(double), typeof(CScaleLinePrompt), new PropertyMetadata(ValueChangedCallback));

        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(CScaleLinePrompt), new PropertyMetadata(ValueChangedCallback));

        public static readonly DependencyProperty BeginValueFormatProperty =
            DependencyProperty.Register("BeginValueFormat", typeof(string), typeof(CScaleLinePrompt), new PropertyMetadata("{0}"));

        public static readonly DependencyProperty EndValueFormatProperty =
            DependencyProperty.Register("EndValueFormat", typeof(string), typeof(CScaleLinePrompt), new PropertyMetadata("{0}"));

        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register("ValueFormat", typeof(string), typeof(CScaleLinePrompt), null);

        public static readonly DependencyProperty ScaleLineColorProperty = DependencyProperty.Register(
            "ScaleLineColor", typeof(Brush), typeof(CScaleLinePrompt), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(ScaleLabelDirection), typeof(CScaleLinePrompt), null);

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(CScaleLinePrompt), null);

        public static readonly DependencyProperty ScaleLabelCountProperty =
            DependencyProperty.Register("ScaleLabelCount", typeof (int), typeof (CScaleLinePrompt), null);

        public static readonly DependencyProperty DefaultDecLengthProperty =
            DependencyProperty.Register("DefaultDecLength", typeof(uint), typeof(CScaleLinePrompt), new PropertyMetadata((uint)0, ValueChangedCallback));
       
        #endregion
        static CScaleLinePrompt()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CScaleLinePrompt), new FrameworkPropertyMetadata(typeof(CScaleLinePrompt)));
        }

        public CScaleLinePrompt()
        {
            ScaleLabelCount = 5;
            DefaultDecLength = 2;
        }
        
        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.Equals(e.OldValue))
                return;
            var prompt = d as CScaleLinePrompt;
            if (prompt != null) prompt.Update();
        }

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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _content = GetTemplateChild("x_content") as ContentPresenter;
            _rangePanel = GetTemplateChild("x_rangePanel") as Grid;
            _scalePanel = GetTemplateChild("x_scalePanel") as Grid;

            if (_content == null || _rangePanel == null || _scalePanel == null)
            {
                return;
            }

            this.Loaded += CScaleLinePrompt_Loaded;
            this.SizeChanged += CScaleLinePrompt_SizeChanged;

            LoadLayout();
        }

        void CScaleLinePrompt_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        void CScaleLinePrompt_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }

        private List<TextBlock> _scaleTextBlocks = new List<TextBlock>();

        private void LoadLayout()
        {
            switch (Direction)
            {
                case ScaleLabelDirection.Left:

                    _rangePanel.ColumnDefinitions.Add(new ColumnDefinition());
                    _rangePanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Pixel) });
                    Grid.SetColumn(_content, 1);
                    Grid.SetColumn(_scalePanel, 0);
                    Grid.SetColumnSpan(_scalePanel, 2);
                    _scalePanel.ColumnDefinitions.Add(new ColumnDefinition());
                    _scalePanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Pixel) });

                    for (int i = 0; i < ScaleLabelCount; i++)
                    {
                        if (i == 0)
                        {
                            _scalePanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        }
                        else if (i == ScaleLabelCount - 1)
                        {
                            _scalePanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        }
                        else
                        {
                            _scalePanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                        }
                    }

                    for (int i = 0; i < ScaleLabelCount; i++)
                    {
                        Line l = new Line
                        {
                            X1 = 0,
                            X2 = 5,
                            Y1 = 0,
                            Y2 = 0,
                            Stroke = new SolidColorBrush(Colors.Black),
                            StrokeThickness = 1,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        _scalePanel.Children.Add(l);
                        Grid.SetColumn(l, 1);
                        Grid.SetRow(l, i);

                        TextBlock tb = new TextBlock
                        {
                            Text = "--:--:--",
                            FontSize = 11,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Right
                        };

                        _scaleTextBlocks.Add(tb);

                        _scalePanel.Children.Add(tb);
                        Grid.SetColumn(tb, 0);
                        Grid.SetRow(tb, i);

                        if (i == 0)
                        {
                            l.VerticalAlignment = VerticalAlignment.Top;
                            tb.VerticalAlignment = VerticalAlignment.Top;
                        }
                        else if (i == ScaleLabelCount - 1)
                        {
                            l.VerticalAlignment = VerticalAlignment.Bottom;
                            tb.VerticalAlignment = VerticalAlignment.Bottom;
                        }
                    }

                    break;
                case ScaleLabelDirection.Right:
                    
                    _rangePanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Pixel) });
                    _rangePanel.ColumnDefinitions.Add(new ColumnDefinition());
                    Grid.SetColumn(_content, 0);
                    Grid.SetColumn(_scalePanel, 0);
                    Grid.SetColumnSpan(_scalePanel, 2);
                    _scalePanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Pixel) });
                    _scalePanel.ColumnDefinitions.Add(new ColumnDefinition());

                    for (int i = 0; i < ScaleLabelCount; i++)
                    {
                        if (i == 0)
                        {
                            _scalePanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        }
                        else if (i == ScaleLabelCount - 1)
                        {
                            _scalePanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                        }
                        else
                        {
                            _scalePanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
                        }
                    }

                    for (int i = 0; i < ScaleLabelCount; i++)
                    {
                        Line l = new Line
                        {
                            X1 = 0,
                            X2 = 5,
                            Y1 = 0,
                            Y2 = 0,
                            Stroke = new SolidColorBrush(Colors.Black),
                            StrokeThickness = 1,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        _scalePanel.Children.Add(l);
                        Grid.SetColumn(l, 0);
                        Grid.SetRow(l, i);

                        TextBlock tb = new TextBlock
                        {
                            Text = "--:--:--",
                            FontSize = 11,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };

                        _scaleTextBlocks.Add(tb);

                        _scalePanel.Children.Add(tb);
                        Grid.SetColumn(tb, 1);
                        Grid.SetRow(tb, i);

                        if (i == 0)
                        {
                            l.VerticalAlignment = VerticalAlignment.Top;
                            tb.VerticalAlignment = VerticalAlignment.Top;
                        }
                        else if (i == ScaleLabelCount - 1)
                        {
                            l.VerticalAlignment = VerticalAlignment.Bottom;
                            tb.VerticalAlignment = VerticalAlignment.Bottom;
                        }
                    }

                    break;
                case ScaleLabelDirection.Bottom:

                    _rangePanel.RowDefinitions.Add(new RowDefinition {Height = new GridLength(5, GridUnitType.Pixel)});
                    _rangePanel.RowDefinitions.Add(new RowDefinition());
                    Grid.SetRow(_content, 0);
                    Grid.SetRow(_scalePanel, 0);
                    Grid.SetRowSpan(_scalePanel, 2);
                    _scalePanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Pixel) });
                    _scalePanel.RowDefinitions.Add(new RowDefinition());

                    for (int i = 0; i < ScaleLabelCount; i++)
                    {
                        if (i == 0)
                        {
                            _scalePanel.ColumnDefinitions.Add(new ColumnDefinition
                            {
                                Width = new GridLength(1, GridUnitType.Star)
                            });
                        }
                        else if (i == ScaleLabelCount - 1)
                        {
                            _scalePanel.ColumnDefinitions.Add(new ColumnDefinition
                            {
                                Width =  new GridLength(1, GridUnitType.Star)
                            });
                        }
                        else
                        {
                            _scalePanel.ColumnDefinitions.Add(new ColumnDefinition
                            {
                                Width = new GridLength(2, GridUnitType.Star)
                            });

                        }
                    }

                    for (int i = 0; i < ScaleLabelCount; i++)
                    {
                        Line l = new Line
                        {
                            X1 = 0,
                            X2 = 0,
                            Y1 = 0,
                            Y2 = 5,
                            Stroke = new SolidColorBrush(Colors.Black),
                            StrokeThickness = 1,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };
                        _scalePanel.Children.Add(l);
                        Grid.SetRow(l, 0);
                        Grid.SetColumn(l, i);
                        
                        TextBlock tb = new TextBlock
                        {
                            Text = "--:--:--",
                            FontSize = 11,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };
                        _scaleTextBlocks.Add(tb);

                        _scalePanel.Children.Add(tb);
                        Grid.SetRow(tb, 1);
                        Grid.SetColumn(tb, i);

                        if (i == 0)
                        {
                            l.HorizontalAlignment = HorizontalAlignment.Left;
                            tb.HorizontalAlignment = HorizontalAlignment.Left;
                            
                        }
                        else if (i == ScaleLabelCount - 1)
                        {
                            l.HorizontalAlignment = HorizontalAlignment.Right;
                            tb.HorizontalAlignment = HorizontalAlignment.Right;
                        }
                    }

                    break;
                case ScaleLabelDirection.Top:
                    break;
            }
        }

        /// <summary>
        /// 更新瀑布图纵坐标时间
        /// </summary>
        /// <param name="pTimeFlows"></param>
        internal void UpdateTimeFlowScales(IList<DateTime> pTimeFlows)
        {
            for (int i = 0; i < _scaleTextBlocks.Count; i++)
            {
                int index = (int) ActualHeight * i/_scaleTextBlocks.Count;
                switch (Direction)
                {
                    case ScaleLabelDirection.Right:
                        if (index < pTimeFlows.Count)
                        {
                            _scaleTextBlocks[i].Text = pTimeFlows[index].ToString("HH:mm:ss");
                        }
                        break;
                }
            }
        }

        public void Update()
        {
            if (!BeginValue.Equals(EndValue))
            {
                double vunit = GetUnitValue();
                uint defaultDec = DefaultDecLength;

                var valStrs = new List<string>();

            start:
                valStrs.Clear();

                for (int i = 0; i < _scaleTextBlocks.Count; i++)
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

                    valStrs.Add(_scaleTextBlocks[i].Text);
                    _scaleTextBlocks[i].Text = v;
                }
            }

            for (int i = 0; i < _scaleTextBlocks.Count; i++)
            {
                switch (Direction)
                {
                    case ScaleLabelDirection.Left:
                    case ScaleLabelDirection.Right:

                        double h = _scaleTextBlocks[i].ActualHeight / 2;
                        if (i == 0)
                        {
                            _scaleTextBlocks[i].Margin = new Thickness(0, -h, 0, 0);
                        }
                        else if (i == ScaleLabelCount - 1)
                        {
                            _scaleTextBlocks[i].Margin = new Thickness(0, 0, 0, -h);
                        }

                        break;

                    case ScaleLabelDirection.Bottom:
                    case ScaleLabelDirection.Top:
                        double w = _scaleTextBlocks[i].ActualWidth / 2;
                        if (i == 0)
                        {
                            _scaleTextBlocks[i].Margin = new Thickness(-w, 0, 0, 0);
                        }
                        else if (i == ScaleLabelCount - 1)
                        {
                            _scaleTextBlocks[i].Margin = new Thickness(0, 0, -w, 0);
                        }

                        break;
                }
            }
        }

        private double GetUnitValue()
        {
            return (EndValue - BeginValue) / (ScaleLabelCount - 1);
        }

        #region 公有

        public Func<double, double> ValueStringFormat;

        public event Action<ScaleValueConverterArgs> ScaleConvertValue;

        #endregion

    }
}
