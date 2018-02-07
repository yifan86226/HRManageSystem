using System;
using System.Collections.Generic;
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

namespace Best.VWPlatform.Controls.Common
{
    public class LineSeparate : Control
    {
        static LineSeparate()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LineSeparate), new FrameworkPropertyMetadata(typeof(LineSeparate)));
        }

        private Orientation lineOrientation;

        #region 属性

        public Orientation LineOrientation
        {
            get { return (Orientation)GetValue(LineOrientationProperty); }
            set { SetValue(LineOrientationProperty, value); }
        }

        public static readonly DependencyProperty LineOrientationProperty =
            DependencyProperty.Register("LineOrientation", typeof(Orientation), typeof(LineSeparate), new PropertyMetadata((d, e) =>
            {
                LineSeparate ls = d as LineSeparate;

                if (e.NewValue != null && ls != null)
                {
                    ls.lineOrientation = (Orientation)e.NewValue;

                    if (ls._gdHorizontal != null && ls._gdVertical != null)
                    {
                        if (ls.lineOrientation == Orientation.Horizontal)
                        {
                            ls._gdHorizontal.Visibility = Visibility.Visible;
                            ls._gdVertical.Visibility = Visibility.Collapsed;
                        }
                        if (ls.lineOrientation == Orientation.Vertical)
                        {
                            ls._gdHorizontal.Visibility = Visibility.Collapsed;
                            ls._gdVertical.Visibility = Visibility.Visible;
                        }
                    }
                }
            }));
        #endregion

        private Grid _gdHorizontal;
        private Grid _gdVertical;

        #region 重写

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _gdHorizontal = GetTemplateChild("gdHorizontal") as Grid;
            _gdVertical = GetTemplateChild("gdVertical") as Grid;

            if (_gdHorizontal != null && _gdVertical != null)
            {
                if (lineOrientation == Orientation.Horizontal)
                {
                    _gdHorizontal.Visibility = Visibility.Visible;
                    _gdVertical.Visibility = Visibility.Collapsed;
                }
                if (lineOrientation == Orientation.Vertical)
                {
                    _gdHorizontal.Visibility = Visibility.Collapsed;
                    _gdVertical.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion
    }
}
