using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 键值对
    /// </summary>
#if ! SILVERLIGHT
    [DataContract]
#endif
    public class StringPair
    {
        public StringPair()
        { }

        public StringPair(string p_key, string p_value)
        {
            this.Key = p_key;
            this.Value = p_value;
        }

#if ! SILVERLIGHT
        [DataMember]
#endif
        public string Key { get; set; }

#if ! SILVERLIGHT
        [DataMember]
#endif
        public string Value { get; set; }
    }
}
