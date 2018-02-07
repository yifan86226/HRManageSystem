#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：人员细节弹出窗口
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using CO_IA.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// PersonDetailInfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PersonDetailInfoDialog : Window
    {

        public int mode = 0;//0:新增； 1：修改

        public PP_PersonInfo PersonInfoData = new PP_PersonInfo();


        private string tempOrgGuid = "";

        public PersonDetailInfoDialog(string orgGuid)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            tempOrgGuid = orgGuid;
        }

        public PersonDetailInfoDialog(PP_PersonInfo personinfo)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;


            PersonInfoData = personinfo;
            mode = 1;//修改

            tb_Name.Text = personinfo.NAME;
            tb_Unit.Text = personinfo.UNIT;
            tb_Duty.Text = personinfo.DUTY;
            tb_Phone.Text = personinfo.PHONE;

            if (personinfo.PHOTO != null)
            {
                try
                {

                    #region read the image from a bytes array

                    System.IO.MemoryStream ms = new System.IO.MemoryStream(personinfo.PHOTO);//imageData是从数据库中读取出来的字节数组

                    ms.Seek(0, System.IO.SeekOrigin.Begin);

                    BitmapImage newBitmapImage = new BitmapImage();

                    newBitmapImage.BeginInit();

                    newBitmapImage.StreamSource = ms;

                    newBitmapImage.EndInit();

                    this.img_PersonImage.Source = newBitmapImage;

                    #endregion
                }
                catch
                {

                }
            }

        }

        /// <summary>
        /// 点击保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            if (mode == 0)
            {
                PersonInfoData.GUID =CO_IA.Client.Utility.NewGuid();
                PersonInfoData.ORG_GUID = tempOrgGuid;
                PersonInfoData.PERSON_TYPE = 4;
                PersonInfoData.ADD_TYPE = 2;
            }

            PersonInfoData.NAME = tb_Name.Text.Trim();
            PersonInfoData.UNIT = tb_Unit.Text.Trim();
            PersonInfoData.DUTY = tb_Duty.Text.Trim();
            PersonInfoData.PHONE = tb_Phone.Text.Trim();
            this.DialogResult = true;
            this.Close();

        }

        /// <summary>
        /// 点击取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

            this.Close();
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_UploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "jpg|*.jpg|png|*.png|jpeg|*.jpeg";//过滤器
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;//获得文件的完整路径
                PersonInfoData.PHOTO = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中
                this.img_PersonImage.Source = new BitmapImage(new Uri(fileName));//将图片显示到Image控件上
            }
        }
    }
}
