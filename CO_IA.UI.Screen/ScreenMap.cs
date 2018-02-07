using CO_IA.UI.MAP;
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.Screen
{
    public class ScreenMap:MapGIS
    {
        public ScreenMap()
        {
            MainMap.IsOverviewVisible = true;
        }
        //public MapExtent GetMapExtentByPoints(List<MapPointEx> points)
        //{ 
        //    double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
        //    for(int i=0;i<points.Count;i++)
        //    {
        //        if (x1 == 0)
        //        {
        //            x1 = points[i].X;
        //            y1 = points[i].Y;
        //            x2 = points[i].X;
        //            y2 = points[i].Y;
        //        }
        //        x1 = x1 > points[i].X ? points[i].X : x1;
        //        y1 = y1 > points[i].Y ? y1 : points[i].Y;
        //        x2 = x2 > points[i].X ? x2 : points[i].X;
        //        y2 = y2 > points[i].Y ? points[i].Y : y2;
        //    } 
        //    return MainMap.MapExtentFactory.Create(MainMap.MapPointFactory.Create(x1, y1), MainMap.MapPointFactory.Create(x2, y2));
        //}
        //public MapExtent GetMapExtentByPoints(List<GeoPoint> points)
        //{
        //    double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
        //    for (int i = 0; i < points.Count; i++)
        //    {
        //        if (x1 == 0)
        //        {
        //            x1 = points[i].X;
        //            y1 = points[i].Y;
        //            x2 = points[i].X;
        //            y2 = points[i].Y;
        //        }
        //        x1 = x1 > points[i].X ? points[i].X : x1;
        //        y1 = y1 > points[i].Y ? y1 : points[i].Y;
        //        x2 = x2 > points[i].X ? x2 : points[i].X;
        //        y2 = y2 > points[i].Y ? points[i].Y : y2;
        //    }
        //    return MainMap.MapExtentFactory.Create(MainMap.MapPointFactory.Create(x1, y1), MainMap.MapPointFactory.Create(x2, y2));
        //}
    }
}
