using CO_IA.Data;
using DevExpress.Xpf.Core.DragAndDrop;
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

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// MapPlace.xaml 的交互逻辑
    /// </summary>
    public partial class MapPlace : UserControl, IFrameworkElement
    {
        public string PlaceGuid
        {
            get;
            set;
        }

        private PP_OrgInfo orginfo;

        public PP_OrgInfo ORGinfo
        {
            get
            {
                return orginfo;
            }
            set
            {
                orginfo = value;
                if (string.IsNullOrEmpty(orginfo.PARENT_GUID))
                {
                    this.txtGroupname.Text = "指挥中心";
                    img.Source = new BitmapImage(new Uri("pack://application:,,,/CO_IA.Themes;component/Images/Area/zhzx.png"));
                    this.Margin = new Thickness(-2,-32,0,0);
                }
                else
                {
                    this.txtGroupname.Text = orginfo.NAME;
                }
            }
        }

        public MapPointEx MapPoint
        {
            get;
            set;
        }

        public string ElementId
        {
            get { return "org_" + ORGinfo.GUID; }
            set{}
        }
        public object ElementTag
        {
            get;
            set;
        }
        private bool ischecked;
        public bool IsChecked
        {
            get
            {
                return ischecked;
            }
            set
            {
                ischecked = value;
                if (ischecked)
                {
                    checkBorder.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    checkBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        public event Action<PP_OrgInfo> BeforeDragPlaceEvent;

        public event Action<PP_OrgInfo> DeletePlaceEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="placeguid">地点ID</param>
        /// <param name="groupid">组ID</param>
        /// <param name="groupname">组名称</param>
        public MapPlace()
        {
            InitializeComponent();
        }

        private void Place_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                e.Handled = true;
                DragDrop.DoDragDrop(this, sender, DragDropEffects.Move);
            }
        }

        private void Place_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (BeforeDragPlaceEvent != null)
            {
                BeforeDragPlaceEvent(ORGinfo);
            }
        }

        private void Place_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
         
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DeletePlaceEvent != null)
            {
                DeletePlaceEvent(ORGinfo);
            }
        }

    }
}
