using AT_BC.Data;
using CO_IA.Client;
using CO_IA.Data;
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

namespace CO_IA.UI.Screen
{
    /// <summary>
    /// VideoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VideoWindow : Window
    {
        public bool Played = false;
        public string AreaID;
        public VideoWindow(string areaID)
        {
            InitializeComponent();

            //if (string.IsNullOrEmpty(areaID))
            //{
            //    MessageBox.Show("没有区域信息");
            //    return;
            //}
            AreaID = areaID;

            if (!string.IsNullOrEmpty(areaID))
            {
                var area = Obj.ActivityPlaces.Where(item => item.Guid == areaID).ToArray();
                if (area == null || area.Length == 0)
                {
                    MessageBox.Show("没有区域信息");
                    return;
                }
                this.Title = "查看视频 - 区域：" + area[0].Name;
            }
            else
                this.Title = "查看视频 - 所有";

            this.Loaded += VideoWindow_Loaded;

            return;
            //var device = AT_BC.Video.Hikvision.VideoDeviceManager.RegisterVideoDevice(new CO_IA.Data.VideoSetting
            //{
            //    IP = "192.168.3.132",
            //    Key = "1234",
            //    Port = 8000,
            //    UserName = "admin",
            //    Password = "best1234",
            //    Name = "测试设备"
            //});
            //device.Login();
            //this.videoMonitor.Play(device);
        }

        void VideoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<VideoItem> itemList = new List<VideoItem>();
            VideoSetting[] vSetting = getVideoSettig(AreaID);
            var Settings = vSetting.OrderBy(p=>p.OwnerGuid).ToArray();
            string area = "";
            VideoItem AreaItem = null;
            foreach (VideoSetting setting in Settings)
            {
                if (!string.IsNullOrEmpty(AreaID) && AreaID != setting.OwnerGuid)
                {
                    continue;
                }
                VideoItem item = null;
                if (area == "" || area != setting.OwnerGuid)
                {
                    area = setting.OwnerGuid;
                    item = new VideoItem();
                    var areainfo = Obj.ActivityPlaces.Where(p => p.Guid == area).ToArray();
                    if (areainfo == null || areainfo.Length == 1)
                    {
                        item.Name = areainfo[0].Name;
                    }
                    item.IsChkVisible = System.Windows.Visibility.Collapsed;
                    item.IsChecked = false;
                    item.ImageSource = @"/CO_IA.Themes;component/Images/Area/defaultPlace.png";
                    itemList.Add(item);
                    AreaItem = item;
                    
                }
                item = new VideoItem();
                item.VideoSet = setting;
                item.Name = setting.Name;
                item.IsChkVisible = System.Windows.Visibility.Visible;
                item.ImageSource = @"/CO_IA.UI.Screen;component/Images/video_64.png";
                AreaItem.Children.Add(item);
            }
            tv.ItemsSource = itemList;
            
        }
        public VideoSetting[] getVideoSettig(string AreaId)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, VideoSetting[]>(channel =>
            {
                return channel.GetVideoSettings(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });
        }
        private void buttonPreview_Click(object sender, RoutedEventArgs e)
        {
            this.videoMonitor.Preview();
        }

        private void buttonCapture_Click(object sender, RoutedEventArgs e)
        {
            CaptureJpg();
        }

        private void buttonRecord_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Content.ToString() == "录像")
            {
                btn.Content = "停止";
                btn.ToolTip = "停止录制视频";
                Record();
            }
            else {
                btn.Content = "录像";
                btn.ToolTip = "开始录制视频";
                StopRecord();
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.videoMonitor.Dispose();
        }

        private void CheckBoxGroups_Click(object sender, RoutedEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
            CheckBox chk = sender as CheckBox;
            if (chk != null)
            {
                SetVideo(chk.IsChecked.Value);
            }
        }
        private void SetVideo(bool flag)
        {
            if (flag)
            {
                this.videoMonitor.Dispose();
                VideoItem selectItem = SetTreeItemCheck(false);
                if(selectItem!=null)
                {
                    Play(selectItem);
                }
            }
            else
            {
                CloseVideo();
            }

        }
        private void Play(VideoItem video)
        {
            Played = false;
            if (video == null)
            {
                return;
            }
            try
            {
                var device = AT_BC.Video.Hikvision.VideoDeviceManager.RegisterVideoDevice(new CO_IA.Data.VideoSetting
                {
                    IP = video.VideoSet.IP,
                    Key = video.VideoSet.Key,
                    Port = video.VideoSet.Port,
                    UserName = video.VideoSet.UserName,
                    Password = video.VideoSet.Password,
                    Name = video.VideoSet.Name
                });
                device.Login();
                this.videoMonitor.Play(device);
                Played = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("播放视频失败，具体原因：" + ex.Message, "出错信息");
            }
        }
        private void CloseVideo()
        {
            Played = false;
            try
            {
                this.videoMonitor.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("关闭视频失败，具体原因：" + ex.Message, "出错信息");
            }
        }
        private void Record()
        {
            if (!Played)
            {
                MessageBox.Show("请先打开视频！");
                return;
            }
            try
            {
                this.videoMonitor.Record();
            }
            catch (Exception ex)
            {
                MessageBox.Show("录制视频失败，具体原因：" + ex.Message, "出错信息");
            }
        }
        private void StopRecord()
        {
            try
            {
                this.videoMonitor.StopRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show("停止录制视频失败，具体原因：" + ex.Message, "出错信息");
            }
        }
        private void CaptureJpg()
        {
            if (!Played)
            {
                MessageBox.Show("请先打开视频！");
                return;
            }
            try
            {
                this.videoMonitor.CaptureJpg();
            }
            catch(Exception ex)
            {
                MessageBox.Show("抓图失败，具体原因：" + ex.Message, "出错信息");
            }
        }
        private VideoItem SetTreeItemCheck(bool flag)
        {
            VideoItem treeitem = tv.SelectedItem as VideoItem;
            List<VideoItem> itemList = tv.ItemsSource as List<VideoItem>;
            if (itemList != null)
            {
                foreach (var item in itemList)
                {
                    if (item.GUID == treeitem.GUID)
                        continue;
                    item.IsChecked = flag;
                    if (item.Children.Count > 0)
                    {
                        foreach (var itm in item.Children)
                        {
                            if (itm.GUID == treeitem.GUID)
                                continue;
                            itm.IsChecked = flag;
                        }
                    }
                }
            }
            return treeitem;
        }
        private DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);
            return source;
        }
        private void listBoxPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //VideoItem item = listBox.SelectedItem as VideoItem;
            //if (item != null)
            //{
            //    this.videoMonitor.Dispose();
            //    var device = AT_BC.Video.Hikvision.VideoDeviceManager.RegisterVideoDevice(new CO_IA.Data.VideoSetting
            //    {
            //        IP = item.VideoSet.IP,
            //        Key = item.VideoSet.Key,
            //        Port = item.VideoSet.Port,
            //        UserName = item.VideoSet.UserName,
            //        Password = item.VideoSet.Password,
            //        Name = item.VideoSet.Name
            //    });
            //    device.Login();
            //    this.videoMonitor.Play(device);
            //}
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CloseVideo();
            //AT_BC.Video.Hikvision.VideoDeviceManager.Clean();

        }

        private void buttonOpenDir_Click(object sender, RoutedEventArgs e)
        {
            //打开存储目录
            string dir = "SaveFiles";
            string baseDir = AppDomain.CurrentDomain.BaseDirectory + dir;
            if (Directory.Exists(baseDir))
            {
                System.Diagnostics.Process.Start("Explorer.exe", baseDir);
            }
        }


    }
    public class VideoItem : NotifyPropertyChangedObject
    {
        public List<VideoItem> Children { get; set; }
        public VideoItem()
        {
            GUID = Utility.NewGuid();
            Children = new List<VideoItem>();
        }

        private string guid;

        public string GUID
        {
            get { return guid; }
            set { guid = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        private VideoSetting vSet;
        public VideoSetting VideoSet
        {
            get { return vSet; }
            set { 
                vSet = value; 
            }
        }

        private string imageSource;

        public string ImageSource
        {
            get { return imageSource; }
            set { imageSource = value; }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value; NotifyPropertyChanged("IsChecked");

            }
        }

        private Visibility _isChkVisible;
        public Visibility IsChkVisible
        {
            get
            {
                return _isChkVisible;
            }
            set
            {
                _isChkVisible = value; NotifyPropertyChanged("IsChkVisible");

            }
        }

        private object tag;

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
    }
}
