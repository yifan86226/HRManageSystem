
#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：加载台站信息 聚合显示
 * 日 期 ：2016-12-29
 ***************************************************************#@#***************************************************************/
#endregion
using CO_IA.Data.Setting;
using CO_IA.UI.MAP;
using CO_IA.UI.Screen.Dialog;
using CO_IA.UI.StationManage;
using GS_MapBase;
using I_GS_MapBase.Portal;
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO_IA.UI.Screen
{
    public class LoadStation
    {
        #region 已设台站
        //public MapLayer haveStationLayer = null;
        #endregion

        FrameworkElement Element;
        private Point _fixedPoint=new Point(0,0);

        private bool doit = false;
        public LoadStation(FrameworkElement element)
        {
            Element = element;
            if (!Obj.screenMap.initialized)
                return;
            Obj.screenMap.MainMap.MapExtentChanged += MainMap_MapExtentChanged;
            Obj.screenMap.HaveStationLayer.OnGraphicsMouseRightButtonUp += HaveStationLayer_OnGraphicsMouseRightButtonUp;
        }

        
        public void unLoad()
        {
            Obj.screenMap.MainMap.MapExtentChanged -= MainMap_MapExtentChanged;
            Obj.screenMap.HaveStationLayer.OnGraphicsMouseLeftButtonUp -= HaveStationLayer_OnGraphicsMouseRightButtonUp;
        }
        /// <summary>
        /// 台站显示、清除
        /// </summary>
        /// <param name="b"></param>
        public void SetDoIt(bool b)
        {
            if (b)
            {
                doit = true;
                //haveStationLayer = Obj.screenMap.MainMap.CreateMapLayer();
                Obj.screenMap.ClearHaveStation();
                LoadJH(Obj.screenMap.MainMap.CurrentMapExtent);
            }
            else
            {
                doit = false;
                Obj.screenMap.ClearHaveStation();

            }
        }

        void MainMap_MapExtentChanged(I_GS_MapBase.Portal.Types.MapExtentEventArgs pArgs)
        {            
            if(doit)
            {
                MapExtent extent = Obj.screenMap.MainMap.MapExtentFactory.Create(Obj.screenMap.MainMap.MapPointFactory.Create(pArgs.NewExtent.Xy1.X - 0.005, pArgs.NewExtent.Xy1.Y + 0.0027),
                    Obj.screenMap.MainMap.MapPointFactory.Create(pArgs.NewExtent.Xy2.X, pArgs.NewExtent.Xy2.Y));
                LoadJH(extent);
            }
        }
        private void LoadJH(MapExtent extent)
        {
            Obj.screenMap.ClearHaveStation();
            if (_fixedPoint.X == 0)
            {
                _fixedPoint.X = extent.Xy1.X;
                _fixedPoint.Y = extent.Xy1.Y;
            }
            List<double[]> list = getJHParam(new double[] { extent.Xy1.X, extent.Xy1.Y }, new double[] { extent.Xy2.X, extent.Xy2.Y }, 50, new double[] { _fixedPoint.X, _fixedPoint.Y });

            JHCondition jhCon = new JHCondition() { Range = list[0], xInterval = list[1][0], yInterval = list[1][1], CC = list[2][0], LotdOffset = list[2][1] };

            List<JHStation> stationList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Display.I_CO_IA_Display, List<JHStation>>(channel =>
            {
                return channel.GetStationToJH(jhCon);
            });

            if (stationList != null && stationList.Count > 0)
            {
                
                for (int i = 0; i < stationList.Count; i++)
                {
                    double x = 0;
                    double y = 0;
                    if (double.TryParse(stationList[i].STAT_LG, out x) && double.TryParse(stationList[i].STAT_LA, out y))
                    {
                        //if (x != 0 && y != 0)
                        //{
                        //    double[] p = I_GS_MapBase.Portal.CoordOffset.transform(x, y);//进行坐标校正
                        //    x = p[1];
                        //    y = p[0];
                        //}
                    }
                    if (x == 0 || y == 0)
                    {
                        continue;
                    }
                    MapPointEx point = Obj.screenMap.MainMap.MapPointFactory.Create(x,y);
                    int statCount = Convert.ToInt32(stationList[i].CO);
                    if (statCount == 1)
                    {
                        Obj.screenMap.DrawStationPoint(point, true,stationList[i],Obj.screenMap.HaveStationLayer);                        
                    }
                    else
                    {
                        double size = 0;
                        if (statCount < 10)
                        {
                            size = 22;
                        }
                        else if (statCount < 50)
                        {
                            size = 24;
                        }
                        else if (statCount < 100)
                        {
                            size = 27;
                        }
                        else if (statCount < 200)
                        {
                            size = 30;
                        }
                        else if (statCount < 1000)
                        {
                            size = 33;
                        }
                        else
                        {
                            size = 37;
                        }
                        Obj.screenMap.DrawPoint(point, new I_GS_MapBase.Portal.SymbolElement("stationCluser") { 
                             ControlTemplate = Obj.screenMap.MainMap.Resources["ClustererSymbol"] as ControlTemplate,
                              DataSources = new List<KeyValuePair<string, object>> { 
                                new KeyValuePair<string,object>("Count",statCount.ToString()),
                                new KeyValuePair<string,object>("Size",size),
                                new KeyValuePair<string, object>("Color",ClustererInterpolateColor(size - 12, 100)),
                                new KeyValuePair<string, object>("Range",new string[]{stationList[i].XMin,stationList[i].YMax,stationList[i].XMax,stationList[i].YMin}),
                            }
                        },Obj.screenMap.HaveStationLayer);
                    }
                }                
            }
        }
        private System.Windows.Media.Brush ClustererInterpolateColor(double value, double max)
        {
            value = (int)Math.Round(value * 255.0 / max);
            if (value > 255)
                value = 255;
            else if (value < 0)
                value = 0;
            return new SolidColorBrush(Color.FromArgb(127, (byte)value, 127, 0));
        }
        void HaveStationLayer_OnGraphicsMouseRightButtonUp(object sender, I_GS_MapBase.Portal.GraphicEventArgs e)
        {
            if (e.QueryBehavior == QueryBehavior.Point)
            {
                string ElementId = e.DC["ElementId"] == null ? null : e.DC["ElementId"].ToString();
                if (ElementId == "stationCluser")
                {
                    Ellipse ellipse = sender as Ellipse;
                    if (ellipse != null)
                    {
                        if (ellipse.ContextMenu == null)
                        {
                            ellipse.ContextMenu = GetStationCluserContextMenu(e.DC["Range"]);

                        }
                    }
                    return;
                }
                    

                Image p = sender as Image;
                if (p == null)
                    return;
                if (p.ContextMenu == null)
                {
                    p.ContextMenu = GetStationContextMenu(ElementId);

                }
                return;

                
            }
        }
        public  ContextMenu GetStationContextMenu(string stationId)
        {
            ContextMenu menu = new ContextMenu();
            MenuItem item = null;
            item = new MenuItem();
            item.Header = "详细";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/detail2.png", UriKind.RelativeOrAbsolute)) };
            item.Tag = stationId;
            item.Click += item_Click;
            menu.Items.Add(item);

            return menu;
        }

        void item_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item.Tag != null && !String.IsNullOrEmpty(item.Tag.ToString()))
            {
                StationDetailDialog dialog = new StationDetailDialog(item.Tag.ToString());
                dialog.ShowDialog(Element);
            }
        }

        public ContextMenu GetStationCluserContextMenu(object obj)
        {
            ContextMenu menu = new ContextMenu();
            MenuItem item = null;
            item = new MenuItem();
            item.Header = "查看列表";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/detail1.png", UriKind.RelativeOrAbsolute)) };
            item.Tag = obj;
            item.ToolTip = "最多显示10条记录";
            item.Click += itemCluser_Click;
            menu.Items.Add(item);

            return menu;
        }
        void itemCluser_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item.Tag != null )
            {
                string[] range = item.Tag as string[];
                if (range != null && range.Length == 4)
                {
                    StationListDialog stationlist = new StationListDialog(range);
                    stationlist.ShowDialog(Element);
                }
            }
        }






        public double[] FixedPoint = null;
        /// <summary>
        /// 根据屏幕范围获得网格信息
        /// </summary>
        /// <param name="p_leftTopPoint"></param>
        /// <param name="p_rightBottomPoint"></param>
        /// <param name="p_intervalValue"></param>
        /// <param name="p_fixedPoint"></param>
        /// <returns>返回大网格范围，和经纬度的网格步长，存储格式double[4],double[2]
        public List<double[]> getJHParam(double[] p_leftTopPoint, double[] p_rightBottomPoint, double p_intervalValue, double[] p_fixedPoint = null)
        {
            if (p_fixedPoint != null)
            {
                FixedPoint = new double[] { p_fixedPoint[0], p_fixedPoint[1] };
            }
            if (FixedPoint == null)
            {
                FixedPoint = new double[] { p_leftTopPoint[0], p_leftTopPoint[1] };
            }
            PointEx pBase = Obj.screenMap.MainMap.MapToScreen(Obj.screenMap.MainMap.MapPointFactory.Create(FixedPoint[0], FixedPoint[1]));

            double xInterval = 0, yInterval = 0;

            PointEx pBase1 = new PointEx(pBase.X + p_intervalValue, pBase.Y);
            MapPointEx p1 = Obj.screenMap.MainMap.ScreenToMap(pBase1);
            xInterval = Math.Abs(p1.X - FixedPoint[0]);

            PointEx pBase2 = new PointEx(pBase.X, pBase.Y + p_intervalValue);
            MapPointEx p2 = Obj.screenMap.MainMap.ScreenToMap(pBase2);
            yInterval = Math.Abs(p2.Y - FixedPoint[1]);

            double x1 = FixedPoint[0];
            if (x1 > p_leftTopPoint[0])
            {
                while (x1 > p_leftTopPoint[0])
                {
                    x1 -= xInterval;
                }
            }
            else
            {
                while (x1 < p_leftTopPoint[0])
                {
                    x1 += xInterval;
                }
            }
            int cc = 0;
            double y1 = FixedPoint[1];
            if (y1 > p_leftTopPoint[1])
            {
                while (y1 > p_leftTopPoint[1])
                {
                    y1 -= yInterval;

                    cc++;
                    if (cc == 4)
                        cc = 0;
                }
            }
            else
            {
                cc = 4;
                while (y1 < p_leftTopPoint[1])
                {
                    y1 += yInterval;
                    cc--;
                    if (cc < 0)
                        cc = 4;
                }
            }
            if (cc == 4)
                cc = 0;

            double x2 = FixedPoint[0];
            if (x2 > p_rightBottomPoint[0])
            {
                while (x2 > p_rightBottomPoint[0])
                {
                    x2 -= xInterval;
                }
            }
            else
            {
                while (x2 < p_rightBottomPoint[0])
                {
                    x2 += xInterval;
                }
            }
            double loOffset = Math.Abs(((FixedPoint[0] - x2) % (xInterval / 3)) * (xInterval / 3));


            double y2 = FixedPoint[1];
            if (y2 > p_rightBottomPoint[1])
            {
                while (y2 > p_rightBottomPoint[1])
                {
                    y2 -= yInterval;
                }
            }
            else
            {
                while (y2 < p_rightBottomPoint[1])
                {
                    y2 += yInterval;
                }
            }
            List<double[]> lst = new List<double[]>();

            double[] xy2 = new double[4] { x1, y1, x2, y2 };
            lst.Add(xy2);//存储范围坐标

            double[] wh = new double[2] { xInterval, yInterval };
            lst.Add(wh);//存储经纬度步长，单位度

            double[] firstOffsetIndex = new double[] { cc, loOffset };
            lst.Add(firstOffsetIndex);
            return lst;
        }

    }
    
}
