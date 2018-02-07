using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.IA.UI.TaskManage
{
    public class OfficeDataBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 规章制度名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 起草人
        /// </summary>
        public string Draft { get; set; }
        /// <summary>
        /// 起草时间
        /// </summary>
        public DateTime? DraftDt { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public string Auditing { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditingDate { get; set; }
        /// <summary>
        /// 发送人
        /// </summary>
        public string Sender { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? SendDate { get; set; }
        /// <summary>
        /// 发布状态
        /// </summary>
        public string SendSate { get; set; }

        public string Address { get; set; }
    }

    public class TaskDataBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string TaskType { get; set; }
        /// <summary>
        /// 执行小组
        /// </summary>
        public string ExecuteUnit { get; set; }
        /// <summary>
        /// 执行状态 
        /// </summary>
        public string TaskState { get; set; }
    }

    public class ExecuteUnits
    {
        public ExecuteUnits()
        {
            this.Nodes = new List<ExecuteUnits>();
            this.ParentID = -1;
        }
        /// <summary>
        /// 树节点id
        /// </summary>
        public int treeId { get; set; }
        /// <summary>
        /// 树节点名称
        /// </summary>
        public string treeName { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public int ParentID { get; set; }

        public List<ExecuteUnits> Nodes { get; set; }

    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeView"></param>
        public static void ExpandAll(this System.Windows.Controls.TreeView treeView)
        {
            ExpandInternal(treeView);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetItemContainer"></param>
        private static void ExpandInternal(System.Windows.Controls.ItemsControl targetItemContainer)
        {
            if (targetItemContainer == null) return;
            if (targetItemContainer.Items == null) return;
            for (int i = 0; i < targetItemContainer.Items.Count; i++)
            {
                System.Windows.Controls.TreeViewItem treeItem = targetItemContainer.Items[i] as System.Windows.Controls.TreeViewItem;
                if (treeItem == null) continue;
                if (!treeItem.HasItems) continue;

                treeItem.IsExpanded = true;
                ExpandInternal(treeItem);
            }

        }

    } 


}
