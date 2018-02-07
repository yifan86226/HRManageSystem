#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：保密频率数据结构
 * 日  期：2016-09-1
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class SecretFreq
    {
        public SecretFreq()
        {
            if (string.IsNullOrEmpty(Guid))
            {
                Guid = System.Guid.NewGuid().ToString();
            }
        }
        public string Guid { get; set; }

        public double? FreqBegin { get; set; }

        public double? FreqEnd { get; set; }

        public string Comments { get; set; }

        public string SecurityCode { get; set; }

        public string SecurityName { get; set; }

        //public IdentifiableData<string> Security;
    }
}
