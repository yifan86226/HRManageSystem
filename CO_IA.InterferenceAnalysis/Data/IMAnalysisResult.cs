#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：互调干扰结果
 * 日  期：2016-09-07
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{
    public class IMAnalysisResult
    {
        /// <summary>
        /// 接收机互调
        /// </summary>
        private IMCompareResult receiverImResult;

        /// <summary>
        /// 发射机互调
        /// </summary>
        private IMCompareResult transmitterImResult;

        /// <summary>
        /// 构造函数
        /// </summary>
        public IMAnalysisResult()
        {
        }

        /// <summary>
        /// 设置接收机互调
        /// </summary>
        /// <param name="receiverImResult">接收机互调结果</param>
        public void SetReceiverImResult(IMCompareResult receiverImResult)
        {
            this.receiverImResult = receiverImResult;
        }

        /// <summary>
        /// 设置发射机互调
        /// </summary>
        /// <param name="transmitterImResult">发射机互调结果</param>
        public void SetTransmitterImResult(IMCompareResult transmitterImResult)
        {
            this.transmitterImResult = transmitterImResult;
        }

        /// <summary>
        /// 获取接收机互调结果
        /// </summary>
        /// <returns>接收机互调结果</returns>
        public IMCompareResult GetReceiverImResult()
        {
            return this.receiverImResult;
        }

        /// <summary>
        /// 获取发射机互调结果
        /// </summary>
        /// <returns>发射机互调结果</returns>
        public IMCompareResult GetTransmitterImResult()
        {
            return this.transmitterImResult;
        }
    }
}
