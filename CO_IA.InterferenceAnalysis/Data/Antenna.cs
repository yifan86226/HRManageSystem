#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：天线信息
 * 日  期：2016-09-07
 * ********************************************************************************/
#endregion
using EMCS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{
    public class Antenna
    {
        /// <summary>
        /// 天线编号
        /// </summary>
        private string antennaID;

        /// <summary>
        /// 天线类型
        /// </summary>
        private string type;

        /// <summary>
        /// 天线型号
        /// </summary>
        private string model;

        /// <summary>
        /// 天线种类(发射或者接收)
        /// </summary>
        private AntennaKindEnum antennaKind;

        /// <summary>
        /// 天线增益
        /// </summary>
        private EMCGainValue gain = new EMCGainValue(EMCGainUnitEnum.dBi, 9);

        /// <summary>
        /// 天线高度
        /// </summary>
        private EMCLengthValue antHeight = new EMCLengthValue();

        /// <summary>
        /// 极化方式
        /// </summary>
        private EMCPolarisationEnum polar = EMCPolarisationEnum.None;

        /// <summary>
        /// 馈线长度
        /// </summary>
        private EMCLengthValue feedLength = new EMCLengthValue();

        /// <summary>
        /// 馈线系统损耗
        /// </summary>
        private EMCDoubleValue feedLoss = new EMCDoubleValue();

        /// <summary>
        /// 工作方位角
        /// </summary>
        private EMCAzimuthValue azimuth = new EMCAzimuthValue();

        /// <summary>
        /// 天线仰角
        /// </summary>
        private EMCDegreeValue angle = new EMCDegreeValue();

        /// <summary>
        /// 构造函数
        /// </summary>
        public Antenna()
        {
            antennaID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">天线类型</param>
        /// <param name="model">天线型号</param>
        /// <param name="gain">天线增益(dBi)</param>
        public Antenna(string type, string model, double gain)
        {
            this.type = type;
            this.model = model;
            this.gain.DBiValue = gain;
        }

        /// <summary>
        /// 获取或设置天线类型
        /// </summary>
        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        /// <summary>
        /// 获取或设置天线型号
        /// </summary>
        public string Model
        {
            get
            {
                return this.model;
            }
            set
            {
                this.model = value;
            }
        }

        /// <summary>
        /// 获取或设置天线增益
        /// </summary>
        public EMCGainValue Gain
        {
            get
            {
                return this.gain;
            }
            set
            {
                this.gain = value;
            }
        }

        /// <summary>
        /// 获取或设置天线增益(dBi)值
        /// </summary>
        public double Gain_dBi
        {
            get
            {
                return this.gain.DBiValue;
            }
            set
            {
                this.gain.DBiValue = value;
            }
        }

        /// <summary>
        /// 获取或设置天线编号
        /// </summary>
        public string AntennaID
        {
            get
            {
                return this.antennaID;
            }
            set
            {
                this.antennaID = value;
            }
        }

        /// <summary>
        /// 获取或设置天线种类
        /// </summary>
        public AntennaKindEnum AntennaKind
        {
            get
            {
                return antennaKind;
            }
            set
            {
                antennaKind = value;
            }
        }

        /// <summary>
        /// 获取或设置天线高度
        /// </summary>
        public EMCLengthValue AntHeight
        {
            get
            {
                return this.antHeight;
            }
            set
            {
                this.antHeight = value;
            }
        }

        /// <summary>
        /// 获取或设置天线极化方式
        /// </summary>
        public EMCPolarisationEnum Polar
        {
            get
            {
                return this.polar;
            }
            set
            {
                this.polar = value;
            }
        }

        /// <summary>
        /// 获取或设置馈线长度
        /// </summary>
        public double FeedLength
        {
            get
            {
                return this.feedLength.MValue;
            }
            set
            {
                this.feedLength.MValue = value;
            }
        }

        /// <summary>
        /// 获取或设置馈线损耗
        /// </summary>
        public EMCDoubleValue FeedLoss
        {
            get
            {
                return this.feedLoss;
            }
            set
            {
                this.feedLoss = value;
            }
        }


        /// <summary>
        /// 获取或设置天线仰角
        /// </summary>
        public EMCDegreeValue Angle
        {
            get
            {
                return this.angle;
            }
            set
            {
                this.angle = value;
            }
        }

        /// <summary>
        /// 获取或设置工作方位角
        /// </summary>
        public EMCAzimuthValue Azimuth
        {
            get
            {
                return this.azimuth;
            }
            set
            {
                this.azimuth = value;
            }
        }

        /// <summary>
        /// 从参数指定的天线加载信息,覆盖当前对象的数据
        /// </summary>
        /// <param name="ant">要加载的参考天线信息</param>
        public void CopyFrom(Antenna ant)
        {
            this.angle.CopyFrom(ant.angle);
            this.antHeight.CopyFrom(ant.antHeight);
            this.azimuth.CopyFrom(ant.azimuth);
            this.feedLength.CopyFrom(ant.feedLength);
            this.feedLoss.CopyFrom(ant.feedLoss);
            this.gain.CopyFrom(ant.gain);
            this.polar = ant.polar;
        }

        /// <summary>
        /// 从参数指定的天线加载信息,选择行加载,保留当前对象已有的合法数据
        /// </summary>
        /// <param name="ant">要加载的参考天线信息</param>
        public void FillFrom(Antenna ant)
        {
            if (!angle.DataInitialized)
            {
                this.angle.CopyFrom(ant.angle);
            }
            if (!this.antHeight.DataInitialized)
            {
                this.antHeight.CopyFrom(ant.antHeight);
            }
            if (!this.azimuth.DataInitialized)
            {
                this.azimuth.CopyFrom(ant.azimuth);
            }
            if (!this.feedLength.DataInitialized)
            {
                this.feedLength.CopyFrom(ant.feedLength);
            }
            if (!this.feedLoss.DataInitialized)
            {
                this.feedLoss.CopyFrom(ant.feedLoss);
            }
            if (!this.gain.DataInitialized)
            {
                this.gain.CopyFrom(ant.gain);
            }
            if (this.polar == EMCPolarisationEnum.None)
            {
                this.polar = ant.polar;
            }
        }
    }

    /// <summary>
    /// 天线类型
    /// </summary>
    [Serializable]
    public enum AntennaKindEnum
    {
        /// <summary>
        /// 发射天线
        /// </summary>
        Send = 1,
        /// <summary>
        /// 接收天线
        /// </summary>
        Receive = 2
    }
}
