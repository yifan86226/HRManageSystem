using Best.VWPlatform.Common.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp
{
    /// <summary>
    /// RMTP协议数据帧尾
    /// </summary>
    public class RmtpDataFrameEnd : ICalculateObjectLength
    {
        /// <summary>
        /// 附加字节，业务数据-》流数据类型，执行结果-》执行过的指令
        /// </summary>
        public byte Tag
        {
            get;
            set;
        }
        /// <summary>
        /// 除检验字段本身外其他部分的CRC校验结果
        /// </summary>
        public Int32 CheckWord
        {
            get;
            set;
        }

        public int GetLength()
        {
            return 5; /*Tag + CheckWord*/
        }

        public void Load(byte[] pBuffer)
        {
            this.Tag = pBuffer[0];
            this.CheckWord = BitConverter.ToInt32(pBuffer, 1);
        }

        public byte[] ToBytes()
        {
            byte[] buffer;
            using (MemoryStream mem = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(mem);
                writer.Write(Tag);
                writer.Write(CheckWord);
                buffer = mem.ToArray();
                writer.Close();
            }
            return buffer;
        }
    }
}
