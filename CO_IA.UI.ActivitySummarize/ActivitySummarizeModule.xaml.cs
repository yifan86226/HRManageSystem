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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.ActivitySummarize
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ActivitySummarizeModule : UserControl
    {
        List<ListItem> itemList = new List<ListItem>();

        bool isOrgStructureShowed = false;
        bool isSummarizeDocItemShowed = false;
        bool isMonitorEquipShowed = false;
        bool isWorkAchievementShowed = false;

        public ActivitySummarizeModule()
        {
            InitializeComponent();
            ListItem item1 = new ListItem();

            item1.Name = "组织结构";
            item1.ImageSource = @"/CO_IA.UI.ActivitySummarize;component/Images/btn组织结构.png";

            itemList.Add(item1);

            ListItem item2 = new ListItem();

            item2.Name = "活动总结";
            item2.ImageSource = @"/CO_IA.UI.ActivitySummarize;component/Images/btn活动总结.png";

            itemList.Add(item2);
            this.listBoxPlace.ItemsSource = itemList;


            ListItem item3 = new ListItem();

            item3.Name = "保障过程";
            item3.ImageSource = @"/CO_IA.UI.ActivitySummarize;component/Images/btn保障过程.png";

            itemList.Add(item3);


            ListItem item4 = new ListItem();

            item4.Name = "量化考核";
            item4.ImageSource = @"/CO_IA.UI.ActivitySummarize;component/Images/btn工作成果.png";

            itemList.Add(item4);



            //ListItem item4 = new ListItem();

            //item4.Name = "工作成果";
            //item4.ImageSource = @"/CO_IA.UI.ActivitySummarize;component/Images/btn工作成果.png";

            //itemList.Add(item4);


            //ListItem item5 = new ListItem();

            //item5.Name = "荣誉奖章";
            //item5.ImageSource = @"/CO_IA.UI.ActivitySummarize;component/Images/btn荣誉奖章.png";

            //itemList.Add(item5);

            this.listBoxPlace.SelectedIndex = 0;
                                     //Source="/CO_IA.UI.ActivitySummarize;component/Images/place.png"
            //this.listBoxPlace.ItemsSource = new string[] { "组织结构", "活动总结", "保障过程", "工作成果", "荣誉奖章" };
        }

        private void listBoxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListItem item = listBoxPlace.SelectedItem as ListItem;

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
                    SummarizeDocItem ap = new SummarizeDocItem();
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
                    MonitorEquip ap = new MonitorEquip(true);
                    //MonitorEquipForShow ap = new MonitorEquipForShow(true);
                    //grid_Show.Children.Clear();
                    grid_ShowMonitorEquip.Children.Add(ap);
                    isMonitorEquipShowed = true;

                }
               
                grid_ShowOrgStructure.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowSummarizeDocItem.Visibility = System.Windows.Visibility.Hidden;
                grid_ShowMonitorEquip.Visibility = System.Windows.Visibility.Visible;
                grid_ShowWorkAchievement.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (item.Name == "工作成果")
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

            else if (item.Name == "量化考核")
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
    public class ListItem {

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string imageSource;

        public string ImageSource
        {
            get { return imageSource; }
            set { imageSource = value; }
        }
    }
}
