#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：地图选点控件
 * 日 期 ：2017-05-18
 ***************************************************************#@#***************************************************************/
#endregion
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.MAP.Control;
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

namespace CO_IA.UI.MAP
{
    /// <summary>
    /// MapCoordinate.xaml 的交互逻辑
    /// </summary>
    public partial class MapCoordinate : UserControl
    {
        private bool CanEdit = true;
        /// <summary>
        /// 是否显示区域，在初始化开始就设置此属性
        /// </summary>
        public bool ShowAreaGeo = true;
        /// <summary>
        /// 地图类
        /// </summary>
        private ActivityPlaceMap _mapGis;
        public Action<Point> OnLocationMove;
        /// <summary>
        /// 是否移动选择点
        /// </summary>
        //private bool MoveSelectType = false;

        private Point _orgPoint  = new Point(-1,-1);
        public Point OrgPoint
        {
            get{return _orgPoint;}
            set 
            {
                _orgPoint.X = value.X ;
                _orgPoint.Y = value.Y;
            }
        }
        private Point _currentPoint = new Point();
        
        private  Point _selectPoint = new Point();
        public Point selectPoint    
        {
            get
            {
                return _selectPoint;
            }
            set
            {
                _selectPoint.X = value.X;
                _selectPoint.Y = value.Y;
                if (_orgPoint.X == -1 && _orgPoint.Y==-1)
                {
                    _orgPoint.X = _selectPoint.X;
                    _orgPoint.Y = _selectPoint.Y;
                    //_mapGis.MainMap.Location(_mapGis.MainMap.MapPointFactory.Create(_orgPoint.X, _orgPoint.Y));
                }

                _mapGis.ShowMap.MainMap.Initialized += ((success) =>
                {
                    if (!success)
                        return;
                    AddTips(_selectPoint);
                    _mapGis.ShowMap.setExtent(new AT_BC.Data.GeoPoint() { Longitude = _selectPoint.X, Latitude = _selectPoint.Y });
                });
                if (_mapGis.ShowMap.initialized)
                {
                    AddTips(_selectPoint);
                    _mapGis.ShowMap.setExtent(new AT_BC.Data.GeoPoint() { Longitude = _selectPoint.X, Latitude = _selectPoint.Y });
                } 
            }
        }

       
        /// <summary>
        /// 指针选点
        /// </summary>
        public MapCoordinate()
        {
            InitializeComponent();
            _mapGis = new ActivityPlaceMap();
            _mapGis.ShowMap.MapInitialized += MapInitialized;
            //_mapGis.MainMap.IsOverviewVisible = true;
            grid_map.Children.Insert(0,_mapGis.ShowMap.MainMap);

            System.Windows.Resources.StreamResourceInfo sri = Application.GetResourceStream(new Uri("/CO_IA.UI.MAP;component/Images/cursor/Link_Hnd.cur", UriKind.RelativeOrAbsolute));
            Cursor customCursor = new Cursor(sri.Stream);
            this.Cursor = customCursor;
        }
        

        /// <summary>
        /// 指针选点
        /// </summary>
        public MapCoordinate(bool canEdit)
        {
            InitializeComponent();
            _mapGis = new ActivityPlaceMap();
            _mapGis.ShowMap.MapInitialized += MapInitialized;
            //_mapGis.MainMap.IsOverviewVisible = true;
            grid_map.Children.Insert(0, _mapGis.ShowMap.MainMap);

            System.Windows.Resources.StreamResourceInfo sri = Application.GetResourceStream(new Uri("/CO_IA.UI.MAP;component/Images/cursor/Link_Hnd.cur", UriKind.RelativeOrAbsolute));
            Cursor customCursor = new Cursor(sri.Stream);
            this.Cursor = customCursor;

            CanEdit = canEdit;
        }
        /// <summary>
        /// 拖动选点
        /// </summary>
        /// <param name="moveSelectType"></param>
        //public MapCoordinate(bool moveSelectType)
        //{
        //    InitializeComponent();
        //    _mapGis = new MapGIS();
        //    _mapGis.MapInitialized += MapInitialized;
        //    _mapGis.MainMap.IsOverviewVisible = true;
        //    grid_map.Children.Insert(0, _mapGis.MainMap);

        //    MoveSelectType = moveSelectType;
        //}
        private void MapInitialized(bool success)
        {
            if (success)
            {
                //_mapGis.MainMap.IsShowXY = true;
                if(CanEdit)
                    _mapGis.ShowMap.MainMap.MapMouseClick += MainMap_MapMouseClick;
                if (ShowAreaGeo)
                    ShowArea();
            }
            else
            {
                MessageBox.Show("加载地图失败！");
            }
        }
        private void ShowArea()
        {
            if (CO_IA.Client.RiasPortal.ModuleContainer==null||CO_IA.Client.RiasPortal.ModuleContainer.Activity == null)
                return;
            ActivityPlaceInfo[] places = Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            Dictionary<string, ActivityPlaceInfo> dicPlaces = new Dictionary<string, ActivityPlaceInfo>();
            foreach (ActivityPlaceInfo place in places)
            {
                dicPlaces.Add(place.Guid, place);
            }
            _mapGis.PlaceLocation = dicPlaces;
            _mapGis.ShowMap.SetAllGraphicsExtent();
        }
        private void AddTips(Point p)
        {

            if (_mapGis.ShowMap.MainMap.DefaultLayer != null)
                _mapGis.ShowMap.MainMap.DefaultLayer.RemoveSymbolElement("selectP");
            if (p.X == 0 || p.Y == 0)
                return;
            _mapGis.ShowMap.MainMap.DefaultLayer.DrawPoint(_mapGis.ShowMap.MainMap.MapPointFactory.Create(p.X, p.Y), new I_GS_MapBase.Portal.SymbolElement("selectP")
            {
                ControlTemplate = _mapGis.ShowMap.MainMap.Resources["PointMapCoor"]
            });
            //if (MoveTips == null)
            //{
            //    MoveTips = new MoveSelect(_mapGis);
                
            //}
            //I_GS_MapBase.Portal.Types.PointEx p = _mapGis.MainMap.MapToScreen(_mapGis.MainMap.MapPointFactory.Create(selectPoint.X, selectPoint.Y));
            //MoveTips.Margin = new Thickness(p.X, p.Y - MoveTips.ActualHeight, 0, 0);
            //if (_mapGis.MainMap.UserGrid.Children.Contains(MoveTips))
            //    _mapGis.MainMap.UserGrid.Children.Remove(MoveTips);
            //_mapGis.MainMap.UserGrid.Children.Add(MoveTips);

        }
        void MainMap_MapMouseClick(I_GS_MapBase.Portal.Types.MapMouseEventArgs pArgs)
        {
            _selectPoint =_currentPoint = new Point(pArgs.MapPoint.X, pArgs.MapPoint.Y);
            AddTips(_currentPoint);
            if (OnLocationMove != null)
                OnLocationMove(_currentPoint);
            
        }
        /// <summary>
        /// 回到原位置
        /// </summary>
        public void ResetLocation()
        {
            if(OrgPoint.X!=-1&&OrgPoint.Y!=-1)
                selectPoint = OrgPoint;
        }
        /// <summary>
        /// 点地图居中显示
        /// </summary>
        /// <param name="p"></param>
        public void Location(Point p)
        {
            if (p.X == 0 || p.Y == 0)
                return;
            _mapGis.ShowMap.MainMap.Initialized += ((success) =>
            {
                if (!success)
                    return;
                _mapGis.ShowMap.MainMap.Location(_mapGis.ShowMap.MainMap.MapPointFactory.Create(p.X, p.Y));
            });
            if (_mapGis.ShowMap.initialized)
            {
                _mapGis.ShowMap.MainMap.Location(_mapGis.ShowMap.MainMap.MapPointFactory.Create(p.X, p.Y));
            }
              
        }
        /// <summary>
        /// 清空点
        /// </summary>
        public void ClearPoint()
        {
            _mapGis.ShowMap.MainMap.Initialized += ((success) =>
            {
                if (!success)
                    return;
                if (_mapGis.ShowMap.MainMap.DefaultLayer != null)
                    _mapGis.ShowMap.MainMap.DefaultLayer.RemoveSymbolElement("selectP");
            });
            if (_mapGis.ShowMap.initialized)
            {
                if (_mapGis.ShowMap.MainMap.DefaultLayer != null)
                    _mapGis.ShowMap.MainMap.DefaultLayer.RemoveSymbolElement("selectP");
            }
             
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_mapGis.ShowMap.initialized)
                return;
            _mapGis.ShowMap.fullExtent();
        }

        private void dwBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            dwBtn.Opacity = 1;
        }

        private void dwBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            dwBtn.Opacity = 0.5;
        }
    }
}
