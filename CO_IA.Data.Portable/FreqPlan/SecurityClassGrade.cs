#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Liuenliang
 * 摘 要 ：保障类别在定义类
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 保障类别定义
    /// </summary>
    public class SecurityClassGrade : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //根据PropertyChanged事件的委托类，实现PropertyChanged事件： 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public SecurityClassGrade()
        {
            if (string.IsNullOrEmpty(ActivityGuid))
            {
                ActivityGuid = System.Guid.NewGuid().ToString();
            }
        }


        private string activityGuid;
        /// <summary>
        /// Guid
        /// </summary>
        public string ActivityGuid
        {
            get { return activityGuid; }
            set { activityGuid = value; this.NotifyPropertyChanged("IsChecked"); }
        }


        private string securityClass;
        /// <summary>
        /// 级别名称
        /// </summary>
        public string SecurityClass
        {
            get { return securityClass; }
            set { securityClass = value; }
        }

        private string securityGrade;

        /// <summary>
        /// 等级
        /// </summary>
        public string SecurityGrade
        {
            get { return securityGrade; }
            set { securityGrade = value; }
        }

        private string comments;
        /// <summary>
        /// 获取或设置保证类别说明信息
        /// </summary>
        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }

    }
}
