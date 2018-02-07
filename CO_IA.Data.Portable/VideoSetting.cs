using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class VideoSetting : AT_BC.Data.CheckableData<string>,INetDevice
    {
        /// <summary>
        /// 识别名称
        /// </summary>
        private string name;

        /// <summary>
        /// 获取或设置识别名称,可以使编号或者地址等能够标识摄像机的任意名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        /// <summary>
        /// 活动标识
        /// </summary>
        private string activityGuid;

        /// <summary>
        /// 获取或设置活动标识
        /// </summary>
        public string ActivityGuid
        {
            get
            {
                return this.activityGuid;
            }
            set
            {
                this.activityGuid = value;
                this.NotifyPropertyChanged("ActivityGuid");
            }
        }

        /// <summary>
        /// 设备归属(可能是布署区域或者使用单位等)
        /// </summary>
        private string ownerGuid;

        /// <summary>
        /// 获取或设置设备归属(可能是布署区域或者使用单位等),类型参考归属类型
        /// </summary>
        public string OwnerGuid
        {
            get
            {
                return this.ownerGuid;
            }
            set
            {
                if (this.ownerGuid != value)
                {
                    this.ownerGuid = value;
                    this.NotifyPropertyChanged("OwnerGuid");
                }
            }
        }

        /// <summary>
        /// 设备归属类型
        /// </summary>
        private string ownerType;

        /// <summary>
        /// 获取或设置设备归属类型,该类型标识设备归属者性质
        /// </summary>
        public string OwnerType
        {
            get
            {
                return this.ownerType;
            }
            set
            {
                this.ownerType = value;
                this.NotifyPropertyChanged("OwnerType");
            }
        }

        /// <summary>
        /// 设备ip
        /// </summary>
        private string ip;

        /// <summary>
        /// 获取或设置设备ip
        /// </summary>
        public string IP
        {
            get
            {
                return this.ip;
            }
            set
            {
                if (value != this.ip)
                {
                    this.ip = value;
                    this.NotifyPropertyChanged("IP");
                }
            }
        }

        /// <summary>
        /// 设备端口号
        /// </summary>
        private int port;

        /// <summary>
        /// 获取或设置设备端口号
        /// </summary>
        public int Port
        {
            get
            {
                return this.port;
            }
            set
            {
                if (this.port != value)
                {
                    this.port = value;
                    this.NotifyPropertyChanged("Port");
                }
            }
        }

        /// <summary>
        /// 设备登录用户名
        /// </summary>
        private string userName;

        /// <summary>
        /// 获取或设置设备登录用户名
        /// </summary>
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                if (this.userName != value)
                {
                    this.userName = value;
                    this.NotifyPropertyChanged("UserName");
                }
            }
        }

        /// <summary>
        /// 设备登录密码
        /// </summary>
        private string password;

        /// <summary>
        /// 获取或设置设备登录密码
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    this.NotifyPropertyChanged("Password");
                }
            }
        }

        string INetDevice.Description
        {
            get 
            {
                return this.Name;
            }
        }

        string INetDevice.Guid
        {
            get 
            {
                return this.Key;
            }
        }

        //public bool Equals(INetDevice other)
        //{
        //    if (this.GetHashCode() == other.GetHashCode())
        //    {
        //        return this.Key.Equals(other.ikey) | this.ip.GetHashCode() | this.port.GetHashCode();
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public override int GetHashCode()
        //{
        //    return this.Key.GetHashCode() | this.ip.GetHashCode() | this.port.GetHashCode();
        //}
    }
}
