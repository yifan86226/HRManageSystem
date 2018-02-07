using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.TaskManage
{
    [Obsolete("数据结构过时,请使用Task")]
    public class TaskListInfo : IdentifiableData<string>, INotifyPropertyChanged
    {
        private bool isChecked;
        /// <summary>
        ///  选择
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                this.isChecked = value;
                this.NotifyPropertyChanged("IsChecked");
            }
        }
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 任务类型id(一般任务、监测任务和干扰任务)
        /// </summary>
        public string CHILDGUID { get; set; }
        /// <summary>
        /// 任务名称 (经过沟通，现在用于保存发送任务的用户组ID)
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
        /// 0一般任务，1干扰任务
        /// </summary>
        public int TASKTYPE { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string EXPANDS { get; set; }
        /// <summary>
        /// 紧急程度 
        /// </summary>
        public int URGENCY { get; set; }
        /// <summary>
        /// 小组父节点
        /// </summary>
        public string PARENTGROUPID { get; set; }

        private string activity_guid;
        /// <summary>
        /// 活动ID
        /// </summary>
        public string ACTIVITY_GUID
        {
            get { return activity_guid; }
            set { activity_guid = value; }
        }

        /// <summary>
        /// 音频文件 WAV格式音频
        /// </summary>
        public byte[] AudioFile { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性变更方法
        /// </summary>
        /// <param name="propertyName">发生变更的属性名称</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// 回执结果 add by wrx 
        /// </summary>
        public string ReceiptMsg { get; set; }
        /// <summary>
        /// 干扰任务回执结果 add by wrx 0关闭，1协调，2改频，3其他
        /// </summary>
        public int? CheckResult { get; set; }
    }
}
