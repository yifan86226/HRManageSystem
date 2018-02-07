
using AT_BC.Common;
using CO_IA.Data;
using DevExpress.Xpf.LayoutControl;
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
    /// ActivityManageImage.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityManageImage : UserControl
    {
        List<ActivityPlaceLocationImageView> viewList = new List<ActivityPlaceLocationImageView>();
        List<ActivityPlaceLocationImage> ActivityPlaceLocationImageList;
        bool CanEdit = false;
        string locationGuid = "";
        public ActivityManageImage()
        {
            InitializeComponent();
        }
       
        public void setSource(List<ActivityPlaceLocationImage> activityPlaceLocationImage, string _locationGuid)
        {
            locationGuid = _locationGuid;
            if (!string.IsNullOrEmpty(_locationGuid))
                CanEdit = true;
            ActivityPlaceLocationImageList = activityPlaceLocationImage;
            if (ActivityPlaceLocationImageList == null)
                ActivityPlaceLocationImageList = new List<ActivityPlaceLocationImage>();

            if (activityPlaceLocationImage != null && activityPlaceLocationImage.Count > 0)
            {
                foreach (var item in activityPlaceLocationImage)
                {
                    ActivityPlaceLocationImageView view = new ActivityPlaceLocationImageView();
                    view.GUID = item.GUID;
                    view.ImageName = item.ImageName;
                    view.Image = item.Image;
                    if (CanEdit)
                        view.IsVisible = Visibility.Visible;
                    else
                        view.IsVisible = Visibility.Collapsed;
                    viewList.Add(view);
                }
            }
            if (CanEdit)
            {
                ActivityPlaceLocationImageView gpvAdd = new ActivityPlaceLocationImageView();
                gpvAdd.GUID = "add";
                gpvAdd.ImageName = "添加";
                gpvAdd.IsVisible = Visibility.Hidden;
                gpvAdd.TYPE = -1;
                gpvAdd.Cursor = "Hand";
                gpvAdd.Image = ClientHelper.ResourceImageToBytes("pack://application:,,,/CO_IA.Themes;component/Images/Area/add.png");
                viewList.Add(gpvAdd);
            }

            this.flc_ImgList.ItemsSource = viewList;
        }

        private void SelectImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            var groupBox = VisualTreeHelperExtension.GetParentObject<DevExpress.Xpf.LayoutControl.GroupBox>(img);
            ActivityPlaceLocationImageView gpv = groupBox.DataContext as ActivityPlaceLocationImageView;
            if (gpv.GUID == "add")
            {
                LocationImageDialog imgDialog = new LocationImageDialog();
                if (imgDialog.ShowDialog(this) == true)
                {
                    imgDialog.EditView.ACTIVITY_PLACE_LOCATION_GUID = locationGuid;
                    viewList.Insert(viewList.Count - 1, imgDialog.EditView);

                    ActivityPlaceLocationImage newitem = new ActivityPlaceLocationImage();
                    newitem.GUID = imgDialog.EditView.GUID;
                    newitem.ImageName = imgDialog.EditView.ImageName;
                    newitem.TYPE = imgDialog.EditView.TYPE;
                    newitem.Image = imgDialog.EditView.Image;
                    newitem.ACTIVITY_PLACE_LOCATION_GUID = imgDialog.EditView.ACTIVITY_PLACE_LOCATION_GUID;
                    ActivityPlaceLocationImageList.Add(newitem);

                    RefreshList();
                }
            }
        }
        private void Img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //var groupBox = (DevExpress.Xpf.LayoutControl.GroupBox)sender;
            Image img = sender as Image;
            var groupBox = VisualTreeHelperExtension.GetParentObject<DevExpress.Xpf.LayoutControl.GroupBox>(img);
            ActivityPlaceLocationImageView gpv = groupBox.DataContext as ActivityPlaceLocationImageView;
            
            groupBox.State = groupBox.State == GroupBoxState.Normal ? GroupBoxState.Maximized : GroupBoxState.Normal;
            
        }
        private void btn_ModifyClick(object sender, MouseButtonEventArgs e)
        {


            e.Handled = true;
            Image btn = sender as Image;
            ActivityPlaceLocationImageView itemview = btn.DataContext as ActivityPlaceLocationImageView;
            if (itemview != null)
            {
                LocationImageDialog imgDialog = new LocationImageDialog(itemview);
                if (imgDialog.ShowDialog() == true)
                {
                    for (int i = 0; i < viewList.Count; i++)
                    {
                        if (viewList[i].GUID == imgDialog.EditView.GUID)
                        {
                            viewList[i].ImageName = imgDialog.EditView.ImageName;
                            viewList[i].Image = imgDialog.EditView.Image;
                            viewList[i].TYPE = imgDialog.EditView.TYPE;
                            break;
                        }
                    }
                    RefreshList();
                    for (int i = 0; i < ActivityPlaceLocationImageList.Count; i++)
                    {
                        if (ActivityPlaceLocationImageList[i].GUID == imgDialog.EditView.GUID)
                        {
                            ActivityPlaceLocationImageList[i].ImageName = imgDialog.EditView.ImageName;
                            ActivityPlaceLocationImageList[i].Image = imgDialog.EditView.Image;
                            ActivityPlaceLocationImageList[i].TYPE = imgDialog.EditView.TYPE;
                            break;
                        }
                    }

                }
            }
        }

        private void btn_DelClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (MessageBox.Show("是否删除此图片？", "询问", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Image btn = sender as Image;
                ActivityPlaceLocationImageView itemview = btn.DataContext as ActivityPlaceLocationImageView;
                if (itemview != null)
                {
                    foreach (var item in viewList)
                    {
                        if (item.GUID == itemview.GUID)
                        {
                            viewList.Remove(item);
                            RefreshList();
                            break;
                        }
                    }
                    //更新源
                    foreach (var item in ActivityPlaceLocationImageList)
                    {
                        if (item.GUID == itemview.GUID)
                        {
                            ActivityPlaceLocationImageList.Remove(item);
                            break;
                        }
                    }

                }
            }
        }

        private void RefreshList()
        {
            this.flc_ImgList.ItemsSource = null;
            this.flc_ImgList.ItemsSource = viewList;

        }
        private string GetMaxID()
        {
            foreach (var group in flc_ImgList.Children)
            {
                DevExpress.Xpf.LayoutControl.GroupBox g = group as DevExpress.Xpf.LayoutControl.GroupBox;
                if (g != null && g.State == GroupBoxState.Maximized)
                    return g.Tag == null ? "" : g.Tag.ToString();
            }
            return "";
        }
        private void SetMaxID(string id)
        {
            foreach (var group in flc_ImgList.Children)
            {
                DevExpress.Xpf.LayoutControl.GroupBox g = group as DevExpress.Xpf.LayoutControl.GroupBox;
                if (g.Tag != null && g.Tag.ToString() == id)
                {
                    g.State = GroupBoxState.Maximized;
                }
            }
        }

    }
    public class ActivityPlaceLocationImageView : ActivityPlaceLocationImage
    {       
        private Visibility _isVisible;
        public Visibility IsVisible
        {
            get
            {
                return this._isVisible;
            }
            set
            {
                this._isVisible = value;
                NotifyPropertyChanged("IsVisible");
            }
        }
        public string Cursor { get; set; }
    }
    public class TypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int type = (int)value;
            if (type == 0)
                return "平面图 - ";
            if (type == 1)
                return "其它 - ";
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
