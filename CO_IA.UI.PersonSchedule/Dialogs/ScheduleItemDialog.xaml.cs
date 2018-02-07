using CO_IA.Client;
using CO_IA.Data;
 
 
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// ScheduleItemDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ScheduleItemDialog : Window
    {
        /// <summary>
        /// 全部日程列表，仅作为校验用 from this tag
        /// </summary>
        public Schedule[] allSchedule = null;
        /// <summary>
        /// 活动的主体
        /// </summary>
        private Activity activity;
        /// <summary>
        /// 日程
        /// </summary>
        private Schedule _scheduleInfo;
      
        DateTime ActivityDateFrom;
        DateTime ActivityDateTo;
        public Action<bool> ItemChanged;
        public ScheduleItemDialog(Schedule scheduleInfo)
        {
            InitializeComponent();

            //if (scheduleInfo.ScheduleDetailInfos == null)
            //    scheduleInfo.ScheduleDetailInfos = new ScheduleDetail[0];

            //this.Title = "日程编辑 - 活动时间：" + CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateFrom.ToString("yyyy年MM月dd日") + " - " + CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateTo.ToString("yyyy年MM月dd日");
            //#region 处理此日程时间
            //DateTime.TryParse(CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateFrom.ToString("yyyy-MM-dd 00:00:01"), out ActivityDateFrom);
            //DateTime.TryParse(CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateTo.ToString("yyyy-MM-dd 23:23:59"), out ActivityDateTo);

            //DateTime dt = scheduleInfo.STARTTIME;
            //DateTime.TryParse(scheduleInfo.STARTTIME.ToString("yyyy-MM-dd 00:00:01"), out dt);
            //scheduleInfo.STARTTIME = dt;
            //DateTime.TryParse(scheduleInfo.STOPTIME.ToString("yyyy-MM-dd 23:23:59"), out dt);
            //scheduleInfo.STOPTIME = dt;
            //#endregion

            //_scheduleInfo = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<Schedule>(scheduleInfo);
            ////_scheduleInfo = scheduleInfo;            

            //activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;

            //mapGrid.Children.Add(placeMap.ShowMap.MainMap);
            //placeMap.ShowMap.MainMap.AllowDrop = true;
            //placeMap.ShowMap.MainMap.Drop += MainMap_Drop;
            //placeMap.ShowMap.MapInitialized += MapInitialized;
            
        }
        #region 去掉ICON
        protected override void OnSourceInitialized(EventArgs e)
        {
            //IconHelper.RemoveIcon(this);
        }
        #endregion

        private void MapInitialized(bool success)
        {
            if (success)
            {
                GetAllSchedule();
                IniData();
                MainGrid.DataContext = _scheduleInfo;//此项包含子项
            }
        }

        #region 初始化数据
        private void IniData()
        {
            //ActivityPlaceInfo[] places = Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            //listPlace.ItemsSource = places;
            //if(places!=null&&places.Length>0)
            //listPlace.SelectedIndex = 0;

            //Dictionary<string, ActivityPlaceInfo> dicPlaces = new Dictionary<string, ActivityPlaceInfo>();
            //foreach (ActivityPlaceInfo place in places)
            //{
            //    dicPlaces.Add(place.Guid, place);
            //}
            //placeMap.PlaceLocation = dicPlaces;
            //placeMap.ShowMap.SetAllGraphicsExtent();

            //List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();
            //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            //{
            //    //更新当前节点
            //    nodes = channel.GetPP_OrgInfos(activity.Guid);
            //});
            //if (nodes.Count == 0)
            //{
            //    MessageBox.Show("没有组信息，请现在人员预案中设置！");
            //}
            //else
            //{
            //    tv_PersonPlan.ItemsSource = null;
            //    PP_OrgInfo tempOrgInfo = new PP_OrgInfo();

            //    foreach (PP_OrgInfo oinfo in nodes)
            //    {
            //        if (string.IsNullOrEmpty(oinfo.PARENT_GUID))
            //        {
            //            tempOrgInfo = oinfo;
            //            break;
            //        }
            //    }
            //    ForeachPropertyNode(tempOrgInfo, tempOrgInfo.GUID, nodes);

            //    List<PP_OrgInfo> itemList = new List<PP_OrgInfo>();
            //    itemList.Add(tempOrgInfo);
            //    this.tv_PersonPlan.ItemsSource = null;
            //    this.tv_PersonPlan.ItemsSource = itemList;

            //}
        }
        //无限接循环子节点添加到根节点下面
        private void ForeachPropertyNode(PP_OrgInfo node, string pid, List<PP_OrgInfo> nodes)
        {
            foreach (PP_OrgInfo tempNode in nodes)
            {
                if (tempNode.PARENT_GUID == pid)
                {
                    ForeachPropertyNode(tempNode, tempNode.GUID, nodes);
                    node.Children.Add(tempNode);
                }
            }
        }
        private void GetAllSchedule()
        {
            //allSchedule = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule, Schedule[]>(channel =>
            //{
            //    return channel.GetScheduleInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            //});
        }
        #endregion

        #region 保存按钮
        /// <summary>
        /// 保存按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidatedScheduleInfo())
                return;
            if (!ValidateScheduleDetail())
                return;
            //保存
            //ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
            //if (selectItem != null)
            //{
                //SaveAreasAndGroups(selectItem);
            //}
            if (SaveSchedule())
            {
                GetAllSchedule();
                MessageBox.Show("保存成功！");
            }
            if (ItemChanged != null)
            {
                ItemChanged(true);
            }
        }
        private bool SaveSchedule()
        {
            bool success = false;
            //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            //{
            //     success = channel.SaveScheduleInfo(_scheduleInfo);
            //});
            return success;
        }
        #endregion

        #region 组
       
        
        public void GetRadomXY(MapPointEx pointBegin, MapPointEx pointEnd, out double x, out double y, Random random = null)
        {
            if (random == null)
                random = new Random();
            x = 0;
            y = 0;
            int xStepCount = 50;
            int yStepCount = 30;
            double xoffset = pointEnd.X - pointBegin.X;
            double yoffset = pointBegin.Y - pointEnd.Y;
            double xStep = xoffset * 1.0 / xStepCount;
            double yStep = yoffset * 1.0 / yStepCount;

            x = pointBegin.X + xStep * 1.0 * random.Next(xStepCount);
            y = pointEnd.Y + yStep * 1.0 * random.Next(yStepCount);
        }
        
        private void CheckGroup()
        {
            //PP_OrgInfo item = tv_PersonPlan.SelectedItem as PP_OrgInfo;
            //ActivityPlaceInfo placeinfo = listPlace.SelectedItem as ActivityPlaceInfo;
            //if (item != null&&placeinfo != null)
            //{                
            //    if (item.IsChecked)
            //    {
            //        double[] p = null;
                    
            //        //计算坐标，通过当前选中的区域

            //        MapExtent extent = GetAreaExtent(placeinfo);
            //        if (extent == null)
            //        {
            //            MessageBox.Show("没有该区域的任何信息，请完善该区域！");
            //            return;
            //        }
            //        p = new double[2] { 0, 0 };
            //        if (extent != null)
            //        {
            //            GetRadomXY(extent.Xy1, extent.Xy2, out p[0], out p[1]);
            //        }
            //        ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
            //        if(selectItem!=null)
            //        {
            //            if (placeMap.ShowMap.CheckCoordinate(p))
            //            {
            //                ScheduleOrg org = new ScheduleOrg();
            //                org.GUID = Utility.NewGuid();
            //                org.AREA_GUID = placeinfo.Guid;
            //                org.GROUP_GUID = item.GUID;
            //                org.SCHEDULE_DETAIL_GUID = selectItem.GUID;
            //                org.LONGITUDE = p[0];
            //                org.LATITUDE = p[1];
            //                List<ScheduleOrg> orgList = selectItem.ScheduleOrgs == null ? new List<ScheduleOrg>() : selectItem.ScheduleOrgs.ToList();
            //                orgList.Add(org);
            //                selectItem.ScheduleOrgs = orgList.ToArray();
                        
            //                //往地图上画
            //                //placeMap.ShowMap.DrawPoint(p[0], p[1], "", item.GUID);
            //                MapPointEx point = placeMap.ShowMap.MainMap.MapPointFactory.Create(p[0], p[1]);
            //                DrawORGToMap(GetMapPlace(item, point), point);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
            //        if (selectItem != null&&selectItem.ScheduleOrgs!=null&&selectItem.ScheduleOrgs.Length>0)
            //        {
            //            List<ScheduleOrg> orgList = selectItem.ScheduleOrgs.ToList();
            //            for (int i=0; i< orgList.Count;i++)
            //            {
            //                if (orgList[i].AREA_GUID == placeinfo.Guid && orgList[i].GROUP_GUID == item.GUID)
            //                {
            //                    RemoveOrgToMap("org_" + orgList[i].GROUP_GUID);
            //                    orgList.RemoveAt(i);                                
            //                    selectItem.ScheduleOrgs = orgList.ToArray();

            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}
        }
        private MapExtent GetAreaExtent(ActivityPlaceInfo placeinfo)
        {
            //if (placeinfo == null)
            //    return null;
            //if(placeinfo.Graphics==null&&(placeinfo.Locations==null||placeinfo.Locations.Length==0))
            //    return null;
            //if (string.IsNullOrEmpty(placeinfo.Graphics))
            //{
            //    if (placeinfo.Locations != null && placeinfo.Locations.Length > 0)
            //    {
            //        List<string> ids = new List<string>();
            //        foreach (var location in placeinfo.Locations)
            //        {
            //            ids.Add(MapGroupTypes.Location_.ToString() + location.GUID);
            //        }
            //        return placeMap.ShowMap.GetGraphicExtentByID(ids.ToArray());                   
            //    }
            //}
            //else
            //{
            //    return placeMap.ShowMap.GetGraphicExtentByID( new string[]{MapGroupTypes.AreaRange_.ToString()+ placeinfo.Guid});
            //}
            return null;
        }
        private void CheckBoxGroups_Click(object sender, RoutedEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
            CheckBox chk = sender as CheckBox;
            if (chk != null && chk.IsChecked == true)
            {
                ActivityPlaceInfo placeinfo = listPlace.SelectedItem as ActivityPlaceInfo;
                if (placeinfo == null || placeinfo.IsChecked == false)
                {
                    chk.IsChecked = false;
                    MessageBox.Show("当前选择区域必须勾选！");
                    return;
                }
            
                ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
                if (selectItem != null && selectItem.ScheduleOrgs != null && selectItem.ScheduleOrgs.Length != 0)
                {
                    PP_OrgInfo item = tv_PersonPlan.SelectedItem as PP_OrgInfo;
                    foreach (ScheduleOrg org in selectItem.ScheduleOrgs)
                    {
                        if (org.GROUP_GUID == item.GUID)
                        {
                            chk.IsChecked = false;
                            MessageBox.Show("已经给此小组指定区域位置，请选择其它小组！");
                            return;
                        }
                    }
                }
            }
            CheckGroup();
        }
       
        private DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);
            return source;
        }
        #region dic 与 groups 转换


        private Dictionary<string, double[]> getGroupsDic(string groups)
        {
            Dictionary<string, double[]> groupsDic = new Dictionary<string, double[]>();
            if (!string.IsNullOrEmpty(groups))
            {
                string[] s1 = groups.Split(';');
                if (s1 != null && s1.Length > 0)
                {
                    for (int i = 0; i < s1.Length; i++)
                    {
                        string[] s2 = s1[i].Split('|');//guid|x,y
                        double[] p = null;
                        if (s2.Length == 2)
                        {
                            string[] p1 = s2[1].Split(',');//x,y
                            if (p1.Length == 2)
                            {
                                p = new double[2];
                                if (double.TryParse(p1[0], out p[0]) && double.TryParse(p1[1], out p[1]))
                                {

                                }
                            }
                        }
                        groupsDic.Add(s2[0], p);
                    }
                }
            }
            return groupsDic;
        }
        private string getStringByGroupDic(Dictionary<string, double[]> groupsDic)
        {
            if (groupsDic == null || groupsDic.Count == 0)
                return "";
            string groups = "";
            foreach (var item in groupsDic)
            {
                double[] p = item.Value as double[];
                if (p == null || p.Length != 2)
                    p = new double[] { 0, 0 };

                groups += item.Key + "|" + p[0].ToString() + "," + p[1].ToString() + ";";
            }

            return groups.Trim(';');
        }
        #endregion

        #endregion

        #region 区域

        //private string getAreas()
        //{
        //    ActivityPlaceInfo[] placeInfos = listPlace.ItemsSource as ActivityPlaceInfo[];
        //    if (placeInfos == null || placeInfos.Length == 0)
        //        return "";
        //    string areas = "";
        //    foreach(var item in placeInfos)
        //    {
        //        if (item.IsChecked)
        //        {
        //            areas += item.Guid + ",";
        //        }
        //    }
        //    return areas.Trim(',');
        //}
        private void listPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ActivityPlaceInfo placeinfo = listPlace.SelectedItem as ActivityPlaceInfo;
            //if (placeinfo == null)
            //    return;
            
            //placeMap.CurrentPlaceId = placeinfo.Guid;            
            //clearOrgSelectState();

            ////勾选组织
            //ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
            //if (selectItem != null)
            //{
            //    if (selectItem.ScheduleOrgs != null && selectItem.ScheduleOrgs.Length > 0)
            //    {
            //        List<PP_OrgInfo> personList = tv_PersonPlan.ItemsSource as List<PP_OrgInfo>;
            //        if (personList != null && personList.Count > 0)
            //        {
            //            var orgs = selectItem.ScheduleOrgs.Where(item=>item.AREA_GUID==placeinfo.Guid).ToArray();
            //            SetAllNodeCheckState(orgs, personList[0]);
            //        }
            //    }
            //}


        }
        protected void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            if (item.IsSelected != true)
                item.IsSelected = true;
        }
        //private void CheckBoxAreas_Click(object sender, RoutedEventArgs e)
        //{
        //    ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
        //    if (selectItem != null)
        //    {
        //        string areas = getAreas();
        //        selectItem.AREAS = areas;
        //    }
        //}
        
        #endregion

        #region 日程详细列表
        
        private void currentItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            clearAllSelectState();
            if (currentItem.DataContext != null)
            {
                if (e.OldValue != null)
                {
                    ScheduleDetail olditem = e.OldValue as ScheduleDetail;
                    //SaveAreasAndGroups(olditem);
                }
                ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
                if (selectItem == null||selectItem.ScheduleOrgs==null||selectItem.ScheduleOrgs.Length==0)
                    return;

                ActivityPlaceInfo[] places = listPlace.ItemsSource as ActivityPlaceInfo[];
                if (places != null && places.Length > 0)
                {
                    foreach (var item in places)
                    {
                        if(selectItem.ScheduleOrgs.Where(itm=>itm.AREA_GUID==item.Guid).Any())
                        {
                            item.IsChecked = true;
                        }
                    }
                }
                listPlace_SelectionChanged(null,null);
                // guid|x,y;guid|x,y;
                //if (!string.IsNullOrEmpty(selectItem.GROUPS))
                //{
                //    Dictionary<string, double[]> groupsDic = getGroupsDic(selectItem.GROUPS);
                //    tv_PersonPlan.Tag = groupsDic;
                //    List<PP_OrgInfo> personList = tv_PersonPlan.ItemsSource as List<PP_OrgInfo>;
                //    if (personList != null && personList.Count > 0)
                //    {
                //        for (int i = 0; i < personList.Count; i++)
                //        {
                //            SetNodeCheckState(groupsDic, personList[0]);
                //        }
                //    }
                //}
            }
        }
      
        private void clearAllSelectState()
        {
            clearPlaceSelectState();
            clearOrgSelectState();
        }
        private void clearPlaceSelectState()
        {
            //清除区域选择
            ActivityPlaceInfo[] places = listPlace.ItemsSource as ActivityPlaceInfo[];
            if (places != null && places.Length > 0)
            {
                foreach (var item in places)
                {
                    item.IsChecked = false;
                }
            }            
        }
        private void clearOrgSelectState()
        {            
            //清除组选择
            //List<PP_OrgInfo> personList = tv_PersonPlan.ItemsSource as List<PP_OrgInfo>;
            //if (personList != null && personList.Count > 0)
            //{
            //    for (int i = 0; i < personList.Count; i++)
            //    {
            //        SetNodeCheckStateFalse(personList[0]);
            //    }
            //}
            //placeMap.ShowMap.RemoveElementByFlag("org_");
            ////ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
            ////if (selectItem == null || selectItem.ScheduleOrgs == null || selectItem.ScheduleOrgs.Length == 0)
            ////    return;
            ////foreach (var item in selectItem.ScheduleOrgs)
            ////{
            ////    RemoveOrgToMap(item.GROUP_GUID);
            ////}
        }
        private void SetNodeCheckStateFalse(PP_OrgInfo node)
        {

            node.IsChecked = false;
            if (node.Children != null && node.Children.Count > 0)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    SetNodeCheckStateFalse(node.Children[i]);
                }
            }
        }
        private void SetAllNodeCheckState(ScheduleOrg[] orgs, PP_OrgInfo node)
        {
            SetNodeCheckState(orgs,node);
            if (node.Children != null && node.Children.Count > 0)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    SetNodeCheckState(orgs, node.Children[i]);
                }
            }
        }
        /// <summary>
        /// 根据已有选项来勾选组织
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="node"></param>
        private void SetNodeCheckState(ScheduleOrg[] orgs, PP_OrgInfo node)
        {
            //if (orgs != null && orgs.Length > 0)
            //{
            //    var org = orgs.Where(item=>item.GROUP_GUID == node.GUID).ToArray();
            //    if (org != null && org.Length ==1)
            //    {
            //        node.IsChecked = true;
            //        //画.
            //        if (placeMap.ShowMap.CheckCoordinate(new double[] { org[0].LONGITUDE, org[0].LATITUDE }))
            //        {
            //            //往地图上画
            //            //placeMap.ShowMap.DrawPoint(p[0], p[1], "", node.GUID);
            //            MapPointEx point = placeMap.ShowMap.MainMap.MapPointFactory.Create(org[0].LONGITUDE, org[0].LATITUDE);
            //            DrawORGToMap(GetMapPlace(node, point), point);
            //        }
            //    }
            //}
        }
        
        private MapPlace GetMapPlace(PP_OrgInfo OrgInfo, MapPointEx point)
        {
            //MapPlace place = placeMap.ShowMap.DrawElementList.FirstOrDefault(r => r.Key == OrgInfo.GUID).Value as MapPlace;
            //if (place == null)
            //{
            //    place = new MapPlace();
            //    place.BeforeDragPlaceEvent += OnBeforeDragPlace;
            //    //place.DeletePlaceEvent += place_DeletePlaceEvent;
            //}
            //place.ORGinfo = OrgInfo;
            //place.MapPoint = point;
            //return place;

            return null;
        }
        private void DrawORGToMap(MapPlace place, MapPointEx mappoint)
        {
            //placeMap.ShowMap.AddElement(place, placeMap.ShowMap.GetMapPointEx(mappoint.X, mappoint.Y));
        }
        private void RemoveOrgToMap(string id)
        {
            //placeMap.ShowMap.RemoveElement(id);
        }
        /// <summary>
        /// 添加详细日程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateScheduleDetail())
                return;
            ScheduleDetail newDetail = new ScheduleDetail();
            newDetail.GUID = Guid.NewGuid().ToString();
            newDetail.SCHEDULE_GUID = _scheduleInfo.GUID;
            newDetail.STARTTIME = DateTime.Now;
            newDetail.STOPTIME = DateTime.Now.AddDays(+1).AddHours(1);
            newDetail.TIMEDESC = "全天";
            newDetail.CONTENT = "";

            ScheduleDetail[] details = dg_ScheduleDetail.ItemsSource as ScheduleDetail[];
            List<ScheduleDetail> scheduleDetails = details.ToList();
            scheduleDetails.Add(newDetail);
            details = scheduleDetails.ToArray();
            dg_ScheduleDetail.ItemsSource = details;
            dg_ScheduleDetail.SelectedItem = newDetail;
        }
        /// <summary>
        /// 删除详细日程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Del_Click(object sender, RoutedEventArgs e)
        {
            ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
            if (selectItem == null)
                return;
            ScheduleDetail[] details = dg_ScheduleDetail.ItemsSource as ScheduleDetail[];
            List<ScheduleDetail> scheduleDetails = details.ToList();
            scheduleDetails.Remove(selectItem);
            clearAllSelectState();
            details = scheduleDetails.ToArray();
            dg_ScheduleDetail.ItemsSource = details;
            
        }
        private bool ValidatedScheduleInfo()
        {
            bool IsSuccess = true;
            StringBuilder strmsg = new StringBuilder();
            if (_scheduleInfo != null)
            {
                if (string.IsNullOrEmpty(_scheduleInfo.NAME))
                {
                    strmsg.Append("名称不能为空! \r");
                    IsSuccess = false;
                }
                if (!string.IsNullOrEmpty(_scheduleInfo.NAME) && _scheduleInfo.NAME.Length > 8)
                {
                    strmsg.Append("名称不能大于8个字! \r");
                    IsSuccess = false;
                }
                if (string.IsNullOrEmpty(_scheduleInfo.MEMO))
                {
                    strmsg.Append("说明不能为空! \r");
                    IsSuccess = false;
                }
                if (!string.IsNullOrEmpty(_scheduleInfo.MEMO) && _scheduleInfo.MEMO.Length > 100)
                {
                    strmsg.Append("说明不能大于100个字! \r");
                    IsSuccess = false;
                }
                if (_scheduleInfo.STARTTIME > _scheduleInfo.STOPTIME)
                {
                    strmsg.Append("日程开始时间不能小于结束时间! \r");
                    IsSuccess = false;
                }
                if (allSchedule != null && allSchedule.Length > 0)
                {
                    foreach (var item in allSchedule)
                    {
                        if (item.GUID == _scheduleInfo.GUID)
                            continue;
                        if ((_scheduleInfo.STARTTIME >= item.STARTTIME && _scheduleInfo.STARTTIME <= item.STOPTIME) 
                            || (_scheduleInfo.STOPTIME >= item.STARTTIME && _scheduleInfo.STOPTIME <= item.STOPTIME))
                        {
                            strmsg.Append("已有此时间段的日程信息! \r");
                            IsSuccess = false;
                            break;
                        }
                    }
                }
            }
            
            //string areas = getAreas();
            //if (string.IsNullOrEmpty(areas))
            //{
            //    strmsg.Append("请至少选择一个区域 \r");
            //    IsSuccess = false;
            //}
            if (!string.IsNullOrEmpty(strmsg.ToString()))
            {
                MessageBox.Show(strmsg.ToString(), "提示", MessageBoxButton.OK);
            }
            return IsSuccess;
        }

        private bool ValidateScheduleDetail()
        {
            //ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
            //if (selectItem != null)
            //{
            //    SaveAreasAndGroups(selectItem);
            //}
            bool IsSuccess = true;
            StringBuilder strmsg = new StringBuilder();
            ScheduleDetail[] details = dg_ScheduleDetail.ItemsSource as ScheduleDetail[];
            if (details == null)
                return true;
            foreach (var item in details)
            {
                if (string.IsNullOrEmpty(item.CONTENT))
                {
                    strmsg.Append("内容不能为空! \r");
                    IsSuccess = false;
                }
                if (!string.IsNullOrEmpty(item.CONTENT) && item.CONTENT.Length > 100)
                {
                    strmsg.Append("内容不能大于100个字! \r");
                    IsSuccess = false;
                }
                if (item.STARTTIME > item.STOPTIME)
                {
                    strmsg.Append("详细日程开始时间不能小于结束时间! \r");
                    IsSuccess = false;
                }

                if (item.STARTTIME < _scheduleInfo.STARTTIME)
                {
                    strmsg.Append("详细日程开始时间不能早于活动开始时间! \r");
                    IsSuccess = false;
                }
                if (item.STARTTIME > _scheduleInfo.STOPTIME)
                {
                    strmsg.Append("详细日程开始时间不能晚于活动结束时间! \r");
                    IsSuccess = false;
                }
                if (item.STOPTIME > _scheduleInfo.STOPTIME)
                {
                    strmsg.Append("详细日程结束时间不能晚于活动结束时间! \r");
                    IsSuccess = false;
                }
                if (item.STOPTIME < _scheduleInfo.STARTTIME)
                {
                    strmsg.Append("详细日程结束时间不能早于活动开始时间! \r");
                    IsSuccess = false;
                }
                if (item.ScheduleOrgs == null || item.ScheduleOrgs.Length == 0)
                {
                    strmsg.Append("必须选择区域和组织结构! \r");
                    IsSuccess = false;
                }
                else
                { 
                //校验地点组织是否重复
                    string groupid = "";
                    //foreach (ScheduleOrg org in item.ScheduleOrgs)
                    //{ 
                    //    if(groupid)
                    //}
                }
                
                if (!IsSuccess)
                {
                    dg_ScheduleDetail.SelectedItem = item;
                }
            }
            if (details.Length > 1)
            {
                #region 校验时间
                string star1 = details[details.Length-1].STARTTIME.ToString("yyyyMMdd");
                string star2 = details[details.Length-1].STOPTIME.ToString("yyyyMMdd");
                string star1_temp = "";
                string star2_temp = "";
                //string[] groups = details[details.Length - 1].GROUPS.Split(',');
                if (star1 == star2) //如果添加的项目是同一天的，那么只校验同一天的已有项
                {                    
                    for (int i = 0; i < details.Length-1; i++)
                    {
                        star1_temp = details[i].STARTTIME.ToString("yyyyMMdd");
                        star2_temp = details[i].STOPTIME.ToString("yyyyMMdd");
                        if (star1_temp == star2_temp&&star1_temp==star1)
                        {
                            if ((details[details.Length - 1].STARTTIME >= details[i].STARTTIME && details[details.Length - 1].STARTTIME <= details[i].STOPTIME) 
                                || (details[details.Length - 1].STOPTIME >= details[i].STARTTIME && details[details.Length - 1].STOPTIME <= details[i].STOPTIME))
                            {
                                strmsg.Append("已有此时间段的日程信息! \r");
                                IsSuccess = false;
                                break;
                            }
                           
                        }
                        //判断 组
                        //if (groups != null && groups.Length > 0)
                        //{
                        //    string[] groups_temp = details[i].GROUPS.Split(',');
                        //    if (groups_temp != null && groups_temp.Length > 0)
                        //    {
                        //        if (groups.Where(item => groups_temp.Contains(item)).Any())
                        //        {
                        //            strmsg.Append("选择的组在同一时间段内有重复! \r");
                        //            IsSuccess = false;
                        //            break;
                        //        }
                        //    }
                        //}


                    }
                }
                else
                {
                    for (int i = 0; i < details.Length-1; i++)
                    {
                        star1_temp = details[i].STARTTIME.ToString("yyyyMMdd");
                        star2_temp = details[i].STOPTIME.ToString("yyyyMMdd");
                        if (star1_temp != star2_temp)
                        {
                            if ((details[details.Length - 1].STARTTIME >= details[i].STARTTIME && details[details.Length - 1].STARTTIME <= details[i].STOPTIME)
                                || (details[details.Length - 1].STOPTIME >= details[i].STARTTIME && details[details.Length - 1].STOPTIME <= details[i].STOPTIME))
                            {
                                strmsg.Append("已有此时间段的日程信息! \r");
                                IsSuccess = false;
                                break;
                            }

                        }
                    }
                }
                #endregion

                #region 校验小组
                
               
                #endregion
            }
            if (!string.IsNullOrEmpty(strmsg.ToString()))
            {
                MessageBox.Show(strmsg.ToString(), "提示", MessageBoxButton.OK);
            }
            return IsSuccess;
        }
        /// <summary>
        /// 经纬度校验
        /// </summary>
        /// <param name="xy"></param>
        /// <returns></returns>
        private bool CheckCoordinate(double[] xy)
        {
            if (xy.Length != 2)
                return false;
            if (xy[0] >= 0 && xy[0] < 180)
                return true;
            if (xy[1] >= 0 || xy[1] < 90)
                return false;
            return false;
        }
        #endregion

        #region 开始、结束时间控制
        private void StopDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                DateTime dt;
                if (DateTime.TryParse(e.AddedItems[0].ToString(), out dt))
                {
                    DateTime.TryParse(dt.ToString("yyyy-MM-dd") + " 23:59:59", out dt);
                    if (dt < ActivityDateFrom || dt > ActivityDateTo)
                        dt = ActivityDateTo;
                    _scheduleInfo.STOPTIME = dt;
                }
            }
        }
        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems!=null&&e.AddedItems.Count>0)
            {
                DateTime dt;
                if (DateTime.TryParse(e.AddedItems[0].ToString(), out dt)) 
                {
                    DateTime.TryParse(dt.ToString("yyyy-MM-dd") + " 00:00:01", out dt);
                    if (dt < ActivityDateFrom || dt > ActivityDateTo)
                        dt = ActivityDateFrom;
                    _scheduleInfo.STARTTIME = dt;
                }
            }
        }
        #endregion

        #region UI控制
        private void splitText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (splitText.Text == "︿")
            {
                splitText.Text = "﹀";
                splitText.ToolTip = "点击展开";
                bottomGrid.Height = new GridLength(350,GridUnitType.Star);
                detailGrid.Height = new GridLength(250, GridUnitType.Pixel);
            }
            else
            {
                splitText.Text = "︿";
                splitText.ToolTip = "点击收回";
                bottomGrid.Height = new GridLength(0, GridUnitType.Pixel);
                detailGrid.Height = new GridLength(250, GridUnitType.Star);
                
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("确实要关闭此窗口吗", "关闭", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
            }
            else
                e.Cancel = true;
        }
        #endregion

        #region 拖动小组
        PP_OrgInfo dropInfo = null;
        private void OnBeforeDragPlace(PP_OrgInfo orginfo)
        {
            dropInfo = orginfo;
        }
        private void MainMap_Drop(object sender, DragEventArgs e)
        {
            //PP_OrgInfo OrgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;
            //if (dropInfo != null)
            //{
            //    Point scpoint = e.GetPosition(mapGrid);
            //    MapPointEx currentmappoint = placeMap.ShowMap.MainMap.ScreenToMap(new PointEx(scpoint.X, scpoint.Y));
            //    RemoveOrgToMap("org_" + dropInfo.GUID);
            //    DrawORGToMap(GetMapPlace(dropInfo, currentmappoint), currentmappoint);

            //    ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
            //    if (selectItem != null)
            //    {
            //        foreach (ScheduleOrg org in selectItem.ScheduleOrgs)
            //        {
            //            if (org.GROUP_GUID == dropInfo.GUID)
            //            {
            //                org.LONGITUDE = currentmappoint.X;
            //                org.LATITUDE = currentmappoint.Y;
            //            }
            //        } 
            //    }
            //    //Dictionary<string, double[]> groupsDic = tv_PersonPlan.Tag as Dictionary<string, double[]>;
            //    //if (groupsDic == null)
            //    //{
            //    //    groupsDic = new Dictionary<string, double[]>();
            //    //    tv_PersonPlan.Tag = groupsDic;
            //    //}
            //    //if (groupsDic.ContainsKey(dropInfo.GUID))//如果包含，
            //    //{
            //    //    groupsDic[dropInfo.GUID] = new double[] { currentmappoint.X, currentmappoint.Y};
            //    //}
            //}
        }
        #endregion

        /// <summary>
        /// 如果不勾选地点，则清空组织勾选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAreas_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceInfo placeinfo = listPlace.SelectedItem as ActivityPlaceInfo;
            {
                if (placeinfo == null || placeinfo.IsChecked == false)
                {
                    clearOrgSelectState();
                    ScheduleDetail selectItem = dg_ScheduleDetail.SelectedItem as ScheduleDetail;
                    if (selectItem != null && selectItem.ScheduleOrgs != null && selectItem.ScheduleOrgs.Length > 0)
                    {
                        List<ScheduleOrg> orgList = selectItem.ScheduleOrgs.ToList();
                        for (int i = 0; i < orgList.Count; i++)
                        {
                            if (orgList[i].AREA_GUID == placeinfo.Guid)
                            {                               
                                orgList.RemoveAt(i);
                                i--;
                            }                           
                        }
                        selectItem.ScheduleOrgs = orgList.ToArray();
                    }
                }
            }
        }

       
    }
    
}
