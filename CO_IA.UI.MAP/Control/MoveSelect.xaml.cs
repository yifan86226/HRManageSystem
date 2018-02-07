using I_GS_MapBase.Portal;
using I_GS_MapBase.Portal.Types;
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

namespace CO_IA.UI.MAP.Control
{
    /// <summary>
    /// MoveSelect.xaml 的交互逻辑
    /// </summary>
    public partial class MoveSelect : UserControl, IFrameworkElement
    {
        public Action<MapPointEx> OnLocationMove;
        private MapGIS _mapGis;
        public MoveSelect(MapGIS mapgis)
        {
            InitializeComponent();
            _mapGis = mapgis;
        }
        private void DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double left = this.Margin.Left + e.HorizontalChange;
            if (left < 0) left = 0;
            if (left > _mapGis.MainMap.ActualWidth - 10)
                left = _mapGis.MainMap.ActualWidth - 10;
            double top = this.Margin.Top + e.VerticalChange;
            if (top < 0)
                top = 0;
            if (top > _mapGis.MainMap.ActualHeight - 10)
                top = _mapGis.MainMap.ActualHeight - 10;
            this.Margin = new Thickness(left, top, 0, 0);

            MapPointEx point = _mapGis.MainMap.ScreenToMap(new PointEx(left,top));
            if (OnLocationMove != null)
                OnLocationMove(point);
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
}
