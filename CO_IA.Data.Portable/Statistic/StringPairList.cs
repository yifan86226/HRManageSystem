using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 键值列表
    /// </summary>
#if ! SILVERLIGHT
    [DataContract]
#endif
    public class StringPairList
    {
#if ! SILVERLIGHT
        [DataMember]
#endif
        public List<StringPair> SPList { get; set; }

        public StringPairList()
        {
            SPList = new List<StringPair>();
        }

        public string this[string p_key]
        {
            get
            {
                var item = SPList.FirstOrDefault(itm => itm.Key == p_key);
                if (item != null)
                {
                    return item.Value;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                var item = SPList.FirstOrDefault(itm => itm.Key == p_key);
                if (item != null)
                {
                    item.Value = value;
                }
                else
                {
                    StringPair strPair = new StringPair();
                    strPair.Key = p_key;
                    strPair.Value = value;
                    SPList.Add(strPair);
                }
            }
        }

    }
}
