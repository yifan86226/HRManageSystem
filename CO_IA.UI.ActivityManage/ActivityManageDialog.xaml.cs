﻿using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO_IA.UI.ActivityManage
{
    /// <summary>
    /// ActivityManageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityManageDialog : Window
    {
        public ActivityManageDialog(List<ActivityPlaceLocationImage> activityPlaceLocationImage, string _locationGuid)
        {
            InitializeComponent();
            imgManage.setSource(activityPlaceLocationImage, _locationGuid);
        }
    }
}
