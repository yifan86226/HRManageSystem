#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：绘制监测站
 * 日 期 ：2017-06-30
 ***************************************************************#@#***************************************************************/
#endregion
using AT_BC.Common;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.MAP;
using CO_IA.UI.Screen.Control;
using I_CO_IA.PlanDatabase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CO_IA.UI.Screen
{
    public class LoadMonitorStation
    {
        FrameworkElement Element;
        public LoadMonitorStation(FrameworkElement element)
        {
            Element = element;
        
        }
        /// <summary>
        /// 监测站显示、清除
        /// </summary>
        /// <param name="b"></param>
        private void SetDoIt(bool b)
        {
            if (b)
            {
                Obj.screenMap.RemoveElementByFlag(MapGroupTypes.FixMonitorPoint_.ToString());
                new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(400);

                    Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        LoadData();
                    }));

                })) { IsBackground = true }.Start();
                
            }
            else
            {
                Obj.screenMap.RemoveElementByFlag(MapGroupTypes.FixMonitorPoint_.ToString());
            }
        }
        public void DrawIt()
        {
            Obj.screenMap.RemoveElementByFlag(MapGroupTypes.FixMonitorPoint_.ToString());
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(400);

                Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadData();
                }));

            })) { IsBackground = true }.Start();
        }
        public void Show(bool b)
        {
            Obj.screenMap.SetElementVisibilityByFlag(MapGroupTypes.FixMonitorPoint_.ToString(), b);
        }

        private void LoadData()
        {
            FixedStationQueryCondition condition = new FixedStationQueryCondition();
            ObservableCollection<FixedStationInfo> monitorList = QueryMonitorStationItemSource(condition);
            if (monitorList != null && monitorList.Count > 0)
            {
                ContextMenu contextMenu = GetContextMenu();
                foreach (FixedStationInfo info in monitorList)
                {
                    if (!Obj.screenMap.CheckCoordinate(new double[] { info.LONG == null ? 0 : info.LONG.Value, info.LAT == null ? 0 : info.LAT.Value }))
                        continue;
                    MonitorStation group = new MonitorStation(info);
                    group.ContextMenu = contextMenu;
                    Obj.screenMap.AddElement(group, Obj.screenMap.GetMapPointEx(info.LONG.Value, info.LAT.Value));

                }
            }
        }
        public ObservableCollection<FixedStationInfo> QueryMonitorStationItemSource(FixedStationQueryCondition querycondition)
        {
            try
            {
                List<FixedStationInfo> statInfos = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase, List<FixedStationInfo>>(channel =>
                {
                    return channel.SelectMonitorStation(querycondition);
                });
                if (statInfos == null) statInfos = new List<FixedStationInfo>();
                return new ObservableCollection<FixedStationInfo>(statInfos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
                return null;
            }
        }

        private ContextMenu GetContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            MenuItem item = null;

            item = new MenuItem();
            item.Header = "详细信息";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/22.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemInfo_Click;
            menu.Items.Add(item);
            return menu;
        }
        private void itemInfo_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.Parent as ContextMenu;
            var target = menu.PlacementTarget as MonitorStation;
            if (target != null)
            {
                FixedStationInfo stationInfo = target.StationInfo;
                Dialog.FixedStationDetailDialog detail = new Dialog.FixedStationDetailDialog(stationInfo);
                detail.Owner = VisualTreeHelperExtension.GetParentObject<System.Windows.Window>(Element); 
                detail.Show();
            }
        }
    }
}
