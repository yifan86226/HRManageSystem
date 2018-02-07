using Best.VWPlatform.Common.Rmtp.DataFrames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 接收设备能力查询完成事件参数
    /// </summary>
    public sealed class DeviceAbilityEventArgs
    {
        public DeviceAbilityDataFrame DeviceAbility { get; internal set; }

        public string Error { get; set; }
    }
    /// <summary>
    /// 接收测量执行结果完成事件参数
    /// </summary>
    public sealed class MeasureExecuteResultEventArgs
    {
        public string Error { get; set; }
    }

    /// <summary>
    /// 接收测量数据完成事件参数
    /// </summary>
    public sealed class WbfqMeasureDataEventArgs
    {
        public WbfqexDataFrame Data { get; set; }
    }
    /// <summary>
    /// 接收中频测量描述头完成事件参数
    /// </summary>
    public sealed class IffqMeasureHeaderEventArgs
    {
        public IffqexDescribeHeader Header { get; set; }
    }
    /// <summary>
    /// 接收中频测量数据完成事件参数
    /// </summary>
    public sealed class IffqMeasureDataEventArgs
    {
        public IffqexDataFrame Data { get; set; }
    }
}
