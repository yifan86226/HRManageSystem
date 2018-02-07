using CO_IA.Data;
using CO_IA.Data.Template;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.PlanDatabase
{
    /// <summary>
    /// TemplateModule.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateModule : UserControl
    {
        public TemplateModule()
        {
            InitializeComponent();
            //this.listBoxActivityType.ItemsSource = CO_IA.Client.Utility.SupportActivityTypes;
        }

        private void listBoxActivityType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            grid_Container.Children.Clear();


            ListBoxItem item = listBoxActivityType.SelectedItem as ListBoxItem;

            if (item.Name == "itemPersonBasicInfo")
            {
                PersonListControl list = new PlanDatabase.PersonListControl();

                grid_Container.Children.Add(list);

            }
            else if (item.Name == "itemPersonOutInfo")
            {
                PersonOutInfoListControl list = new PlanDatabase.PersonOutInfoListControl();

                grid_Container.Children.Add(list);

            }

            else if (item.Name == "itemPersonRewardInfo")
            {
                PersonRewardPunishInfoListControl list = new PlanDatabase.PersonRewardPunishInfoListControl("0");

                grid_Container.Children.Add(list);

            }

            else if (item.Name == "itemPersonPunishInfo")
            {
                PersonRewardPunishInfoListControl list = new PlanDatabase.PersonRewardPunishInfoListControl("1");

                grid_Container.Children.Add(list);

            }
        }


 

 

        private void buttonModify_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
