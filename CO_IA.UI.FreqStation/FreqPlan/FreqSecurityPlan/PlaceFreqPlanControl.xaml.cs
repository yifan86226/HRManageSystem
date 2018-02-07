using AT_BC.Common;
using AT_BC.Data;
using CO_IA.Data;
using CO_IA.Client;
using CO_IA.UI.MAP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CO_IA.UI.Setting;


namespace CO_IA.UI.FreqStation.FreqPlan
{
    /// <summary>
    /// LocationFreqPlanControl.xaml 的交互逻辑
    /// </summary>
    public partial class PLaceFreqPlanControl : UserControl
    {
        private string placeExGuid = Guid.NewGuid().ToString();

        private Dictionary<int, BePolygon> dicDistanceRangPoints = new Dictionary<int, BePolygon>();

        private ActivityPlaceMap activityMap = new ActivityPlaceMap();

        private EquipmentClassFreqRange[] freqRanges;

        public PLaceFreqPlanControl()
        {
            InitializeComponent();
            this.Loaded += PLaceFreqPlanControl_Loaded;
        }

        private void PLaceFreqPlanControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.LoadPlace(e.NewValue as ActivityPlaceInfo);
        }

        private void LoadPlace(ActivityPlaceInfo placeInfo)
        {
            dicDistanceRangPoints.Clear();
            if (placeInfo != null)
            {
                Dictionary<string, ActivityPlaceInfo> dic = new Dictionary<string, ActivityPlaceInfo>();
                dic.Add(placeInfo.Guid, placeInfo);
                this.activityMap.PlaceLocation = dic;
                this.activityMap.ShowMap.SetAllGraphicsExtent();
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation>(channel =>
                {
                    var dataSource = channel.GetPlaceFreqPlans(placeInfo.Guid);
                    foreach (var data in dataSource)
                    {
                        if (data.RangePointList != null)
                        {
                            this.dicDistanceRangPoints[data.mDistanceToActivityPlace] = new BePolygon { PointList = data.RangePointList };
                        }
                    }
                    if (dataSource != null && dataSource.Length > 0)
                    {
                        this.dataGridFreqRange.ItemsSource = new ObservableCollection<PlaceFreqPlan>(dataSource);
                    }
                    else
                    {
                        this.dataGridFreqRange.ItemsSource = new ObservableCollection<PlaceFreqPlan>();
                    }
                });
            }
        }

        private void PLaceFreqPlanControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.borderMapContainer.Child = activityMap.ShowMap.MainMap;
            activityMap.ShowMap.MapInitialized += MapInitialized;
        }

        private void MapInitialized(bool obj)
        {
            if (obj)
            {
                this.LoadPlace(this.DataContext as ActivityPlaceInfo);
                this.DataContextChanged += PLaceFreqPlanControl_DataContextChanged;
            }
        }

        private void buttonAreaSetting_Click(object sender, RoutedEventArgs e)
        {
            var placeInfo = this.DataContext as ActivityPlaceInfo;
            if (placeInfo == null)
            {
                return;
            }
            var dgr = DataGridRow.GetRowContainingElement((sender as Hyperlink).Tag as FrameworkElement);
            if (dgr != null && dgr.DataContext is PlaceFreqPlan)
            {
                var dataContext = dgr.DataContext as PlaceFreqPlan;
                ExtendDistanceSettingWindow wnd = new ExtendDistanceSettingWindow();
                wnd.mDistance = dataContext.mDistanceToActivityPlace;
                if (wnd.ShowDialog(this) == true)
                {
                    int mDistance = wnd.mDistance;
                    if (mDistance == dataContext.mDistanceToActivityPlace)
                    {
                        return;
                    }
                    this.activityMap.ShowMap.RemoveSymbolElement(placeExGuid);
                    BePolygon polygon;
                    if (this.dicDistanceRangPoints.TryGetValue(mDistance, out polygon))
                    {
                        this.SavePlaceFreqPlan(dataContext, mDistance);
                        this.DrawPlaceExtendRange(dataContext);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(placeInfo.Graphics))
                        {
                            throw new Exception("选择区域没有绘制范围,请先绘制区域范围");
                        }
                        this.activityMap.ShowMap.GetGeometryBuffer(wnd.mDistance, placeInfo.Graphics, geoPoints =>
                        {
                            List<GeoPoint> geoPointList = new List<GeoPoint>(geoPoints);
                            this.dicDistanceRangPoints[mDistance] = new BePolygon { PointList = geoPointList };
                            this.SavePlaceFreqPlan(dataContext, mDistance);
                            this.DrawPlaceExtendRange(dataContext);
                        }, null);
                    }
                }
            }
        }


        private CheckBox checkBoxAll;
        private void checkBoxAll_Loaded(object sender, RoutedEventArgs e)
        {
            this.checkBoxAll = sender as CheckBox;
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            var placeInfo = this.DataContext as ActivityPlaceInfo;
            if (placeInfo == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(placeInfo.Graphics))
            {
                MessageBox.Show("选择区域没有绘制范围,请先绘制区域范围");
                return;
            }
            var freqPlanList = this.dataGridFreqRange.ItemsSource as IList<PlaceFreqPlan>;
            if (freqPlanList == null)
            {
                return;
            }
            if (this.freqRanges == null)
            {
                this.freqRanges = CO_IA.Client.Utility.GetEquipmentClassFreqRanges();
            }
            string[] freqRangeGuids = (from data in freqPlanList select data.Key).ToArray();

            EquipmentClassFreqPlanningSelectWindow wnd = new EquipmentClassFreqPlanningSelectWindow();
            var selectableFreqRanges = from data in this.freqRanges where !freqRangeGuids.Contains(data.Key) select data;
            wnd.DataContext = selectableFreqRanges;
            if (wnd.ShowDialog(this) == true)
            {
                var checkedFreqRanges = wnd.GetCheckedFreqPlans();
                if (checkedFreqRanges.Length > 0)
                {
                    var calculateDistances = (from data in checkedFreqRanges where !this.dicDistanceRangPoints.ContainsKey(data.mDistanceToActivityPlace) select data.mDistanceToActivityPlace).Distinct().ToArray();

                    if (calculateDistances.Length > 0)
                    {
                        this.Busy("正在计算周围台站查询区域");
                        System.Threading.SynchronizationContext syncContext = System.Threading.SynchronizationContext.Current;
                        EventWaitHandle[] waitHandles = new EventWaitHandle[calculateDistances.Length];
                        for (int i = 0; i < waitHandles.Length; i++)
                        {
                            waitHandles[i] = new AutoResetEvent(false);
                        }
                        Action<int[], ActivityPlaceInfo, EventWaitHandle[]> action = new Action<int[], ActivityPlaceInfo, EventWaitHandle[]>(this.CalculateExtendPolygon);
                        action.BeginInvoke(calculateDistances, placeInfo, waitHandles, obj =>
                        {
                            var handles = obj.AsyncState as WaitHandle[];
                            WaitHandle.WaitAll(handles);
                            syncContext.Send(arg =>
                            {
                                this.Idle();
                                this.AddPlaceFreqPlans(checkedFreqRanges, placeInfo);
                            }, null);
                        }, waitHandles);
                    }
                    else
                    {
                        this.AddPlaceFreqPlans(checkedFreqRanges, placeInfo);
                    }
                }
            }
        }

        private void SavePlaceFreqPlan(PlaceFreqPlan freqPlan, int mDistance)
        {
            int currentDistance = freqPlan.mDistanceToActivityPlace;
            this.SetDistanceToPlace(freqPlan, mDistance);
            try
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation>(channel =>
                {
                    channel.SavePlaceFreqPlans(new PlaceFreqPlan[] { freqPlan });
                });
            }
            catch
            {
                this.SetDistanceToPlace(freqPlan, currentDistance);
            }
        }

        private void SetDistanceToPlace(PlaceFreqPlan freqRange, int mDistance)
        {
            BePolygon polygon = this.dicDistanceRangPoints[mDistance];
            freqRange.mDistanceToActivityPlace = mDistance;
            freqRange.RangePointList = polygon.PointList;
            var rect = polygon.GetExternalRectangle();
            freqRange.LongitudeRange = new Range<double> { Little = rect.LeftTop.Longitude, Great = rect.RightBottom.Longitude };
            freqRange.LatitudeRange = new Range<double> { Little = rect.RightBottom.Latitude, Great = rect.LeftTop.Latitude };
        }

        private void AddPlaceFreqPlans(EquipmentClassFreqRange[] checkedFreqRanges, ActivityPlaceInfo placeInfo)
        {
            var freqPlanList = this.dataGridFreqRange.ItemsSource as IList<PlaceFreqPlan>;
            var addFreqPlans = new PlaceFreqPlan[checkedFreqRanges.Length];
            for (int i = 0; i < addFreqPlans.Length; i++)
            {
                addFreqPlans[i] = new PlaceFreqPlan();
                addFreqPlans[i].CopyFrom(checkedFreqRanges[i]);
                addFreqPlans[i].ActivityGuid = placeInfo.ActivityGuid;
                addFreqPlans[i].PlaceGuid = placeInfo.Guid;
                this.SetDistanceToPlace(addFreqPlans[i], addFreqPlans[i].mDistanceToActivityPlace);
                //var polygon = this.dicDistanceRangPoints[addFreqPlans[i].mDistanceToActivityPlace];
                //addFreqPlans[i].RangePointList = polygon.PointList;
                //var rect = polygon.GetExternalRectangle();
                //addFreqPlans[i].LongitudeRange = new Range<double> { Little = rect.LeftTop.Longitude, Great = rect.RightBottom.Longitude };
                //addFreqPlans[i].LatitudeRange = new Range<double> { Little = rect.RightBottom.Latitude, Great = rect.LeftTop.Latitude };
            }
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation>(channel =>
            {
                channel.SavePlaceFreqPlans(addFreqPlans);
            });
            foreach (var freqPlan in addFreqPlans)
            {
                freqPlanList.Add(freqPlan);
            }
        }

        private void CalculateExtendPolygon(int[] mDistanceToPlaces, ActivityPlaceInfo placeInfo, EventWaitHandle[] waitHandles)
        {
            for (int i = 0; i < mDistanceToPlaces.Length; i++)
            {
                var waitHandle = waitHandles[i];
                int mDistanceToPlace = mDistanceToPlaces[i];
                this.activityMap.ShowMap.GetGeometryBuffer(mDistanceToPlace, placeInfo.Graphics, geoPoints =>
                    {
                        List<GeoPoint> geoPointList = new List<GeoPoint>(geoPoints);
                        this.dicDistanceRangPoints[mDistanceToPlace] = new BePolygon { PointList = geoPointList };
                        waitHandle.Set();
                    }, null);
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var placeInfo = this.DataContext as ActivityPlaceInfo;
            if (placeInfo == null)
            {
                return;
            }
            var freqPlanList = this.dataGridFreqRange.ItemsSource as IList<PlaceFreqPlan>;
            if (freqPlanList == null)
            {
                return;
            }
            var deleteFreqPlans = freqPlanList.GetCheckedItems();
            var deleteFreqGuids = (from plan in deleteFreqPlans select plan.Key).ToArray();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation>(channel =>
                {
                    channel.DeletePlaceFreqPlans(placeInfo.Guid, deleteFreqGuids);
                });
            foreach (var freqPlan in deleteFreqPlans)
            {
                freqPlanList.Remove(freqPlan);
            }
            checkBoxAll.IsChecked = false;
        }

        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckableCheckBoxHelper.Checked(sender, e, this.checkBoxAll);
        }

        private void dataGridFreqRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.activityMap.ShowMap.RemoveSymbolElement(this.placeExGuid);
            if (e.AddedItems.Count > 0)
            {
                this.DrawPlaceExtendRange(e.AddedItems[0] as PlaceFreqPlan);
            }
        }

        private void DrawPlaceExtendRange(PlaceFreqPlan freqPlan)
        {
            if (freqPlan.RangePointList != null && freqPlan.RangePointList.Count > 1)
            {
                List<I_GS_MapBase.Portal.Types.MapPointEx> list = new List<I_GS_MapBase.Portal.Types.MapPointEx>(freqPlan.RangePointList.Count);
                foreach (var pt in freqPlan.RangePointList)
                {
                    list.Add(activityMap.ShowMap.MainMap.MapPointFactory.Create(pt.Longitude, pt.Latitude));
                }
                this.activityMap.ShowMap.DrawPolygon(list, new I_GS_MapBase.Portal.SymbolElement(placeExGuid));
                this.activityMap.ShowMap.SetAllGraphicsExtent();
            }
        }
    }
}
