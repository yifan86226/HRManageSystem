using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.FileManage
{
    public class WorkFileInfo :  INotifyPropertyChanged
    {
        public WorkFileInfo()
        {
            Catalog = new IdentifiableData<string>();
            Attachments = new List<FileAttachment>();
            IssueDate = DateTime.Now.Date;
        }

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
        /// 目录信息
        /// </summary>
        public IdentifiableData<string> Catalog
        {
            get;
            set;
        }

        private string filename;
        /// <summary>
        /// 规章制度名称
        /// </summary>
        public string FileName 
        {
            get 
            {
                return filename; 
            }
            set
            {
                filename = value;
                NotifyPropertyChanged("FileName");
            }
        }

        /// <summary>
        /// 发布状态，0草拟，1审阅，2发布
        /// </summary>
        public SendStateEnum SendState { get; set; }
        /// <summary>
        /// 起草人
        /// </summary>
        public string DrafTPerson { get; set; }
        /// <summary>
        /// 起草日期
        /// </summary>
        public DateTime? DrafTDate { get; set; }
        /// <summary>
        /// 审核人 Auditing
        /// </summary>
        public string AuditPerson { get; set; }
        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? AuditDate { get; set; }

        private string issueperson;
        /// <summary>
        /// 发布人
        /// </summary>
        public string IssuePerson
        {
            get
            {
                return issueperson;
            }
            set
            {
                issueperson = value;
                NotifyPropertyChanged("IssuePerson");
            }
        }

        private DateTime? issuedate;
        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime? IssueDate 
        {
            get
            {
                return issuedate;
            }
            set
            {
                issuedate = value;
                NotifyPropertyChanged("IssueDate");
            }
        }

        private string summary;
        /// <summary>
        /// 简介
        /// </summary>
        public string Summary 
        {
            get
            {
                return summary;
            }
            set
            {
                summary = value;
                NotifyPropertyChanged("Summary");
            }
        }

        /// <summary>
        /// 附件列表
        /// </summary>
        public List<FileAttachment> Attachments { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// 发布状态
    /// </summary>
    public enum SendStateEnum
    {
        无,
        草拟,
        审阅,
        发布
    }
}