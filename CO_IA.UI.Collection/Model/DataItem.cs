using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.Model
{
    public enum FreqTestStatus : int
    {
        /// <summary>
        /// 初始化状态
        /// </summary>
        init = 0,
        /// <summary>
        /// 停止状态
        /// </summary>
        stop = 1,
        /// <summary>
        /// 测试中状态
        /// </summary>
        testing = 2,
        /// <summary>
        /// 暂停状态
        /// </summary>
        pause = 3

    }
    public class DataItem
    {
        public DataItem(string title)
        {
            Title = title;
        }

        public string Title
        {
            get;
            private set;
        }


        public string MultiPersonTrainingTitle { get; set; }
        public string ChestBitmapTitle { get; set; }
    }
}
