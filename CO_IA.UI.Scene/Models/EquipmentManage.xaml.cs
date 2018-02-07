#region 文件描述
/**********************************************************************************
 * 创建人：lw
 * 摘  要：现场监测设备管控
 * 日  期：2016-07-07
 * ********************************************************************************/
#endregion

using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace CO_IA.UI.Scene
{
    /// <summary>
    /// 现场监测设备管控
    /// </summary>
    public partial class EquipmentManage : Window
    {
        public EquipmentManage()
        {
            InitializeComponent();
            List<EquipConfig> equipConfigs = new List<EquipConfig>()
            {
                new EquipConfig()
                {
                    EquipModel="EM550",IP="192.168.1.10",Port="5535",State=true
                },
                new EquipConfig()
                {
                    EquipModel="EB2000",IP="192.168.1.11",Port="5535",State=true
                }
            };
            this.xDgEquipConfig.ItemsSource = equipConfigs;
        }
    }

    public class EquipConfig
    {
        /// <summary>
        /// 设备型号
        /// </summary>
        public string EquipModel { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
    }
}
