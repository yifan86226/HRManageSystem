using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CO_IA.Data.TaskManage;

namespace CO_IA.Data.FileManage
{
    public class RegulationsInfo : INotifyPropertyChanged
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
        /// 规章制度名称
        /// </summary>
        public string RULESNAME { get; set; }
        /// <summary>
        /// 发布状态，0草拟，1审阅，2发布
        /// </summary>
        public Int32 SENDSTATE { get; set; }
        /// <summary>
        /// 起草人
        /// </summary>
        public string DRAFTPERSON { get; set; }
        /// <summary>
        /// 起草日期
        /// </summary>
        public DateTime DRAFTDATE { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string AUDITINGPERSON { get; set; }
        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime AUDITINGDATE { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        public string ISSUEPERSON { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime ISSUEDATE { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string SUMMARY { get; set; }
        /// <summary>
        /// 0代表规章制度，1代表文件管理
        /// </summary>
        public Int32 RULETYPER { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        public string ACTIVITY_GUID { get; set; }
        /// <summary>
        /// 附件列表
        /// </summary>
        public List<RuleFile> RuleFiles { get; set; }

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
