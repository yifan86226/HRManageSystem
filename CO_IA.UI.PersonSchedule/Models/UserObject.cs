#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：用户类
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

using Microsoft.Practices.Prism.ViewModel;

namespace Betx.Portal.Models
{
    public class UserObject : NotificationObject
    {
        private bool _hasNewMessage;
        /// <summary>
        /// 对象类型，0-组织机构，1-部门，2-用户
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 用户Id或部门Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户名称或部门名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否有新消息
        /// </summary>
        public bool HasNewMessage
        {
            get
            {
                return _hasNewMessage;
            }
            set { _hasNewMessage = value; RaisePropertyChanged("HasNewMessage"); }
        }
        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParentId { get; set; }

    }
}
