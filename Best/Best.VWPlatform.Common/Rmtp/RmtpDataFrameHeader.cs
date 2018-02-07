using Best.VWPlatform.Common.Core;
using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp
{
    /// <summary>
    /// RMTP协议数据帧帧头
    /// </summary>
    public class RmtpDataFrameHeader : ICalculateObjectLength
    {
        internal byte[] MidBytes;
        public RmtpDataFrameHeader()
        {
            Timestamp = new RmtpDataFrameTimestamp();
        }
        /// <summary>
        /// 数据帧的起始符号，占用4个字节
        /// </summary>
        public const UInt32 Fh = 0xEEEEEEEE;
        /// <summary>
        /// 数据帧的长度
        /// </summary>
        public UInt16 Length
        {
            get;
            set;
        }
        /// <summary>
        /// 数据帧时间戳
        /// </summary>
        public RmtpDataFrameTimestamp Timestamp
        {
            get;
            set;
        }
        /// <summary>
        /// 数据类型
        /// </summary>
        public RmtpDataTypes DataType
        {
            get;
            set;
        }
        /// <summary>
        /// 设备通道号
        /// </summary>
        public Int32 ChannelId { get; set; }
        /// <summary>
        /// 标签（测量序号）
        /// </summary>
        public Int32 Tag { get; set; }

        public int GetLength()
        {
            return Utile.SizeOf(Fh) + Utile.SizeOf(Length) + Timestamp.GetLength() + 1/*数据类型*/ + 4/*设备通道号*/ + 4/*标签（测量序号）*/;
        }

        public byte[] ToBytes()
        {
            byte[] bytes;
            using (MemoryStream mem = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(mem);
                writer.Write(Fh);
                writer.Write(Length);
                writer.Write(Timestamp.ToBytes());
                writer.Write((Int16)DataType);
                writer.Write(ChannelId);
                writer.Write(Tag);
                bytes = mem.ToArray();
                writer.Close();
            }
            return bytes;
        }

        public bool Load(byte[] pBuffer)
        {
            UInt32 fhval = BitConverter.ToUInt32(pBuffer, 0);
            if (fhval != Fh)
                throw new Exception("帧头格式错误!");
            int pos = Utile.SizeOf(Fh);
            Length = BitConverter.ToUInt16(pBuffer, pos);
            pos += Utile.SizeOf(Length);

            int timestampLen = Timestamp.GetLength();
            byte[] timestampBuffer = new byte[timestampLen];
            Buffer.BlockCopy(pBuffer, pos, timestampBuffer, 0, timestampLen);
            Timestamp.Load(timestampBuffer);

            pos += timestampLen;
            DataType = (RmtpDataTypes)pBuffer[pos];
            pos += 1;
            ChannelId = BitConverter.ToInt32(pBuffer, pos);
            pos += 4;
            Tag = BitConverter.ToInt32(pBuffer, pos);

            return true;
        }
    }

    /// <summary>
    /// 数据帧时间戳，9个字节
    /// </summary>
    public class RmtpDataFrameTimestamp : ICalculateObjectLength
    {
        /// <summary>
        /// 取值：2000-2100
        /// </summary>
        public Int16 WYear
        {
            get;
            set;
        }
        /// <summary>
        /// 取值：1-12
        /// </summary>
        public byte WMonth
        {
            get;
            set;
        }
        /// <summary>
        /// 取值：1-31
        /// </summary>
        public byte WDay
        {
            get;
            set;
        }
        /// <summary>
        /// 取值：0-23
        /// </summary>
        public byte WHour
        {
            get;
            set;
        }
        /// <summary>
        /// 取值：0-59
        /// </summary>
        public byte WMinute
        {
            get;
            set;
        }
        /// <summary>
        /// 取值：0-59
        /// </summary>
        public byte WSecond
        {
            get;
            set;
        }
        /// <summary>
        /// 取值：0-999
        /// </summary>
        public Int16 WMilliseconds
        {
            get;
            set;
        }

        public int GetLength()
        {
            return Utile.SizeOf(WYear) + Utile.SizeOf(WMonth) + Utile.SizeOf(WDay) + Utile.SizeOf(WHour) + Utile.SizeOf(WMinute) + Utile.SizeOf(WSecond) + Utile.SizeOf(WMilliseconds);
        }

        public DateTime Value
        {
            get
            {
                if (WMonth < 1 || WMonth > 12 || WDay < 1 || WDay > 31 || WHour < 0 || WHour > 23 || WMinute < 0 || WMinute > 59 || WSecond < 0 || WSecond > 59 || WMilliseconds < 0 || WMilliseconds > 999)
                    return new DateTime(1, 1, 1, 1, 1, 1, 1);
                return new DateTime(WYear, WMonth, WDay, WHour, WMinute, WSecond, WMilliseconds);
            }
            set
            {
                WYear = (Int16)value.Year;
                WMonth = (byte)value.Month;
                WDay = (byte)value.Day;
                WHour = (byte)value.Hour;
                WMinute = (byte)value.Minute;
                WSecond = (byte)value.Second;
                WMilliseconds = (Int16)value.Millisecond;
            }
        }

        public byte[] ToBytes()
        {
            byte[] bytes;
            using (MemoryStream mem = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(mem);
                writer.Write(WYear);
                writer.Write(WMonth);
                writer.Write(WDay);
                writer.Write(WHour);
                writer.Write(WMinute);
                writer.Write(WSecond);
                writer.Write(WMilliseconds);
                bytes = mem.ToArray();
                writer.Close();
            }
            return bytes;
        }

        public void Load(byte[] pBuffer)
        {
            WYear = BitConverter.ToInt16(pBuffer, 0);
            int pos = Utile.SizeOf(WYear);
            WMonth = pBuffer[pos];
            pos += 1;
            WDay = pBuffer[pos];
            pos += 1;
            WHour = pBuffer[pos];
            pos += 1;
            WMinute = pBuffer[pos];
            pos += 1;
            WSecond = pBuffer[pos];
            pos += 1;
            WMilliseconds = BitConverter.ToInt16(pBuffer, pos);
        }
    }
}
