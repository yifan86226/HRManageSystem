using AT_BC.Common;
using CO_IA.Client;
using CO_IA.Client.Orgs;
using CO_IA.Data;
using CO_IA.Data.Gps;
using CO_IA.UI.Screen.Control.Point;
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
    public class LoadGPSData
    {
        Dictionary<string, string> idDic = new Dictionary<string, string>();//车或者人ID,组ID
        Dictionary<string, object> ObjDic = new Dictionary<string, object>();//对象ID，对象
        public FrameworkElement Element;
       
        public void Start()
        {
            idDic.Clear();
            ObjDic.Clear();
            Obj.groupGPSThread = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(2000);
                List<KeyValuePair<string, object>> groupList = GetGroupsByMap();//组
                if (groupList == null || groupList.Count == 0)
                    return;

                //根据组找人、车
                if (!LoadDic(groupList))
                    return;

                List<string> ids = new List<string>();
                foreach (var item in idDic)
                {
                    ids.Add(item.Key);
                }
                while (true)
                {
                    Dictionary<string, Point> dic = LoadData(ids);//对象ID 
                    if (dic != null && dic.Count > 0)
                    {                        
                        foreach (var item in dic)
                        {
                            if (ObjDic.ContainsKey(item.Key))
                            {
                                if (ObjDic[item.Key] is PP_VehicleInfo)
                                {
                                    //根据对象ID找组
                                    UpdateVehicle(item);
                                }
                                if (ObjDic[item.Key] is PP_PersonInfo)
                                {
                                    UpdatePerson(item);
                                }
                            }
                        }                        
                    }
                    Thread.Sleep(50*1000);
                }
            })) { IsBackground = true };
            Obj.groupGPSThread.Start();
        }
        private List<KeyValuePair<string, object>> GetGroupsByMap()
        {
            return Obj.screenMap.DrawElementList.Where(item => item.Key.StartsWith(MapGroupTypes.MonitorGroup_.ToString())).ToList<KeyValuePair<string, object>>();
        }
        private bool LoadDic(List<KeyValuePair<string, object>> groupList)
        {
            string groupid = "";
            foreach (var group in groupList)
            {
                groupid = group.Key.Replace(MapGroupTypes.MonitorGroup_.ToString(), "");
                PP_VehicleInfo vehicle = GetVehicleByGroupId(groupid);
                if (vehicle != null && !string.IsNullOrEmpty(vehicle.VEHICLE_NUMB))
                {
                    if (!idDic.ContainsKey(vehicle.VEHICLE_NUMB))
                        idDic.Add(vehicle.VEHICLE_NUMB, groupid);
                    if (!ObjDic.ContainsKey(vehicle.VEHICLE_NUMB))
                        ObjDic.Add(vehicle.VEHICLE_NUMB, vehicle);
                }
                List<PP_PersonInfo> persons = GetPersonByGroupID(groupid);
                if (persons != null && persons.Count > 0)
                {
                    foreach (var person in persons)
                    {
                        if (!idDic.ContainsKey(person.GUID))
                            idDic.Add(person.GUID, groupid);
                        if (!ObjDic.ContainsKey(person.GUID))
                            ObjDic.Add(person.GUID, person);
                    }
                }
            }
            if (idDic.Count == 0)
                return false;
            return true;
        }

        private void UpdateVehicle(KeyValuePair<string, Point> item)
        {
            if (idDic.ContainsKey(item.Key))
            {
                string groupid = idDic[item.Key];
                var group = Obj.screenMap.DrawElementList.Where(o => MapGroupTypes.MonitorGroup_.ToString() + groupid == o.Key).ToArray();
                if (group != null && group.Length == 1)
                {
                    Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        OrgToMapStyle orgPoint = group[0].Value as OrgToMapStyle;
                        if (orgPoint != null)
                        {
                            if (orgPoint.ElementTag != null && orgPoint.ElementTag is MapPointEx)
                            {
                                MapPointEx p = orgPoint.ElementTag as MapPointEx;
                                if (p.X != item.Value.X || p.Y != item.Value.Y)
                                {
                                
                                       //移动位置
                                       Obj.screenMap.RemoveElement(MapGroupTypes.MonitorGroup_.ToString() + groupid);
                                       Obj.screenMap.AddElement(orgPoint, Obj.screenMap.GetMapPointEx(item.Value.X, item.Value.Y));
                              
                                }
                            }
                        }
                    }));
                }
            }
        }
        private void UpdatePerson(KeyValuePair<string, Point> item)
        {
            //根据对象ID找组
            if (idDic.ContainsKey(item.Key))
            {
                string groupid = idDic[item.Key];
                var person = Obj.screenMap.DrawElementList.Where(o => MapGroupTypes.Person_.ToString() + item.Key == o.Key).ToArray();
                if (person != null && person.Length == 1)
                {
                    Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //修改位置
                        PersonPoint personPoint = person[0].Value as PersonPoint;
                        if (personPoint != null)
                        {
                            if (personPoint.ElementTag != null && personPoint.ElementTag is MapPointEx)
                            {
                                MapPointEx p = personPoint.ElementTag as MapPointEx;
                                if (p.X != item.Value.X || p.Y != item.Value.Y)
                                {
                                    //移动位置
                                    Obj.screenMap.RemoveElement(MapGroupTypes.Person_.ToString() + item.Key);
                                    Obj.screenMap.AddElement(personPoint, Obj.screenMap.GetMapPointEx(item.Value.X, item.Value.Y));
                                }
                            }
                        }
                    }));
                }
                else
                {
                    //添加
                    var group = Obj.screenMap.DrawElementList.Where(o => MapGroupTypes.MonitorGroup_.ToString() + groupid == o.Key).ToArray();
                    if (group != null && group.Length == 1)
                    {
                        Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            OrgToMapStyle orgPoint = group[0].Value as OrgToMapStyle;
                            PP_OrgInfo orginfo = orgPoint.OrgInfo;
                            PersonPoint personPoint = new PersonPoint(orginfo, ObjDic[item.Key] as PP_PersonInfo);
                            personPoint.ContextMenu = GetContextMenu();
                            Obj.screenMap.AddElement(personPoint, Obj.screenMap.GetMapPointEx(item.Value.X, item.Value.Y));
                        }));
                    }
                         
                }
            }
        }
        /// <summary>
        /// 获取id的gps信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private Dictionary<string, Point> LoadData(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
                return null;
            string vehicle_Num = "";
            foreach (string s in ids)
            {
                vehicle_Num += "'" + s + "',";
            }
            vehicle_Num = vehicle_Num.TrimEnd(',') ;

            Dictionary<string, Point> dic = new Dictionary<string, Point>();

            GpsOrbitFilter filter = new GpsOrbitFilter();
            filter.ActivityId = Obj.Activity.Guid;
            filter.PlateNumber = vehicle_Num;
            List<GpsOrbit> gpsInfo = GetGPSData(filter);
            if (gpsInfo != null && gpsInfo.Count > 0)
            {
                foreach (var item in gpsInfo)
                {
                    MapUtiles.PointEx px= Obj.clientUtile.Correct(item.Latitude,item.Longitude);
                    dic.Add(item.PlateNumber, new Point(px.X,px.Y));
                }
            }

            return dic;
        }


        private List<PP_PersonInfo> GetPersonByGroupID(string groupId)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule, List<PP_PersonInfo>>(channel =>
                    {
                        return channel.GetPP_PersonInfos(groupId);
                    });
        }
        private PP_VehicleInfo GetVehicleByGroupId(string groupId)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule, PP_VehicleInfo>(channel =>
            {
                return channel.GetPP_VehicleInfo(groupId);
            });
        }

        private List<GpsOrbit> GetGPSData(GpsOrbitFilter filter)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Gps.I_CO_IA_Gps, List<GpsOrbit>>(channel =>
                {
                    return channel.QueryCurrentGpsOrbit(filter);
                });
        }

        private ContextMenu GetContextMenu()
        {
            ContextMenu menu = new ContextMenu();
            MenuItem item = null;

            item = new MenuItem();
            item.Header = "查看组及人员信息";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/22.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemInfo_Click;
            menu.Items.Add(item);

            item = new MenuItem();
            item.Header = "查看移动轨迹";
            item.Icon = new Image() { Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/ContextMenu/22.png", UriKind.RelativeOrAbsolute)) };
            item.Click += itemTrack_Click;
            menu.Items.Add(item);
            return menu;
        }
        private void itemInfo_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.Parent as ContextMenu;
            var target = menu.PlacementTarget as PersonPoint;
            if (target != null)
            {
                PP_OrgInfo orgInfo = target.OrgInfo;
                Group.GroupDialog groupDialog = new Group.GroupDialog(new List<PP_OrgInfo>() { orgInfo });
                groupDialog.ShowDialog(Element);
            }
        }
        private void itemTrack_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.Parent as ContextMenu;
            var target = menu.PlacementTarget as PersonPoint;
            if (target != null)
            {
                PP_OrgInfo orgInfo = target.OrgInfo;
                PP_PersonInfo personInfo = target.PersonInfo;
                
                PersonPoint group = new PersonPoint(orgInfo,personInfo);
                Track.TrackCondition conTrack = new Track.TrackCondition(group);
                conTrack.VehicleNum = personInfo.GUID;
                conTrack.Owner = VisualTreeHelperExtension.GetParentObject<System.Windows.Window>(Element);
                conTrack.Show();
               
            }
        }
    }
}
