using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EMEEnvironment:INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected 
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.NotifyPropertyChanged("IsSelected");
            }
        }
        //"频率" 
        public double Freq { get; set; }
        //"信号来源
        public string SignalSource { get; set; }
        //"使用单位
        public string Department { get; set; }
        //"单位地址
        public string Address { get; set; }
        //"联系人" 
        public string RelationMan { get; set; }
        //"联系方式
        public string Phone { get; set; }
        //"是否合法
        public string IsLegal { get; set; }
        //"清理标识
        public string IsClear { get; set; }
        private string _resultIsClear;
        //"清理结果标识
        public string ResultIsClear 
        {
            get 
            {
                return _resultIsClear;
            } 
            set
            {
                _resultIsClear = value;
                this.NotifyPropertyChanged("ResultIsClear");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(PropertyName));
            }
        }
    }
}
