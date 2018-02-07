using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CO_IA.Scene
{
    public class AppImageRadioButton : RadioButton
    {
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(AppImageRadioButton),
            null);

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource",
            typeof(ImageSource),
            typeof(AppImageRadioButton),
            null);

        public AppImageRadioButton()
        {
            this.DefaultStyleKey = typeof(AppImageRadioButton);
        }

        public string Header
        {
            get
            {
                return (string)this.GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)this.GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }
    }
}
