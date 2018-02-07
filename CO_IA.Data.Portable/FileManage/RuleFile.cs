#region 文件描述
/**********************************************************************************
 * 创建人：fdw
 * 摘  要：规章制度--文件上传列表
 * 日  期：2016-8-6
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.FileManage
{
    /// <summary>
    /// 规章制度--文件上传列表
    /// </summary>
    public class RuleFile : INotifyPropertyChanged
    {
        /// <summary>
        /// 控制DataGrid全选按钮状态
        /// </summary>
        private bool isTBCheck = true;
        public bool IsTBCheck
        {
            get { return isTBCheck; }
            set { isTBCheck = value; NotifyPropertyChanged("IsTBCheck"); }
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
        /// 上传文件名称
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
        /// 上传文件路径，存储相对路径
        /// </summary>
        public byte[] FILEPATH { get; set; }
        /// <summary>
        /// 文件存储类别，为了此表共用，如规章制度rules,文件管理filemanage
        /// </summary>
        public string FILETYPE { get; set; }
        /// <summary>
        /// 存储关联的主表guid
        /// </summary>
        public string MAINGUID { get; set; }


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
