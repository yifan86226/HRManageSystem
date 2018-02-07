#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：绘制参与单位设备
 * 日 期 ：2017-06-30
 ***************************************************************#@#***************************************************************/
#endregion
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.MAP;
using CO_IA.UI.Screen.Control;
using CO_IA.UI.Screen.Dialog;
using I_CO_IA.FreqStation;
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
    public class LoadOrgEqu
    {
        private void SetDoIt(bool b)
        {
            if (b)
            {
                Obj.screenMap.RemoveElementByFlag(MapGroupTypes.OrgEqu_.ToString());
                new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(500);

                    Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        LoadData();
                    }));

                })) { IsBackground = true }.Start();
            }
            else
            {
                Obj.screenMap.RemoveElementByFlag(MapGroupTypes.OrgEqu_.ToString());
            }
        }
        public void DrawIt()
        {
            Obj.screenMap.RemoveElementByFlag(MapGroupTypes.OrgEqu_.ToString());
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(500);

                Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadData();
                }));

            })) { IsBackground = true }.Start();
        }
        public void Show(bool b)
        {
            Obj.screenMap.SetElementVisibilityByFlag(MapGroupTypes.OrgEqu_.ToString(), b);
        }

        private void LoadData()
        {
            OrgQueryCondition condition = new OrgQueryCondition();
            condition.ActivityGuid = Obj.Activity.Guid;
            ActivityOrganization[] orgs = GetActivityOrgSources(condition);
            
            if (orgs != null && orgs.Length > 0)
            {
                if (Obj.ActivityPlaces != null && Obj.ActivityPlaces.Length > 0)
                {
                    ContextMenu contextMenu = GetContextMenu();
                    EquipmentLoadStrategy equcondition = new EquipmentLoadStrategy();
                   
                    foreach (var item in Obj.ActivityPlaces)
                    {
                        foreach (ActivityOrganization org in orgs)
                        {
                            equcondition.ActivityGuid = Obj.Activity.Guid;
                            equcondition.PlaceGuid = item.Guid;
                            equcondition.OrgName = org.Name;
                            ActivityEquipment[] equlist = GetActivityEquipments(equcondition);
                            if (equlist != null && equlist.Length > 0)
                            {
                                foreach (ActivityEquipment equ in equlist)
                                {
                                    if (!Obj.screenMap.CheckCoordinate(new double[] { equ.Longitude == null ? 0 : equ.Longitude.Value, equ.Latitude == null ? 0 : equ.Latitude.Value }))
                                        continue;
                                    OrgEqu orgequ = new OrgEqu(org, equ);
                                    orgequ.ContextMenu = contextMenu;
                                    Obj.screenMap.AddElement(orgequ, Obj.screenMap.GetMapPointEx(equ.Longitude.Value, equ.Latitude.Value));
                                }
                            }
                        }
                    }
                
                }
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
            var target = menu.PlacementTarget as OrgEqu;
            if (item != null)
            {
                ActivityOrganization orginfo = target.Orginfo;
                ActivityEquipment equinfo =target.Equinfo;

                ORGInfoDialog info = new ORGInfoDialog(orginfo,equinfo);
                info.ShowDialog();

            }
        }
       
        private ActivityOrganization[] GetActivityOrgSources(OrgQueryCondition condition)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityOrganization[]>(channel =>
            {
                return channel.GetActivityOrgs(condition);
            });
        }

        /// <summary>
        /// 查询设备
        /// </summary>
        /// <param name="condition"></param>
        private ActivityEquipment[] GetActivityEquipments(EquipmentLoadStrategy condition)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityEquipment[]>(channel =>
            {
                return channel.GetActivityEquipments(condition);
            });
           
           
        }
    }
   
}
