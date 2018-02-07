using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp
{
    /// <summary>
    /// RMTP数据类型
    /// </summary>
    public enum RmtpDataTypes
    {
        业务数据 = 0,
        监测指令 = 1,//（上行：manager发送数据到代理，下行：代理发送数据到manager, 未标注上行的为下行）
        音频数据 = 2,
        经纬度信息 = 3,
        执行结果信息 = 4,
        //RMTP文档中已删除
        //Xmpp消息 = 5,
        数据描述头 = 6,
        设备参数信息 = 7,
        单信道中频音频数据 = 8,
        Tdoa定位结果 = 9,
        设备能力信息 = 10,
        音频数据头 = 11,
        远程环境状态报告 = 12,
        天线因子 = 13,
        监测站查询 = 14,
        Gsmr小站干扰告警 = 100,
        Gsmr小站解码信息 = 101,
        Gsmr小站iq数据 = 102,
        Gsmr小站载干比ci信息 = 103,
        Gsmr小站模板数据 = 104,
        Gsmr小站占用度信息 = 105,
        Gsmr小站频谱数据 = 106
    }
    /// <summary>
    /// RMTP命令，命令相关参数说明
    /// </summary>
    public enum RmtpCommand
    {
        /// <summary>
        /// 没有命令
        /// </summary>
        None,

        #region 监测指令
        /// <summary>
        /// 停止测量指令
        /// 参数：string : 设备ID
        /// </summary>
        RSTOP,
        /// <summary>
        /// 请求参数
        /// </summary>
        PARAM,
        /// <summary>
        /// 监测过程中的参数改变
        /// </summary>
        ALTER,
        /// <summary>
        /// 固定频率测量，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        FIXFQ,
        /// <summary>
        /// 扩展的固定频率测量，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        FIXEX,
        /// <summary>
        /// 中频测量，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        IF_FQ,
        /// <summary>
        /// 中频测量扩展，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        IFQEX,
        /// <summary>
        /// 宽带测量，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        WB_FQ,
        /// <summary>
        /// 扩展宽带测量，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        WBQEX,
        /// <summary>
        /// 单频侧向，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        FIXDF,
        /// <summary>
        /// 中频测向，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        IF_DF,
        /// <summary>
        /// 离散扫描，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        MSCAN,
        /// <summary>
        /// 频段扫描，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        FSCAN,
        /// <summary>
        /// 宽带侧向，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        WB_DF,
        /// <summary>
        /// IQ数据测量，预执行过程具备优先级代码 pricode。
        /// 参数：Tuple《string, string》：item1 - 设备ID，item2  - 用户ID
        /// </summary>
        IQ_FQ,
        /// <summary>
        /// 登陆认证指令
        /// </summary>
        VERIF,
        /// <summary>
        /// 发送测量序号指令
        /// </summary>
        CHANNEL,
        #endregion

        #region 非监测指令
        /// <summary>
        /// 矩形查询请求命令，用于接收经纬度信息
        /// 参数：MapExtent ：地理范围对象
        /// </summary>
        RECTLA,
        /// <summary>
        /// 用于远程控制，如：开启或关闭监测执行站
        /// 参数：string[] : 0 - 设备ID，1 - 开关机状态
        /// </summary>
        /// <remarks>消息格式：RMTP:RCTRL:uid :type=subscribe/ unsubscribe\n</remarks>
        RCTRL,
        /// <summary>
        /// 电源开关命令
        /// </summary>
        /// <remarks>消息格式：RMTP:POWER:uid:receiver=on/off,pic=on/off,controller=on/off,router=on/off\n</remarks>
        POWER,
        /// <summary>
        /// 设备能力信息
        /// 参数：string[] : 0-UID，1-生产厂商，2-型号
        /// </summary>
        DEVCAP,
        /// <summary>
        /// TDOA定位
        /// 参数：string[] : 0-监测频率，1-中频带宽，2-采样点数，3-定位方法，4-设备列表
        /// </summary>
        TDOA,
        /// <summary>
        /// 音频监听开关请求命令
        /// </summary>
        AUDIO,
        /// <summary>
        /// 任务更新通知
        /// 参数：无
        /// </summary>
        TASKU,
        /// <summary>
        /// 监测群查询，返回群内所有监测站的信息
        /// </summary>
        QHAM,
        /// <summary>
        /// GSM-R告警信息查询
        /// </summary>
        GALARM,
        /// <summary>
        /// GSM-R解码信息查询
        /// </summary>
        GDECODE,
        /// <summary>
        /// GSM-R IQ数据查询
        /// </summary>
        GIQFQ,
        /// <summary>
        /// GSM-R 更新模板
        /// </summary>
        GTPLU,
        /// <summary>
        /// GSM-R 设置干扰判定门限
        /// </summary>
        GATHD,
        /// <summary>
        /// GSM-R占用度查询
        /// </summary>
        GOCUPY,
        /// <summary>
        /// GSM-R频谱数据查询
        /// </summary>
        GSPCTRUM
        #endregion

        ///// <summary>
        ///// 发送XMPP消息到Ploy。样例：ExecuteCommand(RmtpCommand.PostMessageToPloy, new object[] { XmppMessageType.Task, "<update/>" });
        ///// </summary>
        //PostMessageToPloy
    }
    /// <summary>
    /// XMPP消息类型
    /// </summary>
    public enum XmppMessageContextType
    {
        /// <summary>
        /// 任务
        /// </summary>
        Task
    }
    /// <summary>
    /// 执行结果
    /// </summary>
    public enum ExecuteResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESSED,
        /// <summary>
        /// 拒绝，该业务功能已被优先级更高的请求占用
        /// </summary>
        REFUSE,
        /// <summary>
        /// 失败，由于设备限制或其他原因，当前监测执行站不支持请求的功能
        /// </summary>
        FAILURE,
        /// <summary>
        /// 操作执行成功，可以返回数据，但存在问题
        /// </summary>
        WARNING
    }

    public static class ExecuteResultMessage
    {
        public const string REFUSE = "拒绝，该业务功能已被优先级更高的请求占用";
        public const string FAILURE = "失败，由于设备限制或其他原因，当前监测执行站不支持请求的功能";
        public const string WARNING = "操作执行成功，可以返回数据，但存在问题";
        public const string ManySend = "多次发送预执行指令";
        public const string Executing = "有同样或更高优先级的指令在执行";
        public const string NotReceivedCmd = "未收到预执行指令";
        public const string ParamError = "测量参数错误";
        public const string Unknown = "未知错误";
    }

    /// <summary>
    /// 执行结果返回的错误编码
    /// </summary>
    public enum ExecuteResultErrorCode
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        未知错误 = 0,
        /// <summary>
        /// 该业务功能已被优先级更高的请求占用
        /// </summary>
        已占用 = 1,
        /// <summary>
        /// 未收到预执行指令
        /// </summary>
        未收到指令,
        /// <summary>
        /// 测量参数错误
        /// </summary>
        参数错误,
        /// <summary>
        /// 设备已经停止
        /// </summary>
        已停止,
        /// <summary>
        /// 该业务功能被优先级更高的请求抢占
        /// </summary>
        已抢占,
        /// <summary>
        /// 该业务功能执行时间超时
        /// </summary>
        超时,
        /// <summary>
        /// 7,转发服务器离线
        /// </summary>
        转发服务器离线
    }

    /// <summary>
    /// 测量数据类型
    /// </summary>
    public enum MeasureDataType
    {
        /// <summary>
        /// 实时
        /// </summary>
        RealTime = 0,
        /// <summary>
        /// 最大值
        /// </summary>
        Max,
        /// <summary>
        /// 最小值
        /// </summary>
        Min,
        /// <summary>
        /// 平均
        /// </summary>
        Ave,
        /// <summary>
        /// 电磁背景
        /// </summary>
        Background,
        /// <summary>
        /// 背景噪声
        /// </summary>
        Noise
    }

    /// <summary>
    /// 预学习时长
    /// </summary>
    public enum LearnTime
    {
        /// <summary>
        /// 10
        /// </summary>
        one,
        /// <summary>
        /// 20
        /// </summary>
        two,
        /// <summary>
        /// 30
        /// </summary>
        three,
        /// <summary>
        /// 40
        /// </summary>
        four,
        /// <summary>
        /// 50
        /// </summary>
        five,
        /// <summary>
        /// 60
        /// </summary>
        six,
    }
    /// <summary>
    /// 门限方式
    /// </summary>
    public enum ThresholdMode
    {
        /// <summary>
        /// 电磁背景
        /// </summary>
        background,
        /// <summary>
        /// 水平线
        /// </summary>
        horizontal
    }
    /// <summary>
    /// 信号门限
    /// </summary>
    public enum SignalShold
    {
        /// <summary>
        /// 电磁背景
        /// </summary>
        background,
        /// <summary>
        /// 水平线
        /// </summary>
        horizontal,
        /// <summary>
        /// 背景噪声
        /// </summary>
        noise
    }
    /// <summary>
    /// 带宽测量模式
    /// </summary>
    public enum Bandmode
    {
        XdB,
        BETA,
    }

    /// <summary>
    /// Gsmr模版枚举
    /// </summary>
    public enum GsmrModelTpye
    {
        CDMA载波模板 = 1,
        GSM载波模板 = 2,
        GSMR载波模板 = 3,
        GSMR频段下行信号强度模板 = 4,
        GSMR频段上行底噪模板 = 5,
        全频段信号强度模板 = 6
    }

    /// <summary>
    /// 极化方式
    /// </summary>
    public enum Polarization
    {
        /// <summary>
        /// 水平极化
        /// </summary>
        horizontal,
        /// <summary>
        /// 垂直极化
        /// </summary>
        vertical,
        /// <summary>
        /// 圆极化
        /// </summary>
        circle
    }

    /// <summary>
    /// 频段扫描模版
    /// </summary>
    public enum FreqModle
    {
        /// <summary>
        /// 上行频段
        /// </summary>
        up,
        /// <summary>
        /// 下行频段
        /// </summary>
        down,
        /// <summary>
        /// 全频段
        /// </summary>
        all
    }
    /// <summary>
    /// 平滑方式
    /// </summary>
    public enum SmoothWay
    {
        none,
        ///// <summary>
        ///// 平均
        ///// </summary>
        //avg,
        /// <summary>
        /// 均方差
        /// </summary>
        rms,
        /// <summary>
        /// 峰值
        /// </summary>
        peak
    }
    /// <summary>
    /// 解调方式
    /// </summary>
    public enum DemodMode
    {
        FWM,
        FM,
        AM,
        PM,
        LSB,
        USB,
        SSB,
        CW
    }
    /// <summary>
    /// 检波方式
    /// </summary>
    public enum Detector
    {
        /// <summary>
        /// 峰值
        /// </summary>
        peak,
        /// <summary>
        /// 平均值
        /// </summary>
        avg,
        /// <summary>
        /// 实时值
        /// </summary>
        fast,
        /// <summary>
        /// 均方根
        /// </summary>
        rms,
        /// <summary>
        /// 准峰值
        /// </summary>
        qbk
    }

    /// <summary>
    /// 测向模式
    /// </summary>
    public enum DirectionMeasurementMode
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 连续
        /// </summary>
        Continue,
        /// <summary>
        /// 脉冲模式
        /// </summary>
        Gate
    }

    public enum YesNo
    {
        yes,
        no
    }
    /// <summary>
    /// 仪表工作模式
    /// </summary>
    public static class MeterMode
    {
        /// <summary>
        /// 未知
        /// </summary>
        public const byte None = 0x00;
        /// <summary>
        /// 宽带测量描述头和结果
        /// </summary>
        public const byte METER_WBFQ = 0x10;
        /// <summary>
        /// 扩展的宽频测量描述头和结果
        /// </summary>
        public const byte METER_WBFQEX = 0x11;
        /// <summary>
        /// 中频测量描述头和结果
        /// </summary>
        public const byte METER_IFFQ = 0x20;
        /// <summary>
        /// 扩展的中频测量描述头和结果
        /// </summary>
        public const byte METER_IFFQEX = 0x21;
        /// <summary>
        /// 固频测量描述头和结果
        /// </summary>
        public const byte METER_FIXFQ = 0x30;
        /// <summary>
        /// 扩展的固频测量描述头和结果
        /// </summary>
        public const byte METER_FIXFQEX = 0x31;
        /// <summary>
        /// TDOA测量描述头和结果
        /// </summary>
        public const byte METER_TDOA = 0x40;
        /// <summary>
        /// 时间测量描述头和结果
        /// </summary>
        public const byte METER_COOR = 0x50;
        /// <summary>
        /// 位置测量描述头和结果
        /// </summary>
        public const byte METER_TIME = 0x60;
        /// <summary>
        /// IQ测量描述头和结果
        /// </summary>
        public const byte METER_IQ = 0x70;
        /// <summary>
        /// 音频数据
        /// </summary>
        public const byte METER_AUDIO = 0x80;
        /// <summary>
        /// 音频数据头
        /// </summary>
        public const byte METER_AUDIOHEAD = 0x81;
        /// <summary>
        /// 离散扫描测量描述头和结果
        /// </summary>
        public const byte METER_MSCAN = 0x90;
        /// <summary>
        /// 频段扫描测量描述头和结果
        /// </summary>
        public const byte METER_FSCAN = 0xa0;
        /// <summary>
        /// 中频测向描述头和结果
        /// </summary>
        public const byte METER_IFDF = 0xb0;
        /// <summary>
        /// 宽带测向描述头和结果
        /// </summary>
        public const byte METER_WBDF = 0xc0;
        /// <summary>
        /// 天线因子
        /// </summary>
        public const byte METER_AntennaFactor = 0xd0;
        /// <summary>
        /// GSM-R小站干扰告警
        /// </summary>
        public const byte METER_GsmrInterfereAlarm = 0xe0;
        /// <summary>
        /// GSM-R小站解码信息
        /// </summary>
        public const byte METER_GsmrDecodeInfo = 0xe1;
        /// <summary>
        /// GSM-R小站IQ数据
        /// </summary>
        public const byte METER_GsmrIqData = 0xe2;
        /// <summary>
        /// GSM-R小站载干比(C/I)信息
        /// </summary>
        public const byte METER_GsmrCiInfo = 0xe3;
        /// <summary>
        /// GSM-R小站模板数据
        /// </summary>
        public const byte METER_GsmrTemplateData = 0xe4;
        /// <summary>
        /// GSM-R小站占用度信息
        /// </summary>
        public const byte METER_GsmrOccupancyInfo = 0xe5;
        /// <summary>
        /// GSM-R小站频谱数据
        /// </summary>
        public const byte METER_GsmrSpectrumData = 0xe6;
    }
    /// <summary>
    /// 帧尾数据类型
    /// </summary>
    public static class RmtpDataFrameEndType
    {
        public const byte RMTP身份认证 = 0x00;
        public const byte RMTP身份认证结果 = 0x01;
        public const byte RMTP指令执行结果 = 0x02;
        public const byte RMTP参数修改 = 0x10;
        public const byte RMTP音频开关 = 0x11;
        public const byte RMTP测量请求 = 0x20;
        public const byte RMTP宽频测量指令 = 0x30;
        public const byte RMTP宽频测量参数 = 0x31;
        public const byte RMTP中频测量指令 = 0x40;
        public const byte RMTP中频测量参数 = 0x41;
        public const byte RMTP固频测量指令 = 0x50;
        public const byte RMTP固频测量参数 = 0x51;
        public const byte RMTP扩展的中频测量指令 = 0x60;
        public const byte RMTP扩展的中频测量参数 = 0x61;
        public const byte RMTP中频测向参数 = 0xc0;
        public const byte RMTP中频测向结果 = 0xc1;
        public const byte RMTP扩展的宽频测量指令 = 0x70;
        public const byte RMTP扩展的宽频测量参数 = 0x71;
        public const byte RMTPIQ数据测量指令 = 0x80;
        public const byte RMTPIQ数据测量参数 = 0x81;
        public const byte RMTP扩展的固频测量指令 = 0x90;
        public const byte RMTP扩展的固频测量参数 = 0x91;
        public const byte RMTP离散扫描参数 = 0xa0;
        public const byte RMTP离散扫描结果 = 0xa1;
        public const byte RMTP频段扫描参数 = 0xb0;
        public const byte RMTP频段扫描结果 = 0xb1;
        public const byte RMTP宽带测向指令 = 0xd0;
        public const byte RMTP宽带测向参数 = 0xd1;

        public const byte GSMR小站干扰告警 = 0xE0;
        public const byte GSMR小站解码信息 = 0xE1;
        public const byte GSMR小站IQ数据 = 0xE2;
        public const byte GSMR载干比信息 = 0xE3;
        public const byte GSMR小站模板数据 = 0xE4;
        public const byte GSMR占用度信息 = 0xE5;
        public const byte GSMR频谱数据 = 0xE6;

        public const byte RMTP停止指令 = 0xff;
    }

    #region 经纬度信息属性
    /// <summary>
    /// 经纬度信息属性关键字
    /// </summary>
    //public class LonLatProperty
    //{
    //    public const string 在线状态 = "PRESENCE";
    //    public const string 设备名称 = "DEVICENAME";
    //    public const string 设备型号 = "DEVICEMODEL";
    //    public const string 设备类型 = "DEVICECATEGORY";
    //    public const string 设备状态 = "DEVICESTATE";
    //    public const string 生产厂商 = "DEVICEFACTORY";
    //    public const string 设备ID = "MONITOR{0}";
    //    public const string 地址 = "DEVICEADDRESS";
    //    public const string 工作状态 = "DEVICESTATE";
    //    public const string 组ID = "GROUPID";
    //    public const string 组名称 = "GROUPNAME";

    //}
    public class LonLatProperty
    {
        public const string 站点类型 = "STATIONTYPE";
        public const string 在线状态 = "PRESENCE";
        public const string 设备名称 = "DEVICENAME";
        public const string 设备型号 = "DEVICEMODEL";
        public const string 设备类型ID = "DEVICECATEGORYID";//(设备类型ID: 1.接收机,2.测向机,3.一体机)
        public const string 设备类型 = "DEVICECATEGORY";
        public const string 生产厂商 = "DEVICEFACTORY";
        //public const string 设备ID = "MONITOR{0}";
        public const string 地址 = "DEVICEADDRESS";
        public const string 工作状态 = "DEVICESTATE";
        public const string 组ID = "GROUPID";
        public const string 组名称 = "GROUPNAME";
        public const string 版本 = "VERSION";
        public const string 设备ID = "DEVICEID";
        public const string 电源ID = "POWERID";
        public const string 设备IP = "LOCALIP";
        public const string 拨号IP = "VPNIP";
        public const string 出口IP = "OUTIP";
        public const string P2P端口 = "P2PPORT";
        public const string 任务ID = "TASKID";//(任务ID，正值为监测任务，无为停止测量，负值时为实时测量)
        public const string 任务名称 = "TASKNAME";
        public const string 占用人 = "OWNER";//(正在占用设备的用户)
        public const string 监测模式 = "FUNCTION";
        public const string 测量开始时间 = "STARTTIME";//(测量开始时间，格式为yyyymmddhhmmss)
        public const string 测量结束时间 = "STOPTIME";//(测量结束时间)
        public const string 下一任务ID = "NEXTTID";//(下一个任务ID)
        public const string 品牌名称 = "BRANDNAME";
        public const string 车辆号码 = "AUTONUMBER";
        public const string 第三方公司名 = "INTEGRATOR";
        public const string 在线监测站数 = "ONLINESTATIONS";
        public const string 离线监测站数 = "OFFLINESTATIONS";
    }

    /// <summary>
    /// GSM-Rg告警信息属性关键字
    /// </summary>
    public class AlarmProperty
    {
        public const string 小站ID = "stationid";
        public const string 通道号 = "channelid";
        public const string 干扰类型 = "alarmtype";
        public const string 干扰编号 = "alarmid";
        public const string 干扰时间 = "alarmtime";
        public const string 起始频率 = "startfreq";
        public const string 结束频率 = "stopfreq";
        public const string 信号强度 = "strength";
        public const string LAC = "lac";
        public const string CI = "ci";
    }
    #endregion

    public static class RmtpDefaultCollection
    {
        public static List<Bandmode> BandmodeSource
        {
            get
            {
                List<Bandmode> bms = new List<Bandmode>();
                bms.Add(Rmtp.Bandmode.XdB);
                bms.Add(Rmtp.Bandmode.BETA);
                return bms;
            }
        }

        //public static List<DemodMode> DemodModeSource
        //{
        //    get
        //    {
        //        List<DemodMode> dms = new List<DemodMode>();
        //        dms.Add(Rmtp.DemodMode.AM);
        //        //dms.Add(Rmtp.DemodMode.CW);
        //        dms.Add(Rmtp.DemodMode.FM);
        //        //dms.Add(Rmtp.DemodMode.FWM);
        //        //dms.Add(Rmtp.DemodMode.LSB);
        //        dms.Add(Rmtp.DemodMode.PM);
        //        //dms.Add(Rmtp.DemodMode.SSB);
        //        //dms.Add(Rmtp.DemodMode.USB);
        //        return dms;
        //    }
        //}

        public static Dictionary<string, DemodMode> DemodModeSource
        {
            get
            {
                Dictionary<string, DemodMode> dms = new Dictionary<string, DemodMode>();
                dms.Add("AM", Rmtp.DemodMode.AM);
                dms.Add("FM", Rmtp.DemodMode.FM);
                dms.Add("PM", Rmtp.DemodMode.PM);
                return dms;
            }
        }

        public static Dictionary<string, Detector> DetectorSource
        {
            get
            {
                Dictionary<string, Detector> ds = new Dictionary<string, Detector>();
                ds.Add("平均值", Rmtp.Detector.avg);
                ds.Add("实时值", Rmtp.Detector.fast);
                ds.Add("峰值", Rmtp.Detector.peak);
                ds.Add("准峰值", Rmtp.Detector.qbk);
                ds.Add("均方根", Rmtp.Detector.rms);
                return ds;
            }
        }

        public static Dictionary<string, double> Gsmr4MHzRbwSource
        {
            get
            {
                Dictionary<string, double> ps = new Dictionary<string, double>();
                ps.Add("25kHz", 25);
                ps.Add("12.5kHz", 12.5);
                ps.Add("5kHz", 5);
                ps.Add("1kHz", 1);
                return ps;
            }
        }

        public static Dictionary<string, FreqModle> Gsmr4FrequencyRangeModleSource
        {
            get
            {
                Dictionary<string, FreqModle> ps = new Dictionary<string, FreqModle>();
                ps.Add("GSM-R上行频段", Rmtp.FreqModle.up);
                ps.Add("GSM-R下行频段", Rmtp.FreqModle.down);
                ps.Add("全频段", Rmtp.FreqModle.all);
                return ps;
            }
        }

        public static Dictionary<string, Polarization> PolarizationSource
        {
            get
            {
                Dictionary<string, Polarization> ps = new Dictionary<string, Polarization>();
                ps.Add("水平极化", Rmtp.Polarization.horizontal);
                ps.Add("垂直极化", Rmtp.Polarization.vertical);
                ps.Add("圆极化", Rmtp.Polarization.circle);
                return ps;
            }
        }

        /// <summary>
        /// 测向模式
        /// </summary>
        public static Dictionary<string, DirectionMeasurementMode> DmmSource
        {
            get
            {
                var dmms = new Dictionary<string, DirectionMeasurementMode>();
                dmms.Add("正常", DirectionMeasurementMode.Normal);
                dmms.Add("连续", DirectionMeasurementMode.Continue);
                dmms.Add("脉冲", DirectionMeasurementMode.Gate);

                return dmms;
            }
        }

        public static Dictionary<string, YesNo> ModulationSource
        {
            get
            {
                Dictionary<string, YesNo> yn = new Dictionary<string, YesNo>();
                yn.Add("需要", YesNo.yes);
                yn.Add("不需要", YesNo.no);
                return yn;
            }
        }

        public static Dictionary<string, SmoothWay> SmoothWaySource
        {
            get
            {
                Dictionary<string, SmoothWay> sw = new Dictionary<string, SmoothWay>();
                sw.Add("实时", SmoothWay.none);
                sw.Add("均方根", SmoothWay.rms);
                sw.Add("峰值", SmoothWay.peak);
                return sw;
            }
        }

        public static Dictionary<string, LearnTime> LearnTimeSource
        {
            get
            {
                Dictionary<string, LearnTime> ls = new Dictionary<string, LearnTime>();
                ls.Add("10", Rmtp.LearnTime.one);
                ls.Add("20", Rmtp.LearnTime.two);
                ls.Add("30", Rmtp.LearnTime.three);
                ls.Add("40", Rmtp.LearnTime.four);
                ls.Add("50", Rmtp.LearnTime.five);
                ls.Add("60", Rmtp.LearnTime.six);
                return ls;
            }
        }

        public static Dictionary<string, SignalShold> SignalSholdSource
        {
            get
            {
                Dictionary<string, SignalShold> ss = new Dictionary<string, SignalShold>();
                ss.Add("电磁背景", Rmtp.SignalShold.background);
                ss.Add("水平线", Rmtp.SignalShold.horizontal);
                ss.Add("背景噪声", Rmtp.SignalShold.noise);
                return ss;
            }
        }

        public static Dictionary<string, ThresholdMode> ThresholdModeSource
        {
            get
            {
                Dictionary<string, ThresholdMode> tm = new Dictionary<string, ThresholdMode>();
                tm.Add("水平线", Rmtp.ThresholdMode.horizontal);
                tm.Add("电磁背景", Rmtp.ThresholdMode.background);
                return tm;
            }
        }

        public static Dictionary<string, GainMode> GainModeSource
        {
            get
            {
                Dictionary<string, GainMode> sw = new Dictionary<string, GainMode>();
                sw.Add("手动增益", GainMode.mgc);
                sw.Add("自动增益", GainMode.agc);
                return sw;
            }
        }

        public static Dictionary<string, HistoryMonitorMode> HistoryMonitorModeSource
        {
            get
            {
                Dictionary<string, HistoryMonitorMode> sw = new Dictionary<string, HistoryMonitorMode>();
                //sw.Add("全部", HistoryMonitorMode.All);
                sw.Add("射频全景", HistoryMonitorMode.WBQEX);
                sw.Add("频段扫描", HistoryMonitorMode.FSCAN);
                sw.Add("离散扫描", HistoryMonitorMode.MSCAN);
                sw.Add("中频扫描",HistoryMonitorMode.IFQEX);
                return sw;
            }
        }
    }
    /// <summary>
    /// 设备参数关键字
    /// </summary>
    public static class DeviceParameter
    {
        public const string 频率 = "frequency";
        public const string 中频带宽 = "ifbw";
        public const string 跨距 = "span";
        public const string 解调模式 = "demodmode";
        public const string 自动频率控制 = "afc";
        public const string 中频衰减模式 = "ifattmode";
        public const string 测量时间 = "measuretime";
        public const string 检波方式 = "detector";
        public const string 增益控制模式 = "gainctrl";
        public const string 手动增益控制 = "mgcvalue";
        public const string 静噪门限 = "squelchthreshold";
        public const string 录音门限 = "recordshold";
        public const string 驻留时间 = "holdtime";
        public const string 保持时间 = "keeptime";
        public const string 射频衰减 = "rfattenuation";
        public const string 带宽测量模式 = "bandmode";
        public const string 下降dB数 = "xdb";
        public const string Beta带宽 = "betapercent";
        public const string 起始频率 = "startfreq";
        public const string 终止频率 = "stopfreq";
        public const string 扫描步长 = "step";
        public const string 扫描次数 = "scancount";
        public const string 电平量程 = "levelrange";
        public const string 参考电平 = "reflevel";
        public const string 测量模式 = "displaymode";
        public const string 前置放大器 = "preamplifier";
        public const string 测向模式 = "dfmode";
        public const string 测向带宽 = "dfbw";
        public const string 积分时间 = "integrationtime";
        public const string 频段范围 = "freqrange";
        public const string 质量门限 = "qualityshold";
        public const string 电平门限 = "levelshold";
        public const string 分辨率带宽 = "rbw";
        public const string 视频带宽 = "vbw";
        public const string 扫描时间 = "sweeptime";
        public const string 极化方式 = "polarization";
        public const string 调制方式 = "modulation";
    }
    /// <summary>
    /// 设备数据种类
    /// </summary>
    public static class DeviceDataType
    {
        private static readonly Dictionary<string, Tuple<string, string>> DataTypeDic = InitDataTypeDic();
        private static Dictionary<string, Tuple<string, string>> InitDataTypeDic()
        {
            Dictionary<string, Tuple<string, string>> dic = new Dictionary<string, Tuple<string, string>>();
            dic["level"] = new Tuple<string, string>("dBμV", "电平");
            dic["fieldstrength"] = new Tuple<string, string>("dBμV /m", "场强");
            dic["freqdev"] = new Tuple<string, string>("kHz", "频偏");
            dic["elvangle"] = new Tuple<string, string>("°", "俯仰角");
            dic["freqoffset"] = new Tuple<string, string>("kHz", "频差");
            dic["freqdevpos"] = new Tuple<string, string>("kHz", "正频偏");
            dic["freqdevneg"] = new Tuple<string, string>("kHz", "负频偏");
            dic["ampdepth"] = new Tuple<string, string>("%", "调幅度");
            dic["ampdepthpos"] = new Tuple<string, string>("%", "正调幅度");
            dic["ampdepthneg"] = new Tuple<string, string>("%", "负调幅度");
            dic["phasedev"] = new Tuple<string, string>("Rad", "相偏");
            dic["pobw"] = new Tuple<string, string>("kHz", "占用带宽");// "%功率占用带宽");
            dic["xdbobw"] = new Tuple<string, string>("kHz", "XdB带宽");//  "下降XdB带宽");
            dic["modulation"] = new Tuple<string, string>("", "调制方式");//后加的数据种类
            dic["freq"] = new Tuple<string, string>("", "中心频率");
            return dic;
        }
        /// <summary>
        /// 获取设备数据种类
        /// </summary>
        /// <param name="pDataTypeId">设备数据种类ID</param>
        /// <returns></returns>
        public static Tuple<string, string> GetDeviceDataType(string pDataTypeId)
        {
            int i = pDataTypeId.IndexOf('\0');
            if (i > 0)
            {
                pDataTypeId = pDataTypeId.Substring(0, i);
            }

            if (!DataTypeDic.ContainsKey(pDataTypeId.ToLower()))
                return null;
            return DataTypeDic[pDataTypeId];
        }
    }
    /// <summary>
    /// 设备数据种类关键字
    /// </summary>
    public static class DeviceDataTypeId
    {
        /// <summary>
        /// 电平
        /// </summary>
        public const string level = "level";
        /// <summary>
        /// 场强
        /// </summary>
        public const string fieldstrength = "fieldstrength";
        /// <summary>
        /// 频偏
        /// </summary>
        public const string freqdev = "freqdev";
        /// <summary>
        /// 俯仰角
        /// </summary>
        public const string elvangle = "elvangle";
        /// <summary>
        /// 频差
        /// </summary>
        public const string freqoffset = "freqoffset";
        /// <summary>
        /// 正频偏
        /// </summary>
        public const string freqdevpos = "freqdevpos";
        /// <summary>
        /// 负频偏
        /// </summary>
        public const string freqdevneg = "freqdevneg";
        /// <summary>
        /// 调幅度
        /// </summary>
        public const string ampdepth = "ampdepth";
        /// <summary>
        /// 正调幅度
        /// </summary>
        public const string ampdepthpos = "ampdepthpos";
        /// <summary>
        /// 负调幅度
        /// </summary>
        public const string ampdepthneg = "ampdepthneg";
        /// <summary>
        /// 相偏
        /// </summary>
        public const string phasedev = "phasedev";
        /// <summary>
        /// %功率占用带宽
        /// </summary>
        public const string pobw = "pobw";
        /// <summary>
        /// 下降XdB带宽
        /// </summary>
        public const string xdbobw = "xdbobw";

    }
    /// <summary>
    /// 调制方式类型
    /// </summary>
    public enum DemodType
    {
        AM = 1,
        FM,
        WFM,
        LSB,
        USB,
        ISB,
        ASK,
        FSK,
        PSK,
        MPSK,
        MFSK,
        GMSK,
        MSK,
        TFM,
        PCM,
        QAM,
        CFM,
        CM,
        CNM,
        LFM,
        NLFM,
        BPC,
        QPC,
        FC,
        PNM,
        PM,
        BPSK,
        QPSK,
        _8PSK,  /* 对应 8PSK */
        SSB,
        DSB,
        RSB,
        PAM,
        OFDM,
        PWM,
        CW,
        IQ,
        QS
    }
    /// <summary>
    /// 设备返回数据
    /// </summary>
    public class DeviceData : INotifyPropertyChanged
    {
        private Single _value;
        /// <summary>
        /// 数据种类ID
        /// </summary>
        public string ID
        {
            get;
            set;
        }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            get;
            set;
        }
        /// <summary>
        /// 数据种类中文说明（不属于数据帧一部分）
        /// </summary>
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// 该数据种类的缺省最小值
        /// </summary>
        public Single Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        protected void OnPropertyChanged(string p_propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p_propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public static DeviceData AnalysisDataClass(RmtpDataFrame pValue, ref int index)
        {
            DeviceData dd = new DeviceData();
            dd.ID = System.Text.Encoding.UTF8.GetString(pValue.Data, index, 16);
            int idIndex = dd.ID.IndexOf('\0');
            if (idIndex < 0)
            {
                //XGZ:错误：StartIndex 不能小于 0。
                return null;
            }
            dd.ID = dd.ID.Remove(idIndex);
            Tuple<string, string> deviceData = DeviceDataType.GetDeviceDataType(dd.ID);
            if (deviceData != null)
                dd.Description = deviceData.Item2;
            index += 16;
            dd.Value = BitConverter.ToSingle(pValue.Data, index);
            index += 4;
            dd.Unit = System.Text.Encoding.UTF8.GetString(pValue.Data, index, 10);
            int uIndex = dd.Unit.IndexOf('\0');
            dd.Unit = dd.Unit.Remove(uIndex);
            if (deviceData == null && dd.ID == "frequency")
            {
                dd.Description = "频率";
            }
            index += 10;
            return dd;
        }
    }
    /// <summary>
    /// 设备返回数据种类关键字
    /// </summary>
    public static class WBQEXDataTypeEnglishNames
    {
        /// <summary>
        /// 电平
        /// </summary>
        public const string level = "level";
        /// <summary>
        /// 场强
        /// </summary>
        public const string fieldstrength = "fieldstrength";
        /// <summary>
        /// 频偏
        /// </summary>
        public const string freqdev = "freqdev";
        /// <summary>
        /// 俯仰角
        /// </summary>
        public const string elvangle = "elvangle";
        /// <summary>
        /// 频差
        /// </summary>
        public const string freqoffset = "freqoffset";
        /// <summary>
        /// 正频偏
        /// </summary>
        public const string freqdevpos = "freqdevpos";
        /// <summary>
        /// 负频偏
        /// </summary>
        public const string freqdevneg = "freqdevneg";
        /// <summary>
        /// 调幅度
        /// </summary>
        public const string ampdepth = "ampdepth";
        /// <summary>
        /// 正调幅度
        /// </summary>
        public const string ampdepthpos = "ampdepthpos";
        /// <summary>
        /// 负调幅度
        /// </summary>
        public const string ampdepthneg = "ampdepthneg";
        /// <summary>
        /// 相偏
        /// </summary>
        public const string phasedev = "phasedev";
        /// <summary>
        /// %功率占用带宽
        /// </summary>
        public const string pobw = "pobw";
        /// <summary>
        /// 下降XdB带宽
        /// </summary>
        public const string xdbobw = "xdbobw";

    }
    /// <summary>
    /// 频率单位
    /// </summary>
    public enum FreqUnit
    {
        GHz,
        MHz,
        kHz,
        Hz
    }
    /// <summary>
    /// 监测测量状态
    /// </summary>
    public class MonitorMeasureState
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        //public string DeviceId { get; set; }
        /// <summary>
        /// 测量序号
        /// </summary>
        public int MeasureSerialNumber { get; set; }
        /// <summary>
        /// 正在进行的控制的优先级代码
        /// </summary>
        public int Pricode { get; set; }
        /// <summary>
        /// 执行任务的设备通道号
        /// </summary>
        public int Channelid { get; set; }
    }
    public enum GainMode
    {
        /// <summary>
        /// 自动增益
        /// </summary>
        agc,
        /// <summary>
        /// 人工增益
        /// </summary>
        mgc
    }

    /// <summary>
    /// 历史回放监测模式
    /// </summary>
    public enum HistoryMonitorMode
    {
        /// <summary>
        /// 全部
        /// </summary>
        //All,
        /// <summary>
        /// 射频全景
        /// </summary>
        WBQEX,
        /// <summary>
        /// 频段扫描
        /// </summary>
        FSCAN,
        /// <summary>
        /// 离散扫描
        /// </summary>
        MSCAN,
        /// <summary>
        /// 中频扫描
        /// </summary>
        IFQEX
    }
}
