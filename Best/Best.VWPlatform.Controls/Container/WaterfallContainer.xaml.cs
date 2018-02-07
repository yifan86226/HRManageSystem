using Best.VWPlatform.Controls.Common;
using Best.VWPlatform.Controls.Freq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    /// WaterfallContainer.xaml 的交互逻辑
    /// </summary>
    public partial class WaterfallContainer : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region 变量
        private readonly SpectrumDataCacheManage _spectrumDataManage = new SpectrumDataCacheManage();
        private int _startSec;
        private int _stopSec;
        private readonly ObservableCollection<Color[]> _waterfallCache = new ObservableCollection<Color[]>();
        private WriteableBitmap _wbColorPanel;
        private string _measureUnit;

        #endregion
        public WaterfallContainer()
        {
            InitializeComponent();
            Init();
        }

        #region 内部
        private void Init()
        {
            MeasureUnit = "dBμV";
            MeasureUnitForeground = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xa8, 0x00));
            DefaultLabelForeground = new SolidColorBrush(Colors.White);
            xMeasureUnitPanel.SizeChanged += OnMeasureUnitPanelSizeChanged;
            Data.UpperLimitValue = 80;
            Data.LowerLimitValue = -20;
            Data.BeginLeftValue = 0;
            Data.EndRightValue = 100;
            StartSec = 0;
            StopSec = 25;
            DataContext = this;
            SizeChanged += WaterfallContainerSizeChanged;
            _waterfallCache.CollectionChanged += WaterfallCacheOnCollectionChanged;
        }

        private void OnMeasureUnitPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //xMeasureUnitPanel.Margin = new Thickness(-(e.NewSize.Width / 2 - 5), 0, 0, 0);
        }

        private void WaterfallCacheOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            UpdateWaterfall();
        }

        private void WaterfallContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //_wbColorPanel = new WriteableBitmap(x_colorRect, null);
        }

        internal void UpdateScaleLinePrompt()
        {
            x_scaleLinePrompt1.Update();
            x_scaleLinePrompt2.Update();
        }

        internal void UpdateWaterfall()
        {

        }
        private Color GetColorFromColorPanel(int pValue)
        {
            if (_wbColorPanel == null)
                return Colors.Transparent;
            if (pValue < 0)
                return _wbColorPanel.GetPixel(1, 0);
            if (pValue >= _wbColorPanel.PixelHeight)
                return _wbColorPanel.GetPixel(1, _wbColorPanel.PixelHeight - 1);
            return _wbColorPanel.GetPixel(1, pValue);
        }

        #endregion

        #region 属性

        public double DefaultColumn3Width { get; private set; }

        public SpectrumDiagramProperty Data
        {
            get { return x_spectrumDiagram.Property; }
        }

        public string MeasureUnit
        {
            get { return _measureUnit; }
            set
            {
                _measureUnit = value;
                OnPropertyChanged("MeasureUnit");
            }
        }

        /// <summary>
        /// 起始秒数
        /// </summary>
        public int StartSec
        {
            get { return _startSec; }
            set
            {
                _startSec = value;
                OnPropertyChanged("StartSec");
            }
        }

        /// <summary>
        /// 终止秒数
        /// </summary>
        public int StopSec
        {
            get { return _stopSec; }
            set
            {
                _stopSec = value;
                OnPropertyChanged("StopSec");
            }
        }
        /// <summary>
        /// 是否隐藏左侧幅度值面板
        /// </summary>
        public bool IsHideLeftAmplitudePanel
        {
            get { return _isHideLeftAmplitudePanel; }
            set
            {
                _isHideLeftAmplitudePanel = value;
                xLeftAmplitudePanel.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
                xContainer.ColumnDefinitions[0].Width = value ? new GridLength(0, GridUnitType.Auto) : new GridLength(45, GridUnitType.Pixel);
                if (IsHideRightAmplitudePanel && !value)
                {
                    xContainer.RowDefinitions[0].Height = value
                                                              ? new GridLength(0, GridUnitType.Pixel)
                                                              : new GridLength(10, GridUnitType.Pixel);
                    xContainer.RowDefinitions[2].Height = value
                                                              ? new GridLength(0, GridUnitType.Pixel)
                                                              : new GridLength(0, GridUnitType.Auto);
                }
            }
        }

        /// <summary>
        /// 是否隐藏右侧幅度值面板
        /// </summary>
        public bool IsHideRightAmplitudePanel
        {
            get { return _isHideRightAmplitudePanel; }
            set
            {
                _isHideRightAmplitudePanel = value;
                xRightAmplitudePanel.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
                xContainer.ColumnDefinitions[2].Width = value ? new GridLength(0, GridUnitType.Auto) : new GridLength(63, GridUnitType.Pixel);
                if (IsHideLeftAmplitudePanel && !value)
                {
                    xContainer.RowDefinitions[0].Height = value
                                                              ? new GridLength(0, GridUnitType.Pixel)
                                                              : new GridLength(10, GridUnitType.Pixel);
                    xContainer.RowDefinitions[2].Height = value
                                                              ? new GridLength(0, GridUnitType.Pixel)
                                                              : new GridLength(0, GridUnitType.Auto);
                }
            }
        }

        private Brush _measureUnitForeground;
        public Brush MeasureUnitForeground
        {
            get { return _measureUnitForeground; }
            set
            {
                _measureUnitForeground = value;
                OnPropertyChanged("MeasureUnitForeground");
            }
        }

        private Brush _defaultLabelForeground;
        public Brush DefaultLabelForeground
        {
            get { return _defaultLabelForeground; }
            set
            {
                _defaultLabelForeground = value;
                x_scaleLinePrompt1.Foreground = _defaultLabelForeground;
                x_scaleLinePrompt2.Foreground = _defaultLabelForeground;
            }
        }

        #endregion

        #region INotifyPropertyChanged
        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion

        #region 公有
        public void InitSpectrumProperty(double? pUpperLimitValue, double? pLowerLimitValue, double? pBeginLeftValue, double? pEndRightValue)
        {
            if (pUpperLimitValue != null) x_spectrumDiagram.Property.UpperLimitValue = pUpperLimitValue.Value;
            if (pLowerLimitValue != null) x_spectrumDiagram.Property.LowerLimitValue = pLowerLimitValue.Value;
            if (pBeginLeftValue != null) x_spectrumDiagram.Property.BeginLeftValue = pBeginLeftValue.Value;
            if (pEndRightValue != null) x_spectrumDiagram.Property.EndRightValue = pEndRightValue.Value;
        }

        public void Initializers(double pStartFreq, double pStep, int pFreqPointCount)
        {
            _spectrumDataManage.InitializersCache(pStartFreq, pStep, pFreqPointCount);
        }

        public void DrawWaterfall(double pStartFreq, double pStep, double pSecStartFreq, double pSecStep, short[] pValues, DateTime? pTime = null)
        {
            if (_spectrumDataManage.FreqPointCount == 0)
                return;
            _spectrumDataManage.UpdateSpectrumData(pStartFreq, pStep, pSecStartFreq, pSecStep, pValues);
            var values = _spectrumDataManage.GetDbuvData(x_spectrumDiagram.Property.BeginLeftValue, x_spectrumDiagram.Property.EndRightValue);
            x_spectrumDiagram.DrawWaterfall(values, pTime);
            x_scaleLinePrompt2.UpdateTimeFlowScales(x_spectrumDiagram.WaterfallTimes);
        }

        public void DrawWaterfall(short[] pValues, DateTime? pTime = null)
        {
            x_spectrumDiagram.DrawWaterfall(pValues, pTime);
            x_scaleLinePrompt2.UpdateTimeFlowScales(x_spectrumDiagram.WaterfallTimes);
        }

        public void Clear()
        {
            _spectrumDataManage.Clear();
            x_spectrumDiagram.UpdateSpectrumDiagram(true);
        }

        private readonly SpectrumBeforeSave _beforeSave = new SpectrumBeforeSave();
        public void BeginSave()
        {
            _beforeSave.MeasureUnitForeground = MeasureUnitForeground;
            _beforeSave.DefaultLabelForeground = DefaultLabelForeground;
            var blackBrush = new SolidColorBrush(Colors.Black);
            MeasureUnitForeground = blackBrush;
            DefaultLabelForeground = blackBrush;
            x_spectrumDiagram.Background = blackBrush;
        }

        public void EndSave()
        {
            MeasureUnitForeground = _beforeSave.MeasureUnitForeground;
            DefaultLabelForeground = _beforeSave.DefaultLabelForeground;
            x_spectrumDiagram.Background = new SolidColorBrush(Colors.Transparent);
        }

        #endregion

        #region IDisposable
        private bool _disposed;
        private bool _isHideLeftAmplitudePanel;
        private bool _isHideRightAmplitudePanel;

        ~WaterfallContainer()
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
                }
                _disposed = true;
            }
        }
        #endregion
    }

    internal class WaterfallTimeCommand : ICommand
    {
        private readonly WaterfallContainer _waterfallContainer;
        private const int Span = 5;
        private const int Range = 60;

        public WaterfallTimeCommand(WaterfallContainer pWaterfallContainer)
        {
            _waterfallContainer = pWaterfallContainer;
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
                case "up":
                    if (_waterfallContainer.StartSec > 0)
                    {
                        _waterfallContainer.StartSec -= Span;
                        _waterfallContainer.StopSec -= Span;
                    }
                    break;
                case "down":
                    if (_waterfallContainer.StopSec < Range)
                    {
                        _waterfallContainer.StopSec += Span;
                        _waterfallContainer.StartSec += Span;
                    }
                    break;
            }
            _waterfallContainer.UpdateScaleLinePrompt();
            _waterfallContainer.UpdateWaterfall();
        }
    }
}
