using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using System.Net;
using Microsoft.Win32;
using CO_IA.Data;
using CO_IA.Data.ActivitySummarize;
using AT_BC.Offices;


namespace CO_IA.UI.ActivitySummarize
{
    /// <summary>
    /// SummarizeDocItem.xaml 的交互逻辑
    /// </summary>
    public partial class SummarizeDocItem : UserControl
    {

        #region 常量
        string activityId = "";
        private CheckBox chkAll;
        SummarizeDoc currentsummarize = new SummarizeDoc();
        List<SummarizeDoc> summarizeList = new List<SummarizeDoc>();
        private int isAddOrUpdate = 0;//默认添加
        static string upfilestr = "";
        public SummarizeDoc SummarizeDocSelected
        {
            get
            {
                if (dgList.SelectedItem == null)
                {
                    return null;
                }
                else
                {
                    return dgList.SelectedItem as SummarizeDoc;
                }
            }
            set
            {
                dgList.SelectedItem = value;
            }
        }
        #endregion

        #region 构造函数
        public SummarizeDocItem()
        {
            //isAddOrUpdate = 0;
            InitializeComponent();
            BindDatGrid();
        }
        public SummarizeDocItem(int mode)
        {
            InitializeComponent();
            BindDatGrid();
            if (mode == 1)
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnDetel.Visibility = Visibility.Collapsed;
                btnTotal.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 绑定gird控件
        /// </summary>
        protected void BindDatGrid()
        {
            activityId = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            currentsummarize.Children = SummarizeHelpData.GetAllSummarizeDoc(activityId).ToList();
            dgList.ItemsSource = currentsummarize.Children;
            if (currentsummarize.Children.Count == 0)
            {
                //btnSave.IsEnabled = true;
                xpsDocViewr.Document = null;
            }
            //RuleName.Text = "";
            //Sender.Text = "";
            //dpSendDate.SelectedDate = Convert.ToDateTime(DateTime.Now);
            //tbSynopsis.Text = "";
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="fileNamePath">上传文件的文件名，全路径</param>
        /// <param name="IsAutoRename">是否自动按照时间重命名</param>
        public bool UpLoadFile(string fileNamePath, bool IsAutoRename)
        {
            string fileName = fileNamePath.Substring(fileNamePath.LastIndexOf("\\") + 1);

            string NewFileName = fileName;
            if (IsAutoRename)
            {
                NewFileName = DateTime.Now.ToString("yyMMddhhmmss") + DateTime.Now.Millisecond.ToString() + fileNamePath.Substring(fileNamePath.LastIndexOf("."));
            }
            string uriString = "./";
            string fileNameExt = fileName.Substring(fileName.LastIndexOf(".") + 1);
            if (uriString.EndsWith("/") == false) uriString = uriString + "/";

            uriString = uriString + NewFileName;

            ///  创建WebClient实例 
            WebClient myWebClient = new WebClient();
            myWebClient.Credentials = CredentialCache.DefaultCredentials;

            try
            {
                // 要上传的文件 
                FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(fs);
                byte[] postArray = r.ReadBytes((int)fs.Length);
                Stream postStream = myWebClient.OpenWrite(uriString, "PUT");

                try
                {
                    // 使用UploadFile方法可以用下面的格式
                    // myWebClient.UploadFile(uriString,"PUT",fileNamePath);

                    if (postStream.CanWrite)
                    {
                        postStream.Write(postArray, 0, postArray.Length);
                        postStream.Close();
                        fs.Dispose();
                    }
                    else
                    {
                        postStream.Close();
                        fs.Dispose();
                    }
                    return true;
                }
                catch (Exception err)
                {
                    postStream.Close();
                    fs.Dispose();
                    return false;
                }
                finally
                {
                    postStream.Close();
                    fs.Dispose();
                }
            }
            catch
            {
                return false;
            }
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
            SereverAndEnclousePath = System.IO.Path.Combine(path,fileName);
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
                        //将文件以二进制形式存储到数据库中
                        currentsummarize.FILEPATH = btArray;
                        currentsummarize.FILENAME = fileName.Substring(0, fileName.LastIndexOf(".")).ToString();
                        //显示文件格式
                        currentsummarize.FILEFORM = fileName;
                        currentsummarize.FILESIZE = (fs.Length).ToString();
                        currentsummarize.FILETYPE = "";
                        currentsummarize.FILEDOC = btArray;
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
        private void getFile(byte[] content, string filePath)
        {
            string fileName = filePath;
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
            fs.Write(content, 0, content.Length);
            fs.Flush();
            fs.Close();
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
        #endregion

        #region 事件
        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            isAddOrUpdate = 0;//上传即新建
            upfilestr = "";
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;//检查文件是否存在
            dlg.Multiselect = false;//是否允许多选，false表示单选
            dlg.CheckPathExists = true;
            //只限制上传word和excel
            dlg.Filter = "Office Files|*.doc;*.docx;*.xls;*.xlsx";
            if ((bool)dlg.ShowDialog())
            {
                string filePath = dlg.FileName;
                upfilestr = dlg.FileName;

                if (UpLodeEnclouseFolder(filePath) == true)
                //if (UpLoadFile(filePath,true) == true)
                {
                    //MessageBox.Show("文件上传成功！");

                    //新增
                    currentsummarize.GUID = CO_IA.Client.Utility.NewGuid();
                    currentsummarize.ACTIVITY_GUID = activityId;//活动id
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_SummarizeDoc>
                           (channel =>
                           {
                               bool monitorResult = channel.SaveSummarizeDoc(currentsummarize);
                               if (monitorResult == true)
                               {
                                   MessageBox.Show("保存成功！");
                                   BindDatGrid();
                               }
                               else
                               {
                                   MessageBox.Show("保存失败!");
                               }
                           });
                    //btnSave.IsEnabled = true;

                    //string xpsFilePath = filePath.Substring(0, filePath.LastIndexOf(".")).ToString() + ".xps";
                    string xpsFilePath = GetPath() + filePath.Substring(filePath.LastIndexOf('\\') + 1, filePath.LastIndexOf(".") - filePath.LastIndexOf('\\') - 1).ToString() + ".xps";

                    try
                    {
                        if (OfficeConverter.IsValidOfficeFile(filePath))
                        {
                            OfficeConverter.ConvertToXps(filePath, xpsFilePath);
                            using (XpsDocument xpsDoc = new XpsDocument(xpsFilePath, FileAccess.Read))
                            {
                                var fsxps = xpsDoc.GetFixedDocumentSequence();
                                xpsDocViewr.Document = fsxps;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.GetExceptionMessage());
                    }
                    //var convertResults = OfficeToXps.ConvertToXps(filePath, ref xpsFilePath);
                    //switch (convertResults.Result)
                    //{
                    //    case ConversionResult.OK:
                    //        try
                    //        {
                    //            using (XpsDocument xpsDoc = new XpsDocument(xpsFilePath, FileAccess.Read))
                    //            {
                    //                var fsxps = xpsDoc.GetFixedDocumentSequence();
                    //                xpsDocViewr.Document = fsxps;
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {

                    //        }
                    //        break;
                    //    case ConversionResult.InvalidFilePath:
                    //        // 处理文件路径错误或文件不存在
                    //        break;
                    //    case ConversionResult.UnexpectedError:

                    //        break;
                    //    case ConversionResult.ErrorUnableToInitializeOfficeApp:
                    //        MessageBox.Show("未安装打印机或未启动打印机服务，请重新安装打印机或重启机器后尝试。", "消息提示", MessageBoxButton.OK);
                    //        // Office 未安装会出现这个异常
                    //        break;
                    //    case ConversionResult.ErrorUnableToOpenOfficeFile:
                    //        // 文件被占用会出现这个异常
                    //        break;
                    //    case ConversionResult.ErrorUnableToAccessOfficeInterop:
                    //        // Office 未安装会出现这个异常
                    //        break;
                    //    case ConversionResult.ErrorUnableToExportToXps:
                    //        // 微软 OFFICE Save As PDF 或 XPS  插件未安装异常
                    //        break;
                    //}
                }
                else
                {
                    MessageBox.Show("请选择附件！");
                }
            }
        }

        /// <summary>
        /// 初始加载全选内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (dgList.ItemsSource != null)
            {
                chkAll.IsChecked = (dgList.ItemsSource as List<SummarizeDoc>).Any(item => item.IsChecked);
            }
        }
        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;
            if (ischecked == true)
            {
                //btnSave.IsEnabled = false;//不可用
                foreach (SummarizeDoc item in dgList.ItemsSource)
                {
                    item.IsChecked = ischecked;
                    summarizeList.Add(item);
                }
            }
            else
            {
                //btnSave.IsEnabled = true;//可用
                foreach (SummarizeDoc item in dgList.ItemsSource)
                {
                    item.IsChecked = ischecked;
                    summarizeList.Remove(item);
                }
            }
            
        }
        /// <summary>
        /// datagrid数据选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;
            bool? isChecked = box.IsChecked;
            if (!isChecked.HasValue)
            {
                return;
            }
            // SummarizeDoc freq = box.DataContext as SummarizeDoc;
            currentsummarize = box.DataContext as SummarizeDoc;
            bool checkedState = isChecked.Value;
            currentsummarize.IsChecked = isChecked.Value;
            if (currentsummarize.IsChecked == true)
            {
                summarizeList.Add(currentsummarize);
               
            }
            else
            {
                summarizeList.Remove(currentsummarize);
            }

            #region 判断全选和保存状态
            int ischeckCount = (dgList.ItemsSource as List<SummarizeDoc>).Count(p => p.IsChecked == true);
            if (ischeckCount == 1 || ischeckCount == 0)
            {
                if (ischeckCount == 1)
                {
                    SummarizeDocSelected = (dgList.ItemsSource as List<SummarizeDoc>).Where(p => p.IsChecked == true).FirstOrDefault();
                    //RuleName.Text = SummarizeDocSelected.SUMMARIZENAME;
                    //Sender.Text = SummarizeDocSelected.SENDPERSON;
                    //dpSendDate.SelectedDate = Convert.ToDateTime(SummarizeDocSelected.SENDDATE);
                    //tbSynopsis.Text = SummarizeDocSelected.SUMMARY;
                }
                //btnSave.IsEnabled = true;
                isAddOrUpdate = 1;//可以进行修改操作
            }
            else
            {
                //RuleName.Text = "";
                //Sender.Text = "";
                //dpSendDate.SelectedDate = Convert.ToDateTime(DateTime.Now);
                //tbSynopsis.Text = "";
                //btnSave.IsEnabled = false;
                isAddOrUpdate = 0;
            }
            foreach (SummarizeDoc result in dgList.ItemsSource)
            {
                if (result.IsChecked != checkedState)
                {
                    this.chkAll.IsChecked = null;
                    return;
                }

            }
            chkAll.IsChecked = checkedState;
            #endregion

        }

        private void FreqInfoGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {

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
                SummarizeDoc summarize = (sender as System.Windows.Documents.Hyperlink).Tag as SummarizeDoc;
                if (summarize == null)
                    return;
                string docstr = summarize.FILEFORM.Substring(summarize.FILEFORM.LastIndexOf('.'));
                string serviceDocPath = ByteConvertDocService(summarize.FILEPATH, summarize.FILEFORM);
                WebClient webClient = new WebClient();
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "活动文件下载";
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
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string errorMsg = string.Empty;
                //if (dpSendDate.SelectedDate == null)
                //{
                //    errorMsg += "发布日期,";
                //}
                //if (RuleName.Text == "")
                //{
                //    errorMsg += "活动名称,";
                //}
                //if (Sender.Text == "")
                //{
                //    errorMsg += "发布人,";
                //}
                //if (!string.IsNullOrEmpty(errorMsg))
                //{
                //    MessageBox.Show(errorMsg + "不能为空");
                //    return;
                //}
                //currentsummarize.SUMMARIZENAME = RuleName.Text;//活动名称
                //currentsummarize.SENDPERSON = Sender.Text;//发布人
                //currentsummarize.SENDDATE = Convert.ToDateTime(dpSendDate.SelectedDate);
                //currentsummarize.SUMMARY = tbSynopsis.Text;//简介
                if (isAddOrUpdate == 0)
                {
                    if (string.IsNullOrEmpty(currentsummarize.FILENAME))
                    {
                        MessageBox.Show("请上传总结文件！");
                        return;
                    }
                    //新增
                    currentsummarize.GUID =CO_IA.Client.Utility.NewGuid();
                    currentsummarize.ACTIVITY_GUID = activityId;//活动id
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_SummarizeDoc>
                           (channel =>
                           {
                               bool monitorResult = channel.SaveSummarizeDoc(currentsummarize);
                               if (monitorResult == true)
                               {
                                   MessageBox.Show("保存成功！");
                                   BindDatGrid();
                               }
                               else
                               {
                                   MessageBox.Show("保存失败!");
                               }
                           });
                }
                else
                {
                    //修改
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_SummarizeDoc>
                           (channel =>
                           {
                               bool monitorResult = channel.UpdateSummarizeDoc(SummarizeDocSelected);
                               if (monitorResult == true)
                               {
                                   MessageBox.Show("修改成功！");
                                   BindDatGrid();
                                   summarizeList.Clear();
                               }
                               else
                               {
                                   MessageBox.Show("修改失败!");
                               }
                           });

                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.GetExceptionMessage());
            }

        }
        /// <summary>
        /// 行选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

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
        /// 删除操作（支持批量删除）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isdeleteOk = true;
                List<SummarizeDoc> checkFreq = (dgList.ItemsSource as List<SummarizeDoc>).Where(p => p.IsChecked == true).ToList();
                if (checkFreq.Count == 0)
                {
                    MessageBox.Show("请选择要删除的数据！"); return;
                }
                if (checkFreq.Count() > 0)
                {
                    foreach (SummarizeDoc item in checkFreq)
                    {
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_SummarizeDoc>
                        (channel =>
                        {
                            bool monitorResult = channel.DeleteSummarizeDoc(item.GUID);
                            if (monitorResult == false)
                            {
                                isdeleteOk = false;
                            }
                            else
                            {
                                xpsDocViewr.Document = null;
                                //删除客户端缓存的附件
                                string savePath = GetPath() + item.FILEFORM.ToString();
                                string xpsPath = savePath.Substring(0, savePath.LastIndexOf(".")).ToString() + ".xps";
                                if (System.IO.File.Exists(savePath))
                                {
                                    System.IO.File.Delete(savePath);
                                    System.IO.File.Delete(xpsPath);
                                }
                            }
                        });
                        if (isdeleteOk == false)
                            break;
                    }
                    if (isdeleteOk == true)
                    {
                        isAddOrUpdate = 0;
                        //summarizeList.Clear();
                        BindDatGrid();
                    }
                    else
                    {
                        //summarizeList.Clear();
                        MessageBox.Show("删除失败!", "提示", MessageBoxButton.OK);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        private void btnTotal_Click(object sender, RoutedEventArgs e)
        {
            CO_IA.UI.ActivitySummarize.SummarizeTotal st = new CO_IA.UI.ActivitySummarize.SummarizeTotal();
            st.ShowDialog();
            BindDatGrid();
        }

        private void dgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            #region 加载基本信息
            if (SummarizeDocSelected == null)
                return;//防止单击空白或标题等触发该事件
            currentsummarize = dgList.SelectedItem as SummarizeDoc;
            isAddOrUpdate = 1;//可以进行修改操作
            //if (currentsummarize.IsChecked == true)
            //{
            //    currentsummarize.IsChecked = false;
            //    summarizeList.Remove(currentsummarize);
            //}
            //else
            //{
            //    currentsummarize.IsChecked = true;
            //    summarizeList.Add(currentsummarize);
            //}
            bool checkedState = currentsummarize.IsChecked;

            #endregion

            #region //预览文件
            //
            string isxpsPath = GetPath() + currentsummarize.FILEFORM.Substring(0, currentsummarize.FILEFORM.LastIndexOf(".")).ToString() + ".xps";
            if (System.IO.File.Exists(isxpsPath))
            {
                //已经存xps文件，直接加载
                using (XpsDocument xpsDoc = new XpsDocument(isxpsPath, FileAccess.Read))
                {
                    var fsxps = xpsDoc.GetFixedDocumentSequence();
                    xpsDocViewr.Document = fsxps;
                }
            }
            else
            {
                //创建xps文件，启动进程耗时
                string serviceDocPathXPS = ByteConvertDocService(currentsummarize.FILEPATH, currentsummarize.FILEFORM);
                string xpsFilePath = serviceDocPathXPS.Substring(0, serviceDocPathXPS.LastIndexOf(".")).ToString() + ".xps";

                try
                {
                    if (OfficeConverter.IsValidOfficeFile(serviceDocPathXPS))
                    {
                        OfficeConverter.ConvertToXps(serviceDocPathXPS, xpsFilePath);
                        using (XpsDocument xpsDoc = new XpsDocument(xpsFilePath, FileAccess.Read))
                        {
                            var fsxps = xpsDoc.GetFixedDocumentSequence();
                            xpsDocViewr.Document = fsxps;
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage());
                }

                //var convertResults = OfficeToXps.ConvertToXps(serviceDocPathXPS, ref xpsFilePath);
                //switch (convertResults.Result)
                //{
                //    case ConversionResult.OK:
                //        try
                //        {
                //            using (XpsDocument xpsDoc = new XpsDocument(xpsFilePath, FileAccess.Read))
                //            {
                //                var fsxps = xpsDoc.GetFixedDocumentSequence();
                //                xpsDocViewr.Document = fsxps;
                //            }
                //        }
                //        catch (Exception ex)
                //        {

                //        }
                //        break;
                //    case ConversionResult.InvalidFilePath:
                //        // 处理文件路径错误或文件不存在
                //        break;
                //    case ConversionResult.UnexpectedError:

                //        break;
                //    case ConversionResult.ErrorUnableToInitializeOfficeApp:
                //        // Office 未安装会出现这个异常
                //        break;
                //    case ConversionResult.ErrorUnableToOpenOfficeFile:
                //        // 文件被占用会出现这个异常
                //        break;
                //    case ConversionResult.ErrorUnableToAccessOfficeInterop:
                //        // Office 未安装会出现这个异常
                //        break;
                //    case ConversionResult.ErrorUnableToExportToXps:
                //        // 微软 OFFICE Save As PDF 或 XPS  插件未安装异常
                //        break;
                //}
            }
            #endregion

            #region 判断全选和保存状态
            int ischeckCount = (dgList.ItemsSource as List<SummarizeDoc>).Count(p => p.IsChecked == true);
            if (ischeckCount == 1 || ischeckCount == 0)
            {
                //if (ischeckCount == 1)
                //{
                //    //SummarizeDocSelected = (dgList.ItemsSource as List<SummarizeDoc>).Where(p => p.IsChecked == true).FirstOrDefault();

                //    //RuleName.Text = SummarizeDocSelected.SUMMARIZENAME;
                //    //Sender.Text = SummarizeDocSelected.SENDPERSON;
                //    //dpSendDate.SelectedDate = Convert.ToDateTime(SummarizeDocSelected.SENDDATE);
                //    //tbSynopsis.Text = SummarizeDocSelected.SUMMARY;
                //}
                //btnSave.IsEnabled = true;
                isAddOrUpdate = 1;//可以进行修改操作
            }
            else
            {
                //RuleName.Text = "";
                //Sender.Text = "";
                //dpSendDate.SelectedDate = Convert.ToDateTime(DateTime.Now);
                //tbSynopsis.Text = "";
                //btnSave.IsEnabled = false;
                isAddOrUpdate = 0;
            }
            foreach (SummarizeDoc result in dgList.ItemsSource)
            {
                if (result.IsChecked != checkedState)
                {
                    this.chkAll.IsChecked = null;
                    return;
                }

            }
            chkAll.IsChecked = checkedState;
            #endregion
        }

    }
}