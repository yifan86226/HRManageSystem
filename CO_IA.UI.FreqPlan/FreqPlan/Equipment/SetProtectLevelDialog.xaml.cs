using CO_IA.Client;
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

namespace CO_IA.UI.FreqPlan
{
    /// <summary>
    /// SetProtectLevelDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SetProtectLevelDialog : Window
    {

        private static List<SecurityGradeTemp> securityGradeTempList = new List<SecurityGradeTemp>();

        public static List<SecurityGradeTemp> SecurityGradeTempList
        {
            get { return securityGradeTempList; }
            set { securityGradeTempList = value; }
        }

        private static List<SecurityClassGrade> protectLevelDataSource = new List<SecurityClassGrade>();



        public static List<SecurityClassGrade> ProtectLevelDataSource
        {
            get { return protectLevelDataSource; }
            set { protectLevelDataSource = value; }
        }
     
        public SetProtectLevelDialog()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            List<SecurityGrade> securityGradeList = new List<SecurityGrade>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
            {
                securityGradeList = channel.GetSecurityGrade().ToList<SecurityGrade>();
            });
            ConvertToSecurityGradeTemp(securityGradeList);

       
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                ProtectLevelDataSource = channel.GetSecurityClassGrade(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });


            this.dg_List.ItemsSource = ProtectLevelDataSource;

        }

        private void ConvertToSecurityGradeTemp(List<SecurityGrade> securityClasses)
        {
            SecurityGradeTempList = new List<SecurityGradeTemp>();
            foreach (SecurityGrade sc in securityClasses)
            {
                SecurityGradeTemp sgt = new SecurityGradeTemp();
                sgt.Code = sc.Guid;
                sgt.Name = sc.Name;
                sgt.Comments = sc.Comments;
                SecurityGradeTempList.Add(sgt);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        

        
        }



        /// <summary>
        /// 保存按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ProtectLevelDataSource != null && ProtectLevelDataSource.Count > 0)
            {

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                {
                    channel.SaveSecurityClassGradeList(ProtectLevelDataSource);
                });
                
                MessageBox.Show("保存成功", "提示", MessageBoxButton.OK);
            }
            this.Close();
        }




        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

     
    }
    public class SecurityGradeTemp
    {
        public string Code{ get; set; }
        public string Name{ get; set; }
        public string Comments{ get; set; }
    
    }
}
