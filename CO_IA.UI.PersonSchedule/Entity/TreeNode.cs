#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：TreeView测试类
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.PersonSchedule
{

    public class TreeNode
    {
        public TreeNode()
        {
            this.Nodes = new List<TreeNode>();
            this.ParentID = -1;
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int ParentID { get; set; }
        public List<TreeNode> Nodes { get; set; }
    }

}
