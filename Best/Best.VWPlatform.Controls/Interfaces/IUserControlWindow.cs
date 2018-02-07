using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Controls.Interfaces
{
    /// <summary>
    /// UserControl窗口状态接口
    /// </summary>
    public interface IUserControlWindow
    {
        /// <summary>
        /// 窗口标题
        /// </summary>
        string Title { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pParameter">初始化参数</param>
        void Initialize(object pParameter);
        /// <summary>
        /// 窗口已打开
        /// </summary>
        void OnOpened();
        /// <summary>
        /// 窗口关闭中
        /// </summary>
        /// <param name="pArgs">CancelClose - true：取消窗口关闭</param>
        /// <remarks>
        /// 在处理函数中使用 pArgs.Close() 方法，可直接关闭窗口
        /// </remarks>
        //void OnClosing(WindowCloseEventArgs pArgs);//@?
        /// <summary>
        /// 窗口已关闭
        /// </summary>
        void OnClosed();
    }
}
