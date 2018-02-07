#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：用户资源类
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using PT.Profile.Types;
using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Betx.Portal.Models
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfoSource : UserObject,  IDataErrorInfo
    {
        //默认用户头像
        public static readonly BitmapImage DefaultUserImage = new BitmapImage(new Uri("/Images/userDefaultImage.png", UriKind.RelativeOrAbsolute));
        private BitmapImage _userImage;
        private bool _isSelected = true;
        private string _deptName;
        private string _email;
        private DateTime _userBirthDay;
        private string _userEducation;
        private string _userFax;
        private string _userMobile;
        private string _userPassword;
        private string _userPhone;

        public UserInfoSource()
        {
            UserImage = DefaultUserImage;
            IsOnline = false;
        }
        /// <summary>
        /// 带域名的用户ID
        /// </summary>
        public string DomainId { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public BitmapImage UserImage
        {
            get { return _userImage; }
            set
            {
                _userImage = value;
                //调用 Freeze，初始化的默认用户头像将无法显示
                //_userImage.Freeze();

                RaisePropertyChanged("UserImage");
            }
        }
        /// <summary>
        /// 用户Email
        /// </summary>
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                RaisePropertyChanged("Email");
            }
        }
        /// <summary>
        /// 4A安全平台人员代码 VARCHAR2(12) 
        /// </summary>
        public string SafePlatformCode;
        /// <summary>
        /// 限定名称,此名称唯一,用户流程处理或单点登录 
        /// </summary>
        public string UniqueUserName;
        /// <summary>
        /// 出生日期 
        /// </summary>
        public DateTime UserBirthDay
        {
            get { return _userBirthDay; }
            set
            {
                _userBirthDay = value;
                RaisePropertyChanged("UserBirthDay");
            }
        }
        /// <summary>
        /// 学历,累积字典 VARCHAR2(64) 
        /// </summary>
        public string UserEducation
        {
            get { return _userEducation; }
            set
            {
                _userEducation = value;
                RaisePropertyChanged("UserEducation");
            }
        }
        /// <summary>
        /// 传真 VARCHAR2(200) 
        /// </summary>
        public string UserFax
        {
            get { return _userFax; }
            set
            {
                _userFax = value;
                RaisePropertyChanged("UserFax");
            }
        }
        /// <summary>
        /// 入职日期 
        /// </summary>
        public DateTime UserJoinDate;
        /// <summary>
        /// 离职日期
        /// </summary>
        public DateTime UserLeaveDate;
        /// <summary>
        /// 移动电话 VARCHAR2(200) 
        /// </summary>
        public string UserMobile
        {
            get { return _userMobile; }
            set
            {
                _userMobile = value;
                RaisePropertyChanged("UserMobile");
            }
        }
        /// <summary>
        /// 密码 
        /// </summary>
        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                _userPassword = value;
                RaisePropertyChanged("UserPassword");
            }
        }
        /// <summary>
        /// 确认密码
        /// </summary>
        public string UserPasswordConfirm
        {
            get { return _userPasswordConfirm; }
            set { _userPasswordConfirm = value; RaisePropertyChanged("UserPasswordConfirm"); }
        }
        /// <summary>
        /// 联系电话 VARCHAR2(200) 
        /// </summary>
        public string UserPhone
        {
            get { return _userPhone; }
            set
            {
                _userPhone = value;
                RaisePropertyChanged("UserPhone");
            }
        }

        /// <summary>
        /// 性别，见枚举UserSex定义
        /// </summary>
        public string UserSex
        {
            get { return _userSex; }
            set { _userSex = value; RaisePropertyChanged("UserSex"); }
        }
        /// <summary>
        /// 用户行政区编码
        /// </summary>
        public string DistrictCode
        {
            get { return _districtCode; }
            set
            {
                _districtCode = value;
                RaisePropertyChanged("DistrictCode");
            }
        }
        /// <summary>
        /// 域名,包含子域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 用户是否上线
        /// </summary>
        public bool IsOnline
        {
            get { return _isOnline; }
            set
            {
                _isOnline = value;
                RaisePropertyChanged("IsOnline");
            }
        }

        /// <summary>
        /// 用户状态，见枚举UserStatus定义 
        /// </summary>
        public UserStatusEnum UserStatus;
        /// <summary>
        /// 用户职务
        /// </summary>
        public string UserDuty;

        private string _userSex;
        private string _userPasswordConfirm;
        private bool _isOnline;

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName
        {
            get { return _deptName; }
            set { _deptName = value; RaisePropertyChanged("DeptName"); }
        }
        public string StringContext
        {
            get { return Name; }
            set { Name = value; }
        }
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; RaisePropertyChanged("IsSelected"); }
        }

        public string Error
        {
            get { return "error"; }
        }

        public string this[string columnName]
        {
            get
            {
                bool isValidPassword = !string.IsNullOrWhiteSpace(UserPassword) && UserPassword.Length > 0;
                if (columnName == "UserPassword")
                    return isValidPassword ? string.Empty : "错误";
                if (columnName == "UserPasswordConfirm")
                {
                    if (string.IsNullOrWhiteSpace(UserPassword))
                        return string.Empty;
                    return UserPassword == UserPasswordConfirm ? string.Empty : "请验证您的密码";
                }
                if (columnName == "Name")
                {
                    return string.IsNullOrWhiteSpace(Name) ? "用户名称不允许为空" : string.Empty;
                }
                return string.Empty;
            }
        }

        private string _districtCode;

    }
}
