using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Map
{
    /// <summary>
    /// 符号元素
    /// </summary>
    /// <remarks>
    /// 元素控件模板为需更改元素样式时使用，默认情况可以不设置
    /// 键值数据源结合为需要进行控件模板绑定时使用，默认情况可以不设置
    /// </remarks>
    public class SymbolElement
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pElementId">元素Id</param>
        public SymbolElement(string pElementId)
        {
            ElementId = pElementId;
        }

        /// <summary>
        /// 元素Id
        /// </summary>
        public string ElementId { get; set; }

        /// <summary>
        /// 元素控件模板
        /// </summary>
        public object ControlTemplate { get; set; }

        /// <summary>
        /// 键值数据源集合
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> DataSources { get; set; }
    }
}
