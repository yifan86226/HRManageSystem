//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;

//namespace CO_IA.Data.MonitorPlan
//{
//    public class Sealed_PP_OrgInfo : PP_OrgInfo
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
//        /// 组织结构下人员信息
//        /// </summary>
//        public List<PP_PersonInfo> Persons { get; set; }
//        public Sealed_PP_OrgInfo()
//        {
//            Persons = new List<PP_PersonInfo>();
//        }
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
