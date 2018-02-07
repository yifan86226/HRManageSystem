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
    public partial class PersonOutInfoListControl : UserControl
    {


        private List<PersonOutInfo> infoList = new List<PersonOutInfo>();




        public PersonOutInfoListControl()
        {
            InitializeComponent();

            double y1 = SystemParameters.PrimaryScreenHeight;//得到屏幕整体高度 

            this.dg_GrouperList.MaxHeight = y1 - 200;

            InitData();
        }


        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {


            PersonOutModel model = new PersonOutModel();

            infoList = model.GetPersonOutInfos("", "", "", "");

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
            PersonOutInfoDialog dialog = new PersonOutInfoDialog();
            dialog.Closed += Dialog_Closed;
            dialog.ShowDialog();


        }

        private void Dialog_Closed(object sender, EventArgs e)
        {

            InitData();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            PersonOutModel model = new PersonOutModel();
            foreach (PersonOutInfo tempInfo in infoList)
            {

                if (tempInfo.ISCHECKED == true)
                {
                    model.DeletePersonOutInfo(tempInfo.ID);
                }

            }


            MessageBox.Show("删除完毕！");

            InitData();
        }

        private void buttonModify_Click(object sender, RoutedEventArgs e)
        {



            foreach (PersonOutInfo tempInfo in infoList)
            {

                if (tempInfo.ISCHECKED == true)
                {
                    PersonOutInfoDialog dialog = new PlanDatabase.PersonOutInfoDialog(tempInfo, false);
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
                    PersonOutInfo personBasicInfo = dgr.DataContext as PersonOutInfo;
                    if (personBasicInfo != null)
                    {
                        PersonOutInfoDialog dialog = new PlanDatabase.PersonOutInfoDialog(personBasicInfo, true);

                        dialog.Show();
                    }
                }
            }
        }

    }
}
