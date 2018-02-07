using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.FileManage
{
    /// <summary>
    /// 文件管理--主表
    /// </summary>
    public class FileManageInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string GUID { get; set; }

        /// <summary>
        /// 目录Guid
        /// </summary>
        public string CatalogGuid { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FILELNAME { get; set; }
        /// <summary>
        /// 文件起草人
        /// </summary>
        public string DRAFTPERSON { get; set; }
        /// <summary>
        /// 起草日期
        /// </summary>
        public DateTime DRAFTDATE {get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string SUMMARY { get; set; }


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
