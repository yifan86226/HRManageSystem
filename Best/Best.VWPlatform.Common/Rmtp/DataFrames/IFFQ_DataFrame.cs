using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// 中频测量数据帧
    /// </summary>
    /// <remarks>
    /// 1、如果中心频率在测量过程中发生改变，中心频率应该反映该变化
    /// 2、场强单位为dBμV/m电平值单位为dBμV，用整数表示，固定后两位表示小数
    /// </remarks>
    public class IffqDataFrame : RmtpDataFrame
    {
        #region 私有

        private Int32 _mPpqxds;
        #endregion

        public IffqDataFrame()
        {
            Header.DataType = RmtpDataTypes.业务数据;
        }

        #region 公有
        /// <summary>
        /// 实际场强或电平*100
        /// </summary>
        public Int16 中心点数据值
        {
            get;
            set;
        }

        public Int32 频谱曲线点数
        {
            get
            {
                return _mPpqxds;
            }
            set
            {
                _mPpqxds = value;
                数据值 = new Int16[_mPpqxds];
            }
        }

        public Int64 中心频率
        {
            get;
            set;
        }
        /// <summary>
        /// 频谱数据类型，2字节整数 (0：实时，1：最大，2：最小，3：平均)
        /// </summary>
        public Int16 频谱数据类型
        {
            get;
            internal set;
        }
        /// <summary>
        /// n=频谱曲线点数
        /// </summary>
        public Int16[] 数据值
        {
            get;
            internal set;
        }
        #endregion

        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            中心点数据值 = BitConverter.ToInt16(pDataFrame.Data, 0);
            频谱曲线点数 = BitConverter.ToInt32(pDataFrame.Data, 2);
            中心频率 = BitConverter.ToInt64(pDataFrame.Data, 6);
            int index = 14;
            for (Int32 i = 0; i < 频谱曲线点数; i++)
            {
                数据值[i] = BitConverter.ToInt16(pDataFrame.Data, index);
                index += 2;
            }
        }
    }

    /// <summary>
    /// 扩展中频测量数据帧
    /// </summary>
    public class IffqexDataFrame : IffqDataFrame
    {
        private Int16 _ifqexCount;
        /// <summary>
        /// IFQEX数据数量n，2字节整数(无ITU测量结果数据时为零)
        /// </summary>
        public Int16 IfqexCount
        {
            get
            {
                return _ifqexCount;
            }
            internal set
            {
                _ifqexCount = value;
                IfqexData = new Single[value];
            }
        }

        /// <summary>
        /// 设备数据种类数值，对应描述头中返回的数据种类顺序
        /// </summary>
        public Single[] IfqexData
        {
            get;
            internal set;
        }
        /// <summary>
        /// 转换为扩展中频测量结果
        /// </summary>
        /// <param name="pDataFrame"></param>
        /// <param name="pParameter"></param>
        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            int index = 0;
            var pDescribeHeader = pParameter as IffqexDescribeHeader;
            IfqexCount = BitConverter.ToInt16(pDataFrame.Data, index);
            频谱曲线点数 = BitConverter.ToInt32(pDataFrame.Data, index += 2);
            中心频率 = BitConverter.ToInt64(pDataFrame.Data, index += 4);
            中心点数据值 = BitConverter.ToInt16(pDataFrame.Data, index += 8);
            频谱数据类型 = BitConverter.ToInt16(pDataFrame.Data, index += 2);
            index += 2;
            for (Int32 i = 0; i < 频谱曲线点数; i++)
            {
                if(index >= pDataFrame.Data.Length)
                {
                    System.Diagnostics.Debug.WriteLine("数据帧大小有问题");
                    return;
                }
                数据值[i] = (Int16)(BitConverter.ToInt16(pDataFrame.Data, index) / 100);  //因senser端做了乘100操作，这里做除100
                index += 2;
            }
            if (pDescribeHeader != null && pDescribeHeader.DataTypeList.Count == IfqexCount)
            {
                for (Int16 i = 0; i < IfqexCount; i++)
                {
                    pDescribeHeader.DataTypeList[i].Value = (float)Utile.MathNoRound(BitConverter.ToSingle(pDataFrame.Data, index), 3);
                    index += 4;
                }
            }
        }
    }

    /// <summary>
    /// 中频测向数据帧
    /// </summary>
    public class IfDmDataFrame : RmtpDataFrame
    {
        public bool Inited { get; set; }

        /// <summary>
        /// 电平
        /// </summary>
        public float ElectricalLevel { get; set; }

        /// <summary>
        /// 场强
        /// </summary>
        public float FieldIntensity { get; set; }

        /// <summary>
        /// 测向质量
        /// </summary>
        public float DmQuality { get; set; }

        /// <summary>
        /// 偏差
        /// </summary>
        public float Deviation { get; set; }

        /// <summary>
        /// 方位角 4字节浮点数（0～360度之间）
        /// </summary>
        public float AzimuthAngle { get; set; }

        /// <summary>
        /// 俯仰角 4字节浮点数（0～90度之间）
        /// </summary>
        public float PitchAngle { get; set; }

        /// <summary>
        /// 相对车头方位角 4字节浮点数（0～360度之间）
        /// </summary>
        public float CompassValue { get; set; }

        /// <summary>
        /// 中心频点数据值
        /// </summary>
        public Int16[] Datas { get; set; }

        /// <summary>
        /// 中心点数据值
        /// </summary>
        public Int16 CenterPointValue { get; set; }

        /// <summary>
        /// 频点数
        /// </summary>
        public int Points { get; set; }

        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            CenterPointValue = BitConverter.ToInt16(pDataFrame.Data, 0);
            Points = BitConverter.ToInt32(pDataFrame.Data, 2);

            Datas = new short[Points];

            var index = 6;
            for (var i = 0; i < Points; i++)
            {
                if (pDataFrame.Data.Length < index + 2) return;

                Datas[i] = (Int16)(BitConverter.ToInt16(pDataFrame.Data, index) / 100);
                index += 2;
            }

            if ((index + 4) < pDataFrame.Data.Length)
            {
                Inited = true;

                ElectricalLevel = BitConverter.ToSingle(pDataFrame.Data, index);
                index += 4;
            }

            if ((index + 4) < pDataFrame.Data.Length)
            {
                FieldIntensity = BitConverter.ToSingle(pDataFrame.Data, index);
                index += 4;
            }

            if ((index + 4) < pDataFrame.Data.Length)
            {
                DmQuality = BitConverter.ToSingle(pDataFrame.Data, index);
                index += 4;
            }

            if ((index + 4) < pDataFrame.Data.Length)
            {
                AzimuthAngle = BitConverter.ToSingle(pDataFrame.Data, index);
                index += 4;
            }

            if ((index + 4) < pDataFrame.Data.Length)
            {
                Deviation = BitConverter.ToSingle(pDataFrame.Data, index);
                index += 4;
            }
            if ((index + 4) < pDataFrame.Data.Length)
            {
                PitchAngle = BitConverter.ToSingle(pDataFrame.Data, index);
                index += 4;
            }

            if ((index + 4) < pDataFrame.Data.Length)
                CompassValue = BitConverter.ToSingle(pDataFrame.Data, index);
        }
    }
}
