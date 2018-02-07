#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：绘制区域标题
 * 日 期 ：2017-06-30
 ***************************************************************#@#***************************************************************/
#endregion
using AT_BC.Data;
using CO_IA.Client;
using CO_IA.UI.MAP;
using CO_IA.UI.Screen.Areas;
using I_GS_MapBase.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CO_IA.UI.Screen
{
    public class LoadAreaTips
    {
        List<AreaTipsData> TipsData = new List<AreaTipsData>();

        private void SetDoIt(bool b)
        {
            if (b)
            {
                Obj.screenMap.RemoveElementByFlag(MapGroupTypes.SiteTipsPoint_.ToString());
                new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(1000);
                    
                    Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() => {
                        LoadData();
                    }));

                })) { IsBackground = true }.Start();
               
            }
            else
            {
                Obj.screenMap.RemoveElementByFlag(MapGroupTypes.SiteTipsPoint_.ToString());
            }
        }
        public void DrawIt()
        {
            Obj.screenMap.RemoveElementByFlag(MapGroupTypes.SiteTipsPoint_.ToString());
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(2000);

                Obj.screenMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadData();
                }));
                ////停止显示百分比
                //Thread.Sleep(1000);
                //StartTipsData();

            })) { IsBackground = true }.Start();
        }
        public void Show(bool b)
        {
            Obj.screenMap.SetElementVisibilityByFlag(MapGroupTypes.SiteTipsPoint_.ToString(), b);
        }
        private void LoadData()
        {
            TipsData.Clear();
            if (Obj.AreaGraphicInfo != null && Obj.AreaGraphicInfo.Count > 0)
            {
                foreach (var info in Obj.AreaGraphicInfo)
                {
                    AreaTipsData tipsData = new AreaTipsData();                    
                    var areainfo = Obj.ActivityPlaces.Where(item=>item.Guid==info.Key).ToList();
                    if (areainfo != null && areainfo.Count == 1)
                    {
                        tipsData.AreaID = areainfo[0].Guid;
                        tipsData.AreaName = areainfo[0].Name;
                        tipsData.FinishValue = 0;
                        ReturnDrawGraphicInfo[] graphicinfo = info.Value as ReturnDrawGraphicInfo[];
                        if (graphicinfo != null && graphicinfo.Length > 0)
                        {                            
                            foreach (ReturnDrawGraphicInfo ginfo in graphicinfo)
                            {
                                AreaTips tips = new AreaTips(tipsData);
                                Obj.screenMap.AddElement(tips, Obj.screenMap.GetMapPointEx(ginfo.CenterPoint.X, ginfo.CenterPoint.Y));
                            }
                        }
                        TipsData.Add(tipsData);
                    }                    
                }
            }
        
        }

        #region 区域完成百分比，功能停用
        private void StartTipsData()
        {
            //if (Obj.Activity.ActivityStage != Types.ActivityStage.Prepare)
            //    return;
            Obj.TipsDataThread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    LoadTipsData();
                    Thread.Sleep(30*1000);
                    //if (Obj.Activity.ActivityStage != Types.ActivityStage.Prepare)
                    //    break;
                }
                
            })) { IsBackground = true };
            Obj.TipsDataThread.Start();
        }
        private void LoadTipsData()
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(p =>
            {
                var stepStates = p.GetFreqPlanningProgresses(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                if (stepStates != null && stepStates.Length > 0)
                {
                    foreach (var tipsdata in TipsData)
                    {
                        var state = stepStates.Where(item => item.PlaceGuid == tipsdata.AreaID).ToArray();
                        if (state == null || state.Length == 0)
                        {
                            tipsdata.FinishValue = 0;
                        }
                        else
                        {
                            double have = 0;
                            for (int i = 0; i < state[0].StepStates.Length; i++)
                            {
                                if (state[0].StepStates[i].IsCompleted)
                                {
                                    have++;
                                }
                            }
                            Random r = new Random();
                            tipsdata.FinishValue = Math.Round(have * 1.0 * 100 / state[0].StepStates.Length * 1.0);
                        }
                    }
                }
            });
        }
        #endregion
    }
    public class AreaTipsData : NotifyPropertyChangedObject
    {
        private string areaID;
        /// <summary>
        /// 地点名称 绑定显示
        /// </summary>
        public string AreaID
        {
            get { return areaID; }
            set
            {
                areaID = value;
                NotifyPropertyChanged("AreaID");
            }
        }
        private string _areaName;
        /// <summary>
        /// 地点名称 绑定显示
        /// </summary>
        public string AreaName
        {
            get { return _areaName; }
            set
            {
                _areaName = value;
                NotifyPropertyChanged("AreaName");
            }
        }
        private double finishValue;
        /// <summary>
        /// 完成百分比
        /// </summary>
        public double FinishValue
        {
            get { return finishValue; }
            set
            {
                finishValue = value;
                NotifyPropertyChanged("FinishValue");
            }
        }
    }
}
