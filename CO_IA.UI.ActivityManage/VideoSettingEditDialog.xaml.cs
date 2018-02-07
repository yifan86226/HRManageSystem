using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO_IA.UI.ActivityManage
{
    /// <summary>
    /// ActivityManageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class VideoSettingEditDialog : Window
    {
        public VideoSettingEditDialog()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            this.SaveSetting();
            this.DialogResult = true;
        }

        private VideoSetting editVideoSetting;

        public void EditVideoSetting(VideoSetting setting, ActivityPlace[] places)
        {
            this.editVideoSetting = setting;
            this.comboBoxPlace.ItemsSource = places;
            this.textBoxName.Text = setting.Name;
            this.comboBoxPlace.SelectedValue = setting.OwnerGuid;
            this.textBoxIP.Text = setting.IP;
            this.textBoxPort.Text = setting.Port.ToString();
            this.textBoxUserName.Text = setting.UserName;
            this.passwordBox.Password = setting.Password;
        }

        private bool ValidIP(string ipAddress)
        {
            return Regex.IsMatch(ipAddress, @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])(\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])){3}$");
        }

        private void SaveSetting()
        {
            if (string.IsNullOrWhiteSpace(this.textBoxName.Text))
            {
                throw new Exception("设备名称不能为空");
            }
            if (comboBoxPlace.SelectedIndex < 0)
            {
                throw new Exception("必须选择设备布署区域");
            }
            if (!ValidIP(this.textBoxIP.Text))
            {
                throw new Exception("IP格式错误");
            }
            int port;
            if (!int.TryParse(this.textBoxPort.Text, out port))
            {
                throw new Exception("端口号必须为整数");
            }
            if (port <= 0 || port > 65535)
            {
                throw new Exception("设备端口号范围为1-65535，请设置正确的端口号!");
            }
            if (string.IsNullOrWhiteSpace(this.textBoxUserName.Text))
            {
                throw new Exception("设备登录用户名不能为空");
            }
            if (string.IsNullOrWhiteSpace(this.passwordBox.Password))
            {
                throw new Exception("设备登录密码不能为空");
            }
            var setting=AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<VideoSetting>(this.editVideoSetting);
            setting.Name = this.textBoxName.Text;
            setting.OwnerGuid = this.comboBoxPlace.SelectedValue as string;
            setting.IP = this.textBoxIP.Text;
            setting.Port = port;
            setting.UserName = this.textBoxUserName.Text;
            setting.Password = this.passwordBox.Password;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(channel =>
                {
                    channel.SaveVideoSetting(setting);
                });
            this.editVideoSetting.Name = setting.Name;
            this.editVideoSetting.OwnerGuid = setting.OwnerGuid;
            this.editVideoSetting.IP = setting.IP;
            this.editVideoSetting.Port = setting.Port;
            this.editVideoSetting.UserName = setting.UserName;
            this.editVideoSetting.Password = setting.Password;
        }
    }
}
