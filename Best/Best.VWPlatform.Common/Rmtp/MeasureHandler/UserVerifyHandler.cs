using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    //登陆认证指令
    public class UserVerifyHandler : INotifyPropertyChanged
    {
        private string _method = "01";
        private string _user = "rxtest";
        private string _password = "rxtest";
        public UserVerifyHandler(string method = "01", string user = "rxtest", string pass = "rxtest")
        {
            _method = method;
            _user = user;
            _password = pass;
        }
        /// <summary>
        /// METHOD登陆方式，01,用户名/口令方式;00,不需要认证的请求
        /// </summary>
        public string Method
        {
            get { return _method; }
            set
            {
                _method = value;
                OnPropertyChanged("Method");
            }
        }
        /// <summary>
        /// USER用户名，rxtest
        /// </summary>
        public string User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged("User");
            }
        }
        /// <summary>
        /// PASSWD口令，rxtest
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }
        #endregion

        public List<Tuple<string, string>> ToList()
        {
            var verifyList = new List<Tuple<string, string>>
                {
                    new Tuple<string,string>("METHOD",_method),
                    new Tuple<string,string>("USER",_user),
                    new Tuple<string,string>("PASSWD",_password),
                };

            return verifyList;
        }
    }
}
