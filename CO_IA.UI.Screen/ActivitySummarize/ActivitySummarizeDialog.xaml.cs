using CO_IA.Client;
using CO_IA.UI.ActivitySummarize;
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

namespace CO_IA.UI.Screen.ActivitySummarize
{
    /// <summary>
    /// ActivitySummarizeDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ActivitySummarizeDialog : Window
    {
        List<CO_IA.UI.ActivitySummarize.ListItem> itemList = new List<CO_IA.UI.ActivitySummarize.ListItem>();

        bool isOrgStructureShowed = false;
        bool isSummarizeDocItemShowed = false;
        bool isMonitorEquipShowed = false;
        bool isWorkAchievementShowed = false;
        public ActivitySummarizeDialog()
        {
            InitializeComponent();
            //ActivitySummarizeModule summarize = new ActivitySummarizeModule();
            //g.Children.Add(summarize);InitializeComponent();
            CO_IA.UI.ActivitySummarize.ListItem item1 = new CO_IA.UI.ActivitySummarize.ListItem();

            item1.Name = "组织结构";
            item1.ImageSource = @"/CO_IA.UI.ActivitySummarize;component/Images/btn组织结构.png";

            itemList.Add(item1);

            CO_IA.UI.ActivitySummarize.ListItem item2 = new CO_IA.UI.ActivitySummarize.ListItem();

            item2.Name = "活动总结";
            item2.ImageSource = @"/CO_IA.UI.ActivitySummarize;component/Images/btn活动总结.png";

            itemList.Add(item2);
            this.listBoxPlace.ItemsSource = itemList;


            CO_IA.UI.ActivitySummarize.ListItem item3 = new CO_IA.UI.ActivitySummarize.ListItem();

            item3.Name = "保障过程";
            item3.ImageSource = @"/CO_IA.UI.ActivitySummarize;component/Images/btn保障过程.png";

            itemList.Add(item3);

            CO_IA.UI.ActivitySummarize.ListItem item4 = new CO_IA.UI.ActivitySummarize.ListItem();

            item4.Name = "数据统计";
            item4.ImageSource = @"/CO_IA.UI.ActivitySummarize;component/Images/btn工作成果.png";

            itemList.Add(item4);


            this.listBoxPlace.SelectedIndex = 0;
        }

        private void listBoxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CO_IA.UI.ActivitySummarize.ListItem item = listBoxPlace.SelectedItem as CO_IA.UI.ActivitySummarize.ListItem;

            if (item.Name == "组织结构")
            {
                if (isOrgStructureShowed == false)
                {
                    OrgStructure ap = new OrgStructure();
                    //grid_Show.Children.Clear();
                    grid_ShowOrgStructure.Children.Add(ap);
                    isOrgStructureShowed = true;
                }
                grid_ShowOrgStructure.Visibility = System.Windows.Visibility.Visible;
                grid_ShowSummarizeDocItem.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowMonitorEquip.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowWorkAchievement.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (item.Name == "活动总结")
            {
                if (isSummarizeDocItemShowed == false)
                {
                    SummarizeDocItem ap = new SummarizeDocItem(1);
                    //grid_Show.Children.Clear();
                    grid_ShowSummarizeDocItem.Children.Add(ap);
                    isSummarizeDocItemShowed = true;

                }
                grid_ShowOrgStructure.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowSummarizeDocItem.Visibility = System.Windows.Visibility.Visible;
                grid_ShowMonitorEquip.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowWorkAchievement.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (item.Name == "保障过程")
            {
                if (isMonitorEquipShowed == false)
                {
                    MonitorEquipForShow ap = new MonitorEquipForShow();
                    //grid_Show.Children.Clear();
                    grid_ShowMonitorEquip.Children.Add(ap);
                    isMonitorEquipShowed = true;

                }
               
                grid_ShowOrgStructure.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowSummarizeDocItem.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowMonitorEquip.Visibility = System.Windows.Visibility.Visible;
                grid_ShowWorkAchievement.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (item.Name == "数据统计")
            {
                if (isWorkAchievementShowed == false)
                {
                    WorkAchievement ap = new WorkAchievement(1);
                    //grid_Show.Children.Clear();
                    grid_ShowWorkAchievement.Children.Add(ap);

                    isWorkAchievementShowed = true;
                }
             
                grid_ShowOrgStructure.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowSummarizeDocItem.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowMonitorEquip.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowWorkAchievement.Visibility = System.Windows.Visibility.Visible;

            }
        }

       
    }
}
