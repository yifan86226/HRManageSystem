using Best.VWPlatform.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 监测测量处理接口
    /// </summary>
    public interface IMonitorMeasureHandler
    {
        /// <summary>
        /// 初始化监测测量操作
        /// </summary>
        /// <param name="pDeviceId">设备ID</param>
        /// <param name="pStationInfo">监测站信息</param>
        void Initialize();
        /// <summary>
        /// 开始测量
        /// </summary>
        /// <param name="pParameters"></param>
        void Start(params object[] pParameters);
        /// <summary>
        /// 停止测量
        /// </summary>
        void Stop();
        /// <summary>
        /// 开始测量完成事件
        /// </summary>
        event Action StartMeasureCompleted;
        /// <summary>
        /// 停止测量完成事件
        /// </summary>
        event Action StopMeasureCompleted;
        ///// <summary>
        ///// 获取设备能力完成事件
        ///// </summary>
        //event Action<DeviceAbilityEventArgs> DeviceAbilityCompleted;
        /// <summary>
        /// 返回执行结果事件
        /// </summary>
        event Action<MeasureExecuteResultEventArgs> ExecuteResultCompleted;
        /// <summary>
        /// 返回描述头事件
        /// </summary>
        event Action<object> HeaderCompleted;
        /// <summary>
        /// 返回数据帧事件
        /// </summary>
        event Action<object> DataCompleted;
        /// <summary>
        /// 每秒接收数据帧事件
        /// </summary>
        event Action<double> PerSecondFrameCount;
    }
}
