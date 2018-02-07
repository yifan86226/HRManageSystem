using CO_IA.Data;
using CO_IA.Data.PlanDatabase;
using DataManager.Public;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace CO_IA.UI.PlanDatabase
{
    /// <summary>
    /// ORGQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PersonOutInfoDialog : Window
    {

        private PersonOutInfo info = new PersonOutInfo();

        private bool IsReadOnly = false;
        private bool IsModify = false;


        public PersonOutInfoDialog()
        {
            InitializeComponent();



            info.OUTTIME = DateTime.Now.ToString("yyyy-MM-dd");
            info.BACKTIME = DateTime.Now.ToString("yyyy-MM-dd");


            InitControl();
    
            this.info.OPERATORTIME = DateTime.Now;
            //this.grid_main.DataContext = null;


            //this.cb_PersonList.s
            this.grid_main.DataContext = info;

            //this.txtOUTTIME.DateTime = DateTime.Now;

            //this.txtBACKTIME.DateTime = DateTime.Now;


            //this.txtOUTTIME.DateTime = DateTime.Now;




        }

        private void InitControl()
        {
            PersonModel model = new PersonModel();

            List<PersonBasicInfo> list = model.GetPersonBasicInfos("", ""," order by NAME");

            this.cb_PersonList.ValueMember = "NAMEID";

            this.cb_PersonList.DisplayMember = "NAME";


            this.cb_PersonList.ItemsSource = list;

            CodeDicModel codedicModel = new CodeDicModel();


            List<CodeDicItem> codeItemList = codedicModel.CodeDicItemList("10001", "");

            this.cb_OutType.ValueMember = "CODE";

            this.cb_OutType.DisplayMember = "NAME";


            this.cb_OutType.ItemsSource = codeItemList;



        }

    

        private void InitData()
        {


            PersonModel model = new PersonModel();

            List<PersonBasicInfo> list = model.GetPersonBasicInfos("", info.NAMEID, "");
            info.NAMEIDCODEITEM = list[0];
            this.cb_PersonList.SelectedItem = info.NAMEIDCODEITEM;


            CodeDicModel codedicModel = new CodeDicModel();

            List<CodeDicItem> codeItemList = codedicModel.CodeDicItemList("10001", info.INCIDENT);

            info.INCIDENTCODEITEM = codeItemList[0];
            this.cb_OutType.SelectedItem = info.INCIDENTCODEITEM;

        }


        public PersonOutInfoDialog(PersonOutInfo basicinfo, bool isReadOnly)
        {
            InitializeComponent();

            InitControl();

            IsReadOnly = isReadOnly;


            info = basicinfo;


            InitData();


            this.grid_main.DataContext = info;

            if (IsReadOnly == true)
            {
                this.grid_main.IsEnabled = false;

            }
            else
            {
                IsModify = true;
            }
        }



        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            PersonBasicInfo bi = this.cb_PersonList.SelectedItem as PersonBasicInfo;
            info.NAMEID = bi.NAMEID;
            info.NAME = bi.NAME;


            CodeDicItem cdI = this.cb_OutType.SelectedItem as CodeDicItem;
            info.INCIDENT = cdI.CODE;



            PersonOutModel model = new PersonOutModel();

            bool isresult = false;
            if (IsModify == false)
            {
                isresult = model.InsertPersoOutInfo(info);
            }
            else
            {
                isresult= model.ModifyPersonOutInfo(info);
            }
            if (isresult)
            {
                MessageBox.Show("保存人员外出信息成功！");

                this.Close();
            }
            else
            {
                MessageBox.Show("保存人员外出信息失败 请确认后再进行保存！");
            }

        }
    }
}
