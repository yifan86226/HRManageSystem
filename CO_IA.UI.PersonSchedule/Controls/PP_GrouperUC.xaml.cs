#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：组员展示控件
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using CO_IA.Data;
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

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// PP_GrouperUC.xaml 的交互逻辑
    /// </summary>
    public partial class PP_GrouperUC : UserControl
    {
        public PP_PersonInfo PersonInfoData = new PP_PersonInfo();

        public PP_GrouperUC(PP_PersonInfo personInfo, bool  isShow=true)
        {
            InitializeComponent();

            PersonInfoData = personInfo;

            if (isShow == false)
            {
                this.cb_PersonName.Visibility= System.Windows.Visibility.Hidden;
                this.tb_PersonName.Visibility = System.Windows.Visibility.Visible;
            }
            this.cb_PersonName.Content = PersonInfoData.NAME;
            this.tb_PersonName.Text = PersonInfoData.NAME;

            if (PersonInfoData.PHOTO != null)
            {
                try
                {

                    #region read the image from a bytes array

                    System.IO.MemoryStream ms = new System.IO.MemoryStream(PersonInfoData.PHOTO);//imageData是从数据库中读取出来的字节数组

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
    }
}
