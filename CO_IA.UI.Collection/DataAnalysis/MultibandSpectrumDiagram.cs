using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Data;

namespace CO_IA.UI.Collection.DataAnalysis
{
    /// <summary>
    /// 多频段谱图控件
    /// </summary>
    public class MultibandSpectrumDiagram : Control
    {
        public static readonly DependencyProperty CurrentDbuvValueProperty = DependencyProperty.Register("CurrentDbuvValue", typeof(int), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty CurrentFreqPointProperty = DependencyProperty.Register("CurrentFreqPoint", typeof(Point), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty CurrentFreqPointVisibilityProperty = DependencyProperty.Register("CurrentFreqPointVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty CurrentFreqValueProperty = DependencyProperty.Register("CurrentFreqValue", typeof(double), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty CurrentOccupancyRateValueProperty = DependencyProperty.Register("CurrentOccupancyRateValue", typeof(int), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty DrawingOccupancyVisibilityProperty = DependencyProperty.Register("DrawingOccupancyVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty DrawingScaleVisibilityProperty = DependencyProperty.Register("DrawingScaleVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty GridColorProperty = DependencyProperty.Register("GridColor", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty GridVisibilityProperty = DependencyProperty.Register("GridVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty IllegalLabelVisibilityProperty = DependencyProperty.Register("IllegalLabelVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty IllegalSignalBackgroundProperty = DependencyProperty.Register("IllegalSignalBackground", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty IllegalSignalBorderProperty = DependencyProperty.Register("IllegalSignalBorder", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty IllegalSignalNameProperty = DependencyProperty.Register("IllegalSignalName", typeof(string), typeof(MultibandSpectrumDiagram), new PropertyMetadata("非法信号"));
        public static readonly DependencyProperty IsIllegalSignalVisibleProperty = DependencyProperty.Register("IsIllegalSignalVisible", typeof(bool), typeof(MultibandSpectrumDiagram), new PropertyMetadata(true, OnIsSignalLabelVisiblePropertyChanged));
        public static readonly DependencyProperty IsLegalSignalVisibleProperty = DependencyProperty.Register("IsLegalSignalVisible", typeof(bool), typeof(MultibandSpectrumDiagram), new PropertyMetadata(true, OnIsSignalLabelVisiblePropertyChanged));
        public static readonly DependencyProperty IsMaxDbuvVisibleProperty = DependencyProperty.Register("IsMaxDbuvVisible", typeof(bool), typeof(MultibandSpectrumDiagram), new PropertyMetadata(true, OnIsSignallVisiblePropertyChanged));
        public static readonly DependencyProperty IsMedianDbuvVisibleProperty = DependencyProperty.Register("IsMedianDbuvVisible", typeof(bool), typeof(MultibandSpectrumDiagram), new PropertyMetadata(true, OnIsSignallVisiblePropertyChanged));
        public static readonly DependencyProperty IsNoiseVisibleProperty = DependencyProperty.Register("IsNoiseVisible", typeof(bool), typeof(MultibandSpectrumDiagram), new PropertyMetadata(true, OnIsSignallVisiblePropertyChanged));
        public static readonly DependencyProperty IsShamSignalVisibleProperty = DependencyProperty.Register("IsShamSignalVisible", typeof(bool), typeof(MultibandSpectrumDiagram), new PropertyMetadata(true, OnIsSignalLabelVisiblePropertyChanged));
        public static readonly DependencyProperty IsStandardProperty = DependencyProperty.Register("IsStandard", typeof(bool), typeof(MultibandSpectrumDiagram), new PropertyMetadata(true));
        public static readonly DependencyProperty IsUnknownSignalVisibleProperty = DependencyProperty.Register("IsUnknownSignalVisible", typeof(bool), typeof(MultibandSpectrumDiagram), new PropertyMetadata(true, OnIsSignalLabelVisiblePropertyChanged));
        public static readonly DependencyProperty LegalLabelVisibilityProperty = DependencyProperty.Register("LegalLabelVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty LegalSignalBackgroundProperty = DependencyProperty.Register("LegalSignalBackground", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty LegalSignalBorderProperty = DependencyProperty.Register("LegalSignalBorder", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty LegalSignalNameProperty = DependencyProperty.Register("LegalSignalName", typeof(string), typeof(MultibandSpectrumDiagram), new PropertyMetadata("合法信号"));
        public static readonly DependencyProperty MaxDbuvLineColorProperty = DependencyProperty.Register("MaxDbuvLineColor", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty MeasureUnitForegroundProperty = DependencyProperty.Register("MeasureUnitForeground", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty MedianDbuvLineColorProperty = DependencyProperty.Register("MedianDbuvLineColor", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty MousePointLinesVisibilityProperty = DependencyProperty.Register("MousePointLinesVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty MousePointProperty = DependencyProperty.Register("MousePoint", typeof(Point), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty NoiseLineColorProperty = DependencyProperty.Register("NoiseLineColor", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty OccupancyRateVisibilityProperty = DependencyProperty.Register("OccupancyRateVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty ScaleLabelForegroundProperty = DependencyProperty.Register("ScaleLabelForeground", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        public static readonly DependencyProperty ShamLabelVisibilityProperty = DependencyProperty.Register("ShamLabelVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty ShamSignalBackGroundProperty = DependencyProperty.Register("ShamSignalBackGround", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty ShamSignalBorderProperty = DependencyProperty.Register("ShamSignalBorder", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty ShamSignalNameProperty = DependencyProperty.Register("ShamSignalName", typeof(string), typeof(MultibandSpectrumDiagram), new PropertyMetadata("虚假信号"));
        public static readonly DependencyProperty SignalTipVisibilityProperty = DependencyProperty.Register("SignalTipVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty UnknownSignalBackgroundProperty = DependencyProperty.Register("UnknownSignalBackground", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty UnknownSignalBorderProperty = DependencyProperty.Register("UnknownSignalBorder", typeof(SolidColorBrush), typeof(MultibandSpectrumDiagram), null);
        public static readonly DependencyProperty UnknownSignalNameProperty = DependencyProperty.Register("UnknownSignalName", typeof(string), typeof(MultibandSpectrumDiagram), new PropertyMetadata("未知信号"));
        public static readonly DependencyProperty UnkonwnLabelVisibilityProperty = DependencyProperty.Register("UnkonwnLabelVisibility", typeof(Visibility), typeof(MultibandSpectrumDiagram), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty MedianDbuvTextProperty = DependencyProperty.Register("MedianDbuvText", typeof(string), typeof(MultibandSpectrumDiagram), new PropertyMetadata("中　 值"));
        public int _startDbuv, _stopDbuv, _startRate, _stopRate;
        private readonly MultibandSpectrumDataManage _dataManage = new MultibandSpectrumDataManage();
        private Size _drawingPanelSize;
        private bool _initializeDbuv, _initializeRate, _mouseDown, _isFreshSpectrum = true;
        private Point _mouseClickPoint;
        private double _positionedFreq;
        private SolidColorBrush _saveBeforeMeasureUnitDefaultForeground;
        private SolidColorBrush _saveBeforeScaleLabelDefaultForeground;
        private Grid _scalePanel;
        private Rectangle _selectedRect;
        private WriteableBitmap _wbRateTargetBitmap;
        private Image _wbRateTargetSource;
        private WriteableBitmap _wbTargetBitmap;
        private Image _wbTargetSource;
        private double _zoomLeftFreq, _zoomRightFreq;
        private StackPanel _zoomLevelButtons;

        public MultibandSpectrumDiagram()
        {
            DefaultStyleKey = typeof(MultibandSpectrumDiagram);
            FreqScaleValues = new ObservableCollection<double>();
            for (int i = 0; i < 7; i++)
            {
                FreqScaleValues.Add(0);
            }

            IsLegalSignalVisible = true;
            IsIllegalSignalVisible = true;
            IsUnknownSignalVisible = true;
            IsShamSignalVisible = false;
            SetSignalVisible(Visibility.Visible);
            MultibandSpectrumDiagramYAxisCommand = new MultibandSpectrumDiagramYAxisCommand(this);
        }

        #region 属性

        public int CurrentDbuvValue
        {
            get { return (int)GetValue(CurrentDbuvValueProperty); }
            set { SetValue(CurrentDbuvValueProperty, value); }
        }

        public Point CurrentFreqPoint
        {
            get { return (Point)GetValue(CurrentFreqPointProperty); }
            set { SetValue(CurrentFreqPointProperty, value); }
        }

        public Visibility CurrentFreqPointVisibility
        {
            get { return (Visibility)GetValue(CurrentFreqPointVisibilityProperty); }
            set { SetValue(CurrentFreqPointVisibilityProperty, value); }
        }

        public double CurrentFreqValue
        {
            get { return (double)GetValue(CurrentFreqValueProperty); }
            set { SetValue(CurrentFreqValueProperty, value); }
        }

        public int CurrentOccupancyRateValue
        {
            get { return (int)GetValue(CurrentOccupancyRateValueProperty); }
            set { SetValue(CurrentOccupancyRateValueProperty, value); }
        }

        public ObservableCollection<int> DbuvScaleValues { get; private set; }

        /// <summary>
        /// 是否显示占用度层
        /// </summary>
        public Visibility DrawingOccupancyVisibility
        {
            get { return (Visibility)GetValue(DrawingOccupancyVisibilityProperty); }
            set { SetValue(DrawingOccupancyVisibilityProperty, value); }
        }

        /// <summary>
        /// 是否显示谱图层
        /// </summary>
        public Visibility DrawingScaleVisibility
        {
            get { return (Visibility)GetValue(DrawingScaleVisibilityProperty); }
            set { SetValue(DrawingScaleVisibilityProperty, value); }
        }

        public ObservableCollection<double> FreqScaleValues { get; private set; }

        public SolidColorBrush GridColor
        {
            get { return (SolidColorBrush)GetValue(GridColorProperty); }
            set { SetValue(GridColorProperty, value); }
        }

        public Visibility GridVisibility
        {
            get { return (Visibility)GetValue(GridVisibilityProperty); }
            set { SetValue(GridVisibilityProperty, value); }
        }

        /// <summary>
        /// 非法信号开关标签显示状态
        /// </summary>
        public Visibility IllegalLabelVisibility
        {
            get { return (Visibility)GetValue(IllegalLabelVisibilityProperty); }
            set { SetValue(IllegalLabelVisibilityProperty, value); }
        }

        public SolidColorBrush IllegalSignalBackground
        {
            get { return (SolidColorBrush)GetValue(IllegalSignalBackgroundProperty); }
            set { SetValue(IllegalSignalBackgroundProperty, value); }
        }

        public SolidColorBrush IllegalSignalBorder
        {
            get { return (SolidColorBrush)GetValue(IllegalSignalBorderProperty); }
            set { SetValue(IllegalSignalBorderProperty, value); }
        }

        /// <summary>
        /// 默认显示“非法信号”
        /// </summary>
        public string IllegalSignalName
        {
            get { return (string)GetValue(IllegalSignalNameProperty); }
            set { SetValue(IllegalSignalNameProperty, value); }
        }

        /// <summary>
        /// 是否显示非法信号
        /// </summary>
        public bool IsIllegalSignalVisible
        {
            get { return (bool)GetValue(IsIllegalSignalVisibleProperty); }
            set { SetValue(IsIllegalSignalVisibleProperty, value); }
        }

        /// <summary>
        /// 是否显示合法信号
        /// </summary>
        public bool IsLegalSignalVisible
        {
            get { return (bool)GetValue(IsLegalSignalVisibleProperty); }
            set { SetValue(IsLegalSignalVisibleProperty, value); }
        }

        public bool IsMaxDbuvVisible
        {
            get { return (bool)GetValue(IsMaxDbuvVisibleProperty); }
            set { SetValue(IsMaxDbuvVisibleProperty, value); }
        }

        public bool IsMedianDbuvVisible
        {
            get { return (bool)GetValue(IsMedianDbuvVisibleProperty); }
            set { SetValue(IsMedianDbuvVisibleProperty, value); }
        }

        public bool IsNoiseVisible
        {
            get { return (bool)GetValue(IsNoiseVisibleProperty); }
            set { SetValue(IsNoiseVisibleProperty, value); }
        }

        /// <summary>
        /// 是否显示虚假信号
        /// </summary>
        public bool IsShamSignalVisible
        {
            get { return (bool)GetValue(IsShamSignalVisibleProperty); }
            set { SetValue(IsShamSignalVisibleProperty, value); }
        }

        /// <summary>
        /// 标识谱图类型，true：标准频段分析，false：自定义频段分析。默认为true。
        /// </summary>
        public bool IsStandard
        {
            get { return (bool)GetValue(IsStandardProperty); }
            set { SetValue(IsStandardProperty, value); }
        }

        /// <summary>
        /// 是否显示未知信号
        /// </summary>
        public bool IsUnknownSignalVisible
        {
            get { return (bool)GetValue(IsUnknownSignalVisibleProperty); }
            set { SetValue(IsUnknownSignalVisibleProperty, value); }
        }

        /// <summary>
        /// 合法信号开关标签显示状态
        /// </summary>
        public Visibility LegalLabelVisibility
        {
            get { return (Visibility)GetValue(LegalLabelVisibilityProperty); }
            set { SetValue(LegalLabelVisibilityProperty, value); }
        }

        public SolidColorBrush LegalSignalBackground
        {
            get { return (SolidColorBrush)GetValue(LegalSignalBackgroundProperty); }
            set { SetValue(LegalSignalBackgroundProperty, value); }
        }

        public SolidColorBrush LegalSignalBorder
        {
            get { return (SolidColorBrush)GetValue(LegalSignalBorderProperty); }
            set { SetValue(LegalSignalBorderProperty, value); }
        }

        /// <summary>
        /// 默认显示“合法信号”
        /// </summary>
        public string LegalSignalName
        {
            get { return (string)GetValue(LegalSignalNameProperty); }
            set { SetValue(LegalSignalNameProperty, value); }
        }

        public SolidColorBrush MaxDbuvLineColor
        {
            get { return (SolidColorBrush)GetValue(MaxDbuvLineColorProperty); }
            set { SetValue(MaxDbuvLineColorProperty, value); }
        }

        public SolidColorBrush MeasureUnitForeground
        {
            get { return (SolidColorBrush)GetValue(MeasureUnitForegroundProperty); }
            set { SetValue(MeasureUnitForegroundProperty, value); }
        }

        public SolidColorBrush MedianDbuvLineColor
        {
            get { return (SolidColorBrush)GetValue(MedianDbuvLineColorProperty); }
            set { SetValue(MedianDbuvLineColorProperty, value); }
        }

        public Point MousePoint
        {
            get { return (Point)GetValue(MousePointProperty); }
            set { SetValue(MousePointProperty, value); }
        }

        public Visibility MousePointLinesVisibility
        {
            get { return (Visibility)GetValue(MousePointLinesVisibilityProperty); }
            set { SetValue(MousePointLinesVisibilityProperty, value); }
        }

        public MultibandSpectrumDiagramYAxisCommand MultibandSpectrumDiagramYAxisCommand { get; set; }

        public SolidColorBrush NoiseLineColor
        {
            get { return (SolidColorBrush)GetValue(NoiseLineColorProperty); }
            set { SetValue(NoiseLineColorProperty, value); }
        }

        public ObservableCollection<int> OccupancyRateValues { get; private set; }

        /// <summary>
        /// 是否显示占用度标尺
        /// </summary>
        public Visibility OccupancyRateVisibility
        {
            get { return (Visibility)GetValue(OccupancyRateVisibilityProperty); }
            set { SetValue(OccupancyRateVisibilityProperty, value); }
        }

        public SolidColorBrush ScaleLabelForeground
        {
            get { return (SolidColorBrush)GetValue(ScaleLabelForegroundProperty); }
            set { SetValue(ScaleLabelForegroundProperty, value); }
        }

        /// <summary>
        /// 虚假信号开关标签显示状态
        /// </summary>
        public Visibility ShamLabelVisibility
        {
            get { return (Visibility)GetValue(ShamLabelVisibilityProperty); }
            set { SetValue(ShamLabelVisibilityProperty, value); }
        }

        public SolidColorBrush ShamSignalBackGround
        {
            get { return (SolidColorBrush)GetValue(ShamSignalBackGroundProperty); }
            set { SetValue(ShamSignalBackGroundProperty, value); }
        }

        public SolidColorBrush ShamSignalBorder
        {
            get { return (SolidColorBrush)GetValue(ShamSignalBorderProperty); }
            set { SetValue(ShamSignalBorderProperty, value); }
        }

        /// <summary>
        /// 默认显示“虚假信号”
        /// </summary>
        public string ShamSignalName
        {
            get { return (string)GetValue(ShamSignalNameProperty); }
            set { SetValue(ShamSignalNameProperty, value); }
        }

        /// <summary>
        /// 显示/隐藏信号前边的提示信息
        /// </summary>
        public Visibility SignalTipVisibility
        {
            get { return (Visibility)GetValue(SignalTipVisibilityProperty); }
            set { SetValue(SignalTipVisibilityProperty, value); }
        }

        public SolidColorBrush UnknownSignalBackground
        {
            get { return (SolidColorBrush)GetValue(UnknownSignalBackgroundProperty); }
            set { SetValue(UnknownSignalBackgroundProperty, value); }
        }

        public SolidColorBrush UnknownSignalBorder
        {
            get { return (SolidColorBrush)GetValue(UnknownSignalBorderProperty); }
            set { SetValue(UnknownSignalBorderProperty, value); }
        }

        /// <summary>
        /// 默认显示“未知信号”
        /// </summary>
        public string UnknownSignalName
        {
            get { return (string)GetValue(UnknownSignalNameProperty); }
            set { SetValue(UnknownSignalNameProperty, value); }
        }

        /// <summary>
        /// 未知信号开关标签显示状态
        /// </summary>
        public Visibility UnkonwnLabelVisibility
        {
            get { return (Visibility)GetValue(UnkonwnLabelVisibilityProperty); }
            set { SetValue(UnkonwnLabelVisibilityProperty, value); }
        }

        /// <summary>
        /// 中值或平均值
        /// </summary>
        public string MedianDbuvText
        {
            get { return (string)GetValue(MedianDbuvTextProperty); }
            set { SetValue(MedianDbuvTextProperty, value); }
        }

        private bool IsInitialized
        {
            get { return _initializeDbuv && _initializeRate && _dataManage != null; }
        }

        #endregion 属性

        #region override & private functions

        /// <summary>
        /// 存储放大时的起始频率和终止频率
        /// </summary>
        public readonly double[] ZoomFreqRange = new double[2];

        public event Action<double> PositioningSignal;

        public event Action<double, double> ZoomWaterfall;

        public void FreshSpectrum()
        {
            if (_zoomLeftFreq.Equals(0) || _zoomRightFreq.Equals(0))
            {
                if (_dataManage != null)
                    DrawFreqBandSpectrum(_dataManage.StartFreq, _dataManage.StopFreq);
            }
            else
                DrawFreqBandSpectrum(_zoomLeftFreq, _zoomRightFreq);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scalePanel = GetTemplateChild("xScalePanel") as Grid;
            if (_scalePanel != null)
            {
                _scalePanel.SizeChanged -= OnScalePanelSizeChanged;
                _scalePanel.SizeChanged += OnScalePanelSizeChanged;
            }
            _wbTargetSource = GetTemplateChild("xDrawingPanel") as Image;
            _wbRateTargetSource = GetTemplateChild("xDrawingRatePanel") as Image;
            var xCanvas = GetTemplateChild("xCanvas") as Canvas;
            if (xCanvas != null)
            {
                xCanvas.MouseMove -= OnCanvasMouseMove;
                xCanvas.MouseMove += OnCanvasMouseMove;
                xCanvas.MouseEnter -= OnCanvasMouseEnter;
                xCanvas.MouseEnter += OnCanvasMouseEnter;
                xCanvas.MouseLeave -= OnCanvasMouseLeave;
                xCanvas.MouseLeave += OnCanvasMouseLeave;
                xCanvas.SizeChanged -= OnCanvasSizeChanged;
                xCanvas.SizeChanged += OnCanvasSizeChanged;
                _selectedRect = GetTemplateChild("xSelectedRect") as Rectangle;
                if (_selectedRect != null)
                {
                    _selectedRect.Visibility = Visibility.Collapsed;
                    _selectedRect.Width = 0;
                    _selectedRect.Height = 0;
                    _selectedRect.Stroke = new SolidColorBrush(Color.FromArgb(0xff, 0xfa, 0xeb, 0xd7));
                    _selectedRect.StrokeThickness = 1;
                }
            }

            var xFreqPanelGrid = GetTemplateChild("x_freqPanelGrid") as Grid;
            if (xFreqPanelGrid != null) 
            {
                xFreqPanelGrid.MouseLeftButtonDown -= OnFreqPanelMouseLeftButtonDown;
                xFreqPanelGrid.MouseLeftButtonUp -= OnFreqPanelMouseLeftButtonUp;
                xFreqPanelGrid.MouseMove -= OnFreqPanelMouseMove;
                xFreqPanelGrid.MouseLeftButtonDown += OnFreqPanelMouseLeftButtonDown;
                xFreqPanelGrid.MouseLeftButtonUp += OnFreqPanelMouseLeftButtonUp;
                xFreqPanelGrid.MouseMove += OnFreqPanelMouseMove;
                xFreqPanelGrid.MouseEnter -= OnFreqPanelGridMouseEnter;
                xFreqPanelGrid.MouseLeave -= OnFreqPanelGridMouseLeave;
                xFreqPanelGrid.MouseEnter += OnFreqPanelGridMouseEnter;
                xFreqPanelGrid.MouseLeave += OnFreqPanelGridMouseLeave;
            }
            _zoomLevelButtons = GetTemplateChild("x_zoomLevelButtons") as StackPanel;
            var autoAdjustButton = GetTemplateChild("xAutoAdjustButton") as ImageButton;
            if (autoAdjustButton != null)
                autoAdjustButton.Click += OnAutoAdjustButtonClick;
        }

        public void UpdateSignal(ElecEnvFlexGridData pEefgd)
        {
            _dataManage.UpdateSignal(pEefgd);
        }

        private static void OnIsSignalLabelVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var msd = d as MultibandSpectrumDiagram;
            if (msd == null)
                return;
            msd.FreshSpectrum();
        }

        private static void OnIsSignallVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var msd = d as MultibandSpectrumDiagram;
            if (msd == null || !msd._isFreshSpectrum)
                return;
            msd.FreshSpectrum();
        }

        /// <summary>
        /// 绘制所有频段谱图
        /// </summary>
        private void DrawFreqBandSpectrum(double pStartFreq, double pStopFreq)
        {
            //if (!IsInitialized || _wbTargetBitmap == null || _wbRateTargetBitmap == null || _wbTargetBitmap.Pixels.Length == 0 || _wbRateTargetBitmap.Pixels.Length == 0)
            if (!IsInitialized || _wbTargetBitmap == null || _wbRateTargetBitmap == null || _wbTargetBitmap.PixelHeight == 0 || _wbRateTargetBitmap.PixelHeight == 0)
                return;

            CurrentFreqPointVisibility = Visibility.Collapsed;
            //_wbTargetBitmap.Clear(Colors.Transparent);
            //_wbRateTargetBitmap.Clear(Colors.Transparent);
            //谱图整体带宽，即逻辑宽度
            double bandWidth = pStopFreq - pStartFreq;
            var tempStartFreq = _dataManage.GetFrequency(pStartFreq);
            var tempStopFreq = _dataManage.GetFrequency(pStopFreq);
            //加减一防止放大时谱图首末可能不画的问题
            int startFreqIndex = _dataManage.GetFrequencyIndex(tempStartFreq) - 1;
            int stopFreqIndex = _dataManage.GetFrequencyIndex(tempStopFreq) + 1;
            const int none = -12345;
            int previousX = none;
            int previousNoiseY = none;
            int previousMedianDbuvY = none;
            int previousMaxDbuvY = none;
            short previousNoise = none;
            short previousMedianDbuv = none;
            short previousMaxDbuv = none;
            //绘制占用度矩形
            foreach (var signal in _dataManage.GetSignalList())
            {
                DrawOccupancyRateRectangle(_drawingPanelSize.Width, bandWidth, signal, pStartFreq);
            }

            for (int i = startFreqIndex; i <= stopFreqIndex; i++)
            {
                var segment = _dataManage.GetData(i);
                if (segment == null)
                    continue;

                var currentX = (int)ViewToScreen(segment.Freq - pStartFreq, _drawingPanelSize.Width, 0, bandWidth, 0);
                if (previousX == none)
                    previousX = currentX;
                if (currentX - previousX < 0)
                    continue;
                //画的是整个频段的起始频点或者各频段的起始频点
                bool isStart = i == startFreqIndex || segment.Index == 0;
                if (segment.Noise != null && IsNoiseVisible)
                {
                    var isOverPrevious = segment.Noise > previousNoise;
                    previousNoiseY = DrawSpectrum((double)segment.Noise, previousX, previousNoiseY, currentX, NoiseLineColor.Color, isStart, isOverPrevious);
                    if (isOverPrevious || previousX != currentX)
                        previousNoise = segment.Noise.Value;
                }
                if (segment.MaxDbuv != null && IsMaxDbuvVisible)
                {
                    var isOverPrevious = segment.MaxDbuv > previousMaxDbuv;
                    previousMaxDbuvY = DrawSpectrum((double)segment.MaxDbuv, previousX, previousMaxDbuvY, currentX, MaxDbuvLineColor.Color, isStart, isOverPrevious);
                    if (isOverPrevious || previousX != currentX)
                        previousMaxDbuv = segment.MaxDbuv.Value;
                }
                if (IsMedianDbuvVisible)
                {
                    var isOverPreviousMedianDbuv = segment.Dbuv > previousMedianDbuv;
                    previousMedianDbuvY = DrawSpectrum(segment.Dbuv, previousX, previousMedianDbuvY, currentX, MedianDbuvLineColor.Color, isStart, isOverPreviousMedianDbuv);
                    if (isOverPreviousMedianDbuv || previousX != currentX)
                        previousMedianDbuv = segment.Dbuv;
                }
                previousX = currentX;
            }
            PositioningFreq(pStartFreq, pStopFreq);
            //_wbTargetBitmap.Invalidate();
            //_wbRateTargetBitmap.Invalidate();
        }

        /// <summary>
        /// 绘制占用度矩形
        /// </summary>
        /// <param name="segmentPixelWidth">画板实际宽度</param>
        /// <param name="segmentFreqWidth">画板逻辑宽度</param>
        /// <param name="signalData">信号信息</param>
        /// <param name="pStartFreq">画板起始逻辑值</param>
        private void DrawOccupancyRateRectangle(double segmentPixelWidth, double segmentFreqWidth, ElecEnvFlexGridData signalData, double pStartFreq)
        {
            if (signalData.SignalCenterFreq * 1000000 + signalData.SignalBandWidth * 1000 / 2 < pStartFreq || signalData.SignalCenterFreq * 1000000 - signalData.SignalBandWidth * 1000 / 2 > pStartFreq + segmentFreqWidth)
                return;
            double midFreq = signalData.SignalCenterFreq * 1000000 - pStartFreq;
            double bandWidth = signalData.SignalBandWidth * 1000;

            var occupancyRate = signalData.SignalOccupy;
            var startFreq = midFreq - bandWidth / 2;
            var stopFreq = midFreq + bandWidth / 2;
            var x1 = (int)ViewToScreen(startFreq, segmentPixelWidth, 0, segmentFreqWidth, 0);
            var x2 = (int)ViewToScreen(stopFreq, segmentPixelWidth, 0, segmentFreqWidth, 0);
            var y1 = (int)ViewToScreen(occupancyRate, _drawingPanelSize.Height, 0, _stopRate, _startRate);
            y1 = (int)(_drawingPanelSize.Height - y1);
            var y2 = (int)_drawingPanelSize.Height;
            Color drawBorderColor, drawBackgroundColor;
            switch (signalData.SignalProperty)
            {
                case SPropertyEnum.Legal:
                    drawBorderColor = LegalSignalBorder.Color;
                    drawBackgroundColor = LegalSignalBackground.Color;
                    break;

                case SPropertyEnum.Illegal:
                    drawBorderColor = IllegalSignalBorder.Color;
                    drawBackgroundColor = IllegalSignalBackground.Color;
                    break;

                case SPropertyEnum.Sham:
                    drawBorderColor = ShamSignalBorder.Color;
                    drawBackgroundColor = ShamSignalBackGround.Color;
                    break;

                default:
                    drawBorderColor = UnknownSignalBorder.Color;
                    drawBackgroundColor = UnknownSignalBackground.Color;
                    break;
            }
            if (signalData.SignalProperty == SPropertyEnum.Legal && IsLegalSignalVisible || signalData.SignalProperty == SPropertyEnum.Illegal && IsIllegalSignalVisible || signalData.SignalProperty == SPropertyEnum.Unknown && IsUnknownSignalVisible || signalData.SignalProperty == SPropertyEnum.Sham && IsShamSignalVisible)
            {
                //_wbRateTargetBitmap.FillRectangle(x1, y1, x2, y2, drawBackgroundColor);
                //_wbRateTargetBitmap.DrawRectangle(x1, y1, x2, y2, drawBorderColor);
            }
        }

        /// <summary>
        /// 绘制谱线
        /// </summary>
        /// <param name="logicalY">当前逻辑Y值</param>
        /// <param name="previousX">上一个点的X值</param>
        /// <param name="previousY">上一个点的Y值</param>
        /// <param name="currentX">当前点的X值</param>
        /// <param name="color">颜色</param>
        /// <param name="isStart">是否频段的起始</param>
        /// <param name="isOverPreviousY">当前逻辑Y值是否大于上一个逻辑Y值</param>
        /// <returns></returns>
        private int DrawSpectrum(double logicalY, int previousX, int previousY, int currentX, Color color, bool isStart, bool isOverPreviousY)
        {
            //if (_wbTargetBitmap == null || _wbTargetBitmap.Pixels.Length == 0)
            if (_wbTargetBitmap == null || _wbTargetBitmap.PixelHeight == 0)    
                return 0;

            //遇到频段的第一个点时不画线，只记录该点的Y值
            if (!isStart)
            {
                if (previousX != currentX)
                {
                    var currentY = (int)(_drawingPanelSize.Height - ViewToScreen(logicalY, _drawingPanelSize.Height, 0, _stopDbuv, _startDbuv));
                    //_wbTargetBitmap.DrawLine(previousX, previousY, currentX, currentY, color);
                    //if (currentX - previousX > 10)
                        //_wbTargetBitmap.FillEllipse(previousX - 3, previousY - 3, previousX + 3, previousY + 3, color);
                    return currentY;
                }
                else
                {
                    //如果当前X值和上一个X值相等，且当前逻辑Y值大于上一个逻辑Y值才绘制新线，否则不绘制
                    //因为一般屏幕像素宽度不超过2000，而相应频点可能有上万个，每个点都计算其屏幕坐标很浪费时间
                    if (isOverPreviousY)
                    {
                        var currentY = (int)(_drawingPanelSize.Height - ViewToScreen(logicalY, _drawingPanelSize.Height, 0, _stopDbuv, _startDbuv));
                        //_wbTargetBitmap.DrawLine(previousX, previousY, currentX, currentY, color);
                        return currentY;
                    }
                    else
                        return previousY;
                }
            }
            else
            {
                var currentY = (int)(_drawingPanelSize.Height - ViewToScreen(logicalY, _drawingPanelSize.Height, 0, _stopDbuv, _startDbuv));
                return currentY;
            }
        }

        /// <summary>
        /// 获取鼠标位置频率值
        /// </summary>
        /// <param name="pPt"></param>
        /// <returns></returns>
        private double GetMousePositionToFreq(Point pPt)
        {
            var startFreqValue = FreqScaleValues[0];
            var stopFreqValue = FreqScaleValues[FreqScaleValues.Count - 1];
            var freqRange = stopFreqValue - startFreqValue;
            var fy = ScreenToViewDouble(pPt.X, freqRange, 0, _drawingPanelSize.Width, 0);
            return startFreqValue + fy;
        }

        private void InitializeFreqScale(double pStartFreq, double pStopFreq)
        {
            if ((pStartFreq != pStopFreq || pStartFreq != 0) && !(pStartFreq < pStopFreq))
                return;

            const int count = 7;
            double scaleValue = pStartFreq;
            double step = (pStopFreq - pStartFreq) / (count - 1);
            FreqScaleValues[0] = pStartFreq;
            for (int i = 1; i < count; i++)
            {
                scaleValue += step;
                FreqScaleValues[i] = scaleValue;
            }
            UpdateFreqScales();
        }

        /// <summary>
        /// 调整谱图Y轴范围使其适合当前谱图范围
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAutoAdjustButtonClick(object sender, RoutedEventArgs e)
        {
            double? min;
            double? max = _dataManage.GetDbuvExtremum(out min);
            if (max != null && min != null && (max - min) >= 20)
            {
                InitializeDbuvScale((int)Math.Floor((double)min) - 5, (int)Math.Ceiling((double)max) + 5);
                EndUpdate();
            }
        }

        private void OnCanvasMouseEnter(object sender, MouseEventArgs e)
        {
            MousePointLinesVisibility = Visibility.Visible;
        }

        private void OnCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            MousePointLinesVisibility = Visibility.Collapsed;
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            var canvas = sender as Canvas;
            var pt = e.GetPosition(canvas);
            UpdateSpectrumTooltipInfo(pt);
        }

        private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _drawingPanelSize = e.NewSize;
            UpdateSize((int)_drawingPanelSize.Width, (int)_drawingPanelSize.Height);
        }

        private void OnFreqPanelGridMouseEnter(object sender, MouseEventArgs e)
        {
            MousePointLinesVisibility = Visibility.Visible;
        }

        private void OnFreqPanelGridMouseLeave(object sender, MouseEventArgs e)
        {
            MousePointLinesVisibility = Visibility.Collapsed;
        }

        private void OnFreqPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (CurrentFreqValue > 0.0 && PositioningSignal != null)
                    PositioningSignal(CurrentFreqValue);

                return;
            }

            var xFreqPanelGrid = sender as Grid;
            if (xFreqPanelGrid == null || !IsInitialized)
                return;

            _mouseDown = true;
            xFreqPanelGrid.CaptureMouse();
            _mouseClickPoint = e.GetPosition(xFreqPanelGrid);
            ZoomFreqRange[0] = GetMousePositionToFreq(_mouseClickPoint);

            if (_selectedRect != null)
            {
                _selectedRect.Width = 0;
                _selectedRect.Height = 0;
                _selectedRect.Visibility = Visibility.Visible;
                Canvas.SetLeft(_selectedRect, _mouseClickPoint.X);
                Canvas.SetTop(_selectedRect, _mouseClickPoint.Y);
            }
        }

        private void OnFreqPanelMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var xFreqPanelGrid = sender as Grid;
            if (xFreqPanelGrid == null || !IsInitialized)
                return;
            _mouseDown = false;
            xFreqPanelGrid.ReleaseMouseCapture();
            var pt = e.GetPosition(xFreqPanelGrid);
            ZoomFreqRange[1] = GetMousePositionToFreq(pt);
            if (_selectedRect != null)
            {
                _selectedRect.Visibility = Visibility.Collapsed;
                //如果矩形宽度小于20，认为是误操作，即不缩放
                if (_selectedRect.Width < 20)
                    return;

                Zoom(ZoomFreqRange[0], ZoomFreqRange[1], _mouseClickPoint.X > pt.X);
            }
        }

        private void OnFreqPanelMouseMove(object sender, MouseEventArgs e)
        {
            var xFreqPanelGrid = sender as Grid;
            if (xFreqPanelGrid == null)
                return;
            if (!_mouseDown)
                //XGZ:显示鼠标当前位置的幅度值和频率值
                return;
            Point pt = e.GetPosition(xFreqPanelGrid);
            double x = _mouseClickPoint.X; // Canvas.GetLeft(_rect);
            double y = _mouseClickPoint.Y; // Canvas.GetTop(_rect);

            if (_selectedRect != null)
            {
                if (pt.X > x && pt.X < xFreqPanelGrid.ActualWidth)
                    _selectedRect.Width = Math.Abs(pt.X - x);
                else if (pt.X < x && pt.X > 0)
                {
                    Canvas.SetLeft(_selectedRect, pt.X);
                    _selectedRect.Width = Math.Abs(pt.X - x);
                }
                if (pt.Y > y && pt.Y < xFreqPanelGrid.ActualHeight)
                    _selectedRect.Height = Math.Abs(pt.Y - y);
                else if (pt.Y < y && pt.Y > 0)
                {
                    Canvas.SetTop(_selectedRect, pt.Y);
                    _selectedRect.Height = Math.Abs(pt.Y - y);
                }
            }
            UpdateSpectrumTooltipInfo(pt);
        }

        private void OnScalePanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateFreqScales();
        }

        private void UpdateFreqScales()
        {
            if (_scalePanel == null)
                return;

            int scaleElementCount = _scalePanel.Children.Count;
            for (int i = 0; i < scaleElementCount; i++)
            {
                var scaleTextBlock = _scalePanel.Children[i] as FrameworkElement;
                if (scaleTextBlock == null)
                    continue;

                double translateX;
                if (i < scaleElementCount - 1)
                    translateX = -scaleTextBlock.ActualWidth / 2;
                else
                    translateX = scaleTextBlock.ActualWidth / 2;
                //scaleTextBlock.RenderTransform = new CompositeTransform
                //{
                //    TranslateX = translateX
                //};
            }
        }

        private void UpdateSize(int w, int h)
        {
            //if (_wbTargetBitmap != null && _wbTargetBitmap.Pixels.Count() > 0)
            //    _wbTargetBitmap.Clear(Colors.Transparent);
            //if (_wbTargetBitmap == null || _wbTargetBitmap.PixelWidth != w || _wbTargetBitmap.PixelHeight != h)
            //{
            //    _wbTargetBitmap = new WriteableBitmap(w, h);
            //    _wbTargetSource.Source = _wbTargetBitmap;
            //}
            //if (_wbRateTargetBitmap != null && _wbRateTargetBitmap.Pixels.Count() > 0)
            //    _wbRateTargetBitmap.Clear(Colors.Transparent);
            //if (_wbRateTargetBitmap == null || _wbRateTargetBitmap.PixelWidth != w || _wbRateTargetBitmap.PixelHeight != h)
            //{
            //    _wbRateTargetBitmap = new WriteableBitmap(w, h);
            //    _wbRateTargetSource.Source = _wbRateTargetBitmap;
            //}

            FreshSpectrum();
        }

        private void UpdateSpectrumTooltipInfo(Point pt)
        {
            if (!IsInitialized)
                return;
            MousePoint = new Point(pt.X, pt.Y);
            //获取频率值
            CurrentFreqValue = GetMousePositionToFreq(MousePoint);
            //获取幅度值
            var startDbuvValue = DbuvScaleValues[0];
            var stopDbuvValue = DbuvScaleValues[DbuvScaleValues.Count - 1];
            var dbuvValueStep = stopDbuvValue - DbuvScaleValues[DbuvScaleValues.Count - 2];
            var dbuvRange = (stopDbuvValue + dbuvValueStep) - startDbuvValue;
            var dy = ScreenToView(MousePoint.Y, dbuvRange, 0, _drawingPanelSize.Height, 0);
            CurrentDbuvValue = (int)(stopDbuvValue + dbuvValueStep - dy);
            //获取占用度
            var startRateValue = OccupancyRateValues[0];
            var stopRateValue = OccupancyRateValues[OccupancyRateValues.Count - 1];
            var rateValueStep = stopRateValue - OccupancyRateValues[OccupancyRateValues.Count - 2];
            var rateRange = (stopRateValue + rateValueStep) - startRateValue;
            var oy = ScreenToView(MousePoint.Y, rateRange, 0, _drawingPanelSize.Height, 0);
            CurrentOccupancyRateValue = (int)(stopRateValue + rateValueStep - oy);
        }

        /// <summary>
        /// 缩放谱图
        /// </summary>
        /// <param name="pStartFreq">起始频率</param>
        /// <param name="pStopFreq">终止频率</param>
        /// <param name="pIsZoomOut">true - 缩小，false - 放大</param>
        private void Zoom(double pStartFreq, double pStopFreq, bool pIsZoomOut)
        {
            if (!IsInitialized)
                return;

            if (pIsZoomOut)
            {
                FullView();
                if (ZoomWaterfall != null)
                    ZoomWaterfall(0, 0);
            }
            else
            {
                var zoomChildCount = _zoomLevelButtons.Children.Count;
                if (zoomChildCount >= 6)
                {
                    //DxDialogEx.ShowMessage("放大次数已到达上限");
                    return;
                }
                var signalList = _dataManage.GetSignalList();
                if (signalList.Count != 0)
                {
                    double minBandWidth = IsStandard ? signalList.Select(t => t.SignalBandWidth).Min() : signalList.Select(t => t.SignalBandWidth).Min();

                    if ((pStopFreq - pStartFreq) < minBandWidth * 1000)
                    {
                        //DxDialogEx.ShowMessage("范围太小无法满足放大需求");
                        return;
                    }
                }
                else
                {
                    var startIndex = _dataManage.GetFrequencyIndex(pStartFreq);
                    var stopIndex = _dataManage.GetFrequencyIndex(pStopFreq);
                    if (startIndex == 0 && stopIndex == 0)
                        return;
                    //如果谱图中的频点个数小于20则不予放大
                    if (stopIndex - startIndex < 20)
                    {
                        //DxDialogEx.ShowMessage("范围太小无法满足放大需求");
                        return;
                    }
                }
                //当放很大时，可能会触发WriteableBitmap中画矩形方法的bug
                //if (pStopFreq - pStartFreq < 12)
                //{
                //    DxDialogEx.ShowMessage("范围太小无法满足放大需求");
                //    return;
                //}

                _zoomLeftFreq = pStartFreq;
                _zoomRightFreq = pStopFreq;
                var zlButton = new ImageButton
                {
                    Content = _zoomLevelButtons.Children.Count + 1,
                    Margin = new Thickness(5, 0, 0, 0),
                    Padding = new Thickness(5, 3, 5, 5),
                    Tag = new[]
                    {
                        pStartFreq,
                        pStopFreq
                    },
                    Foreground = new SolidColorBrush(Colors.White)
                };
                _zoomLevelButtons.Children.Add(zlButton);
                zlButton.Click += (sender, args) =>
                {
                    var z = (double[])((Button)sender).Tag;
                    _zoomLeftFreq = z[0];
                    _zoomRightFreq = z[1];
                    InitializeFreqScale(_zoomLeftFreq, _zoomRightFreq);
                    DrawFreqBandSpectrum(_zoomLeftFreq, _zoomRightFreq);
                };
                InitializeFreqScale(pStartFreq, pStopFreq);
                DrawFreqBandSpectrum(pStartFreq, pStopFreq);
                if (ZoomWaterfall != null)
                    ZoomWaterfall(pStartFreq, pStopFreq);
            }
        }

        #endregion override & private functions

        #region public functions

        /// <summary>
        /// 添加谱数据
        /// </summary>
        /// <param name="pMultibandSpectrumData">多频段谱数据</param>
        public void AddSpectrumData(MultibandSpectrumData pMultibandSpectrumData)
        {
            if (pMultibandSpectrumData == null)
                throw new Exception("pMultibandSpectrumData 参数不能为空");
            _dataManage.Add(pMultibandSpectrumData);
        }

        public void BeginSave()
        {
            _saveBeforeMeasureUnitDefaultForeground = MeasureUnitForeground;
            _saveBeforeScaleLabelDefaultForeground = ScaleLabelForeground;
            var blackBrush = new SolidColorBrush(Colors.Black);
            MeasureUnitForeground = blackBrush;
            ScaleLabelForeground = blackBrush;
            GridVisibility = Visibility.Collapsed;
            CurrentFreqPointVisibility = Visibility.Collapsed;
        }

        public void BeginUpdate()
        {
            _dataManage.Clear();
        }

        public void Clear()
        {
            _dataManage.Clear();
            FullView();
            //if (_wbTargetBitmap == null || _wbRateTargetBitmap == null || _wbTargetBitmap.Pixels.Length == 0 || _wbRateTargetBitmap.Pixels.Length == 0)
            if (_wbTargetBitmap == null || _wbRateTargetBitmap == null || _wbTargetBitmap.PixelHeight == 0 || _wbRateTargetBitmap.PixelHeight == 0)   
                return;
            //_wbTargetBitmap.Clear(Colors.Transparent);
            //_wbRateTargetBitmap.Clear(Colors.Transparent);
            //_wbTargetBitmap.Invalidate();
            //_wbRateTargetBitmap.Invalidate();
            CurrentFreqPointVisibility = Visibility.Collapsed;
        }

        public void EndSave()
        {
            MeasureUnitForeground = _saveBeforeMeasureUnitDefaultForeground;
            ScaleLabelForeground = _saveBeforeScaleLabelDefaultForeground;
            GridVisibility = Visibility.Visible;
        }

        /// <summary>
        /// 刷新谱图
        /// </summary>
        public void EndUpdate()
        {
            _dataManage.UpdateData();
            if (_dataManage.StartFreq.Equals(0) || _dataManage.StopFreq.Equals(0))
                return;
            //画图后未进行缩放时
            if (_zoomLeftFreq.Equals(0) || _zoomRightFreq.Equals(0))
                InitializeFreqScale(_dataManage.StartFreq, _dataManage.StopFreq);
            else //谱图缩放时
                InitializeFreqScale(_zoomLeftFreq, _zoomRightFreq);

            FreshSpectrum();
        }

        /// <summary>
        /// 谱图全景
        /// </summary>
        public void FullView()
        {
            _zoomLeftFreq = 0;
            _zoomRightFreq = 0;
            if (_zoomLevelButtons != null)
                _zoomLevelButtons.Children.Clear();
            InitializeFreqScale(_dataManage.StartFreq, _dataManage.StopFreq);
            DrawFreqBandSpectrum(_dataManage.StartFreq, _dataManage.StopFreq);
        }

        /// <summary>
        /// 初始化幅度值刻度内容
        /// </summary>
        /// <param name="pStartDbuv"></param>
        /// <param name="pStopDbuv"></param>
        public void InitializeDbuvScale(int pStartDbuv, int pStopDbuv)
        {
            _initializeDbuv = false;
            if (pStartDbuv > pStopDbuv)
                return;
            _startDbuv = pStartDbuv;
            _stopDbuv = pStopDbuv;
            _initializeDbuv = true;
            const int count = 5;
            int scaleValue = pStartDbuv;
            int step = (pStopDbuv - pStartDbuv) / count;
            if (DbuvScaleValues == null)
            {
                DbuvScaleValues = new ObservableCollection<int>();
                for (int i = 0; i < count; i++)
                {
                    DbuvScaleValues.Add(0);
                }
            }
            for (int i = 0; i < count; i++)
            {
                DbuvScaleValues[i] = scaleValue;
                scaleValue += step;
            }
        }

        /// <summary>
        /// 初始化占用度值刻度内容
        /// </summary>
        /// <param name="pStartRate"></param>
        /// <param name="pStopRate"></param>
        public void InitializeOccupancyRateScale(int pStartRate, int pStopRate)
        {
            _initializeRate = false;
            if (pStartRate > pStopRate)
                return;
            _startRate = pStartRate;
            _stopRate = pStopRate;
            _initializeRate = true;
            const int count = 5;
            int scaleValue = pStartRate;
            int step = (pStopRate - pStartRate) / count;
            if (OccupancyRateValues == null)
            {
                OccupancyRateValues = new ObservableCollection<int>();
                for (int i = 0; i < count; i++)
                {
                    OccupancyRateValues.Add(0);
                }
            }
            for (int i = 0; i < count; i++)
            {
                OccupancyRateValues[i] = scaleValue;
                scaleValue += step;
            }
        }

        /// <summary>
        /// 在谱图中定位指定的频点
        /// </summary>
        /// <param name="pFreq">频率值（单位：Hz）</param>
        public void PositioningFreq(double pFreq)
        {
            _positionedFreq = pFreq;
            if (pFreq < _dataManage.StartFreq || pFreq > _dataManage.StopFreq)
                return;

            double startFreq, stopFreq;
            //没有缩放时
            if (_zoomLeftFreq.Equals(0) && _zoomRightFreq.Equals(0))
            {
                startFreq = _dataManage.StartFreq;
                stopFreq = _dataManage.StopFreq;
            }
            else
            {
                if (pFreq < _zoomLeftFreq || pFreq > _zoomRightFreq)
                {
                    var width = _zoomRightFreq - _zoomLeftFreq;
                    var centerFreq = width / 2d;
                    var tempStartFreq = pFreq - centerFreq;
                    var tempStopFreq = pFreq + centerFreq;
                    //保证缩放比例不变
                    if (tempStartFreq <= _dataManage.StartFreq)
                    {
                        startFreq = _dataManage.StartFreq;
                        stopFreq = startFreq + width;
                    }
                    //保证缩放比例不变
                    else if (tempStopFreq >= _dataManage.StopFreq)
                    {
                        stopFreq = _dataManage.StopFreq;
                        startFreq = stopFreq - width;
                    }
                    else
                    {
                        startFreq = _dataManage.GetFrequency(tempStartFreq);
                        stopFreq = _dataManage.GetFrequency(tempStopFreq);
                    }
                    _zoomLeftFreq = startFreq;
                    _zoomRightFreq = stopFreq;
                }
                else
                {
                    startFreq = _zoomLeftFreq;
                    stopFreq = _zoomRightFreq;
                }
            }

            InitializeFreqScale(startFreq, stopFreq);
            DrawFreqBandSpectrum(startFreq, stopFreq);
        }

        /// <summary>
        /// 设置显示/隐藏所有信号按钮
        /// </summary>
        public void SetSignalVisible(Visibility pVisibility)
        {
            LegalLabelVisibility = pVisibility;
            IllegalLabelVisibility = pVisibility;
            UnkonwnLabelVisibility = pVisibility;
            ShamLabelVisibility = pVisibility;
        }

        /// <summary>
        /// 设置信号按钮显示/隐藏
        /// </summary>
        public void SetSignalVisible(Visibility pLegalLabelVisibility, Visibility pIllegalLabelVisibility, Visibility pUnkonwnLabelVisibility, Visibility pShamLabelVisibility)
        {
            LegalLabelVisibility = pLegalLabelVisibility;
            IllegalLabelVisibility = pIllegalLabelVisibility;
            UnkonwnLabelVisibility = pUnkonwnLabelVisibility;
            ShamLabelVisibility = pShamLabelVisibility;
        }

        /// <summary>
        /// 设置谱线的显示状态
        /// </summary>
        /// <param name="pIsMaxDbuvVisible"></param>
        /// <param name="pIsMedianDbuvVisible"></param>
        /// <param name="pIsNoiseVisible"></param>
        public void SetSpectrumVisibility(bool pIsMaxDbuvVisible, bool pIsMedianDbuvVisible, bool pIsNoiseVisible)
        {
            _isFreshSpectrum = false;
            IsMaxDbuvVisible = pIsMaxDbuvVisible;
            IsMedianDbuvVisible = pIsMedianDbuvVisible;
            IsNoiseVisible = pIsNoiseVisible;
            _isFreshSpectrum = true;
        }

        private void PositioningFreq(double pStartFreq, double pStopFreq)
        {
            //定位当前中值频点
            if (!_positionedFreq.Equals(0))
            {
                var index = _dataManage.GetFrequencyIndex(_positionedFreq);
                var segment = _dataManage.GetData(index);
                double height;
                if (segment != null)
                    height = ViewToScreen(segment.Dbuv, _drawingPanelSize.Height, 0, _stopDbuv, _startDbuv);
                else
                    height = ViewToScreen(0, _drawingPanelSize.Height, 0, _stopDbuv, _startDbuv);
                var x = ViewToScreen(_positionedFreq, _drawingPanelSize.Width, 0, pStopFreq, pStartFreq);
                CurrentFreqPoint = new Point(x - 5, _drawingPanelSize.Height - height - 24);
                CurrentFreqPointVisibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// 逻辑视图范围的值转换为屏幕范围内的像素值
        /// </summary>
        /// <param name="pLogicalXorY">X | Y</param>
        /// <param name="pXYwmax">windows窗口坐标最大值</param>
        /// <param name="pXYwmin">windows窗口坐标最小值</param>
        /// <param name="pXYvmax">实际可视窗口逻辑最大值</param>
        /// <param name="pXYvmin">实际可视窗口逻辑最小值</param>
        /// <param name="pReversal">取反值 </param>
        /// <returns>像素坐标 x | y</returns>
        public long ViewToScreen(double pLogicalXorY, double pXYwmax, double pXYwmin, double pXYvmax, double pXYvmin, bool pReversal = false)
        {
            return (long)ViewToScreenDouble(pLogicalXorY, pXYwmax, pXYwmin, pXYvmax, pXYvmin, pReversal);
        }
        public double ViewToScreenDouble(double pLogicalXorY, double pXYwmax, double pXYwmin, double pXYvmax, double pXYvmin, bool pReversal = false)
        {
            double logicalXorY = pLogicalXorY;
            double xywmax = pXYwmax;
            double xywmin = pXYwmin;
            double xyvmax = pXYvmax;
            double xyvmin = pXYvmin;
            double d = 0;
            if (xyvmin < 0)
            {
                d = Math.Abs(xyvmin);
            }
            else if (xyvmin > 0)
            {
                d = -xyvmin;
            }
            xyvmin = 0;
            xyvmax = xyvmax + d;
            logicalXorY = logicalXorY + d;
            if (pReversal)
            {
                logicalXorY = xyvmax - logicalXorY;
            }

            return ((xywmax - xywmin) / (xyvmax - xyvmin)) * logicalXorY +
                   (xyvmin - ((xywmax - xywmin) / (xyvmax - xyvmin)) * xywmin);
        }
        /// <summary>
        /// 屏幕范围内像素值转换为逻辑视图的像素值
        /// </summary>
        /// <param name="pLogicalXorY">X | Y</param>
        /// <param name="pXYvmax"></param>
        /// <param name="pXYvmin"></param>
        /// <param name="pXYwmax"></param>
        /// <param name="pXYwmin"></param>
        /// <param name="pReversal"> </param>
        /// <returns></returns>
        public long ScreenToView(double pLogicalXorY, double pXYvmax, double pXYvmin, double pXYwmax, double pXYwmin, bool pReversal = false)
        {
            return ViewToScreen(pLogicalXorY, pXYvmax, pXYvmin, pXYwmax, pXYwmin, pReversal);
        }
        public double ScreenToViewDouble(double pLogicalXorY, double pXYvmax, double pXYvmin, double pXYwmax, double pXYwmin, bool pReversal = false)
        {
            return ViewToScreenDouble(pLogicalXorY, pXYvmax, pXYvmin, pXYwmax, pXYwmin, pReversal);
        }
        #endregion public functions
    }

    public class MultibandSpectrumDiagramYAxisCommand : ICommand
    {
        private const int Range = 30;
        private const int Span = 5;
        private readonly MultibandSpectrumDiagram _diagram;

        public event EventHandler CanExecuteChanged;

        public MultibandSpectrumDiagramYAxisCommand(MultibandSpectrumDiagram pSpectrumContainer)
        {
            _diagram = pSpectrumContainer;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            switch (parameter.ToString())
            {
                case "du1":
                    if (_diagram._stopDbuv > (_diagram._startDbuv + Range))
                    {
                        _diagram._stopDbuv -= Span;
                    }
                    break;

                case "du2":
                    _diagram._stopDbuv += Span;
                    break;

                case "db1":
                    _diagram._startDbuv -= Span;
                    break;

                case "db2":
                    if (_diagram._startDbuv < (_diagram._stopDbuv - Range))
                    {
                        _diagram._startDbuv += Span;
                    }
                    break;
            }
            _diagram.InitializeDbuvScale(_diagram._startDbuv, _diagram._stopDbuv);
            _diagram.EndUpdate();
        }
    }

    #region 转换接口

    public class MultibandSpectrumDiagramCanvasObjLeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var tb = value as TextBlock;
            if (tb == null)
                return new Thickness(0);
            return new Thickness(-tb.ActualWidth / 2, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class MultibandSpectrumDiagramLeftCanvasConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var left = (double)value;
            return -left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class MultibandSpectrumDiagramMarginTopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var top = (double)value;

            return new Thickness(0, top - 7, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    #endregion 转换接口
}
