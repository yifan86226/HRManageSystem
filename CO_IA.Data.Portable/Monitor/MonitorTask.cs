using System;
using System.Collections.Generic;
using System.Linq;

namespace CO_IA.Data.Monitor
{
    public class MonitorTask
    {
        private string _taskID = Guid.NewGuid().ToString();

        public string TaskID
        {
            get { return _taskID; }
            set { _taskID = value; }
        }

        /// <summary>
        /// 工作地点
        /// </summary>
        public string WorkAddress { get; set; }

        private DateTime _workDateBegin = DateTime.Now;
        private DateTime _workDateEnd = DateTime.Now;
        /// <summary>
        /// 工作时间
        /// </summary>
        public DateTime WorkDateBegin
        {
            get { return _workDateBegin; }
            set { _workDateBegin = value; }
        }
        /// <summary>
        /// 工作时间
        /// </summary>
        public DateTime WorkDateEnd
        {
            get { return _workDateEnd; }
            set { _workDateEnd = value; }
        }
        private DateTime _workDate = DateTime.Now;
        /// <summary>
        /// 工作时间
        /// </summary>
        public DateTime WorkDate
        {
            get { return _workDate; }
            set { _workDate = value; }
        }

        /// <summary>
        /// 工作时长 - 单位小时
        /// </summary>
        public int TimeLength { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public MonitorTaskTypeEnum TaskType { get; set; }
        /// <summary>
        /// 工作内容描述
        /// </summary>
        public string WorkDescribe { get; set; }

        private List<PersonGroup> _workerGroup = new List<PersonGroup>();
        /// <summary>
        /// 工作人员列表
        /// </summary>
        public List<PersonGroup> WorkerGroup
        {
            get
            {
                return _workerGroup;
            }
            set
            {
                _workerGroup = value;
            }
        }

        private List<FreqRange> _protectFreqRanges = new List<FreqRange>();
        /// <summary>
        /// 频段列表
        /// </summary>
        public List<FreqRange> ProtectFreqRanges
        {
            get
            {
                return _protectFreqRanges;
            }
            set
            {
                _protectFreqRanges = value;
            }
        }
        private List<double> _protectFreqPoints = new List<double>();
        /// <summary>
        /// 频点列表
        /// </summary>
        public List<double> ProtectFreqPoints
        {
            get { return _protectFreqPoints; }
            set { _protectFreqPoints = value; }
        }
    }

    public class FreqRange
    {
        public double FreqFrom { get; set; }

        public double FreqTo { get; set; }
    }

    public class PersonGroup
    {
        public PersonGroup()
        {
            Persons = new List<PersonInfo>();
            Equipments = new List<string>();
        }
        /// <summary>
        /// 所在组名
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 所在组类型
        /// </summary>
        public string GroupType { get; set; }

        /// <summary>
        /// 工作区域
        /// </summary>
        public string WorkArea { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDescribe { get; set; }
        /// <summary>
        /// 工作人员信息
        /// </summary>
        public List<PersonInfo> Persons { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public List<string> Equipments { get; set; }
    }

    public class PersonInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Dept { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }
    }
}
