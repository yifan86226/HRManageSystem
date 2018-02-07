using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class LicenseTemplete : INotifyPropertyChanged
    {
        public LicenseTemplete()
        {
            if (string.IsNullOrEmpty(Guid))
            {
                Guid = System.Guid.NewGuid().ToString();
            }
            LicenseItems = new List<LicenseItem>();
        }
        /// <summary>
        /// 模板标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 活动标识
        /// </summary>
        public string ActivityGuid { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public List<LicenseItem> LicenseItems
        {
            get;
            set;
        }

        /// <summary>
        /// 显示背景图片
        /// </summary>
        public bool IsShowImage { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public byte[] BackgroundImage { get; set; }

        /// <summary>
        /// 是否显示二维码
        /// </summary>
        public bool IsShowQRCode { get; set; }

        private byte[] qrCode;
        /// <summary>
        /// 二维码
        /// </summary>
        public byte[] QRCode
        {
            get
            {
                return qrCode;
            }
            set
            {
                qrCode = value;
                NotifyPropertyChanged("QRCode");
            }
        }

        /// <summary>
        /// XML格式
        /// </summary>
        public string XMLLicenseItems { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string proname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(proname));
            }
        }
    }
}
