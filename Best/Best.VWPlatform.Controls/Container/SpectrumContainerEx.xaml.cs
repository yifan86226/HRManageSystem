using Best.VWPlatform.Common.Utility;
using Best.VWPlatform.Controls.Common;
using Best.VWPlatform.Controls.Freq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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

namespace Best.VWPlatform.Controls.Container
{
    /// <summary>
    /// SpectrumContainerEx.xaml 的交互逻辑
    /// </summary>
    public partial class SpectrumContainerEx : UserControl, IDisposable, INotifyPropertyChanged
    {
        #region 变量

        private readonly SpectrumDataCacheManage _spectrumDataManage = new SpectrumDataCacheManage();
        private readonly SpectrumDataCacheManage _spectrumBackgroundDataManage = new SpectrumDataCacheManage();
        private readonly SpectrumDataCacheManage _spectrumNoiseDatamanage = new SpectrumDataCacheManage();
        private readonly SpectrumDataCacheManage _spectrumMaxDataManage = new SpectrumDataCacheManage();
        private readonly SpectrumDataCacheManage _spectrumMinDataManage = new SpectrumDataCacheManage();
        private readonly Rectangle _rect = new Rectangle();
        private Point _mouseClickPoint;
        private bool _mouseDown;
        private Point _freqMarkInsertPoint;
        private Point _dbuvMarkInsertPoint;
        private SpectrumMark _thresholdMark;
        private bool _isMouseCaptured;
        private SpectrumType _spectrumType;
        private readonly Queue<Color> _markGroupColor = new Queue<Color>();
        private bool _initGroupIffqMark;
        private Color _backgroundLineColor;
        private Color _noiseLineColor;
        private Color _maxLineColor;
        private Color _minLineColor;
        private SpectrumLineType _backSpectrumLineType;
        private Color _realTimeLineColor;
        private SpectrumLineType _realTimeLineType;
        public static readonly DependencyProperty ThresholdMarkVisibilityProperty = DependencyProperty.Register("ThresholdMarkVisibility", typeof(Visibility), typeof(SpectrumContainerEx), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ThresholdValueProperty = DependencyProperty.Register("ThresholdValue", typeof(double), typeof(SpectrumContainerEx), new PropertyMetadata(10.0d, OnThresholdValuePropertyChangedCallback));
        private double _beginLeftValue;
        private double _endRightValue;
        private const string DefaultMajorMarkId = "DF8C9A4E-6FA0-4460-80C0-C9CB36C2F7CE";
        private const string DefaultThresholMarkId = "393BA9D0-E7F5-4AD2-9542-E4EB9A957276";
        private SpectrumMark _majorMarkObject;
        private double _zoomLeftFreq;
        private double _zoomRightFreq;
        private string _cursorPanelId = Guid.NewGuid().ToString();
        private bool _drawBackground;
        private bool _drawNoise;
        private bool _drawMax;
        private bool _drawMin;

        /// <summary>
        /// 中频带宽显示区域宽度
        /// </summary>
        private double _ifbwwidth;
        /// <summary>
        /// 中频带宽显示区域高度
        /// </summary>
        private double _ifbwheight;
        /// <summary>
        /// 中频扫描中心频率
        /// </summary>
        private double _ifbwfreq;
        /// <summary>
        /// 中频扫描中频带宽
        /// </summary>
        private double _ifqexifbw;
        /// <summary>
        ///中频荧光谱
        /// </summary>
        private bool _drawFluoro;
        private DateTime _interFreTime;
        private int _interFreDuration;
        /// <summary>
        /// 是否是触摸屏模式，true表示触摸屏
        /// </summary>
        //private bool _isTouch = ConfigurationManager.AppSettings["IsTouchModel"].Equals("1");
        private bool _isTouch = false;

        #endregion 变量
        public SpectrumContainerEx()
        {
            InitializeComponent();
            Loaded += SpectrumContainerEx_Loaded;
            Init();
        }

        void SpectrumContainerEx_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDbuvMarks();
        }

        #region Private

        /// <summary>
        /// 参数变化时谱图改变
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnThresholdValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sc = d as SpectrumContainerEx;
            if (!e.NewValue.Equals(e.OldValue))
            {
                if (sc != null)
                {
                    sc.UpdateDbuvMarks(0, (double)e.NewValue);
                }
            }
        }

        private void Init()
        {
            DefaultMajorMarkColor = Color.FromArgb(0xff, 0xd3, 0x80, 0x00);
            MeasureUnitForeground = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xa8, 0x00));
            DefaultLabelForeground = new SolidColorBrush(Colors.White);

            FreqGroupInfo = new FreqGroupInfo();
            MeasureUnit = "dBμV";
            xMeasureUnitPanel.SizeChanged += OnMeasureUnitPanelSizeChanged;
            Data.UpperLimitValue = 80;
            Data.LowerLimitValue = -20;
            Data.BeginLeftValue = 0;
            Data.EndRightValue = 100;
            DataContext = this;

            MarkMenuCommand = new MarkPanelMenuCommandEx(this);
            x_btnDbuvTop1.Command = new SpectrumContainerCommandEx(this);
            x_btnDbuvTop2.Command = new SpectrumContainerCommandEx(this);
            x_btnDbuvBottom1.Command = new SpectrumContainerCommandEx(this);
            x_btnDbuvBottom2.Command = new SpectrumContainerCommandEx(this);
            
             x_freqPanelGrid.SizeChanged += OnFreqPanelGridSizeChanged;
            //缩放操作
            if(!_isTouch)
            {
                _rect.Visibility = Visibility.Collapsed;
                _rect.Width = 0;
                _rect.Height = 0;
                _rect.Stroke = new SolidColorBrush(Color.FromArgb(0xff, 0xfa, 0xeb, 0xd7));
                _rect.StrokeThickness = 1;
                x_freqPanelCanvas.Children.Add(_rect);

                x_freqPanelGrid.MouseLeftButtonDown += OnFreqPanelMouseLeftButtonDown;
                x_freqPanelGrid.MouseLeftButtonUp += OnFreqPanelMouseLeftButtonUp;
                x_freqPanelGrid.MouseMove += OnFreqPanelMouseMove;
            }
            else
            {
                x_freqPanelGrid.TouchDown += x_freqPanelGrid_TouchDown;
                x_freqPanelGrid.TouchUp += x_freqPanelGrid_TouchUp;
                x_freqPanelGrid.TouchMove += x_freqPanelGrid_TouchMove;

                x_dbuvTopPanelGrid.TouchDown += x_dbuvTopPanelGrid_TouchDown;
                x_dbuvTopPanelGrid.TouchMove += x_dbuvTopPanelGrid_TouchMove;
                x_dbuvTopPanelGrid.TouchUp += x_dbuvTopPanelGrid_TouchUp;

                x_dbuvTopPanelCanvas.TouchDown += x_dbuvTopPanelGrid_TouchDown;
                x_dbuvTopPanelCanvas.TouchMove += x_dbuvTopPanelGrid_TouchMove;
                x_dbuvTopPanelCanvas.TouchUp += x_dbuvTopPanelGrid_TouchUp;

                x_dbuvBottomPanelGrid.TouchDown += x_dbuvBottomPanelGrid_TouchDown;
                x_dbuvBottomPanelGrid.TouchMove += x_dbuvBottomPanelGrid_TouchMove;
                x_dbuvBottomPanelGrid.TouchUp+= x_dbuvBottomPanelGrid_TouchUp;

                x_dbuvBottomPanelCanvas.TouchDown += x_dbuvBottomPanelGrid_TouchDown;
                x_dbuvBottomPanelCanvas.TouchMove += x_dbuvBottomPanelGrid_TouchMove;
                x_dbuvBottomPanelCanvas.TouchUp += x_dbuvBottomPanelGrid_TouchUp;
            }

            x_spectrumDiagram.SizeChanged += OnSpectrumDiagramSizeChanged;
            x_scaleLineFreq.ScaleConvertValue += OnScaleLineFreq_ScaleConvertValue;

            InitMarkGroupColors();
            //添加门限Mark
            _thresholdMark = AddThresholMark(Colors.White);
            var mvBinding = new Binding("ThresholdMarkVisibility");
            _thresholdMark.SetBinding(VisibilityProperty, mvBinding);
            _spectrumDataManage.UpdateSpectrumPointData += OnSpectrumDataManageUpdateSpectrumPointData;

            //鼠标控制
            xCursorPanel.CursorPoint += OnPanelCursorPoint;
            xCursorPanel.CursorVisibilityChanged += OnCursorVisibilityChanged;

            FreqScaleLabelCount = 7;
            UpperLowerScaleLabelCount = 5;
        }

        private void OnMeasureUnitPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //xMeasureUnitPanel.Margin = new Thickness(-(e.NewSize.Width / 2 - 5), 0, 0, 0);
        }

        /// <summary>
        /// 缩放时标记数值改变
        /// </summary>
        /// <param name="pValue"></param>
        private void OnScaleLineFreq_ScaleConvertValue(ScaleValueConverterArgs pValue)
        {
            double v;
            if (pValue.Value != null && double.TryParse(pValue.Value.ToString(), out v))
            {
                v = Utile.MathNoRound((v / 1000000), pValue.DefaultDec);
                pValue.NewValue = pValue.ScaleIndex.Equals(0) ? string.Format("{0:N" + pValue.DefaultDec + "}MHz", v) : string.Format("{0:N" + pValue.DefaultDec + "}", v);
            }
        }

        /// <summary>
        /// 更新dbuvMark
        /// </summary>
        /// <param name="pHeight"></param>
        /// <param name="pNewMarkValue"></param>
        private void UpdateDbuvMarks(double pHeight = 0, double pNewMarkValue = -1)
        {
            if (pHeight.Equals(0))
                pHeight = x_dbuvMarkPanel.ActualHeight;

            foreach (var uiElement in x_dbuvMarkPanel.Children)
            {
                var mark = uiElement as SpectrumMark;
                if (mark == null)
                    continue;

                double pixelVal = WMonitorUtile.ViewToScreen(mark.GroupName == DefaultThresholMarkId && !pNewMarkValue.Equals(-1) ? pNewMarkValue : mark.MarkValue, 
                    pHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);

                mark.TranslateY = -pixelVal;
            }
        }

        /// <summary>
        /// 刷新频点Mark
        /// </summary>
        private void UpdateFreqMarks()
        {
            foreach (var m in x_freqMarkPanel.Children)
            {
                var mark = m as SpectrumMark;
                if (mark == null)
                    continue;

                UpdateFreqMarkOffset(mark);

                if (MarkMoveChanged != null)
                {
                    MarkMoveChanged(mark);
                }
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

            if (NoZoom)
            {
                long index = _spectrumDataManage.GetIndex(pMark.MarkValue);
                pMark.TranslateX = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0);

                if (pMark.Id != _cursorPanelId && pMark.Visibility == Visibility.Collapsed && _spectrumDataManage.FreqPointCount > 0)
                    pMark.Visibility = Visibility.Visible;
            }
            else
            {
                if (pMark.MarkValue < _zoomLeftFreq || pMark.MarkValue >= _zoomRightFreq)
                {
                    if (pMark.Id != _cursorPanelId)
                        pMark.Visibility = Visibility.Collapsed; /*Mark指向的频点超出缩放范围，隐藏*/
                }
                else
                {
                    long lIndex = _spectrumDataManage.GetIndex(_zoomLeftFreq);
                    long rIndex = _spectrumDataManage.GetIndex(_zoomRightFreq);
                    long index = _spectrumDataManage.GetIndex(pMark.MarkValue);
                    index = index - lIndex;
                    long count = rIndex - lIndex;
                    pMark.TranslateX = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, count - 1, 0);

                    if (pMark.Id != _cursorPanelId && pMark.Visibility == Visibility.Collapsed && _spectrumDataManage.FreqPointCount > 0)
                        pMark.Visibility = Visibility.Visible;
                }
            }

            if (MarkMoveChanged != null)
            {
                MarkMoveChanged(pMark);
            }
        }

        private void OnPanelCursorPoint(CursorPointEventArgs cpArgs)
        {
            if (xCursorPanel.Tag == null)
                return;

            var mark = xCursorPanel.Tag as SpectrumMark;

            if (mark != null)
            {
                var sp = GetNeighborPointFromX(cpArgs.OldPoint.X);
                if (sp == null)
                    return;

                mark.TranslateX = cpArgs.OldPoint.X;
                mark.MarkValue = sp.Data.Freq;
                mark.BuoyOffset = sp.Point.Y;
                cpArgs.NewPoint = new Point(sp.Point.X, sp.Point.Y);
            }
        }

        private void OnCursorVisibilityChanged(bool pVisibility, Point pt)
        {
            SpectrumMark mark = null;

            foreach (var t in x_freqMarkPanel.Children)
            {
                if (t is SpectrumMark && ((SpectrumMark)t).Id == _cursorPanelId)
                {
                    mark = t as SpectrumMark;
                    break;
                }
            }

            //var markObj = x_freqMarkPanel.Children.SingleOrDefault(t => (t is SpectrumMark && ((SpectrumMark)t).Id == _cursorPanelId));
            //if (markObj != null)
            //{
            //    mark = markObj as SpectrumMark;
            //}
            //else
            if (mark == null)
            {
                mark = new SpectrumMark
                               {
                                   Id = _cursorPanelId,
                                   Direction = MarkDirection.Bottom,
                                   HorizontalAlignment = HorizontalAlignment.Left,
                                   Color = new SolidColorBrush(xCursorPanel.LineColor),
                                   Foreground = new SolidColorBrush(Colors.Yellow),
                                   MarkValueConverter = new MarkFreqValueConverter(),
                                   Visibility = Visibility.Visible,
                                   HorizontalLineVisibility = Visibility.Collapsed,
                                   VerticalLineVisibility = Visibility.Collapsed,
                                   BuoyVisibility = Visibility.Visible,
                                   TooltipVisibility = Visibility.Collapsed
                               };

                xCursorPanel.Tag = mark;
                x_freqMarkPanel.Children.Add(mark);
            }

            var sp = GetNeighborPointFromX(pt.X);
            if (sp == null)
            {
                mark.Visibility = Visibility.Collapsed;
                return;
            }
            mark.TranslateX = sp.Point.X;
            mark.MarkValue = sp.Data.Freq;
            mark.TranslateY = sp.Data.Dbuv;
            mark.Visibility = pVisibility ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnDbuvMarkPanelMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dbuvMarkInsertPoint = e.GetPosition(sender as FrameworkElement);
        }

        private void OnFreqMarkPanelMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _freqMarkInsertPoint = e.GetPosition(sender as FrameworkElement);
        }

        private void OnFreqPanelGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }

        private void OnFreqPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = true;
            x_freqPanelGrid.CaptureMouse();
            _mouseClickPoint = e.GetPosition(x_freqPanelGrid);
            _rect.Width = 0;
            _rect.Height = 0;
            _rect.Visibility = Visibility.Visible;
            Canvas.SetLeft(_rect, _mouseClickPoint.X);
            Canvas.SetTop(_rect, _mouseClickPoint.Y);
        }

        private void OnFreqPanelMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = false;
            x_freqPanelGrid.ReleaseMouseCapture();
            _rect.Visibility = Visibility.Collapsed;

            double x = Canvas.GetLeft(_rect);
            double y = Canvas.GetTop(_rect);

            var pt1 = new Point(x, y);
            var pt2 = new Point(x + _rect.Width, y + _rect.Height);
            if (Math.Abs(pt1.X - pt2.X) < 20 || Math.Abs(pt1.Y - pt2.Y) < 20)
                return;

            Zoom(pt1, pt2, _mouseClickPoint.X > pt1.X);
        }

        /// <summary>
        /// x_freqPanelGrid上鼠标拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFreqPanelMouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown)
            {
                //XGZ:显示鼠标当前位置的幅度值和频率值
                return;
            }
            Point pt = e.GetPosition(x_freqPanelGrid);
            double x = _mouseClickPoint.X;
            double y = _mouseClickPoint.Y;

            if (pt.X > x && pt.X < x_freqPanelGrid.ActualWidth)
            {
                _rect.Width = Math.Abs(pt.X - x);
            }
            else if (pt.X < x && pt.X > 0)
            {
                Canvas.SetLeft(_rect, pt.X);
                _rect.Width = Math.Abs(pt.X - x);
            }

            if (pt.Y > y && pt.Y < x_freqPanelGrid.ActualHeight)
            {
                _rect.Height = Math.Abs(pt.Y - y);
            }
            else if (pt.Y < y && pt.Y > 0)
            {
                Canvas.SetTop(_rect, pt.Y);
                _rect.Height = Math.Abs(pt.Y - y);
            }
        }

        private void OnSpectrumDiagramSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resize(); 
            ClearFluorogram();
        }

        private void Resize()
        {
            UpdateSpectrumLines();
            UpdateFreqMarks();
            UpdateScanFreqMarks();
        }

        private void UpdateSpectrumLines()
        {
            if (NoZoom)
            {
                Zoom(_spectrumDataManage.GetFreq(0), _spectrumDataManage.GetFreq(_spectrumDataManage.FreqPointCount - 1));
            }
            else
            {
                Zoom(_zoomLeftFreq, _zoomRightFreq);
            }
        }

        private void InitMarkGroupColors()
        {
            _markGroupColor.Enqueue(Colors.Red);
            _markGroupColor.Enqueue(Color.FromArgb(0xff, 0x83, 0x00, 0xCC));
            _markGroupColor.Enqueue(Color.FromArgb(0xff, 0x23, 0x9C, 0x00));
            _markGroupColor.Enqueue(Color.FromArgb(0xff, 0x00, 0x9B, 0x97));
        }

        private SpectrumMark AddThresholMark(Color pMarkColor)
        {
            var mark = new SpectrumMark
            {
                Direction = MarkDirection.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(2, 0, 0, 0),
                Color = new SolidColorBrush(pMarkColor),
                TranslateY = _dbuvMarkInsertPoint.Y - x_dbuvMarkPanel.ActualHeight,
                Visibility = Visibility.Collapsed,
                GroupName = DefaultThresholMarkId,
                HorizontalLineVisibility = Visibility.Visible
            };

            ThresholdValue = x_spectrumDiagram.GetDbuvValue(_dbuvMarkInsertPoint.Y);
            var markValueBinding = new Binding("ThresholdValue") { Mode = BindingMode.TwoWay, StringFormat = "{0:N0}" };
            mark.SetBinding(SpectrumMark.MarkValueProperty, markValueBinding);
            mark.MouseLeftButtonDown += MarkOnMouseLeftButtonDown;
            mark.MouseMove += MarkOnMouseMove;
            mark.MouseLeftButtonUp += MarkOnMouseLeftButtonUp;
            x_dbuvMarkPanel.Children.Add(mark);
            mark.Visibility = Visibility.Visible;
            return mark;
        }

        internal SpectrumMark AddDbuvMark(Color pMarkColor, string pGroupName = "-1")
        {
            var mark = new SpectrumMark
            {
                Direction = MarkDirection.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(2, 0, 0, 0),
                Color = new SolidColorBrush(pMarkColor),
                Foreground = pMarkColor.Equals(Colors.White) ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White),
                TranslateY = _dbuvMarkInsertPoint.Y - x_dbuvMarkPanel.ActualHeight,
                Visibility = Visibility.Collapsed
            };

            if (pGroupName != "-1")
            {
                mark.GroupName = pGroupName;
            }
            mark.MarkValue = x_spectrumDiagram.GetDbuvValue(_dbuvMarkInsertPoint.Y);
            mark.MouseLeftButtonDown += MarkOnMouseLeftButtonDown;
            mark.MouseMove += MarkOnMouseMove;
            mark.MouseLeftButtonUp += MarkOnMouseLeftButtonUp;
            x_dbuvMarkPanel.Children.Add(mark);
            mark.Visibility = Visibility.Visible;
            return mark;
        }

        internal void ClearDbuvMark()
        {
            var removeObjs = x_dbuvMarkPanel.Children.OfType<SpectrumMark>().Where(mark => mark.GroupName != DefaultThresholMarkId).Cast<object>().ToList();
            foreach (SpectrumMark mark in removeObjs)
            {
                x_dbuvMarkPanel.Children.Remove(mark);
            }
        }

        private SpectrumMark AddFreqMark(Color pMarkColor, int pX = -1, string pGroupName = "-1")
        {
            var mark = new SpectrumMark
            {
                Direction = MarkDirection.Bottom,
                HorizontalAlignment = HorizontalAlignment.Left,
                Color = new SolidColorBrush(pMarkColor),
                Foreground = new SolidColorBrush(Colors.White),
                MarkValueConverter = new MarkFreqValueConverter(),
                Visibility = Visibility.Collapsed
            };

            if (pGroupName != "-1")
            {
                mark.GroupName = pGroupName;
            }

            mark.MouseLeftButtonDown += MarkOnMouseLeftButtonDown;
            mark.MouseMove += MarkOnMouseMove;
            mark.MouseLeftButtonUp += MarkOnMouseLeftButtonUp;
            mark.MarkMoveChanged += OnMarkMoveChanged;
            x_freqMarkPanel.Children.Insert(0, mark);

            SpectrumPointEx sp =  GetNeighborPointFromX(pX != -1 ? pX : _freqMarkInsertPoint.X);
            if (sp != null)
            {
                mark.TranslateX = sp.Point.X;
                mark.TranslateY = sp.Data.Dbuv;
                mark.BuoyOffset = sp.Point.Y;
                mark.MarkValue = sp.Data.Freq;
            }
            else
            {
                mark.TranslateX = pX;
            }
            mark.Visibility = Visibility.Visible;
            return mark;
        }

        internal void ClearFreqMark()
        {
            var removeObjs = x_freqMarkPanel.Children.OfType<SpectrumMark>().Where(mark => mark.GroupName != DefaultMajorMarkId).Cast<object>().ToList();
            foreach (var removeObj in removeObjs)
            {
                var mark = removeObj as SpectrumMark;
                if (mark == null)
                    continue;
                x_freqMarkPanel.Children.Remove(mark);
            }

            SpectrumMark foundMajorMark = null;// = x_freqMarkPanel.Children.SingleOrDefault(mark => (mark is SpectrumMark) && ((SpectrumMark)mark).GroupName == DefaultMajorMarkId);

            foreach (var mark in x_freqMarkPanel.Children)
            {
                if (mark is SpectrumMark && ((SpectrumMark)mark).GroupName == DefaultMajorMarkId)
                {
                    foundMajorMark = mark as SpectrumMark;
                    break;
                }
            }

            if (foundMajorMark != null)
            {
                foundMajorMark.Visibility = Visibility.Collapsed;
            }

            _markGroupColor.Clear();
            InitMarkGroupColors();
        }

        internal void AddGroupFreqMark()
        {
            if (_markGroupColor.Count == 0)
                return;
            string groupName = Guid.NewGuid().ToString();
            Color c = _markGroupColor.Dequeue();
            SpectrumMark mark = AddFreqMark(c);
            mark.GroupName = groupName;
            mark = AddFreqMark(c);
            mark.GroupName = groupName;
            _initGroupIffqMark = true;
        }

        private void UpdateIffqInfo()
        {
            if (_majorMarkObject == null)
                return;
            //var groupMarks = from i in x_freqMarkPanel.Children
            //                 where ((SpectrumMark)i).GroupName != "1"
            //                 group i by ((SpectrumMark)i).GroupName
            //                     into groups
            //                     select groups;
            
            Dictionary<string, List<SpectrumMark>> dicMarks = new Dictionary<string, List<SpectrumMark>>();
            foreach (var item in x_freqMarkPanel.Children)
            {
                SpectrumMark mark = item as SpectrumMark;
                if (mark == null)
                {
                    continue;
                }
                if (mark.GroupName != null && mark.GroupName != "1")
                {
                    if (dicMarks.ContainsKey(mark.GroupName))
                    {
                        dicMarks[mark.GroupName].Add(mark);
                    }
                    else
                    {
                        List<SpectrumMark> marks = new List<SpectrumMark>();
                        dicMarks.Add(mark.GroupName, marks);
                        dicMarks[mark.GroupName].Add(mark);
                    }
                }
            }

            bool inGroup = false;

            foreach (var groupMark in dicMarks.Values)
            {
                if (inGroup)
                    break;
                byte index = 0;
                var gm = new SpectrumMark[2];
                foreach (var gMark in groupMark)
                {
                    if (gMark != null)
                        gm[index++] = gMark;
                }

                if (gm.Length != 2 || gm[0] == null || gm[1] == null)
                    continue;

                if (gm[0].TranslateX < gm[1].TranslateX)
                {
                    if (_majorMarkObject != null && _majorMarkObject.TranslateX > gm[0].TranslateX && _majorMarkObject.TranslateX < gm[1].TranslateX)
                    {
                        inGroup = true;
                        UpdateIffqInfo(gm[0], gm[1]);
                    }
                }
                else
                {
                    if (_majorMarkObject != null && _majorMarkObject.TranslateX > gm[1].TranslateX && _majorMarkObject.TranslateX < gm[0].TranslateX)
                    {
                        inGroup = true;
                        UpdateIffqInfo(gm[1], gm[0]);
                    }
                }
            }
            xFreqInfo.Visibility = inGroup ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateIffqInfo(SpectrumMark pMark1, SpectrumMark pMark2)
        {
            if (pMark1 == null || pMark2 == null || FreqGroupInfo == null)
                return;
            FreqGroupInfo.GroupName = pMark1.GroupName;
            FreqGroupInfo.LeftDbuv = pMark1.TranslateY;
            FreqGroupInfo.RightDbuv = pMark2.TranslateY;
            FreqGroupInfo.LeftFreq = Utile.MathNoRound(pMark1.MarkValue / 1000000, 6);
            FreqGroupInfo.RightFreq = Utile.MathNoRound(pMark2.MarkValue / 1000000, 6);
            FreqGroupInfo.DbuvColor = pMark1.Color;
            FreqGroupInfo.FreqLengthUnit = "MHz";
            FreqGroupInfo.FreqLength = Utile.MathNoRound((pMark2.MarkValue - pMark1.MarkValue) / 1000000, 6);
        }

        private void MarkOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseCaptured = true;
        }

        /// <summary>
        /// 拖动mark
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MarkOnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMouseCaptured)
                return;

            var mark = sender as SpectrumMark;
            if (mark == null) return;

            Point pt = e.GetPosition(x_spectrumDiagram);

            if (mark.Direction == MarkDirection.Bottom)
            {
                SpectrumPointEx sp = GetNeighborPointFromX(pt.X);
                if (sp != null)
                {
                    mark.MarkValue = sp.Data.Freq;
                    mark.TranslateY = sp.Data.Dbuv;
                    mark.BuoyOffset = sp.Point.Y;

                    if (MarkMoveChanged != null)
                    {
                        MarkMoveChanged(mark);
                    }
                }
            }
            else
            {
                mark.MarkValue = x_spectrumDiagram.GetDbuvValue(pt.Y);
            }
        }

        private void MarkOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!_isMouseCaptured)
                return;
            var mark = sender as SpectrumMark;
            if (mark == null)
                return;
            Point pt = mouseButtonEventArgs.GetPosition(x_spectrumDiagram);
            if (mark.Direction == MarkDirection.Bottom)
            {
                if (pt.X < 0)
                    pt.X = 0;
                if (pt.X > x_spectrumDiagram.ActualWidth)
                    pt.X = x_spectrumDiagram.ActualWidth;

                SpectrumPointEx sp = GetNeighborPointFromX(pt.X);
                if (sp != null)
                {
                    mark.MarkValue = sp.Data.Freq;
                    mark.TranslateX = sp.Point.X;
                    mark.TranslateY = sp.Data.Dbuv;
                    mark.BuoyOffset = sp.Point.Y;

                    if (MarkMoveFrequencyChanged != null)
                    {
                        MarkMoveFrequencyChanged(sp.Data.Freq);
                    }
                }
            }
            _isMouseCaptured = false;
        }

        private void OnMarkMoveChanged(object pSender, bool pDirection)
        {
            var mark = pSender as SpectrumMark;
            if (mark == null)
                return;
            SpectrumPointEx sp = pDirection
                                   ? GetPrevNeighborPointFromFreq(mark.MarkValue)
                                   : GetNextNeighborPointFromFreq(mark.MarkValue);
            if (sp != null)
            {
                mark.MarkValue = sp.Data.Freq;
                mark.TranslateX = sp.Point.X;
                mark.TranslateY = sp.Data.Dbuv;
                mark.BuoyOffset = sp.Point.Y;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pX"></param>
        /// <returns></returns>
        private SpectrumPointEx GetNeighborPointFromX(double pX)
        {
            var sp = new SpectrumPointEx();
            long index;
            if (NoZoom)
            {
                index = WMonitorUtile.ScreenToView(pX, _spectrumDataManage.FreqPointCount - 1, 0, x_spectrumDiagram.ActualWidth - 1, 0);
                SpectrumPointData? dt = _spectrumDataManage.GetSpectrumPointData(index);
                if (dt == null)
                    return null;
                sp.Data = dt.Value;
                pX = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0);
            }
            else
            {
                //有缩放级别后
                long lfIndex = _spectrumDataManage.GetRoughlyIndex(_zoomLeftFreq);
                long rfIndex = _spectrumDataManage.GetRoughlyIndex(_zoomRightFreq);
                long fpCount = rfIndex - lfIndex;
                index = WMonitorUtile.ScreenToView(pX, fpCount - 1, 0, x_spectrumDiagram.ActualWidth - 1, 0);
                SpectrumPointData? dt = _spectrumDataManage.GetSpectrumPointData(lfIndex + index);
                if (dt == null)
                    return null;
                sp.Data = dt.Value;
                pX = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, fpCount - 1, 0);
            }

            double pixelVal = WMonitorUtile.ViewToScreen(sp.Data.Dbuv, x_spectrumDiagram.ActualHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
            pixelVal = x_spectrumDiagram.ActualHeight - pixelVal;
            sp.Point = new Point(pX, pixelVal);
            return sp;
        }

        private SpectrumPointEx GetPrevNeighborPointFromFreq(double pFreq)
        {
            var sp = new SpectrumPointEx();
            long index = _spectrumDataManage.GetRoughlyIndex(pFreq);
            long lfIndex = NoZoom ? 0 : _spectrumDataManage.GetRoughlyIndex(_zoomLeftFreq);
            SpectrumPointData? dt;
            if (index > lfIndex)
            {
                index = index - 1;
                dt = _spectrumDataManage.GetSpectrumPointData(index);
            }
            else
            {
                index = lfIndex;
                dt = _spectrumDataManage.GetSpectrumPointData(index);
            }
            if (dt == null)
                return null;
            sp.Data = dt.Value;
            double pixelVal = WMonitorUtile.ViewToScreen(sp.Data.Dbuv, x_spectrumDiagram.ActualHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
            pixelVal = x_spectrumDiagram.ActualHeight - pixelVal;
            long x;
            if (NoZoom)
            {
                x = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0);
            }
            else
            {
                long rfIndex = _spectrumDataManage.GetRoughlyIndex(_zoomRightFreq);
                long fpCount = rfIndex - lfIndex;
                x = WMonitorUtile.ViewToScreen(index - lfIndex, x_spectrumDiagram.ActualWidth - 1, 0, fpCount - 1, 0);
            }
            sp.Point = new Point(x, pixelVal);
            return sp;
        }

        private SpectrumPointEx GetNextNeighborPointFromFreq(double pFreq)
        {
            var sp = new SpectrumPointEx();
            long index = _spectrumDataManage.GetRoughlyIndex(pFreq);
            long rfIndex = NoZoom ? _spectrumDataManage.FreqPointCount : _spectrumDataManage.GetRoughlyIndex(_zoomRightFreq);
            SpectrumPointData? dt;
            index = index + 1;
            if (index < rfIndex)
            {
                dt = _spectrumDataManage.GetSpectrumPointData(index);
            }
            else
            {
                index = rfIndex - 1;
                dt = _spectrumDataManage.GetSpectrumPointData(index);
            }
            if (dt == null)
                return null;
            sp.Data = dt.Value;
            double pixelVal = WMonitorUtile.ViewToScreen(sp.Data.Dbuv, x_spectrumDiagram.ActualHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
            pixelVal = x_spectrumDiagram.ActualHeight - pixelVal;
            long x;
            if (NoZoom)
            {
                x = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0);
            }
            else
            {
                long lfIndex = _spectrumDataManage.GetRoughlyIndex(_zoomLeftFreq);
                long fpCount = rfIndex - lfIndex;
                x = WMonitorUtile.ViewToScreen(index - lfIndex, x_spectrumDiagram.ActualWidth - 1, 0, fpCount - 1, 0);
            }
            sp.Point = new Point(x, pixelVal);
            return sp;
        }

        /// <summary>
        /// 获取频率范围内最大峰值数据
        /// </summary>
        /// <param name="pStartFreq">起始频率</param>
        /// <param name="pStopFreq">终止频率</param>
        /// <returns>谱图点数据</returns>
        private SpectrumPointEx GetMaxPointData(double pStartFreq, double pStopFreq)
        {
            var sp = new SpectrumPointEx();
            long lfIndex = _spectrumDataManage.GetRoughlyIndex(pStartFreq);
            long rfIndex = _spectrumDataManage.GetRoughlyIndex(pStopFreq);
            var dt = _spectrumDataManage.GetMaxSpectrumPointData(pStartFreq, pStopFreq);
            if (dt == null)
                return null;
            long index = _spectrumDataManage.GetIndex(dt.Value.Freq);
            sp.Data = dt.Value;
            double pixelVal = WMonitorUtile.ViewToScreen(sp.Data.Dbuv, x_spectrumDiagram.ActualHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
            pixelVal = x_spectrumDiagram.ActualHeight - pixelVal;
            long x;
            if (NoZoom)
            {
                x = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0);
            }
            else
            {
                long fpCount = rfIndex - lfIndex;
                x = WMonitorUtile.ViewToScreen(index - lfIndex, x_spectrumDiagram.ActualWidth - 1, 0, fpCount - 1, 0);
            }
            sp.Point = new Point(x, pixelVal);
            return sp;
        }

        /// <summary>
        /// 获取频率范围内最小峰值数据
        /// </summary>
        /// <param name="pStartFreq">起始频率</param>
        /// <param name="pStopFreq">终止频率</param>
        /// <returns>谱图点数据</returns>
        private SpectrumPointEx GetMinPointData(double pStartFreq, double pStopFreq)
        {
            var sp = new SpectrumPointEx();
            long lfIndex = _spectrumDataManage.GetRoughlyIndex(pStartFreq);
            long rfIndex = _spectrumDataManage.GetRoughlyIndex(pStopFreq);
            var dt = _spectrumDataManage.GetMinSpectrumPointData(pStartFreq, pStopFreq);
            if (dt == null)
                return null;
            long index = _spectrumDataManage.GetIndex(dt.Value.Freq);
            sp.Data = dt.Value;
            double pixelVal = WMonitorUtile.ViewToScreen(sp.Data.Dbuv, x_spectrumDiagram.ActualHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
            pixelVal = x_spectrumDiagram.ActualHeight - pixelVal;
            long x;
            if (NoZoom)
            {
                x = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0);
            }
            else
            {
                long fpCount = rfIndex - lfIndex;
                x = WMonitorUtile.ViewToScreen(index - lfIndex, x_spectrumDiagram.ActualWidth - 1, 0, fpCount - 1, 0);
            }
            sp.Point = new Point(x, pixelVal);
            return sp;
        }

        /// <summary>
        /// 在频率范围内的频率点属性
        /// </summary>
        /// <param name="pFreq">当前频率</param>
        /// <returns></returns>
        private SpectrumPointEx GetWithinPointData(double pFreq)
        {
            var sp = new SpectrumPointEx();
            long index = _spectrumDataManage.GetIndex(pFreq);
            var dt = _spectrumDataManage.GetSpectrumPointData(index);
            if (dt == null)
                return null;
            if(!NoZoom)
                Zoom(new Point(), new Point(), true);
            sp.Data = dt.Value;
            double pixelVal = WMonitorUtile.ViewToScreen(sp.Data.Dbuv, x_spectrumDiagram.ActualHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
            pixelVal = x_spectrumDiagram.ActualHeight - pixelVal;
            long x = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0);
            sp.Point = new Point(x, pixelVal);
            return sp;
        }

        /// <summary>
        /// 缩放谱图
        /// </summary>
        /// <param name="pPt1">矩形起始坐标，单位像素</param>
        /// <param name="pPt2">矩形终止坐标，单位像素</param>
        /// <param name="pOut">true - 缩小，false - 放大</param>
        private void Zoom(Point pPt1, Point pPt2, bool pOut)
        {
            if (pOut)
            {
                ResetZoom();
                Zoom(_spectrumDataManage.GetFreq(0), _spectrumDataManage.GetFreq(_spectrumDataManage.FreqPointCount - 1));
            }
            else
            {
                double zoomLeftFreq = _zoomLeftFreq;
                double zoomRightFreq = _zoomRightFreq;
                if (NoZoom)
                {
                    long lIndex = WMonitorUtile.ScreenToView(pPt1.X, _spectrumDataManage.FreqPointCount, 0, x_spectrumDiagram.ActualWidth, 0);
                    long rIndex = WMonitorUtile.ScreenToView(pPt2.X, _spectrumDataManage.FreqPointCount, 0, x_spectrumDiagram.ActualWidth, 0);
                    _zoomLeftFreq = _spectrumDataManage.GetFreq(lIndex);
                    _zoomRightFreq = _spectrumDataManage.GetFreq(rIndex);
                }
                else
                {
                    long lfIndex = _spectrumDataManage.GetRoughlyIndex(_zoomLeftFreq);
                    long rfIndex = _spectrumDataManage.GetRoughlyIndex(_zoomRightFreq);
                    long fpCount = rfIndex - lfIndex;
                    long lIndex = WMonitorUtile.ScreenToView(pPt1.X, fpCount - 1, 0, x_spectrumDiagram.ActualWidth - 1, 0);
                    long rIndex = WMonitorUtile.ScreenToView(pPt2.X, fpCount - 1, 0, x_spectrumDiagram.ActualWidth - 1, 0);
                    rIndex = fpCount - rIndex;
                    _zoomLeftFreq = _spectrumDataManage.GetFreq(lfIndex + lIndex);
                    _zoomRightFreq = _spectrumDataManage.GetFreq(rfIndex - rIndex);
                }
                if (!Zoom(_zoomLeftFreq, _zoomRightFreq))
                {
                    _zoomLeftFreq = zoomLeftFreq;
                    _zoomRightFreq = zoomRightFreq;
                    return;
                }

                x_spectrumDiagram.Property.BeginLeftValue = _zoomLeftFreq;
                x_spectrumDiagram.Property.EndRightValue = _zoomRightFreq;
                var zlButton = new ImageButton
                                   {
                                       Content = x_zoomLevelButtons.Children.Count + 1,
                                       Margin = new Thickness(5, 0, 0, 0),
                                       Padding = new Thickness(5, 0, 5, 0),
                                       Tag = new[] { _zoomLeftFreq, _zoomRightFreq },
                                       Foreground = new SolidColorBrush(Colors.White),
                                       Width = 40,
                                       Height = 30
                                   };
                x_zoomLevelButtons.Children.Add(zlButton);
                zlButton.Click += (sender, args) =>
                                      {
                                          var z = (double[])((Button)sender).Tag;
                                          _zoomLeftFreq = z[0];
                                          _zoomRightFreq = z[1];
                                          x_spectrumDiagram.Property.BeginLeftValue = _zoomLeftFreq;
                                          x_spectrumDiagram.Property.EndRightValue = _zoomRightFreq;
                                          Zoom(_zoomLeftFreq, _zoomRightFreq);
                                          UpdateFreqMarks();
                                          UpdateScanFreqMarks();
                                      };
                OnFreqRangeChanged(_zoomLeftFreq, _zoomRightFreq);
            }
            UpdateFreqMarks();
            UpdateScanFreqMarks();
        }

        /// <summary>
        /// 缩放绘制的谱图
        /// </summary>
        /// <param name="pLeftFreq"></param>
        /// <param name="pRightFreq"></param>
        private bool Zoom(double pLeftFreq, double pRightFreq)
        {
            short[] dbuvs = _spectrumDataManage.GetDbuvData(pLeftFreq, pRightFreq);
            if (dbuvs == null)
                return false;
            if (dbuvs.Length < 5)
                return false;
            if (dbuvs.Length < x_scaleLineFreq.ScaleLabelCount)
                x_scaleLineFreq.ScaleLabelCount = dbuvs.Length;

            if (SpectrumType == SpectrumType.射频全景)
            {
                //电磁背景
                if (_drawBackground)
                {
                    short[] dbuvsBk = _spectrumBackgroundDataManage.GetDbuvData(pLeftFreq, pRightFreq);
                    if (_backSpectrumLineType == SpectrumLineType.Column)
                        x_spectrumDiagram.DrawColumnLineToBackground(dbuvsBk, _backgroundLineColor);
                    else if (_backSpectrumLineType == SpectrumLineType.Wave)
                        x_spectrumDiagram.DrawWaveLineToBackground(dbuvsBk, _backgroundLineColor);
                }
                //背景噪声
                if (_drawNoise)
                {
                    short[] dbuvsBk = _spectrumNoiseDatamanage.GetDbuvData(pLeftFreq, pRightFreq);
                    x_spectrumDiagram.DrawWaveLineToNoise(dbuvsBk, _noiseLineColor);
                }
                //实时谱线
                if (_realTimeLineType == SpectrumLineType.Column)
                {
                    x_spectrumDiagram.DrawColumnLine(dbuvs, _realTimeLineColor);
                }
                else if (_realTimeLineType == SpectrumLineType.Wave)
                {
                    x_spectrumDiagram.DrawWaveLine(dbuvs, _realTimeLineColor);
                }
                if (_drawMax)
                {
                    var dbuvsMax = _spectrumMaxDataManage.GetDbuvData(pLeftFreq, pRightFreq);
                    if (_realTimeLineType == SpectrumLineType.Column)
                        x_spectrumDiagram.DrawColumnLineToMax(dbuvsMax, _maxLineColor);
                    else if (_realTimeLineType == SpectrumLineType.Wave)
                        x_spectrumDiagram.DrawWaveLineToMax(dbuvsMax, _maxLineColor);
                }
                if (_drawMin)
                {
                    var dbuvsMin = _spectrumMinDataManage.GetDbuvData(pLeftFreq, pRightFreq);
                    if (_realTimeLineType == SpectrumLineType.Column)
                        x_spectrumDiagram.DrawColumnLineToMin(dbuvsMin, _minLineColor);
                    else if (_realTimeLineType == SpectrumLineType.Wave)
                        x_spectrumDiagram.DrawWaveLineToMin(dbuvsMin, _minLineColor);
                }
            }
            else if (SpectrumType == SpectrumType.中频全景)
            {
                x_spectrumDiagram.DrawWaveLine(dbuvs, _realTimeLineColor);
                if (_drawMax)
                {
                    var dbuvsMax = _spectrumMaxDataManage.GetDbuvData(pLeftFreq, pRightFreq);
                    x_spectrumDiagram.DrawWaveLineToMax(dbuvsMax, _maxLineColor);
                }
                if (_drawMin)
                {
                    var dbuvsMin = _spectrumMinDataManage.GetDbuvData(pLeftFreq, pRightFreq);
                    x_spectrumDiagram.DrawWaveLineToMin(dbuvsMin, _minLineColor);
                }
                if (_drawFluoro)
                {
                    x_spectrumDiagram.ClearCacheForZoom(_zoomLeftFreq, _zoomRightFreq);
                    x_spectrumDiagram.DrawFluoroPoint(dbuvs, _interFreTime, _interFreDuration);
                }

                if(x_IfbwArea.Visibility == Visibility.Visible)
                   UpdateIfbwArea(_ifqexifbw, _ifbwfreq);
            }
            return true;
        }

        private void ResetZoom()
        {
            _zoomLeftFreq = _zoomRightFreq = 0;
            x_spectrumDiagram.Property.BeginLeftValue = _beginLeftValue;
            x_spectrumDiagram.Property.EndRightValue = _endRightValue;
            x_zoomLevelButtons.Children.Clear();
            OnFreqRangeChanged(_beginLeftValue, _endRightValue);
        }

        /// <summary>
        /// 刷新谱图点数据事件
        /// </summary>
        /// <param name="pFreqIndex"></param>
        /// <param name="pPointData"></param>
        private void OnSpectrumDataManageUpdateSpectrumPointData(long pFreqIndex, SpectrumPointData pPointData)
        {
            foreach (var m in x_freqMarkPanel.Children)
            {
                var mark = m as SpectrumMark;
                if (mark == null)
                    continue;
                if (!mark.MarkValue.Equals(pPointData.Freq))
                    continue;

                mark.TranslateY = pPointData.Dbuv;
                double pixelVal = WMonitorUtile.ViewToScreen(pPointData.Dbuv, x_spectrumDiagram.ActualHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
                pixelVal = x_spectrumDiagram.ActualHeight - pixelVal;
                mark.BuoyOffset = pixelVal;
                UpdateFreqMarkOffset(mark);
            }
        }

        /// <summary>
        /// 主Mark寻找当前频点左侧范围的峰值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLeftPeakButtonClick(object sender, RoutedEventArgs e)
        {
            XBtnAlwaysMaxPeak.IsChecked = false;
            var freqIndex = _spectrumDataManage.GetIndex(_majorMarkObject.MarkValue);
            freqIndex -= 1;
            if (freqIndex < 0)
                return;
            var preFreq = _spectrumDataManage.GetFreq(freqIndex);
            UpdateMajorFreqPosition(preFreq);

            if (MarkMoveFrequencyChanged != null)
            {
                MarkMoveFrequencyChanged(preFreq);
            }
        }

        /// <summary>
        /// 主Mark寻找当前频点左侧范围的最大峰值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLeftMaxPeakButtonClick(object sender, RoutedEventArgs e)
        {
            XBtnAlwaysMaxPeak.IsChecked = false;

            if (_majorMarkObject == null)
                return;
            double sfreq = NoZoom ? Data.BeginLeftValue : _zoomLeftFreq;
            var sp = GetMaxPointData(sfreq, _majorMarkObject.MarkValue);
            if (sp == null)
                return;
            _majorMarkObject.TranslateX = sp.Point.X;
            _majorMarkObject.TranslateY = sp.Data.Dbuv;
            _majorMarkObject.BuoyOffset = sp.Point.Y;
            _majorMarkObject.MarkValue = sp.Data.Freq;

            if (MarkMoveFrequencyChanged != null)
            {
                MarkMoveFrequencyChanged(sp.Data.Freq);
            }
        }

        /// <summary>
        /// 寻找当前频率范围的峰值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPeakButtonClick(object sender, RoutedEventArgs e)
        {
            XBtnAlwaysMaxPeak.IsChecked = false;

            var sp = NoZoom ? GetMaxPointData(x_spectrumDiagram.Property.BeginLeftValue, x_spectrumDiagram.Property.EndRightValue) : GetMaxPointData(_zoomLeftFreq, _zoomRightFreq);
            if (sp == null || _majorMarkObject == null)
                return;
            _majorMarkObject.TranslateX = sp.Point.X;
            _majorMarkObject.TranslateY = sp.Data.Dbuv;
            _majorMarkObject.BuoyOffset = sp.Point.Y;
            _majorMarkObject.MarkValue = sp.Data.Freq;

            if (MarkMoveFrequencyChanged != null)
            {
                MarkMoveFrequencyChanged(sp.Data.Freq);
            }
        }

        /// <summary>
        /// 主Mark寻找当前频右侧范围的最大峰值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRightMaxPeakButtonClick(object sender, RoutedEventArgs e)
        {
            XBtnAlwaysMaxPeak.IsChecked = false;

            if (_majorMarkObject == null)
                return;
            double efreq = NoZoom ? Data.EndRightValue : _zoomRightFreq;
            var sp = GetMaxPointData(_majorMarkObject.MarkValue, efreq);
            if (sp == null)
                return;
            _majorMarkObject.TranslateX = sp.Point.X;
            _majorMarkObject.TranslateY = sp.Data.Dbuv;
            _majorMarkObject.BuoyOffset = sp.Point.Y;
            _majorMarkObject.MarkValue = sp.Data.Freq;

            if (MarkMoveFrequencyChanged != null)
            {
                MarkMoveFrequencyChanged(sp.Data.Freq);
            }
        }

        /// <summary>
        /// 主Mark寻找当前频右侧范围的峰值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRightPeakButtonClick(object sender, RoutedEventArgs e)
        {
            XBtnAlwaysMaxPeak.IsChecked = false;

            var freqIndex = _spectrumDataManage.GetIndex(_majorMarkObject.MarkValue);
            freqIndex += 1;
            if (freqIndex >= _spectrumDataManage.FreqPointCount)
                return;
            var preFreq = _spectrumDataManage.GetFreq(freqIndex);
            UpdateMajorFreqPosition(preFreq);

            if (MarkMoveFrequencyChanged != null)
            {
                MarkMoveFrequencyChanged(preFreq);
            }
        }


        /// <summary>
        /// 不断寻找当前频率范围内最大峰值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLeftRightMaxPeakButtonClick(object sender, RoutedEventArgs e)
        {
            if (XBtnAlwaysMaxPeak.IsChecked == true)
            {
                var sp = NoZoom ? GetMaxPointData(x_spectrumDiagram.Property.BeginLeftValue, x_spectrumDiagram.Property.EndRightValue) : GetMaxPointData(_zoomLeftFreq, _zoomRightFreq);
                if (sp == null || _majorMarkObject == null)
                    return;
                _majorMarkObject.TranslateX = sp.Point.X;
                _majorMarkObject.TranslateY = sp.Data.Dbuv;
                _majorMarkObject.BuoyOffset = sp.Point.Y;
                _majorMarkObject.MarkValue = sp.Data.Freq;

                if (MarkMoveFrequencyChanged != null)
                {
                    MarkMoveFrequencyChanged(sp.Data.Freq);
                }
            }
        }

        /// <summary>
        /// 自适应调整
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDbuvAdaptiveButtonClick(object sender, RoutedEventArgs e)
        {
            var maxsp = NoZoom ? GetMaxPointData(x_spectrumDiagram.Property.BeginLeftValue, x_spectrumDiagram.Property.EndRightValue) : GetMaxPointData(_zoomLeftFreq, _zoomRightFreq);
            var minsp = NoZoom ? GetMinPointData(x_spectrumDiagram.Property.BeginLeftValue, x_spectrumDiagram.Property.EndRightValue) : GetMinPointData(_zoomLeftFreq, _zoomRightFreq);
            if (_majorMarkObject == null)
                return;
            double? ud = null;
            double? ld = null;
            if (maxsp != null)
            {
                if (maxsp.Data.Dbuv == 123)
                    ud = 60;
                else
                    ud = maxsp.Data.Dbuv + 20;              
            }
                
            if (minsp != null)
            {
                if(minsp.Data.Dbuv == -999)
                    ld = - 20;
                else
                    ld = minsp.Data.Dbuv - 10;
            }

            InitSpectrumProperty(ud, ld, null, null);
            OnDbuvRangeChanged(x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
            Update();
            UpdateSpectrumLines();
            ClearFluorogram();
        }

        #endregion Private

        #region Property

        /// <summary>
        /// 获取或设置谱图类型
        /// </summary>
        public SpectrumType SpectrumType
        {
            get { return _spectrumType; }
            set
            {
                _spectrumType = value;
                switch (_spectrumType)
                {
                    case SpectrumType.射频全景:
                        x_wbfqMarkMenu.Visibility = Visibility.Visible;
                        x_iffqMarkMenu.Visibility = Visibility.Collapsed;
                        break;

                    case SpectrumType.中频全景:
                        x_wbfqMarkMenu.Visibility = Visibility.Collapsed;
                        x_iffqMarkMenu.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        /// <summary>
        /// 获取谱图当前是否未处于缩放状态
        /// </summary>
        private bool NoZoom
        {
            get { return _zoomLeftFreq.Equals(0) && _zoomRightFreq.Equals(0); }
        }

        /// <summary>
        /// 获取或设置门限控制是否可见
        /// </summary>
        public Visibility ThresholdMarkVisibility
        {
            get { return (Visibility)GetValue(ThresholdMarkVisibilityProperty); }
            set { SetValue(ThresholdMarkVisibilityProperty, value); }
        }

        /// <summary>
        /// 获取或设置门限值
        /// </summary>
        public double ThresholdValue
        {
            get { return (double)GetValue(ThresholdValueProperty); }
            set { SetValue(ThresholdValueProperty, value); }
        }

        /// <summary>
        /// 获取或设置谱图上下幅度值单位名称
        /// </summary>
        public string MeasureUnit
        {
            get { return _measureUnit; }
            set
            {
                _measureUnit = value;
                OnPropertyChanged("MeasureUnit");
            }
        }

        public ICommand MarkMenuCommand { get; set; }

        /// <summary>
        /// 获取谱图属性
        /// </summary>
        public SpectrumDiagramProperty Data
        {
            get { return x_spectrumDiagram.Property; }
        }

        /// <summary>
        /// 获取频率组信息
        /// </summary>
        public FreqGroupInfo FreqGroupInfo { get; private set; }

        /// <summary>
        /// 获取或设置频率标尺节点数量，默认：7
        /// </summary>
        public int FreqScaleLabelCount
        {
            get { return x_scaleLineFreq.ScaleLabelCount; }
            set { x_scaleLineFreq.ScaleLabelCount = value; }
        }

        /// <summary>
        /// 获取或设置上下幅度值标尺节点数量，默认：5
        /// </summary>
        public int UpperLowerScaleLabelCount
        {
            get { return x_scaleLinePrompt1.ScaleLabelCount; }
            set
            {
                x_scaleLinePrompt1.ScaleLabelCount = value;
                x_scaleLinePrompt2.ScaleLabelCount = value;
            }
        }

        /// <summary>
        /// 获取或设置默认主Mark颜色
        /// </summary>
        public Color DefaultMajorMarkColor { get; set; }

        /// <summary>
        /// 获取门限 Mark对象
        /// </summary>
        public SpectrumMark ThresholdMark
        {
            get { return _thresholdMark; }
        }

        /// <summary>
        /// 获取或设置是否显示顶部控件面板
        /// </summary>
        public bool IsShowTopControler
        {
            get { return xTopControl.Visibility == Visibility.Visible; }
            set { xTopControl.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        /// <summary>
        /// 获取或设置是否显示右侧幅度值控制面板
        /// </summary>
        public bool IsShowAmplitudeControler
        {
            get { return xAmplitudeControler.Visibility == Visibility.Visible; }
            set { xAmplitudeControler.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Brush MeasureUnitForeground
        {
            get { return _measureUnitForeground; }
            set
            {
                _measureUnitForeground = value;
                OnPropertyChanged("MeasureUnitForeground");
            }
        }

        public Brush DefaultLabelForeground
        {
            get { return _defaultLabelForeground; }
            set
            {
                _defaultLabelForeground = value;
                x_scaleLinePrompt1.Foreground = _defaultLabelForeground;
                x_scaleLinePrompt2.Foreground = _defaultLabelForeground;
                x_scaleLineFreq.Foreground = _defaultLabelForeground;
            }
        }

        /// <summary>
        /// 获取或设置中频扫描中频带宽显示区域宽度
        /// </summary>
        public double IfbwWidth
        {
            get { return _ifbwwidth; }
            set
            {
                _ifbwwidth = value;
                OnPropertyChanged("IfbwWidth");
            }
        }

        /// <summary>
        /// 获取或设置中频扫描中频带宽显示区域高度
        /// </summary>
        public double IfbwHeight
        {
            get { return _ifbwheight; }
            set
            {
                _ifbwheight = value;
                OnPropertyChanged("IfbwHeight");
            }
        }
        #endregion Property

        #region Public

        private readonly SpectrumBeforeSave _beforeSave = new SpectrumBeforeSave();

        public void BeginSave()
        {
            _beforeSave.MeasureUnitForeground = MeasureUnitForeground;
            _beforeSave.DefaultLabelForeground = DefaultLabelForeground;
            var blackBrush = new SolidColorBrush(Colors.Black);
            MeasureUnitForeground = blackBrush;
            DefaultLabelForeground = blackBrush;

            for (int i = 0; i < x_ScanfreqMarkPanel.Children.Count; i++ )
            {
                SpectrumScanMark sp = x_ScanfreqMarkPanel.Children[i] as SpectrumScanMark;
                if (sp != null)
                {
                    sp.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        public void EndSave()
        {
            MeasureUnitForeground = _beforeSave.MeasureUnitForeground;
            DefaultLabelForeground = _beforeSave.DefaultLabelForeground;

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
        /// 添加频率主Mark，谱图中只有一个主Mark，如果主Mark已经存在，则返回主Mark
        /// </summary>
        /// <returns>主Mark</returns>
        public SpectrumMark AddMajorFreqMark()
        {
            if (_spectrumDataManage.FreqPointCount <= 0)
                return null;

            List<SpectrumMark> foundMajorMark = new List<SpectrumMark>();
            foreach (var mmk in x_freqMarkPanel.Children)
            {
                var mk = mmk as SpectrumMark;
                if (mk == null)
                {
                    continue;
                }
                if (((SpectrumMark)mk).GroupName == DefaultMajorMarkId)
                {
                    foundMajorMark.Add(mk);
                }
            }


            if (foundMajorMark.Any())
            {
                x_freqMarkPanel.Children.Remove(foundMajorMark.First());
            }

            int markshowpos = 0;
            if (SpectrumType == SpectrumType.中频全景)
                markshowpos = (int)x_spectrumDiagram.ActualWidth / 2;
 
            _majorMarkObject = AddFreqMark(DefaultMajorMarkColor, markshowpos, DefaultMajorMarkId);
            if (_majorMarkObject != null & MajorMarkChangedHandler != null)
                MajorMarkChangedHandler();

            if (_majorMarkObject == null)
            {
                return null;
            }

            _majorMarkObject.MarkArrowVisibility = Visibility.Collapsed;
            //xTopControl.Visibility = Visibility.Visible;
            xTopControl.IsEnabled = true;
            return _majorMarkObject;
        }

        /// <summary>
        /// 根据频率值刷新主Mark位置
        /// </summary>
        /// <param name="pFreq">频率值</param>
        public void UpdateMajorFreqPosition(double pFreq)
        {
            if (_majorMarkObject == null)
                return;
            var sp = GetWithinPointData(pFreq);
            if (sp == null)
                return;
            _majorMarkObject.TranslateX = sp.Point.X;
            _majorMarkObject.TranslateY = sp.Data.Dbuv;
            _majorMarkObject.BuoyOffset = sp.Point.Y;
            _majorMarkObject.MarkValue = sp.Data.Freq;
        }

        /// <summary>
        /// 添加频率Mark
        /// </summary>
        public void AddFreqMark()
        {
            if (_markGroupColor.Count == 0)
                return;
            AddFreqMark(_markGroupColor.Dequeue());
        }

        /// <summary>
        /// 初始化谱图范围属性
        /// </summary>
        /// <param name="pUpperLimitValue"></param>
        /// <param name="pLowerLimitValue"></param>
        /// <param name="pBeginLeftValue"></param>
        /// <param name="pEndRightValue"></param>
        public void InitSpectrumProperty(double? pUpperLimitValue, double? pLowerLimitValue, double? pBeginLeftValue, double? pEndRightValue)
        {
            if (pUpperLimitValue != null)
            {
                x_spectrumDiagram.Property.UpperLimitValue = pUpperLimitValue.Value;
            }
            if (pLowerLimitValue != null)
            {
                x_spectrumDiagram.Property.LowerLimitValue = pLowerLimitValue.Value;
            }
            if (pBeginLeftValue != null)
            {
                x_spectrumDiagram.Property.BeginLeftValue = pBeginLeftValue.Value;
                _beginLeftValue = pBeginLeftValue.Value;
            }
            if (pEndRightValue != null)
            {
                x_spectrumDiagram.Property.EndRightValue = pEndRightValue.Value;
                _endRightValue = pEndRightValue.Value;
            }
        }

        /// <summary>
        /// 初始化谱图缓存
        /// </summary>
        /// <param name="pStartFreq"></param>
        /// <param name="pStep"></param>
        /// <param name="pFreqPointCount"></param>
        public void Initializers(double pStartFreq, double pStep, int pFreqPointCount)
        {
            _spectrumDataManage.InitializersCache(pStartFreq, pStep, pFreqPointCount, -999);
            _spectrumBackgroundDataManage.InitializersCache(pStartFreq, pStep, pFreqPointCount, -999);
            _spectrumNoiseDatamanage.InitializersCache(pStartFreq, pStep, pFreqPointCount, -999);
            _spectrumMaxDataManage.InitializersCache(pStartFreq, pStep, pFreqPointCount, -999);
            _spectrumMinDataManage.InitializersCache(pStartFreq, pStep, pFreqPointCount, 123);
            _drawBackground = false;
            _drawNoise = false;
            _drawMax = false;
            _drawMin = false;
            _drawFluoro = false;
            _interFreDuration = 0;
        }

        public void InitializeBackground(double pStartFreq, double pStep, int pFreqPointCount)
        {
            _spectrumBackgroundDataManage.InitializersCache(pStartFreq, pStep, pFreqPointCount);
            _drawBackground = false;
        }

        /// <summary>
        /// 绘制射频全景谱线
        /// </summary>
        /// <param name="pStartFreq">第一个信号频率值 double hz</param>
        /// <param name="pStep">信号之间间隔 hz</param>
        /// <param name="pSecStartFreq">起始频率 double</param>
        /// <param name="pSecStep">步长 int</param>
        /// <param name="pDbuvData">数据值 short[]</param>
        /// <param name="pNumber">频率序号</param>
        /// <param name="pLineColor"></param>
        /// <param name="pLineType"></param>
        /// <param name="pTime">时间戳</param>
        public void DrawLine(double pStartFreq, double pStep, double pSecStartFreq, double pSecStep, short[] pDbuvData, uint pNumber, Color pLineColor, SpectrumLineType pLineType, DateTime? pTime = null)
        {
            if (_spectrumDataManage.FreqPointCount == 0)
                return;
            _realTimeLineColor = pLineColor;
            _realTimeLineType = pLineType;
            _spectrumDataManage.UpdateSpectrumData(pStartFreq, pStep, pSecStartFreq, pSecStep, pNumber, pDbuvData);

            if (NoZoom)
            {
                long freqIndex = pNumber;
                if (freqIndex != -1)
                {
                    x_spectrumDiagram.DrawLine(_spectrumDataManage.FreqPointCount,
                        freqIndex,
                        pDbuvData,
                        pLineColor,
                        pLineType,
                        (short)(freqIndex != 0 ? _spectrumDataManage.GetDbuv(freqIndex - 1) : -12345));
                }
                double bf = _spectrumDataManage.GetFreq(0);
                double ef = _spectrumDataManage.GetFreq(_spectrumDataManage.FreqPointCount - 1);
                OnDrawSpectrumPointCompleted(_spectrumDataManage.GetDbuvData(bf, ef), pTime);
            }
            else
            {
                //为了避免缩放时卡死，缩放范围大时只重绘新的数据部分,但是因为抛数据，缩放后只有一部分在刷新 
                long zoomLeftFreqIndex = _spectrumDataManage.GetIndex(_zoomLeftFreq);
                long zoomRightFreqIndex = _spectrumDataManage.GetIndex(_zoomRightFreq);
                long freqIndex = pNumber;

                if (zoomLeftFreqIndex != -1 && zoomRightFreqIndex != -1 && freqIndex != -1)
                {
                    var dbuvs = _spectrumDataManage.GetDbuvData(_zoomLeftFreq, _zoomRightFreq);
                    if (dbuvs == null)
                        return;
                    if ((_zoomRightFreq - _zoomLeftFreq) < 20000000)//缩放范围20MHz以内，全部重绘
                    {
                        //实时谱线
                        if (_realTimeLineType == SpectrumLineType.Column)
                        {
                            x_spectrumDiagram.DrawColumnLine(dbuvs, _realTimeLineColor);
                        }
                        else if (_realTimeLineType == SpectrumLineType.Wave)
                        {
                            x_spectrumDiagram.DrawWaveLine(dbuvs, _realTimeLineColor);
                        } 
                    }
                    else
                    {  
                        short[] newdbuvData;
                        if ((pNumber <= zoomLeftFreqIndex) && ((pNumber + pDbuvData.Length) >= zoomLeftFreqIndex))
                        {
                            newdbuvData = new short[pDbuvData.Length - (zoomLeftFreqIndex -pNumber)];
                            for (int i = (int)(zoomLeftFreqIndex - pNumber); i < pDbuvData.Length; i++)
                            {
                                newdbuvData[i - (zoomLeftFreqIndex - pNumber)] = pDbuvData[i];
                            }
                            freqIndex = zoomLeftFreqIndex;
                        }
                        else
                        {
                            newdbuvData = pDbuvData;
                            freqIndex = pNumber;
                        }

                        if (freqIndex >= zoomLeftFreqIndex && freqIndex <= zoomRightFreqIndex)
                        {
                            long newindex =  freqIndex - zoomLeftFreqIndex;
                            x_spectrumDiagram.DrawLine(dbuvs.Length,
                            newindex,
                            newdbuvData,
                            pLineColor,
                            pLineType,
                            (short)(freqIndex != 0 ? _spectrumDataManage.GetDbuv(freqIndex - 1) : -12345));
                        }
                    }
                }

                OnDrawSpectrumPointCompleted(_spectrumDataManage.GetDbuvData(_zoomLeftFreq, _zoomRightFreq), pTime);
            }

            if (XBtnAlwaysMaxPeak.IsChecked == true)
            {
                OnLeftRightMaxPeakButtonClick(null, null);
            }
        }

        /// <summary>
        /// 绘制电磁背景
        /// </summary>
        /// <param name="pStartFreq"></param>
        /// <param name="pStep"></param>
        /// <param name="pSecStartFreq"></param>
        /// <param name="pSecStep"></param>
        /// <param name="pDbuvData"></param>
        /// <param name="pLineColor"></param>
        /// <param name="pLineType"></param>
        public void DrawLineToBackground(double pStartFreq, double pStep, double pSecStartFreq, double pSecStep, short[] pDbuvData, Color pLineColor, SpectrumLineType pLineType)
        {
            if (_spectrumBackgroundDataManage.FreqPointCount == 0)
                return;
            _drawBackground = true;
            _backgroundLineColor = pLineColor;
            _backSpectrumLineType = pLineType;
            _spectrumBackgroundDataManage.UpdateSpectrumData(pStartFreq, pStep, pSecStartFreq, pSecStep, pDbuvData);
            double startFreq = pStartFreq;
            if (!pSecStartFreq.Equals(0))
            {
                startFreq = pSecStartFreq;
            }

            if (NoZoom)
            {
                long freqIndex = _spectrumBackgroundDataManage.GetIndex(startFreq);
                if (freqIndex != -1)
                {
                    x_spectrumDiagram.DrawLineToBackground(_spectrumDataManage.FreqPointCount,
                                               freqIndex,
                                               pDbuvData,
                                               pLineColor,
                                               pLineType,
                                               (short)(freqIndex != 0 ? _spectrumBackgroundDataManage.GetDbuv(freqIndex) : -12345));
                }
            }
            else
            {
                //为了避免缩放时卡死缩放范围大时只重绘新的数据部分
                long zoomLeftFreqIndex = _spectrumBackgroundDataManage.GetIndex(_zoomLeftFreq);
                long zoomRightFreqIndex = _spectrumBackgroundDataManage.GetIndex(_zoomRightFreq);
                long freqIndex = _spectrumBackgroundDataManage.GetIndex(startFreq);
                if (zoomLeftFreqIndex != -1 && zoomRightFreqIndex != -1 && freqIndex != -1)
                {
                    short[] dbuvsBk = _spectrumBackgroundDataManage.GetDbuvData(_zoomLeftFreq, _zoomRightFreq);
                    if (dbuvsBk == null)
                    {
                        return;
                    }

                    if ((_zoomRightFreq - _zoomLeftFreq) < 20000000)
                    {
                        //电磁背景
                        if (_drawBackground)
                        {  
                            if (_backSpectrumLineType == SpectrumLineType.Column)
                                x_spectrumDiagram.DrawColumnLineToBackground(dbuvsBk, _backgroundLineColor);
                            else if (_backSpectrumLineType == SpectrumLineType.Wave)
                                x_spectrumDiagram.DrawWaveLineToBackground(dbuvsBk, _backgroundLineColor);
                        }    
                    }
                    else
                    {
                        short[] newdbuvData;
                        if ((freqIndex <= zoomLeftFreqIndex) && ((freqIndex + pDbuvData.Length) >= zoomLeftFreqIndex))
                        {
                            newdbuvData = new short[pDbuvData.Length - (zoomLeftFreqIndex - freqIndex)];
                            for (int i = (int)(zoomLeftFreqIndex - freqIndex); i < pDbuvData.Length; i++)
                            {
                                newdbuvData[i - (zoomLeftFreqIndex - freqIndex)] = pDbuvData[i];
                            }
                            freqIndex = zoomLeftFreqIndex;
                        }
                        else
                        {
                            newdbuvData = pDbuvData;
                            freqIndex = _spectrumBackgroundDataManage.GetIndex(startFreq); ;
                        }
                        if (freqIndex >= zoomLeftFreqIndex && freqIndex <= zoomRightFreqIndex)
                        {
                            long newindex = freqIndex - zoomLeftFreqIndex;
                            x_spectrumDiagram.DrawLineToBackground(dbuvsBk.Length,
                                                    newindex,
                                                    newdbuvData,
                                                    pLineColor,
                                                    pLineType,
                                                    (short)(freqIndex != 0 ? _spectrumBackgroundDataManage.GetDbuv(freqIndex) : -12345));
                        }
                    }
                } 
            }
        }

        /// <summary>
        /// 绘制背景噪声
        /// </summary>
        /// <param name="pStartFreq"></param>
        /// <param name="pStep"></param>
        /// <param name="pSecStartFreq"></param>
        /// <param name="pSecStep"></param>
        /// <param name="pDbuvData"></param>
        /// <param name="pLineColor"></param>
        /// <param name="pLineType"></param>
        public void DrawLineToNoise(double pStartFreq, double pStep, double pSecStartFreq, double pSecStep, short[] pDbuvData, Color pLineColor, SpectrumLineType pLineType)
        {
            if (_spectrumNoiseDatamanage.FreqPointCount == 0)
                return;
            _drawNoise = true;
            _noiseLineColor = pLineColor;
            _backSpectrumLineType = pLineType;
            _spectrumNoiseDatamanage.UpdateSpectrumData(pStartFreq, pStep, pSecStartFreq, pSecStep, pDbuvData);
            double startFreq = pStartFreq;
            if (!pSecStartFreq.Equals(0))
            {
                startFreq = pSecStartFreq;
            }

            if (NoZoom)
            {
                long freqIndex = _spectrumNoiseDatamanage.GetIndex(startFreq);
                if (freqIndex != -1)
                {
                    short predbuv = _spectrumNoiseDatamanage.GetDbuv(freqIndex - 1);
                    if (predbuv == -999)
                    {
                        predbuv = _spectrumNoiseDatamanage.GetDbuv(freqIndex);
                    }
                    x_spectrumDiagram.DrawLineToNoise(_spectrumDataManage.FreqPointCount,
                                               freqIndex,
                                               pDbuvData,
                                               pLineColor,
                                               pLineType,
                                               (short)(freqIndex != 0 ? predbuv : -12345));
                }
            }
            else
            {
                //为了避免缩放时卡死缩放范围大时只重绘新的数据部分
                long zoomLeftFreqIndex = _spectrumDataManage.GetIndex(_zoomLeftFreq);
                long zoomRightFreqIndex = _spectrumDataManage.GetIndex(_zoomRightFreq);
                long freqIndex = _spectrumNoiseDatamanage.GetIndex(startFreq);
                if (zoomLeftFreqIndex != -1 && zoomRightFreqIndex != -1 && freqIndex != -1)
                {
                    short[] dbuvsBk = _spectrumNoiseDatamanage.GetDbuvData(_zoomLeftFreq, _zoomRightFreq);
                    if (dbuvsBk == null)
                    {
                        return;
                    }

                    if ((_zoomRightFreq - _zoomLeftFreq) < 20000000)
                    {
                        if (_drawNoise)
                        {
                            x_spectrumDiagram.DrawWaveLineToNoise(dbuvsBk, _noiseLineColor);
                        }
                    }
                    else
                    {
                        short[] newdbuvData;
                        if ((freqIndex <= zoomLeftFreqIndex) && ((freqIndex + pDbuvData.Length) >= zoomLeftFreqIndex))
                        {
                            newdbuvData = new short[pDbuvData.Length - (zoomLeftFreqIndex - freqIndex)];
                            for (int i = (int)(zoomLeftFreqIndex - freqIndex); i < pDbuvData.Length; i++)
                            {
                                newdbuvData[i - (zoomLeftFreqIndex - freqIndex)] = pDbuvData[i];
                            }
                            freqIndex = zoomLeftFreqIndex;
                        }
                        else
                        {
                            newdbuvData = pDbuvData;
                            freqIndex = _spectrumNoiseDatamanage.GetIndex(startFreq); ;
                        }
                        if (freqIndex >= zoomLeftFreqIndex && freqIndex <= zoomRightFreqIndex)
                        {
                            long newindex = freqIndex - zoomLeftFreqIndex;
                            short predbuv = _spectrumNoiseDatamanage.GetDbuv(freqIndex - 1);
                            if (predbuv == -999)
                            {
                                predbuv = _spectrumNoiseDatamanage.GetDbuv(freqIndex);
                            }
                            x_spectrumDiagram.DrawLineToNoise(dbuvsBk.Length,
                                 newindex,
                                 newdbuvData,
                                 pLineColor,
                                 pLineType,
                                 (short)(freqIndex != 0 ? predbuv : -12345));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 绘制射频全景最大值谱线
        /// </summary>
        /// <param name="pStartFreq"></param>
        /// <param name="pStep"></param>
        /// <param name="pSecStartFreq"></param>
        /// <param name="pSecStep"></param>
        /// <param name="pDbuvData"></param>
        /// <param name="pNumber"></param>
        /// <param name="pLineColor"></param>
        /// <param name="pLineType"></param>
        public void DrawMaxLine(double pStartFreq, double pStep, double pSecStartFreq, double pSecStep, short[] pDbuvData, uint pNumber, Color pLineColor, SpectrumLineType pLineType)
        {
            if (_spectrumMaxDataManage.FreqPointCount == 0)
                return;
            _drawMax = true;
            _maxLineColor = pLineColor;
            _realTimeLineType = pLineType;
            double startFreq = pStartFreq;
            if (!pSecStartFreq.Equals(0))
            {
                startFreq = pSecStartFreq;
            }
            double step = pStep;
            if (!pSecStep.Equals(0))
            {
                step = pSecStep;
            }

            int valueLen = pDbuvData.Length;
            short[] srcDbuvs = new short[valueLen];
            if (valueLen > 1)
            {
                double stopFreq = startFreq + valueLen * step;
                var sDbuvs = _spectrumMaxDataManage.GetDbuvData(startFreq, stopFreq);

                if (sDbuvs == null)
                {
                    return;
                }

                if (sDbuvs.Length < valueLen)
                {
                    for (int i = 0; i < sDbuvs.Length; i++ )
                        srcDbuvs[i] = sDbuvs[i];
                    for (int i = sDbuvs.Length; i < valueLen; i++)
                        srcDbuvs[i] = sDbuvs[sDbuvs.Length - 1];
                } 
                else
                    srcDbuvs = sDbuvs;

                for (int i = 0; i < valueLen; i++)
                {
                    if (srcDbuvs[i].Equals(0) || pDbuvData[i] > srcDbuvs[i])
                        srcDbuvs[i] = pDbuvData[i];
                }
            }
            else if (valueLen == 1)
            {
                srcDbuvs = pDbuvData;
                short dbuv = _spectrumMaxDataManage.GetDbuv(_spectrumMaxDataManage.GetIndex(startFreq));
                if (dbuv > pDbuvData[0])
                    srcDbuvs[0] = dbuv;   
            }
            
            _spectrumMaxDataManage.UpdateSpectrumData(pStartFreq, pStep, pSecStartFreq, pSecStep, srcDbuvs);

            if (NoZoom)
            {
                long freqIndex = _spectrumMaxDataManage.GetIndex(startFreq);
                if (freqIndex != -1)
                {
                    short predbuv = _spectrumMaxDataManage.GetDbuv(freqIndex - 1);
                    if (predbuv == -999)
                    {
                        predbuv = _spectrumDataManage.GetDbuv(freqIndex - 1);
                    }
                    x_spectrumDiagram.DrawLineToMax(_spectrumMaxDataManage.FreqPointCount,
                                               freqIndex,
                                               srcDbuvs,
                                               pLineColor,
                                               pLineType,
                                               (short)(freqIndex != 0 ? predbuv : -12345));
                }
            }
            else
            {
                //为了避免缩放时卡死缩放范围大时只重绘新的数据部分
                long zoomLeftFreqIndex = _spectrumMaxDataManage.GetIndex(_zoomLeftFreq);
                long zoomRightFreqIndex = _spectrumMaxDataManage.GetIndex(_zoomRightFreq);
                long freqIndex = _spectrumMaxDataManage.GetIndex(startFreq);
                if (zoomLeftFreqIndex != -1 && zoomRightFreqIndex != -1 && freqIndex != -1)
                {
                    var dbuvsMax = _spectrumMaxDataManage.GetDbuvData(_zoomLeftFreq, _zoomRightFreq);
                    if (dbuvsMax == null)
                    {
                        return;
                    }

                    if ((_zoomRightFreq - _zoomLeftFreq) < 20000000)
                    {
                        if (_realTimeLineType == SpectrumLineType.Column)
                            x_spectrumDiagram.DrawColumnLineToMax(dbuvsMax, _maxLineColor);
                        else if (_realTimeLineType == SpectrumLineType.Wave)
                            x_spectrumDiagram.DrawWaveLineToMax(dbuvsMax, _maxLineColor);    
                    }
                    else
                    {
                        short[] newdbuvData;
                        if ((freqIndex <= zoomLeftFreqIndex) && ((freqIndex + srcDbuvs.Length) >= zoomLeftFreqIndex))
                        {
                            newdbuvData = new short[srcDbuvs.Length - (zoomLeftFreqIndex - freqIndex)];
                            for (int i = (int)(zoomLeftFreqIndex - freqIndex); i < srcDbuvs.Length; i++)
                            {
                                newdbuvData[i - (zoomLeftFreqIndex - freqIndex)] = srcDbuvs[i];
                            }
                            freqIndex = zoomLeftFreqIndex;
                        }
                        else
                        {
                            newdbuvData = srcDbuvs;
                            freqIndex = _spectrumMaxDataManage.GetIndex(startFreq);
                        }
                        if (freqIndex >= zoomLeftFreqIndex && freqIndex <= zoomRightFreqIndex)
                        {
                            long newindex = freqIndex - zoomLeftFreqIndex;
                            short predbuv = _spectrumMaxDataManage.GetDbuv(freqIndex - 1);
                            if (predbuv == -999)
                            {
                                predbuv = _spectrumDataManage.GetDbuv(freqIndex - 1);
                            }
                            x_spectrumDiagram.DrawLineToMax(dbuvsMax.Length,
                                                newindex,
                                                newdbuvData,
                                                pLineColor,
                                                pLineType,
                                                (short)(freqIndex != 0 ? predbuv : -12345));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 绘制射频全景最小值谱线
        /// </summary>
        /// <param name="pStartFreq"></param>
        /// <param name="pStep"></param>
        /// <param name="pSecStartFreq"></param>
        /// <param name="pSecStep"></param>
        /// <param name="pDbuvData"></param>
        /// <param name="pNumber"></param>
        /// <param name="pLineColor"></param>
        /// <param name="pLineType"></param>
        public void DrawMinLine(double pStartFreq, double pStep, double pSecStartFreq, double pSecStep, short[] pDbuvData, uint pNumber, Color pLineColor, SpectrumLineType pLineType)
        {
            if (_spectrumMinDataManage.FreqPointCount == 0)
                return;
            _drawMin = true;
            _minLineColor = pLineColor;
            _realTimeLineType = pLineType;
            double startFreq = pStartFreq;
            if (!pSecStartFreq.Equals(0))
            {
                startFreq = pSecStartFreq;
            }
            double step = pStep;
            if (!pSecStep.Equals(0))
            {
                step = pSecStep;
            }

            int valueLen = pDbuvData.Length;
            short[] srcDbuvs = new short[valueLen];
            if (valueLen > 1)
            {
                double stopFreq = startFreq + pDbuvData.Length * step;
                var sDbuvs = _spectrumMinDataManage.GetDbuvData(startFreq, stopFreq);

                if (sDbuvs == null)
                {
                    return;
                }

                if (sDbuvs.Length < valueLen)
                {
                    for (int i = 0; i < sDbuvs.Length; i++)
                        srcDbuvs[i] = sDbuvs[i];

                    for (int i = sDbuvs.Length; i < valueLen; i++)
                        srcDbuvs[i] = sDbuvs[sDbuvs.Length - 1];
                }
                else
                    srcDbuvs = sDbuvs;

                for (int i = 0; i < valueLen; i++)
                {
                    if (srcDbuvs[i].Equals(0) || pDbuvData[i] < srcDbuvs[i])
                        srcDbuvs[i] = pDbuvData[i];
                }
            }
            else if (valueLen == 1)
            {
                srcDbuvs = pDbuvData;
                short dbuv = _spectrumMinDataManage.GetDbuv(_spectrumMinDataManage.GetIndex(startFreq));
                if (dbuv < pDbuvData[0])
                    srcDbuvs[0] = dbuv;
            }
            _spectrumMinDataManage.UpdateSpectrumData(pStartFreq, pStep, pSecStartFreq, pSecStep, srcDbuvs);

            if (NoZoom)
            {
                long freqIndex = _spectrumMinDataManage.GetIndex(startFreq);
                if (freqIndex != -1)
                {
                    short predbuv = _spectrumMinDataManage.GetDbuv(freqIndex - 1);
                    if (predbuv == 123)
                    {
                        predbuv = _spectrumDataManage.GetDbuv(freqIndex - 1);
                    }
                    x_spectrumDiagram.DrawLineToMin(_spectrumMinDataManage.FreqPointCount,
                                               freqIndex,
                                               srcDbuvs,
                                               pLineColor,
                                               pLineType,
                                               (short)(freqIndex != 0 ? predbuv : -12345));
                }
            }
            else
            {
                //为了避免缩放时卡死缩放范围大时只重绘新的数据部分
                long zoomLeftFreqIndex = _spectrumMinDataManage.GetIndex(_zoomLeftFreq);
                long zoomRightFreqIndex = _spectrumMinDataManage.GetIndex(_zoomRightFreq);
                long freqIndex = _spectrumMinDataManage.GetIndex(startFreq);
                if (zoomLeftFreqIndex != -1 && zoomRightFreqIndex != -1 && freqIndex != -1)
                {
                    var dbuvsMin = _spectrumMinDataManage.GetDbuvData(_zoomLeftFreq, _zoomRightFreq);
                    if (dbuvsMin == null)
                    {
                        return;
                    }
                    if ((_zoomRightFreq - _zoomLeftFreq) < 20000000)
                    {
                        if (_realTimeLineType == SpectrumLineType.Column)
                            x_spectrumDiagram.DrawColumnLineToMin(dbuvsMin, _minLineColor);
                        else if (_realTimeLineType == SpectrumLineType.Wave)
                            x_spectrumDiagram.DrawWaveLineToMin(dbuvsMin, _minLineColor);    
                    }
                    else
                    {
                        short[] newdbuvData;
                        if ((freqIndex <= zoomLeftFreqIndex) && ((freqIndex + srcDbuvs.Length) >= zoomLeftFreqIndex))
                        {
                            newdbuvData = new short[srcDbuvs.Length - (zoomLeftFreqIndex - freqIndex)];
                            for (int i = (int)(zoomLeftFreqIndex - freqIndex); i < srcDbuvs.Length; i++)
                            {
                                newdbuvData[i - (zoomLeftFreqIndex - freqIndex)] = srcDbuvs[i];
                            }
                            freqIndex = zoomLeftFreqIndex;
                        }
                        else
                        {
                            newdbuvData = srcDbuvs;
                            freqIndex = _spectrumMinDataManage.GetIndex(startFreq);
                        }

                        if (freqIndex >= zoomLeftFreqIndex && freqIndex <= zoomRightFreqIndex)
                        {
                            long newindex = freqIndex - zoomLeftFreqIndex;
                            short predbuv = _spectrumMinDataManage.GetDbuv(freqIndex - 1);
                            if (predbuv == 123)
                            {
                                predbuv = _spectrumDataManage.GetDbuv(freqIndex - 1);
                            }
                            x_spectrumDiagram.DrawLineToMin(dbuvsMin.Length,
                                newindex,
                                newdbuvData,
                                pLineColor,
                                pLineType,
                                (short)(freqIndex != 0 ? predbuv : -12345));
                        }  
                    }
                }
            }
        }
        /// <summary>
        /// 绘制中频全景最大值谱线
        /// </summary>
        /// <param name="pStartFreq"></param>
        /// <param name="pStep"></param>
        /// <param name="pDbuvData"></param>
        /// <param name="pLineColor"></param>
        public void DrawWaveMaxLine(double pStartFreq, double pStep, short[] pDbuvData, Color pLineColor)
        {
            if (pDbuvData == null)
                return;
            if (_spectrumMaxDataManage.FreqPointCount == 0)
                return;
            _drawMax = true;
            _maxLineColor = pLineColor;
            //频率点加步进等于下一个频率点
            var stopFreq = pStartFreq + pDbuvData.Length * pStep;
            var srcDbuvs = _spectrumMaxDataManage.GetDbuvData(pStartFreq, stopFreq);

            for (int i = 0; i < srcDbuvs.Length; i++)
            {
                if (pDbuvData[i] > srcDbuvs[i])
                    srcDbuvs[i] = pDbuvData[i];
            }
            _spectrumMaxDataManage.UpdateSpectrumData(srcDbuvs);

            if (NoZoom)
            {
                long freqIndex = _spectrumMaxDataManage.GetIndex(pStartFreq);
                if (freqIndex != -1)
                {
                    x_spectrumDiagram.DrawLineToMax(_spectrumMaxDataManage.FreqPointCount,
                                               freqIndex,
                                               srcDbuvs,
                                               pLineColor,
                                               SpectrumLineType.Wave);
                }
            }
            else
            {
                Zoom(_zoomLeftFreq, _zoomRightFreq);
            }
        }
        /// <summary>
        /// 绘制中频全景最小值谱线
        /// </summary>
        /// <param name="pStartFreq"></param>
        /// <param name="pStep"></param>
        /// <param name="pDbuvData"></param>
        /// <param name="pLineColor"></param>
        public void DrawWaveMinLine(double pStartFreq, double pStep, short[] pDbuvData, Color pLineColor)
        {
            if (pDbuvData == null)
                return;
            if (_spectrumMinDataManage.FreqPointCount == 0)
                return;
            _drawMin = true;
            _minLineColor = pLineColor;
            //频率点加步进等于下一个频率点
            var stopFreq = pStartFreq + pDbuvData.Length * pStep;
            var srcDbuvs = _spectrumMinDataManage.GetDbuvData(pStartFreq, stopFreq);

            for (int i = 0; i < srcDbuvs.Length; i++)
            {
                if (pDbuvData[i] < srcDbuvs[i])
                    srcDbuvs[i] = pDbuvData[i];
            }
            _spectrumMinDataManage.UpdateSpectrumData(srcDbuvs);
            if (NoZoom)
            {
                long freqIndex = _spectrumMinDataManage.GetIndex(pStartFreq);
                if (freqIndex != -1)
                {
                    x_spectrumDiagram.DrawLineToMin(_spectrumMinDataManage.FreqPointCount,
                                               freqIndex,
                                               srcDbuvs,
                                               pLineColor,
                                               SpectrumLineType.Wave);
                }
            }
            else
            {
                Zoom(_zoomLeftFreq, _zoomRightFreq);
            }
        }

        /// <summary>
        /// 绘制中频全景频谱图
        /// </summary>
        /// <param name="pStartFreq"></param>
        /// <param name="pStep"></param>
        /// <param name="pDbuvData"></param>
        /// <param name="pLineColor"></param>
        /// <param name="pTime"></param>
        public void DrawWaveLine(double pStartFreq, double pStep, short[] pDbuvData, Color pLineColor, DateTime? pTime = null)
        {
            if (_spectrumDataManage.FreqPointCount == 0)
                return;
            _realTimeLineColor = pLineColor;
            _spectrumDataManage.UpdateSpectrumData(pDbuvData);
            if (NoZoom)
            {
                x_spectrumDiagram.DrawWaveLine(pDbuvData, _realTimeLineColor);
                OnDrawSpectrumPointCompleted(pDbuvData, pTime);
            }
            else
            {
                Zoom(_zoomLeftFreq, _zoomRightFreq);
                OnDrawSpectrumPointCompleted(_spectrumDataManage.GetDbuvData(_zoomLeftFreq, _zoomRightFreq), pTime);
            }
            if (XBtnAlwaysMaxPeak.IsChecked == true)
            {
                OnLeftRightMaxPeakButtonClick(null, null);
            }
        }
        /// <summary>
        /// 绘制中频荧光谱图
        /// </summary>
        /// <param name="pStartFreq"></param>
        /// <param name="pStep"></param>
        /// <param name="pDbuvData"></param>
        /// <param name="pLineColor"></param>
        /// <param name="pTime"></param>
        public void DrawFluoroPoint(double pStartFreq, double pStep, short[] pDbuvData,int duration, DateTime pTime)
        {
            if (_spectrumDataManage.FreqPointCount == 0)
                return;
            _drawFluoro = true;
            _interFreTime = pTime;
            _interFreDuration = duration;
            _spectrumDataManage.UpdateSpectrumData(pDbuvData);
            if (NoZoom)
            {
                x_spectrumDiagram.ClearCacheForZoom(_zoomLeftFreq, _zoomRightFreq);
                x_spectrumDiagram.DrawFluoroPoint(pDbuvData, pTime, duration);
            }
            else
            {
                Zoom(_zoomLeftFreq, _zoomRightFreq);
            }

            if (XBtnAlwaysMaxPeak.IsChecked == true)
            {
                OnLeftRightMaxPeakButtonClick(null, null);
            }
        }

        public void SetFluoro(int duaration)
        {
            x_spectrumDiagram.SetFluoro(duaration);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Update()
        {
            x_scaleLinePrompt1.Update();
            x_scaleLinePrompt2.Update();
            x_scaleLineFreq.Update();
            UpdateDbuvMarks();
            UpdateFreqMarks();
            UpdateScanFreqMarks();
        }

        /// <summary>
        /// 清除谱图容器内所有内容
        /// </summary>
        public void Clear()
        {
            _spectrumDataManage.Clear();
            _spectrumBackgroundDataManage.Clear();
            _spectrumNoiseDatamanage.Clear();
            _spectrumMaxDataManage.Clear();
            _spectrumMinDataManage.Clear();
            x_spectrumDiagram.UpdateSpectrumDiagram(true);
            ClearFreqMark();
            ClearDbuvMark();
            ResetZoom();
            //xTopControl.Visibility = Visibility.Collapsed;
            xTopControl.IsEnabled = false;
            xFreqInfo.Visibility = Visibility.Collapsed;
            InitMarkGroupColors();
            _initGroupIffqMark = false;
            ClearScanFreqMark();
            x_IfbwArea.Visibility = Visibility.Collapsed;
            _ifbwfreq = 0;
            _ifqexifbw = 0;
        }

        public void ClearMaxLines()
        {
            _drawMax = false;
            x_spectrumDiagram.ClearMax();
        }

        public void ClearMinLines()
        {
            _drawMin = false;
            x_spectrumDiagram.ClearMin();
        }

        public void ClearFluorogram()
        {
            if (SpectrumType == SpectrumType.中频全景)
            {
                _drawFluoro = false;
                _interFreDuration = 0;
                x_spectrumDiagram.ClearFluorogram();
            }  
        }

        public void StopFluoroTimer()
        {
            x_spectrumDiagram.StopFluoroTimer();
        }

        /// <summary>
        /// 克隆——哪里用？
        /// </summary>
        /// <returns></returns>
        public SpectrumContainerEx Clone()
        {
            var container = new SpectrumContainerEx();
            container.IsShowAmplitudeControler = false;
            container.IsShowTopControler = false;
            container.IsHitTestVisible = false;
            container.UseLayoutRounding = UseLayoutRounding;
            container._thresholdMark = _thresholdMark;
            container._spectrumType = _spectrumType;
            container._initGroupIffqMark = _initGroupIffqMark;
            container._backgroundLineColor = _backgroundLineColor;
            container._noiseLineColor = _noiseLineColor;
            container._backSpectrumLineType = _backSpectrumLineType;
            container._realTimeLineColor = _realTimeLineColor;
            container._realTimeLineType = _realTimeLineType;
            container._beginLeftValue = _beginLeftValue;
            container._endRightValue = _endRightValue;
            container._majorMarkObject = _majorMarkObject;
            container._cursorPanelId = _cursorPanelId;
            container.SpectrumType = SpectrumType;
            container.MeasureUnit = MeasureUnit;
            container.FreqScaleLabelCount = FreqScaleLabelCount;
            container.UpperLowerScaleLabelCount = UpperLowerScaleLabelCount;
            container.DefaultMajorMarkColor = DefaultMajorMarkColor;
            container.Data.BeginLeftValue = Data.BeginLeftValue;
            container.Data.CenterLimitValueColor = Data.CenterLimitValueColor;
            container.Data.EndRightValue = Data.EndRightValue;
            container.Data.LowerLimitValue = Data.LowerLimitValue;
            container.Data.LowerLimitValueColor = Data.LowerLimitValueColor;
            container.Data.PointCount = Data.PointCount;
            container.Data.Span = Data.Span;
            container.Data.UpperLimitValue = Data.UpperLimitValue;
            container.Data.UpperLimitValueColor = Data.UpperLimitValueColor;
            container._spectrumDataManage.InitializersCache(_spectrumDataManage.DefaultStartFreq, _spectrumDataManage.DefaultStep, _spectrumDataManage.FreqPointCount);
            container._spectrumDataManage.InitializersCache(_spectrumDataManage.Stream);
            container._spectrumBackgroundDataManage.InitializersCache(_spectrumBackgroundDataManage.DefaultStartFreq, _spectrumBackgroundDataManage.DefaultStep, _spectrumBackgroundDataManage.FreqPointCount);
            container._spectrumBackgroundDataManage.InitializersCache(_spectrumBackgroundDataManage.Stream);
            container._spectrumNoiseDatamanage.InitializersCache(_spectrumNoiseDatamanage.DefaultStartFreq, _spectrumNoiseDatamanage.DefaultStartFreq, _spectrumNoiseDatamanage.FreqPointCount);
            container._spectrumNoiseDatamanage.InitializersCache(_spectrumNoiseDatamanage.Stream);
            container.Update();
            container.Zoom(container._spectrumDataManage.GetFreq(0), container._spectrumDataManage.GetFreq(container._spectrumDataManage.FreqPointCount - 1));
            return container;
        }

        public SpectrumMark MajorMarkObject
        {
            get { return _majorMarkObject; }
            set { _majorMarkObject = value; }
        }
        /// <summary>
        /// 中频测量中频带宽显示区域
        /// </summary>
        public void UpdateIfbwArea(double ifbw, double freq)
        {
            _ifqexifbw = ifbw;
            _ifbwfreq = freq;
            x_IfbwArea.Visibility = Visibility.Visible;
            double freqleft = freq - ifbw / 2;
            double freqright = freq + ifbw / 2;
            double pXleft;
            double pXright;
            if(NoZoom)
            {
                pXleft = WMonitorUtile.ViewToScreen(_spectrumDataManage.GetIndex(freqleft), x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0);
                pXright = WMonitorUtile.ViewToScreen(_spectrumDataManage.GetIndex(freqright), x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0);
            }
            else
            {
                if(freqleft <= _zoomLeftFreq || freqright >= _zoomRightFreq)
                {
                    x_IfbwArea.Visibility = Visibility.Collapsed;
                    return;
                }

                long lfIndex = _spectrumDataManage.GetRoughlyIndex(_zoomLeftFreq);
                long rfIndex = _spectrumDataManage.GetRoughlyIndex(_zoomRightFreq);
                long fpCount = rfIndex - lfIndex;

                pXleft = WMonitorUtile.ViewToScreen(_spectrumDataManage.GetIndex(freqleft), x_spectrumDiagram.ActualWidth - 1, 0, fpCount - 1, 0);
                pXright = WMonitorUtile.ViewToScreen(_spectrumDataManage.GetIndex(freqright), x_spectrumDiagram.ActualWidth - 1, 0, fpCount - 1, 0);
            }

            IfbwHeight = x_spectrumDiagram.ActualHeight;
            IfbwWidth = pXright - pXleft;

            Canvas.SetZIndex(x_IfbwArea, 1000);
        }

        #endregion Public

        #region events

        public event Action MajorMarkChangedHandler;

        /// <summary>
        /// 移动Mark时引发的事件
        /// </summary>
        public event Action<SpectrumMark> MarkMoveChanged;
        /// <summary>
        /// 移动Mark时频率改变的事件
        /// </summary>
        public event Action<double> MarkMoveFrequencyChanged;

        /// <summary>
        /// 谱线绘制完毕事件
        /// </summary>
        /// <remarks>
        /// short[] - 当前频率范围内绘制的频点对应的幅度值集合
        /// </remarks>
        public event Action<short[], DateTime?> DrawSpectrumPointCompleted;

        private void OnDrawSpectrumPointCompleted(short[] pDbuvs, DateTime? pTime = null)
        {
            if (_initGroupIffqMark && SpectrumType == SpectrumType.中频全景)
            {
                UpdateIffqInfo();
            }

            Action<short[], DateTime?> handler = DrawSpectrumPointCompleted;
            if (handler != null) handler(pDbuvs, pTime);
        }

        /// <summary>
        /// 谱图频率范围发生变化
        /// </summary>
        /// <remarks>
        /// double1 - 起始频率
        /// double2 - 终止频率
        /// </remarks>
        public event Action<double, double> FreqRangeChanged;

        private void OnFreqRangeChanged(double arg1, double arg2)
        {
            Action<double, double> handler = FreqRangeChanged;
            if (handler != null) handler(arg1, arg2);
        }

        /// <summary>
        /// 谱图上下幅度值范围发生变化
        /// </summary>
        /// <remarks>
        /// double1 - 幅度值上限
        /// double2 - 幅度值下限
        /// </remarks>
        public event Action<double, double> DbuvRangeChanged;

        public void OnDbuvRangeChanged(double arg1, double arg2)
        {
            UpdateSpectrumLines();
            Action<double, double> handler = DbuvRangeChanged;
            if (handler != null) handler(arg1, arg2);
        }

        #endregion events

        #region IDisposable

        private bool _disposed;
        private string _measureUnit;
        private Brush _measureUnitForeground;
        private Brush _defaultLabelForeground;

        ~SpectrumContainerEx()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool mDisposing)
        {
            if (!_disposed)
            {
                if (mDisposing)
                {
                    if (_spectrumDataManage != null)
                    {
                        _spectrumDataManage.Dispose();
                    }
                    if (_spectrumBackgroundDataManage != null)
                    {
                        _spectrumBackgroundDataManage.Dispose();
                    }
                    if (_spectrumNoiseDatamanage != null)
                    {
                        _spectrumNoiseDatamanage.Dispose();
                    }
                    if (_spectrumMaxDataManage != null)
                    {
                        _spectrumMaxDataManage.Dispose();
                    }
                    if (_spectrumMinDataManage != null)
                    {
                        _spectrumMinDataManage.Dispose();
                    }
                }
                _disposed = true;
            }
        }

        #endregion IDisposable

        #region INotifyPropertyChanged

        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #region ScanFreqPoint
        public void ShowBuoy()
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
        public void MoveBuoyTo(double xvalue, double yvalue)
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

            if (!marks.Any()) return;

            var mark = marks.First();
            mark.MarkFreValue = xvalue;
            mark.TranslateY = yvalue;
            if (NoZoom)
            {
                long index = _spectrumDataManage.GetIndex(xvalue);
                mark.TranslateX = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0) - 50;
                double pixelVal = WMonitorUtile.ViewToScreen(yvalue, x_spectrumDiagram.ActualHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
                pixelVal = x_spectrumDiagram.ActualHeight - pixelVal;
                mark.MarkOffset = pixelVal - 28;

                if (mark.Visibility == Visibility.Collapsed && _spectrumDataManage.FreqPointCount > 0)
                    mark.Visibility = Visibility.Visible;
            }
            else
            {
                if (mark.MarkFreValue < _zoomLeftFreq || mark.MarkFreValue >= _zoomRightFreq)
                {
                    mark.Visibility = Visibility.Collapsed; /*Mark指向的频点超出缩放范围，隐藏*/
                }
                else
                {
                    long lIndex = _spectrumDataManage.GetIndex(_zoomLeftFreq);
                    long rIndex = _spectrumDataManage.GetIndex(_zoomRightFreq);
                    long index = _spectrumDataManage.GetIndex(xvalue);
                    index = index - lIndex;
                    long count = rIndex - lIndex;
                    mark.TranslateX = WMonitorUtile.ViewToScreen(index, x_spectrumDiagram.ActualWidth - 1, 0, count - 1, 0) - 50;
                    double pixelVal = WMonitorUtile.ViewToScreen(yvalue, x_spectrumDiagram.ActualHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
                    pixelVal = x_spectrumDiagram.ActualHeight - pixelVal;
                    mark.MarkOffset = pixelVal - 28;

                    if (mark.Visibility == Visibility.Collapsed && _spectrumDataManage.FreqPointCount > 0)
                        mark.Visibility = Visibility.Visible;
                }
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
                MarkFreValueConverter = new MarkFreqValueConverter()
            };

            double bf = _spectrumDataManage.GetFreq(0);
            short dbuv = _spectrumDataManage.GetDbuv(0);

            mark.TranslateX = WMonitorUtile.ViewToScreen(0, x_spectrumDiagram.ActualWidth - 1, 0, _spectrumDataManage.FreqPointCount - 1, 0);
            mark.TranslateY = dbuv;
            double pixelVal = WMonitorUtile.ViewToScreen(dbuv, x_spectrumDiagram.ActualHeight, 0, x_spectrumDiagram.Property.UpperLimitValue, x_spectrumDiagram.Property.LowerLimitValue);
            pixelVal = x_spectrumDiagram.ActualHeight - pixelVal;
            mark.MarkOffset = pixelVal;
            mark.MarkFreValue = bf;
            mark.Visibility = Visibility.Visible;

            x_ScanfreqMarkPanel.Children.Add(mark);

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

                MoveBuoyTo(mark.MarkFreValue, mark.TranslateY);
            }
        }
        #endregion ScanFreqPoint

        #region 触摸屏谱图缩放操作
        private TouchPoint _touchPoint;
        private Dictionary<int, TouchPoint> _dicOldTouchPoint = new Dictionary<int, TouchPoint>();
        private Dictionary<int, TouchPoint> _dicNewTouchPoint = new Dictionary<int, TouchPoint>();
        void x_freqPanelGrid_TouchDown(object sender, TouchEventArgs e)
        {
            _mouseDown = true;

            if (_dicOldTouchPoint.Count >= 3)
            {
                _dicOldTouchPoint.Clear();
            }
            _touchPoint = e.GetTouchPoint(x_freqPanelGrid);
            _dicOldTouchPoint[e.TouchDevice.Id] = _touchPoint;
        }
        void x_freqPanelGrid_TouchUp(object sender, TouchEventArgs e)
        {
            _mouseDown = false;

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

                if (Math.Abs((_dicNewTouchPoint.First().Value.Position.X - _dicNewTouchPoint.Last().Value.Position.X)) < Math.Abs((_dicOldTouchPoint.First().Value.Position.X - _dicOldTouchPoint.Last().Value.Position.X)))
                {
                    bzoom = true;
                }
                else
                {
                    bzoom = false;
                }
                if (pt1.X != 0 && pt2.X != 0)
                {
                    Zoom(pt1, pt2, bzoom);
                }
            }

            movecount = 0;
            _dicNewTouchPoint.Clear();
            _dicOldTouchPoint.Clear();
        }
        private TouchPoint _moveoldpoint;
        private int movecount = 0;
        void x_freqPanelGrid_TouchMove(object sender, TouchEventArgs e)
        {
            movecount++;
            if (!_mouseDown)
            {
                //XGZ:显示鼠标当前位置的幅度值和频率值
                return;
            }
            if (_dicNewTouchPoint.Count >= 3)
            {
                _dicNewTouchPoint.Clear();
            }
            TouchPoint pt = e.GetTouchPoint(x_freqPanelGrid);
             _dicNewTouchPoint[e.TouchDevice.Id] = pt;

            if (_dicNewTouchPoint.Count == 1 && _dicOldTouchPoint.Count == 1)
            {
                if (!NoZoom)
                {
                    var startfreq = _spectrumDataManage.GetFreq(0);
                    var endfreq = _spectrumDataManage.GetFreq(_spectrumDataManage.FreqPointCount - 1);
                    double ss = x_spectrumDiagram.ActualWidth / ((endfreq - startfreq) / 1000000);
                    if (movecount == 1)
                    {                       
                        double moves = Math.Abs(_dicNewTouchPoint.First().Value.Position.X - _dicOldTouchPoint.First().Value.Position.X) / ss;
                        if (_dicNewTouchPoint.First().Value.Position.X > _dicOldTouchPoint.First().Value.Position.X)
                        {
                            if (_zoomLeftFreq == startfreq)
                            {
                                _moveoldpoint = pt;
                                return;
                            }
                                
                            _zoomLeftFreq = _zoomLeftFreq - moves * 1000000;
                            _zoomRightFreq = _zoomRightFreq - moves * 1000000;
                        }
                        else
                        {
                            if (_zoomRightFreq == endfreq)
                            {
                                _moveoldpoint = pt;
                                return;
                            }
                            _zoomLeftFreq = _zoomLeftFreq + moves * 1000000;
                            _zoomRightFreq = _zoomRightFreq + moves * 1000000;
                        }
                    }
                    else if (movecount > 1)
                    {
                        double moves = Math.Abs(pt.Position.X - _moveoldpoint.Position.X) / ss;
                        if (_dicNewTouchPoint.First().Value.Position.X > _moveoldpoint.Position.X)
                        {
                            if (_zoomLeftFreq == startfreq)
                            {
                                _moveoldpoint = pt;
                                return;
                            }
                            _zoomLeftFreq = _zoomLeftFreq - moves * 1000000;
                            _zoomRightFreq = _zoomRightFreq - moves * 1000000;
                        }
                        else
                        {
                            if (_zoomRightFreq == endfreq)
                            {
                                _moveoldpoint = pt;
                                return;
                            }
                            _zoomLeftFreq = _zoomLeftFreq + moves * 1000000;
                            _zoomRightFreq = _zoomRightFreq + moves * 1000000;
                        }
                    }
                    
                    if (_zoomLeftFreq < startfreq)
                    {
                        double mm = startfreq - _zoomLeftFreq;
                        _zoomLeftFreq = startfreq;
                        _zoomRightFreq = _zoomRightFreq + mm;
                    }
                    if (_zoomRightFreq > endfreq)
                    {
                        double mm = _zoomRightFreq - endfreq;
                        _zoomLeftFreq = _zoomLeftFreq - mm;
                        _zoomRightFreq = endfreq;
                    }
                    x_spectrumDiagram.Property.BeginLeftValue = _zoomLeftFreq;
                    x_spectrumDiagram.Property.EndRightValue = _zoomRightFreq;

                    OnFreqRangeChanged(_zoomLeftFreq, _zoomRightFreq);

                    UpdateFreqMarks();
                    Zoom(_zoomLeftFreq, _zoomRightFreq);
                }               
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

            int Range = 30;
            if (_dbuvTopNewTouchPoint.Count == 1 && _dbuvTopOldTouchPoint.Count == 1)
            {
                double ss = x_spectrumDiagram.ActualHeight / (this.Data.UpperLimitValue - this.Data.LowerLimitValue);
                if (_dbuvTopmovecount == 1)
                {
                    double moves = Math.Abs(_dbuvTopNewTouchPoint.First().Value.Position.Y - _dbuvTopOldTouchPoint.First().Value.Position.Y) / ss;
                    if (_dbuvTopNewTouchPoint.First().Value.Position.Y > _dbuvTopOldTouchPoint.First().Value.Position.Y)
                    {
                        if (this.Data.UpperLimitValue > (this.Data.LowerLimitValue + Range))
                        {
                            this.Data.UpperLimitValue -= moves;

                            x_btnDbuvTop2.Visibility = Visibility.Collapsed;
                            x_btnDbuvTop1.Visibility = Visibility.Visible;
                        }
                    }
                    else if (_dbuvTopNewTouchPoint.First().Value.Position.Y < _dbuvTopOldTouchPoint.First().Value.Position.Y)
                    {
                        this.Data.UpperLimitValue += moves;

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
                        if (this.Data.UpperLimitValue > (this.Data.LowerLimitValue + Range))
                        {
                            this.Data.UpperLimitValue -= moves;

                            x_btnDbuvTop2.Visibility = Visibility.Collapsed;
                            x_btnDbuvTop1.Visibility = Visibility.Visible;
                        }
                    }
                    else if (_dbuvTopNewTouchPoint.First().Value.Position.Y < _dbuvTopmoveoldpoint.Position.Y)
                    {
                        this.Data.UpperLimitValue += moves;

                        x_btnDbuvTop2.Visibility = Visibility.Visible;
                        x_btnDbuvTop1.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        _dbuvTopmoveoldpoint = pt;
                        return;
                    }
                }
                this.OnDbuvRangeChanged(this.Data.UpperLimitValue, this.Data.LowerLimitValue);
                this.Update();
                ClearFluorogram();
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

            int Range = 30;
            if (_dbuvBottomNewTouchPoint.Count == 1 && _dbuvBottomOldTouchPoint.Count == 1)
            {
                double ss = x_spectrumDiagram.ActualHeight / (this.Data.UpperLimitValue - this.Data.LowerLimitValue);
                if (_dbuvBottommovecount == 1)
                {
                    double moves = Math.Abs(_dbuvBottomNewTouchPoint.First().Value.Position.Y - _dbuvBottomOldTouchPoint.First().Value.Position.Y) / ss;
                    if (_dbuvBottomNewTouchPoint.First().Value.Position.Y > _dbuvBottomOldTouchPoint.First().Value.Position.Y)
                    {
                        if (this.Data.LowerLimitValue < (this.Data.UpperLimitValue - Range))
                        {
                            this.Data.LowerLimitValue += moves;

                            x_btnDbuvBottom2.Visibility = Visibility.Visible;
                            x_btnDbuvBottom1.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        this.Data.LowerLimitValue -= moves;

                        x_btnDbuvBottom2.Visibility = Visibility.Collapsed;
                        x_btnDbuvBottom1.Visibility = Visibility.Visible;
                    }
                }
                else if (_dbuvBottommovecount > 1)
                {
                    double moves = Math.Abs(pt.Position.Y - _dbuvBottommoveoldpoint.Position.Y) / ss;
                    if (_dbuvBottomNewTouchPoint.First().Value.Position.Y > _dbuvBottommoveoldpoint.Position.Y)
                    {
                        if (this.Data.LowerLimitValue < (this.Data.UpperLimitValue - Range))
                        {
                            this.Data.LowerLimitValue += moves;

                            x_btnDbuvBottom2.Visibility = Visibility.Visible;
                            x_btnDbuvBottom1.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        this.Data.LowerLimitValue -= moves;

                        x_btnDbuvBottom2.Visibility = Visibility.Collapsed;
                        x_btnDbuvBottom1.Visibility = Visibility.Visible;
                    }
                }
                this.OnDbuvRangeChanged(this.Data.UpperLimitValue, this.Data.LowerLimitValue);
                this.Update();
                ClearFluorogram();
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

    internal class MarkPanelMenuCommandEx : ICommand
    {
        private readonly SpectrumContainerEx _spectrumContainer;

        public MarkPanelMenuCommandEx(SpectrumContainerEx pSpectrum)
        {
            _spectrumContainer = pSpectrum;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter == null)
                return;
            switch (parameter.ToString())
            {
                case "addDbuvMark":
                    _spectrumContainer.AddDbuvMark(Color.FromArgb(0xff, 0x43, 0x80, 0xff));
                    break;

                case "clearDbuvMark":
                    _spectrumContainer.ClearDbuvMark();
                    break;

                case "addFreqMark":
                    _spectrumContainer.AddFreqMark();
                    break;

                case "clearFreqMark":
                    _spectrumContainer.ClearFreqMark();
                    break;

                case "addGroupFreqMark":
                    _spectrumContainer.AddGroupFreqMark();
                    break;
            }
        }
    }

    internal class SpectrumContainerCommandEx : ICommand
    {
        private readonly SpectrumContainerEx _spectrumContainer;
        private const int Span = 5;
        private const int Range = 30;

        public SpectrumContainerCommandEx(SpectrumContainerEx pSpectrumContainer)
        {
            _spectrumContainer = pSpectrumContainer;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            switch (parameter.ToString())
            {
                case "du1":
                    if (_spectrumContainer.Data.UpperLimitValue > (_spectrumContainer.Data.LowerLimitValue + Range))
                    {
                        _spectrumContainer.Data.UpperLimitValue -= Span;
                    }
                    break;

                case "du2":
                    _spectrumContainer.Data.UpperLimitValue += Span;
                    break;

                case "db1":
                    _spectrumContainer.Data.LowerLimitValue -= Span;
                    break;

                case "db2":
                    if (_spectrumContainer.Data.LowerLimitValue < (_spectrumContainer.Data.UpperLimitValue - Range))
                    {
                        _spectrumContainer.Data.LowerLimitValue += Span;
                    }
                    break;
            }
            _spectrumContainer.OnDbuvRangeChanged(_spectrumContainer.Data.UpperLimitValue, _spectrumContainer.Data.LowerLimitValue);
            _spectrumContainer.Update();
            _spectrumContainer.ClearFluorogram();
        }
    }

    public class SpectrumPointEx
    {
        public SpectrumPointData Data { get; set; }

        public Point Point { get; set; }
    }

    public class MarkFreqValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return 0;
            double v;
            if (double.TryParse(value.ToString(), out v))
            {
                v = Utile.MathNoRound((v / 1000000), 6);
            }
            return string.Format("{0:N6}MHz", v);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public enum SpectrumType
    {
        射频全景,
        中频全景
    }

    public class FreqGroupInfo : INotifyPropertyChanged
    {
        private double _leftFreq;
        private double _rightFreq;
        private double _leftDbuv;
        private double _rightDbuv;
        private SolidColorBrush _dbuvColor;
        private double _freqLength;
        private string _freqLengthUnit;

        public string GroupName { get; set; }

        public double LeftFreq
        {
            get { return _leftFreq; }
            set
            {
                _leftFreq = value;
                OnPropertyChanged("LeftFreq");
            }
        }

        public double RightFreq
        {
            get { return _rightFreq; }
            set
            {
                _rightFreq = value;
                OnPropertyChanged("RightFreq");
            }
        }

        public double LeftDbuv
        {
            get { return _leftDbuv; }
            set
            {
                _leftDbuv = value;
                OnPropertyChanged("LeftDbuv");
            }
        }

        public double RightDbuv
        {
            get { return _rightDbuv; }
            set
            {
                _rightDbuv = value;
                OnPropertyChanged("RightDbuv");
            }
        }

        public SolidColorBrush DbuvColor
        {
            get { return _dbuvColor; }
            set
            {
                _dbuvColor = value;
                OnPropertyChanged("DbuvColor");
            }
        }

        public double FreqLength
        {
            get { return _freqLength; }
            set
            {
                _freqLength = value;
                if (_freqLength < 1)
                {
                    if (FreqLengthUnit != "Hz")
                    {
                        _freqLength = _freqLength * 1000;
                    }
                    switch (FreqLengthUnit)
                    {
                        case "GHz":
                            FreqLengthUnit = "MHz";
                            break;

                        case "MHz":
                            FreqLengthUnit = "kHz";
                            break;

                        case "kHz":
                            FreqLengthUnit = "Hz";
                            break;
                    }
                }
                else if (_freqLength > 999)
                {
                    if (FreqLengthUnit != "GHz")
                    {
                        _freqLength = _freqLength / 1000;
                    }
                    switch (FreqLengthUnit)
                    {
                        case "MHz":
                            FreqLengthUnit = "GHz";
                            break;

                        case "kHz":
                            FreqLengthUnit = "MHz";
                            break;

                        case "Hz":
                            FreqLengthUnit = "khz";
                            break;
                    }
                }
                OnPropertyChanged("FreqLength");
            }
        }

        public string FreqLengthUnit
        {
            get { return _freqLengthUnit; }
            set
            {
                _freqLengthUnit = value;
                OnPropertyChanged("FreqLengthUnit");
            }
        }

        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class SpectrumBeforeSave
    {
        public Brush MeasureUnitForeground { get; set; }

        public Brush DefaultLabelForeground { get; set; }
    }
}
