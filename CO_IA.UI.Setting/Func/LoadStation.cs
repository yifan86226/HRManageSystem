
#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：加载台站信息 聚合显示
 * 日 期 ：2016-12-29
 ***************************************************************#@#***************************************************************/
#endregion
using CO_IA.Data.Setting;
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CO_IA.UI.Screen
{
    public class LoadStation
    {
        private double _mapOldScale;
        private Point _fixedPoint=new Point(0,0);

        private bool doit = false;
        //private double _colStep=0;
        //private double _rowStep=0;
        public LoadStation()
        {
            //if (!Utils.MapFun.initialized)
            //    return;
            Utils.MapFun.MainMap.MapExtentChanged += MainMap_MapExtentChanged;
        }
        public void SetDoIt(bool b)
        {
            if (b)
            {
                doit = true;
                LoadJH(Utils.MapFun.MainMap.CurrentMapExtent);
            }
            else
            {
                doit = false;
                Utils.MapFun.haveStationLayer.ClearSymbolElements();
            }
        }
        void MainMap_MapExtentChanged(I_GS_MapBase.Portal.Types.MapExtentEventArgs pArgs)
        {            
            if(doit)
            {
                MapExtent extent = Utils.MapFun.MainMap.MapExtentFactory.Create(Utils.MapFun.MainMap.MapPointFactory.Create(pArgs.NewExtent.Xy1.X-0.005, pArgs.NewExtent.Xy1.Y+0.0027), 
                    Utils.MapFun.MainMap.MapPointFactory.Create(pArgs.NewExtent.Xy2.X,pArgs.NewExtent.Xy2.Y));
                LoadJH(extent);
            }
        }
        private void LoadJH(MapExtent extent)
        {
            Utils.MapFun.haveStationLayer.ClearSymbolElements();
            

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
                string imgUrl = "/CO_IA.UI.Display;component/Images/station/GIS_SHF4.png";
                for (int i = 0; i < stationList.Count; i++)
                {                    
                    string id = Utils.stationGroupFlag + stationList[i].GUID;
                    Utils.MapFun.RemoveSymbolElement(id);

                    double x = 0;
                    double y = 0;
                    if (double.TryParse(stationList[i].STAT_LG, out x) && double.TryParse(stationList[i].STAT_LA, out y))
                    {
                        if (x != 0 && y != 0)
                        {
                            double[] p = I_GS_MapBase.Portal.CoordOffset.transform(x, y);//进行坐标校正
                            x = p[1];
                            y = p[0];
                        }
                    }
                    if (x == 0 || y == 0)
                    {
                        continue;
                    }

                    int statCount = Convert.ToInt32(stationList[i].CO);
                    if (statCount == 1)
                    {
                        //重写
                        //Utils.MapFun.DrawPointToStation(x, y, null, id, new List<KeyValuePair<string, object>> { 
                        //        new KeyValuePair<string,object>(GraphicStyle.ImageSource.ToString(), "/CO_IA.UI.Display;component/Images/station/map2.png"),
                        //        new KeyValuePair<string,object>("ImageSource2",imgUrl),
                        //        new KeyValuePair<string,object>(GraphicStyle.zIndex.ToString(),Utils.z_index.ToString()),
                        //        new KeyValuePair<string, object>(GraphicStyle.ToolTipText.ToString(),stationList[i].NAME),
                        //        new KeyValuePair<string, object>("data",stationList[i]),
                        //    }, "StationTemplete");
                    }
                    else
                    {
                        //continue;
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
                        //重写
                        //Utils.MapFun.DrawPointToStation(x, y, null, "jh_"+id, new List<KeyValuePair<string, object>> { 
                        //        new KeyValuePair<string,object>("Count",statCount.ToString()),
                        //        new KeyValuePair<string,object>("Size",size),
                        //        new KeyValuePair<string,object>(GraphicStyle.zIndex.ToString(),Utils.z_index.ToString()),
                        //        new KeyValuePair<string, object>("Color",ClustererInterpolateColor(size - 12, 100)),
                        //    }, "ClustererSymbol");
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
            PointEx pBase = Utils.MapFun.MainMap.MapToScreen(Utils.MapFun.MainMap.MapPointFactory.Create(FixedPoint[0], FixedPoint[1]));

            double xInterval = 0, yInterval = 0;

            PointEx pBase1 = new PointEx(pBase.X + p_intervalValue, pBase.Y);
            MapPointEx p1 = Utils.MapFun.MainMap.ScreenToMap(pBase1);
            xInterval = Math.Abs(p1.X - FixedPoint[0]);

            PointEx pBase2 = new PointEx(pBase.X, pBase.Y + p_intervalValue);
            MapPointEx p2 = Utils.MapFun.MainMap.ScreenToMap(pBase2);
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
