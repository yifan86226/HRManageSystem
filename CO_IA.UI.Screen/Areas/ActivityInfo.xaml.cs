using CO_IA.Client;
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

namespace CO_IA.UI.Screen.Areas
{
    /// <summary>
    /// ActivityInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityInfo : UserControl
    {
        public ActivityInfo()
        {
            InitializeComponent();
           
        }
        public void IniData()
        {
            this.DataContext = Obj.Activity;
            this.txtType.Text = Utility.GetActivityTypeName(Obj.Activity.ActivityType);
        }
    }
     public class DateTimeConverter : IValueConverter
     {
         public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         {
             if (value is DateTime)
             {
                 DateTime dt = (DateTime)value;
                 return dt.ToString("yyyy年MM月dd日");
             }
             return value;
         }

         public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         {
             throw new NotImplementedException();
         }
     }
}
