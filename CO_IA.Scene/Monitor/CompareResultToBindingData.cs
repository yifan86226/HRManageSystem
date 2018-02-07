using CO_IA.Data;
using CO_IA.InterferenceAnalysis;
using CO_IA_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Scene.Monitor
{
    /// <summary>
    /// 计算结果转换为绑定到DataGrid的数据结构InterferedBindingData
    /// </summary>
    public class CompareResultToBindingData
    {
        private Func<string, ActivityEquipment> GetEquipment;
        private Func<string, ActivitySurroundStation> GetSurroundStation;
        public CompareResultToBindingData(Func<string, ActivityEquipment> p_equipmentFunc,Func<string, ActivitySurroundStation> p_surroundStationFunc)
        {
            this.GetEquipment = p_equipmentFunc;
            this.GetSurroundStation = p_surroundStationFunc;
        }
        /// <summary>
        /// 互调
        /// </summary>
        /// <param name="imInterfResult"></param>
        /// <returns></returns>
        internal List<InterferedBindingData> GetBindingData(IMCompareResult imInterfResult)
        {
            List<InterferedBindingData> bindingDatas = new List<InterferedBindingData>();
            IMItemBase[] results = imInterfResult.Values;
            if (results == null || results.Length == 0)
                return bindingDatas;
           
            foreach(IMItemBase imItemBase in results)
            {
                InterferedBindingData bindingData = new InterferedBindingData();
                if (imItemBase is SecondOrderIMItem)
                {
                    bindingData.InterferedType = "二阶互调干扰";
                }
                else if (imItemBase is ThirdOrderIMItem)
                {
                    bindingData.InterferedType = "三阶互调干扰";
                }
                else if (imItemBase is FifthIMItem)
                {
                    bindingData.InterferedType = "五阶阶互调干扰";
                }
                foreach (ComparableFreq freqinfo in imItemBase.IMFreqs)
                {
                    bindingData.InterferedFreq =  string.Join(",", imItemBase.IMFreqs);
                    if (LoadBaseBindingData(bindingData, freqinfo.FreqID))
                    {
                        bindingDatas.Add(bindingData);
                    }
                        
                }
            }

            return bindingDatas;
        }
        /// <summary>
        /// 邻频
        /// </summary>
        /// <param name="adjInterfResult"></param>
        /// <returns></returns>
        internal List<InterferedBindingData> GetBindingData(AdjFreqCompareResult[] adjInterfResult)
        {
            List<InterferedBindingData> bindingDatas = new List<InterferedBindingData>();
            foreach (AdjFreqCompareResult rst in adjInterfResult)
            {
                foreach (var freqinfo in rst.Keys)
                {
                    InterferedBindingData bindingData = new InterferedBindingData() { InterferedType = "邻频干扰" };
                    List<ComparableFreq> allAdjFreqlist = new List<ComparableFreq>();
                    allAdjFreqlist.AddRange(rst.UpperAdjFreqs);
                    allAdjFreqlist.AddRange(rst.LowerAdjFreqs);
                    ComparableFreq adjFreq = allAdjFreqlist.Where(p => p.FreqID == freqinfo.FreqID).SingleOrDefault();
                    bindingData.InterferedBand = adjFreq.Band.ToString();
                    bindingData.InterferedFreq = adjFreq.Freq.ToString();
                    if (LoadBaseBindingData(bindingData, freqinfo.FreqID))
                        bindingDatas.Add(bindingData);
                }
            }
            return bindingDatas;
        }
        /// <summary>
        /// 同频
        /// </summary>
        /// <param name="sameResult"></param>
        /// <returns></returns>
        internal List<InterferedBindingData> GetBindingData(SameFreqCompareResult sameResult)
        {
            List<InterferedBindingData> bindingDatas = new List<InterferedBindingData>();
            foreach (ComparableFreq freqinfo in sameResult.Keys)
            {
                //freqinfo.FreqID
                InterferedBindingData bindingData = new InterferedBindingData() { InterferedType = "同频干扰" };
                List<ComparableFreq> list = sameResult.Values.Where(p => p.FreqID == freqinfo.FreqID).ToList();
                ComparableFreq comparableFreq = sameResult.Values.Where(p => p.FreqID == freqinfo.FreqID).SingleOrDefault();
                bindingData.InterferedBand = comparableFreq.Band.ToString();
                bindingData.InterferedFreq = comparableFreq.Freq.ToString();
                if (LoadBaseBindingData(bindingData, freqinfo.FreqID))
                    bindingDatas.Add(bindingData);
            }
            return bindingDatas;
        }

        bool LoadBaseBindingData(InterferedBindingData bindingData,string freqGuid)
        {
            ActivityEquipment equip = GetEquipment(freqGuid);
            ActivitySurroundStation stat = GetSurroundStation(freqGuid);
            if (equip != null)
            {
                bindingData.AssignFreq = equip.AssignSendFreq.ToString();
                bindingData.BdWith = equip.Band_kHz.ToString();
                bindingData.EmitPower = equip.Power_W.ToString();
                bindingData.EquipName = equip.Name;
                bindingData.StationType = "现场设台";
                bindingData.SetDept = equip.OrgInfo.Name;
            }
            else if (stat != null)
            {
                StationEmitInfo emitInfo = stat.EmitInfo.Where(p => p.StationGuid == freqGuid).SingleOrDefault();
                if (emitInfo!=null)
                {
                    bindingData.AssignFreq = emitInfo.FreqEC == null ? "" : emitInfo.FreqEC.ToString();
                    bindingData.BdWith = emitInfo.FreqBand == null ? "" : emitInfo.FreqBand.ToString();
                    bindingData.EmitPower = emitInfo.EquPow.ToString();
                }
               
                
                bindingData.EquipName = "";
                bindingData.StationType = "周围台站";
                bindingData.SetDept = stat.ORG_NAME;
            }
            if (equip != null || stat != null)
                return true;
            return false;
        }
    }
    
}
