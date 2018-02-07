using CO_IA.Client;
using CO_IA.Data;
using I_GS_MapBase.Portal;
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.Screen.MainPage
{
    /// <summary>
    /// StateInfo.xaml 的交互逻辑
    /// </summary>
    public partial class StateInfo : UserControl
    {
        //监测小组
        Group.GroupList grouplist = new Group.GroupList();
        //活动简介
        Areas.ActivityInfo activityInfo;
        //区域简介
        Areas.AreaInfo areaInfo;

        /// <summary>
        /// 已完成任务
        /// </summary>
        Task.TaskedList taskedList = new Task.TaskedList();
        /// <summary>
        /// 未完成的任务
        /// </summary>
        Task.TaskingList taskingList = new Task.TaskingList();

        private ActivityPlaceInfo _placeInfo;
        public ActivityPlaceInfo PlaceInfo
        {
            get { return _placeInfo; }
            set {
                _placeInfo = value;

                #region 设置地图范围
                if (value.Name == "全部区域")
                {
                    Obj.screenMap.SetAllGraphicsExtent();
                }
                else
                {
                    if (string.IsNullOrEmpty(value.Graphics))
                    {
                        //Obj.screenMap.SetAllGraphicsExtent();
                        //获得地点
                        if (value.Locations != null && value.Locations.Length > 0)
                        {
                            List<string> ids = new List<string>();
                            foreach (var location in value.Locations)
                            {
                                ids.Add(MapGroupTypes.Location_.ToString()+location.GUID);
                            }
                            MapExtent extent = Obj.screenMap.GetGraphicExtentByID(ids.ToArray());
                            if (extent != null)
                                Obj.screenMap.setExtent(extent.Xy1, extent.Xy2, true);
                        }
                    }
                    else
                    {
                        QueryBehavior type = QueryBehavior.None;
                        List<MapPointEx> inputPoint = null;
                        inputPoint = Obj.screenMap.getPointsByGraphics(value.Graphics, out type);
                        MapExtent extent = Obj.screenMap.GetGraphicExtentByPoints(inputPoint.ToArray());
                        Obj.screenMap.setExtent(extent.Xy1, extent.Xy2, true);
                    }
                }
                
                #endregion

                SetState(value.Name == "全部区域" ? "活动简介" : "区域简介");

                Obj.taskData.Begin();
                
            }
        }
        private string tabFlag = "";
        public string TabFlag
        {
            get { return tabFlag; }
            set
            {
                SetState(value);
            }
        }
        public StateInfo()
        {
            InitializeComponent();

            this.Loaded += StateInfo_Loaded;

            //监测小组数量获取：一次性获取小组数量，再查看小组的时候更新小组数量
            //select * from ACTIVITY_PP_ORGINFO t where duty like '%05%' and activity_guid=''

            //完成任务数量获取：一次性获取，查看完成任务时更新

            //执行任务数量获取：一次性获取，查看执行任务时更新
        }

        void StateInfo_Loaded(object sender, RoutedEventArgs e)
        {
            //Obj.taskData.Start();
            Obj.taskData.Begin();
            this.DataContext = Obj.taskData;

            grouplist.IniData();
            txtorgCount.Text = grouplist.orgCount.ToString();
        }

        //public StateInfo(ActivityPlaceInfo placeinfo)
        //{
        //    InitializeComponent();
        //    placeinfo = placeinfo;
        //}

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            if (tb != null)
            {
                TabFlag = tb.Text;                
            }
        }

        private void SetState(string Flag)
        {
            if (tabFlag == Flag)
                return;
            tabFlag = Flag;
            if (string.IsNullOrEmpty(Flag))
                Flag = "组织结构";
            switch (Flag)
            {
                case "组织结构":
                    if (grouplist == null)
                    {
                        grouplist = new Group.GroupList();
                    }
                    
                    frame_window.Children.Clear();
                    frame_window.Children.Add(grouplist);
                    break;
                case "完成任务":
                    Obj.taskData.GetTaskData();
                    frame_window.Children.Clear();
                    frame_window.Children.Add(taskedList);
                    break;
                case "执行任务":
                    Obj.taskData.GetTaskData();
                    frame_window.Children.Clear();
                    frame_window.Children.Add(taskingList);
                    break;
                case "活动简介":
                    if (activityInfo == null)
                    {
                        activityInfo = new Areas.ActivityInfo();
                    }
                    activityInfo.IniData();
                    frame_window.Children.Clear();
                    frame_window.Children.Add(activityInfo);
                    break;
                case "区域简介":
                    if (areaInfo == null)
                    {
                        areaInfo = new Areas.AreaInfo();
                    }
                    areaInfo.IniData(_placeInfo);
                    frame_window.Children.Clear();
                    frame_window.Children.Add(areaInfo);
                    break;
            }
            
        }

        public void ChangeGroupState(string groupId, bool online)
        {
            grouplist.ChangeGroupState(groupId,online);
        }
    }
}
