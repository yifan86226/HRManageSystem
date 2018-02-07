using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Best.VWPlatform.Common.Map
{
    /// <summary>
    /// 地图工厂
    /// </summary>
    internal static class MapFactory
    {
        private static readonly string MapAssemblyName;
        static MapFactory()
        {
            MapAssemblyName = string.Format("Best.VWPlatform.{0}", VWPlatformConfig.Current.MapConfig.MapInterfaceType.ToString());
        }
        /// <summary>
        /// 创建地图入口实例
        /// </summary>
        /// <returns>IMapOperate 实例</returns>
        public static IMap CreateMap()
        {
            return (IMap)GetMapInstance("MapPortal");
        }

        public static IMapGeneralStation CreateMapGeneralStation(IMap pMap)
        {
            if (pMap == null) throw new ArgumentNullException("pMap");   
            object instance = GetMapInstance("Behaviors.MapGeneralStation");
            ((IMapGeneralStation)instance).Initialize(pMap);
            return (IMapGeneralStation)instance;
        }

        public static object GetMapInstance(string name)
        {
            //加载程序集(dll文件地址)，使用Assembly类   
            Assembly assembly = Assembly.LoadFile(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + MapAssemblyName + ".dll");
            //获取类型，参数（名称空间+类）   
            Type type = assembly.GetType(MapAssemblyName + "." + name);
            //创建该对象的实例，object类型，参数（名称空间+类）   
            return assembly.CreateInstance(MapAssemblyName + "." + name);          
        }
    }
}
