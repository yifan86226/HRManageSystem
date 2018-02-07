#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：参数信息暂存类
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
//using Betx.Portal.Common;
//using Betx.Portal.Common.Helper;
//using Betx.Portal.Models.Account;
using Microsoft.Practices.Prism.ViewModel;

namespace Betx.Portal.Models
{
    /// <summary>
    /// 参数信息
    /// </summary>
    [DataContract]
    public sealed class ParameterInformation : NotificationObject, IEditableObject
    {
        private static ParameterInformation _parameterInformation;
        private static readonly object LockObj = new object();
        private static string _systemDirectory;
        private UserParameterInformation _currentUserParameter;
        private GlobalParameterInformation _globalParameter;
        private GlobalParameterInformation _backGlobalParameter;
        private const string ConfigFileName = "BetxportalParameter.xml";

        public ParameterInformation()
        {
            UserParameters = new List<UserParameterInformation>();
            if (_globalParameter == null)
            {
                _globalParameter = new GlobalParameterInformation();
            }
            if (_currentUserParameter == null)
                _currentUserParameter = new UserParameterInformation();
        }
        /// <summary>
        /// 当前系统参数信息
        /// </summary>
        public static ParameterInformation Current
        {
            get
            {
                if (_parameterInformation == null)
                {
                    lock (LockObj)
                    {
                        if (_parameterInformation == null)
                        {
                            _parameterInformation = new ParameterInformation();
                            _systemDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        }
                    }
                }
                return _parameterInformation;
            }
            private set
            {
                lock (LockObj)
                {
                    _parameterInformation = value;
                }
            }
        }
        /// <summary>
        /// 全局参数
        /// </summary>
        [DataMember]
        public GlobalParameterInformation GlobalParameter
        {
            get { return _globalParameter; }
            set { _globalParameter = value; }
        }
        /// <summary>
        /// 当前用户参数
        /// </summary>
        [DataMember]
        public UserParameterInformation CurrentUserParameter
        {
            get { return _currentUserParameter; }
            set { _currentUserParameter = value; }
        }
        /// <summary>
        /// 用户参数集合
        /// </summary>
        [DataMember]
        public List<UserParameterInformation> UserParameters { get; set; }

        internal void Load(string pCurrentUserId = "")
        {
            //var fileName = string.Format("{0}\\{1}", _systemDirectory, ConfigFileName);
            //if (!File.Exists(fileName))
            //{
            //    return;
            //}
            //try
            //{
            //    using (var fileStream = new FileStream(fileName, FileMode.Open))
            //    {
            //        using (var reader = XmlDictionaryReader.CreateTextReader(fileStream, new XmlDictionaryReaderQuotas()))
            //        {
            //            var ser = new DataContractSerializer(typeof(ParameterInformation));
            //            Current = ser.ReadObject(reader, true) as ParameterInformation;
            //            reader.Close();
            //        }
            //        fileStream.Close();
            //    }
            //    bool changed = false;
            //    // 当 ID 有子域名时，清除之前的登陆历史
            //    if (Current != null && Current.CurrentUserParameter != null && Current.CurrentUserParameter.LoginUser != null &&
            //        Current.CurrentUserParameter.LoginUser.Id.Length > 36)
            //    {
            //        Current.CurrentUserParameter.LoginUser.Clear();
            //        Current.CurrentUserParameter.IsRememberPassword = false;
            //        changed = true;
            //    }
            //    if (Current != null && Current.UserParameters != null)
            //    {
            //        var invalidUserList = Current.UserParameters.Where(user => user.LoginUser.Id.Length > 36).ToList();
            //        foreach (var user in invalidUserList)
            //        {
            //            Current.UserParameters.Remove(user);
            //        }
            //        if (invalidUserList.Count > 0)
            //        {
            //            changed = true;
            //        }
            //    }
            //    if (changed)
            //    {
            //        Save();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.WriteLog("读取系统配置参数", Utile.GetExceptionContext(ex));
            //}
        }

        internal void Save()
        {
            //if (Current == null || CurrentUserParameter == null)
            //    return;
            //if (Current.UserParameters == null)
            //    Current.UserParameters = new List<UserParameterInformation>();
            //var userPassword = CurrentUserParameter.LoginUser.Password;
            //var foundUser = (from i in Current.UserParameters where (!string.IsNullOrWhiteSpace(CurrentUserParameter.LoginUser.Id) && i.LoginUser.Id == CurrentUserParameter.LoginUser.Id) select i).FirstOrDefault();
            //if (foundUser == null && !string.IsNullOrWhiteSpace(CurrentUserParameter.LoginUser.Id))
            //{
            //    CurrentUserParameter.LoginUser.Password = CurrentUserParameter.IsRememberPassword ? CurrentUserParameter.LoginUser.Password : string.Empty;
            //    Current.UserParameters.Add(CurrentUserParameter);
            //}
            //else if (foundUser != null)
            //{
            //    foundUser.IsAutoLogin = CurrentUserParameter.IsAutoLogin;
            //    foundUser.IsRememberPassword = CurrentUserParameter.IsRememberPassword;
            //    foundUser.LoginUser.Name = CurrentUserParameter.LoginUser.Name;
            //    foundUser.LoginUser.Password = CurrentUserParameter.IsRememberPassword ? CurrentUserParameter.LoginUser.Password : string.Empty;
            //    CurrentUserParameter.LoginUser.Password = CurrentUserParameter.IsRememberPassword ? CurrentUserParameter.LoginUser.Password : string.Empty;
            //}

            //var fileName = string.Format("{0}\\{1}", _systemDirectory, ConfigFileName);
            //using (var fileStream = new FileStream(fileName, FileMode.Create))
            //{
            //    var ser = new DataContractSerializer(typeof(ParameterInformation));
            //    ser.WriteObject(fileStream, Current);
            //    fileStream.Close();
            //}
            //CurrentUserParameter.LoginUser.Password = userPassword;
        }

        #region IEditableObject
        public void BeginEdit()
        {
            if (_globalParameter == null)
                return;
            _backGlobalParameter = _globalParameter.Clone();
        }

        public void CancelEdit()
        {
            if (_globalParameter == null || _backGlobalParameter == null)
                return;
            _globalParameter.Address = _backGlobalParameter.Address;
            _globalParameter.Code = _backGlobalParameter.Code;
            _globalParameter.Port = _backGlobalParameter.Port;
            _globalParameter.IsPopupPrompts = _backGlobalParameter.IsPopupPrompts;
            _globalParameter.ShowPromptsTime = _backGlobalParameter.ShowPromptsTime;
        }

        public void EndEdit()
        {

        }
        #endregion
    }
    /// <summary>
    /// 全局参数信息
    /// </summary>
    [DataContract]
    public sealed class GlobalParameterInformation : NotificationObject, IDataErrorInfo
    {
        private string _code;
        private string _address;
        private int _port;
        private bool _isPopupPrompts;
        private int _showPromptsTime;
        private string _domain;

        /// <summary>
        /// 行政区编码
        /// </summary>
        [DataMember]
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                RaisePropertyChanged("Code");
            }
        }
        /// <summary>
        /// 服务器地址
        /// </summary>
        [DataMember]
        public string Address
        {
            get { return _address; }
            set { _address = value; RaisePropertyChanged("Address"); }
        }
        /// <summary>
        /// 服务器端口
        /// </summary>
        [DataMember]
        public int Port
        {
            get { return _port; }
            set { _port = value; RaisePropertyChanged("Port"); }
        }

        [DataMember]
        public bool IsPopupPrompts
        {
            get { return _isPopupPrompts; }
            set { _isPopupPrompts = value; RaisePropertyChanged("IsPopupPrompts"); }
        }

        [DataMember]
        public int ShowPromptsTime
        {
            get { return _showPromptsTime; }
            set { _showPromptsTime = value; RaisePropertyChanged("ShowPromptsTime"); }
        }
        #region IDataErrorInfo
        public string Error
        {
            get { return string.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Code":
                        return string.IsNullOrWhiteSpace(Code) ? "行政区编码不能为空" : string.Empty;
                    case "Address":
                        return string.IsNullOrWhiteSpace(Address) ? "服务器地址不能为空" : string.Empty;
                    case "Port":
                        return Port == 0 ? "端口号不能为0" : string.Empty;
                }
                return string.Empty;
            }
        }
        #endregion

        public GlobalParameterInformation Clone()
        {
            return this.MemberwiseClone() as GlobalParameterInformation;
        }
    }
    /// <summary>
    /// 用户参数信息
    /// </summary>
    [DataContract]
    public sealed class UserParameterInformation : NotificationObject
    {
        private bool _isRememberPassword;
        private bool _isAutoLogin;
        public UserParameterInformation()
        {
            //LoginUser = new LoginUser();
            CsFilePaths = new Dictionary<string, string>();
        }
        /// <summary>
        /// 用户信息
        /// </summary>
        //[DataMember]
        //public LoginUser LoginUser { get; set; }
        /// <summary>
        /// 记住密码
        /// </summary>
        //[DataMember]
        //public bool IsRememberPassword
        //{
        //    get { return _isRememberPassword; }
        //    set
        //    {
        //        _isRememberPassword = value;
        //        RaisePropertyChanged("IsRememberPassword");
        //        if (!_isRememberPassword)
        //        {
        //            IsAutoLogin = false;
        //        }
        //    }
        //}
        /// <summary>
        /// 自动登录
        /// </summary>
        //[DataMember]
        //public bool IsAutoLogin
        //{
        //    get { return _isAutoLogin; }
        //    set
        //    {
        //        _isAutoLogin = value;
        //        RaisePropertyChanged("IsAutoLogin");
        //        if (_isAutoLogin)
        //        {
        //            IsRememberPassword = true;
        //        }
        //    }
        //}
        /// <summary>
        /// Cs程序文件路径
        /// </summary>
        [DataMember]
        public Dictionary<string, string> CsFilePaths { get; set; }
        /// <summary>
        /// 获取Cs程序完整路径
        /// </summary>
        /// <param name="pKey"></param>
        /// <returns></returns>
        public string GetCsFilePath(string pKey)
        {
            if (CsFilePaths == null)
                CsFilePaths = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(pKey) && CsFilePaths.ContainsKey(pKey))
                return CsFilePaths[pKey];
            return string.Empty;
        }

        public void Clear()
        {
            //LoginUser.Clear();
            //CsFilePaths.Clear();
        }
    }
}
