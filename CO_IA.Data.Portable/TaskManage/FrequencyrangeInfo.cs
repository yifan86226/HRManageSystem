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
//    /// 临时任务管理--临时监测子表--监测频段
//    /// </summary>
//    public class FrequencyrangeInfo : IdentifiableData<string>, INotifyPropertyChanged
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
//        /// 临时监测主表id
//        /// </summary>
//        public string MONITORGUDI { get; set; }
//        /// <summary>
//        /// 0频段，1频点
//        /// </summary>
//        public int FREQUENCYTYPE { get; set; }
//        /// <summary>
//        /// 业务名称
//        /// </summary>
//        public string BUSINESSNAME { get; set; }
//        /// <summary>
//        /// 频段/频点起
//        /// </summary>
//        public string FREQUENCYSTART { get; set; }
//        /// <summary>
//        /// 频段/频点止
//        /// </summary>
//        public string FREQUENCYEND { get; set; }
//        /// <summary>
//        /// 带宽
//        /// </summary>
//        public string TAPEWIDTH { get; set; }
//        /// <summary>
//        /// 频段/频点起,单位kHz、MHz、GHz三种
//        /// </summary>
//        public string STARTUNIT { get; set; }
//        /// <summary>
//        /// 频段/频点止单位
//        /// </summary>
//        public string ENDUNIT { get; set; }
//        /// <summary>
//        /// 带宽单位
//        /// </summary>
//        public string TAPEWIDTHUNIT { get; set; }
   

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
