using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.FileManage
{
    public class FileAttachment : INotifyPropertyChanged
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
        /// 文件Guid
        /// </summary>
        public string FileGuid { get; set; }

        /// <summary>
        /// 上传文件名称
        /// </summary>
        public string AttName { get; set; }

        /// <summary>
        /// 上传文件大小
        /// </summary>
        public int AttSize { get; set; }

        /// <summary>
        /// 上传文件路径，存储相对路径
        /// </summary>
        public byte[] AttContent { get; set; }

        /// <summary>
        /// 上传日期
        /// </summary>
        public DateTime? UploadData { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Index  { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public DataStateEnum DataState { get; set; }

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
