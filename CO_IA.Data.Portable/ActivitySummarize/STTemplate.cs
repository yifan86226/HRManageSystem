using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.ActivitySummarize
{
    public class STTemplate : INotifyPropertyChanged
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
        private string name;
        public string NAME
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.NotifyPropertyChanged("NAME");
            }
        }
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 上传文件格式,以Word、Excel格式存储的附件
        /// </summary>
        public string FILEFORM { get; set; }
        /// <summary>
        /// 上传文件大小
        /// </summary>
        public string FILESIZE { get; set; }
        /// <summary>
        /// 用来存储原文件
        /// </summary>
        public byte[] FILEDOC { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
            }
        }
    }
}
