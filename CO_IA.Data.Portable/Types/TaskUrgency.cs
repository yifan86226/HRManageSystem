using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT_BC.Types;
namespace CO_IA.Types
{
    public enum TaskUrgency
    {
        [AT_BC.Types.EnumDisplayName("一般")]
        Normal,

        [AT_BC.Types.EnumDisplayName("紧急")]
        Urgent,

        [AT_BC.Types.EnumDisplayName("特别紧急")]
        ParticularlyUrgent
    }
}
