using CO_IA.Data;
using System;
using System.Collections.Generic;
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

namespace CO_IA.UI.PlanDatabase.Equipments
{

    /// <summary>
    /// 查询设备详细信息
    /// 使用方法:
    /// 设置DataContext属性为ActivityEquipment对象
    /// AllowEdit:true,编辑界面。false,查看详细信息界面
    /// </summary>
    public partial class EquipmentManageDialog : Window
    {
        private Equipment CurrentEquipment { get; set; }

        private Equipment DefaultEquipment { get; set; }

        private ActivityEquipment DefaultActivityEquipment { get; set; }

        public event Func<Equipment, bool> OnSaveEvent;

        bool allowedit = true;
        public bool AllowEdit
        {
            set
            {
                allowedit = value;
                if (allowedit)
                {
                    equipmentedit.IsReadOnly = false;
                    // this.borderPanel.Visibility = Visibility.Collapsed;
                    this.btnOk.Visibility = Visibility.Visible;
                    this.btnCancel.Visibility = Visibility.Visible;
                }
                else
                {
                    equipmentedit.IsReadOnly = true;
                    //this.borderPanel.Visibility = Visibility.Visible;
                    this.btnOk.Visibility = Visibility.Collapsed;
                    this.btnCancel.Visibility = Visibility.Collapsed;
                }
            }
        }

        public EquipmentManageDialog()
        {
            InitializeComponent();
            this.DataContextChanged += EquipmentManageDialog_DataContextChanged;
        }

        private void EquipmentManageDialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Type type = this.DataContext.GetType();
            if (type == typeof(Equipment))
            {
                this.CurrentEquipment = this.DataContext as Equipment;

                this.DefaultEquipment = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<Equipment>(this.CurrentEquipment);
                this.equipmentedit.DataContext = this.DefaultEquipment;
            }
            else
            {
                this.CurrentEquipment = this.DataContext as ActivityEquipment;

                this.DefaultActivityEquipment = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<ActivityEquipment>(this.DataContext as ActivityEquipment);
                this.equipmentedit.DataContext = this.DefaultActivityEquipment;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (OnSaveEvent != null)
            {
                //Equipment equ = this.equipmentedit.DataContext as Equipment;
                Equipment equ = this.equipmentedit.CurrentEquipment;
                if (VerifyEquipment(equ))
                {
                    bool result = OnSaveEvent(equ);
                    if (result)
                    {
                        this.Close();
                    }
                }
            }
            else
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool VerifyEquipment(Equipment equ)
        {
            StringBuilder errormsg = new StringBuilder();
            bool result = true;
            if (string.IsNullOrEmpty(equ.Name))
            {
                errormsg.AppendLine("名称不能为空");
                result = false;
            }
            if (equ.EQUCount == 0)
            {
                errormsg.AppendLine("设备数量应该大于0");
                result = false;
            }
            if (equ.EquipmentClassID == null)
            {
                errormsg.AppendLine("设备类别不能为空");
                result = false;
            }
            if (equ.IsStation)
            {
                if (string.IsNullOrEmpty(equ.StationName))
                {
                    errormsg.AppendLine("台站名称不能为空");
                    result = false;
                }
                if (string.IsNullOrEmpty(equ.StationTDI))
                {
                    errormsg.AppendLine("台站编号不能为空");
                    result = false;
                }
            }
            if (equ.SendFreq == null)
            {
                errormsg.AppendLine("发射频率不能为空");
                result = false;
            }
            if (equ.Power_W == null)
            {
                errormsg.AppendLine("发射功率不能为空");
                result = false;
            }
            if (equ.Band_kHz == null)
            {
                errormsg.AppendLine("波道带宽不能为空");
                result = false;
            }
            if (equ.IsTunable)
            {
                if (equ.FreqRange != null)
                {
                    if (equ.FreqRange.Little == null)
                    {
                        errormsg.AppendLine("开始频率不能为空");
                        result = false;
                    }
                    if (equ.FreqRange.Great == null)
                    {
                        errormsg.AppendLine("结束频率不能为空");
                        result = false;
                    }
                    if (equ.FreqRange.Little != null && equ.FreqRange.Great != null)
                    {
                        if (equ.FreqRange.Little >= equ.FreqRange.Great)
                        {
                            errormsg.AppendLine("结束频率应该大于开始频率");
                            result = false;
                        }
                    }
                }
            }
            if (!equ.IsMobile) //移动设备
            {
                if (equ.Longitude == null)
                {
                    errormsg.AppendLine("经度不能为空");
                    result = false;
                }
                if (equ.Latitude == null)
                {
                    errormsg.AppendLine("纬度不能为空");
                    result = false;
                }
                if (string.IsNullOrEmpty(equ.Address))
                {
                    errormsg.AppendLine("地点不能为空");
                    result = false;
                }
            }


            //if (equ.Modulation == null)
            //{
            //    errormsg.Append("调制方式不能为空 \n");
            //    result = false;
            //}

            if (!result)
            {
                MessageBox.Show(errormsg.ToString(), "验证失败");
            }
            return result;
        }
    

    
    }
}
