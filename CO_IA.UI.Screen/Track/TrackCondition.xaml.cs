using CO_IA.Client;
using CO_IA.Client.Orgs;
using CO_IA.Data.Gps;
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
using System.Windows.Shapes;

namespace CO_IA.UI.Screen.Track
{
    /// <summary>
    /// TrackCondition.xaml 的交互逻辑
    /// </summary>
    public partial class TrackCondition : Window
    {
        UserControl ucType;
        bool stop = false;
        public string VehicleNum = "";
        
       

        public TrackCondition(UserControl uc)
        {
            InitializeComponent();

            dE.EditValue = DateTime.Now;

            beginTime.DateValue = DateTime.Now;
            endTime.HourValue = "23";
            endTime.MinitValue = "59";
            ucType = uc;
        }
        private void Track()
        {
            Obj.drawTrack.Reset(ucType);
            int speed = GetSpeed();
            List<GpsOrbit> ps = LoadData();
            if (ps == null || ps.Count == 0)
            {
                MessageBox.Show("没有查询到相关位置信息！");
                btnBegin.Content = "查询";
                return;
            }
            new Thread(() =>
            {                
                Obj.drawTrack.Drawing = true;
                foreach (var p in ps)
                {
                    MapUtiles.PointEx pp = Obj.clientUtile.Correct(p.Latitude,p.Longitude);
                    Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Obj.drawTrack.AddPoint(Obj.screenMap.GetMapPointEx(pp.X,pp.Y));
                        if (chkTrace.IsChecked==true)
                        {
                            Obj.screenMap.MainMap.Location(Obj.screenMap.GetMapPointEx(pp.X, pp.Y));
                        }
                    }));
                    Thread.Sleep(speed);
                    if (stop)
                    {
                        break;
                    }
                }
                
                Obj.drawTrack.Drawing = false;
                Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                {
                    btnBegin.Content = "查询";
                }));
                
            }).Start();
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (btnBegin.Content.ToString() == "查询")
            {
                stop = false;
                btnBegin.Content = "停止";
                Track();
                
            }
            else
            {
                btnBegin.Content = "查询";
                stop = true;
            }
        }
        private List<GpsOrbit> LoadData()
        {
            //OrgToMapStyle orgPoint = ucType as OrgToMapStyle;
            //if (orgPoint != null&&orgPoint.OrgInfo!=null&&!string.IsNullOrEmpty(orgPoint.OrgInfo.GUID))
            //{
                GpsOrbitFilter filter = new GpsOrbitFilter();
                filter.RunDate = Convert.ToDateTime(dE.EditValue);// Convert.ToDateTime("2016-09-19");
                filter.PlateNumber = VehicleNum;
                filter.ActivityId = Obj.Activity.Guid;
                filter.RunTimeRange.Little = double.Parse(beginTime.TimeValue.ToString("HHmmss"));
                filter.RunTimeRange.Great = double.Parse(endTime.TimeValue.ToString("HHmmss")); 

                List<GpsOrbit> gpsOrbitlist =
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Gps.I_CO_IA_Gps, List<GpsOrbit>>(channel =>
                {
                    return channel.QueryGpsOrbit(filter);
                });
                return gpsOrbitlist;
            //}
           
            //List<MapPointEx> ps = new List<MapPointEx>()
            //    {
            //        Obj.screenMap.GetMapPointEx(108.775554,34.291461),
            //        Obj.screenMap.GetMapPointEx(108.822962, 34.278429),
            //        Obj.screenMap.GetMapPointEx(108.815323, 34.270131),
            //        Obj.screenMap.GetMapPointEx(108.822618, 34.269776),
            //        Obj.screenMap.GetMapPointEx(108.829742, 34.275592),
            //        Obj.screenMap.GetMapPointEx(108.829313, 34.265591),
            //        Obj.screenMap.GetMapPointEx(108.833862, 34.261548)
            //    };
            //return ps;
        }
        private int GetSpeed()
        {
            int speed = 2000;
            if (cmbSpeed.SelectedIndex == 0)
            {
                speed = 3000;
            }
            if (cmbSpeed.SelectedIndex == 1)
            {
                speed = 1000;
            }
            if (cmbSpeed.SelectedIndex == 2)
            {
                speed = 500;
            }
            return speed;
        }

        private void BaseWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Obj.drawTrack.Drawing)
            {
                MessageBox.Show("请先停止查询！");
                e.Cancel = true;
            }
        }
    }
}
