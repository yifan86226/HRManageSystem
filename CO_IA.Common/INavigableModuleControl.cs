using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Common
{
    public interface INavigableModuleControl : AT_BC.SystemPortal.IActivable
    {
        string Title
        {
            get;
        }

        string Guid
        {
            get;
        }

        //List<INavigateContentControl> SubControlList
        //{
        //    get;
        //}

        //void Navigate(INavigateContentControl destContent);
    }
}
