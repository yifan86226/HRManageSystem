using AT_BC.Data;
using CO_IA.Client;
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CO_IA.UI.Screen.Task
{
    public class TaskData:NotifyPropertyChangedObject
    {
        private int taskedCount = 0;
        public int TaskedCount
        {
            get
            {
                return taskedCount;
            }
            set
            {
                taskedCount = value;
                NotifyPropertyChanged("TaskedCount");
            }
        }
        private int taskingCount = 0;
        public int TaskingCount
        {
            get
            {
                return taskingCount;
            }
            set
            {
                taskingCount = value;
                NotifyPropertyChanged("TaskingCount");
            }
        }

        private CO_IA.Data.Task[] taskDisturb;
        /// <summary>
        /// 干扰任务数量
        /// </summary>
        public CO_IA.Data.Task[] TaskDisturb
        {
            get
            {
                return taskDisturb;
            }
            set
            {
                taskDisturb = value;
                NotifyPropertyChanged("TaskDisturb");
            }
        }

        private CO_IA.Data.Task[] tasked;
        public CO_IA.Data.Task[] Tasked
        {
            get
            {
                return tasked;
            }
            set
            {
                tasked = value;
                NotifyPropertyChanged("Tasked");
            }
        }
        public CO_IA.Data.Task[] tasking;
        public CO_IA.Data.Task[] Tasking
        {
            get
            {
                return tasking;
            }
            set
            {
                tasking = value;
                NotifyPropertyChanged("Tasking");
            }
        }
        public void Begin()
        {
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(1000);
                GetTaskData();                  
               
            })) { IsBackground = true }.Start();
        }
        public void Start()
        {
            
            Obj.TaskThread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    GetTaskData();

                    Thread.Sleep(10000);//5分钟                   
                }  
            })) { IsBackground = true };
            Obj.TaskThread.Start();
        }
        public CO_IA.Data.Task[] GetTaskData()
        {
            CO_IA.Data.Task[] taskList = null;
            Tasked = null;
            Tasking = null;
            TaskedCount = 0;
            TaskingCount = 0;

            if (Obj.SelectedAreaID == "")
            {
                taskList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, CO_IA.Data.Task[]>(channel =>
                {
                    return channel.GetTasksByActivityGuid(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                });
            }
            else
            {
                taskList = GetTaskByAreaId(Obj.SelectedAreaID);
            }

            if (taskList == null || taskList.Length == 0)
                return null;

            Tasked = taskList.Where(item =>
            {
                foreach (var a in item.Executors)
                {
                    if (!a.Executed)
                        return false;
                }
                if (item.FormState != FormState.Check)
                    return false;
                //if (!string.IsNullOrEmpty(item.TaskPlaceID) && item.TaskPlaceID != Obj.SelectedAreaID && Obj.SelectedAreaID!="")
                //    return false;
                return true;
            }).ToArray();

            if (Tasked != null)
                TaskedCount = Tasked.Length;

            Tasking = taskList.Where(item =>
            {
                int h = 0;
                foreach (var a in item.Executors)
                {
                    if (a.Executed)
                        h++;
                }
                if (h == item.Executors.Length)
                    return false;
                if (item.FormState != FormState.Check)
                    return false;
                //if (string.IsNullOrEmpty(item.TaskPlaceID))
                //    return true;
                //if (!string.IsNullOrEmpty(item.TaskPlaceID) && item.TaskPlaceID != Obj.SelectedAreaID && Obj.SelectedAreaID != "")
                //    return false;
                return true;
            }).ToArray();
            if (Tasking != null)
                TaskingCount = Tasking.Length;

            TaskDisturb = taskList.Where(item => item.TaskType == Types.TaskType.Disturb).ToArray();

            return taskList;
        }

       

        private CO_IA.Data.Task[] GetTaskByAreaId(string AreaId)
        {
            ScheduleDetail[] scheduleDetails = Utility.getOrgGroupsBySchedule(Obj.Activity.Guid, Obj.Activity.ActivityStage);
            if (scheduleDetails != null && scheduleDetails.Length > 0)
            {
                List<string> orgList = new List<string>();
                foreach (var detail in scheduleDetails)
                {
                    if (detail.ScheduleOrgs != null && detail.ScheduleOrgs.Length > 0)
                    {
                        foreach (var info in detail.ScheduleOrgs)
                        {
                            if (info.OrgInfo != null && info.AREA_GUID == AreaId)
                            {
                                orgList.Add(info.OrgInfo.GUID);
                            }
                        }
                    }
                }
                if (orgList.Count != 0)
                {
                    var existTasks = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, CO_IA.Data.Task[]>(channel =>
                    {
                        return channel.GetTasksByActivityGuid(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                    });
                    var tasks = existTasks.Where(item =>
                    {
                        if (item.FormState != FormState.Check)
                            return false;
                        if (item.Executors != null && item.Executors.Length > 0)
                        {
                            foreach (var o in item.Executors)
                            {
                                foreach (string s in orgList)
                                {
                                    if (o.Executor == s)
                                        return true;
                                }
                            }
                        }
                        return false;
                    }).ToArray();
                    return tasks;
                }
            }
            return null;
        }

    }
}
