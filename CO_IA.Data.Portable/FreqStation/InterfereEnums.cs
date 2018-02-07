using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    [Flags]
    /// <summary>
    /// 干扰类型
    /// </summary>
    public enum InterfereTypeEnum
    {
        无干扰 = 0,
        同频干扰 = 1,
        邻频干扰 = 2,
        接收机互调干扰 = 4,
        发射机互调干扰 = 8
    }

    /// <summary>
    /// 干扰阶数
    /// </summary>
    [Flags]
    public enum InterfereOrderEnum : short
    {
        干扰 = 0,
        同频 = 1,
        上邻频 = 2,
        下邻频 = 4,
        二阶互调 = 8,
        三阶互调 = 16,
        五阶互调 = 32
    }

    /// <summary>
    /// 干扰物
    /// </summary>
    public enum InterfereObjectEnum
    {
        设备,
        周围台站,
        非法信号,
        其他
    }

    /// <summary>
    /// 干扰频率
    /// </summary>
    public enum InterfereFreqEnum
    {
        主频,
        备频
    }
}
