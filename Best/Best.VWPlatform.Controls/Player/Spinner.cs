using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;

namespace Best.VWPlatform.Controls.Player
{
    [ContentProperty("Child")]
    [TemplatePart(Name = AnimateElement, Type = typeof(Storyboard))]
    public class Spinner : Control
    {
        internal const string AnimateElement = "Animate";
        private Storyboard _animate = new Storyboard();
        public Storyboard Animate
        {
            get { return _animate; }
            set { _animate = value; }
        }

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(Spinner), new PropertyMetadata(default(bool), new PropertyChangedCallback(OnIsBusyChanged)));

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        private static void OnIsBusyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Spinner s = sender as Spinner;
            s.SetBusy(e.NewValue);
        }

        public void SetBusy(object isBusy)
        {
            if (_animate != null)
                if ((bool)isBusy)
                {
                    this.Visibility = Visibility.Visible;
                    _animate.Begin();
                }
                else
                {
                    this.Visibility = Visibility.Collapsed;
                    _animate.Stop();
                }
        }

        static Spinner()
        {
            //base.DefaultStyleKey = typeof(Spinner);
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(typeof(Spinner)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Animate = base.GetTemplateChild(AnimateElement) as Storyboard;

            if (IsBusy)
            {
                this.Visibility = Visibility.Visible;
                _animate.Begin();
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
                _animate.Stop();
            }
        }
    }
}
