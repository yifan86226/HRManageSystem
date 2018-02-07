//#region 文件描述
///**********************************************************************************
// * 创建人：fdw
// * 摘  要：任务管理--一般任务
// * 日  期：2016-8-6
// * ********************************************************************************/
//#endregion
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;

//namespace CO_IA.Data.TaskManage
//{
//    /// <summary>
//    /// 任务管理--一般任务
//    /// </summary>
//    public class TaskInfo :INotifyPropertyChanged
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
//        /// <summary>
//        /// 唯一标识
//        /// </summary>
//        public string GUID { get; set; }
//        /// <summary>
//        /// 任务标题
//        /// </summary>
//        public string GENERICNAME { get; set; }
//        /// <summary>
//        /// 任务描述
//        /// </summary>
//        public string GENERICDESCRIBE { get; set; }
//        /// <summary>
//        /// 监测结果
//        /// </summary>
//        public string MONITORRESULT { get; set; }
//        /// <summary>
//        /// 紧急程度，0一般，1紧急
//        /// </summary>
//        public int URGENCY { get; set; }
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
//        /// 回执信息发送人
//        /// </summary>
//        public string ResultSenderID { get; set; }
//        /// <summary>
//        /// 回执信息发送日期
//        /// </summary>
//        public DateTime? ResultSendDate { get; set; } 
//        /// <summary>
//        /// 活动id
//        /// </summary>
//        public string ActivityGuid { get; set; }
//        public event PropertyChangedEventHandler PropertyChanged;

//        /// <summary>
//        /// 通知属性变更方法
//        /// </summary>
//        /// <param name="propertyName">发生变更的属性名称</param>
//        private void NotifyPropertyChanged(string propertyName)
//        {
//            if (this.PropertyChanged != null)
//            {
//                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
//            }
//        }
//    }

//}
