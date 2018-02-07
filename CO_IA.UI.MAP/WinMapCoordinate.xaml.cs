#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：地图选点控件window窗口
 * 日 期 ：2017-05-18
 ***************************************************************#@#***************************************************************/
#endregion
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
using System.Windows.Shapes;

namespace CO_IA.UI.MAP
{
    /// <summary>
    /// WinMapCoordinate.xaml 的交互逻辑
    /// </summary>
    public partial class WinMapCoordinate : Window
    {
        bool CanEdit = true;
        /// <summary>
        /// 是否显示区域，在初始化开始就设置此属性
        /// </summary>
        public bool ShowAreaGeo
        {
            set
            {
                if (mapcoor != null)
                    mapcoor.ShowAreaGeo = value;
            }
        }
        //public Action<Point> OnLocationMove;
        public MapCoordinate mapcoor = null;
        public WinMapCoordinate()
        {
            InitializeComponent();
            mapcoor = new MapCoordinate(CanEdit);
            g.Children.Add(mapcoor);
            //mapcoor.OnLocationMove += OnLocationMoveEvent;
        }
        public WinMapCoordinate(bool canEdit)
        {
            InitializeComponent();
            CanEdit = canEdit;
            mapcoor = new MapCoordinate(CanEdit);
            g.Children.Add(mapcoor);
            //mapcoor.OnLocationMove += OnLocationMoveEvent;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        //private void OnLocationMoveEvent(Point p)
        //{
        //    if (OnLocationMove != null)
        //    {
        //        OnLocationMove(p);
        //    }
        //}
        /// <summary>
        /// 回到原位置
        /// </summary>
        //public void ResetLocation()
        //{
        //    mapcoor.ResetLocation();
        //}
        /// <summary>
        /// 点地图居中显示
        /// </summary>
        /// <param name="p"></param>
        //public void Location(Point p)
        //{
        //    mapcoor.Location(p);

        //}
    }
}
