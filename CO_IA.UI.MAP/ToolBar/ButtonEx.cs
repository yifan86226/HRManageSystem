#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：Bar按钮控件
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace CO_IA.UI.MAP
{
    public class ButtonEx : Button, INotifyPropertyChanged
    {
        private ImageSource _buttonIcon;
        private ImageSource _buttonIconBK;
        private string _toolTipString;
        private string _backColor;
        public event PropertyChangedEventHandler PropertyChanged;
        public ImageSource ButtonIcon
        {
            get
            {
                return this._buttonIcon;
            }
            set
            {
                this._buttonIcon = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ButtonIcon"));
                }
            }
        }
        public ImageSource ButtonIconBK
        {
            get
            {
                return this._buttonIconBK;
            }
            set
            {
                this._buttonIconBK = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ButtonIconBK"));
                }
            }
        }
        public string ToolTipString
        {
            get
            {
                return this._toolTipString;
            }
            set
            {
                this._toolTipString = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ToolTipString"));
                }
            }
        }
        public string BackColor
        {
            get
            {
                return this._backColor;
            }
            set
            {
                this._backColor = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("BackColor"));
                }
            }
        }
    }
}
