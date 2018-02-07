using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using Best.VWPlatform.Common.Utility;
using Best.VWPlatform.Controls.Freq;

namespace Best.VWPlatform.Controls.Container
{
    /// <summary>
    /// DiscreteWaterfall.xaml 的交互逻辑
    /// </summary>
    public partial class DiscreteWaterfall : UserControl
    {
        private const int EdgeMargin = 5;
        private bool _receiveData = true;
        private SpectrumMark _spectrumMark;
        private readonly Dictionary<double, List<Tuple<double, Point, Color>>> _waterfallHistory = new Dictionary<double, List<Tuple<double, Point, Color>>>();
        private readonly Dictionary<int, TextBlock> _coordinateY = new Dictionary<int, TextBlock>();
        private readonly Dictionary<int, TextBlock> _coordinateY1 = new Dictionary<int, TextBlock>();

        private Size _previousSize;
        private int _timestampCount;
        private ObservableCollection<string> _timestamps = new ObservableCollection<string>();

        #region Dependency property

        public static readonly DependencyProperty XAxisMinimumProperty =
            DependencyProperty.Register("XAxisMinimum", typeof(double), typeof(DiscreteWaterfall),
                                        new PropertyMetadata(1.0, XAxisMinimumPropertyChangedCallback));

        public static readonly DependencyProperty XAxisMaximumProperty =
            DependencyProperty.Register("XAxisMaximum", typeof(double), typeof(DiscreteWaterfall),
                                        new PropertyMetadata(300.0, XAxisMaximumPropertyChangedCallback));

        public static readonly DependencyProperty YAxisMinimumProperty =
            DependencyProperty.Register("YAxisMinimum", typeof(double), typeof(DiscreteWaterfall),
                                        new PropertyMetadata(-20.0, YAxisMinimumPropertyChangedCallback));

        public static readonly DependencyProperty YAxisMaximumProperty =
            DependencyProperty.Register("YAxisMaximum", typeof(double), typeof(DiscreteWaterfall),
                                        new PropertyMetadata(80.0, YAxisMaximumPropertyChangedCallback));

        public static readonly DependencyProperty YAxisIntervalProperty =
            DependencyProperty.Register("YAxisInterval", typeof(double), typeof(DiscreteWaterfall),
                                        new PropertyMetadata(20.0));

        public static readonly DependencyProperty MeasureUnitProperty =
            DependencyProperty.Register("MeasureUnit", typeof(string), typeof(DiscreteWaterfall), new PropertyMetadata("[dBμV]"));

        public static readonly DependencyProperty YAxisUnitVisibilityProperty =
            DependencyProperty.Register("YAxisUnitVisibility", typeof(Visibility), typeof(DiscreteWaterfall), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty YaxisRightVisibilityProperty =
            DependencyProperty.Register("YaxisRightVisibility", typeof(Visibility), typeof(DiscreteWaterfall), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty DefaultForeColorProperty =
            DependencyProperty.Register("DefaultForeColor", typeof(Brush), typeof(DiscreteWaterfall), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public static readonly DependencyProperty RightArrowVisibilityProperty =
     DependencyProperty.Register("RightArrowVisibility", typeof(Visibility), typeof(DiscreteWaterfall), new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty MeasureUnitForeColorProperty =
            DependencyProperty.Register("MeasureUnitForeColor", typeof(Brush), typeof(DiscreteWaterfall), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xa8, 0x00))));

        public Brush DefaultForeColor
        {
            get { return (Brush)GetValue(DefaultForeColorProperty); }
            set { SetValue(DefaultForeColorProperty, value); }
        }

        public Visibility YaxisRightVisibility
        {
            get { return (Visibility)GetValue(YaxisRightVisibilityProperty); }
            set { SetValue(YaxisRightVisibilityProperty, value); }
        }

        public Visibility YAxisUnitVisibility
        {
            get { return (Visibility)GetValue(YAxisUnitVisibilityProperty); }
            set { SetValue(YAxisUnitVisibilityProperty, value); }
        }

        public string MeasureUnit
        {
            get { return (string)GetValue(MeasureUnitProperty); }
            set { SetValue(MeasureUnitProperty, value); }
        }

        public Visibility RightArrowVisibility
        {
            get { return (Visibility)GetValue(YAxisUnitVisibilityProperty); }
            set { SetValue(YAxisUnitVisibilityProperty, value); }
        }

        /// <summary>
        /// y轴间隔
        /// </summary>
        public double YAxisInterval
        {
            get { return (double)GetValue(YAxisIntervalProperty); }
            set { SetValue(YAxisIntervalProperty, value); }
        }

        /// <summary>
        /// y轴最大值
        /// </summary>
        public double YAxisMaximum
        {
            get { return (double)GetValue(YAxisMaximumProperty); }
            set { SetValue(YAxisMaximumProperty, value); }
        }

        /// <summary>
        /// y轴最小值
        /// </summary>
        public double YAxisMinimum
        {
            get { return (double)GetValue(YAxisMinimumProperty); }
            set { SetValue(YAxisMinimumProperty, value); }
        }

        /// <summary>
        /// x轴最大值
        /// </summary>
        public double XAxisMaximum
        {
            get { return (double)GetValue(XAxisMaximumProperty); }
            set { SetValue(XAxisMaximumProperty, value); }
        }

        /// <summary>
        /// x轴最小值
        /// </summary>
        public double XAxisMinimum
        {
            get { return (double)GetValue(XAxisMinimumProperty); }
            set { SetValue(XAxisMinimumProperty, value); }
        }

        public Brush MeasureUnitForeColor
        {
            get { return (Brush)GetValue(MeasureUnitForeColorProperty); }
            set { SetValue(MeasureUnitForeColorProperty, value); }
        }
        #endregion

        public ObservableCollection<string> Timestamps
        {
            get { return _timestamps; }
            set
            {
                _timestamps = value;
            }
        }

        /// <summary>
        /// step MHz
        /// </summary>
        public double Step
        {
            get { return _step; }
            set
            {
                _step = value;
                _maxColumn = (int)((XAxisMaximum - XAxisMinimum) / value) + 1;
            }
        }

        /// <summary>
        /// X 轴单位长度
        /// </summary>
        public double XUnitLength
        {
            get { return _xUnitLength; }
            set { _xUnitLength = value; }
        }

        /// <summary>
        /// 门限值
        /// </summary>
        public double ThresholdValue { get; set; }

        public bool IsCompletion
        {
            get { return _isCompletion; }
            set { _isCompletion = value; }
        }

        #region Callback handler

        private static void YAxisMinimumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dw = (DiscreteWaterfall)d;
            if (dw.uxYAxis == null) return;

            //dw.CreateYAxis();hjy
            dw.AxisMininum();
            dw._cachedDatas.Clear();
            dw._cachedColors.Clear();
        }

        private static void YAxisMaximumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dw = (DiscreteWaterfall)d;
            if (dw.uxYAxis == null) return;

            //dw.CreateYAxis();
            dw.AxisMaximum();
            dw._cachedDatas.Clear();
            dw._cachedColors.Clear();
        }

        private static void XAxisMinimumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dw = (DiscreteWaterfall)d;
            if (dw.uxWaterfall == null) return;

            dw.InitWaterfall();
        }

        private static void XAxisMaximumPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dw = (DiscreteWaterfall)d;
            if (dw.uxWaterfall == null) return;

            dw.InitWaterfall();
        }

        #endregion

        public DiscreteWaterfall()
        {
            InitializeComponent();

            DataContext = this;

            CreateYAxis();
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (_previousSize.Width.Equals(sizeChangedEventArgs.NewSize.Width) &&
    _previousSize.Height.Equals(sizeChangedEventArgs.NewSize.Height))
                return;
            _previousSize = sizeChangedEventArgs.NewSize;

            _receiveData = false;

            Clear();
            InitWaterfall();
            //CreateYAxis();
            _receiveData = true;
        }
        #region  控制谱图纵坐标刻度显示
        private void AxisMininum()
        {
            _yUnitLength = uxChartBackground.ActualHeight / (YAxisMaximum - YAxisMinimum);

            if (_coordinateY.Count == 0 || _coordinateY1.Count == 0)
            {
                return;
            }

            var axisSpan = (Int32)(YAxisMaximum - YAxisMinimum) / 5;
              _coordinateY[0].Text = ((int)YAxisMaximum).ToString();
            //_coordinateY1[0].Text = ((int)YAxisMaximum).ToString();
            for (int i = 1; i < _coordinateY.Count - 1; i++)
            {
                _coordinateY[i].Text = (YAxisMinimum + (_coordinateY.Count - 1 - i) * axisSpan).ToString();
                //_coordinateY1[i].Text = (YAxisMinimum + (_coordinateY.Count - 1 - i) * axisSpan).ToString();
            }
            _coordinateY[_coordinateY.Count - 1].Text = (YAxisMinimum).ToString();
            //_coordinateY1[_coordinateY.Count - 1].Text = (YAxisMinimum).ToString();
        }
        private void AxisMaximum()
        {
            _yUnitLength = uxChartBackground.ActualHeight / (YAxisMaximum - YAxisMinimum);

            var axisSpan = (Int32)(YAxisMaximum - YAxisMinimum) / 5;
            if (_coordinateY.Count == 0) return;

            _coordinateY[0].Text = ((int)YAxisMaximum).ToString();
            //_coordinateY1[0].Text = ((int)YAxisMaximum).ToString();
            for (int i = 1; i < _coordinateY.Count - 1; i++)
            {
                _coordinateY[i].Text = (YAxisMaximum - i * axisSpan).ToString();
                //_coordinateY1[i].Text = (YAxisMaximum - i * axisSpan).ToString();
            }
            _coordinateY[_coordinateY.Count - 1].Text = (YAxisMinimum).ToString();
            //_coordinateY1[_coordinateY.Count - 1].Text = (YAxisMinimum).ToString();
        }
        #endregion 
        #region Private fields

        private readonly SolidColorBrush _whiteBrush = new SolidColorBrush(Colors.White);
        private readonly SolidColorBrush _blankBrush = new SolidColorBrush(Color.FromArgb(255, 9, 9, 9));
        private readonly SolidColorBrush _yellowBrush = new SolidColorBrush(Color.FromArgb(255, 249, 167, 0));
        private double _xUnitLength = 1;
        private double _yUnitLength = 1;

        private WriteableBitmap _writeableBmp;
        private readonly Dictionary<int, Color> _cachedColors = new Dictionary<int, Color>();
        /// <summary>
        /// 频率、值
        /// </summary>
        private readonly Dictionary<double, short> _cachedDatas = new Dictionary<double, short>();
        private int _maxColumn, _maxRow, _row;
        private double _step;
        private bool _isCompletion;

        #endregion

        private void InitWaterfall()
        {
            uxWaterfall.Width = uxChartBackground.ActualWidth;
            uxWaterfall.Height = uxChartBackground.ActualHeight;

            if (uxWaterfall.Width.Equals(0) || uxWaterfall.Height.Equals(0))
            {
                return;
            }

            XUnitLength = (uxWaterfall.Width - 10) / (XAxisMaximum - XAxisMinimum);
            _writeableBmp = new WriteableBitmap((int)uxWaterfall.Width, (int)uxWaterfall.Height, 96, 96, PixelFormats.Pbgra32, null);
            _yUnitLength = uxChartBackground.ActualHeight / (YAxisMaximum - YAxisMinimum);
            _writeableBmp.FillRectangle(0, 0, (int)uxWaterfall.Width, (int)uxWaterfall.Height, Colors.Transparent);
            uxWaterfall.Source = _writeableBmp;
        }

        public void BeginSave()
        {
            MeasureUnitForeColor = new SolidColorBrush(Colors.Black);
            DefaultForeColor = _blankBrush;

            //保存时点颜色变更

            var color = Colors.Black;
            foreach (var dots in _waterfallHistory)
            {
                foreach (var c in dots.Value)
                {
                    _writeableBmp.SetPixel((int)c.Item2.X, (int)c.Item2.Y, color.A, color.R, color.G, color.B);
                }
            }
            //if (_waterfallHistory.Count > 0)
            //    _writeableBmp.Invalidate();

        }

        public void EndSave()
        {
            MeasureUnitForeColor = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xa8, 0x00));
            DefaultForeColor = _whiteBrush;

            // 保存结束时点颜色恢复
            foreach (var dots in _waterfallHistory)
            {
                foreach (var c in dots.Value)
                {
                    _writeableBmp.SetPixel((int)c.Item2.X, (int)c.Item2.Y, 85, c.Item3.R, c.Item3.G, c.Item3.B);
                }
            }
            //if (_waterfallHistory.Count > 0)
            //    _writeableBmp.Invalidate();
        }

        private int c1 = 1;
        /// <summary>
        /// 绘制谱图
        /// </summary>
        /// <param name="data">频率、值</param>
        public void DrawData(Dictionary<double, short> data, int pRemainder, DateTime? pTime = null)
        {
            if (_writeableBmp == null) return;
            if (!_receiveData) return;

            if (_writeableBmp.PixelHeight == 0 || _writeableBmp.PixelWidth == 0)
            {
                _writeableBmp = new WriteableBitmap((int)uxWaterfall.Width, (int)uxWaterfall.Height, 96, 96, PixelFormats.Pbgra32, null);
                _writeableBmp.FillRectangle(0, 0, (int)uxWaterfall.Width, (int)uxWaterfall.Height, Colors.Transparent);
                uxWaterfall.Source = _writeableBmp;
            }

            foreach (var fv in data)
            {
                if (_maxColumn == 0 || _maxColumn < _writeableBmp.PixelWidth)
                {
                    _maxColumn = _writeableBmp.PixelWidth;
                }
                _cachedDatas[fv.Key] = fv.Value;
            }

            _writeableBmp.Clear(Colors.Transparent);

            var c = (_maxRow * (_timestampCount - 1) / _writeableBmp.PixelHeight) % _timestampCount;
            if (c != c1)
            {
                Timestamps.RemoveAt(0);
                Timestamps.Insert(_timestampCount - 1, pTime == null ? "--:--:--" : pTime.Value.ToLongTimeString());
                c1 = c;
            }

            if (_isCompletion)
            {
                _maxRow += 1;
            }
            else
            {
                _maxRow = _cachedDatas.Count;
            }

            _writeableBmp.FillRectangle(0, 0, _writeableBmp.PixelWidth, _maxRow > _writeableBmp.PixelHeight ? _writeableBmp.PixelHeight : _maxRow, Color.FromArgb(210, 22, 22, 255));
            _row = _maxRow;
            foreach (var tuple in _cachedDatas)
            {
                if (tuple.Key < XAxisMinimum || tuple.Key > XAxisMaximum)
                    continue;
                var currFreq = Math.Floor(tuple.Key);
                var xAxisMaximumKhz = WMonitorUtile.ConvertFreqValue("mhz", "khz", XAxisMaximum);
                var xAxisMinimumKhz = WMonitorUtile.ConvertFreqValue("mhz", "khz", XAxisMinimum);
                var xKey = WMonitorUtile.ConvertFreqValue("mhz", "khz", tuple.Key);
                //if (currFreq.Equals(Math.Floor(XAxisMaximum)) || WMonitorUtile.ConvertFreqValue("mhz", "khz", tuple.Key).Equals(XAxisMaximumKhz - pRemainder))//起始减去终止频率除以步进有余数时,瀑布图显示一半的bug 
                //if (currFreq.Equals(Math.Floor(XAxisMaximum)) || (xAxisMaximumKhz - xKey) < pRemainder)
                //{
                    _isCompletion = true;
                //}

                var col = (int)((tuple.Key - XAxisMinimum) * XUnitLength) + 1;
                if (col < EdgeMargin)
                    col = EdgeMargin;
                if (col > _writeableBmp.PixelWidth)
                    col = _writeableBmp.PixelWidth - 1;

                var y = tuple.Value;
                if (y <= YAxisMinimum || y < ThresholdValue) //门限
                    continue;

                if (_cachedColors.Keys.Contains(y))
                {
                    _writeableBmp.DrawLine(col, 0, col, _row, _cachedColors[y]);
                }
                else
                {
                    _cachedColors[y] = GetColorAtPoint(uxYAxisRect,
                                                       new Point(uxYAxisRect.ActualWidth,
                                                                 uxYAxisRect.ActualHeight - Math.Abs(y - YAxisMinimum) * _yUnitLength));
                    _writeableBmp.DrawLine(col, 0, col, _row, _cachedColors[y]);
                }
                if (!IsCompletion || _maxRow >= _cachedDatas.Count)
                    _row--;
            }
           // _writeableBmp.Invalidate();
        }

        /// <summary>
        /// 绘制瀑布图
        /// </summary>
        /// <param name="step"></param>
        /// <param name="sequency"></param>
        /// <param name="dValues"></param>
        public void DrawWaterfall(double step, int sequency, short[] dValues)
        {
            if (_writeableBmp == null || dValues == null) return;
            var len = dValues.Length;
            //if (_writeableBmp.Pixels.Count() == 0)
            //    return;

            // 删除旧数据
            var time = DateTime.Now.TimeOfDay.TotalSeconds;
            foreach (var dots in _waterfallHistory)
            {
                var oldValues = new List<Tuple<double, Point, Color>>(dots.Value);
                foreach (var oldValue in oldValues)
                {
                    if (time - oldValue.Item1 > 5)
                    {
                        _waterfallHistory[dots.Key].Remove(oldValue);
                        _writeableBmp.SetPixel((int)oldValue.Item2.X, (int)oldValue.Item2.Y, 0, 0, 0, 0);
                    }
                }
            }

            // 绘制瀑布图
            foreach (var dots in _waterfallHistory)
            {
                foreach (var c in dots.Value)
                {
                    _writeableBmp.SetPixel((int)c.Item2.X, (int)c.Item2.Y, 85, c.Item3.R, c.Item3.G, c.Item3.B);
                }
            }
            //if (_waterfallHistory.Count > 0)
            //    _writeableBmp.Invalidate();

            // 添加新数据
            var now = DateTime.Now.TimeOfDay.TotalSeconds;
            for (var i = 0; i < len; i++)
            {
                var dvalue = dValues[i];
                if (dvalue == 0) continue;

                var xfreq = XAxisMinimum + step * (sequency + i);
                var x = (_writeableBmp.PixelWidth / (XAxisMaximum - XAxisMinimum)) * (xfreq - XAxisMinimum);
                var y = (_writeableBmp.PixelHeight / (YAxisMaximum - YAxisMinimum)) * dvalue;

                if (y < 1 || y > _writeableBmp.PixelHeight) y = 1;
                y = _writeableBmp.PixelHeight - y;

                if (x >= _writeableBmp.PixelWidth)
                    x = _writeableBmp.PixelWidth - 1;

                var dot = new Tuple<double, Point, Color>(now, new Point(x, y), Colors.White);

                if (_waterfallHistory.ContainsKey(xfreq))
                {
                    _waterfallHistory[xfreq].Add(dot);
                }
                else
                {
                    _waterfallHistory.Add(xfreq, new List<Tuple<double, Point, Color>> { dot });
                }
            }
        }

        public void MoveXAxisMark(double translatex)
        {
            if (_spectrumMark == null)
            {
                _spectrumMark = new SpectrumMark
                {
                    Direction = MarkDirection.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Color = new SolidColorBrush(Color.FromArgb(0xff, 0x33, 0xf0, 0x10)),
                    Foreground = new SolidColorBrush(Colors.White),
                    MarkArrowVisibility = Visibility.Collapsed,
                    VerticalLineVisibility = Visibility.Visible,
                    BuoyVisibility = Visibility.Collapsed,
                    TooltipVisibility = Visibility.Collapsed,
                    GroupName = "-1"
                };

                uxXAxisMarkPanel.Children.Add(_spectrumMark);
            }
            _spectrumMark.TranslateX = translatex;
            _spectrumMark.UpdateLayout();
        }

        /// <summary>
        /// 建Y轴
        /// </summary>
        private void CreateYAxis()
        {
            _coordinateY.Clear();
            _coordinateY1.Clear();

            #region y轴

            uxYAxis.Children.Clear();
            uxYAxis.RowDefinitions.Clear();

            uxYAxisRight.Children.Clear();
            uxYAxisRight.RowDefinitions.Clear();

            var j = 0;
            _timestampCount = (int)((YAxisMaximum - YAxisMinimum) / YAxisInterval) + 1;
            InitTimestamps(_timestampCount);
            for (var i = YAxisMaximum; i > YAxisMinimum; i -= YAxisInterval, j++)
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
                CreateYscale(j, i);
            }
            if (j > 0)
            {
                CreateYscale(j, YAxisMinimum);
            }
            Grid.SetRowSpan(uxWaterfall, j + 1);
            //Grid.SetRowSpan(uxYaxisAdjust, j + 1);
            #endregion
        }

        /// <summary>
        /// 计算Y轴刻度、时间戳
        /// </summary>
        /// <param name="j">行</param>
        /// <param name="i">当i==YAxisMinimum时，绘制最底端Y轴</param>
        private void CreateYscale(int j, double i)
        {
            var txt = new TextBlock
            {
                VerticalAlignment = i.Equals(YAxisMinimum) ? VerticalAlignment.Bottom : VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Text = i.ToString(CultureInfo.InvariantCulture),
                //Foreground = _yellowBrush,
                FontSize = 10,
                Margin = i.Equals(YAxisMinimum) ? new Thickness(0, 0, 0, -5) : new Thickness(0, -5, 0, 0)
            };
            var binding = new Binding("YAxisUnitVisibility") { Source = this, Mode = BindingMode.TwoWay };
            txt.SetBinding(VisibilityProperty, binding);

                var foreBinding = new Binding("DefaultForeColor") { Mode = BindingMode.TwoWay };
                txt.SetBinding(TextBlock.ForegroundProperty, foreBinding);

            _coordinateY.Add(j, txt);
            uxYAxis.Children.Add(txt);
            Grid.SetRow(txt, j);

            var txtR = new TextBlock
            {
                VerticalAlignment = i.Equals(YAxisMinimum) ? VerticalAlignment.Bottom : VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                //Text = YAxisMinimum.ToString(CultureInfo.InvariantCulture),
                FontSize = 10,
                Margin = i.Equals(YAxisMinimum) ? new Thickness(0, 0, 0, -5) : new Thickness(0, -5, 0, 0)
            };

            var foreBinding1 = new Binding("DefaultForeColor") { Mode = BindingMode.TwoWay };
            txtR.SetBinding(TextBlock.ForegroundProperty, foreBinding1);

            txtR.DataContext = this;
            var binding1 = new Binding(string.Format("Timestamps[{0}]", _timestampCount - 1 - j)) { Source = this, Mode = BindingMode.OneWay };
            //var binding1 = new Binding(string.Format("[{0}]", v.Value)) { Source = this, Mode = BindingMode.OneWay };
            txtR.SetBinding(TextBlock.TextProperty, binding1);
            _coordinateY1.Add(j, txtR);
            uxYAxisRight.Children.Add(txtR);
            Grid.SetColumn(txtR, 1);
            Grid.SetRow(txtR, j);
        }

        private void InitTimestamps(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Timestamps.Add("--:--:--");
            }
        }

        public void Clear()
        {
            if (_writeableBmp != null)
            {
                uxWaterfall.Source = null;
                _writeableBmp = null;
                _writeableBmp = new WriteableBitmap((int)uxWaterfall.Width, (int)uxWaterfall.Height, 96, 96, PixelFormats.Pbgra32, null);
                _writeableBmp.FillRectangle(0, 0, (int)uxWaterfall.Width, (int)uxWaterfall.Height, Colors.Transparent);
                uxWaterfall.Source = _writeableBmp;
            }

            if (_spectrumMark != null)
            {
                uxXAxisMarkPanel.Children.Remove(_spectrumMark);
                _spectrumMark = null;
            }

            _maxRow = 0;
            _maxColumn = 0;
            _cachedDatas.Clear();
            _cachedColors.Clear();
            _waterfallHistory.Clear();
            _isCompletion = false;
        }

        #region 取颜色值
        /// <summary>
        /// 提取SolidColorBrush中某一点的颜色
        /// </summary>
        /// <param name="theRec">使用SolidColorBrush填充的Rectangle</param>
        /// <param name="thePoint">Rectangle中的点</param>
        /// <returns>Color</returns>
        private static Color GetColorAtPoint(Rectangle theRec, Point thePoint)
        {
            //Get properties
            var br = (LinearGradientBrush)theRec.Fill;

            double y3 = thePoint.Y;
            double x3 = thePoint.X;

            double x1 = br.StartPoint.X * theRec.ActualWidth;
            double y1 = br.StartPoint.Y * theRec.ActualHeight;
            var p1 = new Point(x1, y1); //Starting point

            double x2 = br.EndPoint.X * theRec.ActualWidth;
            double y2 = br.EndPoint.Y * theRec.ActualHeight;
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
                double m = (y2 - y1) / (x2 - x1);
                double m2 = -1 / m;
                double b = y1 - m * x1;
                double c = y3 - m2 * x3;

                double x4 = (c - b) / (m - m2);
                double y4 = m * x4 + b;
                p4 = new Point(x4, y4);
            }

            //Calculate distances relative to the vector start
            double d4 = Dist(p4, p1, p2);
            double d2 = Dist(p2, p1, p2);

            double x = d4 / d2;

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

            if (double.IsNaN(x)) return Colors.Green;

            //Find gradient stops that surround the input value
            GradientStop gs0 = br.GradientStops.Where(n => n.Offset <= x).OrderBy(n => n.Offset).Last();
            GradientStop gs1 = br.GradientStops.Where(n => n.Offset >= x).OrderBy(n => n.Offset).First();

            float y = 0f;
            if (gs0.Offset != gs1.Offset)
            {
                y = (float)((x - gs0.Offset) / (gs1.Offset - gs0.Offset));
            }

            //Interpolate color channels
            var aVal = (byte)((gs1.Color.A - gs0.Color.A) * y + gs0.Color.A);
            var rVal = (byte)((gs1.Color.R - gs0.Color.R) * y + gs0.Color.R);
            var gVal = (byte)((gs1.Color.G - gs0.Color.G) * y + gs0.Color.G);
            var bVal = (byte)((gs1.Color.B - gs0.Color.B) * y + gs0.Color.B);

            return Color.FromArgb(aVal, rVal, gVal, bVal);
        }

        //Returns the signed magnitude of a point on a vector with origin po and pointing to pf
        private static double Dist(Point px, Point po, Point pf)
        {
            double d = Math.Sqrt((px.Y - po.Y) * (px.Y - po.Y) + (px.X - po.X) * (px.X - po.X));
            if (((px.Y < po.Y) && (pf.Y > po.Y)) ||
                ((px.Y > po.Y) && (pf.Y < po.Y)) ||
                ((px.Y == po.Y) && (px.X < po.X) && (pf.X > po.X)) ||
                ((px.Y == po.Y) && (px.X > po.X) && (pf.X < po.X)))
            {
                d = -d;
            }
            return d;
        }
        #endregion
    }
}
