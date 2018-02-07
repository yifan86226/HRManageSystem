//#region 文件描述
///***************************************************************#@#***************************************************************
// * 创建人：Lizk
// * 摘 要 ：可标记数据定义,要去数据有编码(或标识GUID)和值
// * 日 期 ：2016-08-09
// ***************************************************************#@#***************************************************************/
//#endregion

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace CO_IA.Data
//{
//    /// <summary>
//    /// 可标记数据定义,要求数据有编码(或标识GUID)和值,泛型定义
//    /// </summary>
//    /// <typeparam name="T">值类型</typeparam>
//    public class IdentifiableData<T> :AT_BC.Data.NotifyPropertyChangedObject,  IIdentifiableData
//    {
//        /// <summary>
//        /// 获取或设置数据标识(GUID或编码)
//        /// </summary>
//        public string Guid
//        {
//            get;
//            set;
//        }

//        private T value;

//        /// <summary>
//        /// 获取或设置数据值,可以是数据名称等
//        /// </summary>
//        public T Name
//        {
//            get
//            {
//                return this.value;
//            }
//            set
//            {
//                this.value = value;
//                this.NotifyPropertyChanged("Name");
//            }
//        }

//        /// <summary>
//        /// 获取object类型的数据值,支持IdentifiableData接口
//        /// </summary>
//        object IIdentifiableData.Value
//        {
//            get 
//            {
//                return this.Name;
//            }
//        }
//    }

//    /// <summary>
//    /// 可标记数据定义,要去数据有编码(或标识GUID)和值
//    /// </summary>
//    public interface IIdentifiableData
//    {
//        /// <summary>
//        /// 获取或设置数据标识(GUID或编码)
//        /// </summary>
//        string Guid
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// 获取object类型的数据值
//        /// </summary>
//        object Value
//        {
//            get;
//        }
//    }
//}
