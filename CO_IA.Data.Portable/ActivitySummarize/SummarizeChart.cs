#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：总结统计
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class SummarizeChart
    {

        #region 字段
        private string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private List<BasicSummarizeChart> _list = new List<BasicSummarizeChart>();

        public List<BasicSummarizeChart> List
        {
            get { return _list; }
            set { _list = value; }
        }
        #endregion

    }


    public class BasicSummarizeChart
    {
        #region 字段
        private string _name;
        private double _vaule;


        #endregion


        #region 属性


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double Value
        {
            get { return _vaule; }
            set { _vaule = value; }
        }



        #endregion
    }
}
