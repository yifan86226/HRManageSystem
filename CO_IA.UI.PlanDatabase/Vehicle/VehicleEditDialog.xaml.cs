using CO_IA.Client;
using CO_IA.Data;
using I_CO_IA.PlanDatabase;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CO_IA.UI.PlanDatabase.Vehicle
{
    /// <summary>
    /// CarInputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleEditDialog : Window, INotifyPropertyChanged
    {
        public event Action AfterSaveEvent;
        private VehicleInfo OriginalVehicleInfo;

        private VehicleInfo currentvehicleInfo;
        public VehicleInfo CurrentVehicleInfo
        {
            get
            {
                return currentvehicleInfo;
            }
            set
            {
                currentvehicleInfo = value;
                NotifyPropertyChanged("CurrentVehicleInfo");
            }
        }

        bool allowedite = true;
        bool IsModify;


        /// <summary>
        /// 是允许修改监测属性(车牌号码和监测车)
        /// true:允许修改
        /// false:不允许修改
        /// </summary>
        public bool ModifyMonitorProperty
        {
            get { return allowedite; }
            set
            {
                allowedite = value;
                if (allowedite) //允许修改
                {
                    txtVehicleNo.IsEnabled = true;
                    chkIsMonitor.IsEnabled = true;
                }
                else
                {

                    txtVehicleNo.IsEnabled = false;
                    chkIsMonitor.IsEnabled = false;
                }
            }
        }

        public VehicleEditDialog(VehicleInfo vehicleInfo, bool isModify = true)
        {
            InitializeComponent();
            IsModify = isModify;
            OriginalVehicleInfo = vehicleInfo;
            CurrentVehicleInfo = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<VehicleInfo>(vehicleInfo);
            this.DataContext = CurrentVehicleInfo;
            this.combArea.ItemsSource = CO_IA.Client.Utility.GetProvinceAreaCode();
        }


        private void imgBus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "车辆图片|*.jpg;*.png;*.jpeg;*.bmp";//过滤器
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;

                //Bitmap tempBm = ImageUtility.SmallPic(fileName, 800, 600);
                //byte[] imageData = null;

                //if (tempBm != null)
                //{
                //    MemoryStream ms = new MemoryStream();
                //    tempBm.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                //    imageData = ms.GetBuffer();
                //}
                //else
                //{
                byte[] imageData = File.ReadAllBytes(fileName);
                //}

                byte[] imageOut = null;
                if (imageData != null)
                {
                    imageOut = AT_BC.Common.ImageZipper.ZipAsJpg(imageData, 800, 600);
                }
                CurrentVehicleInfo.Picture = imageOut;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentVehicleInfo.VehicleNo))
            {
                MessageBox.Show("车牌号码不能为空! ");

            }
            else if (string.IsNullOrEmpty(CurrentVehicleInfo.AreaCode))
            {
                MessageBox.Show("所属地区不能为空!");
            }
            else
            {
                //如果是修改设备
                if (IsModify)
                {
                    //如果将监测站改为非监测站,判断监测站下是否有设备
                    if (CurrentVehicleInfo.IsMonitor == false && CurrentVehicleInfo.IsMonitor != OriginalVehicleInfo.IsMonitor)
                    {
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>(channel =>
                        {
                            int count = channel.GetMonitorEquCount(CurrentVehicleInfo.GUID);
                            if (count > 0)
                            {
                                MessageBox.Show("该车辆已经有所属所属设备,不能将监测车改为非监测车");
                                return;
                            }
                            else
                            {
                                SaveVehicleInfo();
                            }
                        });
                    }
                    else
                    {
                        SaveVehicleInfo();
                    }
                }
                else
                {
                    SaveVehicleInfo();
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveVehicleInfo()
        {
            try
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>
                (channel =>
                {
                    int count = channel.GetVehicleNoCount(CurrentVehicleInfo.GUID, CurrentVehicleInfo.VehicleNo);
                    if (count == 0)
                    {
                        channel.SaveVehicleInfo(CurrentVehicleInfo);
                        MessageBox.Show("保存成功!", "提示", MessageBoxButton.OK);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(string.Format("车牌号码{0}已经存在,请重新输入", CurrentVehicleInfo.VehicleNo), "提示", MessageBoxButton.OK);
                        CurrentVehicleInfo.VehicleNo = null;
                    }
                });

                if (AfterSaveEvent != null)
                {
                    AfterSaveEvent();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.CurrentVehicleInfo.Picture = null;
        }

        private void Image_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {

        }

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

    }
}
