#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：绘制工具栏
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion
using I_GS_MapBase.Portal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace CO_IA.UI.MAP
{
    /// <summary>
    /// Bar.xaml 的交互逻辑
    /// </summary>
    public partial class Bar : UserControl
    {
        MapGIS mapGis = null;
        RadioButton checkedRadioButton = null;
        int Index = 0;

        public Action<bool> GraphicChange;
        public Bar(MapGIS _mapgis)
        {
            InitializeComponent();
            mapGis = _mapgis;

            List<PenSize> Sizelist = new List<PenSize>();
            
            //添加画笔大小
            Sizelist.Add(new PenSize() { size = 1,desc="1px" });
            Sizelist.Add(new PenSize() { size = 2, desc = "2px" });
            Sizelist.Add(new PenSize() { size = 3, desc = "3px" });
            Sizelist.Add(new PenSize() { size = 4, desc = "4px" });
            Sizelist.Add(new PenSize() { size = 5, desc = "5px" });
            Sizelist.Add(new PenSize() { size = 6, desc = "6px" });
            Sizelist.Add(new PenSize() { size = 7, desc = "7px" });
            Sizelist.Add(new PenSize() { size = 8, desc = "8px" });
            Sizelist.Add(new PenSize() { size = 9, desc = "9px" });
            Sizelist.Add(new PenSize() { size = 10, desc = "10px" });
            Sizelist.Add(new PenSize() { size = 11, desc = "11px" });
            Sizelist.Add(new PenSize() { size = 12, desc = "12px" });
            Sizelist.Add(new PenSize() { size = 13, desc = "13px" });
            Sizelist.Add(new PenSize() { size = 14, desc = "14px" });
            Sizelist.Add(new PenSize() { size = 15, desc = "15px" });
            cbx.ItemsSource = Sizelist;
            //cbx.DisplayMemberPath = "desc";
            //cbx.SelectedValuePath = "size";
            
            cbx.SelectedIndex = 0;

            mapGis.MapInitialized += MainMap_Initialized;
        }

        void MainMap_Initialized(bool obj)
        {
            mapGis.MainMap.DefaultLayer.QueryBehaviorBegin += DefaultLayer_QueryBehaviorBegin;
            mapGis.MainMap.DefaultLayer.QueryBehaviorCompleted += DefaultLayer_QueryBehaviorCompleted;

            mapGis.MainMap.DefaultLayer.OnGraphicsMouseLeftButtonUp += DefaultLayer_OnGraphicsMouseLeftButtonUp;

            mapGis.MainMap.MapMouseClick += MainMap_MapMouseClick;
        }

        void MainMap_MapMouseClick(I_GS_MapBase.Portal.Types.MapMouseEventArgs pArgs)
        {
            //可以添加文字
        }

        void DefaultLayer_OnGraphicsMouseLeftButtonUp(object sender, GraphicEventArgs e)
        {
            if (rbDel.IsChecked==true)
            {
                IDictionary<string, object> Dic = e.DC as IDictionary<string, object>;
                string ElementId = Dic["ElementId"] == null ? null : Dic["ElementId"].ToString();
                if (!string.IsNullOrEmpty(ElementId))
                {
                    if (MessageBox.Show("是否要删除该图形？", "询问", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        mapGis.RemoveSymbolElement(ElementId);
                        if (GraphicChange != null)
                        {
                            GraphicChange(true);
                        }
                    }
                }
            }
            rbDel.IsChecked = false;
        }

        void DefaultLayer_QueryBehaviorBegin(object sender, EventArgs e)
        {
            string Bordercolor = ColorPicker1.CurrentColor.ToString();
            string Fillcolor = ColorPicker2.CurrentColor.ToString();

            int size = 1;
            PenSize ps = cbx.SelectedItem as PenSize;
            if (ps != null)
                size = ps.size;

            mapGis.MainMap.BorderColorValue = Bordercolor;
            mapGis.MainMap.BorderWidth = size;
            mapGis.MainMap.FillColorValue = Fillcolor;
        }

        void DefaultLayer_QueryBehaviorCompleted(QueryBehaviorEventArgs pQueryBehaviorEventArgs)
        {
           
            if (pQueryBehaviorEventArgs == null)
                return;
            checkedRadioButton.IsChecked = false;

            

            string Bordercolor = ColorPicker1.CurrentColor.ToString();
            string Fillcolor = ColorPicker2.CurrentColor.ToString();
            
            int size = 1;
            PenSize ps = cbx.SelectedItem as PenSize;
            if (ps != null)
                size = ps.size;

            //MapFunPortal.mapPortal.setDrawBorderColor(Bordercolor);
            //MapFunPortal.mapPortal.setDrawBorderWidth(size);
            //MapFunPortal.mapPortal.setFillColor(Fillcolor);
            Index++;
            switch (pQueryBehaviorEventArgs.QueryBehavior)
            {
                case QueryBehavior.Circle:
                    var argsCircle = pQueryBehaviorEventArgs as CircleBehaviorEventArgs;
                    if (argsCircle != null)
                    {
                        mapGis.DrawCircle(argsCircle.CenterPoint, argsCircle.Radius, "Circle"+Index.ToString(), null, null, Bordercolor, Fillcolor, size);                       
                    }
                    break;

                case QueryBehavior.Rectangle:
                    var argsRectangle = pQueryBehaviorEventArgs as RectangleQueryBehaviorEventArgs;
                    if (argsRectangle != null)
                    {
                        mapGis.DrawRectangle(argsRectangle.LeftTopPoint, argsRectangle.Width, argsRectangle.Height, "Rectangle" + Index.ToString(), null, null, Bordercolor, Fillcolor, size);                        
                    }
                    break;

                case QueryBehavior.Polygon:
                    var argsPolygon = pQueryBehaviorEventArgs as PolygonQueryBehaviorEventArgs;
                    if (argsPolygon != null)
                    {
                        mapGis.DrawPolygon(argsPolygon.Points, "Polygon" + Index.ToString(), null, null, Bordercolor, Fillcolor, size);                        
                    }
                    break;
                case QueryBehavior.Ellipse:
                    var argsEllipse = pQueryBehaviorEventArgs as PolygonQueryBehaviorEventArgs;
                    if (argsEllipse != null)
                    {
                        mapGis.DrawPolygon(argsEllipse.Points, "Ellipse" + Index.ToString(), null, null, Bordercolor, Fillcolor, size);     
                    }
                    break;
                case QueryBehavior.Polyline:
                    var argsPolyline = pQueryBehaviorEventArgs as PolygonQueryBehaviorEventArgs;
                    if (argsPolyline != null)
                    {
                        mapGis.DrawPolyline(argsPolyline.Points, "Polyline" + Index.ToString(), null, null, Bordercolor, size);  
                    }
                    break;

                case QueryBehavior.Arrow:
                    var argsArrow = pQueryBehaviorEventArgs as PolygonQueryBehaviorEventArgs;
                    if (argsArrow != null)
                    {
                        mapGis.DrawPolygon(argsArrow.Points, "Arrow" + Index.ToString(), null, null, Bordercolor, Fillcolor, size);  
                    }
                    break;

                case QueryBehavior.Freehand:
                    var argsFreehand = pQueryBehaviorEventArgs as PolygonQueryBehaviorEventArgs;
                    if (argsFreehand != null)
                    {
                        mapGis.DrawPolyline(argsFreehand.Points, "Freehand" + Index.ToString(), null, null, Bordercolor, size);  
                    }
                    break;

                case QueryBehavior.LineSegment:
                    var argsLineSegment = pQueryBehaviorEventArgs as PolygonQueryBehaviorEventArgs;
                    if (argsLineSegment != null)
                    {
                        mapGis.DrawPolyline(argsLineSegment.Points, "LineSegment" + Index.ToString(), null, null, Bordercolor, size);  
                    }
                    break;

                case QueryBehavior.Triangle:
                    var argsTriangle = pQueryBehaviorEventArgs as PolygonQueryBehaviorEventArgs;
                    if (argsTriangle != null)
                    {
                        mapGis.DrawPolygon(argsTriangle.Points, "Triangle" + Index.ToString(), null, null, Bordercolor, Fillcolor, size);  
                    }
                    break;
            }
            if (GraphicChange != null)
            {
                GraphicChange(true);
            }
        }
        
        private void DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //Canvas.SetLeft(this, this.Margin.Left + e.HorizontalChange);
            //Canvas.SetTop(this, this.Margin.Top + e.VerticalChange);
            //if (this.Margin.Left < 0)
            //    return;
            //if (this.Margin.Left >= (MapFun.MainMap.ActualWidth))
            //    return;
            double left = this.Margin.Left + e.HorizontalChange;
            if (left < 0) left = 0;
            if (left > mapGis.MainMap.ActualWidth - 10)
                left = mapGis.MainMap.ActualWidth - 10;
            double top = this.Margin.Top + e.VerticalChange;
            if (top < 0)
                top = 0;
            if (top > mapGis.MainMap.ActualHeight - 10)
                top = mapGis.MainMap.ActualHeight - 10;
            this.Margin = new Thickness(left, top, 0, 0);
        }

        private void DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            thumb1.Background = Brushes.Red;
        }

        private void DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            thumb1.Background = Brushes.White;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            //先初始
            checkedRadioButton = sender as RadioButton;
            if (checkedRadioButton == null || string.IsNullOrEmpty(checkedRadioButton.Name))
                return;

            switch (checkedRadioButton.Name)
            {                
                case "rbCircle":
                case "rbEllipse":      
                case "rbRect":                    
                case "rbPolygon":                   
                case "rbPolyline":                   
                case "rbLine":                    
                case "rbPen":                    
                case "rbFont":
                    if (mapGis.DrawList.Count > 0) //只能绘制一个图形
                    {
                        if (MessageBox.Show("当前已经存在地点图形，点“是”覆盖，点“否”退出", "询问", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mapGis.RemoveSymbolElement();
                        }
                        else
                        {
                            checkedRadioButton.IsChecked = false;
                            return;
                        }

                    }
                    break;
                default:
                    break;
            }

            mapGis.Modify = true;//做了修改

            switch (checkedRadioButton.Name)
            {
                case "rbUnSelect":
                    mapGis.MainMap.DefaultLayer.GetQueryParameters(QueryBehavior.None);
                    break;
                case "rbDelAll":
                    mapGis.MainMap.DefaultLayer.GetQueryParameters(QueryBehavior.None);
                    mapGis.RemoveSymbolElement();
                    rbDelAll.IsChecked = false;
                    break;
                case "rbCircle":
                    mapGis.MainMap.DefaultLayer.GetQueryParameters(QueryBehavior.Circle);
                    break;
                case "rbEllipse":
                    mapGis.MainMap.DefaultLayer.GetQueryParameters(QueryBehavior.Ellipse);
                    break;
                case "rbRect":
                    mapGis.MainMap.DefaultLayer.GetQueryParameters(QueryBehavior.Rectangle);
                    break;
                case "rbPolygon":
                    mapGis.MainMap.DefaultLayer.GetQueryParameters(QueryBehavior.Polygon);
                    break;
                case "rbPolyline":
                    mapGis.MainMap.DefaultLayer.GetQueryParameters(QueryBehavior.Polyline);
                    break;
                case "rbLine":
                    mapGis.MainMap.DefaultLayer.GetQueryParameters(QueryBehavior.LineSegment);
                    break;
                case "rbPen":
                    mapGis.MainMap.DefaultLayer.GetQueryParameters(QueryBehavior.Freehand);
                    break;
                case "rbFont":
                    
                    break;
                default:
                    break;
            }
            if (GraphicChange != null)
            {
                GraphicChange(true);
            }
        }
        
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid grid = this.Parent as Grid;
            if (grid != null)
            {
                mapGis.MainMap.DefaultLayer.QueryBehaviorCompleted -= DefaultLayer_QueryBehaviorCompleted;
                grid.Children.Remove(this);
            }
        }

        private void cbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //PenSize psize = cbx.SelectedItem as PenSize;
            //if (psize != null)
            //{
            //    cbx.Text = psize.desc;
            //    }
        }
    }
    public class PenSize
    {
        public int size { set; get; }
        public string desc { set; get; }
    }

}
