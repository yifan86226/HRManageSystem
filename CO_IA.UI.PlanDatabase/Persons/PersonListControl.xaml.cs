using CO_IA.Data.PlanDatabase;
using DataManager.Public;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
    public partial class PersonListControl : System.Windows.Controls.UserControl
    {


        private List<PersonBasicInfo> infoList = new List<PersonBasicInfo>();



        public PersonListControl()
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

            //System.Windows.MessageBox.Show(this.ActualWidth.ToString());
            //System.Windows.MessageBox.Show(this.ActualHeight.ToString());


            //double x = SystemParameters.WorkArea.Width;//得到屏幕工作区域宽度
            //double y = SystemParameters.WorkArea.Height;//得到屏幕工作区域高度
            //double x1 = SystemParameters.PrimaryScreenWidth;//得到屏幕整体宽度
        

            PersonModel model = new PersonModel();


            infoList = model.GetPersonBasicInfos("", "", "",false);

            this.dg_GrouperList.ItemsSource = null;
            this.dg_GrouperList.ItemsSource = infoList;
            


        }





        private void buttonImportClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            //openFile.Filter = "Excel(*.xlsx)|*.xlsx|Excel(*.xls)|*.xls";

            //openFile.InitialDirectory = System.Windows.Forms.Application.StartupPath;

            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            openFile.Multiselect = false;

            openFile.RestoreDirectory = true;


            DialogResult dr = openFile.ShowDialog();


            if (dr == DialogResult.Cancel)
            {

                //var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                //{
                //    Filter = "Excel Files (*.sql)|*.sql"
                //};
                //var result = openFileDialog.ShowDialog();
                //if (result == true)
                //{
                //string  aa = openFileDialog.FileName;
                //}



                return ;
            }
            var filePath = openFile.FileName;
          //  var filePath = "C:\\刘坤.xls";
            string fileType = System.IO.Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(fileType))
            {
                return;
            }
 

            PersonInfoDialog dialog = new PersonInfoDialog(filePath);

            dialog.Closed += Dialog_Closed;
            dialog.ShowDialog();
        }








        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            PersonInfoDialog  dialog = new PersonInfoDialog();

            dialog.Closed += Dialog_Closed;
            dialog.ShowDialog();


        }

        private void Dialog_Closed(object sender, EventArgs e)
        {

            InitData();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            PersonModel model = new PersonModel();


            foreach (PersonBasicInfo tempInfo in infoList)
            {

                if (tempInfo.ISCHECKED == true)
                {
                    model.DeletePersonInfo(tempInfo.NAMEID);
                }

            }

            System.Windows.Forms.MessageBox.Show("删除完毕！");
            InitData();
        }

        private void buttonModify_Click(object sender, RoutedEventArgs e)
        {



            foreach (PersonBasicInfo tempInfo in infoList)
            {

                if (tempInfo.ISCHECKED == true)
                {
                    PersonInfoDialog dialog = new PlanDatabase.PersonInfoDialog(tempInfo,false);
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
                    PersonBasicInfo personBasicInfo = dgr.DataContext as PersonBasicInfo;
                    if (personBasicInfo != null)
                    {
                        PersonInfoDialog dialog = new PlanDatabase.PersonInfoDialog(personBasicInfo,true);

                        dialog.Show();
                    }
                }
            }
        }

       
    }
}
