using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BeiLiNu.Ui.Controls.WPF.Controls
{
    public class AnimationImage : Image
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private int count = 1;

        //播放间隔
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(double), typeof(AnimationImage), new FrameworkPropertyMetadata(300d, new PropertyChangedCallback(OnTimeChanged)));
        public double Time
        {
            get { return (double)this.GetValue(TimeProperty); }
            set { this.SetValue(TimeProperty, value); }
        }
        private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimationImage obj = (AnimationImage)d;
            if (obj.Time != 300)
            {
                obj.dispatcherTimer.Interval = TimeSpan.FromMilliseconds(obj.Time);
            }
        }
        //图片的张数
        public static readonly DependencyProperty ImageCountProperty = DependencyProperty.Register("ImageCount", typeof(int), typeof(AnimationImage), new PropertyMetadata());
        public int ImageCount
        {
            get { return (int)this.GetValue(ImageCountProperty); }
            set { this.SetValue(ImageCountProperty, value); }
        }
        //图片文件夹路径

        public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register("ImagePath", typeof(string), typeof(AnimationImage), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnImagePathChanged)));
        public string ImagePath
        {
            get { return (string)this.GetValue(ImagePathProperty); }
            set { this.SetValue(ImagePathProperty, value); }
        }
        private static void OnImagePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimationImage obj = (AnimationImage)d;
            if (obj.ImagePath!= null)
            {
                BitmapImage _BitmapImage = new BitmapImage();
                _BitmapImage.BeginInit();
                _BitmapImage.UriSource = new Uri(obj.ImagePath + "1.png", UriKind.Relative);
                obj.Source = _BitmapImage;
                _BitmapImage.EndInit();
            }
        }
        public AnimationImage()
        {
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Time);

        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            BitmapImage _BitmapImage = new BitmapImage();
            _BitmapImage.BeginInit();
            _BitmapImage.UriSource = new Uri(ImagePath + count + ".png", UriKind.Relative);
            this.Source = _BitmapImage;
            _BitmapImage.EndInit();
            count = count == ImageCount ? 1 : count + 1;//如果count==ImageCount即已经到了最后一帧，那么count回到第一帧即count=1;否则count+=1
        }

        public void Start()
        {
            if (dispatcherTimer.IsEnabled == false)
            {
                dispatcherTimer.Start();
            }
        }
        public void Stop()
        {
            if (dispatcherTimer.IsEnabled == true)
            {
                dispatcherTimer.Stop();
            }
        }
    }
}
