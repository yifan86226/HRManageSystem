using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using CO_IA.UI.StationPlan;

namespace CO_IA.UI.Scene
{
    /// <summary>
    /// StationPlanView.xaml 的交互逻辑
    /// </summary>
    public partial class StationPlanView : UserControl
    {
        public StationPlanView()
        {
            InitializeComponent();
            StationPlanModule stationPlanModule = new StationPlanModule(LoginService.CurrentActivity, LoginService.CurrentActivityPlace);
            LayOutRoot.Children.Add(stationPlanModule);
           // LoginService.ActivityPlaceChanged += LoginService_ActivityPlaceChanged;
           
        }

        void LoginService_ActivityPlaceChanged(Data.ActivityPlace obj)
        {
            StationPlanModule stationPlanModule = new StationPlanModule(LoginService.CurrentActivity, obj);
            LayOutRoot.Children.Add(stationPlanModule);
        }
    }
}
