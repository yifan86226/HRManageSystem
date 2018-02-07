using CO_IA.Client;
using CO_IA.UI.MAP;
using I_GS_MapBase.Portal;
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
    /// AreaTips.xaml 的交互逻辑
    /// </summary>
    public partial class AreaTips : UserControl, IFrameworkElement
    {
        public AreaTips(AreaTipsData tipsData)
        {
            InitializeComponent();
            this.Loaded += AreaTips_Loaded;
            this.DataContext = tipsData;
            ElementId = MapGroupTypes.SiteTipsPoint_.ToString();
        }

        void AreaTips_Loaded(object sender, RoutedEventArgs e)
        {
            progressGrid.Visibility = System.Windows.Visibility.Collapsed;
            //if (Obj.Activity.ActivityStage == Types.ActivityStage.Prepare)
            //    progressGrid.Visibility = System.Windows.Visibility.Visible;
            //else
            //    progressGrid.Visibility = System.Windows.Visibility.Collapsed;
        }
        public string ElementId
        {
            get;
            set;
        }
        public object ElementTag
        {
            get;
            set;
        }
    }
    public class ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return value.ToString() + "%";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
