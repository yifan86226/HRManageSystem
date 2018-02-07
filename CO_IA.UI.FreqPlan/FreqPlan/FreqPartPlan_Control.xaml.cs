using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.MAP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// FreqPartPlan_Control.xaml 的交互逻辑
    /// </summary>
    public partial class FreqPartPlan_Control : UserControl
    {
        public event Action<FreqPlanActivity,Action> OnInitAreaSelect; 
        private int _callNum = 0;
        private string _activityPlaceId;
        /// <summary>
        /// 地点显示用地图
        /// </summary>
        public FreqPartPlan_Control()
        {
            InitializeComponent();
        }
        public string ActivityPlaceId
        {
            get { return _activityPlaceId; }
            set 
            { 
                _activityPlaceId = value;
                GetActivityFreqPlanInfoSource(value);
            }
        }
        private void xbtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (xfreqPartPlanList.FreqPlanInfoItemsSource != null && xfreqPartPlanList.FreqPlanInfoItemsSource.Count >0)
            {
                if (!ActivityPlaceExistMap(_activityPlaceId))
                {
                    _callNum = xfreqPartPlanList.FreqPlanInfoItemsSource.Count;
                    SaveFreqPlanActivity();
                }
                else if (xfreqPartPlanList.FreqPlanInfoItemsSource.FirstOrDefault(p => p.Points == null || p.Points.Length == 0) == null)
                {
                    _callNum = xfreqPartPlanList.FreqPlanInfoItemsSource.Count;
                    SaveFreqPlanActivity();
                }
                else
                {
                    _callNum = 0;
                    for (int i = 0; i < xfreqPartPlanList.FreqPlanInfoItemsSource.Count; i++)
                    {
                        if ((xfreqPartPlanList.FreqPlanInfoItemsSource[i].Points == null ||
                            xfreqPartPlanList.FreqPlanInfoItemsSource[i].Points.Length ==0 ) && OnInitAreaSelect != null)
                        {
                            busyIndicator.IsBusy = true;
                            xfreqPartPlanList.FreqPlanInfoItemsSource[i].Distance = 0.1;
                            OnInitAreaSelect(xfreqPartPlanList.FreqPlanInfoItemsSource[i], SaveFreqPlanActivity);
                        }
                        else
                            SaveFreqPlanActivity();
                    }
                }    
                //System.Threading.AutoResetEvent
            }
        }

        private bool ActivityPlaceExistMap(string pPlaceId)
        {
            bool existMap = false;
            CO_IA.Data.ActivityPlaceInfo activityPlaceInfo =
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceInfo>(
                channel =>
                {
                    return channel.GetPlaceInfo(pPlaceId);
                });
            if (activityPlaceInfo != null)
                existMap = !string.IsNullOrEmpty(activityPlaceInfo.Graphics);
            return existMap;
        }
        private void SaveFreqPlanActivity()
        {
            _callNum ++;
            if (_callNum >= xfreqPartPlanList.FreqPlanInfoItemsSource.Count)
            {
                busyIndicator.IsBusy = false;
                try
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                    {
                        channel.SaveFreqPlanActivitys(xfreqPartPlanList.FreqPlanInfoItemsSource.ToList());
                    });
                    MessageBox.Show("保存成功!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage());
                }
            }
        }
        private void GetActivityFreqPlanInfoSource(string pPlaceId)
        {
            if (ActivityPlaceExistMap(pPlaceId))
            {
                xfreqPartPlanList.BtnRoundIsVisiable = true;
            }
            else
            {
                xfreqPartPlanList.BtnRoundIsVisiable = false;
            }
            try
            {
                List<FreqPlanActivity> freqPlanActivitys =
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<FreqPlanActivity>>(channel =>
                {
                    return channel.GetFreqPlanActivitys(pPlaceId);
                });
                if (freqPlanActivitys != null)
                    xfreqPartPlanList.FreqPlanInfoItemsSource = new System.Collections.ObjectModel.ObservableCollection<FreqPlanActivity>(freqPlanActivitys);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
            }
        }

        private void xSelAdd_Click(object sender, RoutedEventArgs e)
        {
            DialogFreqPartPlan win = new DialogFreqPartPlan();
            win.OnSelectList += win_OnSelectList;
            win.ShowDialog(this);
        }

        void win_OnSelectList(List<FreqPlanSegment> obj)
        {
            List<FreqPlanActivity> freqActivitys = SelectFreqPartConvert(obj);
                //获取当前频率使用数量
            ApplyFreqToValue(ref freqActivitys);
            if (xfreqPartPlanList.FreqPlanInfoItemsSource == null)
            {
                xfreqPartPlanList.FreqPlanInfoItemsSource = new System.Collections.ObjectModel.ObservableCollection<FreqPlanActivity>(freqActivitys);
            }
            else
            {
                for (int i = 0; i < obj.Count; i++)
                {
                    if (xfreqPartPlanList.FreqPlanInfoItemsSource.FirstOrDefault(p => p.FreqId == obj[i].FreqId) == null)
                        xfreqPartPlanList.FreqPlanInfoItemsSource.Add(freqActivitys[i]);
                }
            }
        }
        private void ApplyFreqToValue(ref List<FreqPlanActivity> pFreqActivitys)
        {
            //调用接口，获取当前频率使用数量
            EquipmentQueryCondition queryCondition = new EquipmentQueryCondition();
            queryCondition.PlaceGuid = ActivityPlaceId;
            try
            {
                List<ActivityEquipmentInfo> activityEquipmentInfos =
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<ActivityEquipmentInfo>>(channel =>
                {
                    return channel.GetEquipmentInfos(queryCondition);
                });
                if (activityEquipmentInfos != null && activityEquipmentInfos.Count > 0)
                {
                    for (int i = 0; i < pFreqActivitys.Count; i++)
                    {
                        FreqPlanActivity freqActivity = pFreqActivitys[i];
                        pFreqActivitys[i].ApplyFreqPointNum = activityEquipmentInfos.Where(p => p.AssignFreq * 1000000 >= freqActivity.FreqValue.Little &&
                            p.AssignFreq * 1000000 <= freqActivity.FreqValue.Great).Count();
                        pFreqActivitys[i].ApplyEquipments =
                            activityEquipmentInfos.Where(p => p.AssignFreq * 1000000 >= freqActivity.FreqValue.Little && p.AssignFreq * 1000000 <=
                                freqActivity.FreqValue.Great).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
            }
        }
        private List<FreqPlanActivity> SelectFreqPartConvert(List<FreqPlanSegment> obj)
        {
            List<FreqPlanActivity> freqActivitys = new List<FreqPlanActivity>(obj.Count);
            for (int i = 0; i < obj.Count; i++)
            {
                FreqPlanSegment freqActivity = new FreqPlanActivity();
                StructClone.ClassClone(obj[i], ref freqActivity);
                ((FreqPlanActivity)freqActivity).ActivityId = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
                ((FreqPlanActivity)freqActivity).PlaceId = _activityPlaceId;
                freqActivitys.Add((FreqPlanActivity)freqActivity);
            }
            return freqActivitys;
        }

        private void xbtnDelete_Click(object sender, RoutedEventArgs e)
        {
            //删除
            string[] selGuids = xfreqPartPlanList.FreqPlanInfoItemsSource.Where(p=>p.IsSelected == true).Select(p=>p.Guid).ToArray();
            if (selGuids != null && selGuids.Length >0)
            {
                if (MessageBox.Show("确定删除所选择的记录信息吗？", "删除确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                try
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                    {
                        channel.DelFreqPlanActivitys(selGuids);
                    });
                    List<FreqPlanActivity> delfreqPlanActivitys = xfreqPartPlanList.FreqPlanInfoItemsSource.Where(p => p.IsSelected == true).ToList();
                    for (int i = 0; i < delfreqPlanActivitys.Count;i++ )
                        xfreqPartPlanList.FreqPlanInfoItemsSource.Remove(delfreqPlanActivitys[i]);
                    MessageBox.Show("删除成功!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage());
                }
            }
        }
    }
}
