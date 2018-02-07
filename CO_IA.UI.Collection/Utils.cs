#region 文件描述
/*************************************************************************
 * 创建人：王若兴
 * 摘  要：SQLite数据库连接和操作
 * 日  期：2016-09-08
 * ***********************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using CO_IA.Data;

namespace CO_IA.UI.Collection
{
    public class LoginService
    {
        private static Activity _currentActivity = new Activity();
        private static ActivityPlace _currentActivityPlace = new ActivityPlace();
        private static bool _isLogin = true;
        public static event Action<ActivityPlace> ActivityPlaceChanged;
        /// <summary>
        /// 当前操作活动
        /// </summary>
        public static Activity CurrentActivity
        {
            get { return LoginService._currentActivity; }
            set { LoginService._currentActivity = value; }
        }
        /// <summary>
        /// 当前选择的地点
        /// </summary>
        public static ActivityPlace CurrentActivityPlace
        {
            get { return LoginService._currentActivityPlace; }
            set 
            { 
                LoginService._currentActivityPlace = value;
                if (ActivityPlaceChanged != null)
                {
                    ActivityPlaceChanged(value);
                }
            }
        }
        /// <summary>
        /// 登录状态
        /// </summary>
        public static bool IsLogin
        {
            get { return LoginService._isLogin; }
            set { LoginService._isLogin = value; }
        }

        private static PP_OrgInfo _userOrgInfo;
        /// <summary>
        /// 当前用户组织机构
        /// </summary>
        public static PP_OrgInfo UserOrgInfo
        {
            get { return LoginService._userOrgInfo; }
            set { LoginService._userOrgInfo = value; }
        }
    }
    //    /// <summary>
    //    /// 活动所处阶段
    //    /// </summary>
    //    private static ActivityStage currentActivityStage = ActivityStage.None;
    //    /// <summary>
    //    /// 活动所处阶段
    //    /// </summary>
    //    public static ActivityStage CurrentActivityStage
    //    {
    //        get { return currentActivityStage; }
    //        set
    //        {
    //            currentActivityStage = value;
    //            SetCurrentState();
    //        }
    //    }
    //    private static string _currentSite;
    //    /// <summary>
    //    /// 当前选中地点
    //    /// </summary>
    //    public static string CurrentSite
    //    {
    //        set
    //        {
    //            if (string.IsNullOrEmpty(value))
    //            {
    //                Utils.siteTile.Visibility = System.Windows.Visibility.Hidden;
    //                Utils.siteMenu.Visibility = System.Windows.Visibility.Hidden;

    //                Utils.siteTips.Visibility = System.Windows.Visibility.Hidden;
    //            }


    //        }
    //    }
    //    ///// <summary>
    //    ///// 主窗口右侧
    //    ///// </summary>
    //    //public static RightContent RContent = null;
    //    ///// <summary>
    //    ///// 窗口管理
    //    ///// </summary>
    //    //public static WindowList winList = null;
    //    //public static MapMenu mapMenu = new MapMenu();
    //    ///// <summary>
    //    ///// 图层管理
    //    ///// </summary>
    //    ////public static LayerList layerList = new LayerList();
    //    ///// <summary>
    //    ///// 主地图
    //    ///// </summary>
    //    //public static MapFunPortal MapFun = new MapFunPortal();
    //    ////任务
    //    //public static TaskInfo taskInfo = null;

    //    //public static Site.SiteTitle siteTile = null;
    //    //public static UControl.SiteTips siteTips = null;
    //    //public static Site.SiteMenu siteMenu = null;
    //    //public static Site.SiteDetail sitDetail = null;
    //    //public static Site.SiteList siteList = null;
    //    //public static UControl.Loading loading = null;
    //    ///// <summary>
    //    ///// 右侧功能区宽度
    //    ///// </summary>
    //    //public static double RightContentWidth = 300;

    //    //#region 界面显示控制
    //    ///// <summary>
    //    ///// 右侧功能区显示控制
    //    ///// </summary>
    //    //public static void SetCurrentState()
    //    //{
    //    //    if (currentActivityStage == ActivityStage.Prepare)
    //    //    {
    //    //        if (RContent != null)
    //    //            SetRightVisible(false);
    //    //        if (siteMenu != null)
    //    //            siteMenu.setState(false);
    //    //    }
    //    //    if (currentActivityStage == ActivityStage.Execute)
    //    //    {
    //    //        if (RContent != null)
    //    //            SetRightVisible(true);
    //    //        if (siteMenu != null)
    //    //            siteMenu.setState(true);
    //    //    }
    //    //    CurrentSite = "";
    //    //    if (Utils.siteList != null)
    //    //        //Utils.siteList.DrawSiteList();
    //    //        if (Utils.sitDetail != null)
    //    //            Utils.sitDetail.ChangeData();

    //    //}
    //    ///// <summary>
    //    ///// 控制右侧功能区可见性
    //    ///// </summary>
    //    ///// <param name="v"></param>
    //    //public static void SetRightVisible(bool v)
    //    //{
    //    //    if (RContent == null)
    //    //        return;
    //    //    var obj = RContent.Parent;
    //    //    Grid g = obj as Grid;


    //    //    if (v)
    //    //    {
    //    //        RContent.Visibility = Visibility.Visible;

    //    //        MapFun.SetOverviewAndZoomLine(300);
    //    //        if (g != null)
    //    //            g.Width = 300;
    //    //    }
    //    //    else
    //    //    {
    //    //        RContent.Visibility = Visibility.Hidden;
    //    //        MapFun.SetOverviewAndZoomLine(-300);
    //    //        if (g != null)
    //    //            g.Width = 0;
    //    //    }
    //    //}
    //    //public static void setRight(string flag)
    //    //{
    //    //    if (RContent.Visibility != Visibility.Visible)
    //    //        return;
    //    //    if (flag == "0")//开
    //    //    {
    //    //        PublicFun.ExecStorybordX(Utils.RContent, 300, 0, 0.7);
    //    //        PublicFun.ExecStorybord(Utils.RContent, FrameworkElement.OpacityProperty, 0, 1, 500);
    //    //        PublicFun.ExecStorybordX(Utils.MapFun.MainMap.OverViewElement, 300, 0, 0.7);
    //    //        PublicFun.ExecStorybordX(Utils.MapFun.MainMap.ZoomLineElement, 300, 0, 0.7);
    //    //    }
    //    //    if (flag == "1")//关
    //    //    {
    //    //        PublicFun.ExecStorybordX(Utils.RContent, 0, 300, 0.7);
    //    //        PublicFun.ExecStorybord(Utils.RContent, FrameworkElement.OpacityProperty, 1, 0, 500);
    //    //        PublicFun.ExecStorybordX(Utils.MapFun.MainMap.OverViewElement, 0, 300, 0.7);
    //    //        PublicFun.ExecStorybordX(Utils.MapFun.MainMap.ZoomLineElement, 0, 300, 0.7);

    //    //    }
    //    //}
    //    //#endregion


    //    ///// <summary>
    //    ///// 监测组右键
    //    ///// </summary>
    //    ///// <param name="info"></param>
    //    ///// <returns></returns>
    //    //public static CirclePanel SetButtonInfo(object info)
    //    //{
    //    //    string elementid = "p1";
    //    //    MonitorGroup iteminfo = info as MonitorGroup;

    //    //    if (iteminfo == null)
    //    //    {
    //    //        string[] s = info as string[];
    //    //        if (s != null)
    //    //        {
    //    //            string stype = "";
    //    //            if (s[0].StartsWith("group_"))
    //    //                stype = s[0].Replace("group_", "");
    //    //            iteminfo = new MonitorGroup() { Id = s[0], Name = s[1], Type = stype };
    //    //        }
    //    //    }
    //    //    if (iteminfo != null) elementid = iteminfo.Name;
    //    //    CirclePanel circlePanel = new CirclePanel(elementid, null);

    //    //    circlePanel.SetButtonInformation(new BtnInfo() { Id = elementid, bType = 2, bText = "任务信息", bUri = new Uri("/CO_IA.UI.Display;component/Images/White/cdbdjj.png", 0), bTag = iteminfo }, mapMenu.OnCircleMenuButtonClick);
    //    //    if (iteminfo.Type != "3")
    //    //    {
    //    //        circlePanel.SetButtonInformation(new BtnInfo() { Id = elementid, bType = 3, bText = "查看监测组信息", bUri = new Uri("/CO_IA.UI.Display;component/Images/White/cb.png", 0), bTag = iteminfo }, mapMenu.OnCircleMenuButtonClick);
    //    //        circlePanel.SetButtonInformation(new BtnInfo() { Id = elementid, bType = 4, bText = "现场监测情况", bUri = new Uri("/CO_IA.UI.Display;component/Images/White/dqz.png", 0), bTag = iteminfo }, mapMenu.OnCircleMenuButtonClick);
    //    //    }
    //    //    if (iteminfo.Type == "1")
    //    //        circlePanel.SetButtonInformation(new BtnInfo() { Id = elementid, bType = 5, bText = "查看移动轨迹", bUri = new Uri("/CO_IA.UI.Display;component/Images/White/30mhyxhk.png", 0), bTag = iteminfo }, mapMenu.OnCircleMenuButtonClick);

    //    //    return circlePanel;
    //    //}
    //    ///// <summary>
    //    ///// 监测站右键
    //    ///// </summary>
    //    ///// <param name="info"></param>
    //    ///// <returns></returns>
    //    //public static CirclePanel SetButtonInfo_Monitor(object info)
    //    //{
    //    //    //string[] s = info as string[];
    //    //    string elementid = "123";
    //    //    FixedStationInfo iteminfo = info as FixedStationInfo;
    //    //    if (iteminfo != null) elementid = iteminfo.Name;
    //    //    CirclePanel circlePanel = new CirclePanel(elementid, null);

    //    //    circlePanel.SetButtonInformation(new BtnInfo() { Id = elementid, bType = 2, bText = "监测站详细信息", bUri = new Uri("/CO_IA.UI.Display;component/Images/White/场强24.png", 0), bTag = iteminfo }, mapMenu.OnCircleMenuButtonClick);
    //    //    return circlePanel;
    //    //}

    //    ///// <summary>
    //    ///// 创建右键菜单
    //    ///// </summary>
    //    ///// <returns></returns>
    //    //public static ContextMenu GetContextMenu()
    //    //{
    //    //    ContextMenu menu = new ContextMenu();
    //    //    MenuItem item = null;


    //    //    item = new MenuItem();
    //    //    item.Header = "地点信息查看";
    //    //    item.Click += Utils.mapMenu.MenuItem_Click;
    //    //    menu.Items.Add(item);

    //    //    item = new MenuItem();
    //    //    item.Header = "所驻人员信息";
    //    //    item.Click += Utils.mapMenu.MenuItem_Click;
    //    //    menu.Items.Add(item);


    //    //    item = new MenuItem();
    //    //    item.Header = "周围监测设施";
    //    //    item.Click += Utils.mapMenu.MenuItem_Click;
    //    //    menu.Items.Add(item);

    //    //    item = new MenuItem();
    //    //    item.Header = "周围台站查询";
    //    //    item.Click += Utils.mapMenu.MenuItem_Click;
    //    //    menu.Items.Add(item);

    //    //    if (Utils.currentActivityStage != 0)
    //    //    {
    //    //        item = new MenuItem();
    //    //        item.Header = "现场监测情况";
    //    //        item.Click += Utils.mapMenu.MenuItem_Click;
    //    //        menu.Items.Add(item);


    //    //        item = new MenuItem();
    //    //        item.Header = "任务信息";
    //    //        item.Click += Utils.mapMenu.MenuItem_Click;
    //    //        menu.Items.Add(item);
    //    //    }

    //    //    item = new MenuItem();
    //    //    item.Header = "历史监测记录";
    //    //    item.Click += Utils.mapMenu.MenuItem_Click;
    //    //    menu.Items.Add(item);
    //    //    return menu;
    //    //}
    //    ///// <summary>
    //    ///// 模态弹出窗口
    //    ///// </summary>
    //    ///// <param name="title">标题</param>
    //    ///// <param name="root">内嵌页面</param>
    //    ///// <param name="width">窗口宽度</param>
    //    ///// <param name="height">窗口高度</param>
    //    ///// <param name="top">窗口位置X</param>
    //    ///// <param name="left">窗口位置Y</param>
    //    ///// <param name="owner">窗口 Owner</param>
    //    ///// <param name="showTaskbar">是否在任务栏显示</param>
    //    ///// <returns></returns>
    //    //public static bool? ShowDialogWindow(string title, FrameworkElement root, double width, double height, double top = -1, double left = -1, Window owner = null, bool showTaskbar = false)
    //    //{
    //    //    PopPanel pop = new PopPanel();
    //    //    WindowStartupLocation LocationStart = WindowStartupLocation.CenterOwner;
    //    //    if (owner == null)
    //    //    {
    //    //        pop.Owner = Application.Current.MainWindow;

    //    //    }
    //    //    else
    //    //        pop.Owner = owner;
    //    //    pop.ShowInTaskbar = showTaskbar;

    //    //    pop.Width = width + 10; pop.Height = height + 10;
    //    //    pop.titleName = title;
    //    //    if (root != null)
    //    //    {
    //    //        pop.URL = root;
    //    //        root.Tag = pop;
    //    //    }

    //    //    if (top >= 0 && left >= 0)
    //    //    {
    //    //        pop.WindowStartupLocation = WindowStartupLocation.Manual;
    //    //        pop.Top = top;
    //    //        pop.Left = left;
    //    //    }
    //    //    else
    //    //    {
    //    //        pop.WindowStartupLocation = LocationStart;
    //    //    }


    //    //    return pop.ShowDialog();
    //    //}
    //    ///// <summary>
    //    ///// 弹出窗口
    //    ///// </summary>
    //    ///// <param name="title">标题</param>
    //    ///// <param name="root">内嵌页面</param>
    //    ///// <param name="width">窗口宽度</param>
    //    ///// <param name="height">窗口高度</param>
    //    ///// <param name="top">窗口位置X</param>
    //    ///// <param name="left">窗口位置Y</param>
    //    ///// <param name="owner">窗口 Owner</param>
    //    ///// <param name="showTaskbar">是否在任务栏显示</param>
    //    //public static void ShowWindow(string title, FrameworkElement root, double width, double height, double top = -1, double left = -1, Window ower = null, bool showTaskbar = false)
    //    //{
    //    //    PopPanel pop = new PopPanel();
    //    //    WindowStartupLocation LocationStart = WindowStartupLocation.CenterOwner;
    //    //    if (ower == null)
    //    //    {
    //    //        pop.Owner = Application.Current.MainWindow;

    //    //    }
    //    //    else
    //    //        pop.Owner = ower;
    //    //    pop.ShowInTaskbar = showTaskbar;

    //    //    pop.Width = width + 10; pop.Height = height + 10;
    //    //    pop.titleName = title;
    //    //    if (root != null)
    //    //    {
    //    //        pop.URL = root;
    //    //        root.Tag = pop;
    //    //    }
    //    //    if (top >= 0 && left >= 0)
    //    //    {
    //    //        pop.WindowStartupLocation = WindowStartupLocation.Manual;
    //    //        pop.Top = top;
    //    //        pop.Left = left;
    //    //    }
    //    //    else
    //    //    {
    //    //        pop.WindowStartupLocation = LocationStart;
    //    //    }


    //    //    pop.Show();
    //    //}
    //    //public static void ClosePopPanel(string titleName)
    //    //{
    //    //    winList.Sub(titleName);
    //    //}


    //}
    //public class thisServer
    //{
    //    private static CO_IA.Data.Activity _activity;
    //    /// <summary>
    //    /// 当前操作的活动
    //    /// </summary>
    //    public static CO_IA.Data.Activity Activity
    //    {
    //        get
    //        {
    //            return _activity;
    //        }
    //        set
    //        {
    //            _activity = value;
    //            if (value != null)
    //            {
    //                //Utils.CurrentActivityStage = value.ActivityStage;
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 获取活动地点列表
    //    /// </summary>
    //    /// <param name="_activityGuid">活动guid</param>
    //    /// <returns></returns>
    //    public static CO_IA.Data.ActivityPlaceInfo[] GetPlacesByActivityId(string _activityGuid)
    //    {
    //        return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceInfo[]>(
    //            channel =>
    //            {
    //                return channel.GetPlaceInfosByActivityId(_activityGuid);
    //            });
    //    }
    //}
}
