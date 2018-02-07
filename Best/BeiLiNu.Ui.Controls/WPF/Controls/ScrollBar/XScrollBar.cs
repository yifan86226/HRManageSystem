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

namespace BeiLiNu.Ui.Controls.WPF.Controls
{
    public class XScrollBar : System.Windows.Controls.Primitives.ScrollBar
    {
        static XScrollBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XScrollBar), new FrameworkPropertyMetadata(typeof(XScrollBar)));
        }
    }
}
