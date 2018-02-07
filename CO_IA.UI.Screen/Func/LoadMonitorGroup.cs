#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：绘制小组
 * 日 期 ：2017-06-30
 ***************************************************************#@#***************************************************************/
#endregion
using AT_BC.Common;
using CO_IA.Client;
using CO_IA.Client.Orgs;
using CO_IA.Data;
using CO_IA.UI.MAP;
using CO_IA.UI.Screen.Control;
using CO_IA.UI.Screen.Dialog;
using I_CO_IA.MessageCenter;
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CO_IA.UI.Screen
{
    public class LoadMonitorGroup
    {
        FrameworkElement Element;
        public LoadMonitorGroup(FrameworkElement element)
        {
            Element = element;
        }
        /// <summary>
        /// 监测小组显示、清除
        /// </summary>
        /// <param name="b"></param>
        private void SetDoIt(bool b)
        {
            if (b)
            {
                Obj.screenMap.RemoveElementByFlag(MapGroupTypes.MonitorGroup_.ToString());
                new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(400);

                    Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        LoadData();
                    }));

                })) {  IsBackground=true}.Start();                
            }
            else
            {
                Obj.screenMap.RemoveElementByFlag(MapGroupTypes.MonitorGroup_.ToString());
            }
        }
        public void DrawIt()
        {
            Obj.screenMap.RemoveElementByFlag(MapGroupTypes.MonitorGroup_.ToString());
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(400);

                Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadData();

                }));
                Thread.Sleep(400);

                Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Obj.SetGroupAllStateToMap(false);

                    ChangeGroupState();

                }));

            })).Start();   
        }
        public void Show(bool b)
        {
            Obj.screenMap.SetElementVisibilityByFlag(MapGroupTypes.MonitorGroup_.ToString(),b);
        }
        public void LoadData()
        {
            
            ToMapHelper.DrawOrgsToMap(Obj.screenMap, Obj.Activity.Guid, Obj.Activity.ActivityStage,CreateContextMenu());

            Obj.gpsData.Element = Element;
            Obj.gpsData.Start();
        }
        
        public ContextMenu CreateContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            menu.Opened += menu_Opened;
            MenuItem item = null;

            //陕西去掉此功能
            item = new MenuItem();
            //item.Tag = orgInfo;
            item.Header = "语音通话";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/voice.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemVoice_Click;
            //item.Visibility = Visibility.Collapsed;
            //menu.Items.Add(item);

            item = new MenuItem();
            //item.Tag = orgInfo;
            item.Header = "查看监测组信息";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/22.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemInfo_Click;
            menu.Items.Add(item);

            item = new MenuItem();
            //item.Tag = orgInfo;
            item.Header = "查看任务";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/66.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemTask_Click;
            menu.Items.Add(item);

            item = new MenuItem();
            item.Header = "现场监测情况";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/55.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemLIVEMonitor_Click;
            menu.Items.Add(item);

            item = new MenuItem();
            //item.Tag = orgInfo;
            item.Header = "查看移动轨迹";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/33.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemTrack_Click;
            menu.Items.Add(item);

            item = new MenuItem();
            //item.Tag = orgInfo;
            item.Header = "远程监控";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/44.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemRemoteWatch_Click;
            item.Visibility = Visibility.Collapsed;
            //menu.Items.Add(item);//广东去掉

            return menu;
        }

        void menu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            var target = menu.PlacementTarget as OrgToMapStyle;
            if (target != null)
            {
                ClientInfo clientinfo = target.clientInfo;
                if (clientinfo == null || string.IsNullOrEmpty(clientinfo.ClientAddress))
                {
                    SetContextMenu(menu, false);
                }
                else
                {
                    SetContextMenu(menu, true);
                }
            }
        }

        /// <summary>
        /// 设置远程监控功能是否可见
        /// </summary>
        /// <param name="online"></param>
        public void SetContextMenu(ContextMenu menu,bool online)
        {
            MenuItem menuitem = null;
            if (menu != null && menu.Items.Count > 0)
            {
                for (int i = 0; i < menu.Items.Count; i++)
                {
                    MenuItem item = menu.Items[i] as MenuItem;
                    if (item != null && item.Header.ToString() == "远程监控")
                    {
                        menuitem = item;
                    }
                }
            }
            if (menuitem != null)
            {
                menuitem.Visibility = online ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        /// <summary>
        /// 语音通话
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemVoice_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.Parent as ContextMenu;
            var target = menu.PlacementTarget as OrgToMapStyle;
            if (target != null)
            {
                PP_OrgInfo orgInfo = target.OrgInfo;
                Audio.AudioDialog p = new Audio.AudioDialog(orgInfo);
                p.Show();
                
            }
        }
        /// <summary>
        /// 查看检测组信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemInfo_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.Parent as ContextMenu;
            var target = menu.PlacementTarget as OrgToMapStyle;
            if (target != null)
            {
                PP_OrgInfo orgInfo = target.OrgInfo;
                Group.GroupDialog groupDialog = new Group.GroupDialog(new List<PP_OrgInfo>(){ orgInfo});
                groupDialog.ShowDialog(Element);
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
            ContextMenu menu = item.Parent as ContextMenu;
            var target = menu.PlacementTarget as OrgToMapStyle;
            if (target != null)
            {
                PP_OrgInfo orgInfo = target.OrgInfo;

                Task.TaskAllList taskList = new Task.TaskAllList(new string[]{orgInfo.GUID});
                taskList.ShowDialog(Element);
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
            ContextMenu menu = item.Parent as ContextMenu;
            var target = menu.PlacementTarget as OrgToMapStyle;
            if (target != null)
            {
                PP_OrgInfo orgInfo = target.OrgInfo;

                ScheduleDetail[] scheduleDetails = Utility.getOrgGroupsBySchedule(Obj.Activity.Guid, Obj.Activity.ActivityStage);
                var obj = scheduleDetails.Where(itm => {
                    if (itm.ScheduleOrgs != null && itm.ScheduleOrgs.Length>0 && itm.ScheduleOrgs[0].OrgInfo.GUID == orgInfo.GUID)
                        return true;
                    else
                        return false;
                }).ToArray();
                if (obj != null && obj.Length == 1)
                {
                    string areaid = obj[0].ScheduleOrgs[0].AREA_GUID;
                    Monitor.MonitorView rh = new Monitor.MonitorView(areaid, new List<PP_OrgInfo> { orgInfo });
                    rh.Title = "现场监测情况";
                    rh.ShowDialog();
                }

                   
                

            }
        }
        /// <summary>
        /// 查看移动轨迹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemTrack_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.Parent as ContextMenu;
            var target = menu.PlacementTarget as OrgToMapStyle;
            if (target != null)
            {
                PP_OrgInfo orgInfo = target.OrgInfo;
                //先查有没有车，或者车牌
                PP_VehicleInfo itemVehicle=null;
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    itemVehicle = channel.GetPP_VehicleInfo(orgInfo.GUID);
                });
                if (itemVehicle != null && !string.IsNullOrEmpty(itemVehicle.VEHICLE_NUMB))
                {
                    OrgToMapStyle group = new OrgToMapStyle(orgInfo);
                    Track.TrackCondition conTrack = new Track.TrackCondition(group);
                    conTrack.VehicleNum = itemVehicle.VEHICLE_NUMB;
                    conTrack.Owner = VisualTreeHelperExtension.GetParentObject<System.Windows.Window>(Element); 
                    conTrack.Show();
                }
                else
                {
                    MessageBox.Show("没有查询到车辆信息！");
                }
                
            }
        }

        private void itemRemoteWatch_Click(object sender, RoutedEventArgs e)
        { 
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.Parent as ContextMenu;
            var target = menu.PlacementTarget as OrgToMapStyle;
            if (target != null)
            {
                PP_OrgInfo orgInfo = target.OrgInfo;
                ClientInfo clientinfo = target.clientInfo;
                if (!string.IsNullOrEmpty(clientinfo.ClientAddress))
                {
                    RemoteWatch watch = new RemoteWatch(clientinfo.ClientAddress.Substring(0,clientinfo.ClientAddress.IndexOf(":")));
                    watch.org = target;
                    watch.Show();
                }
            }
        }


        #region 获取现场端是否在线
        /// <summary>
        /// 一次性获取在线客户端状态，进行地图与导航条状态的改变
        /// </summary>
        public void ChangeGroupState()
        {
            //SetGroupStateToGroup("1", true);
            ClientInfo[] Clients = GetSceneState();
            if (Clients == null || Clients.Length == 0)
                return;
            foreach (ClientInfo info in Clients)
            {
                if (info.ClientType == ClientType.SceneManage)
                {
                    //修改地图监测组图标
                    Obj.SetGroupStateToMap(info, true);

                }
            }
        }
        /// <summary>
        /// 获取现场端是否在线
        /// </summary>
        private ClientInfo[] GetSceneState()
        {
            ClientInfo[] clients = null;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MessageCenter.I_CO_IA_MessageCenter>
             (channel =>
             {
                 clients = channel.GetOnlineSceneClient(Obj.Activity.Guid);
             });
            return clients;
        }
        
        #endregion
    }
}
