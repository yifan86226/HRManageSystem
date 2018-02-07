using CO_IA.Data.ActivitySummarize;
using CO_IA.Data.Setting;
using I_CO_IA.ActivitySummarize;
using Microsoft.Win32;
using PT_BS_Service.Client.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO_IA.UI.ActivitySummarize
{
    /// <summary>
    /// SummarizeTotalTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class SummarizeTotalTemplate : Window
    {
        public SummarizeTotalTemplate()
        {
            InitializeComponent();
            STTemplateItemsSource = GetSTTemplate();
            this.DataContext = this;
        }
        #region 常量
        STTemplate sTTemplate = new STTemplate();
        #endregion
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;//检查文件是否存在
            dlg.Multiselect = false;//是否允许多选，false表示单选
            dlg.CheckPathExists = true;
            //只限制上传word和excel
            dlg.Filter = "Office Files|*.doc;*.docx;*.xls;*.xlsx";
            if ((bool)dlg.ShowDialog())
            {
                string filePath = dlg.FileName;
                if (UpLodeEnclouseFolder(filePath) == true)
                {
                    sTTemplate.GUID = CO_IA.Client.Utility.NewGuid();
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>
                        (channel =>
                        {
                            bool monitorResult = channel.SaveSummarizeTemplate(sTTemplate);
                            if (monitorResult == true)
                            {
                                STTemplateItemsSource = GetSTTemplate();
                                MessageBox.Show("保存成功！");
                                //this.Close();
                            }
                            else
                            {
                                MessageBox.Show("保存失败!");
                            }
                        });
                }
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int checkcount = STTemplateItemsSource.Count(r => r.IsChecked == true);
            if (checkcount == 0)
            {
                MessageBox.Show("请勾选需要删除的模板", "消息提示", MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show("确定删除已勾选的模板吗？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    List<string> guids = STTemplateItemsSource.Where(o => o.IsChecked == true).Select(r => r.GUID).ToList();
                    BeOperationInvoker.Invoke<I_CO_IA_ActivitySummarize>(channel =>
                    {
                        if (channel.DeleteSummarizeTemplate(guids))
                        {
                            //GetExamPlace();
                            STTemplateItemsSource = GetSTTemplate();
                            MessageBox.Show("删除成功!", "提示", MessageBoxButton.OK);
                        }
                        else
                        {
                            MessageBox.Show("删除失败!", "提示", MessageBoxButton.OK);
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("删除失败!原因：" + ex.Message.ToString(), "提示", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// 下载功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                STTemplate summarize = (sender as System.Windows.Documents.Hyperlink).Tag as STTemplate;
                if (summarize == null)
                    return;
                string guid = summarize.GUID;

                STTemplate sTTemplateFile = new STTemplate();
                BeOperationInvoker.Invoke<I_CO_IA_ActivitySummarize>(channel =>
                {
                    sTTemplateFile = channel.GetSummarizeTemplateFile(guid);
                });

                byte[] file = sTTemplateFile.FILEDOC;
                string docstr = summarize.NAME.Substring(summarize.NAME.LastIndexOf('.'));
                string serviceDocPath = ByteConvertDocService(file, summarize.NAME);
                WebClient webClient = new WebClient();
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = summarize.NAME;
                //dlg.Reset();
                if (docstr == ".doc" || docstr == ".docx")
                {
                    dlg.Filter += "Word 文件|*.doc;*.docx";
                }
                else if (docstr == ".xls" || docstr == ".xlsx")
                {
                    dlg.Filter += "Excel 文件|*.xls;*.xlsx";
                }
                // dlg.Filter = "Office Files|*.doc;*.docx;*.xls;*.xlsx";
                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        webClient.DownloadFile(serviceDocPath, dlg.FileName);
                        MessageBox.Show("文件下载成功！");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.GetExceptionMessage());
            }
        }
        /// <summary>
        /// 二进制数据转换为word文件(缓存到服务器)
        /// </summary>
        /// <param name="data">二进制数据</param>
        /// <param name="fileName">word文件名</param>
        /// <returns>文件保存的相对路径</returns>
        public string ByteConvertDocService(byte[] data, string fileName)
        {
            FileStream fs;
            string savePath = GetPath();
            savePath += fileName;
            if (System.IO.File.Exists(savePath))
            {
                //文件已经存在
                fs = new FileStream(savePath, FileMode.Truncate);
            }
            else
            {
                //先删除文件
                System.IO.File.Delete(savePath);
                //重新创建
                fs = new FileStream(savePath, FileMode.CreateNew);
            }
            BinaryWriter br = new BinaryWriter(fs);
            br.Write(data, 0, data.Length);
            br.Flush();
            br.Close();
            fs.Close();
            return savePath;
        }
        /// <summary>
        /// 上传功能
        /// </summary>
        /// <param name="ServerPath"></param>
        /// <param name="EnclouseFolderPath"></param>
        /// <returns></returns>
        private bool UpLodeEnclouseFolder(string ServerPath)
        {
            bool isresult = false;
            string SereverAndEnclousePath = "";
            //获取文件名
            string fileName = ServerPath.Substring(ServerPath.LastIndexOf("\\") + 1);
            string path = this.GetPath();
            SereverAndEnclousePath = System.IO.Path.Combine(path, fileName);
            FileStream fs = null;
            try
            {
                WebClient web = new WebClient();
                web.Credentials = CredentialCache.DefaultCredentials;
                //初始化上传文件  打开读取
                fs = new FileStream(ServerPath, FileMode.Open, FileAccess.Read);
                if (fs.Length / 1024 / 1024 > 20)
                {
                    MessageBox.Show("上传附件不支持超过20M大小的文件。");
                    isresult = false;
                }
                else
                {
                    BinaryReader br = new BinaryReader(fs);
                    byte[] btArray = br.ReadBytes((int)fs.Length);
                    Stream uplodeStream = web.OpenWrite(SereverAndEnclousePath, "PUT");
                    if (uplodeStream.CanWrite)
                    {
                        uplodeStream.Write(btArray, 0, btArray.Length);
                        uplodeStream.Flush();
                        uplodeStream.Close();
                        //显示文件格式
                        sTTemplate.NAME = fileName;
                        sTTemplate.FILESIZE = (fs.Length).ToString();
                        sTTemplate.FILEDOC = btArray;
                        isresult = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                fs.Close();//上传成功后，关闭流
            }

            return isresult;
        }
        /// <summary>
        /// 项目所在路径
        /// </summary>
        /// <returns></returns>
        public string GetPath()
        {
            string savePath = @"SysDoc\" + FormatNowTime(2) + @"\";
            //string allPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //int proPathLength = allPath.Substring(allPath.LastIndexOf("Output") - 1).Length;
            //string proPath = allPath.Substring(0, allPath.Length - proPathLength);
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, savePath);// proPath + @"\Output\" + "textoffice" + savePath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
        /// <summary>
        /// 格式化当前时间: 
        /// 1:yyMMddHHmmss; 2:yyyy-MM\dd\
        /// </summary>
        /// <returns></returns>
        public string FormatNowTime(int num)
        {
            if (num == 1)
            {
                return DateTime.Now.ToString("yyMM");
            }
            else if (num == 2)
            {
                return DateTime.Now.ToString("yyyy-MM");
            }
            return "";
        }
        private CheckBox chkAll;
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            if (STTemplateItemsSource != null)
            {
                CheckBox chk = sender as CheckBox;
                bool ischecked = chk.IsChecked.Value;

                foreach (STTemplate item in STTemplateItemsSource)
                {
                    item.IsChecked = ischecked;
                }
            }
        }
        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (this.STTemplateItemsSource != null)
            {
                chkAll.IsChecked = STTemplateItemsSource.Any(item => item.IsChecked);
            }
        }
        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            if (STTemplateItemsSource != null)
            {
                int checkcount = STTemplateItemsSource.Count(r => r.IsChecked == true);
                if (checkcount == STTemplateItemsSource.Length)
                {
                    chkAll.IsChecked = true;
                }
                else if (checkcount == 0)
                {
                    chkAll.IsChecked = false;
                }
                else
                {
                    chkAll.IsChecked = null;
                }
            }
        }

        private STTemplate[] GetSTTemplate()
        {
            //STTemplateItemsSource =
            List<STTemplate> list = new List<STTemplate>();
            BeOperationInvoker.Invoke<I_CO_IA_ActivitySummarize>(channel =>
            {
                list = channel.GetSummarizeTemplateAllName();
            });
            return list.ToArray();
        }
        public STTemplate[] STTemplateItemsSource
        {
            get { return (STTemplate[])GetValue(STTemplateItemsSourceProperty); }
            set { SetValue(STTemplateItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty STTemplateItemsSourceProperty =
    DependencyProperty.Register("STTemplateItemsSource", typeof(STTemplate[]), typeof(SummarizeTotalTemplate), new PropertyMetadata(null, null));
    }
}
