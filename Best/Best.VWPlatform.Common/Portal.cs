using Best.VWPlatform.Common.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common
{
    /// <summary>
    /// 模块入口
    /// </summary>
    public static class Portal
    {
        private static IMap _map;
        private static IMapGeneralStation _mapGeneralStation;

        private static readonly object MapLockObj = new object();

        /// <summary>
        /// 获取地图基本接口
        /// </summary>
        public static IMap Map
        {
            get
            {
                if (_map == null)
                {
                    lock (MapLockObj)
                    {
                        if (_map == null)
                            _map = MapFactory.CreateMap();
                    }
                }
                return _map;
            }
            set
            {
                _map = value;
            }
        }

        /// <summary>
        /// 获取台站相关地图操作接口
        /// </summary>                                                                                                                                                                                                              
        public static IMapGeneralStation MapGeneralStation
        {
            get
            {
                if (_mapGeneralStation == null)
                {
                    lock (MapLockObj)
                    {
                        if (_mapGeneralStation == null)
                            _mapGeneralStation = MapFactory.CreateMapGeneralStation(Map);
                    }
                }
                return _mapGeneralStation;
            }
            set
            {
                _mapGeneralStation = value;
            }
        }
    }
}
