using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class TaskExecuteConclusion : AT_BC.Data.NotifyPropertyChangedObject
    {
        public string ActivityGuid
        {
            get;
            set;
        }

        public TaskType TaskType
        {
            get;
            set;
        }

        public string TaskGuid
        {
            get;
            set;
        }

        public string Executor
        {
            get;
            set;
        }

        private TaskCompleteState completeState;

        public TaskCompleteState CompleteState
        {
            get
            {
                return this.completeState;
            }
            set
            {
                if (this.completeState != value)
                {
                    this.completeState = value;
                    this.NotifyPropertyChanged("CompleteState");
                }
            }
        }

        private string completeDescription;

        public string CompleteDescription
        {
            get
            {
                return this.completeDescription;
            }
            set
            {
                if (this.completeDescription != value)
                {
                    this.completeDescription = value;
                    this.NotifyPropertyChanged("CompleteDescription");
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
                    this.NotifyPropertyChanged("Executed");
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

        private List<TaskStuff> stuffList;

        public List<TaskStuff> StuffList
        {
            get
            {
                return this.stuffList;
            }
            set
            {
                this.stuffList = value;
                this.NotifyPropertyChanged("StuffList");
            }
        }

        public bool Executed
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.submitter);
            }
        }

        private DisturbDisposeType disposeType;

        public DisturbDisposeType DisposeType
        {
            get
            {
                return this.disposeType;
            }
            set
            {
                this.disposeType = value;
                this.NotifyPropertyChanged("DisposeType");
            }
        }
    }
}
