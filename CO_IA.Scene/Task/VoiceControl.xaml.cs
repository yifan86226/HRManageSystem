using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CO_IA.Scene.Task
{
    /// <summary>
    /// VoiceControl.xaml 的交互逻辑
    /// </summary>
    public partial class VoiceControl : UserControl
    {
        private MediaPlayer _mediaPlayer = new MediaPlayer();
        
        private DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private bool _isPlay = false;
        public VoiceControl()
        {
            InitializeComponent();
           
        }
        void Play()
        {
            _mediaPlayer.Open(new Uri("D:\\NewSVN\\重大活动保障1.0\\CO_IA.Scene\\Task\\Voices\\nhd.mp3", UriKind.Absolute));   
            Duration duration = _mediaPlayer.NaturalDuration;
            if (duration.HasTimeSpan)
                xTimeLength.Text = string.Format("{0}′{1}″", duration.TimeSpan.Hours * 60 + duration.TimeSpan.Minutes, duration.TimeSpan.Seconds);
            _mediaPlayer.Volume = 1;
            _mediaPlayer.Play();
            _isPlay = true;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;
            _dispatcherTimer.Start();
        }

        void _dispatcherTimer_Tick(object sender, EventArgs e)
        {
            string imagePath = xImage.Source.ToString();
            BitmapImage bit = new BitmapImage();
            if(imagePath.Contains("v1.png"))
            {
                xImage.Source = new BitmapImage(new Uri("/CO_IA.Scene;component/Task/Voices/v2.png", UriKind.RelativeOrAbsolute));
            }
            else if (imagePath.Contains("v2.png"))
            {
                xImage.Source = new BitmapImage(new Uri("/CO_IA.Scene;component/Task/Voices/v3.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                xImage.Source = new BitmapImage(new Uri("/CO_IA.Scene;component/Task/Voices/v1.png", UriKind.RelativeOrAbsolute));
            }
        }

        void Stop()
        {
            _mediaPlayer.Stop();
            _isPlay = false;
            _dispatcherTimer.Stop();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(_isPlay == false)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }
   

}
