using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.IA.UI.TaskManage
{
   public class DataOfficeHelper
    {
       public static List<OfficeDataBase> CreateOffice()
       {
           List<OfficeDataBase> equs = new List<OfficeDataBase>();
      
           for (int i = 0; i < 3; i++)
           {
               OfficeDataBase equ = new OfficeDataBase();
               equ.Name= "西安市无线活动手册";
               equ.Draft = "王小一";
               equ.DraftDt = DateTime.Today.AddMonths(-1);
               equ.Code = "编号" + i;
               equ.Auditing = "王小二";
               equ.AuditingDate = DateTime.Today.AddMonths(1);
               equ.Sender = "王小三";
               equ.SendDate = DateTime.Today.AddMonths(1);
               equ.SendSate = "草拟";
               equ.Address = "规章制度";
               equs.Add(equ);
           }


           for (int i = 0; i < 4; i++)
           {
               OfficeDataBase equ = new OfficeDataBase();
               equ.Name = "西安市无线活动手册";
               equ.Draft = "李小一";
               equ.DraftDt = DateTime.Today.AddMonths(-1);
               equ.Code = "编号" + i;
               equ.Auditing = "李小二";
               equ.AuditingDate = DateTime.Today.AddMonths(1);
               equ.Sender = "李小三";
               equ.SendDate = DateTime.Today.AddMonths(1);
               equ.SendSate = "审阅";
               equ.Address = "文件管理";
               equs.Add(equ);
           }

           return equs;
       }

       public static List<TaskDataBase> CreateTask()
       {
           List<TaskDataBase> equs = new List<TaskDataBase>();

           for (int i = 0; i < 5; i++)
           {
               TaskDataBase equ = new TaskDataBase();
               equ.Code = "编号" + i;
               equ.TaskName = "任务" + i;
               if (i == 0)
               {
                   equ.TaskType = "一般任务";
               }
               else if (i == 1)
               {
                   equ.TaskType = "临时监测";
               }
               else if (i == 2)
               {
                   equ.TaskType = "干扰任务";
               }
               else
               {
                   equ.TaskType = "一般任务";
               }
               equ.ExecuteUnit = "第"+i+"组";
                if (i == 0)
               {
                   equ.TaskState = "进行中";
               }
               else if (i == 1)
               {
                   equ.TaskState = "已完成";
               }
                else if (i == 2)
                {
                    equ.TaskState = "进行中";
                }
                else
                {
                    equ.TaskState = "进行中";
                }
               equs.Add(equ);
           }

           return equs;
       }


    }
}
