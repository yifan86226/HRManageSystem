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
using System.Data;
using System.Drawing.Imaging;
using System.Data.OleDb;

namespace CO_IA.UI.PlanDatabase
{
    /// <summary>
    /// ORGQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PersonInfoDialog : Window
    {

        private PersonBasicInfo info = new PersonBasicInfo();

        private bool IsReadOnly = false;

        private bool IsModify = false;
  

        public PersonInfoDialog()
        {
            InitializeComponent();
   

            this.grid_main.DataContext = info;
        }



        


        public PersonInfoDialog(PersonBasicInfo basicinfo, bool isReadOnly)
        {
            InitializeComponent();


            IsReadOnly = isReadOnly;

            PersonModel model = new PersonModel();

            info= model.GetPersonBasicInfos("", basicinfo.NAMEID, "")[0];
            //info = basicinfo;

            this.grid_main.DataContext = info;

            if (IsReadOnly == true)
            {
                this.grid_detail.IsEnabled = false;
                this.btnSave.IsEnabled = false;
                this.btnCancel.IsEnabled = false;

            }
            else
            {
                IsModify = true;
            }
        }

        public PersonInfoDialog(string filePath)
        {
            InitializeComponent();


            EduceExcelNPOI(filePath);

            this.grid_main.DataContext = info;

        }


        private void EduceExcel(string filePath)
        {
            //Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();//建立Excel对象 
            //app.Visible = false;//让Excel文件可见 
            //Microsoft.Office.Interop.Excel.Workbook objbook;
            //objbook = app.Workbooks.Add(filePath);
            //Microsoft.Office.Interop.Excel.Worksheet worksheet;
            //worksheet = (Microsoft.Office.Interop.Excel.Worksheet)objbook.Worksheets[1];


            //info.NAME = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[3, 3]).Text.ToString();   //姓名
            //info.SEX = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[3, 5]).Text.ToString();           //性别     
            //info.BIRTHDATE = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[3, 7]).Text.ToString();//出生年月
            //info.NATION = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[4, 3]).Text.ToString();//民族
            //info.ENLISTMENTDATE = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[4, 5]).Text.ToString();//入伍年月
            //info.MILITARYRANK = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[4, 7]).Text.ToString();//军衔
            //info.ORIGINPLACE = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[5, 3]).Text.ToString();//籍贯
            //info.ARMYSEAT = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[5, 5]).Text.ToString();//入伍所在地
            //info.MAJOR = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[5, 7]).Text.ToString();//专业
            //info.EDUCATION = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[6, 3]).Text.ToString();//文化程度
            //info.POLITICAL = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[6, 5]).Text.ToString();//政治面貌
            //info.PARTYTIME = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[6, 7]).Text.ToString();//党团时间
            //info.HJQK = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[7, 3]).Text.ToString();//户籍情况
            //info.BLOODTYPE = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[7, 5]).Text.ToString();//血型
            //info.IDCARD = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[7, 7]).Text.ToString();//身份证号
            //info.HOBBY = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[8, 3]).Text.ToString();//兴趣爱好
            //info.CHARACTERTYPE = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[8, 5]).Text.ToString();//性格类型
            //info.QQID = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[8, 7]).Text.ToString();//QQ号码
            //info.HOMEADDRESS = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[9, 3]).Text.ToString();//家庭住址
            //info.PHONE = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[9, 7]).Text.ToString();//联系电话
            //info.SPOUSENAME = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[10, 3]).Text.ToString();//配偶名称
            //info.SPOUSEMARRIAGETIME = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[10, 5]).Text.ToString();//结婚时间
            //info.SPOUSESUNIT = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[10, 7]).Text.ToString();//配偶工作单位
            //info.SPOUSESHOMEADDRESS = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[11, 3]).Text.ToString();//配偶家庭住址
            //info.SPOUSESPHONE = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[11, 7]).Text.ToString();//配偶联系电话
            //info.CHILDRENNAME = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[12, 3]).Text.ToString();//子女姓名
            //info.CHILDRENSEX = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[12, 5]).Text.ToString();//子女性别
            //info.CHILDRENBIRTH = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[12, 7]).Text.ToString();//出生年月
            //info.ENLISTINGRESUME = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[13, 2]).Text.ToString();//入伍后简历
            //info.TRAININGSITUATION = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[14, 2]).Text.ToString();//培训情况
            //info.REWARDSPUNISHMENTS = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[15, 2]).Text.ToString();//奖惩情况
            //info.FAMILYMEMBER = ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[16, 2]).Text.ToString();//家庭主要成员

            ////取存图片；
            //if (worksheet.Shapes.Count > 0)
            //{

            //    Microsoft.Office.Interop.Excel.Shape s = worksheet.Shapes.Item(1) as Microsoft.Office.Interop.Excel.Shape;

            //    System.Windows.Forms.Clipboard.Clear();//Clipboard类是引用 .NET组件中System.Windows.Forms的
            //    s.CopyPicture(System.Windows.Forms.Appearance.Button, Microsoft.Office.Interop.Excel.XlCopyPictureFormat.xlBitmap); //COPY到内存。
            //    System.Windows.Forms.IDataObject iData = System.Windows.Forms.Clipboard.GetDataObject();


            //    if (iData != null && iData.GetDataPresent(System.Windows.Forms.DataFormats.Bitmap))
            //    {
            //        System.Drawing.Image img = System.Windows.Forms.Clipboard.GetImage(); //从内存读取图片
            //        if (img != null)
            //        {

            //            info.PHOTO = ImageToBytes(img);                        
            //        }
            //    }
            //    else
            //    {
            //    }


            //}
        }



        private void EduceExcelNPOI(string filePath)
        {

            DataTable dt =   NPOIHelper.ImportExceltoDt(filePath);
   

            info.NAME = dt.Rows[1][2].ToString();   //姓名
            info.SEX = dt.Rows[1][4] .ToString();           //性别     
            info.BIRTHDATE = dt.Rows[1][6].ToString();//出生年月
            info.NATION = dt.Rows[2][2].ToString();//民族
            info.ENLISTMENTDATE = dt.Rows[2][4].ToString();//入伍年月
            info.MILITARYRANK = dt.Rows[2][6].ToString();//军衔
            info.ORIGINPLACE = dt.Rows[3][2].ToString();//籍贯
            info.ARMYSEAT = dt.Rows[3][4].ToString();//入伍所在地
            info.MAJOR = dt.Rows[3][6].ToString();//专业
            info.EDUCATION = dt.Rows[4][2].ToString();//文化程度
            info.POLITICAL = dt.Rows[4][4].ToString();//政治面貌
            info.PARTYTIME = dt.Rows[4][6].ToString();//党团时间
            info.HJQK = dt.Rows[5][2].ToString();//户籍情况
            info.BLOODTYPE = dt.Rows[5][4].ToString();//血型
            info.IDCARD = dt.Rows[5][6].ToString();//身份证号
            info.HOBBY = dt.Rows[6][2].ToString();//兴趣爱好
            info.CHARACTERTYPE = dt.Rows[6][4].ToString();//性格类型
            info.QQID = dt.Rows[6][6].ToString();//QQ号码
            info.HOMEADDRESS = dt.Rows[7][2].ToString();//家庭住址
            info.PHONE = dt.Rows[7][6].ToString();//联系电话
            info.SPOUSENAME = dt.Rows[8][2].ToString();//配偶名称
            info.SPOUSEMARRIAGETIME = dt.Rows[8][4].ToString();//结婚时间
            info.SPOUSESUNIT = dt.Rows[8][6].ToString();//配偶工作单位
            info.SPOUSESHOMEADDRESS = dt.Rows[9][2].ToString();//配偶家庭住址
            info.SPOUSESPHONE = dt.Rows[9][6].ToString();//配偶联系电话
            info.CHILDRENNAME = dt.Rows[10][2].ToString();//子女姓名
            info.CHILDRENSEX = dt.Rows[10][4].ToString();//子女性别
            info.CHILDRENBIRTH = dt.Rows[10][6].ToString();//出生年月
            info.ENLISTINGRESUME = dt.Rows[11][1].ToString();//入伍后简历
            info.TRAININGSITUATION = dt.Rows[12][1].ToString();//培训情况
            info.REWARDSPUNISHMENTS = dt.Rows[13][1].ToString();//奖惩情况
            info.FAMILYMEMBER = dt.Rows[14][1].ToString();//家庭主要成员

            //取存图片
            info.PHOTO = NPOIHelper.ExcelToImage(filePath);


            
        }






        /// <summary>
        /// Convert Image to Byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private byte[] ImageToBytes(System.Drawing.Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                else
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }


        //public DataSet LoadDataFromExcel(string filePath)
        //{
        //    try
        //    {
        //        string strConn;


        //        EduceExcel(filePath);


        //        if (filePath.EndsWith(".xls"))
        //        {
        //            strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
        //        }

        //        else
        //        {
        //            strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";

        //        }

        //        //strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=False;IMEX=1'";
        //        OleDbConnection OleConn = new OleDbConnection(strConn);
        //        OleConn.Open();
        //        String sql = "SELECT * FROM  [Sheet1$]";//可是更改Sheet名称，比如sheet2，等等  

        //        OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(sql, OleConn);
        //        DataSet OleDsExcle = new DataSet();
        //        OleDaExcel.Fill(OleDsExcle, "Sheet1");
        //        OleConn.Close();
        //        return OleDsExcle;
        //    }
        //    catch (Exception err)
        //    {
        //        System.Windows.MessageBox.Show("数据绑定Excel失败!失败原因：" + err.Message);
        //        return null;
        //    }
        //}


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {


            PersonModel model = new PersonModel();
            bool isresult = false;
            if (IsModify == false)
            {
                isresult = model.InsertPersonBasicInfo(info);

            } 
            else
            {
                isresult = model.ModifyPersonBasicInfo(info);

            } 

            if (isresult)
            {
                MessageBox.Show("保存人员信息成功！");

                this.Close();
            }
            else
            {
                MessageBox.Show("保存人员信息失败 请确认后再进行保存！");
            }

        }



        private void menuItemClearGrouper_Click(object sender, RoutedEventArgs e)
        {
            //清除
            //var element = ContextMenuService.GetPlacementTarget(LogicalTreeHelper.GetParent(sender as MenuItem));
            //Image img = element as Image;
            //PersonBasicInfo info = img.DataContext as PersonBasicInfo;
            if (info != null)
            {

                info.PHONE = null;
            }
        }




        private void img_photo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //PP_PersonInfo temp = grid_OrgLeader.DataContext as PP_PersonInfo;
            if (info == null || string.IsNullOrEmpty(info.NAME))
            {
                //MessageBox.Show("姓名不能为空！");
                //return;
            }

            if (IsReadOnly)
            {
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "图片|*.jpg;*.png;*.jpeg;*.bmp";//过滤器
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;//获得文件的完整路径

                byte[] imageData = File.ReadAllBytes(fileName);
                System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData);//imageData是从数据库中读取出来的字节数组

                ms.Seek(0, System.IO.SeekOrigin.Begin);

                BitmapImage newBitmapImage = new BitmapImage();

                newBitmapImage.BeginInit();

                newBitmapImage.StreamSource = ms;

                newBitmapImage.EndInit();

                image.Source = newBitmapImage;
            }
        }
    }
}
