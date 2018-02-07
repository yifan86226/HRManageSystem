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
    public partial class PersonRewardPunishInfoDialog : Window
    {

        private PersonRewardPunishInfo info = new PersonRewardPunishInfo();

        private bool IsReadOnly = false;
        private bool IsModify = false;




        public PersonRewardPunishInfoDialog(string type)
        {
            InitializeComponent();



            info.RPTIME = DateTime.Now.ToString("yyyy-MM-dd");



            InitControl(type);


            info.RPTYPE = type;
            info.OPERATORTIME = DateTime.Now;

         
            this.grid_main.DataContext = info;


            //this.editor.DateTime = DateTime.Now;
        }

        private void InitControl(string type)
        {

            if (type == "0")
            {
                this.tb_title.Text = "人员量化考核信息加分登记";
            }
            else if (type == "1")
            {
                this.tb_title.Text = "人员量化考核信息减分登记";
            }



            PersonModel model = new PersonModel();

            List<PersonBasicInfo> list = model.GetPersonBasicInfos("", "", " order by NAME");

            this.cb_PersonList.ValueMember = "NAMEID";

            this.cb_PersonList.DisplayMember = "NAME";


            this.cb_PersonList.ItemsSource = list;


            CodeDicModel codedicModel = new CodeDicModel();

            string codeType = "10002";
            if (type == "0")
            {
                codeType = "10002";
            }
            else if (type == "1")
            { codeType = "10003"; }

            List<CodeDicItem> codeItemList = codedicModel.CodeDicItemList(codeType, "");

            this.cb_type.ValueMember = "CODE";

            this.cb_type.DisplayMember = "NAME";


            this.cb_type.ItemsSource = codeItemList;

        }


        private void InitData(string type)
        {


            PersonModel model = new PersonModel();

            List<PersonBasicInfo> list = model.GetPersonBasicInfos("", info.NAMEID, "");
            info.NAMEIDCODEITEM = list[0];
            this.cb_PersonList.SelectedItem = info.NAMEIDCODEITEM;


            CodeDicModel codedicModel = new CodeDicModel();

            string codeType = "10002";
            if (type == "0")
            {
                codeType = "10002";
            }
            else if (type == "1")
            { codeType = "10003"; }



            List<CodeDicItem> codeItemList = codedicModel.CodeDicItemList(codeType, info.INCIDENT);

            info.INCIDENTCODEITEM = codeItemList[0];
            this.cb_type.SelectedItem = info.INCIDENTCODEITEM;

        }



        public PersonRewardPunishInfoDialog(PersonRewardPunishInfo basicinfo, bool isReadOnly)
        {
            InitializeComponent();


            IsReadOnly = isReadOnly;

            InitControl(basicinfo.RPTYPE);


            info = basicinfo;


            InitData(info.RPTYPE);

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


            CodeDicItem cdI = this.cb_type.SelectedItem as CodeDicItem;
            info.INCIDENT = cdI.CODE;




            PersonRewardPunishInfoModel model = new PersonRewardPunishInfoModel();

            bool isresult = false;
            if (IsModify == false)
            {
                isresult = model.InsertPersonRewardPunishInfo(info);
            }
            else
            {
                isresult= model.ModifyPersonRewardPunishInfo(info);
            }
            if (isresult)
            {
                MessageBox.Show("保存人员量化考核信息成功！");

                this.Close();
            }
            else
            {
                MessageBox.Show("保存人员量化考核信息失败 请确认后再进行保存！");
            }

        }
    }
}
