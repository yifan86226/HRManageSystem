﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CO_IA.UI.Setting.Converter
{
    public class DefaultImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return value;
            }
            else
            {
                return new BitmapImage(new Uri("/CO_IA.UI.Setting;component/Images/EventImg.png", UriKind.RelativeOrAbsolute));
                //byte[] imageData = File.ReadAllBytes("/CO_IA.UI.Setting;component/Images/EventImg.png");
                //return imageData;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
