using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO_IA.Data;

namespace CO_IA.UI.Scene
{
    public class LoginService
    {
        private static Activity _currentActivity = new Activity();
        private static ActivityPlace _currentActivityPlace = new ActivityPlace();
        private static bool _isLogin = true;
        public static event Action<ActivityPlace> ActivityPlaceChanged;
        private static PP_OrgInfo _userOrgInfo;
        /// <summary>
        /// 当前用户组织机构
        /// </summary>
        public static PP_OrgInfo UserOrgInfo
        {
            get { return LoginService._userOrgInfo; }
            set { LoginService._userOrgInfo = value; }
        }
        
        /// <summary>
        /// 当前操作活动
        /// </summary>
        public static Activity CurrentActivity
        {
            get { return LoginService._currentActivity; }
            set { LoginService._currentActivity = value; }
        }
        /// <summary>
        /// 当前选择的地点
        /// </summary>
        public static ActivityPlace CurrentActivityPlace
        {
            get { return LoginService._currentActivityPlace; }
            set
            {
                LoginService._currentActivityPlace = value;
                //if (ActivityPlaceChanged != null)
                //{
                //    ActivityPlaceChanged(value);
                //}
            }
        }
    }
}
