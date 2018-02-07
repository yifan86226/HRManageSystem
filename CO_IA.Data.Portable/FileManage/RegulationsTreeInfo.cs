using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.FileManage
{
    public class RegulationsTreeInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// 子集合
        /// </summary>
        public List<RegulationsTreeInfo> Children { get; set; }
        public RegulationsTreeInfo()
        {
            Children = new List<RegulationsTreeInfo>();
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
                NotifyPropertyChange("Guid");
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
                NotifyPropertyChange("Name");
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
                NotifyPropertyChange("Parent_guid");
            }
        }

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
                NotifyPropertyChange("Activity_guid");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
