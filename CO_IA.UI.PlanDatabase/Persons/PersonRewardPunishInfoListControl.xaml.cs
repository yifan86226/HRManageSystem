using CO_IA.Data.PlanDatabase;
using DataManager.Public;
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

namespace CO_IA.UI.PlanDatabase
{
    /// <summary>
    /// PersonListControl.xaml 的交互逻辑
    /// </summary>
    public partial class PersonRewardPunishInfoListControl : UserControl
    {


        private List<PersonRewardPunishInfo> infoList = new List<PersonRewardPunishInfo>();



        private string rptype = "0";

        public PersonRewardPunishInfoListControl(string type)
        {
            InitializeComponent();

            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度 

            this.dg_GrouperList.MaxHeight = y1 - 200;

            rptype = type;
            InitData(type);
        }


        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData(string type)
        {

            PersonRewardPunishInfoModel model = new PersonRewardPunishInfoModel();

            infoList = model.GetPersonRewardPunishInfos(type, "", "","","", "");

            this.dg_GrouperList.ItemsSource = null;
            this.dg_GrouperList.ItemsSource = infoList;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            PersonRewardPunishInfoDialog dialog = new PersonRewardPunishInfoDialog(rptype);
            dialog.Closed += Dialog_Closed;
            dialog.ShowDialog();

        }

        private void Dialog_Closed(object sender, EventArgs e)
        {
            InitData(rptype);
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            PersonRewardPunishInfoModel model = new PersonRewardPunishInfoModel();

            foreach (PersonRewardPunishInfo tempInfo in infoList)
            {

                if (tempInfo.ISCHECKED == true)
                {
                    model.DeletePersonRewardPunishInfo(tempInfo.ID);
                }

            }


            MessageBox.Show("删除完毕！");
            InitData(rptype);


        }

        private void buttonModify_Click(object sender, RoutedEventArgs e)
        {


            PersonRewardPunishInfoModel model = new PersonRewardPunishInfoModel();

            foreach (PersonRewardPunishInfo tempInfo in infoList)
            {

                if (tempInfo.ISCHECKED == true)
                {
                    PersonRewardPunishInfoDialog dialog = new PlanDatabase.PersonRewardPunishInfoDialog(tempInfo, false);
                    dialog.Closed += Dialog_Closed;
                    dialog.Show();
                }

            } 

        }



        /// <summary>
        /// 控制人员修改按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_SingleGrouper_Click(object sender, RoutedEventArgs e)
        {
            //int i = 0;

            //foreach (PP_PersonInfo pinfo in this.itemGrouperList)
            //{

            //    if (pinfo.ISCHECKED == true)
            //    {
            //        i++;
            //    }
            //}
        }



        public void dataGridTemplate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    PersonRewardPunishInfo personBasicInfo = dgr.DataContext as PersonRewardPunishInfo;
                    if (personBasicInfo != null)
                    {
                        PersonRewardPunishInfoDialog dialog = new PlanDatabase.PersonRewardPunishInfoDialog(personBasicInfo, true);

                        dialog.Show();
                    }
                }
            }
        }
    }
}
