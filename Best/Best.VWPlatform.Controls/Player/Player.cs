using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Threading;

namespace Best.VWPlatform.Controls.Player
{
    public delegate void EventHandler(Object sender, EventArgs e);
    public delegate void SoundEventHandler(Object sender, bool isMuted);

    [TemplatePart(Name = btnPlayElement, Type = typeof(ToggleButton))]
    [TemplatePart(Name = btnSpeakerElement, Type = typeof(ToggleButton))]
    [TemplatePart(Name = btnFullScreenElement, Type = typeof(ToggleButton))]
    [TemplatePart(Name = tbCurrentTimeElement, Type = typeof(TextBlock))]
    [TemplatePart(Name = tbTotalTimeElement, Type = typeof(TextBlock))]
    [TemplatePart(Name = sliderTimelineElement, Type = typeof(CustomSlider))]
    [TemplatePart(Name = sliderVolumeElement, Type = typeof(CustomSlider))]
    [TemplatePart(Name = spinner, Type = typeof(Spinner))]
    [TemplatePart(Name = gridCol2Element, Type = typeof(Grid))]

    [StyleTypedProperty(Property = "MarkerStyle", StyleTargetType = typeof(Marker))]
    public class Player : Control
    {
        public delegate TimeSpan TimeLine(TimeSpan position);

        //private DispatcherTimer _playTimer = new DispatcherTimer();
        //private TimeSpan _duration;

        //private Marker _activeMarker;
        //private Dictionary<TimeSpan, Marker> _markers = new Dictionary<TimeSpan, Marker>();
        //private Dictionary<TimeSpan, Marker> _cCs = new Dictionary<TimeSpan, Marker>();
        //private List<MarkerData> _externalMarkerData = new List<MarkerData>();

        #region Events / Delegates

        public event EventHandler PlayClicked;
        public event SoundEventHandler SoundChanged;
        public event EventHandler PauseClicked;
        public event EventHandler StopClicked;
        public event EventHandler FullScreenClicked;
        public event EventHandler BufferingStart;
        public event EventHandler BufferingEnd;
        public event EventHandler MediaCompleted;
        //public event MarkerEventHandler MarkerReached;

        #endregion

        //private MediaElement _media = null;
        private bool _buffering = false;
        private bool _timeRemaining = false;
        private double _lastVolume = 0;
        private bool _seekable = true;

        #region Constants

        internal const string btnPlayElement = "btnPlay";
        internal const string btnSpeakerElement = "btnSpeaker";
        internal const string btnFullScreenElement = "btnFullScreen";
        internal const string tbCurrentTimeElement = "tbCurrentTime";
        internal const string tbTotalTimeElement = "tbTotalTime";
        internal const string sliderTimelineElement = "sliderTimeline";
        internal const string sliderVolumeElement = "sliderVolume";
        internal const string gridCol2Element = "gridCol2";
        internal const string spinner = "spinner";

        #endregion

        public static readonly DependencyProperty PlayerVmProperty =
            DependencyProperty.Register("PlayerVm", typeof(PlayerVm), typeof(Player), new PropertyMetadata(OnPlayerVmChanged));

        public PlayerVm PlayerVm
        {
            get { return (PlayerVm)GetValue(PlayerVmProperty); }
            set { SetValue(PlayerVmProperty, value); }
        }
        private static void OnPlayerVmChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Player ctrl = (Player)d;
            ctrl.PlayerVmChanged(e.OldValue as PlayerVm, e.NewValue as PlayerVm);
        }

        public void PlayerVmChanged(PlayerVm oldValue, PlayerVm newValue)
        {
            this.DataContext = newValue;
        }

        #region TemplateParts

        //private ToggleButton _btnPlay;

        //public ToggleButton btnPlay
        //{
        //    get { return _btnPlay; }
        //    set { _btnPlay = value; }
        //}

        //private ToggleButton _btnSpeaker;
        //public ToggleButton btnSpeaker
        //{
        //    get { return _btnSpeaker; }
        //    set { _btnSpeaker = value; }
        //}
        //private ToggleButton _btnFullScreen;
        //public ToggleButton btnFullScreen
        //{
        //    get { return _btnFullScreen; }
        //    set { _btnFullScreen = value; }
        //}
        //private TextBlock _tbCurrentTime;

        //public TextBlock tbCurrentTime
        //{
        //    get { return _tbCurrentTime; }
        //    set { _tbCurrentTime = value; }
        //}

        //private TextBlock _tbTotalTime;

        //public TextBlock tbTotalTime
        //{
        //    get { return _tbTotalTime; }
        //    set { _tbTotalTime = value; }
        //}

        //private CustomSlider _sliderTimeline;

        //public CustomSlider sliderTimeline
        //{
        //    get { return _sliderTimeline; }
        //    set { _sliderTimeline = value; }
        //}

        //private Slider _sliderVolume;

        //public Slider sliderVolume
        //{
        //    get { return _sliderVolume; }
        //    set { _sliderVolume = value; }
        //}

        private Spinner _spinner;

        public Spinner Spinner
        {
            get { return _spinner; }
            set { _spinner = value; }
        }

        //private Grid _gridCol2;

        //public Grid GridCol2
        //{
        //    get { return _gridCol2; }
        //    set { _gridCol2 = value; }
        //}

        #endregion



        static Player()
        {
            //base.DefaultStyleKey = typeof(Player);
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Player), new FrameworkPropertyMetadata(typeof(Player)));

        }

        public Player()
        {
            this.Loaded += new RoutedEventHandler(Controls_Loaded);
        }

        private void Controls_Loaded(object sender, RoutedEventArgs e)
        {
            this.ApplyTemplate();
        }

        private CustomSlider _slider;

        private ToggleButton _btnPlay;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _slider = base.GetTemplateChild(sliderTimelineElement) as CustomSlider;

            _btnPlay = base.GetTemplateChild(btnPlayElement) as ToggleButton;

            ////this.btnSpeaker = base.GetTemplateChild(btnSpeakerElement) as ToggleButton;
            ////this.btnFullScreen = base.GetTemplateChild(btnFullScreenElement) as ToggleButton;
            //this.tbCurrentTime = base.GetTemplateChild(tbCurrentTimeElement) as TextBlock;
            //this.tbTotalTime = base.GetTemplateChild(tbTotalTimeElement) as TextBlock;
            //this.sliderTimeline = base.GetTemplateChild(sliderTimelineElement) as CustomSlider;
            ////this.sliderVolume = base.GetTemplateChild(sliderVolumeElement) as Slider;
            //this.GridCol2 = base.GetTemplateChild(gridCol2Element) as Grid;
            _spinner = base.GetTemplateChild(spinner) as Spinner;
            ////_spinner.Visibility = Visibility.Collapsed;
            _spinner.Animate.Begin();

            //sliderTimeline.IsHitTestVisible = _seekable;
            //var vm = new PlayerVm() { CurrTime = 10000, TotalTime = 5000 };
            //vm.NextCommand.ExecuteAction = new Action<object>(test);
            //vm.PreviousCommand.ExecuteAction = new Action<object>(test);
            //vm.PlayAndPauseCommand.ExecuteAction = new Action<object>(test);
            //vm.ValueChangedCommand.ExecuteAction = new Action<object>(test);

            if (PlayerVm != null)
                this.DataContext = PlayerVm;
        }

        public void PlayingAt(int pV)
        {
            _slider.MovingSliderThumb(pV);
        }

        public void ResetPlayer()
        {
            _btnPlay.IsChecked = false;
            _slider.MovingSliderThumb(0);
        }

        public void test(object o)
        {
        }
    }
}
