using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Net;
using CO_IA.Data.FileManage;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CO_IA.Types;
using AT_BC.Offices;
namespace CO_IA.UI.ActivityManage.FileManage
{

    /// <summary>
    /// RulesAndRegulationsDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WorkFileManage : Window, INotifyPropertyChanged
    {
        #region 常量

        private CheckBox chkAll;
        public event Action<WorkFileInfo> AfterSaveEvent;

        //当前操作的数据信息
        public WorkFileInfo CurrentFile
        {
            get;
            set;
        }

        /// <summary>
        /// 选中的附件
        /// </summary>
        private FileAttachment SelectFileAttachment
        {
            get { return dgList.SelectedItem as FileAttachment; }
        }

        /// <summary>
        /// 附件数据源
        /// </summary>
        private ObservableCollection<FileAttachment> AttachmentSources
        {
            get;
            set;
        }

        public string WindowTitle
        {
            set { this.Title = value; }
        }

        #endregion

        #region 默认构造函数

        public WorkFileManage(WorkFileInfo file)
        {
            InitializeComponent();
            this.DataContext = this;
            CurrentFile = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<WorkFileInfo>(file);
            AttachmentSources = this.QueryAttachments(CurrentFile.GUID);
            UpdateAttSource();
            this.Closed += WorkFileManage_Closed;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 初始加载附件列表
        /// </summary>
        protected ObservableCollection<FileAttachment> QueryAttachments(string attguid)
        {
            return FileManageHelper.QueryFileAttachments(attguid);
        }

        /// <summary>
        /// 更新附件数据源
        /// </summary>
        private void UpdateAttSource()
        {
            CurrentFile.Attachments = new List<FileAttachment>(AttachmentSources);
            dgList.ItemsSource = AttachmentSources.Where(r => r.DataState != DataStateEnum.Deleted);
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool FileValidate(WorkFileInfo file)
        {
            bool result = true;
            StringBuilder errmsg = new StringBuilder();
            if (string.IsNullOrEmpty(file.FileName))
            {
                errmsg.Append("名称不能为空! \r");
                result = false;
            }
            //if (string.IsNullOrEmpty(file.DrafTPerson))
            //{
            //    errmsg.Append("起草人不能为空! \r");
            //    result = false;
            //}
            //if (string.IsNullOrEmpty(file.AuditPerson))
            //{
            //    errmsg.Append("审核人不能为空! \r");
            //    result = false;
            //}
            if (string.IsNullOrEmpty(file.IssuePerson))
            {
                errmsg.Append("发布人不能为空! \r");
                result = false;
            }
            if (!string.IsNullOrEmpty(errmsg.ToString()))
            {
                MessageBox.Show(errmsg.ToString(), "验证失败");
            }
            return result;
        }

        /// <summary>
        /// 上传到服务器指定目录
        /// </summary>
        /// <param name="ServerPath"></param>
        /// <param name="EnclouseFolderPath"></param>
        /// <returns></returns>
        private void UplodeAttToService(string ServerPath)
        {
            //获取文件名
            string fileName = ServerPath.Substring(ServerPath.LastIndexOf("\\") + 1);
            string filePath = System.IO.Path.Combine(FileManageHelper.GetPath(), fileName);
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
                }
                else
                {
                    BinaryReader br = new BinaryReader(fs);
                    byte[] btArray = br.ReadBytes((int)fs.Length);
                    Stream uplodeStream = web.OpenWrite(filePath, "PUT");
                    if (uplodeStream.CanWrite)
                    {
                        uplodeStream.Write(btArray, 0, btArray.Length);
                        uplodeStream.Flush();
                        uplodeStream.Close();

                        FileAttachment att = new FileAttachment();
                        att.DataState = DataStateEnum.Added;
                        att.GUID = System.Guid.NewGuid().ToString();
                        att.FileGuid = CurrentFile.GUID;
                        att.AttName = fileName;
                        att.AttSize = (Int32)fs.Length;
                        att.AttContent = btArray;
                        att.UploadData = DateTime.Now.Date;

                        AttachmentSources.Add(att);
                        UpdateAttSource();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();//上传成功后，关闭流
                }
            }
        }

        /// <summary>
        ///删除客户端缓存的文件 
        /// </summary>
        /// <param name="file"></param>
        private void DeleteClientAtts(List<FileAttachment> attachments)
        {
            foreach (FileAttachment att in attachments)
            {
                string savePath = FileManageHelper.GetPath() + att.AttName;
                string xpsPath = savePath.Substring(0, savePath.LastIndexOf(".")).ToString() + ".xps";
                if (System.IO.File.Exists(savePath))
                {
                    System.IO.File.Delete(savePath);
                    System.IO.File.Delete(xpsPath);
                }
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 保存操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            WorkFileInfo file = this.CurrentFile;

            if (file != null)
            {
                if (!FileValidate(file)) return;

                string error = string.Empty;

                bool result = FileManageHelper.SaveWorkFiles(file, out error);
                if (result)
                {
                    this.Close();

                    if (file.Attachments != null || file.Attachments.Count > 0)
                    {
                        DeleteClientAtts(file.Attachments);
                    }

                    if (AfterSaveEvent != null)
                    {
                        AfterSaveEvent(CurrentFile);
                    }
                }
                else
                {
                    MessageBox.Show(error, "保存失败");
                }
            }
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 附件上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;//检查文件是否存在
            dlg.Multiselect = false;//是否允许多选，false表示单选
            dlg.CheckPathExists = true;
            //只限制上传word和excel
            dlg.Filter = "Office Files|*.doc;*.docx;*.xls;*.xlsx";
            if ((bool)dlg.ShowDialog())
            {
                string attpath = dlg.FileName;
                string attname = dlg.SafeFileName;

                string xpsname = attname.Substring(0, attname.LastIndexOf(".")) + ".xps";
                string xpspath = System.IO.Path.Combine(FileManageHelper.GetPath(), xpsname);

                try
                {
                    if (OfficeConverter.IsValidOfficeFile(attpath))
                    {
                        OfficeConverter.ConvertToXps(attpath, xpspath);
                        UplodeAttToService(attpath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage());
                }
            }
        }

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
                if (AttachmentSources != null || AttachmentSources.Count > 0)
                {
                    List<FileAttachment> delatt = AttachmentSources.Where(p => p.IsChecked == true).ToList();
                    foreach (FileAttachment att in delatt)
                    {
                        att.DataState = DataStateEnum.Deleted;
                    }
                    UpdateAttSource();
                    chkAll.IsChecked = false;
                }

                if (AttachmentSources == null || AttachmentSources.Count == 0)
                {
                    xpsDocViewr.Document = null;
                }
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileAttachment attachment = (sender as System.Windows.Documents.Hyperlink).DataContext as FileAttachment;
                if (attachment == null)
                    return;
                string docstr = attachment.AttName.Substring(attachment.AttName.LastIndexOf('.'));
                string serviceDocPath = FileManageHelper.ByteConvertDocService(attachment.AttContent, attachment.AttName);
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
                    catch (Exception ec)
                    {
                        MessageBox.Show(ec.GetExceptionMessage());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
            }
        }

        private void dgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectFileAttachment == null)
                return; //防止单击空白或标题等触发该事件

            string xpspath = FileManageHelper.GetPath() + SelectFileAttachment.AttName.Substring(0, SelectFileAttachment.AttName.LastIndexOf(".")) + ".xps";
            if (System.IO.File.Exists(xpspath))
            {
                //已经存xps文件，直接加载
                using (XpsDocument xpsDoc = new XpsDocument(xpspath, FileAccess.Read))
                {
                    var fsxps = xpsDoc.GetFixedDocumentSequence();
                    xpsDocViewr.Document = fsxps;
                }
            }
            else
            {
                string filepath = FileManageHelper.ByteConvertDocService(SelectFileAttachment.AttContent, SelectFileAttachment.AttName);
                string xpsfilepath = filepath.Substring(0, filepath.LastIndexOf(".")).ToString() + ".xps";

                try
                {
                    if (OfficeConverter.IsValidOfficeFile(filepath))
                    {
                        OfficeConverter.ConvertToXps(filepath, xpsfilepath);
                        using (XpsDocument xpsDoc = new XpsDocument(xpsfilepath, FileAccess.Read))
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

            if (AttachmentSources != null)
            {
                chkAll.IsChecked = (AttachmentSources).Any(item => item.IsChecked);
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
            foreach (FileAttachment att in AttachmentSources)
            {
                att.IsChecked = ischecked;
            }
            //foreach (RuleFile item in dgList.ItemsSource)
            //{
            //    item.IsChecked = ischecked;
            //}
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

            FileAttachment att = box.DataContext as FileAttachment;
            bool checkedState = isChecked.Value;
            att.IsChecked = isChecked.Value;

            int ischeckCount = AttachmentSources.Count(p => p.IsChecked == true);

            if (ischeckCount == AttachmentSources.Count)
            {
                chkAll.IsChecked = true;
            }
            else if (ischeckCount == 0)
            {
                chkAll.IsChecked = false;
            }
            else
            {
                this.chkAll.IsChecked = null;
            }
        }

        /// <summary>
        /// 窗口管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkFileManage_Closed(object sender, EventArgs e)
        {
            DeleteClientAtts(CurrentFile.Attachments);
        }

        #endregion

        #region INotifyPropertyChanged接口

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性变更方法
        /// </summary>
        /// <param name="propertyName">发生变更的属性名称</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}



