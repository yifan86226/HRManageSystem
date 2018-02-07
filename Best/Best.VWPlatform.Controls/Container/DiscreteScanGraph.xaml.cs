using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using Best.VWPlatform.Controls.Freq;
using System.Configuration;
using Best.VWPlatform.Controls.Common;
using Best.VWPlatform.Common.Utility;

namespace Best.VWPlatform.Controls.Container
{
    /// <summary>
    /// 离散/频段扫描图
    /// </summary>
    public partial class DiscreteScanGraph : UserControl
    {
        private const string BuoyMarkName = "E8D80A36-1377-4362-851A-106207138029";
        private const int EdgeMargin = 5;
        private const string PeakMarkName = "D684ABB7-2A05-4F57-ACAC-12467D52694C";
        private const string ThresholdMarkName = "9990C459-0878-408F-B125-71047AC87BFF";

        private readonly SolidColorBrush _blankBrush = new SolidColorBrush(Color.FromArgb(255, 9, 9, 9));
        private readonly Dictionary<int, Brush> _cacheColors = new Dictionary<int, Brush>();
        private readonly SolidColorBrush _chartBackgroundA = new SolidColorBrush(Color.FromArgb(127, 0, 0, 0));
        private readonly SolidColorBrush _chartBackgroundB = new SolidColorBrush(Color.FromArgb(95, 0, 0, 0));
        private readonly Dictionary<int, TextBlock> _coordinateY = new Dictionary<int, TextBlock>(); 
        private readonly Dictionary<int, TextBlock> _coordinateY1 = new Dictionary<int, TextBlock>();

        private readonly Dictionary<double, DataPoint> _dataPoints = new Dictionary<double, DataPoint>();

        /// <summary>
        /// 数据点
        /// </summary>
        private readonly Rectangle _rect;

        private readonly List<Tuple<double, double, Button>> _scaleHistory = new List<Tuple<double, double, Button>>();
        private readonly SolidColorBrush _whiteBrush = new SolidColorBrush(Colors.White);
        private readonly SolidColorBrush _yellowBrush = new SolidColorBrush(Color.FromArgb(255, 249, 167, 0));
        private double _currentMarkTranslateY;
        //导出excel时，获得当前监测的频率及电平值
        private double _currentMarkValue;

        private Point _mouseEnd;
        private Point _mouseStart;
        private bool _panning;
        private bool _receiveData = true;
        private SpectrumMark _thresholdMark;
        private Point _viewMousePos;
        private WriteableBitmap _writeableBmp;
        private Point _xAxisMarkInsertPoint;
        private double _xAxisUnitLength = 1;
        private Point _yAxisMarkInsertPoint;
        private double _yAxisUnitLength = 1;

        private string _cursorPanelId = Guid.NewGuid().ToString();

        /// <summary>
        /// 是否是触摸屏模式，true表示触摸屏
        /// </summary>
        private bool _isTouch = ConfigurationManager.AppSettings["IsTouchModel"].Equals("1");

        private double _xMinInitValue;
        private double _xMaxInitValue;
        public DiscreteScanGraph()
        {
            InitializeComponent();

            _rect = new Rectangle
            {
                Visibility = Visibility.Collapsed,
                Width = 0,
                Height = 0,
                Stroke = new SolidColorBrush(Color.FromArgb(0xff, 0xfa, 0xeb, 0xd7)),
                StrokeThickness = 1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0)
            };
            uxChartBackground.Children.Add(_rect);
            Canvas.SetZIndex(_rect, 1000);
            FormatText = "{0}MHz - {1}dBμV";
            MarkMenuCommand = new DsMarkPanelMenuCommand(this);

            DataContext = this;

            if (YAxisVisiblity == Visibility.Visible)
                CreateYAxis();

            CreateXAxis(XAxisMinimum, XAxisMaximum);

            uxChartBackground.Loaded += (o, eventArgs) =>
            {
                if (_isTouch)
                {
                    uxChartBackground.TouchDown += uxChartBackground_TouchDown;
                    uxChartBackground.TouchUp += uxChartBackground_TouchUp;
                    uxChartBackground.TouchMove += uxChartBackground_TouchMove;
                }
                else
                {
                    uxChartBackground.MouseLeftButtonDown += OnUxChartAreaOnMouseLeftButtonDown;
                    uxChartBackground.MouseLeftButtonUp += OnUxChartAreaOnMouseLeftButtonUp;
                    uxChartBackground.MouseMove += OnUxChartAreaOnMouseMove;
                }
            };
            SizeChanged += OnDiscreteScanGraphSizeChanged;
            KeyUp += OnChartAreaKeyDown;

            if (_isTouch)
            {
                x_dbuvTopPanelGrid.TouchDown += x_dbuvTopPanelGrid_TouchDown;
                x_dbuvTopPanelGrid.TouchMove += x_dbuvTopPanelGrid_TouchMove;
                x_dbuvTopPanelGrid.TouchUp += x_dbuvTopPanelGrid_TouchUp;

                x_dbuvTopPanelCanvas.TouchDown += x_dbuvTopPanelGrid_TouchDown;
                x_dbuvTopPanelCanvas.TouchMove += x_dbuvTopPanelGrid_TouchMove;
                x_dbuvTopPanelCanvas.TouchUp += x_dbuvTopPanelGrid_TouchUp;

                x_dbuvBottomPanelGrid.TouchDown += x_dbuvBottomPanelGrid_TouchDown;
                x_dbuvBottomPanelGrid.TouchMove += x_dbuvBottomPanelGrid_TouchMove;
                x_dbuvBottomPanelGrid.TouchUp += x_dbuvBottomPanelGrid_TouchUp;

                x_dbuvBottomPanelCanvas.TouchDown += x_dbuvBottomPanelGrid_TouchDown;
                x_dbuvBottomPanelCanvas.TouchMove += x_dbuvBottomPanelGrid_TouchMove;
                x_dbuvBottomPanelCanvas.TouchUp += x_dbuvBottomPanelGrid_TouchUp;
            }
        }

        public event Action<double> ThresholdValueChanged;

        public event Action<double> XAxisMaximumChanged;

        public event Action<double> XAxisMinimumChanged;

        public event Action<double> XUnitlengthChanged;

        public event Action<double> YAxisMaximumChanged;

        public event Action<double> YAxisMinimumChanged;
        public event Action<double> YUnitlengthChanged;

        public double CurrentMarkTranslateY
        {
            get { return _currentMarkTranslateY; }
        }

        public double CurrentMarkValue
        {
            get { return _currentMarkValue; }
        }

        public string FormatText { get; set; }

        public ICommand MarkMenuCommand { get; set; }

        public ICommand ShowThresholdCmd { get; set; }

        /// <summary>
        /// X轴单位长度，(uxChartBackground.ActualWidth - 2*EdgeMargin)/(max - min)
        /// </summary>
        public double XAxisUnitLength
        {
            get { return _xAxisUnitLength; }
            set
            {
                _xAxisUnitLength = value;
                RaiseXUnitlengthChanged(value);
            }
        }

        /// <summary>
        /// Y轴单位长度
        /// </summary>
        public double YAxisUnitLength
        {
            get { return _yAxisUnitLength; }
            set
            {
                _yAxisUnitLength = value;
                RaiseYUnitlengthChanged(value);
            }
        }

        public DataPoint this[double xaxis]
        {
            get
            {
                if (!_dataPoints.ContainsKey(xaxis))
                {
                    _dataPoints.Add(xaxis, new DataPoint(xaxis, 0));
                }

                return _dataPoints[xaxis];
            }
            set { _dataPoints[xaxis] = value; }
        }

        #region Dependency property

        public static readonly DependencyProperty DataPointColorProperty =
            DependencyProperty.Register("DataPointColor", typeof (Brush), typeof (DiscreteScanGraph),
                new PropertyMetadata(null));

        public static readonly DependencyProperty DataPointWidthProperty =
            DependencyProperty.Register("DataPointWidth", typeof (int), typeof (DiscreteScanGraph),
                new PropertyMetadata(1));

        public static readonly DependencyProperty DefaultForeColorProperty =
            DependencyProperty.Register("DefaultForeColor", typeof (Brush), typeof (DiscreteScanGraph),
                new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty ThresholdValueProperty =
            DependencyProperty.Register("ThresholdValue", typeof (double), typeof (DiscreteScanGraph),
                new PropertyMetadata(0d, ThresholdValuePropertyChangedCallback));

        public static readonly DependencyProperty ThresholdVisibilityProperty =
            DependencyProperty.Register("ThresholdVisibility", typeof(Visibility), typeof(DiscreteScanGraph),
                new PropertyMetadata(Visibility.Collapsed, ThresholdVisibilityChangedCallback));

        public static readonly DependencyProperty XAxisMaximumProperty =
            DependencyProperty.Register("XAxisMaximum", typeof (double), typeof (DiscreteScanGraph),
                new PropertyMetadata(300.0, XAxisMaximumPropertyChangedCallback));

        public static readonly DependencyProperty XAxisMinimumProperty =
            DependencyProperty.Register("XAxisMinimum", typeof (double), typeof (DiscreteScanGraph),
                new PropertyMetadata(1.0, XAxisMinimumPropertyChangedCallback));

        public static readonly DependencyProperty YAxisIntervalProperty =
            DependencyProperty.Register("YAxisInterval", typeof (double), typeof (DiscreteScanGraph),
                new PropertyMetadata(5.0));

        public static readonly DependencyProperty YAxisMaximumProperty =
            DependencyProperty.Register("YAxisMaximum", typeof (double), typeof (DiscreteScanGraph),
                new PropertyMetadata(80.0, YAxisMaximumPropertyChangedCallback));

        public static readonly DependencyProperty YAxisMinimumProperty =
            DependencyProperty.Register("YAxisMinimum", typeof (double), typeof (DiscreteScanGraph),
                new PropertyMetadata(-20.0, YAxisMinimumPropertyChangedCallback));

        public static readonly DependencyProperty YAxisUnitVisibilityProperty =
            DependencyProperty.Register("YAxisUnitVisibility", typeof (Visibility), typeof (DiscreteScanGraph),
                new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty YAxisVisiblityProperty =
            DependencyProperty.Register("YAxisVisiblity", typeof (Visibility), typeof (DiscreteScanGraph),
                new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty MeasureUnitForeColorProperty =
            DependencyProperty.Register("MeasureUnitForeColor", typeof(Brush), typeof(DiscreteScanGraph), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xa8, 0x00))));
        public Brush DataPointColor
        {
            get { return (Brush) GetValue(DataPointColorProperty); }
            set { SetValue(DataPointColorProperty, value); }
        }

        public int DataPointWidth
        {
            get { return (int) GetValue(DataPointWidthProperty); }
            set { SetValue(DataPointWidthProperty, value); }
        }

        public Brush DefaultForeColor
        {
            get { return (Brush) GetValue(DefaultForeColorProperty); }
            set { SetValue(DefaultForeColorProperty, value); }
        }

        public double ThresholdValue
        {
            get { return (double) GetValue(ThresholdValueProperty); }
            set { SetValue(ThresholdValueProperty, value); }
        }

        public Visibility ThresholdVisibility
        {
            get { return (Visibility) GetValue(ThresholdVisibilityProperty); }
            set { SetValue(ThresholdVisibilityProperty, value); }
        }

        /// <summary>
        /// X轴最大值
        /// </summary>
        public double XAxisMaximum
        {
            get { return (double) GetValue(XAxisMaximumProperty); }
            set { SetValue(XAxisMaximumProperty, value); }
        }

        /// <summary>
        /// X轴最小值
        /// </summary>
        public double XAxisMinimum
        {
            get { return (double) GetValue(XAxisMinimumProperty); }
            set { SetValue(XAxisMinimumProperty, value); }
        }

        /// <summary>
        /// Y轴间隔
        /// </summary>
        public double YAxisInterval
        {
            get { return (double) GetValue(YAxisIntervalProperty); }
            set { SetValue(YAxisIntervalProperty, value); }
        }

        /// <summary>
        /// Y轴最大值
        /// </summary>
        public double YAxisMaximum
        {
            get { return (double) GetValue(YAxisMaximumProperty); }
            set { SetValue(YAxisMaximumProperty, value); }
        }

        /// <summary>
        /// Y轴最小值
        /// </summary>
        public double YAxisMinimum
        {
            get { return (double) GetValue(YAxisMinimumProperty); }
            set { SetValue(YAxisMinimumProperty, value); }
        }

        public Visibility YAxisUnitVisibility
        {
            get { return (Visibility) GetValue(YAxisUnitVisibilityProperty); }
            set { SetValue(YAxisUnitVisibilityProperty, value); }
        }

        public Visibility YAxisVisiblity
        {
            get { return (Visibility) GetValue(YAxisVisiblityProperty); }
            set { SetValue(YAxisVisiblityProperty, value); }
        }

        public Brush MeasureUnitForeColor
        {
            get { return (Brush)GetValue(MeasureUnitForeColorProperty); }
            set { SetValue(MeasureUnitForeColorProperty, value); }
        }
        #endregion Dependency property

        /// <summary>
        /// 添加数据点
        /// </summary>
        /// <param name="xValue">X轴位置</param>
        /// <param name="yValue">Y轴值</param>
        [Obsolete("使用AddDataPointBmp", true)]
        public void AddDataPoint(double xValue, double yValue)
        {
            if (xValue > XAxisMaximum || xValue < XAxisMinimum)
            {
                if (_dataPoints.ContainsKey(xValue))
                {
                    DataPoint dp = this[xValue];
                    dp.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // 添加数据点
                    var dataPoint = new DataPoint(xValue, yValue)
                    {
                        YAxisUnitLength = _yAxisUnitLength,
                        YAxisHeight = Math.Abs(yValue - YAxisMinimum), // Height
                        Width = DataPointWidth,
                        Label = yValue.ToString(CultureInfo.InvariantCulture),
                        FormatText = FormatText,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Visibility = Visibility.Collapsed
                    };

                    _dataPoints.Add(xValue, dataPoint);
                    uxChartArea.Children.Add(dataPoint);
                }
                return;
            }

            if (yValue > YAxisMaximum)
            {
                yValue = YAxisMaximum;
            }
            else if (yValue < YAxisMinimum)
            {
                YAxisMinimum = yValue - 5;
            }

            //if (uxPeakContainer.Visibility == Visibility.Collapsed && YAxisUnitVisibility == Visibility.Visible)
            //    uxPeakContainer.Visibility = Visibility.Visible;
            uxPeakContainer.IsEnabled = true;

            // x margin
            double x = (xValue - XAxisMinimum)*_xAxisUnitLength;
            x = x < EdgeMargin ? EdgeMargin : x > uxChartArea.ActualWidth ? uxChartArea.ActualWidth - EdgeMargin : x;
            //hjy 初始计算坐标轴显示的比例
            if (_yAxisUnitLength.Equals(0))
                _yAxisUnitLength = uxChartBackground.ActualHeight/(YAxisMaximum - YAxisMinimum);
            // 使用统一的颜色
            Brush brush;
            if (DataPointColor != null)
            {
                brush = DataPointColor;
            }
            else
            {
                if (_cacheColors.ContainsKey((int) yValue))
                {
                    brush = _cacheColors[(int) yValue];
                }
                else
                {
                    brush = new SolidColorBrush(GetColorAtPoint(uxYAxisRect,
                        new Point(uxYAxisRect.ActualWidth,
                            uxYAxisRect.ActualHeight - Math.Abs(yValue - YAxisMinimum)*_yAxisUnitLength)));
                    _cacheColors.Add((int) yValue, brush);
                }
            }

            if (!_dataPoints.ContainsKey(xValue))
            {
                if (x >= uxChartArea.Width)
                    return;

                // 添加数据点
                var dataPoint = new DataPoint(xValue, yValue)
                {
                    YAxisUnitLength = _yAxisUnitLength,
                    YAxisHeight = Math.Abs(yValue - YAxisMinimum), // Height
                    Width = DataPointWidth,
                    Label = yValue.ToString(CultureInfo.InvariantCulture),
                    FormatText = FormatText,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Foreground = brush,
                    Visibility =
                        ThresholdVisibility == Visibility.Visible
                            ? yValue > ThresholdValue ? Visibility.Visible : Visibility.Collapsed
                            : Visibility.Visible
                };

                _dataPoints.Add(xValue, dataPoint);
                dataPoint.Margin = new Thickness(x, 0, 0, 0);
                uxChartArea.Children.Add(dataPoint);
            }
            else
            {
                this[xValue].Visibility = ThresholdVisibility == Visibility.Visible
                    ? yValue > ThresholdValue ? Visibility.Visible : Visibility.Collapsed
                    : Visibility.Visible;
                this[xValue].YAxisUnitLength = _yAxisUnitLength;
                this[xValue].YAxisValue = yValue;
                this[xValue].Foreground = brush;
                this[xValue].YAxisHeight = Math.Abs(yValue - YAxisMinimum);
                this[xValue].Label = yValue.ToString(CultureInfo.InvariantCulture);
                this[xValue].Margin = new Thickness(x, 0, 0, 0);
                this[xValue].UpdateLayout();
            }

            // 更新mark浮标
            if (uxXAxisMarkPanel.Children.Count > 0)
                UpdateMarkValue(xValue);
        }

        public void AddDataPointBmp(double xValue, double yValue)
        {
            if (!_receiveData)
                return;

            if (_writeableBmp.PixelHeight == 0 || _writeableBmp.PixelWidth == 0)
            {
                _writeableBmp = new WriteableBitmap((int)uxWaterfall.Width, (int)uxWaterfall.Height, 96, 96, PixelFormats.Pbgra32, null);
                _writeableBmp.FillRectangle(0, 0, (int) uxWaterfall.Width, (int) uxWaterfall.Height, Colors.Transparent);
                uxWaterfall.Source = _writeableBmp;
            }

            if (yValue > YAxisMaximum)
            {
                yValue = YAxisMaximum;
            }
            else if (yValue < YAxisMinimum)
            {
                YAxisMinimum = yValue;
            }

            //if (uxPeakContainer.Visibility == Visibility.Collapsed && YAxisUnitVisibility == Visibility.Visible)
            //    uxPeakContainer.Visibility = Visibility.Visible;
            uxPeakContainer.IsEnabled = true;

            //初始计算坐标轴显示的比例
            if (_yAxisUnitLength.Equals(0))
                _yAxisUnitLength = uxChartBackground.ActualHeight/(YAxisMaximum - YAxisMinimum);

            // 使用统一的颜色
            Brush brush;
            if (DataPointColor != null)
            {
                brush = DataPointColor;
            }
            else
            {
                if (_cacheColors.ContainsKey((int) yValue))
                {
                    brush = _cacheColors[(int) yValue];
                }
                else
                {
                    brush =
                        new SolidColorBrush(GetColorAtPoint(uxYAxisRect,
                            new Point(uxYAxisRect.ActualWidth,
                                uxYAxisRect.ActualHeight - Math.Abs(yValue - YAxisMinimum)*_yAxisUnitLength)));
                    _cacheColors.Add((int) yValue, brush);
                }
            }

            if (_dataPoints.ContainsKey(xValue)) //更新数据点
            {
                _dataPoints[xValue].Foreground = brush;
                _dataPoints[xValue].FormatText = FormatText;
                _dataPoints[xValue].Label = yValue.ToString(CultureInfo.InvariantCulture);
                _dataPoints[xValue].Width = DataPointWidth;
                _dataPoints[xValue].YAxisValue = yValue;
            }
            else //添加数据点
            {
                var dataPoint = new DataPoint(xValue, yValue)
                {
                    YAxisValue = yValue,
                    Width = DataPointWidth,
                    Label = yValue.ToString(CultureInfo.InvariantCulture),
                    FormatText = FormatText,
                    Foreground = brush
                };
                _dataPoints.Add(xValue, dataPoint);
            }

            _writeableBmp.FillRectangle(0, 0, (int)uxWaterfall.Width, (int)uxWaterfall.Height, Colors.Transparent);
            DrawDataPoints();

            // 更新浮标
            if (uxXAxisMarkPanel.Children.Count > 0)
                UpdateMarkValue(xValue);
        }

        public void BeginSave()
        {
            MeasureUnitForeColor = new SolidColorBrush(Colors.Black);
            DefaultForeColor = _blankBrush;
            //uxChartBackground.Background = new SolidColorBrush(Colors.Transparent);

            for (int i = 0; i < x_ScanfreqMarkPanel.Children.Count; i++)
            {
                SpectrumScanMark sp = x_ScanfreqMarkPanel.Children[i] as SpectrumScanMark;
                if (sp != null)
                {
                    sp.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        /// <summary>
        /// 清除所有统计值
        /// </summary>
        public void Clear()
        {
            ClearChart();
            _dataPoints.Clear();
            _cacheColors.Clear();
            _scaleHistory.Clear();
            ClearXAxisMark();
            ClearYAxisMark();
            uxZoomButtons.Children.Clear();
            uxChartArea.Children.Clear();
            //uxPeakContainer.Visibility = Visibility.Collapsed;
            uxPeakContainer.IsEnabled = false;
            ClearScanFreqMark();
        }

        /// <summary>
        /// 清除浮标
        /// </summary>
        public void ClearBuoy()
        {
            //@?
            //var buoyMarks = from m in uxXAxisMarkPanel.Children
            //    where (m as SpectrumMark) != null && ((SpectrumMark) m).GroupName == BuoyMarkName
            //    select m;

            List<SpectrumMark> marks = new List<SpectrumMark>();
            foreach (var v in uxXAxisMarkPanel.Children)
            {
                SpectrumMark sp = v as SpectrumMark;
                if (sp != null && sp.GroupName == BuoyMarkName)
                {
                    marks.Add(sp);
                }
            }

            if (marks.Any())
            {
                var mark = marks.First();
                uxXAxisMarkPanel.Children.Remove(mark);
            }
        }

        public void EndSave()
        {
            MeasureUnitForeColor = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xa8, 0x00));
            DefaultForeColor = _whiteBrush;
            //uxChartBackground.Background = new SolidColorBrush(Color.FromArgb(95, 0, 0, 0));

            for (int i = 0; i < x_ScanfreqMarkPanel.Children.Count; i++)
            {
                SpectrumScanMark sp = x_ScanfreqMarkPanel.Children[i] as SpectrumScanMark;
                if (sp != null)
                {
                    sp.Foreground = new SolidColorBrush(Colors.White);
                }
            }
        }

        /// <summary>
        /// 移动浮标
        /// </summary>
        /// <param name="xvalue"></param>
        /// <param name="yvalue"></param>
        public void MoveBuoyTo(double xvalue, double yvalue)
        {
            if (yvalue < ThresholdValue) return;

            //var buoyMarks = from m in uxXAxisMarkPanel.Children
            //    where
            //        (m as SpectrumMark) != null &&
            //        ((SpectrumMark) m).GroupName == BuoyMarkName
            //    select m;
            //var marks = buoyMarks as UIElement[] ?? buoyMarks.ToArray();

            List<SpectrumMark> marks = new List<SpectrumMark>();
            foreach (var v in uxXAxisMarkPanel.Children)
            {
                SpectrumMark sp = v as SpectrumMark;
                if (sp != null && sp.GroupName == BuoyMarkName)
                {
                    marks.Add(sp);
                }
            }

            if (!marks.Any()) return;

            var mark = marks.First();

            if (Math.Abs(mark.TranslateY - 0d) < 1)
                mark.TranslateY = double.MinValue;

            var kv = _dataPoints.FirstOrDefault(d => Equals(d.Key, xvalue));
            if (kv.Value == null) return;

            var dp = kv.Value;
            mark.BuoyOffset = uxChartBackground.ActualHeight - (dp.YAxisValue - YAxisMinimum)*_yAxisUnitLength;
            mark.TranslateY = dp.YAxisValue;
            mark.MarkValue = Convert.ToDouble(dp.XAxisValue.ToString("0.000"));
            var x = (dp.XAxisValue - XAxisMinimum)*_xAxisUnitLength;
            mark.TranslateX = x <= 0 ? 5 : x;
            mark.UpdateLayout();
            _currentMarkValue = mark.MarkValue;
            _currentMarkTranslateY = mark.TranslateY;
            if (mark.MarkValue < XAxisMinimum || mark.MarkValue > XAxisMaximum)
            {
                if (mark.Id != _cursorPanelId)
                    mark.Visibility = Visibility.Collapsed; /*Mark指向的频点超出缩放范围，隐藏*/
            }
            else
            {
                if (mark.Id != _cursorPanelId && mark.Visibility == Visibility.Collapsed)
                    mark.Visibility = Visibility.Visible;
            }
        }

        public void ShowBuoy()
        {
            //var buoyMarks = from m in uxXAxisMarkPanel.Children
            //    where
            //        (m as SpectrumMark) != null &&
            //        ((SpectrumMark) m).GroupName == BuoyMarkName
            //    select m;
            //var marks = buoyMarks as UIElement[] ?? buoyMarks.ToArray();

            List<SpectrumMark> marks = new List<SpectrumMark>();
            foreach (var v in uxXAxisMarkPanel.Children)
            {
                SpectrumMark sp = v as SpectrumMark;
                if (sp != null && sp.GroupName == BuoyMarkName)
                {
                    marks.Add(sp);
                }
            }

            if (!marks.Any())
            {
                AddXAxisMark(Color.FromArgb(0xff, 0x33, 0xf0, 0x10), BuoyMarkName);
            }
        }

        private static void ThresholdValuePropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var dsg = (DiscreteScanGraph) d;
            var newValue = (double) e.NewValue;
            if (dsg._thresholdMark != null)
            {
                dsg._thresholdMark.TranslateY = (dsg.YAxisMaximum - newValue)*dsg._yAxisUnitLength -
                                                dsg.uxYAxisMarkPanel.ActualHeight;
                dsg._thresholdMark.MarkValue = Math.Abs(newValue - 0) > .1
                    ? Convert.ToDouble(newValue.ToString("#.#"))
                    : 0d;
            }
            if (dsg.ThresholdValueChanged != null)
                dsg.ThresholdValueChanged(newValue);
        }

        private static void ThresholdVisibilityChangedCallback(DependencyObject dependency,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var dsg = (DiscreteScanGraph) dependency;
            Visibility visibility;
            var result = Enum.TryParse(eventArgs.NewValue.ToString(), out visibility);
            if (result)
            {
               dsg.SwitchThreshold(visibility);
            }
        }

        private static void XAxisMaximumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dsg = (DiscreteScanGraph) d;
            if (dsg.uxXAxis == null) return;

            dsg.RaiseXAxisMaximumChanged(Convert.ToDouble(e.NewValue));
            dsg.AdjustPosition();
        }

        private static void XAxisMinimumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dsg = (DiscreteScanGraph) d;
            if (dsg.uxXAxis == null) return;

            dsg.RaiseXAxisMinimumChanged(Convert.ToDouble(e.NewValue));
            dsg.AdjustPosition();
        }

        private static void YAxisMaximumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dsg = (DiscreteScanGraph) d;
            if (dsg.uxYAxis == null) return;

            dsg.RaiseYAxisMaximumChanged(Convert.ToInt32(e.NewValue));
            dsg.AxisMaximum();
            dsg.AdjustPosition();
        }

        private static void YAxisMinimumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dsg = (DiscreteScanGraph) d;
            if (dsg.uxYAxis == null) return;

            dsg.RaiseYAxisMinimumChanged(Convert.ToInt32(e.NewValue));
            if (dsg.YAxisVisiblity == Visibility.Visible)
                dsg.AxisMininum();
            dsg.AdjustPosition();
        }

        /// <summary>
        /// 添加X轴值刻度
        /// </summary>
        /// <param name="xAxis">X轴该点距原点的绝对长度</param>
        /// <param name="min">X轴最小值</param>
        private void AddXAxis(double xAxis, double min)
        {
            if (xAxis > XAxisMaximum) return;

            double left = xAxis*_xAxisUnitLength;
            if (double.IsNaN(left)) return;

            var rect = new Rectangle
            {
                Height = 5,
                Width = 2,
                Fill = _blankBrush,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(left + EdgeMargin, 0, 0, 0)
            };
            var txt = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = (min + xAxis).ToString("#0.00"),
                FontSize = 11,
                Margin = new Thickness(left + EdgeMargin/2, 0, 0, 0)
            };
            var forbinding = new Binding("DefaultForeColor") { Source = this, Mode = BindingMode.TwoWay };
            txt.SetBinding(TextBlock.ForegroundProperty, forbinding);

            uxXAxis.Children.Add(rect);
            uxXAxis.Children.Add(txt);
            Grid.SetRow(txt, 1); //@?
        }

        /// <summary>
        /// 重新计算坐标轴
        /// </summary>
        private void AdjustPosition()
        {
            uxWaterfall.Width = uxChartBackground.ActualWidth;
            uxWaterfall.Height = uxChartBackground.ActualHeight;
            CreateXAxis(XAxisMinimum, XAxisMaximum);

            UpdateScanFreqMarks();
            UpdateYAxisMarks();

            if (_writeableBmp == null) return;
            ClearChart();
            _receiveData = false;
            DrawDataPoints();
            _receiveData = true;   
            UpdateMarkPosition();
        }

        private void ClearChart()
        {
            uxWaterfall.Source = null;
            _writeableBmp = null;
            _writeableBmp = new WriteableBitmap((int)uxWaterfall.Width, (int)uxWaterfall.Height, 96, 96, PixelFormats.Pbgra32, null);
            _writeableBmp.FillRectangle(0, 0, (int) uxWaterfall.Width, (int) uxWaterfall.Height, Colors.Transparent);
            uxWaterfall.Source = _writeableBmp;
        }

        /// <summary>
        /// 建X轴 
        /// </summary>
        private void CreateXAxis(double xAxisMin, double xAxisMax)
        {
            double min = xAxisMin < XAxisMinimum ? XAxisMinimum : xAxisMin;
            double max = xAxisMax > XAxisMaximum ? XAxisMaximum : xAxisMax;

            if (uxChartBackground.ActualWidth > uxXAxisDash.ActualWidth)
                uxChartBackground.Width = uxXAxisDash.ActualWidth;

            _xAxisUnitLength = (uxChartBackground.ActualWidth - 2*EdgeMargin)/(max - min);

            #region x轴

            uxXAxis.Children.Clear();
            var minRect = new Rectangle
            {
                Height = 5,
                Width = 2,
                Fill = _blankBrush,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(EdgeMargin, 0, 0, 0)
            };
            var minTxt = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = min.ToString("#0.00[MHz]"),
                FontSize = 11,
                Margin = new Thickness(EdgeMargin/2, 0, 0, 0)
            };
            var minforeBinding = new Binding("DefaultForeColor") { Mode = BindingMode.TwoWay };
            minTxt.SetBinding(TextBlock.ForegroundProperty, minforeBinding);

            uxXAxis.Children.Add(minRect);
            uxXAxis.Children.Add(minTxt);
            Grid.SetRow(minTxt, 1);

            var maxRect = new Rectangle
            {
                Height = 5,
                Width = 2,
                Fill = _blankBrush,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, EdgeMargin, 0)
            };
            var maxTxt = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Text = max.ToString("#0.00"),
                FontSize = 11,
                Margin = new Thickness(0, 0, EdgeMargin/2, 0)
            };
            var maxforeBinding = new Binding("DefaultForeColor") { Mode = BindingMode.TwoWay };
            maxTxt.SetBinding(TextBlock.ForegroundProperty, maxforeBinding);

            uxXAxis.Children.Add(maxRect);
            uxXAxis.Children.Add(maxTxt);
            Grid.SetRow(maxTxt, 1);

            #endregion x轴

            double step = (max - min)/3d;

            for (int i = 1; i < 3; i++)
            {
                AddXAxis(i*step, min);
            }
        }

        /// <summary>
        /// 建Y轴，添加刻度
        /// </summary>
        private void CreateYAxis()
        {
            _yAxisUnitLength = uxChartBackground.ActualHeight/(YAxisMaximum - YAxisMinimum);

            #region y轴

            _cacheColors.Clear();
            uxYAxis.Children.Clear();
            uxYAxis.RowDefinitions.Clear();

            uxYAxisRight.Children.Clear();
            uxYAxisRight.RowDefinitions.Clear();

            int j = 0;
            for (double i = YAxisMaximum; i > YAxisMinimum; i -= 20, j++)
            {
                uxYAxis.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
                uxYAxisRight.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});

                if (i < YAxisMaximum)
                {
                    var rect = new Rectangle
                    {
                        Height = 2,
                        Width = 5,
                        Fill = _blankBrush,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    uxYAxis.Children.Add(rect);
                    Grid.SetColumn(rect, 1);
                    Grid.SetRow(rect, j);

                    var rectR = new Rectangle
                    {
                        Height = 2,
                        Width = 5,
                        Fill = _blankBrush,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    uxYAxisRight.Children.Add(rectR);
                    Grid.SetRow(rectR, j);
                }

                var txt = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Text = i.ToString(CultureInfo.InvariantCulture),
                    FontSize = 11,
                    Margin = new Thickness(0, -5, 0, 0),
                    Visibility = YAxisUnitVisibility
                };
                var binding = new Binding("YAxisUnitVisibility") {Source = this, Mode = BindingMode.TwoWay};
                txt.SetBinding(VisibilityProperty, binding);

                var foreBinding = new Binding("DefaultForeColor") {Mode = BindingMode.TwoWay};
                txt.SetBinding(TextBlock.ForegroundProperty, foreBinding);

                _coordinateY.Add(j, txt);
                uxYAxis.Children.Add(txt);
                Grid.SetRow(txt, j);

                var txtR = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Text = i.ToString(CultureInfo.InvariantCulture),
                    FontSize = 11,
                    Margin = new Thickness(0, -5, 0, 0),
                    Visibility = YAxisUnitVisibility
                };

                var bindingR = new Binding("DefaultForeColor") {Source = this, Mode = BindingMode.TwoWay};
                txtR.SetBinding(TextBlock.ForegroundProperty, bindingR);

                _coordinateY1.Add(j, txtR);
                uxYAxisRight.Children.Add(txtR);
                Grid.SetColumn(txtR, 1);
                Grid.SetRow(txtR, j);
            }

            if (j > 0)
            {
                var txt = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Text = YAxisMinimum.ToString(CultureInfo.InvariantCulture),
                    FontSize = 11,
                    Margin = new Thickness(0, 0, 0, -5),
                    Visibility = YAxisUnitVisibility
                };
                var forbinding = new Binding("DefaultForeColor") { Source = this, Mode = BindingMode.TwoWay };
                txt.SetBinding(TextBlock.ForegroundProperty, forbinding);

                var binding = new Binding("YAxisUnitVisibility") {Source = this, Mode = BindingMode.TwoWay};
                txt.SetBinding(VisibilityProperty, binding);
                _coordinateY.Add(j, txt);
                uxYAxis.Children.Add(txt);
                Grid.SetRow(txt, j);

                var txtR = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Text = YAxisMinimum.ToString(CultureInfo.InvariantCulture),
                    FontSize = 11,
                    Margin = new Thickness(0, 0, 0, -5),
                    Visibility = YAxisUnitVisibility
                };
                var bindingR = new Binding("DefaultForeColor") { Source = this, Mode = BindingMode.TwoWay };
                txtR.SetBinding(TextBlock.ForegroundProperty, bindingR);

                _coordinateY1.Add(j, txtR);
                uxYAxisRight.Children.Add(txtR);
                Grid.SetColumn(txtR, 1);
                Grid.SetRow(txtR, j);
            }
            Grid.SetRowSpan(uxChartArea, j + 1);
            Grid.SetRowSpan(uxYaxisAdjust, j + 1);

            #endregion y轴
        }

        /// <summary>
        /// 画频谱线
        /// </summary>
        private void DrawDataPoints()
        {
            foreach (var dataPoint in _dataPoints)
            {
                if(ThresholdVisibility == System.Windows.Visibility.Visible)
                {
                    if (dataPoint.Value.YAxisValue < ThresholdValue) continue;
                }
                
                if (dataPoint.Value.XAxisValue < XAxisMinimum || dataPoint.Value.XAxisValue > XAxisMaximum) continue;
                var x = (dataPoint.Value.XAxisValue - XAxisMinimum)*_xAxisUnitLength;
                x = x < EdgeMargin
                    ? EdgeMargin
                    : x > uxChartBackground.ActualWidth ? uxChartBackground.ActualWidth - EdgeMargin : x;
                
                _writeableBmp.DrawLine((int) x, _writeableBmp.PixelHeight, (int) x,
                    _writeableBmp.PixelHeight - (int) ((dataPoint.Value.YAxisValue - YAxisMinimum)*_yAxisUnitLength),
                    ((SolidColorBrush) dataPoint.Value.Foreground).Color);
            }
            //_writeableBmp.Invalidate();
        }

        private void OnChartAreaKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                OnLeftPeakButtonClick(null, null);
            }
            else if (e.Key == Key.Right)
            {
                OnRightPeakButtonClick(null, null);
            }
        }

        private void OnDiscreteScanGraphSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _yAxisUnitLength = uxChartBackground.ActualHeight/(YAxisMaximum - YAxisMinimum);
            AdjustPosition();
        }

        /// <summary>
        /// uxChartBackground内按下鼠标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUxChartAreaOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _panning = true;
            uxChartBackground.CaptureMouse();
            _mouseStart = e.GetPosition(uxChartBackground);

            _viewMousePos = e.GetPosition(uxChartBackground);

            _rect.Width = 0;
            _rect.Width = 0;
            _rect.Visibility = Visibility.Visible;
            _rect.Margin = new Thickness(_viewMousePos.X, _viewMousePos.Y, 0, 0);
        }

        private void OnUxChartAreaOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _panning = false;

            uxChartBackground.ReleaseMouseCapture();
            _mouseEnd = e.GetPosition(uxChartBackground);
            // 拖动范围最小 5
            if (Math.Abs(_mouseStart.X - 0) < .1 || Math.Abs(_mouseStart.Y - 0) < .1 ||
                Math.Abs(_mouseEnd.X - _mouseStart.X) < 5) return;

            if (_dataPoints.Count == 0)
            {
                _rect.Visibility = Visibility.Collapsed;
                return;
            }

            double x1 = _rect.Margin.Left;
            double x2 = x1 + _rect.Width;

            // 计算框选的x轴的范围
            double xv1 = (x1 - EdgeMargin)/_xAxisUnitLength + XAxisMinimum;
            double xv2 = (x2 - EdgeMargin)/_xAxisUnitLength + XAxisMinimum;
            if (xv1 >= xv2)
            {
                return;
            }
            if (double.IsNaN(xv1) || double.IsNaN(xv2)) return;

            double y = _rect.Margin.Top;

            _rect.Visibility = Visibility.Collapsed;
            if (x1 + _rect.Width > uxChartBackground.ActualWidth || y + _rect.Height > uxChartBackground.ActualHeight)
            {
                return;
            }

            int delta = _mouseStart.X > _mouseEnd.X ? -1 : 1;

            // 起点在终点的左侧：放大 否则：缩小
            if (delta > 0)
            {
                if (uxZoomButtons.Children.Count >= 7)
                    return;
                var btn = new ImageButton
                {
                    Margin = new Thickness(5, 0, 0, 0),
                    Padding = new Thickness(5, 0, 5, 0),
                    Width = 40,
                    Height = 30,
                    Foreground = new SolidColorBrush(Colors.White),
                    Tag = _scaleHistory.Count,
                };
                btn.Click += (o, args) =>
                {
                    var button = o as Button;
                    if (button == null) return;

                    var index = (int) button.Tag;
                    if (_scaleHistory.Count < index || index < 0) return;

                    var tuple = _scaleHistory[index];

                    XAxisMinimum = tuple.Item1;
                    XAxisMaximum = tuple.Item2;
                    UpdateFreqMarks();
                    UpdateScanFreqMarks();
                };
                // 记录缩放的x轴的范围和对应的按钮
                _scaleHistory.Add(new Tuple<double, double, Button>(XAxisMinimum, XAxisMaximum, btn));
                btn.Content = _scaleHistory.Count;
                uxZoomButtons.Children.Add(btn);

                XAxisMinimum = xv1;
                XAxisMaximum = xv2;
            }
            else if (delta < 0)
            {
                if (_scaleHistory.Count == 0) return;
                var tuple = _scaleHistory[0];
                _scaleHistory.Clear();
                uxZoomButtons.Children.Clear();

                XAxisMinimum = tuple.Item1;
                XAxisMaximum = tuple.Item2;
            }

            UpdateFreqMarks();
            UpdateScanFreqMarks();
            _mouseStart = new Point(0d, 0d);
        }

        /// <summary>
        /// 刷新频点Mark
        /// </summary>
        private void UpdateFreqMarks()
        {
            foreach (var m in uxXAxisMarkPanel.Children)
            {
                var mark = m as SpectrumMark;
                if (mark == null)
                    continue;
                UpdateFreqMarkOffset(mark);

            }
        }

        /// <summary>
        /// 刷新频点Mark X轴偏移
        /// </summary>
        /// <param name="pMark"></param>
        private void UpdateFreqMarkOffset(SpectrumMark pMark)
        {
            if (pMark.Id == _cursorPanelId)
                return;
            if (uxZoomButtons.Children.Count == 0)
            {
                pMark.Visibility = Visibility.Visible;
            }
            else
            {
                if (pMark.MarkValue < XAxisMinimum || pMark.MarkValue > XAxisMaximum)
                {
                    if (pMark.Id != _cursorPanelId)
                        pMark.Visibility = Visibility.Collapsed; /*Mark指向的频点超出缩放范围，隐藏*/
                }
                else
                {
                    if (pMark.Id != _cursorPanelId && pMark.Visibility == Visibility.Collapsed)
                        pMark.Visibility = Visibility.Visible;
                }
            }

        }

        private void OnUxChartAreaOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_panning) return;

            Point pt = e.GetPosition(uxChartBackground);

            if (pt.X > _viewMousePos.X && pt.X < uxChartBackground.ActualWidth)
            {
                _rect.Width = Math.Abs(pt.X - _viewMousePos.X);
            }
            else if (pt.X < _viewMousePos.X && pt.X > 0)
            {
                var margin = new Thickness(pt.X, _rect.Margin.Top, 0, 0);
                _rect.Margin = margin;
                _rect.Width = Math.Abs(_viewMousePos.X - pt.X);
            }
            if (pt.Y > _viewMousePos.Y && pt.Y < uxChartBackground.ActualHeight)
            {
                _rect.Height = Math.Abs(pt.Y - _viewMousePos.Y);
            }
            else if (pt.Y < _viewMousePos.Y && pt.Y > 0)
            {
                var margin = new Thickness(_rect.Margin.Left, pt.Y, 0, 0);
                _rect.Margin = margin;
                _rect.Height = Math.Abs(pt.Y - _viewMousePos.Y);
            }
        }

        private void RaiseXAxisMaximumChanged(double newValue)
        {
            if (XAxisMaximumChanged != null)
            {
                XAxisMaximumChanged(newValue);
            }
        }

        private void RaiseXAxisMinimumChanged(double newValue)
        {
            if (XAxisMinimumChanged != null)
            {
                XAxisMinimumChanged(newValue);
            }
        }

        private void RaiseXUnitlengthChanged(double newValue)
        {
            if (XUnitlengthChanged != null)
            {
                XUnitlengthChanged(newValue);
            }
        }

        private void RaiseYAxisMaximumChanged(double newValue)
        {
            if (YAxisMaximumChanged != null)
            {
                YAxisMaximumChanged(newValue);
            }
        }

        private void RaiseYAxisMinimumChanged(double newValue)
        {
            if (YAxisMinimumChanged != null)
            {
                YAxisMinimumChanged(newValue);
            }
        }

        private void RaiseYUnitlengthChanged(double newValue)
        {
            if (YUnitlengthChanged != null)
            {
                YUnitlengthChanged(newValue);
            }
        }

        private void SwitchThreshold(Visibility obj)
        {
            if (obj == Visibility.Visible)
            {
                _thresholdMark = AddYAxisMark(Color.FromArgb(255, 249, 167, 0), ThresholdMarkName);
                _thresholdMark.TranslateY = (YAxisMaximum - ThresholdValue)*_yAxisUnitLength -
                                            uxYAxisMarkPanel.ActualHeight;
                _thresholdMark.MarkValue = Convert.ToDouble(ThresholdValue.ToString("0.#"));
                if (ThresholdValueChanged != null)
                    ThresholdValueChanged(ThresholdValue);
            }
            else
            {
                uxYAxisMarkPanel.Children.Remove(_thresholdMark);
                _thresholdMark = null;
                if (ThresholdValueChanged != null)
                {
                    ThresholdValueChanged(double.MinValue);
                }
            }
        }

        #region 调节Y轴值

        private void uxDbuvBottomDecrement_Click(object sender, RoutedEventArgs e)
        {
            YAxisMinimum -= YAxisInterval;
            AxisMininum();
        }

        private void uxDbuvBottomIncrement_Click(object sender, RoutedEventArgs e)
        {
            if (YAxisMinimum + YAxisInterval >= YAxisMaximum)
                return;

            YAxisMinimum += YAxisInterval;
            AxisMininum();
        }

        private void uxDbuvTopDecrement_Click(object sender, RoutedEventArgs e)
        {
            if (YAxisMaximum - YAxisInterval <= YAxisMinimum)
                return;

            YAxisMaximum -= YAxisInterval;
            AxisMaximum();
        }

        private void uxDbuvTopIncrement_Click(object sender, RoutedEventArgs e)
        {
            YAxisMaximum += YAxisInterval;
            AxisMaximum();
        }

        #endregion

        #region Mark 相关操作

        /// <summary>
        /// 在X轴添加标记点
        /// </summary>
        /// <param name="pMarkColor">颜色</param>
        /// <param name="pGroupName">组名</param>
        /// <returns></returns>
        internal SpectrumMark AddXAxisMark(Color pMarkColor, string pGroupName = "-1")
        {
            var mark = new SpectrumMark
            {
                Direction = MarkDirection.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                Color = new SolidColorBrush(pMarkColor),
                Foreground = new SolidColorBrush(Colors.White),
                TranslateX = _xAxisMarkInsertPoint.X,
                BuoyVisibility = Visibility.Visible,
                VerticalLineVisibility = Visibility.Visible,
                TooltipVisibility = Visibility.Visible,
                GroupName = pGroupName
            };

            mark.MouseMove += OnMarkMouseMove;
            mark.MouseLeftButtonUp += OnMarkMouseLeftButtonUp;
            double xvalue = (_xAxisMarkInsertPoint.X - EdgeMargin)/_xAxisUnitLength + XAxisMinimum;
            double yUnitLength = uxChartBackground.ActualHeight/(YAxisMaximum - YAxisMinimum);
            mark.MarkValue = Convert.ToDouble(xvalue.ToString("0.0"));
            uxXAxisMarkPanel.Children.Add(mark);
            Canvas.SetZIndex(mark, 10001);

            if (_dataPoints.Count == 0) return mark;

            var lst = _dataPoints.Where(k => k.Value.YAxisValue > ThresholdValue);

            if (!lst.Any())
            {
                return null;
            }

            double key = lst.Min(k => Math.Abs(k.Key - xvalue)); 
            
            DataPoint dp;
            if (_dataPoints.ContainsKey(key + xvalue))
            {
                dp = _dataPoints[key + xvalue];
            }
            else
            {
                dp = _dataPoints[xvalue - key];
            }

            mark.BuoyVisibility = Visibility.Visible;
            mark.BuoyOffset = uxChartBackground.ActualHeight - (dp.YAxisValue - YAxisMinimum)*yUnitLength;
            mark.TranslateY = dp.YAxisValue;
            double x = (dp.XAxisValue - XAxisMinimum)*_xAxisUnitLength;
            if (x < EdgeMargin)
                x = EdgeMargin;
            else if (x > (uxChartBackground.ActualWidth - EdgeMargin))
                x = uxChartBackground.ActualWidth - EdgeMargin;
            mark.TranslateX = x + 1;
            mark.MarkValue = dp.XAxisValue;
            mark.UpdateLayout();

            return mark;
        }

        /// <summary>
        /// 在Y轴添加标记点
        /// </summary>
        /// <param name="pMarkColor">颜色</param>
        /// <param name="pGroupName">组名</param>
        /// <returns></returns>
        internal SpectrumMark AddYAxisMark(Color pMarkColor, string pGroupName = "-1")
        {
            var mark = new SpectrumMark
            {
                Direction = MarkDirection.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Color = new SolidColorBrush(pMarkColor),
                Foreground =
                    pMarkColor.Equals(Colors.White)
                        ? new SolidColorBrush(Colors.Black)
                        : new SolidColorBrush(Colors.White),
                GroupName = pGroupName,
                TranslateY = _yAxisMarkInsertPoint.Y > 0 ? _yAxisMarkInsertPoint.Y - uxYAxisMarkPanel.ActualHeight : 0,
                FontSize = 10
            };
            mark.MouseMove += OnMarkMouseMove;
            mark.MouseLeftButtonUp += YAxisMarkOnMouseLeftButtonUp;

            var yUnitLength = uxYAxisRect.ActualHeight / (YAxisMaximum - YAxisMinimum);
            if (_yAxisMarkInsertPoint.Y > 0)
            {
                var markValue = YAxisMaximum - _yAxisMarkInsertPoint.Y / yUnitLength;
                mark.MarkValue = Convert.ToDouble(markValue.ToString("0.0"));
            }
            else
            {
                mark.MarkValue = 0;
            }

            uxYAxisMarkPanel.Children.Add(mark);
            return mark;
        }

        /// <summary>
        /// 清除X轴标记点
        /// </summary>
        internal void ClearXAxisMark()
        {
            var removeObjs = new List<object>();
            for (int i = 0; i < uxXAxisMarkPanel.Children.Count; i++)
            {
                var mark = uxXAxisMarkPanel.Children[i] as SpectrumMark;
                if (mark == null )
                    //|| mark.GroupName == "1"
                    //|| mark.GroupName == "2"
                    //|| mark.GroupName == BuoyMarkName
                    //|| mark.GroupName == PeakMarkName
                    //|| mark.GroupName == ThresholdMarkName)
                    continue;
                removeObjs.Add(mark);
            }

            foreach (object removeObj in removeObjs)
            {
                var mark = removeObj as SpectrumMark;
                if (mark == null)
                    continue;

                uxXAxisMarkPanel.Children.Remove(mark);
            }
        }

        /// <summary>
        /// 清除Y轴标记点
        /// </summary>
        internal void ClearYAxisMark()
        {
            //ThresholdVisibility = Visibility.Collapsed;
            var removeObjs = new List<object>();
            for (int i = 0; i < uxYAxisMarkPanel.Children.Count; i++)
            {
                var mark = uxYAxisMarkPanel.Children[i] as SpectrumMark;
                if (mark == null || mark.GroupName == "1" || mark.GroupName == "2" || mark.GroupName == ThresholdMarkName)
                    continue;
                removeObjs.Add(mark);
            }
            foreach (object removeObj in removeObjs)
            {
                var mark = removeObj as SpectrumMark;
                if (mark == null)
                    continue;
                uxYAxisMarkPanel.Children.Remove(mark);
            }
        }

        private void GetNextPeak(SpectrumMark mark, double start, double stop)
        {
            if (_dataPoints.Count <= 0) return;
            if (Math.Abs(mark.TranslateY - 0d) < 1)
                mark.TranslateY = double.MinValue;

            var kvs = _dataPoints.Where(d => d.Key > start && d.Key < stop);
            var pairs = kvs as KeyValuePair<double, DataPoint>[] ?? kvs.ToArray();
            if (!pairs.Any()) return;

            var datas = pairs.Select(d => d.Value);
            var enumerable = datas as DataPoint[] ?? datas.ToArray();
            if (enumerable.Any())
            {
                var dp = enumerable.OrderByDescending(p => p.YAxisValue).First();
                mark.BuoyOffset = uxChartBackground.ActualHeight - (dp.YAxisValue - YAxisMinimum)*_yAxisUnitLength;
                mark.TranslateY = dp.YAxisValue;
                mark.MarkValue = Convert.ToDouble(dp.XAxisValue.ToString("0.0"));
                var x = (dp.XAxisValue - XAxisMinimum)*_xAxisUnitLength;
                mark.TranslateX = x <= 0 ? 5 : x;
                mark.UpdateLayout();
            }
        }

        private void OnMarkMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var mark = sender as SpectrumMark;
                if (mark == null) return;

                var pt = e.GetPosition(uxChartBackground);

                // X 轴的频率值
                var xvalue = (pt.X - EdgeMargin) / _xAxisUnitLength + XAxisMinimum;
                // 重新计算 Y 轴的单位长度
                var yUnitLength = uxChartBackground.ActualHeight / (YAxisMaximum - YAxisMinimum);

                if (_dataPoints.Count == 0) return;
                var key = _dataPoints.Where(k => k.Value.YAxisValue > ThresholdValue).Min(k => Math.Abs(k.Key - xvalue));

                DataPoint dp;
                if (_dataPoints.ContainsKey(key + xvalue))
                {
                    dp = _dataPoints[key + xvalue];
                }
                else if (_dataPoints.ContainsKey(xvalue - key))
                {
                    dp = _dataPoints[xvalue - key];
                }
                else
                {
                    return;
                }
                if (dp.XAxisValue < XAxisMinimum || dp.XAxisValue > XAxisMaximum) return;

                mark.BuoyVisibility = Visibility.Visible;
                mark.BuoyOffset = uxChartBackground.ActualHeight - (dp.YAxisValue - YAxisMinimum) * yUnitLength;
                mark.TranslateY = dp.YAxisValue;
                double x = (dp.XAxisValue - XAxisMinimum) * _xAxisUnitLength;
                if (x < EdgeMargin)
                    x = EdgeMargin;
                else if (x > (uxChartBackground.ActualWidth - EdgeMargin))
                    x = uxChartBackground.ActualWidth - EdgeMargin;
                mark.TranslateX = x - 1;
                mark.MarkValue = Convert.ToDouble(dp.XAxisValue.ToString("0.000000"));
                mark.UpdateLayout();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void OnMarkMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var mark = sender as SpectrumMark;
                if (mark == null) return;

                var pt = e.GetPosition(uxChartBackground);
                if (mark.Direction == MarkDirection.Bottom)
                {
                    // X 轴的频率值
                    var xvalue = (pt.X - EdgeMargin) / _xAxisUnitLength + XAxisMinimum;
                    // 重新计算 Y 轴的单位长度
                    var yUnitLength = uxChartBackground.ActualHeight / (YAxisMaximum - YAxisMinimum);
                    if (_dataPoints.Count == 0) return;

                    var key = _dataPoints.Where(k => k.Value.YAxisValue > ThresholdValue).Min(k => Math.Abs(k.Key - xvalue));

                    DataPoint dp;
                    if (_dataPoints.ContainsKey(key + xvalue))
                    {
                        dp = _dataPoints[key + xvalue];
                    }
                    else if (_dataPoints.ContainsKey(xvalue - key))
                    {
                        dp = _dataPoints[xvalue - key];
                    }
                    else
                    {
                        return;
                    }
                    if (dp.XAxisValue < XAxisMinimum || dp.XAxisValue > XAxisMaximum) return;

                    mark.BuoyVisibility = Visibility.Visible;
                    mark.BuoyOffset = uxChartBackground.ActualHeight - (dp.YAxisValue - YAxisMinimum) * yUnitLength;
                    mark.TranslateY = dp.YAxisValue;
                    double x = (xvalue - XAxisMinimum) * _xAxisUnitLength;
                    if (x < EdgeMargin)
                        x = EdgeMargin;
                    else if (x > (uxChartBackground.ActualWidth - EdgeMargin))
                        x = uxChartBackground.ActualWidth - EdgeMargin;
                    mark.TranslateX = x;
                    mark.MarkValue = Convert.ToDouble(xvalue.ToString("0.0"));
                    mark.UpdateLayout();
                }
                else if (mark.Direction == MarkDirection.Left)
                {
                    var yUnitLength = uxYAxisRect.ActualHeight / (YAxisMaximum - YAxisMinimum);
                    var markValue = YAxisMaximum - pt.Y / yUnitLength;

                    if (markValue > YAxisMaximum || markValue < YAxisMinimum) return;

                    mark.MarkValue = Convert.ToDouble(markValue.ToString("0.0"));
                    var y = pt.Y - uxYAxisMarkPanel.ActualHeight;
                    mark.TranslateY = y;

                    // Y轴Mark值
                    if (mark.GroupName != ThresholdMarkName || ThresholdVisibility != Visibility.Visible) return;
                    ThresholdValue = mark.MarkValue;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void OnLeftPeakButtonClick(object sender, RoutedEventArgs e)
        {
            //var peakMark = from m in uxXAxisMarkPanel.Children
            //    where
            //        (m as SpectrumMark) != null &&
            //        ((SpectrumMark) m).GroupName == PeakMarkName
            //    select m;
            //var marks = peakMark as UIElement[] ?? peakMark.ToArray();

            List<SpectrumMark> marks = new List<SpectrumMark>();
            foreach (var v in uxXAxisMarkPanel.Children)
            {
                SpectrumMark sp = v as SpectrumMark;
                if (sp != null && sp.GroupName == PeakMarkName)
                {
                    marks.Add(sp);
                }
            }

            SpectrumMark mark;
            if (marks.Any())
            {
                mark = (SpectrumMark) marks.First();
                double start = XAxisMinimum;
                double stop = mark.MarkValue;

                GetNextPeak(mark, start, stop);
            }
            else
            {
                mark = AddXAxisMark(Color.FromArgb(0xff, 0xd3, 0x80, 0x00), PeakMarkName);

                GetNextPeak(mark, XAxisMinimum, XAxisMaximum);
            }
        }

        private void OnPeakButtonClick(object sender, RoutedEventArgs e)
        {
            //var peakMark = from m in uxXAxisMarkPanel.Children
            //    where
            //        (m as SpectrumMark) != null &&
            //        ((SpectrumMark) m).GroupName == PeakMarkName
            //    select m;
            //var marks = peakMark as UIElement[] ?? peakMark.ToArray();

            List<SpectrumMark> marks = new List<SpectrumMark>();
            foreach (var v in uxXAxisMarkPanel.Children)
            {
                SpectrumMark sp = v as SpectrumMark;
                if (sp != null && sp.GroupName == PeakMarkName)
                {
                    marks.Add(sp);
                }
            }

            SpectrumMark mark;
            if (marks.Any())
            {
                mark = (SpectrumMark) marks.First();
            }
            else
            {
                mark = AddXAxisMark(Color.FromArgb(0xff, 0xd3, 0x80, 0x00), PeakMarkName);
            }

            GetNextPeak(mark, XAxisMinimum, XAxisMaximum);
        }

        private void OnRightPeakButtonClick(object sender, RoutedEventArgs e)
        {
            //var peakMark = from m in uxXAxisMarkPanel.Children
            //    where
            //        (m as SpectrumMark) != null &&
            //        ((SpectrumMark) m).GroupName == PeakMarkName
            //    select m;
            //var marks = peakMark as UIElement[] ?? peakMark.ToArray();


            List<SpectrumMark> marks = new List<SpectrumMark>();
            foreach (var v in uxXAxisMarkPanel.Children)
            {
                SpectrumMark sp = v as SpectrumMark;
                if (sp != null && sp.GroupName == PeakMarkName)
                {
                    marks.Add(sp);
                }
            }

            SpectrumMark mark;
            if (marks.Any())
            {
                mark = (SpectrumMark) marks.First();
                double start = mark.MarkValue;
                double stop = XAxisMaximum;
                GetNextPeak(mark, start, stop);
            }
            else
            {
                mark = AddXAxisMark(Color.FromArgb(0xff, 0xd3, 0x80, 0x00), PeakMarkName);
                GetNextPeak(mark, XAxisMinimum, XAxisMaximum);
            }
        }

        /// <summary>
        /// 更新标记点位置
        /// </summary>
        private void UpdateMarkPosition()
        {
            //更新X轴mark位置
            foreach (UIElement item in uxXAxisMarkPanel.Children)
            {
                var mark = item as SpectrumMark;
                if (mark == null)
                    return;
                double markValue = mark.MarkValue;
                if (!_dataPoints.Any())
                    return;
                double key = _dataPoints.Min(k => Math.Abs(k.Key - markValue));
                DataPoint dataPoint;
                if (_dataPoints.ContainsKey(key + markValue))
                {
                    dataPoint = _dataPoints[key + markValue];
                }
                else if (_dataPoints.ContainsKey(markValue - key))
                {
                    dataPoint = _dataPoints[markValue - key];
                }
                else
                {
                    return;
                }
                double x = (dataPoint.XAxisValue - XAxisMinimum)*_xAxisUnitLength;
                if (x < 1)
                    x = EdgeMargin;
                mark.TranslateX = x;
                mark.BuoyOffset = uxChartBackground.ActualHeight -
                                  (dataPoint.YAxisValue - YAxisMinimum)*_yAxisUnitLength;
                mark.UpdateLayout();
            }

            //更新Y轴mark位置
            foreach (UIElement item in uxYAxisMarkPanel.Children)
            {
                var mark = item as SpectrumMark;
                if (mark == null)
                    return;
                mark.TranslateY = (YAxisMinimum - mark.MarkValue)*_yAxisUnitLength;
                mark.UpdateLayout();
            }
        }

        /// <summary>
        /// 根据信号列表频率值刷新主Mark位置
        /// </summary>
        public void UpdateFreqMarkPosition(double xvalue)
        {
            double key = _dataPoints.Min(k => Math.Abs(k.Key - xvalue));
            DataPoint dataPoint;
            if (_dataPoints.ContainsKey(key + xvalue))
            {
                dataPoint = _dataPoints[key + xvalue];
            }
            else if (_dataPoints.ContainsKey(xvalue - key))
            {
                dataPoint = _dataPoints[xvalue - key];
            }
            else
            {
                return;
            }

            SpectrumMark mark = null;

            foreach (var m in uxXAxisMarkPanel.Children)
            {
                SpectrumMark sp = m as SpectrumMark;
                if (sp != null)
                {
                    mark = sp;
                    break;
                }
            }

            if (mark == null)
            {
                mark = AddXAxisMark(Color.FromArgb(0xff, 0xd3, 0x80, 0x00), PeakMarkName);
            }

            if (dataPoint.XAxisValue < XAxisMinimum || dataPoint.XAxisValue > XAxisMaximum) return;

            mark.BuoyVisibility = Visibility.Visible;
            mark.BuoyOffset = uxChartBackground.ActualHeight - (dataPoint.YAxisValue - YAxisMinimum) * _yAxisUnitLength;
            mark.TranslateY = dataPoint.YAxisValue;
            double x = (dataPoint.XAxisValue - XAxisMinimum) * _xAxisUnitLength;
            if (x < EdgeMargin)
                x = EdgeMargin;
            else if (x > (uxChartBackground.ActualWidth - EdgeMargin))
                x = uxChartBackground.ActualWidth - EdgeMargin;
            mark.TranslateX = x + 1;
            mark.MarkValue = dataPoint.XAxisValue;
            mark.UpdateLayout();
        }

        /// <summary>
        /// 更新标记点值
        /// </summary>
        /// <param name="xvalue"></param>
        private void UpdateMarkValue(double xvalue)
        {
            double key = _dataPoints.Min(k => Math.Abs(k.Key - xvalue));
            DataPoint dataPoint;
            if (_dataPoints.ContainsKey(key + xvalue))
            {
                dataPoint = _dataPoints[key + xvalue];
            }
            else if (_dataPoints.ContainsKey(xvalue - key))
            {
                dataPoint = _dataPoints[xvalue - key];
            }
            else
            {
                return;
            }

            SpectrumMark mark = null;

            foreach (var m in uxXAxisMarkPanel.Children)
            {
                SpectrumMark sp = m as SpectrumMark;
                if (sp != null && sp.MarkValue.Equals(dataPoint.XAxisValue))
                {
                    mark = sp;
                    break;
                }
            }

            if (mark == null) return;

            if (dataPoint.XAxisValue < XAxisMinimum || dataPoint.XAxisValue > XAxisMaximum) return;

            mark.BuoyVisibility = Visibility.Visible;
            mark.BuoyOffset = uxChartBackground.ActualHeight - (dataPoint.YAxisValue - YAxisMinimum)*_yAxisUnitLength;
            mark.TranslateY = dataPoint.YAxisValue;
            mark.UpdateLayout();
        }

        private void XAxisMark_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _xAxisMarkInsertPoint = e.GetPosition(sender as FrameworkElement);
        }

        private void YAxisMark_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _yAxisMarkInsertPoint = e.GetPosition(sender as FrameworkElement);
        }

        private void YAxisMarkOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var mark = sender as SpectrumMark;
            if (mark == null || mark.GroupName != ThresholdMarkName) return;
        }

        #endregion Mark 相关操作

        #region  控制谱图纵坐标刻度显示

        private void AxisMaximum()
        {
            _yAxisUnitLength = uxChartBackground.ActualHeight/(YAxisMaximum - YAxisMinimum);

            int axisSpan = (Int32) (YAxisMaximum - YAxisMinimum)/5;
            _coordinateY[0].Text = ((int)YAxisMaximum).ToString();
            _coordinateY1[0].Text = ((int)YAxisMaximum).ToString();
            for (int i = 1; i < _coordinateY.Count - 1; i++)
            {
                _coordinateY[i].Text = ((int)(YAxisMaximum - i*axisSpan)).ToString();
                _coordinateY1[i].Text = ((int)(YAxisMaximum - i*axisSpan)).ToString();
            }
            _coordinateY[_coordinateY.Count - 1].Text = ((int)(YAxisMinimum)).ToString();
            _coordinateY1[_coordinateY.Count - 1].Text = ((int)(YAxisMinimum)).ToString();

            UpdateYAxisMarks();
        }

        private void AxisMininum()
        {
            _yAxisUnitLength = uxChartBackground.ActualHeight/(YAxisMaximum - YAxisMinimum);

            int axisSpan = (Int32) (YAxisMaximum - YAxisMinimum)/5;
            _coordinateY[0].Text = ((int)YAxisMaximum).ToString();
            _coordinateY1[0].Text = ((int)YAxisMaximum).ToString();
            for (int i = 1; i < _coordinateY.Count - 1; i++)
            {
                _coordinateY[i].Text = ((int)(YAxisMinimum + (_coordinateY.Count - 1 - i)*axisSpan)).ToString();
                _coordinateY1[i].Text = ((int)(YAxisMinimum + (_coordinateY.Count - 1 - i)*axisSpan)).ToString();
            }
            _coordinateY[_coordinateY.Count - 1].Text = ((int)(YAxisMinimum)).ToString();
            _coordinateY1[_coordinateY.Count - 1].Text = ((int)(YAxisMinimum)).ToString();

            UpdateYAxisMarks();
        }

        private void UpdateYAxisMarks()
        {
            foreach (var element in uxYAxisMarkPanel.Children)
            {
                var mark = element as SpectrumMark;
                if (mark != null)
                {
                    double pixelVal = WMonitorUtile.ViewToScreen(mark.MarkValue,
                    uxYAxisMarkPanel.ActualHeight, 0, YAxisMaximum, YAxisMinimum);

                    mark.TranslateY = -pixelVal;
                }
            }
        }

        #endregion  控制谱图纵坐标刻度显示

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(null, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region 取颜色值

        //Returns the signed magnitude of a point on a vector with origin po and pointing to pf
        private static double Dist(Point px, Point po, Point pf)
        {
            double d = Math.Sqrt((px.Y - po.Y)*(px.Y - po.Y) + (px.X - po.X)*(px.X - po.X));
            if (((px.Y < po.Y) && (pf.Y > po.Y)) ||
                ((px.Y > po.Y) && (pf.Y < po.Y)) ||
                ((px.Y == po.Y) && (px.X < po.X) && (pf.X > po.X)) ||
                ((px.Y == po.Y) && (px.X > po.X) && (pf.X < po.X)))
            {
                d = -d;
            }
            return d;
        }

        /// <summary>
        /// 提取SolidColorBrush中某一点的颜色
        /// </summary>
        /// <param name="theRec">使用SolidColorBrush填充的Rectangle</param>
        /// <param name="thePoint">Rectangle中的点</param>
        /// <returns>Color</returns>
        private static Color GetColorAtPoint(Rectangle theRec, Point thePoint)
        {
            //Get properties
            var br = (LinearGradientBrush) theRec.Fill;

            double y3 = thePoint.Y;
            double x3 = thePoint.X;

            double x1 = br.StartPoint.X*theRec.ActualWidth;
            double y1 = br.StartPoint.Y*theRec.ActualHeight;
            var p1 = new Point(x1, y1); //Starting point

            double x2 = br.EndPoint.X*theRec.ActualWidth;
            double y2 = br.EndPoint.Y*theRec.ActualHeight;
            var p2 = new Point(x2, y2); //End point

            //Calculate intersecting points
            Point p4; //with tangent

            if (y1 == y2) //Horizontal case
            {
                p4 = new Point(x3, y1);
            }
            else if (x1 == x2) //Vertical case
            {
                p4 = new Point(x1, y3);
            }
            else //Diagnonal case
            {
                double m = (y2 - y1)/(x2 - x1);
                double m2 = -1/m;
                double b = y1 - m*x1;
                double c = y3 - m2*x3;

                double x4 = (c - b)/(m - m2);
                double y4 = m*x4 + b;
                p4 = new Point(x4, y4);
            }

            //Calculate distances relative to the vector start
            double d4 = Dist(p4, p1, p2);
            double d2 = Dist(p2, p1, p2);

            double x = d4/d2;

            if (double.IsNaN(x)) return Colors.Green;

            //Clip the input if before or after the max/min offset values
            double max = br.GradientStops.Max(n => n.Offset);
            if (x > max)
            {
                x = max;
            }
            double min = br.GradientStops.Min(n => n.Offset);
            if (x < min)
            {
                x = min;
            }

            //Find gradient stops that surround the input value
            GradientStop gs0 = br.GradientStops.Where(n => n.Offset <= x).OrderBy(n => n.Offset).Last();
            GradientStop gs1 = br.GradientStops.Where(n => n.Offset >= x).OrderBy(n => n.Offset).First();

            float y = 0f;
            if (gs0.Offset != gs1.Offset)
            {
                y = (float) ((x - gs0.Offset)/(gs1.Offset - gs0.Offset));
            }

            //Interpolate color channels
            var aVal = (byte) ((gs1.Color.A - gs0.Color.A)*y + gs0.Color.A);
            var rVal = (byte) ((gs1.Color.R - gs0.Color.R)*y + gs0.Color.R);
            var gVal = (byte) ((gs1.Color.G - gs0.Color.G)*y + gs0.Color.G);
            var bVal = (byte) ((gs1.Color.B - gs0.Color.B)*y + gs0.Color.B);

            return Color.FromArgb(aVal, rVal, gVal, bVal);
        }

        #endregion 取颜色值

        #region ScanFreqPoint
        public void ShowScanMark()
        {
            List<SpectrumScanMark> marks = new List<SpectrumScanMark>();
            foreach (var v in x_ScanfreqMarkPanel.Children)
            {
                SpectrumScanMark sp = v as SpectrumScanMark;
                if (sp != null)
                {
                    marks.Add(sp);
                }
            }

            if (!marks.Any())
            {
                AddScanFreqMark(Color.FromArgb(0xff, 0xff, 0x00, 0x00));
            }
        }
        /// <summary>
        /// 移动扫描频点
        /// </summary>
        /// <param name="xvalue"></param>
        /// <param name="yvalue"></param>
        public void MoveScanMark(double xvalue, double yvalue)
        {
            if (ThresholdVisibility == Visibility.Visible && yvalue < ThresholdValue) return;

            List<SpectrumScanMark> marks = new List<SpectrumScanMark>();
            foreach (var v in x_ScanfreqMarkPanel.Children)
            {
                SpectrumScanMark sp = v as SpectrumScanMark;
                if (sp != null)
                {
                    marks.Add(sp);
                }
            }

            if (!marks.Any()) return;

            var mark = marks.First();

            if (Math.Abs(mark.TranslateY - 0d) < 1)
                mark.TranslateY = double.MinValue;

            var kv = _dataPoints.FirstOrDefault(d => Equals(d.Key, xvalue));
            if (kv.Value == null) return;

            var dp = kv.Value;
            mark.MarkOffset = uxChartBackground.ActualHeight - (dp.YAxisValue - YAxisMinimum) * _yAxisUnitLength - 28;
            mark.TranslateY = dp.YAxisValue;
            mark.MarkFreValue = Convert.ToDouble(dp.XAxisValue.ToString("0.000"));
            _currentMarkValue = mark.MarkFreValue;
            _currentMarkTranslateY = mark.TranslateY;
            var x = (dp.XAxisValue - XAxisMinimum) * _xAxisUnitLength;
            mark.TranslateX = x <= 0 ? (5 - 50) : (x - 50);
            mark.Visibility = Visibility.Visible;
            mark.UpdateLayout();

            if (mark.MarkFreValue < XAxisMinimum || mark.MarkFreValue > XAxisMaximum)
            {
                 mark.Visibility = Visibility.Collapsed; /*Mark指向的频点超出缩放范围，隐藏*/
            }
            else
            {
                 mark.Visibility = Visibility.Visible;
            }
        }

        private SpectrumScanMark AddScanFreqMark(Color pMarkColor)
        {
            var mark = new SpectrumScanMark
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Color = new SolidColorBrush(pMarkColor),
                Foreground = new SolidColorBrush(Colors.White),
                MarkVisibility = Visibility.Visible,
            };

            double xvalue = XAxisMinimum;
            double yUnitLength = uxChartBackground.ActualHeight / (YAxisMaximum - YAxisMinimum);
            mark.TranslateX = 0;
            mark.MarkFreValue = xvalue;
            mark.Visibility = Visibility.Collapsed;
            x_ScanfreqMarkPanel.Children.Add(mark);

            if (_dataPoints.Count == 0) return mark;

            var lst = _dataPoints.Where(k => k.Value.YAxisValue > ThresholdValue);

            if (!lst.Any())
            {
                return null;
            }

            double key = lst.Min(k => Math.Abs(k.Key - xvalue));

            DataPoint dp;
            if (_dataPoints.ContainsKey(key + xvalue))
            {
                dp = _dataPoints[key + xvalue];
            }
            else
            {
                dp = _dataPoints[xvalue - key];
            }

            double x = (dp.XAxisValue - XAxisMinimum) * _xAxisUnitLength;
            if (x < EdgeMargin)
                x = EdgeMargin;
            else if (x > (uxChartBackground.ActualWidth - EdgeMargin))
                x = uxChartBackground.ActualWidth - EdgeMargin;
            mark.TranslateX = x + 1;
            mark.TranslateY = dp.YAxisValue;
            mark.MarkFreValue = dp.XAxisValue;
            mark.MarkOffset = uxChartBackground.ActualHeight - (dp.YAxisValue - YAxisMinimum) * yUnitLength - 28 ;
            mark.UpdateLayout();

            return mark;
        }

        internal void ClearScanFreqMark()
        {
            List<SpectrumScanMark> marks = new List<SpectrumScanMark>();
            foreach (var v in x_ScanfreqMarkPanel.Children)
            {
                SpectrumScanMark sp = v as SpectrumScanMark;
                if (sp != null)
                {
                    marks.Add(sp);
                }
            }

            if (marks.Any())
            {
                var mark = marks.First();
                x_ScanfreqMarkPanel.Children.Remove(mark);
            }
        }
        private void UpdateScanFreqMarks()
        {
            if (x_ScanfreqMarkPanel.Children.Count == 0)
                return;
            foreach (var m in x_ScanfreqMarkPanel.Children)
            {
                var mark = m as SpectrumScanMark;
                if (mark == null)
                    continue;

                MoveScanMark(mark.MarkFreValue, mark.TranslateY);
            }
        }
        #endregion ScanFreqPoint

        #region 触摸屏谱图缩放操作
        private TouchPoint _starttouchPoint;
        private TouchPoint _moveoldpoint;
        private int movecount = 0;
        private Dictionary<int, TouchPoint> _dicOldTouchPoint = new Dictionary<int, TouchPoint>();
        private Dictionary<int, TouchPoint> _dicNewTouchPoint = new Dictionary<int, TouchPoint>();
        void uxChartBackground_TouchDown(object sender, TouchEventArgs e)
        {
            _panning = true;

            if (_dicOldTouchPoint.Count >= 3)
            {
                _dicOldTouchPoint.Clear();
            }
            _starttouchPoint = e.GetTouchPoint(uxChartBackground);
            _dicOldTouchPoint[e.TouchDevice.Id] = _starttouchPoint;
            if (_scaleHistory.Count == 0)
            {
                _xMinInitValue = XAxisMinimum;
                _xMaxInitValue = XAxisMaximum;
            }
        }
        void uxChartBackground_TouchUp(object sender, TouchEventArgs e)
        {
            _panning = false;

            if (_dataPoints.Count == 0)
            {
                return;
            }

            Point pt1 = new Point(0, 0);
            Point pt2 = new Point(0, 0);
            bool bzoom = true;
            if (_dicNewTouchPoint.Count == 2)
            {
                if (_dicNewTouchPoint.First().Value.Position.X > _dicNewTouchPoint.Last().Value.Position.X)
                {
                    pt2 = new Point(_dicNewTouchPoint.First().Value.Position.X, _dicNewTouchPoint.First().Value.Position.Y);
                    pt1 = new Point(_dicNewTouchPoint.Last().Value.Position.X, _dicNewTouchPoint.Last().Value.Position.Y);
                }
                else
                {
                    pt1 = new Point(_dicNewTouchPoint.First().Value.Position.X, _dicNewTouchPoint.First().Value.Position.Y);
                    pt2 = new Point(_dicNewTouchPoint.Last().Value.Position.X, _dicNewTouchPoint.Last().Value.Position.Y);
                }
                // 计算框选的x轴的范围
                double xv1 = (pt1.X - EdgeMargin) / _xAxisUnitLength + XAxisMinimum;
                double xv2 = (pt2.X - EdgeMargin) / _xAxisUnitLength + XAxisMinimum;
                if (xv1 >= xv2)
                {
                    return;
                }

                if (Math.Abs((_dicNewTouchPoint.First().Value.Position.X - _dicNewTouchPoint.Last().Value.Position.X)) < Math.Abs((_dicOldTouchPoint.First().Value.Position.X - _dicOldTouchPoint.Last().Value.Position.X)))
                {
                    bzoom = false;
                }
                else
                {
                    bzoom = true;
                }
                if (pt1.X != 0 && pt2.X != 0)
                {
                    if (bzoom)
                    {
                        if (uxZoomButtons.Children.Count >= 7)
                            return;
                        var btn = new ImageButton
                        {
                            Margin = new Thickness(5, 0, 0, 0),
                            Padding = new Thickness(5, 0, 5, 0),
                            Foreground = new SolidColorBrush(Colors.White),
                            Tag = uxZoomButtons.Children.Count,
                            Content = uxZoomButtons.Children.Count + 1,
                            Width = 40,
                            Height = 30
                        };

                        XAxisMinimum = xv1;
                        XAxisMaximum = xv2;
                        // 记录缩放的x轴的范围和对应的按钮
                        _scaleHistory.Add(new Tuple<double, double, Button>(XAxisMinimum, XAxisMaximum, btn));
                        uxZoomButtons.Children.Add(btn);

                        btn.TouchDown += (o, args) =>
                        {
                            var button = o as Button;
                            if (button == null) return;

                            var index = (int)button.Tag;
                            if (_scaleHistory.Count < index || index < 0) return;

                            var tuple = _scaleHistory[index];

                            XAxisMinimum = tuple.Item1;
                            XAxisMaximum = tuple.Item2;
                            UpdateFreqMarks();
                        };
                    }
                    else
                    {
                        if (_scaleHistory.Count == 0) return;

                        _scaleHistory.Clear();
                        uxZoomButtons.Children.Clear();

                        XAxisMinimum = _xMinInitValue;
                        XAxisMaximum = _xMaxInitValue;
                    }

                }
            }
            UpdateFreqMarks();

            movecount = 0;
            _dicNewTouchPoint.Clear();
            _dicOldTouchPoint.Clear();
        }

        void uxChartBackground_TouchMove(object sender, TouchEventArgs e)
        {
            movecount++;
            if (!_panning) return;
            if (_dataPoints.Count == 0) return;

            if (_dicNewTouchPoint.Count >= 3)
            {
                _dicNewTouchPoint.Clear();
            }

            TouchPoint pt = e.GetTouchPoint(uxChartBackground);
            _dicNewTouchPoint[e.TouchDevice.Id] = pt;

            if (_dicNewTouchPoint.Count == 1 && _dicOldTouchPoint.Count == 1)
            {
                double startX = _xMinInitValue;
                double endX = _xMaxInitValue;

                double ss = uxChartBackground.ActualWidth / (XAxisMaximum - XAxisMinimum);
                if (movecount == 1)
                {
                    double moves = Math.Abs(_dicNewTouchPoint.First().Value.Position.X - _dicOldTouchPoint.First().Value.Position.X) / ss;
                    if (_dicNewTouchPoint.First().Value.Position.X > _dicOldTouchPoint.First().Value.Position.X)
                    {
                        if (XAxisMinimum == startX)
                        {
                            _moveoldpoint = pt;
                            return;
                        }

                        XAxisMinimum = XAxisMinimum - moves;
                        XAxisMaximum = XAxisMaximum - moves;
                    }
                    else
                    {
                        if (XAxisMaximum == endX)
                        {
                            _moveoldpoint = pt;
                            return;
                        }
                        XAxisMinimum = XAxisMinimum + moves;
                        XAxisMaximum = XAxisMaximum + moves;
                    }
                }
                else if (movecount > 1)
                {
                    double moves = Math.Abs(pt.Position.X - _moveoldpoint.Position.X) / ss;
                    if (_dicNewTouchPoint.First().Value.Position.X > _moveoldpoint.Position.X)
                    {
                        if (XAxisMinimum == startX)
                        {
                            _moveoldpoint = pt;
                            return;
                        }
                        XAxisMinimum = XAxisMinimum - moves;
                        XAxisMaximum = XAxisMaximum - moves;
                    }
                    else
                    {
                        if (XAxisMaximum == endX)
                        {
                            _moveoldpoint = pt;
                            return;
                        }
                        XAxisMinimum = XAxisMinimum + moves;
                        XAxisMaximum = XAxisMaximum + moves;
                    }
                }

                if (XAxisMinimum < startX)
                {
                    double mm = startX - XAxisMinimum;
                    XAxisMinimum = startX;
                    XAxisMaximum = XAxisMaximum + mm;
                }
                if (XAxisMaximum > endX)
                {
                    double mm = XAxisMaximum - endX;
                    XAxisMinimum = XAxisMinimum - mm;
                    XAxisMaximum = endX;
                }

                UpdateFreqMarks();
            }

            _moveoldpoint = pt;
        }
       
        #endregion

        #region 触摸屏谱图幅度值缩放操作
        private Dictionary<int, TouchPoint> _dbuvTopOldTouchPoint = new Dictionary<int, TouchPoint>();
        private Dictionary<int, TouchPoint> _dbuvTopNewTouchPoint = new Dictionary<int, TouchPoint>();
        private void x_dbuvTopPanelGrid_TouchDown(object sender, TouchEventArgs e)
        {
            if (_dbuvTopOldTouchPoint.Count > 1)
            {
                _dbuvTopOldTouchPoint.Clear();
            }
            Canvas can = e.OriginalSource as Canvas;
            if (can != null)
            {
                _dbuvTopOldTouchPoint[e.TouchDevice.Id] = e.GetTouchPoint(can);
                can.CaptureTouch(e.TouchDevice);
            }
        }
        private TouchPoint _dbuvTopmoveoldpoint;
        private int _dbuvTopmovecount = 0;
        private void x_dbuvTopPanelGrid_TouchMove(object sender, TouchEventArgs e)
        {
            double height = ((Canvas)sender).ActualHeight;
            _dbuvTopmovecount++;
            if (_dbuvTopNewTouchPoint.Count > 1)
            {
                _dbuvTopNewTouchPoint.Clear();
            }
            TouchPoint pt = e.GetTouchPoint((Canvas)sender);
            //if (pt.Position.Y < height && pt.Position.Y > height - 1)
            //{
            //    x_dbuvTopPanelGrid_TouchUp(null, null);
            //    return;
            //}
            _dbuvTopNewTouchPoint[e.TouchDevice.Id] = pt;

            if (_dbuvTopNewTouchPoint.Count == 1 && _dbuvTopOldTouchPoint.Count == 1)
            {
                double ss = uxChartBackground.ActualHeight / (YAxisMaximum - YAxisMinimum);
                if (_dbuvTopmovecount == 1)
                {
                    double moves = Math.Abs(_dbuvTopNewTouchPoint.First().Value.Position.Y - _dbuvTopOldTouchPoint.First().Value.Position.Y) / ss;
                    if (_dbuvTopNewTouchPoint.First().Value.Position.Y > _dbuvTopOldTouchPoint.First().Value.Position.Y)
                    {
                        if (YAxisMinimum + YAxisInterval >= YAxisMaximum)
                            return;
                        YAxisMaximum -= moves;

                        x_btnDbuvTop2.Visibility = Visibility.Collapsed;
                        x_btnDbuvTop1.Visibility = Visibility.Visible;
                    }
                    else if (_dbuvTopNewTouchPoint.First().Value.Position.Y < _dbuvTopOldTouchPoint.First().Value.Position.Y)
                    {
                        YAxisMaximum += moves;

                        x_btnDbuvTop2.Visibility = Visibility.Visible;
                        x_btnDbuvTop1.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        _dbuvTopmoveoldpoint = pt;
                        return;
                    }
                }
                else if (_dbuvTopmovecount > 1)
                {
                    double moves = Math.Abs(pt.Position.Y - _dbuvTopmoveoldpoint.Position.Y) / ss;
                    if (_dbuvTopNewTouchPoint.First().Value.Position.Y > _dbuvTopmoveoldpoint.Position.Y)
                    {
                        if (YAxisMinimum + YAxisInterval >= YAxisMaximum)
                            return;
                        YAxisMaximum -= moves;

                        x_btnDbuvTop2.Visibility = Visibility.Collapsed;
                        x_btnDbuvTop1.Visibility = Visibility.Visible;
                    }
                    else if (_dbuvTopNewTouchPoint.First().Value.Position.Y < _dbuvTopmoveoldpoint.Position.Y)
                    {
                        YAxisMaximum += moves;

                        x_btnDbuvTop2.Visibility = Visibility.Visible;
                        x_btnDbuvTop1.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        _dbuvTopmoveoldpoint = pt;
                        return;
                    }
                }
                AxisMaximum();
            }
            _dbuvTopmoveoldpoint = pt;
        }

        private void x_dbuvTopPanelGrid_TouchUp(object sender, TouchEventArgs e)
        {
            _dbuvTopmovecount = 0;
            _dbuvTopNewTouchPoint.Clear();
            _dbuvTopOldTouchPoint.Clear();

            Canvas can = e.OriginalSource as Canvas;
            if (can != null)
            {
                can.ReleaseTouchCapture(e.TouchDevice);
            }

            x_btnDbuvTop1.Visibility = Visibility.Visible;
            x_btnDbuvTop2.Visibility = Visibility.Visible;
        }
        private Dictionary<int, TouchPoint> _dbuvBottomOldTouchPoint = new Dictionary<int, TouchPoint>();
        private Dictionary<int, TouchPoint> _dbuvBottomNewTouchPoint = new Dictionary<int, TouchPoint>();
        private void x_dbuvBottomPanelGrid_TouchDown(object sender, TouchEventArgs e)
        {
            if (_dbuvBottomOldTouchPoint.Count > 1)
            {
                _dbuvBottomOldTouchPoint.Clear();
            }

            Canvas can = e.OriginalSource as Canvas;
            if (can != null)
            {
                _dbuvBottomOldTouchPoint[e.TouchDevice.Id] = e.GetTouchPoint(can);
                can.CaptureTouch(e.TouchDevice);
            }
        }
        private TouchPoint _dbuvBottommoveoldpoint;
        private int _dbuvBottommovecount = 0;
        private void x_dbuvBottomPanelGrid_TouchMove(object sender, TouchEventArgs e)
        {
            _dbuvBottommovecount++;
            if (_dbuvBottomNewTouchPoint.Count > 1)
            {
                _dbuvBottomNewTouchPoint.Clear();
            }
            TouchPoint pt = e.GetTouchPoint((Canvas)sender);
            _dbuvBottomNewTouchPoint[e.TouchDevice.Id] = pt;

            if (_dbuvBottomNewTouchPoint.Count == 1 && _dbuvBottomOldTouchPoint.Count == 1)
            {
                double ss = uxChartBackground.ActualHeight / (YAxisMaximum - YAxisMinimum);
                if (_dbuvBottommovecount == 1)
                {
                    double moves = Math.Abs(_dbuvBottomNewTouchPoint.First().Value.Position.Y - _dbuvBottomOldTouchPoint.First().Value.Position.Y) / ss;
                    if (_dbuvBottomNewTouchPoint.First().Value.Position.Y > _dbuvBottomOldTouchPoint.First().Value.Position.Y)
                    {
                        if (YAxisMinimum + YAxisInterval >= YAxisMaximum)
                            return;

                        YAxisMinimum += moves;

                        x_btnDbuvBottom2.Visibility = Visibility.Visible;
                        x_btnDbuvBottom1.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        YAxisMinimum -= moves;

                        x_btnDbuvBottom2.Visibility = Visibility.Collapsed;
                        x_btnDbuvBottom1.Visibility = Visibility.Visible;
                    }
                }
                else if (_dbuvBottommovecount > 1)
                {
                    double moves = Math.Abs(pt.Position.Y - _dbuvBottommoveoldpoint.Position.Y) / ss;
                    if (_dbuvBottomNewTouchPoint.First().Value.Position.Y > _dbuvBottommoveoldpoint.Position.Y)
                    {
                        if (YAxisMinimum + YAxisInterval >= YAxisMaximum)
                            return;

                        YAxisMinimum += moves;

                        x_btnDbuvBottom2.Visibility = Visibility.Visible;
                        x_btnDbuvBottom1.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        YAxisMinimum -= moves;

                        x_btnDbuvBottom2.Visibility = Visibility.Collapsed;
                        x_btnDbuvBottom1.Visibility = Visibility.Visible;
                    }
                }
                AxisMininum();
            }
            _dbuvBottommoveoldpoint = pt;
        }

        private void x_dbuvBottomPanelGrid_TouchUp(object sender, TouchEventArgs e)
        {
            _dbuvBottommovecount = 0;
            _dbuvBottomNewTouchPoint.Clear();
            _dbuvBottomOldTouchPoint.Clear();

            Canvas can = e.OriginalSource as Canvas;
            if (can != null)
            {
                can.ReleaseTouchCapture(e.TouchDevice);
            }

            x_btnDbuvBottom2.Visibility = Visibility.Visible;
            x_btnDbuvBottom1.Visibility = Visibility.Visible;
        }
        #endregion
    }

    internal class DsMarkPanelMenuCommand : ICommand
    {
        private readonly DiscreteScanGraph _discreteScan;

        public DsMarkPanelMenuCommand(DiscreteScanGraph discreteScan)
        {
            _discreteScan = discreteScan;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            if (parameter == null)
                return;
            switch (parameter.ToString())
            {
                case "addYAxisMark":
                    _discreteScan.AddYAxisMark(Color.FromArgb(0xff, 0xd3, 0x80, 0x00));
                    break;

                case "clearYAxisMark":
                    _discreteScan.ClearYAxisMark();
                    break;

                case "addXAxisMark":
                    _discreteScan.AddXAxisMark(Color.FromArgb(0xff, 0xd3, 0x80, 0x00));
                    break;

                case "clearXAxisMark":
                    _discreteScan.ClearXAxisMark();
                    break;
            }
        }

        #endregion ICommand Members
    }

    internal class ShowThresholdCommand : ICommand
    {
        private readonly Func<object, bool> _canExecute = o => true;
        private readonly Action<object> _execute = o => { };

        public ShowThresholdCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion ICommand Members
    }
}
