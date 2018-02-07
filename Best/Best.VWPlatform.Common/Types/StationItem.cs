using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 站点基项
    /// </summary>

    [Serializable]
    public class StationItem : INotifyPropertyChanged
    {
        public StationItem()
        {
            DicProperties = new Dictionary<string, string>();    
        }

        public Dictionary<string, string> DicProperties { set; get; }


        public string Longitude
        {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.STAT_LG"))
                {
                    return null;
                }
                return DicProperties["RSBT_STATION_CACHE.STAT_LG"];
            }
        }

        public string Latitude {
            get
            {
                if (!DicProperties.ContainsKey("RSBT_STATION_CACHE.STAT_LA"))
                {
                    return null;
                }
                return DicProperties["RSBT_STATION_CACHE.STAT_LA"];
            } 
        }


        private MapPointEx _point;
        
        /// <summary>
        /// 站点地理坐标
        /// </summary>
        public MapPointEx Point
        {
            get
            {
                if (!string.IsNullOrEmpty(Latitude) && !string.IsNullOrEmpty(Longitude))
                {
                    _point = new MapPointEx(double.Parse(Longitude), double.Parse(Latitude));
                }

                return _point;
            }
            set
            {
                _point = value;
                OnPropertyChanged("Point");
            }
        }

        protected void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
