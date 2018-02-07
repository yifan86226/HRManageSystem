using CO_IA.Data;
using I_GS_MapBase.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.UI.Screen
{
    #region 活动相关结构
    /// <summary>
    /// 大屏端活动地点存储类
    /// </summary>
    public class ActivityPlaceInfoEx : INotifyPropertyChanged
    {
        /// <summary>
        /// 同地点的GUID
        /// </summary>
        public string Guid { get; set; }
        
        /// <summary>
        /// 地点浮动显示的信息
        /// </summary>
        private ActivityPlaceTips _tips { get; set; }
        public ActivityPlaceTips Tips { get; set; }

        private FreqPlanningProgress _planProgress;
        public FreqPlanningProgress PlanProgress
        {
            get { return _planProgress; }
            set
            {
                _planProgress = value;
                if (_planProgress == null)
                {
                    Tips = new ActivityPlaceTips()
                    {
                        FinishValue = 0,
                        //DsList = new List<KeyValuePair<string, double>>() {
                        //            new KeyValuePair<string,double>("已知信号",0),
                        //            new KeyValuePair<string,double>("未知信号",0),
                        //            new KeyValuePair<string,double>("处理任务",0),
                        //            new KeyValuePair<string,double>("干扰数量",0),
                        //        }
                    };
                }
                else
                {
                    double have = 0;
                    for (int i = 0; i < PlanProgress.StepStates.Length; i++)
                    {
                        if (PlanProgress.StepStates[i].IsCompleted)
                        {
                            have++;
                        }
                    }
                    if (Tips != null)
                    {
                        //if (Utils.CurrentActivityStage == ActivityStage.Prepare)
                        //{
                        //    Tips.FinishValue = Math.Round(have * 1.0 * 100 / PlanProgress.StepStates.Length * 1.0);
                        //}
                        //else
                        //{
                           
                        //}

                    }
                    else
                    {
                        Tips = new ActivityPlaceTips()
                        {
                            FinishValue = Math.Round(have * 1.0 * 100 / PlanProgress.StepStates.Length * 1.0),
                            //DsList = new List<KeyValuePair<string, double>>() {
                            //        new KeyValuePair<string,double>("已知信号",0),
                            //        new KeyValuePair<string,double>("未知信号",0),
                            //        new KeyValuePair<string,double>("处理任务",0),
                            //        new KeyValuePair<string,double>("干扰数量",0),
                            //    }
                        };
                    }
                }
            }
        }
        /// <summary>
        /// 地点的基本信息
        /// </summary>
        public ActivityPlaceInfo _activityPlaceInfo { get; set; }
        /// <summary>
        /// 地点的绘制返回信息
        /// </summary>
        public ReturnDrawGraphicInfo[] GInfo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
            }
        }
    }
    /// <summary>
    /// 地点浮动显示的信息结构
    /// </summary>
    public class ActivityPlaceTips : INotifyPropertyChanged
    {
        private string _placeID;
        /// <summary>
        /// 地点名称 绑定显示
        /// </summary>
        public string PlaceID
        {
            get { return _placeID; }
            set
            {
                _placeID = value;
                OnPropertyChanged("PlaceID");
            }
        }
        private string _placeName;
        /// <summary>
        /// 地点名称 绑定显示
        /// </summary>
        public string PlaceName
        {
            get { return _placeName; }
            set
            {
                _placeName = value;
                OnPropertyChanged("PlaceName");
            }
        }
        private double _finishValue;
        /// <summary>
        /// 完成百分比
        /// </summary>
        public double FinishValue
        {
            get { return _finishValue; }
            set
            {
                _finishValue = value;
                OnPropertyChanged("FinishValue");
            }
        }
        private List<KeyValuePair<string, double>> _list;
        public List<KeyValuePair<string, double>> DsList
        {
            get { return _list; }
            set
            {
                _list = value;
                if (_list == null || _list.Count == 0)
                {
                    _list = null;
                    _list = new List<KeyValuePair<string, double>>() {
                        new KeyValuePair<string,double>("已知信号",0),
                        new KeyValuePair<string,double>("未知信号",0),
                        new KeyValuePair<string,double>("处理任务",0),
                        new KeyValuePair<string,double>("干扰数量",0),
                    };
                }
                OnPropertyChanged("DsList");
            }
        }        

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
            }
        }


    }
    #endregion
}
