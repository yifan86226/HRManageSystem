using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 保障级别
    /// </summary>
    public class ProtectLevelData : INotifyPropertyChanged
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
        /// 类别
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 保障级别
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }


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
