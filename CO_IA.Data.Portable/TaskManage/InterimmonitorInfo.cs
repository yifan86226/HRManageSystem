//#region 文件描述
///**********************************************************************************
// * 创建人：fdw
// * 摘  要：
// * 日  期：2016-8-6
// * ********************************************************************************/
//#endregion
//using AT_BC.Data;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;

//namespace CO_IA.Data.TaskManage
//{
//    /// <summary>
//    /// 任务管理--临时监测
//    /// </summary>
//    public class InterimmonitorInfo : IdentifiableData<string>
//    {
//        private bool isChecked;
//        /// <summary>
//        ///  选择
//        /// </summary>
//        public bool IsChecked
//        {
//            get
//            {
//                return this.isChecked;
//            }
//            set
//            {
//                this.isChecked = value;
//                this.NotifyPropertyChanged("IsChecked");
//            }
//        }
//#warning 需要删除,使用基类的Guid属性
//        /// <summary>
//        /// 唯一标识
//        /// </summary>
//        public string GUID { get; set; }
//        /// <summary>
//        /// 监测标题
//        /// </summary>
//        public string MONITORNAME { get; set; }
//        /// <summary>
//        /// 监测描述
//        /// </summary>
//        public string MONITORMONITOR { get; set; }
//        /// <summary>
//        /// 监测结果
//        /// </summary>
//        public string MONITOROFRESULT { get; set; }

//        /// <summary>
//        /// 执行小组名称
//        /// </summary>
//        public string GROUPNAME { get; set; }
//        /// <summary>
//        /// 执行小组id
//        /// </summary>
//        public string GROUPID { get; set; }
//        /// <summary>
//        /// 执行状态，0进行中，1已完成
//        /// </summary>
//        public int TASKSTATE { get; set; }
//        /// <summary>
//        /// 活动id
//        /// </summary>
//        public string ActivityGuid { get; set; }

//    }
//}
