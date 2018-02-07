#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：部门信息暂存类
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Betx.Portal.Models
{
    /// <summary>
    /// 部门信息
    /// </summary>
    public class DepartmentInfoSource : UserObject
    {
        public DepartmentInfoSource()
        {
            Childs = new ObservableCollection<UserObject>();
            //Childs.CollectionChanged += OnChildsCollectionChanged;
        }

        //private void OnChildsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    //XGZ:频繁调用，影响性能
        //    //            RaisePropertyChanged("OnlineUserCount");
        //}

        internal int InternalOnlineUserCount
        {
            get
            {
                if (Childs.Count == 0)
                    return 0;
                var count = Childs.Sum(i =>
                {
                    var di = i as DepartmentInfoSource;
                    if (di == null)
                    {
                        var ui = i as UserInfoSource;
                        if (ui == null)
                            return 0;
                        return ui.IsOnline ? 1 : 0;
                    }
                    return di.InternalOnlineUserCount;
                });
                return count;
            }
        }

        internal int InternalUserCount
        {
            get
            {
                if (Childs.Count == 0)
                    return 0;
                var count = Childs.Sum(i =>
                {
                    var di = i as DepartmentInfoSource;
                    if (di == null)
                    {
                        return 1;
                    }
                    return di.InternalUserCount;
                });
                return count;
            }
        }
        /// <summary>
        /// 该部门在线用户数
        /// </summary>
        public string OnlineUserCount
        {
            get
            {
                int olCount = 0;
                if (Childs.Count > 0)
                {
                    olCount = Childs.Sum(i =>
                    {
                        var di = i as DepartmentInfoSource;
                        if (di == null)
                        {
                            var ui = i as UserInfoSource;
                            if (ui == null)
                                return 0;
                            return ui.IsOnline ? 1 : 0;
                        }
                        return di.InternalOnlineUserCount;
                    });
                }

                var count = InternalUserCount;
                return string.Format("({0}/{1})", olCount, count);
            }
        }

        /// <summary>
        /// 机构传真 VARCHAR2(200) 
        /// </summary>
        public string DeptFax { get; set; }

        //public string DeptId { get; set; }
        //public string DeptName { get; set; }
        /// <summary>
        /// 机构电话 VARCHAR2(200) 
        /// </summary>
        public string DeptPhone { get; set; }
        /// <summary>
        /// 显示顺序,用于在组织机构树中排序 
        /// </summary>
        public int DisplaySequence { get; set; }
        /// <summary>
        /// 机构代码/地区代码 VARCHAR2(32) 
        /// </summary>
        public string DistrictCode { get; set; }
        /// <summary>
        /// 是否是无委组织机构或部门 
        /// </summary>
        public bool IsOrganization { get; set; }
        ///// <summary>
        ///// 上级机构-Guid VARCHAR2(36) 
        ///// </summary>
        //public string ParentDeptId { get; set; }
        /// <summary>
        /// 该部门下的子部门集合或用户集合
        /// </summary>
        public ObservableCollection<UserObject> Childs { get; set; }

        public void Update()
        {
            RaisePropertyChanged("OnlineUserCount");
        }
    }
}
