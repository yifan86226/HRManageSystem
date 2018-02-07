using CO_IA.Client;
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

namespace CO_IA.Client
{
    /// <summary>
    /// LocationImageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LocationImageDialog : Window
    {
        bool changeImage = false;
        public ActivityPlaceLocationImageView EditView;
        public LocationImageDialog()
        {
            InitializeComponent();
            this.Title = "添加";
            EditView = new ActivityPlaceLocationImageView();
            EditView.GUID = Utility.NewGuid();
            SetDefaultImage();
            this.DataContext = EditView;
        }
        public LocationImageDialog(ActivityPlaceLocationImageView editView)
        {
            InitializeComponent();
            EditView = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<ActivityPlaceLocationImageView>(editView);
            SetDefaultImage();
            this.Title = "编辑";
            this.DataContext = EditView;
        }
        public void SetDefaultImage()
        {
            if (EditView.Image == null)
            {
                EditView.Image = ClientHelper.ResourceImageToBytes("pack://application:,,,/CO_IA.Themes;component/Images/Area/add.png");
            }
            else
                changeImage = true;
        }
        private bool Validated()
        {
            bool IsSuccess = true;
            StringBuilder strmsg = new StringBuilder();

            if (string.IsNullOrEmpty(EditView.ImageName))
            {
                strmsg.Append("名称不能为空! \r");
                IsSuccess = false;
            }
            if (!changeImage)
            {
                strmsg.Append("请选择图片! \r");
                IsSuccess = false;
            }

            
            if (!string.IsNullOrEmpty(strmsg.ToString()))
            {
                MessageBox.Show(strmsg.ToString(), "提示", MessageBoxButton.OK);
            }
            return IsSuccess;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Validated())
                this.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EditView = null;
            this.DialogResult = false;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();//打开选择文件窗口
                ofd.Filter = "jpg|*.jpg|png|*.png|bmp|*.bmp|gif|*.gif";//过滤器
                if (ofd.ShowDialog() == true)
                {
                    string fileName = ofd.FileName;//获得文件的完整路径
                    EditView.Image = File.ReadAllBytes(fileName);
                    changeImage = true;
                }
            }
            catch 
            {
                MessageBox.Show("更换图片失败！");
            }
        }
    }
}
