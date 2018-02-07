using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Best.VWPlatform.Common
{
    public class NavigationHelper
    {
        private static ContentControl _curCon = null;
        private static UserControl _mainCon = null;
        private static Stack<UserControl> _hisCon = null;

        public static void Init(ContentControl pConCtl)
        {
            _curCon = pConCtl;

            _mainCon = new UserControl();
            _mainCon.Content = pConCtl.Content;
            _hisCon = new Stack<UserControl>();
            _hisCon.Push(_mainCon);
        }

        /// <summary>
        /// 导航到XX
        /// </summary>
        /// <param name="pUCtl"></param>
        public static void Navigate(UserControl pUCtl)
        {
            _hisCon.Push(pUCtl);
            _curCon.Content =  _hisCon.Peek();
        }

        /// <summary>
        /// 返回上一界面
        /// </summary>
        public static void NavigateBack()
        {
            if (_hisCon.Count == 0)
            {
                return;
            }

            _hisCon.Pop();
            _curCon.Content = _hisCon.Peek();
        }

        /// <summary>
        ///  返回主界面
        /// </summary>
        public static void NavigateMainWnd()
        {
            _hisCon.Clear();

            _hisCon.Push(_mainCon);
            _curCon.Content =  _mainCon;
        }
    }
}
