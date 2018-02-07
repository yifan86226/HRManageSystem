using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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

namespace CO_IA.UI.Screen.Dialog
{
    /// <summary>
    /// ExtentProgram.xaml 的交互逻辑
    /// </summary>
    public partial class ExtentProgram : Window
    {
        ObservableCollection<ExtentProgramData> ProgramInfo = new ObservableCollection<ExtentProgramData>();
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private CheckBox chkAll;
        public ExtentProgram()
        {
            InitializeComponent();

           
            this.DataContext = ProgramInfo;
            this.Loaded += ExtentProgram_Loaded;
        }

        void ExtentProgram_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }
       
        private void LoadConfig()
        {
            ProgramInfo.Clear();         
            ConfigurationSectionGroup group = GetSectionGroup();
            if (group == null)
            {
                IniSectionGroup();
                return;
            }
            if (group.Sections.Count > 0)
            {
                foreach (var item in group.Sections)
                { 
                    ExtentProgramInfo info = item as ExtentProgramInfo;

                    if (info != null)
                        ProgramInfo.Add(ToExtentProgramData(info));
                }
            }            
        }

        private ConfigurationSectionGroup GetSectionGroup()
        {
            ConfigurationSectionGroup group = config.SectionGroups["ExtentProgram"];
            if(group==null)
                config.SectionGroups.Add("ExtentProgram", new ConfigurationSectionGroup());
            return group;
        }

        private ConfigurationSectionGroup IniSectionGroup()
        {
            ConfigurationSectionGroup group = config.SectionGroups["ExtentProgram"];
            if(group!=null)
                config.SectionGroups.Remove("ExtentProgram");
            config.SectionGroups.Add("ExtentProgram", new ConfigurationSectionGroup());
            return  config.SectionGroups["ExtentProgram"]; ;
        }
        private void AddSectionItem(ConfigurationSectionGroup group,string index,ExtentProgramData data)
        {
            ExtentProgramInfo info = ToExtentProgramInfo(data);
            group.Sections.Add(index.ToString(),info);
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            //this.UpdateCheckAllState();
        }
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;
            //IEnumerable<Equipment> dataSource = this.DataContext as IEnumerable<Equipment>;
            //if (dataSource != null)
            //{
            //    foreach (Equipment result in dataSource)
            //    {
            //        result.IsChecked = ischecked;
            //    }
            //}
        }
        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = (sender as CheckBox).IsChecked;
            if (!isChecked.HasValue)
            {
                return;
            }
            bool checkedState = isChecked.Value;
            //IEnumerable<Equipment> dataSource = this.DataContext as IEnumerable<Equipment>;
            //if (dataSource != null)
            //{
            //    foreach (Equipment result in dataSource)
            //    {
            //        if (result.IsChecked != checkedState)
            //        {
            //            this.chkAll.IsChecked = null;
            //            return;
            //        }
            //    }
            //}

            chkAll.IsChecked = checkedState;
        }
        private ExtentProgramData ToExtentProgramData(ExtentProgramInfo info)
        {
            if (info != null)
            {
                ExtentProgramData data = new ExtentProgramData();
                data.Name = info.Name;
                data.Path = info.Path;
                data.IconURL = info.IconURL;
                return data;
            }
            return null;
        }
        private ExtentProgramInfo ToExtentProgramInfo(ExtentProgramData info)
        {
            if (info != null)
            {
                ExtentProgramInfo data = new ExtentProgramInfo();
                data.Name = info.Name;
                data.Path = info.Path;
                data.IconURL = info.IconURL;
                if (string.IsNullOrEmpty(data.IconURL))
                    data.IconURL = "/CO_IA.UI.Screen;component/Images/defaultprogram.png";
                return data;
            }
            return null;
        }
        private void DeleteImageDir()
        {
            string AppPath = AppDomain.CurrentDomain.BaseDirectory + "EPImage";
            if (Directory.Exists(AppPath))
            {
                try
                {
                    Directory.Delete(AppPath);
                }
                catch
                {
                    //MessageBox.Show("删除文件夹失败！");
                }
            }
        }
        private void DeleteImageFile(string filePath)
        {//暂时不删除
            return;
            if (string.IsNullOrEmpty(filePath)||filePath.StartsWith("/"))
                return;
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    MessageBox.Show("删除图片文件失败！");
                }
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ExtentProgramData data = new ExtentProgramData();
            ProgramInfo info = new ProgramInfo(data);
            info.Title = "添加";
            if (info.ShowDialog(this)==true)
            {
                ProgramInfo.Add(data);
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            ExtentProgramData data = dglist.SelectedItem as ExtentProgramData;
            if (data != null)
            {
                string imgfile = data.IconURL;
                ProgramInfo info = new ProgramInfo(data);
                info.Title = "修改";
                if (info.ShowDialog(this) == true)
                {
                    if (data.IconURL != imgfile)
                    {
                        DeleteImageFile(imgfile);
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择修改项！");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ExtentProgramData data = dglist.SelectedItem as ExtentProgramData;
            if (data != null)
            {
                DeleteImageFile(data.IconURL);
                ProgramInfo.Remove(data);
                Save();
            }
            else
            {
                MessageBox.Show("请选择需要删除的项！");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(Save())
                MessageBox.Show("保存成功！");
            else
            {
                MessageBox.Show("保存失败！");
            }
        }
        private bool Save()
        {
            try
            {
                ConfigurationSectionGroup group = IniSectionGroup();
                if (ProgramInfo.Count > 0)
                {
                    for (int i = 0; i < ProgramInfo.Count; i++)
                    {
                        AddSectionItem(group, "item" + i.ToString(), ProgramInfo[i]);
                    }
                }
                if (ProgramInfo.Count == 0)
                {
                    DeleteImageDir();
                }
                config.Save(ConfigurationSaveMode.Minimal);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    public class ExtentProgramInfo : ConfigurationSection
    {

        [ConfigurationProperty("Name")]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }
        [ConfigurationProperty("Path")]
        public string Path
        {
            get { return (string)this["Path"]; }
            set { this["Path"] = value; }
        }

        [ConfigurationProperty("IconURL")]
        public string IconURL
        {
            get { return (string)this["IconURL"]; }
            set { this["IconURL"] = value; }
        }
        
    }
    public class ExtentProgramData : CheckableData<string>
    {
        private string name;
        public string Name
        {
            get { return name; }
            set {
                name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                this.NotifyPropertyChanged("Path");
            }
        }


        private string iconURL;
        public string IconURL
        {
            get { return iconURL; }
            set
            {
                iconURL = value;
                this.NotifyPropertyChanged("IconURL");
            }
        }
        
    }
}
