using CO_IA.UI.Screen.Track;
using GS_MapBase;
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CO_IA.UI.Screen
{
    public class DrawTrack
    {
        private MapLayer mapLayer;

        private List<MapPointEx> points = new List<MapPointEx>();

        private ClearButton closeButton;
        public bool Drawing = false;
        public DrawTrack()
        {
            
        }
        //public DrawTrack(UserControl uc)
        //{
        //    closeButton = new ClearButton(uc);
        //    mapLayer = Obj.screenMap.MainMap.CreateMapLayer();
        //    closeButton.button.Click += (m, n) =>
        //    {
        //        Clear();
        //    };
        //    Reset();
        //}
      
        //~DrawTrack()
        //{
        //    if(mapLayer!=null)
        //        Obj.screenMap.MainMap.RemoveMapLayer(mapLayer.Id);
        //}
        public void Reset(UserControl uc=null)
        {
            points.Clear();
            if (mapLayer != null)
            {
                Obj.screenMap.MainMap.RemoveMapLayer(mapLayer.Id);                
            }
            mapLayer = Obj.screenMap.MainMap.CreateMapLayer();
            mapLayer.ClearSymbolElements();
            mapLayer.ClearSlElements();

            closeButton = new ClearButton(uc);

            closeButton.button.Click += (m, n) =>
            {
                Clear();
            };
            
        }
        public void Clear()
        {
            if (Drawing)
                return;
            if (mapLayer != null)
            {
                mapLayer.ClearSymbolElements();
                mapLayer.ClearSlElements();
                points.Clear();
            }
        }
        public void AddPoint(MapPointEx point)
        {
            if (points.Count == 0)
            {
                mapLayer.DrawPoint(point, new I_GS_MapBase.Portal.SymbolElement("track")
                {
                    ControlTemplate = Obj.screenMap.MainMap.Resources["TrackBegin"],
                });
            }
            else
            {
                DrawLine(new MapPointEx[] { points[points.Count-1],point});
            }

            points.Add(point);

            DrawClose(point);
        }


        private void DrawLine(MapPointEx[] arrPoint)
        {
            if (arrPoint != null && arrPoint.Length > 1)
            {
                mapLayer.DrawPolyLine(arrPoint.ToList(), new I_GS_MapBase.Portal.SymbolElement("track")
                {
                    ControlTemplate = Obj.screenMap.MainMap.Resources["TrackLine"],
                    DataSources = new List<KeyValuePair<string, object>>{ 
                        new KeyValuePair<string,object>("Visibled","Visible"),
                    }                   
                });
                ClientUtile client = ClientUtile.Create();
                double angle = client.GetAzimuthAngle(arrPoint[1].X, arrPoint[1].Y, arrPoint[0].X, arrPoint[0].Y);
                PointEx pNew = client.GetRelativePoint(new PointEx(arrPoint[1].X, arrPoint[1].Y),angle,2);
                mapLayer.DrawArrow(new List<MapPointEx>() { Obj.screenMap.GetMapPointEx(pNew.X,pNew.Y), arrPoint[1] }, new I_GS_MapBase.Portal.SymbolElement("track")
                {
                    DataSources = new List<KeyValuePair<string, object>>{ 
                        new KeyValuePair<string,object>(GraphicStyle.ArrowSize.ToString(),"10"),
                        new KeyValuePair<string,object>(GraphicStyle.ArrowColor.ToString(),"Green"),
                    }
                });
            }
        }
        private void DrawClose(MapPointEx point)
        {
            mapLayer.AddSlElement(closeButton,point);
        }
       
    }
}
