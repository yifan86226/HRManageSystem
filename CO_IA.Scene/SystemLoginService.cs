using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CO_IA.Data;
using I_CO_IA.ActivityManage;
using CO_IA.Client;

namespace CO_IA.Scene
{
    internal class SystemLoginService //: INotifyPropertyChanged
    {
        //private static Activity _currentActivity = new Activity();
        //private static ActivityPlace _currentActivityPlace = new ActivityPlace();
        //private static PP_OrgInfo _userOrgInfo;
        //private static List<Type> _synTypes = new List<Type>();
        //private static SystemLoginService instance = new SystemLoginService();
        //public SystemLoginService()
        //{
        //    this.PropertyChanged += SystemLoginService_PropertyChanged;
        //}
        //void SystemLoginService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    var currentValue = this.GetType().GetProperty(e.PropertyName).GetValue(this, null);
        //    if (SynTypes != null && SynTypes.Count > 0)
        //    {
        //        foreach (Type synType in SynTypes)
        //        {
        //            if (synType.GetProperty(e.PropertyName) != null)
        //            {
        //                synType.GetProperty(e.PropertyName).SetValue(synType, currentValue, null);
        //            }
        //        }
        //    }
        //}
        ///// <summary>
        ///// 注册需要为LoginService提供数据同步的类型
        ///// </summary>
        //public static List<Type> SynTypes
        //{
        //    get { return SystemLoginService._synTypes; }
        //    set { SystemLoginService._synTypes = value; }
        //}
        /// <summary>
        /// 当前用户组织机构
        /// </summary>
        public static PP_OrgInfo UserOrgInfo
        {
            get { return RiasPortal.ModuleContainer.GetExecutorLoginInfo().LoginOrg; }
            //set { SystemLoginService._userOrgInfo = value; instance.NotifyPropertyChanged("UserOrgInfo"); }
        }

        /// <summary>
        /// 当前操作活动
        /// </summary>
        public static Activity CurrentActivity
        {
            get { return RiasPortal.ModuleContainer.Activity; }
            //set { SystemLoginService._currentActivity = value; instance.NotifyPropertyChanged("CurrentActivity"); }
        }
        /// <summary>
        /// 当前选择的地点
        /// </summary>
        public static ActivityPlaceInfo CurrentActivityPlace
        {
            get 
            {
                return CO_IA.Client.RiasPortal.ModuleContainer.GetExecutorLoginInfo().LoginPlace;
                //return SystemLoginService._currentActivityPlace; 
            }
            //set
            //{
            //    SystemLoginService._currentActivityPlace = value;
            //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_ActivityManage>(channel =>
            //    {
            //        SystemLoginService.CurrentActivityPlaceInfo = channel.GetPlaceInfo(_currentActivityPlace.Guid);
            //    });
            //    instance.NotifyPropertyChanged("CurrentActivityPlace"); 
            //}
        }

        //public static ActivityPlaceInfo CurrentActivityPlaceInfo
        //{
        //    get;
        //    set;
        //}

        //internal static void CopyLoginInfomation(Type p_type)
        //{
        //    if (p_type.GetProperty("CurrentActivity") != null)
        //    {
        //        p_type.GetProperty("CurrentActivity").SetValue(p_type, _currentActivity,null);
        //    }
        //    if (p_type.GetProperty("CurrentActivityPlace") != null)
        //    {
        //        p_type.GetProperty("CurrentActivityPlace").SetValue(p_type, _currentActivityPlace, null);
        //    }
        //    if (p_type.GetProperty("UserOrgInfo") != null)
        //    {
        //        p_type.GetProperty("UserOrgInfo").SetValue(p_type, _userOrgInfo, null);
        //    }
        //}
        //private void NotifyPropertyChanged(string propName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propName));
        //    }
        //}

        //public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
