using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.ActivitySummarize
{
    public class SummarizeDoc : INotifyPropertyChanged
    {
        /// <summary>
        /// 子集合
        /// </summary>
        public List<SummarizeDoc> Children { get; set; }
        public SummarizeDoc()
        {
            Children = new List<SummarizeDoc>();
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
        /// 总结名称
        /// </summary>
        public string SUMMARIZENAME { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        public string SENDPERSON { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime SENDDATE { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string SUMMARY { get; set; }
        /// <summary>
        /// 上传文件名
        /// </summary>
        public string FILENAME { get; set; }
        /// <summary>
        /// 上传文件格式,以Word、Excel格式存储的附件
        /// </summary>
        public string FILEFORM { get; set; }
        /// <summary>
        /// 上传文件大小
        /// </summary>
        public string FILESIZE { get; set; }
        /// <summary>
        /// 文件存储路径
        /// </summary>
        public byte[] FILEPATH { get; set; }
        /// <summary>
        /// 文件存储类别，为了此表共用，如规章制度rules,文件管理filemanage
        /// </summary>
        public string FILETYPE { get; set; }
        /// <summary>
        /// 用来存储原文件
        /// </summary>
        public byte[] FILEDOC { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        public string ACTIVITY_GUID { get; set; }

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
