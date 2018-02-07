using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{
    /// <summary>
    /// 互调阶数
    /// </summary>
    [Flags]
    public enum AnalysisType : short
    {
        //不进行计算
        None = 0,

        //同频计算
        SameFreq = 4,

        //邻频计算
        ADJFreq = 8,

        //互调计算
        IM = 32

        //计算所有：AnalysisType.SameFreq| AnalysisType.ADJFreq| AnalysisType.IM;
    }
}
