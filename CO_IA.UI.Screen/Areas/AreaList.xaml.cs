using AT_BC.Common;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.MAP;
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

namespace CO_IA.UI.Screen.Areas
{
    /// <summary>
    /// AreaList.xaml 的交互逻辑
    /// </summary>
    public partial class AreaList : UserControl
    {
        public Action<ActivityPlaceInfo> AreaChange;
        public AreaList()
        {
            InitializeComponent();            
        }
        public void iniData()
        {
            Obj.screenMap.MainMap.DefaultLayer.OnGraphicsMouseRightButtonUp += DefaultLayer_OnGraphicsMouseRightButtonUp;
            Obj.AreaGraphicInfo.Clear();
            cmbAreas.Items.Clear();
            ActivityPlaceInfo defaultSelect = new ActivityPlaceInfo() {  Name="全部区域"};
            defaultSelect.Image = ClientHelper.ResourceImageToBytes("pack://application:,,,/CO_IA.Themes;component/ActivityTypeImages/defaultActivity.png");
            cmbAreas.Items.Add(defaultSelect);

            if (Obj.ActivityPlaces!=null&&Obj.ActivityPlaces.Length>0)
            {
                foreach (var item in Obj.ActivityPlaces)
                {
                    cmbAreas.Items.Add(item);
                    //此处绘制区域图形
                    ReturnDrawGraphicInfo[] infos = Obj.screenMap.DrawArea(MapGroupTypes.AreaRange_.ToString() + item.Guid, item.Graphics);
                    Obj.AreaGraphicInfo.Add(item.Guid,infos);
                    //画地点
                    Obj.screenMap.DrawLocation(item.Locations);
                }
                //Obj.screenMap.SetAllAreaGraphicsExtent();
            }
            cmbAreas.SelectedIndex = 0;
           
        }

        private void cmbAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAreas.SelectedItem != null)
            {
                ActivityPlaceInfo SelectItem = cmbAreas.SelectedItem as ActivityPlaceInfo;
                if (SelectItem != null)
                {
                    //Obj.screenMap.RemoveElementByFlag(MapGroupTypes.Location_.ToString());
                    //Obj.screenMap.RemoveSymbolElementByFlag(MapGroupTypes.Location_.ToString());
                    if (SelectItem.Name == "全部区域")
                    {
                        Obj.SelectedAreaID = "";
                        Obj.screenMap.SetElementVisibilityByFlag(MapGroupTypes.Location_.ToString(), true);

                    }
                    else
                    {
                        Obj.SelectedAreaID = SelectItem.Guid;
                        Obj.screenMap.SetElementVisibilityByFlag(MapGroupTypes.Location_.ToString(), false);
                        foreach (var location in SelectItem.Locations)
                        {
                            Obj.screenMap.SetElementVisibilityByID(MapGroupTypes.Location_.ToString() + location.GUID, true);
                        }

                    }
                    if (AreaChange != null)
                        AreaChange(SelectItem);
                }
            }
        }
                
        /// <summary>
        /// 活动地点右键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DefaultLayer_OnGraphicsMouseRightButtonUp(object sender, GraphicEventArgs e)
        {
            IDictionary<string, object> Dic = e.DC as IDictionary<string, object>;
            string ElementId = Dic["ElementId"] == null ? null : Dic["ElementId"].ToString();
            #region 图形
            if (e.QueryBehavior == QueryBehavior.Polygon || e.QueryBehavior == QueryBehavior.Polyline)
            {
                if (ElementId.StartsWith(MapGroupTypes.AreaRange_.ToString()))
                {
                    Path p = sender as Path;
                    if (p == null)
                        return;
                    //if (p.Tag == null)
                    //    p.Tag = GetSiteAreaId(ElementId);
                                    
                    if (p.ContextMenu == null)
                    {
                        p.ContextMenu = GetAreaContextMenu(GetSiteAreaId(ElementId));
                    }
                }
            }
            #endregion
        }
        #region 地点
        /// <summary>
        /// 根据图形ID获取地点的GUID
        /// </summary>
        /// <param name="elementID"></param>
        /// <returns></returns>
        private string GetSiteAreaId(string elementID)
        {
            string ElementId = elementID.Replace(MapGroupTypes.AreaRange_.ToString(), "");
            return ElementId.Substring(0, ElementId.LastIndexOf('-'));
        }
        /// <summary>
        /// 创建右键菜单 用于地点区域右键
        /// </summary>
        /// <returns></returns>
        private ContextMenu GetAreaContextMenu(string AreaId)
        {
            ContextMenu menu = new ContextMenu();
            MenuItem item = null;                      

            item = new MenuItem();
            item.Tag = AreaId;
            item.Header = "所驻人员信息";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/22.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemAreaPerson_Click;
            menu.Items.Add(item);
                        
            item = new MenuItem();
            item.Tag = AreaId;
            item.Header = "周围台站查询";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/44.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemArroundSation_Click;
            menu.Items.Add(item);

            item = new MenuItem();
            item.Tag = AreaId;
            item.Header = "参与单位查询";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/88.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemOrg_Click;
            menu.Items.Add(item);

            item = new MenuItem();
            item.Tag = AreaId;
            item.Header = "任务信息";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/66.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemTask_Click;
            menu.Items.Add(item);

            item = new MenuItem();
            item.Tag = AreaId;
            item.Header = "现场监测情况";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/55.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemLIVEMonitor_Click;
            menu.Items.Add(item);           
            
            item = new MenuItem();
            item.Tag = AreaId;
            item.Header = "历史监测记录";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/77.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemHistoryMonitor_Click;
            menu.Items.Add(item);

            item = new MenuItem();
            item.Tag = AreaId;
            item.Header = "查看视频";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/video.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemVideo_Click;
            menu.Items.Add(item);
            return menu;
        }
        /// <summary>
        /// 所驻人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemAreaPerson_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null && item.Tag != null)
            {
                string AreaId = item.Tag.ToString();
                List<PP_OrgInfo> orgList = new List<PP_OrgInfo>();
                ScheduleDetail[] scheduleDetails = null;
                if (Obj.Activity.ActivityStage == Types.ActivityStage.Prepare)
                {
                    ScheduleDetail[] Details = Utility.getOrgGroupsBySchedule(Obj.Activity.Guid, Obj.Activity.ActivityStage);
                    scheduleDetails = Details.Where(itm => {
                        if (itm.ScheduleOrgs != null && itm.ScheduleOrgs.Length > 0)
                        {
                            foreach (var info in itm.ScheduleOrgs)
                            { 
                                if(info.AREA_GUID==AreaId)
                                    return true;
                            }
                        }
                        return false;
                    }).ToArray();
                }
                else
                {
                    scheduleDetails = Utility.getOrgGroupsBySchedule(Obj.Activity.Guid, AreaId);
                }
                if (scheduleDetails != null && scheduleDetails.Length > 0)
                {
                        
                    foreach (var detail in scheduleDetails)
                    {
                        if (detail.ScheduleOrgs != null && detail.ScheduleOrgs.Length > 0)
                        {
                            foreach (var info in detail.ScheduleOrgs)
                            {
                                if (info.OrgInfo != null)
                                {
                                    orgList.Add(info.OrgInfo);
                                }
                            }
                        }
                    }
                    if (orgList.Count != 0)
                    {
                        Group.GroupDialog groupDialog = new Group.GroupDialog(orgList);
                        groupDialog.ShowDialog(this);
                        return;
                    }

                }
                
                MessageBox.Show("此区域内没有组及人员信息！");
                
            }
        }
        /// <summary>
        /// 周围监测设施
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemArroundSation_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null && item.Tag != null)
            {
                string AreaId = item.Tag.ToString();
                Dialog.SurroundStationDialog stationDialog = new Dialog.SurroundStationDialog(AreaId);
                stationDialog.Owner = VisualTreeHelperExtension.GetParentObject<System.Windows.Window>(this); 
                stationDialog.Show();
            }
        }
        /// <summary>
        /// 参与单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemOrg_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null && item.Tag != null)
            {
                string AreaId = item.Tag.ToString();
                Dialog.ORGAndEquipmentList orglist = new Dialog.ORGAndEquipmentList(AreaId);
                
                orglist.ShowDialog();
            }
        }
        /// <summary>
        /// 任务信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemTask_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null && item.Tag != null)
            {
                string AreaId = item.Tag.ToString();
                //ScheduleDetail[] scheduleDetails = Utility.getOrgGroupsBySchedule(Obj.Activity.Guid, AreaId);
                ScheduleDetail[] scheduleDetails = Utility.getOrgGroupsBySchedule(Obj.Activity.Guid, Obj.Activity.ActivityStage);
                if (scheduleDetails != null && scheduleDetails.Length > 0)
                {
                    List<string> orgList = new List<string>();
                    foreach (var detail in scheduleDetails)
                    {
                        if (detail.ScheduleOrgs != null && detail.ScheduleOrgs.Length > 0)
                        {
                            foreach (var info in detail.ScheduleOrgs)
                            {
                                if (info.OrgInfo != null&&info.AREA_GUID==AreaId)
                                {
                                    orgList.Add(info.OrgInfo.GUID);
                                }
                            }
                        }
                    }
                    if (orgList.Count != 0)
                    {
                        Task.TaskAllList taskList = new Task.TaskAllList(orgList.ToArray());
                        taskList.ShowDialog(this);
                        return;
                    }
                   
                }
                MessageBox.Show("此区域查询不到任务信息！");
            }
        }
        /// <summary>
        /// 历史监测记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemHistoryMonitor_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null && item.Tag != null)
            {
                string AreaId = item.Tag.ToString();
                ActivityPlaceInfo[] placeInfo = Obj.ActivityPlaces.Where(itm => itm.Guid == AreaId).ToArray();
                if (placeInfo != null && placeInfo.Length == 1)
                {
                    Monitor.MonitorView rh = new Monitor.MonitorView(placeInfo[0]);
                    rh.Title = "历史监测记录";
                    rh.ShowDialog();
                }
            }
        }
        /// <summary>
        /// 现场监测情况
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemLIVEMonitor_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null && item.Tag != null)
            {
                string AreaId = item.Tag.ToString();
                var area = Obj.ActivityPlaces.Where(itm => itm.Guid == AreaId).ToArray();
                if (area == null || area.Length == 0)
                {
                    MessageBox.Show("没有区域信息");
                    return;
                }
                else
                {
                    ScheduleDetail[] scheduleDetails = Utility.getOrgGroupsBySchedule(Obj.Activity.Guid, Obj.Activity.ActivityStage);
                    if (scheduleDetails == null)
                    {
                        MessageBox.Show("此区域没有相关信息！");
                        return;
                    }
                    var obj = scheduleDetails.Where(itm =>
                    {
                        if (itm.ScheduleOrgs != null && itm.ScheduleOrgs.Length > 0 && itm.ScheduleOrgs[0].AREA_GUID == AreaId)
                            return true;
                        else
                            return false;
                    }).ToArray();
                    if (obj != null && obj.Length > 0)
                    {
                        List<PP_OrgInfo> orgs = new List<PP_OrgInfo>();
                        foreach (var itm in obj)
                        {
                            orgs.Add(itm.ScheduleOrgs[0].OrgInfo);
                        }
                        string areaid = obj[0].ScheduleOrgs[0].AREA_GUID;
                        Monitor.MonitorView rh = new Monitor.MonitorView(AreaId, orgs);
                        rh.Title = "现场监测情况";
                        rh.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("此区域没有相关信息！");
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// 历史监测记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemVideo_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null && item.Tag != null)
            {
                string AreaId = item.Tag.ToString();
                VideoWindow v = new VideoWindow(AreaId);
                v.Show();
            }
        }
        #endregion

        private void DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double left = this.Margin.Left + e.HorizontalChange;
            if (left < 0) left = 0;
            if (left > Obj.screenMap.MainMap.ActualWidth - 10)
                left = Obj.screenMap.MainMap.ActualWidth - 10;
            double top = this.Margin.Top + e.VerticalChange;
            if (top < 0)
                top = 0;
            if (top > Obj.screenMap.MainMap.ActualHeight - 10)
                top = Obj.screenMap.MainMap.ActualHeight - 10;
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

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //查看视频
            VideoWindow v = new VideoWindow(Obj.SelectedAreaID);
            v.Show();
        }
    }
}
