using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.TaskManage
{
    [Obsolete("数据结构过时,请使用Task")]
    public class TaskQueryList : IdentifiableData<string>
    {
        /// <summary>
        /// 小组名称
        /// </summary>
        public string GROUPNAME { get; set; }
        /// <summary>
        /// 执行小组id
        /// </summary>
        public string GROUPID { get; set; }
        /// <summary>
        /// 任务状态，0进行中，1已完成
        /// </summary>
        public int TASKSTATE { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TASKNAME { get; set; }
        /// <summary>
        /// 0一般任务，1监测任务，2干扰任务
        /// </summary>
        public int TASKTYPE { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        public string ACTIVITY_GUID { get; set; }

        /// <summary>
        /// 小组父节点
        /// </summary>
        public string PARENTGROUPID { get; set; }

    }
}
