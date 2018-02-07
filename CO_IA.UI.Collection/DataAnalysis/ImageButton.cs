using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CO_IA.UI.Collection.DataAnalysis
{
    public class ImageButton : Button
    {
        private Image _xIcon;
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon",
                                                                                             typeof(ImageSource),
                                                                                             typeof(ImageButton), new PropertyMetadata(OnIconPropertyChangedCallback));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation",
                                                                                                    typeof(Orientation),
                                                                                                    typeof(ImageButton),
                                                                                                    new PropertyMetadata(OnOrientationPropertyChangedCallback));

        private static void OnIconPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imgButton = d as ImageButton;
            if (imgButton == null)
                return;
            if (imgButton._xIcon != null)
                imgButton._xIcon.Visibility = imgButton.Icon != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void OnOrientationPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imgButton = d as ImageButton;
            if (imgButton == null)
                return;
            if (imgButton.Orientation == Orientation.Horizontal)
            {
                imgButton.HorizontalContentAlignment = HorizontalAlignment.Left;
                imgButton.VerticalContentAlignment = VerticalAlignment.Center;
            }
            else
            {
                imgButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                imgButton.VerticalContentAlignment = VerticalAlignment.Top;
            }
            imgButton.ChangeIconProperty(imgButton.Orientation);
        }

        public ImageButton()
        {
            DefaultStyleKey = typeof(ImageButton);
        }

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _xIcon = GetTemplateChild("xIcon") as Image;
            ChangeIconProperty(Orientation);
        }

        private void ChangeIconProperty(Orientation pOrientation)
        {
            if (_xIcon != null)
            {
                _xIcon.Visibility = Icon != null ? Visibility.Visible : Visibility.Collapsed;
                if (Content == null)
                {
                    _xIcon.Margin = new Thickness(0);
                }
                else
                {
                    if (pOrientation == Orientation.Horizontal)
                    {
                        _xIcon.Margin = new Thickness(0, 0, 5, 0);
                    }
                    else
                    {
                        _xIcon.Margin = new Thickness(0, 0, 0, 5);
                    }
                }
            }
        }
    }
}
