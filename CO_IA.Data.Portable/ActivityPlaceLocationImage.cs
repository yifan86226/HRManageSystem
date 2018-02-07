using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ActivityPlaceLocationImage : INotifyPropertyChanged
    {
        public string GUID { get; set; }
        public string ACTIVITY_PLACE_LOCATION_GUID { get; set; }
        /// <summary>
        /// 图片类型（0：平面图；1：其它）
        /// </summary>
        public int TYPE { get; set; }

        private byte[] image;
        public byte[] Image 
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                NotifyPropertyChanged("Image");
            }
        }

        private string _ImageName;
        public string ImageName
        {
            get
            {
                return _ImageName;
            }
            set
            {
                _ImageName = value;
                NotifyPropertyChanged("ImageName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(PropertyName));
            }
        }
    }

    public class ListImage
    {
        public string locationGuid { get; set; }
        public List<ActivityPlaceLocationImage> listAPLImage { get; set; }
    }
}
