using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.MonitorPlan
{
    [Obsolete("DetailMonitorPlan已经过时,请使用MonitorPlanInfo")]
    public class DetailMonitorPlan : IdentifiableData<string>, INotifyPropertyChanged
    {
        //private List<Sealed_PP_OrgInfo> _workerGroup = new List<Sealed_PP_OrgInfo>();
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
#warning 需要删除,使用基类的Guid属性
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 生成任务的频率id,将存储多条id信息
        /// </summary>
        public string MAINGUID { get; set; }
        /// <summary>
        /// 工作地点
        /// </summary>
        public string WORKPLACE { get; set; }
        /// <summary>
        /// 任务开始日期
        /// </summary>
        public DateTime STARTTASKDATE { get; set; }
        /// <summary>
        /// 任务结束日期
        /// </summary>
        public DateTime ENDTASKDATE { get; set; }
        /// <summary>
        /// 工作内容
        /// </summary>
        public string WORKCONTENT { get; set; }
        /// <summary>
        /// 工作地点id
        /// </summary>
        public string WORKPLACEID { get; set; }
        /// <summary>
        /// 预定派出组
        /// </summary>
        public string SENDGROUPS { get; set; }
        /// <summary>
        /// 重点频段信息，多个频率值段用，分隔
        /// </summary>
        public string IMPORTFREQUENCYRANGE { get; set; }
        /// <summary>
        /// 重点频点信息，多个频点值用，分隔
        /// </summary>
        public string IMPORTFREQUENCYPOINT { get; set; }
        /// <summary>
        /// 监测组id
        /// </summary>
        public string SENDGROUPIDS { get; set; }
        /// <summary>
        /// 地点对应的位置
        /// </summary>
        public string POSITIONINFO { get; set; }
        /// <summary>
        /// 位置信息id
        /// </summary>
        public string POSITIONID { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        public string ACTIVITY_GUID { get; set; }

        /// <summary>
        /// 临时存储，频段范围
        /// </summary>
        public List<string> FrequencyRange { get; set; }
        /// <summary>
        /// 临时存储，频点范围
        /// </summary>
        public List<string> FreqPointsRange { get; set; }
        /// <summary>
        /// 临时存储，生成状态的频率预案id
        /// </summary>
        public List<string> FreqMAINGUIDs { get; set; }
        /// <summary>
        /// 临时存储，获取监测组信息列表
        /// </summary>
        public List<string> GroupIDS { get; set; }
        /// <summary>
        /// 监测组人员信息
        /// </summary>
        public List<PP_PersonInfo> Persons { get; set; }

          /// <summary>
        /// 扩展坐标点集
        /// </summary>
        public CustomPoint[] Points { get; set; }

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
    }
}
