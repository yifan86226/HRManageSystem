using CO_IA.Data;
using CO_IA.UI.Screen.Control;
using I_CO_IA.FreqStation;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.Screen.MainPage
{
    /// <summary>
    /// StateNotice.xaml 的交互逻辑
    /// </summary>
    public partial class StateNotice : UserControl
    {
        System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();
        List<object> UIList = new List<object>();       
        Storyboard story = null;
        int currentSelect = 0;
        ShowOne one;
        ShowTwo two;
        ShowThree three;
        public StateNotice()
        {
            InitializeComponent();
            one = new ShowOne(); 
            UIList.Add(one);
            two = new ShowTwo();
            UIList.Add(two);
            three = new ShowThree();
            UIList.Add(three);           

            this.Loaded += StateNotice_Loaded;
        }

        void StateNotice_Loaded(object sender, RoutedEventArgs e)
        {
            story = (Storyboard)FindResource("storybegin");
            tm.Interval = 8000;
            tm.Tick += tm_Tick;
            tm.Start();
            tm_Tick(null,null);

            Start();
        }

        void tm_Tick(object sender, EventArgs e)
        {

            contenter.Content = UIList[currentSelect];
            //story.Completed += delegate
            //{
            //    var a = contenter.Content;
            //    contenter.Content = null;
            //    contenter.Content = a;
            //};
            story.Begin(contenter, true);
            currentSelect++;
            if (currentSelect >= UIList.Count)
                currentSelect = 0;
        }
        
        public void Begin()
        {
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(1000);
                GetData();
            })) { IsBackground = true }.Start();            
        }
        public void Start()
        {
            Obj.ExecThread = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(1000);
                while (true)
                {
                    Thread.Sleep(20 * 1000);
                    GetData();

                }
            })) { IsBackground = true };
            Obj.ExecThread.Start();
        }
        public void Stop()
        {
            if (Obj.ExecThread.IsAlive)
                Obj.ExecThread.Abort();
        }

        private void GetData()
        {
            StateData stateData = GetSignal();


            StateData data1 = GetPrepareData();
            one.PlanFreqPointNum = data1.PlanFreqPointNum;
            one.ApplyFreqPointNum = data1.ApplyFreqPointNum;

            one.ClearFreqCount = stateData.ClearFreqCount;

            //干扰数量
            two.DistCount = GetDisturb();
            two.KnowSignalCount = stateData.KnowSignalCount;
            two.UnKnowSignalcount = stateData.UnKnowSignalcount;

            int[] data3 = GetPersonData();
            three.PersonNum = data3[0];
            three.VehicleNum = data3[2];
            three.EquipmentNum = data3[1];

            
        }


        #region 获取频率数据
        private StateData GetPrepareData()
        {
            StateData preData = new StateData();
            EquipmentLoadStrategy equcondition = new EquipmentLoadStrategy();
            equcondition.ActivityGuid = Obj.Activity.Guid;
            ActivityEquipment[] equlist = GetActivityEquipments(equcondition);
            if (equlist != null && equlist.Length > 0)
            {
                if (!string.IsNullOrEmpty(Obj.SelectedAreaID))
                {
                    equlist = equlist.Where(item => item.PlaceGuid == Obj.SelectedAreaID).ToArray();
                }
                preData.PlanFreqPointNum = equlist.Length;
                foreach (var item in equlist)
                {
                    if (item.SpareFreq != null)
                    {
                        preData.ApplyFreqPointNum++;
                    }
                }
            }
            return preData;
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
        #endregion

        #region 干扰数量
        /// <summary>
        /// 干扰数量
        /// </summary>
        /// <returns></returns>
        private int GetDisturb()
        {
            //干扰数量 从干扰任务中获取
            int count = 0;
            if (Obj.taskData != null)
            {
                if (Obj.taskData.tasking != null && Obj.taskData.tasking.Length > 0)
                {
                    var task = Obj.taskData.tasking.Where(item =>
                    {
                        if (item.TaskType == Types.TaskType.Disturb)
                        {
                            if (!string.IsNullOrEmpty(Obj.SelectedAreaID))
                            {
                                if (item.TaskPlaceID != Obj.SelectedAreaID)
                                    return false;
                            }
                            return true;
                        }
                        return false;
                    });
                    if (task != null)
                        count = task.ToArray().Length;
                }
                if (Obj.taskData.Tasked != null && Obj.taskData.Tasked.Length > 0)
                {
                    var task = Obj.taskData.Tasked.Where(item =>
                    {
                        if (item.TaskType == Types.TaskType.Disturb)
                        {
                            if (!string.IsNullOrEmpty(Obj.SelectedAreaID))
                            {
                                if (item.TaskPlaceID != Obj.SelectedAreaID)
                                    return false;
                            }
                            return true;
                        }
                        return false;
                    });
                    if (task != null)
                        count += task.ToArray().Length;
                }
            }
            return count;
        }
        #endregion

        #region 获取人数据
        private int[] GetPersonData()
        {
            int[] itemCounts = new int[3] { 0, 0, 0 };
            List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                //更新当前节点
                nodes = channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });
            if (nodes.Count > 0)
            {
                foreach (var org in nodes)
                {
                    //人
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        var PersonList = channel.GetPP_PersonInfos(org.GUID);
                        if (PersonList != null && PersonList.Count > 0)
                        {
                            var list = PersonList.Where(item => !string.IsNullOrEmpty(item.NAME)).ToArray();
                            itemCounts[0] = itemCounts[0] + list.Length;
                        }
                    });
                    //设备
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        var EquipList = channel.GetPP_EqupInfos(org.GUID);
                        itemCounts[1] = itemCounts[1] + EquipList.Count;
                    });
                    //车
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        var VehicleList = channel.GetPP_VehicleInfo(org.GUID);
                        if (VehicleList != null && !string.IsNullOrEmpty(VehicleList.VEHICLE_NUMB))
                            itemCounts[2]++;
                    });
                }
            }
            return itemCounts;
        }
        #endregion

        private StateData GetSignal()
        {
            StateData preData = new StateData();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation>(channel =>
            {
                var result = channel.StatisticClearFreqs(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, (int)CO_IA.Client.RiasPortal.ModuleContainer.Activity.ActivityStage, Obj.SelectedAreaID);
                if (result != null&&result.Length>0)
                {
                    foreach (var item in result)
                    {
                        if (item.Name == "清理频点")
                            preData.ClearFreqCount = item.Value;
                        if (item.Name == "已知信号")
                            preData.KnowSignalCount = item.Value;
                        if (item.Name == "未知信号")
                            preData.UnKnowSignalcount = item.Value;
                    }
                }
            });//清理频点数
            return preData;
        }
        

    }

    internal class ShowOne : StateData
    {
        
    }
    internal class ShowTwo : StateData
    {
    }
    internal class ShowThree : StateData
    {
    }
}
