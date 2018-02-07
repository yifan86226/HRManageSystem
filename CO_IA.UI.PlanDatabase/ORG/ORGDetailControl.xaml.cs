using CO_IA.Data;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CO_IA.Client;
using AT_BC.Common;
namespace CO_IA.UI.PlanDatabase.ORG
{
    /// <summary>
    /// CompanyDetailControl.xaml 的交互逻辑
    /// </summary>
    public partial class ORGDetailControl : UserControl
    {

        public event Action BeginAddEvent;

        public event Action AfterSaveEvent;

        public event Action CancelEvent;

        private bool IsAdd = false;

        private ORGInfo BackUpORG
        {
            get;
            set;
        }

        public ORGInfo ORGDataContext
        {
            get { return (ORGInfo)GetValue(ORGDataContextProperty); }
            set { SetValue(ORGDataContextProperty, value); }
        }

        public static readonly DependencyProperty ORGDataContextProperty =
            DependencyProperty.Register("ORGDataContext", typeof(ORGInfo), typeof(ORGDetailControl), new PropertyMetadata(new PropertyChangedCallback(ORGDataContextPropertyChanged)));

        private static void ORGDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //(d as ORGDetailControl).BackUpORG = SettingHelper.Clone<ORGInfo>(e.NewValue as ORGInfo);
        }

        public ORGDetailControl()
        {
            InitializeComponent();
            this.DataContext = this;
          
            InitData();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            IsAdd = true;
            this.ORGDataContext = new ORGInfo();
            FrozenControls(true);
            if (BeginAddEvent != null)
            {
                BeginAddEvent();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //如果是修改单位信息
            if (!IsAdd)
            {
                MessageBoxResult result = MessageBox.Show("确认要对选择的单位信息进行修改？", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    SaveORG();
                }
            }
            else //新增
            {
                SaveORG();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确认要删除选项的单位", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
                {
                    try
                    {
                        //string strres = channel.DeleteORGInfo(ORGDataContext.Guid);
                        //if (string.IsNullOrEmpty(strres))
                        //{
                        //    MessageBox.Show("删除成功", "提示");
                        //    if (AfterSaveEvent != null)
                        //    {
                        //        AfterSaveEvent();
                        //    }
                        //}
                        //else
                        //{
                        //    MessageBox.Show(strres, "提示");
                        //}
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.GetExceptionMessage());
                        throw;
                    }
                });
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            FrozenControls(false);
            if (CancelEvent != null)
            {
                CancelEvent();
            }
            IsAdd = false;
        }

        private void InitData()
        {
            combClass.ItemsSource = CO_IA.Client.Utility.GetSecurityClasses();
        }

        private void SaveORG()
        {
            ORGInfo orginfo = ORGDataContext;
            orginfo.Name = this.txtName.Text;
            if (Validate(orginfo))
            {
                //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
                //{
                //    channel.SaveORGInfo(ORGDataContext);
                //    MessageBox.Show("保存成功!", "提示", MessageBoxButton.OK);
                //    FrozenControls(false);
                //    if (AfterSaveEvent != null)
                //    {
                //        AfterSaveEvent();
                //    }
                //});
            }
        }

        private void FrozenControls(bool state)
        {
            btnAdd.IsEnabled = !state;
            btnDelete.IsEnabled = !state;
        }

        private void Reset()
        {
            this.ORGDataContext = BackUpORG;
            //this.txtName.Text = null;
            //this.txtShortName.Text = null;
            //this.txtAddress.Text = null;
            //this.combClass.SelectedValue = null;
            //this.txtContact.Text = null;
            //this.txtPhone.Text = null;
        }

        private bool Validate(ORGInfo orginfo)
        {
            bool issuccess = true;

            StringBuilder strmsg = new StringBuilder();

            if (string.IsNullOrEmpty(orginfo.Name))
            {
                issuccess = false;
                strmsg.Append("单位名称不能为空! \r");
            }
            if (string.IsNullOrEmpty(orginfo.ShortName))
            {
                issuccess = false;
                strmsg.Append("单位简称不能为空! \r");
            }
            if (string.IsNullOrEmpty(orginfo.Address))
            {
                issuccess = false;
                strmsg.Append("单位地址不能为空! \r");
            }
            if (string.IsNullOrEmpty(orginfo.Contact))
            {
                issuccess = false;
                strmsg.Append("联系人不能为空! \r");
            }
            if (string.IsNullOrEmpty(orginfo.Phone))
            {
                issuccess = false;
                strmsg.Append("联系电话不能为空! \r");
            }
            if (!string.IsNullOrEmpty(strmsg.ToString()))
            {
                MessageBox.Show(strmsg.ToString(), "提示", MessageBoxButton.OK);
            }
            return issuccess;
        }

        private void txtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //bool isNumberic = SettingHelper.IsNumberic(e.Text);

            //if (!isNumberic)
            //{
            //    e.Handled = true;
            //}
            //else
            //    e.Handled = false;
        }
    }
}
