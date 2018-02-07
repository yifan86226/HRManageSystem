using AT_BC.Data;
using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 任务定义
    /// </summary>
    public class Task : CheckableData<string>
    {
        /// <summary>
        /// 获取或设置任务类型
        /// </summary>
        public TaskType TaskType
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置任务所属活动
        /// </summary>
        public string ActivityGuid
        {
            get;
            set;
        }

        /// <summary>
        /// 任务描述
        /// </summary>
        private string description;

        /// <summary>
        /// 获取或设置任务描述
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (value != this.description)
                {
                    this.description = value;
                    this.NotifyPropertyChanged("Description");
                }
            }
        }

        private string taskPlaceID;

        public string TaskPlaceID
        {
            get
            {
                return this.taskPlaceID;
            }
            set
            {
                if (this.taskPlaceID != value)
                {
                    this.taskPlaceID = value;
                    this.NotifyPropertyChanged("TaskPlaceID");
                }
            }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        private string creator;

        /// <summary>
        /// 获取或设置创建人
        /// </summary>
        public string Creator
        {
            get
            {
                return this.creator;
            }
            set
            {
                if (value != this.creator)
                {
                    this.creator = value;
                    this.NotifyPropertyChanged("Creator");
                }
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime createTime;

        /// <summary>
        /// 获取或设置创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return this.createTime;
            }
            set
            {
                if (value != this.createTime)
                {
                    this.createTime = value;
                    this.NotifyPropertyChanged("CreateTime");
                }
            }
        }

        /// <summary>
        /// 紧急程度
        /// </summary>
        private TaskUrgency urgency;

        /// <summary>
        /// 获取或设置任务紧急程度
        /// </summary>
        public TaskUrgency Urgency
        {
            get
            {
                return this.urgency;
            }
            set
            {
                if (value != this.urgency)
                {
                    this.urgency = value;
                    this.NotifyPropertyChanged("Urgency");
                }
            }
        }

        /// <summary>
        /// 任务内容,如果不支持语音视频类任务暂不使用
        /// </summary>
        public byte[] Content
        {
            get;
            set;
        }

        private ExecutorCompleteState[] executors;

        /// <summary>
        /// 任务执行者
        /// </summary>
        public ExecutorCompleteState[] Executors
        {
            get
            {
                return this.executors;
            }
            set
            {
                this.executors = value;
                this.NotifyPropertyChanged("Executors");
            }
        }

        private TaskDisturbInfo disturbInfo;
        public TaskDisturbInfo DisturbInfo
        {
            get
            {
                return this.disturbInfo;
            }
            set
            {
                this.disturbInfo = value;
                this.NotifyPropertyChanged("DisturbInfo");
            }
        }

        private AT_BC.Data.FormState formState;
        public AT_BC.Data.FormState FormState
        {
            get
            {
                return this.formState;
            }
            set
            {
                if (this.formState != value)
                {
                    this.formState = value;
                    this.NotifyPropertyChanged("FormState");
                }
            }
        }

        /// <summary>
        /// 提交人
        /// </summary>
        private string submitter;

        /// <summary>
        /// 获取或设置任务提交人
        /// </summary>
        public string Submitter
        {
            get
            {
                return this.submitter;
            }
            set
            {
                if (value != this.submitter)
                {
                    this.submitter = value;
                    this.NotifyPropertyChanged("Submitter");
                }
            }
        }

        /// <summary>
        /// 任务提交时间
        /// </summary>
        private DateTime? submitTime;

        /// <summary>
        /// 获取或设置任务提交时间
        /// </summary>
        public DateTime? SubmitTime
        {
            get
            {
                return this.submitTime;
            }
            set
            {
                if (value != this.submitTime)
                {
                    this.submitTime = value;
                    this.NotifyPropertyChanged("SubmitTime");
                }
            }
        }
    }
}
