using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Core
{
    /// <summary>
    /// 程序集模块管理
    /// </summary>
    public class ModuleManage
    {
        private static readonly ModuleManage _moduleManage = new ModuleManage();
        private readonly Dictionary<string, Assembly> _assemblyCaches = new Dictionary<string, Assembly>();
        private readonly Dictionary<string, object> _modulePortalCaches = new Dictionary<string, object>();
        private ModuleManage()
        {

        }
        /// <summary>
        /// 模块管理当前实例
        /// </summary>
        public static ModuleManage Current
        {
            get { return _moduleManage; }
        }
        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="assemblyStream"></param>
        internal void LoadAssembly(Stream assemblyStream)
        {
            //@?
            //AssemblyPart assemblyPart = new AssemblyPart();
            //Assembly loadedAssembly = assemblyPart.Load(assemblyStream);
            //string key = loadedAssembly.FullName.Substring(0, loadedAssembly.FullName.IndexOf(','));
            //_assemblyCaches[key] = loadedAssembly;
        }

        /// <summary>
        /// 获取已加载的程序集列表
        /// </summary>
        public IEnumerable<Assembly> LoadedAssemblys
        {
            get { return _assemblyCaches.Values; }
        }

        public IEnumerable<Assembly> GetAssemblysFromCompany(string pCompanyName)
        {
            IEnumerable<Assembly> assemblies = LoadedAssemblys;
            return (from aly in assemblies
                    let attrs = aly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)
                    where attrs != null && attrs.Length != 0
                    let acAttr = attrs[0] as AssemblyCompanyAttribute
                    where acAttr.Company.Equals(pCompanyName)
                    select aly).ToList();
        }
    }
}
