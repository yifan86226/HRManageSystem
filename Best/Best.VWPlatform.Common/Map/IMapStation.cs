using System;
using System.Windows;
using Best.VWPlatform.Common.Types;
using Best.VWPlatform.Common.Interfaces;
using System.Collections.Generic;
using ESRI.ArcGIS.Client;

namespace Best.VWPlatform.Common.Map
{
    public interface IMapStation : IMapInitialize
    {
        #region 方法
        /// <summary>
        /// 添加站点
        /// </summary>
        /// <param name="pStationInfo">站点信息</param>
        Station AddStation(StationInfo pStationInfo);
        /// <summary>
        /// 删除站点
        /// </summary>
        /// <param name="pStationInfo">站点信息</param>
        void RemoveStation(StationInfo pStationInfo);
        /// <summary>
        /// 地图上是否包含指定站点
        /// </summary>
        /// <param name="pStationInfo">站点信息</param>
        /// <returns>true - 包含</returns>
        bool ContainStation(StationInfo pStationInfo);
        /// <summary>
        /// 获取地图上已经存在的站点
        /// </summary>
        /// <param name="pStationInfo">站点信息</param>
        /// <returns>站点对象</returns>
        Station GetStation(StationInfo pStationInfo);
        /// <summary>
        /// 查找指定坐标点是否有站信息
        /// </summary>
        /// <param name="pPoint">坐标</param>
        /// <returns>站信息</returns>
        StationInfo FindStation(MapPointEx pPoint);
        /// <summary>
        /// 清除所有站点
        /// </summary>
        void ClearStation(ClearStationOperate pCsOperate);
        /// <summary>
        /// 在指定点闪烁
        /// </summary>
        /// <param name="pFlarePoint">闪烁点</param>
        void Flare(MapPointEx pFlarePoint);
        /// <summary>
        /// 改变站的显示状态
        /// </summary>
        /// <param name="pIsShow">是否可见，true - 可见，false - 隐藏</param>
        void ChangeStationDisplayStatus(bool pIsShow);

        /// <summary>
        /// 添加车行走时的车位置
        /// </summary>
        void AddVehicleMovePosition(Graphic graphic);
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置当前站图层是否为站选择模式，true - 站选择模式，false - 非选择模式
        /// </summary>
        bool IsSelectMode
        {
            get;
            set;
        }

        /// <summary>
        /// 站点列表
        /// </summary>
        IEnumerable<Station> Stations { get; }
        #endregion

        #region 事件
        /// <summary>
        /// 站可见状态发生变化
        /// </summary>
        /// <remarks>
        /// StationInfo - 站信息
        /// bool - true：可见，false：不可见
        /// </remarks>
        event Action<StationInfo, bool> StationVisibleChanged;

        event Action SelectStationChanged;

        #endregion
    }
}
