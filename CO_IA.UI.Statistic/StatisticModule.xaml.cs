using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CO_IA.Data;
using System.Collections;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// StatisticModule.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticModule : UserControl
    {

        //private ORGAndEQUStatisticControl _organdequStatistic;
        //private FreqPartPlanStatisticControl _freqPartPlanStatistic;
        //private RoundStatStatisticControl _roundStatStatistic;
        //private FreqAssignStatisticControl _freqAssignStatistic;
        //private EquInspectionStatisticControl _equInspectionStatisticControl;
        private PersonPlanStatisticControl _personPlanStatisticControl;
        private PersonOutStatisticControl _personOutStatisticControl;
        private PersonQuantitativeStatisticControl _personquantitativeStatisticControl;


        




        private IList _statisticSource;
        private IList _statisticSource1;
        private String _activityGuid;


        public StatisticModule()
        {
            InitializeComponent();


            InitStatisticUI();
            itemPersonRF.Visibility = Visibility.Collapsed;
        }


        public StatisticModule(int  i)
        {
            InitializeComponent();


            if (i == 1)
            {

                itemPersonRF.Visibility = Visibility.Collapsed;
            }
            else if(i==2)
            {
                itemPersonPlan.Visibility = Visibility.Collapsed;
                itemPersonOut.Visibility = Visibility.Collapsed;



                _activityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
                Dictionary<string, string> dicActivitySite = StatisticHelper.InitActivityPlace();
                xComboboxSite.ItemsSource = dicActivitySite;
                xComboboxSite.DisplayMemberPath = "Value";
                xComboboxSite.SelectedValuePath = "Key";
                this.xComboboxSite.SelectedIndex = 2;
                this.listBoxMenu.SelectedIndex = 2;


            }
            //InitStatisticUI();
        }

        private void InitEvent()
        {

        }

        /// <summary>
        /// 初始化时，默认统计单位设备管理
        /// </summary>
        private void InitStatisticUI()
        {
            _activityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            Dictionary<string, string> dicActivitySite = StatisticHelper.InitActivityPlace();
            xComboboxSite.ItemsSource = dicActivitySite;
            xComboboxSite.DisplayMemberPath = "Value";
            xComboboxSite.SelectedValuePath = "Key";
            this.xComboboxSite.SelectedIndex = 0;
            this.listBoxMenu.SelectedIndex = 0;
        }

        private IList CreateStatisticDatas(StatisticTypes sType)
        {
            switch (sType)
            {
                //case StatisticTypes.ORGAndEQUStatisticType:
                //    return StatisticHelper.StatisticEquipment(_activityGuid);
                //case StatisticTypes.FreqPartPlanStatisticType:
                //    return StatisticHelper.StatisticFreqPartPlan(_activityGuid);
                //case StatisticTypes.SurroundStatStatisticType:
                //    return StatisticHelper.StatisticSurroundStation(_activityGuid);
                //case StatisticTypes.FreqAssignStatisticType:
                //    return StatisticHelper.StatisticFreqAssign(_activityGuid);
                //case StatisticTypes.EquInspectionSticType: //设备检测
                //    return StatisticHelper.StatisticEquInspection(_activityGuid);
                case StatisticTypes.PersonPlanStatisticType:
                    return StatisticHelper.StatisticPersonPlan(_activityGuid);


                case StatisticTypes.PersonRPStatisticType:
                    return StatisticHelper.StatisticPersonRP();




                case StatisticTypes.PersonOutStatisticType:
                    return StatisticHelper.StatisticPersonOut();



                default:
                    return null;
            }
        }

        private void SiteChangeSource<T>(string pName, IList<T> pSource) where T : StatisticData
        {
            if (pSource != null)
            {
                if (pName == "all")
                {
                    _statisticSource = pSource.ToList();
                }
                else
                {
                    _statisticSource = pSource.Where(r => r.AddressGuid == pName).ToList();
                }
            }
        }

        /// <summary>
        /// 活动地点改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xComboboxSite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = listBoxMenu.SelectedItem as ListBoxItem;
            if (item != null)
            {
                HiddenAllContainer();
                KeyValuePair<string, string> selectitem = (KeyValuePair<string, string>)xComboboxSite.SelectedItem;
                string placeguid = selectitem.Key;
                if (placeguid == "all")
                {
                    itemPersonPlan.Visibility = Visibility.Visible;
                }
                else
                {
                    itemPersonPlan.Visibility = Visibility.Collapsed;
                }

                switch (item.Name)
                {
                    //case "itemOrg": //单位设备
                    //    if (_organdequStatistic != null)
                    //    {
                    //        SiteChangeSource(placeguid, _statisticSource1 as IList<EquStatisticData>);
                    //        _organdequStatistic.SelectedPlace = placeguid;
                    //        _organdequStatistic.StatisticSource = _statisticSource;
                    //    }
                    //    break;
                    //case "itemFreq": //频率保障方案
                    //    if (_freqPartPlanStatistic != null)
                    //    {
                    //        SiteChangeSource(placeguid, _statisticSource1 as IList<FreqPartPlanStatisticData>);
                    //        _freqPartPlanStatistic.SelectedPlace = placeguid;
                    //        _freqPartPlanStatistic.StatisticSource = _statisticSource;
                    //    }
                    //    break;
                    //case "itemSurround": //周围台站
                    //    if (_roundStatStatistic != null)
                    //    {
                    //        SiteChangeSource(placeguid, _statisticSource1 as IList<SurroundStatStatisticData>);
                    //        _roundStatStatistic.SelectedPlace = placeguid;
                    //        _roundStatStatistic.StatisticSource = _statisticSource;
                    //    }
                    //    break;
               

                    //case "itemEquInspection": //设备检测
                    //    if (_equInspectionStatisticControl != null)
                    //    {
                    //        SiteChangeSource(placeguid, _statisticSource1 as IList<EquInspectionStatisticData>);
                    //        _equInspectionStatisticControl.SelectedPlace = placeguid;
                    //        _equInspectionStatisticControl.StatisticSource = _statisticSource;
                    //    }
                    //    break;

                    case "itemPersonPlan":
                        if (_personPlanStatisticControl != null)
                        {
                            _personPlanStatisticControl.PersonPlanStatisticSource = _statisticSource1 as IList<PersonPlanStatisticData>;
                            _personPlanStatisticControl.StatisticSource = _statisticSource;
                        }
                        break;




                    case "itemPersonOut":
                        if (_personPlanStatisticControl != null)
                        {
                            _personPlanStatisticControl.PersonPlanStatisticSource = _statisticSource1 as IList<PersonPlanStatisticData>;
                            _personPlanStatisticControl.StatisticSource = _statisticSource;
                        }
                        break;




                    case "itemPersonRF":
                        if (_personquantitativeStatisticControl != null)
                        {
                            _personquantitativeStatisticControl.PersonPlanStatisticSource = _statisticSource1 as List<PersonPlanStatisticData>;
                            _personquantitativeStatisticControl.StatisticSource = _statisticSource;
                        }
                        break;





                }
            }
        }

        /// <summary>
        /// 菜单选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = listBoxMenu.SelectedItem as ListBoxItem;
            if (item != null)
            {
                HiddenAllContainer();
                //KeyValuePair<string, string> selectitem = (KeyValuePair<string, string>)xComboboxSite.SelectedItem;
                //string placeguid = selectitem.Key;
                switch (item.Name)
                {
                    //case "itemOrg"://单位设备管理
                    //    if (_organdequStatistic == null) _organdequStatistic = new ORGAndEQUStatisticControl();
                    //    ShowBorderContent(_organdequStatistic);
                    //    _statisticSource1 = CreateStatisticDatas(StatisticTypes.ORGAndEQUStatisticType);
                    //    SiteChangeSource(placeguid, _statisticSource1 as IList<EquStatisticData>);
                    //    _organdequStatistic.SelectedPlace = placeguid;
                    //    _organdequStatistic.StatisticSource = _statisticSource;
                    //    break;
                    //case "itemFreq"://频率保障方案
                    //    if (_freqPartPlanStatistic == null) _freqPartPlanStatistic = new FreqPartPlanStatisticControl();
                    //    ShowBorderContent(_freqPartPlanStatistic);
                    //    _statisticSource1 = CreateStatisticDatas(StatisticTypes.FreqPartPlanStatisticType);
                    //    SiteChangeSource(placeguid, _statisticSource1 as IList<FreqPartPlanStatisticData>);
                    //    _freqPartPlanStatistic.SelectedPlace = placeguid;
                    //    _freqPartPlanStatistic.StatisticSource = _statisticSource;
                    //    break;
                    //case "itemSurround"://周围台站
                    //    if (_roundStatStatistic == null) _roundStatStatistic = new RoundStatStatisticControl();
                    //    ShowBorderContent(_roundStatStatistic);
                    //    _statisticSource1 = CreateStatisticDatas(StatisticTypes.SurroundStatStatisticType);
                    //    SiteChangeSource(placeguid, _statisticSource1 as IList<SurroundStatStatisticData>);
                    //    _roundStatStatistic.SelectedPlace = placeguid;
                    //    _roundStatStatistic.StatisticSource = _statisticSource;
                    //    break;
                    //case "itemAssignFreq"://频率指配
                    //    if (_freqAssignStatistic == null) _freqAssignStatistic = new FreqAssignStatisticControl();
                    //    ShowBorderContent(_freqAssignStatistic);
                    //    _statisticSource1 = CreateStatisticDatas(StatisticTypes.FreqAssignStatisticType);
                    //    SiteChangeSource(placeguid, _statisticSource1 as IList<FreqAssignStatisticData>);
                    //    _freqAssignStatistic.SelectedPlace = placeguid;
                    //    _freqAssignStatistic.StatisticSource = _statisticSource;
                    //    break;
                    //case "itemEquInspection": //设备检测
                    //    if (_equInspectionStatisticControl == null) _equInspectionStatisticControl = new EquInspectionStatisticControl();
                    //    ShowBorderContent(_equInspectionStatisticControl);
                    //    _statisticSource1 = CreateStatisticDatas(StatisticTypes.EquInspectionSticType);
                    //    SiteChangeSource(placeguid, _statisticSource1 as IList<EquInspectionStatisticData>);
                    //    _equInspectionStatisticControl.SelectedPlace = placeguid;
                    //    _equInspectionStatisticControl.StatisticSource = _statisticSource;
                    //    break;
                    case "itemPersonPlan":  //人员外出统计2017
                        if (_personPlanStatisticControl == null)
                            _personPlanStatisticControl = new PersonPlanStatisticControl();
                        ShowBorderContent(_personPlanStatisticControl);
                        _statisticSource1 = CreateStatisticDatas(StatisticTypes.PersonPlanStatisticType);
                        _personPlanStatisticControl.PersonPlanStatisticSource = _statisticSource1 as IList<PersonPlanStatisticData>;
                        _personPlanStatisticControl.StatisticSource = _statisticSource1;
                        break;



                    case "itemPersonOut":  //人员外出统计
                        if (_personOutStatisticControl == null)
                            _personOutStatisticControl = new PersonOutStatisticControl();
                        ShowBorderContent(_personOutStatisticControl);
                        _statisticSource1 = CreateStatisticDatas(StatisticTypes.PersonOutStatisticType);
                        _personOutStatisticControl.PersonPlanStatisticSource = _statisticSource1 as IList<PersonPlanStatisticData>;
                        _personOutStatisticControl.StatisticSource = _statisticSource1;
                        break;




                         


                    case "itemPersonRF": //频率指配
                 

                        if (_personquantitativeStatisticControl == null)
                            _personquantitativeStatisticControl = new PersonQuantitativeStatisticControl();
                        ShowBorderContent(_personquantitativeStatisticControl);
                        _statisticSource1 = CreateStatisticDatas(StatisticTypes.PersonRPStatisticType);
                        _personquantitativeStatisticControl.PersonPlanStatisticSource = _statisticSource1 as List<PersonPlanStatisticData>;
                        _personquantitativeStatisticControl.StatisticSource = _statisticSource1;



                        break;
                }
            }
        }

        private void ShowBorderContent(UserControl control)
        {
            this.borderContent.Visibility = System.Windows.Visibility.Visible;
            this.borderContent.Child = null;
            this.borderContent.Child = control;
        }

        private void HiddenAllContainer()
        {
            //if (_organdequStatistic != null)
            //{
            //    _organdequStatistic.ShowContainer = false;
            //}
            //if (_freqPartPlanStatistic != null)
            //{
            //    _freqPartPlanStatistic.ShowContainer = false;
            //}
            //if (_roundStatStatistic != null)
            //{
            //    _roundStatStatistic.ShowContainer = false;
            //}
            //if (_freqAssignStatistic != null)
            //{
            //    _freqAssignStatistic.ShowContainer = false;
            //}
            //if (_equInspectionStatisticControl != null)
            //{
            //    _equInspectionStatisticControl.ShowContainer = false;
            //}
            if (_personPlanStatisticControl != null)
            {
                _personPlanStatisticControl.ShowContainer = false;
            }
        }
    }
}
