using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Microsoft.Win32;
using Office_To_XPS;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Net;
using CO_IA.Data.TaskManage;
using CO_IA.Data.FileManage;
namespace CO.IA.UI.TaskManage.Rules
{
   
    /// <summary>
    /// RulesAndRegulationsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class RulesAndRegulationsDialog : Window
    {
        #region 常量
        string activityId = "";
       RegulationsInfo currentRegulate = new RegulationsInfo();
        private CheckBox chkAll;
        RuleFile rulefile = new RuleFile();
        List<RuleFile> currentFileList = new List<RuleFile>();
        private int isAddOrUpdate = 0;//默认添加
        static string upfilestr = "";
        private int checkresult = 0;//发布状态，0草拟，1审阅，2发布
        public event Action AfterSaveEvent;
        public RuleFile RuleSelected
        {
            get
            {
                if (dgList.SelectedItem == null)
                {
                    return null;
                }
                else
                {
                    return dgList.SelectedItem as RuleFile;
                }
            }
            set
            {
                dgList.SelectedItem = value;
            }
        }
        #endregion

        #region 默认构造函数
        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="activityid"></param>
        public RulesAndRegulationsDialog(string activityid)
        {
            InitializeComponent();
            activityId = activityid;
            dpDraft.SelectedDate = Convert.ToDateTime(DateTime.Now);
            dpAuditing.SelectedDate = Convert.ToDateTime(DateTime.Now); 
            dpSend.SelectedDate = Convert.ToDateTime(DateTime.Now); ;
        }
        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="regulate"></param>
        public RulesAndRegulationsDialog(RegulationsInfo regulate)
        {
            InitializeComponent();
            isAddOrUpdate = 1;
            currentRegulate = regulate;
            dpDraft.SelectedDate = regulate.DRAFTDATE;
            dpAuditing.SelectedDate = regulate.AUDITINGDATE;
            dpSend.SelectedDate = regulate.ISSUEDATE; 
            BindDatGrid();
            this.DataContext = regulate;//绑定前台变量
           
        }
        public string WindowTitle
        {
            set { this.Title = value; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始加载附件列表
        /// </summary>
        protected void BindDatGrid()
        {
            //currentFileList.Clear();
            RuleFile[] getRuleFiles = FileManageHelper.GetRuleFiles(currentRegulate.GUID);
            //if (getRuleFiles != null && getRuleFiles.Count() > 0)
            //{
            //    currentFileList = getRuleFiles.ToList();
            //}
            dgList.ItemsSource = getRuleFiles.ToList();
        }
        private bool ReValidate(RegulationsInfo regulate)
        {
            bool result = true;
            StringBuilder errmsg = new StringBuilder();
            if (string.IsNullOrEmpty(regulate.RULESNAME))
            {
                errmsg.Append("名称不能为空! \r");
                result = false;
            }
            if (string.IsNullOrEmpty(regulate.DRAFTPERSON))
            {
                errmsg.Append("起草人不能为空! \r");
                result = false;
            }
            if (string.IsNullOrEmpty(regulate.AUDITINGPERSON))
            {
                errmsg.Append("审核人不能为空! \r");
                result = false;
            }
            if (string.IsNullOrEmpty(regulate.ISSUEPERSON))
            {
                errmsg.Append("发布人不能为空! \r");
                result = false;

            }
            if (!string.IsNullOrEmpty(errmsg.ToString()))
            {
                MessageBox.Show(errmsg.ToString(), "验证失败", MessageBoxButton.OKCancel);
            }
            return result;
        }
        #endregion

        #region 事件

        /// <summary>
        /// 保存操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //if (currentFileList == null || currentFileList.Count() == 0)
            //{
            //    MessageBox.Show("请上传附件");
            //    return;
            //}
            RegulationsInfo regulate = new RegulationsInfo();
            if (isAddOrUpdate == 0)
            {
                //新增
                regulate.GUID =CO_IA.Client.Utility.NewGuid();
                regulate.ACTIVITY_GUID = activityId;//活动id
            }
            else
            {
                //修改
                regulate.GUID = currentRegulate.GUID.ToString();
                regulate.ACTIVITY_GUID = currentRegulate.ACTIVITY_GUID;//活动id
            }
            regulate.RULESNAME = RuleName.Text;//名称
            regulate.SENDSTATE = checkresult;//发布状态

            regulate.DRAFTPERSON = drawupPerson.Text;//起草人
            if (dpDraft.SelectedDate != null)
            {
                regulate.DRAFTDATE = Convert.ToDateTime(dpDraft.SelectedDate);//起草时间
            }
            regulate.AUDITINGPERSON = Auditing.Text;//审核人
            if (dpAuditing.SelectedDate != null)
            {
                regulate.AUDITINGDATE = Convert.ToDateTime(dpAuditing.SelectedDate);//审核时间
            }
            regulate.ISSUEPERSON = Sender.Text;//发布人
            regulate.ISSUEDATE = Convert.ToDateTime(dpSend.SelectedDate);//发布时间

            regulate.SUMMARY = tbSynopsis.Text;//简介
            regulate.RULETYPER = 0;//规章制度

            regulate.SENDSTATE = checkresult;//发布状态
            regulate.RULESNAME = RuleName.Text;//名称
            regulate.SENDSTATE = checkresult;//发布状态
            regulate.RuleFiles = currentFileList;//附件列表
            currentRegulate = regulate;
            if (!ReValidate(currentRegulate)) return;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.RuleAndFileManage.I_CO_IA_RuleAndFile>
                 (channel =>
                 {
                     try
                     {
                         if (isAddOrUpdate == 0)
                         {
                             bool isResult = channel.SaveRegulationsInfo(currentRegulate);
                             if (isResult == true)
                             {
                                 if (dgList.ItemsSource != null)
                                 {
                                     foreach (RuleFile rulefile in dgList.ItemsSource)
                                     {
                                         rulefile.GUID =CO_IA.Client.Utility.NewGuid();
                                         rulefile.MAINGUID = currentRegulate.GUID;
                                         bool isaddfile = channel.SaveRulesFile(rulefile);//添加附件
                                         if (isaddfile == false)
                                         {
                                             //删除客户端缓存的附件
                                             string savePath = FileManageHelper.GetPath() + rulefile.FILEFORM.ToString();
                                             string xpsPath = savePath.Substring(0, savePath.LastIndexOf(".")).ToString() + ".xps";
                                             if (System.IO.File.Exists(savePath))
                                             {
                                                 System.IO.File.Delete(savePath);
                                                 System.IO.File.Delete(xpsPath);
                                             }
                                         }
                                     }
                                 }
                             }
                         }
                         else
                         {
                             bool isupdateResutl = channel.UpdateRegulationsInfo(currentRegulate);
                             if (isupdateResutl == true)
                             {
                                 RuleFile[] rulefiles = FileManageHelper.GetRuleFiles(currentRegulate.GUID);
                                 //删除附件
                                 if (rulefiles != null && rulefiles.Count() > 0)
                                 {
                                     //List<RulesFiile> OriginalrulefileList = rulefiles.Where(r => r.MAINGUID == currentRegulate.GUID).ToList();
                                     bool deleteServerFils = channel.DeleteRulesFileMainID(currentRegulate.GUID);
                                     if (deleteServerFils == true)
                                     {
                                         foreach (RuleFile file in rulefiles)
                                         {
                                             //删除客户端缓存的附件
                                             string savePath = FileManageHelper.GetPath() + file.FILEFORM.ToString();
                                             string xpsPath = savePath.Substring(0, savePath.LastIndexOf(".")).ToString() + ".xps";
                                             if (System.IO.File.Exists(savePath))
                                             {
                                                 System.IO.File.Delete(savePath);
                                                 System.IO.File.Delete(xpsPath);
                                             }

                                         }
                                     }

                                 }
                                 if (dgList.ItemsSource != null)
                                 {
                                     foreach (RuleFile rulefile in dgList.ItemsSource)
                                     {
                                         rulefile.GUID =CO_IA.Client.Utility.NewGuid();
                                         rulefile.MAINGUID = currentRegulate.GUID;
                                         channel.SaveRulesFile(rulefile);//添加附件
                                     }
                                 }
                                 MessageBox.Show("修改成功!", "提示", MessageBoxButton.OK);
                             }
                         }
                     }
                     catch (Exception ex)
                     {
                         //MessageBox.Show(ex.GetExceptionMessage());
                     }
                 });

            if (AfterSaveEvent != null)
            {
                AfterSaveEvent();
            }
            this.Close();
        }

        #endregion

        #region 附件相关
        /// <summary>
        /// 附件列表删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确认要删除选中项？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                List<RuleFile> newFileList = new List<RuleFile>();
                List<RuleFile> ruleList = dgList.ItemsSource as List<RuleFile>;
                if (ruleList != null)
                {
                    var queryList = ruleList.Where(p => p.IsChecked == false);
                    newFileList = queryList.ToList();
                }
                dgList.ItemsSource = null;
                dgList.ItemsSource = newFileList;
                btnSave.IsEnabled = true;
                if (newFileList == null || newFileList.Count() == 0)
                {
                    xpsDocViewr.Document = null;
                }
            }
        }
        /// <summary>
        /// 附件上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
           // isAddOrUpdate = 0;//上传即新建
            RuleFile[] getRuleFiles = FileManageHelper.GetRuleFiles(currentRegulate.GUID);
            if (getRuleFiles != null && getRuleFiles.Count() > 0)
            {
                currentFileList = getRuleFiles.ToList();
            }
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
                string xpsFilePath = filePath.Substring(0, filePath.LastIndexOf(".")).ToString() + ".xps";
                var convertResults = OfficeToXps.ConvertToXps(filePath, ref xpsFilePath);
                switch (convertResults.Result)
                {
                    case ConversionResult.OK:
                        try
                        {
                            using (XpsDocument xpsDoc = new XpsDocument(xpsFilePath, FileAccess.Read))
                            {
                                var fsxps = xpsDoc.GetFixedDocumentSequence();
                                xpsDocViewr.Document = fsxps;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        break;
                    case ConversionResult.InvalidFilePath:
                        // 处理文件路径错误或文件不存在
                        break;
                    case ConversionResult.UnexpectedError:

                        break;
                    case ConversionResult.ErrorUnableToInitializeOfficeApp:
                        // Office 未安装会出现这个异常
                        break;
                    case ConversionResult.ErrorUnableToOpenOfficeFile:
                        // 文件被占用会出现这个异常
                        break;
                    case ConversionResult.ErrorUnableToAccessOfficeInterop:
                        // Office 未安装会出现这个异常
                        break;
                    case ConversionResult.ErrorUnableToExportToXps:
                        // 微软 OFFICE Save As PDF 或 XPS  插件未安装异常
                        break;
                }

                if (UpLodeEnclouseFolder(filePath) == true)
                {
                    MessageBox.Show("文件上传成功！");
                    dgList.ItemsSource = null;
                    dgList.ItemsSource = currentFileList;
                    btnSave.IsEnabled = true;

                }
                else
                {
                    MessageBox.Show("请选择附件！");
                }
            }
        }
        /// <summary>
        /// 上传到服务器指定目录
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
            //string getPath = FileManageHelper.GetPath();
            //SereverAndEnclousePath = getPath + fileName;
            SereverAndEnclousePath = System.IO.Path.Combine(FileManageHelper.GetPath(), fileName);
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
                        rulefile = new RuleFile();
                        //将文件以二进制形式存储到数据库中
                        rulefile.FILEPATH = btArray;
                        rulefile.FILENAME = fileName.Substring(0, fileName.LastIndexOf(".")).ToString();
                        //显示文件格式
                        rulefile.FILEFORM = fileName;
                        rulefile.FILESIZE = (fs.Length).ToString();
                        rulefile.FILETYPE = "制度管理";
                        //rulefile.MAINGUID = currentRegulate.GUID;//存外键
                        currentFileList.Add(rulefile);
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
        /// 下载文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadFile_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                RuleFile summarize = (sender as System.Windows.Documents.Hyperlink).Tag as RuleFile;
                if (summarize == null)
                    return;
                string docstr = summarize.FILEFORM.Substring(summarize.FILEFORM.LastIndexOf('.'));
                string serviceDocPath = FileManageHelper.ByteConvertDocService(summarize.FILEPATH, summarize.FILEFORM);
                WebClient webClient = new WebClient();
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = "文件下载";
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
        /// 全选附件列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;
            currentFileList.Clear();
            if (ischecked == true)
            {
                btnSave.IsEnabled = false;//不可用
                foreach (RuleFile item in dgList.ItemsSource)
                {
                    item.IsChecked = ischecked;
                    currentFileList.Add(item);
                }
            }
            else
            {
                btnSave.IsEnabled = true;//可用
                foreach (RuleFile item in dgList.ItemsSource)
                {
                    item.IsChecked = ischecked;
                    currentFileList.Remove(item);
                }
            }
        }
        /// <summary>
        /// 初始加载附附列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (dgList.ItemsSource != null)
            {
                chkAll.IsChecked = (dgList.ItemsSource as List<RuleFile>).Any(item => item.IsChecked);
            }
        }
        /// <summary>
        /// 附件列表，单选
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
            rulefile = box.DataContext as RuleFile;
            bool checkedState = isChecked.Value;
            rulefile.IsChecked = isChecked.Value;
            if (rulefile.IsChecked == true)
            {
                currentFileList.Add(rulefile);
            }
            else
            {
                currentFileList.Remove(rulefile);
            }

            #region 判断全选和保存状态
            int ischeckCount = (dgList.ItemsSource as List<RuleFile>).Count(p => p.IsChecked == true);
            if (ischeckCount == 1 || ischeckCount == 0)
            {
                btnSave.IsEnabled = true;
            }
            else
            {
                btnSave.IsEnabled = false;
            }
            foreach (RuleFile result in dgList.ItemsSource)
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
        /// <summary>
        /// 发布状态，选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkResult(object sender, RoutedEventArgs e)
        {
            if (rbdrawup.IsChecked == true)
            {
                checkresult = 0;//草批
            }
            else if (rbcheck.IsChecked == true)
            {
                checkresult = 1;//审阅
            }
            else if (rbpublish.IsChecked == true)
            {
                checkresult = 2;//发布
            }
        }
        #endregion

        private void dgList_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            #region 加载基本信息
            rulefile = new RuleFile();
            rulefile = dgList.SelectedItem as RuleFile;
            if (rulefile == null)
                return;//防止单击空白或标题等触发该事件
            isAddOrUpdate = 1;//可以进行修改操作
            if (rulefile.IsChecked == true)
            {
                rulefile.IsChecked = false;
                currentFileList.Remove(rulefile);
            }
            else
            {
                rulefile.IsChecked = true;
                currentFileList.Add(rulefile);
            }
            bool checkedState = rulefile.IsChecked;

            #endregion

            #region //预览文件
            //
            string isxpsPath = FileManageHelper.GetPath() + rulefile.FILEFORM.Substring(0, rulefile.FILEFORM.LastIndexOf(".")).ToString() + ".xps";
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
                string serviceDocPathXPS = FileManageHelper.ByteConvertDocService(rulefile.FILEPATH, rulefile.FILEFORM);
                string xpsFilePath = serviceDocPathXPS.Substring(0, serviceDocPathXPS.LastIndexOf(".")).ToString() + ".xps";
                var convertResults = OfficeToXps.ConvertToXps(serviceDocPathXPS, ref xpsFilePath);
                switch (convertResults.Result)
                {
                    case ConversionResult.OK:
                        try
                        {
                            using (XpsDocument xpsDoc = new XpsDocument(xpsFilePath, FileAccess.Read))
                            {
                                var fsxps = xpsDoc.GetFixedDocumentSequence();
                                xpsDocViewr.Document = fsxps;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        break;
                    case ConversionResult.InvalidFilePath:
                        // 处理文件路径错误或文件不存在
                        break;
                    case ConversionResult.UnexpectedError:

                        break;
                    case ConversionResult.ErrorUnableToInitializeOfficeApp:
                        // Office 未安装会出现这个异常
                        break;
                    case ConversionResult.ErrorUnableToOpenOfficeFile:
                        // 文件被占用会出现这个异常
                        break;
                    case ConversionResult.ErrorUnableToAccessOfficeInterop:
                        // Office 未安装会出现这个异常
                        break;
                    case ConversionResult.ErrorUnableToExportToXps:
                        // 微软 OFFICE Save As PDF 或 XPS  插件未安装异常
                        break;
                }
            }
            #endregion

            #region 判断全选和保存状态
            int ischeckCount = (dgList.ItemsSource as List<RuleFile>).Count(p => p.IsChecked == true);
            if (ischeckCount == 1 || ischeckCount == 0)
            {
                btnSave.IsEnabled = true;
                isAddOrUpdate = 1;//可以进行修改操作
            }
            else
            {
                btnSave.IsEnabled = false;
                isAddOrUpdate = 0;
            }
            foreach (RuleFile result in dgList.ItemsSource)
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



