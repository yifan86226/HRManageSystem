using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA_Data
{

    public class SurroundStationInfo : StationInfo
    {
        /// <summary>
        /// 发射信息
        /// </summary>
        public List<StationEmitInfo> EmitInfo { get; set; }

    }
}
