using Best.VWPlatform.Common.Core;
using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Best.VWPlatform.Common.Rmtp
{
    public class RmtpDataFrame : ISocketSend
    {
        private int _headerSize;
        protected Dictionary<string, string> TargetProperty = new Dictionary<string, string>();

        public RmtpDataFrame()
        {
            Header = new RmtpDataFrameHeader();
            _headerSize = Header.GetLength();
        }
        /// <summary>
        /// 帧头： 固定17字节，说明该帧数据大小、日期、业务数据类型等
        /// </summary>
        public RmtpDataFrameHeader Header
        {
            get;
            internal set;
        }
        /// <summary>
        /// 数据内容
        /// </summary>
        public byte[] Data
        {
            get;
            internal set;
        }

        /// <summary>
        /// 帧尾：固定4个字节，为除帧尾外数据帧其他部分的CRC校验结果
        /// </summary>
        public RmtpDataFrameEnd End
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="pPropertyName">属性名称</param>
        /// <returns>属性值</returns>
        public string this[string pPropertyName]
        {
            get
            {
                return GetPropertyValue(this, pPropertyName);
            }
            set
            {
                TargetProperty[pPropertyName] = value;
            }
        }
        /// <summary>
        /// 获取数据帧字节数组
        /// </summary>
        public virtual byte[] ToBytes()
        {
            byte[] bytes;
            Header.Length =(UInt16)(/*_headerSize + (*/Data == null ? 0 : Data.Length);
            using (MemoryStream mem = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(mem);
                //writer.Write(Header.ToBytes());
                if (Data != null && Data.Length > 0)
                    writer.Write(Data);
                //writer.Write(End.ToBytes());
                bytes = mem.ToArray();
                writer.Close();
            }
            return bytes;
        }

        /// <summary>
        /// 加载字节流，解析Rmtp数据帧
        /// </summary>
        /// <param name="pBuffer">字节流</param>
        /// <param name="pSurplusBuffer">处理完当前字节流后剩余的字节流</param>
        /// <returns>Rmtp数据帧</returns>
        internal static RmtpDataFrame Load(byte[] pBuffer, out byte[] pSurplusBuffer)
        {
            try
            {
                pSurplusBuffer = null;
                RmtpDataFrame dataFrame = new RmtpDataFrame();
                if (pBuffer == null || pBuffer.Length == 0)
                    return null;

                if (pBuffer.Length < dataFrame._headerSize)
                    return null;
                bool isLoadSuccess = dataFrame.Header.Load(pBuffer);
                if (isLoadSuccess)
                {
                    if (dataFrame.Header.Length <= 0)
                        throw new Exception("数据帧格式错误。");

                    int dataLen = dataFrame.Header.Length + 4 /*帧头长度*/ - dataFrame._headerSize;

                    //帧总长度, 帧头 + 数据部分
                    int frameOverallLen = dataFrame._headerSize + dataLen;
                    if (frameOverallLen > pBuffer.Length)
                        return null;
                    //获取数据部分
                    dataFrame.Data = new byte[dataLen];
                    Buffer.BlockCopy(pBuffer, dataFrame._headerSize, dataFrame.Data, 0, dataLen);

                    //帧数据提取完毕，传出剩余的部分
                    int surplusLen = pBuffer.Length - frameOverallLen;
                    if (surplusLen > 0)
                    {
                        pSurplusBuffer = new byte[surplusLen];
                        Buffer.BlockCopy(pBuffer, frameOverallLen, pSurplusBuffer, 0, surplusLen);
                    }
                }
                else
                {
                    //帧头加载失败，将当前缓存累计到下一个包中，等待下次重新加载帧头
                    dataFrame = null;
                }
                return dataFrame;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                pSurplusBuffer = null;
                return null;
            }
        }

        public IEnumerable<string> PropertyKeys
        {
            get { return TargetProperty.Keys; }
        }

        public IEnumerable<string> PropertyValues
        {
            get { return TargetProperty.Values; }
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="pDataFrame"> </param>
        /// <param name="pPropertyName">属性名称</param>
        /// <returns>属性值</returns>
        internal static string GetPropertyValue(RmtpDataFrame pDataFrame, string pPropertyName)
        {
            if (!pDataFrame.TargetProperty.ContainsKey(pPropertyName))
                return string.Empty;
            return pDataFrame.TargetProperty[pPropertyName];
        }

        internal static void SetPropertyValue(RmtpDataFrame pDataFrame, string pTargetProperty)
        {
           // string msg = string.Format("Id：{0}，RmtpDataFrame属性值：{1}", pDataFrame.Header.ID, pTargetProperty);
            //@?RmtpDebug.Output(msg, Colors.Green);
            if (string.IsNullOrEmpty(pTargetProperty))
                return;

            string[] pvs = pTargetProperty.Split(';');
            foreach (string v in pvs)
            {
                //KevValue分隔符 : 或 =
                char splitChar = ':';
                //去掉= 号，字符串中有base64字符串
                //if (v.IndexOf('=') != -1)
                //    splitChar = '=';
                string[] pvs2 = v.Split(splitChar);
                if (pvs2.Length != 2)
                    continue;
                pDataFrame.TargetProperty[pvs2[0]] = pvs2[1];
            }

        }

        internal static void SetPropertyValue(RmtpDataFrame pDataFrame, string pPropertyName, string pPropertyValue)
        {
            pDataFrame.TargetProperty[pPropertyName] = pPropertyValue;
        }

        /// <summary>
        /// 数据帧转换函数
        /// </summary>
        /// <param name="pDataFrame">基础数据帧</param>
        /// <param name="pParameter">转换过程中需要的参数</param>
        protected virtual void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {

        }
        #region 转换
        /// <summary>
        /// 获取数据帧
        /// </summary>
        /// <typeparam name="T">数据帧类型</typeparam>
        /// <param name="pDataFrame">基础数据帧</param>
        /// <param name="pParameter">InternalConverter 重载方法需要的转换过程中的参数值</param>
        /// <returns></returns>
        public static T GetDataFrame<T>(RmtpDataFrame pDataFrame, object pParameter = null)
            where T : RmtpDataFrame, new()
        {
            try
            {
                T t;
                //XGZ：这里不加 try.. catch ，实时数据量大，频繁 try..catch 影响性能。有异常，说明创建的数据帧类有问题，必须修正。
                t = Activator.CreateInstance<T>();
                if (t != null)
                {
                    t.Header = pDataFrame.Header;
                    t.Data = pDataFrame.Data;
                    t.InternalConverter(pDataFrame, pParameter);
                }
                //@?RmtpDebug.Output("解析后", t, Color.FromArgb(0xFF, 0xAD, 0xFF, 0x2F));
                return t;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
