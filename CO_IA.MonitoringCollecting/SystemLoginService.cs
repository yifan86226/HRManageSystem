using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CO_IA.Data;
using CO_IA.Data.Collection;

namespace CO_IA.MonitoringCollecting
{
    public class SystemLoginService : INotifyPropertyChanged
    {
        private static Activity _currentActivity = new Activity();
        private static ActivityPlace _currentActivityPlace = new ActivityPlace();
        private static bool _isLogin = true;
        private static PP_OrgInfo _userOrgInfo;
        private static List<Type> _synTypes = new List<Type>();
        /// <summary>
        /// 注册需要为LoginService提供数据同步的类型
        /// </summary>
        public static List<Type> SynTypes
        {
            get { return SystemLoginService._synTypes; }
            set { SystemLoginService._synTypes = value; }
        }
        public SystemLoginService()
        {
            this.PropertyChanged += SystemLoginService_PropertyChanged;
        }
        private static SystemLoginService instance = new SystemLoginService();

        void SystemLoginService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var currentValue = this.GetType().GetProperty(e.PropertyName).GetValue(this,null);
            if (SynTypes != null && SynTypes.Count > 0)
            {
                foreach (Type synType in SynTypes)
                {
                    if (synType.GetProperty(e.PropertyName) != null)
                    {
                        synType.GetProperty(e.PropertyName).SetValue(synType, currentValue, null);
                    }
                }
            }
        }
        /// <summary>
        /// 当前用户组织机构
        /// </summary>
        public static PP_OrgInfo UserOrgInfo
        {
            get { return SystemLoginService._userOrgInfo; }
            set 
            { 
                SystemLoginService._userOrgInfo = value;
                instance.NotifyPropertyChanged("UserOrgInfo"); 
            }
        }

        /// <summary>
        /// 当前操作活动
        /// </summary>
        public static Activity CurrentActivity
        {
            get { return SystemLoginService._currentActivity; }
            set { SystemLoginService._currentActivity = value; instance.NotifyPropertyChanged("CurrentActivity"); }
        }
        /// <summary>
        /// 当前选择的地点
        /// </summary>
        public static ActivityPlace CurrentActivityPlace
        {
            get { return SystemLoginService._currentActivityPlace; }
            set
            {
                SystemLoginService._currentActivityPlace = value; instance.NotifyPropertyChanged("CurrentActivityPlace"); 
            }
        }
        /// <summary>
        /// 登录状态
        /// </summary>
        public static bool IsLogin
        {
            get { return SystemLoginService._isLogin; }
            set { SystemLoginService._isLogin = value; instance.NotifyPropertyChanged("IsLogin"); }
        }
       
        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propName));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

}
