using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CO_IA.UI.Collection.DataAnalysis
{
    public enum SPropertyEnum : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        New = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Legal = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Illegal = 5,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Unknown = 4,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Sham = 7,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        NotM = 10,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        MCheck = 11,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        MNotCheck = 12,
    }
}
