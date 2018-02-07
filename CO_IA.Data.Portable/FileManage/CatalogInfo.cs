using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.FileManage
{
    public class CatalogInfo : INotifyPropertyChanged
    {
        private string _Activity_guid;
        public string Activity_guid
        {
            get
            {
                return _Activity_guid;
            }
            set
            {
                _Activity_guid = value;
            }
        }

        private string _Guid;
        public string Guid
        {
            get
            {
                return _Guid;
            }

            set
            {
                _Guid = value;
            }
        }

        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _Parent_guid;
        public string Parent_guid
        {
            get
            {
                return _Parent_guid;
            }
            set
            {
                _Parent_guid = value;
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
