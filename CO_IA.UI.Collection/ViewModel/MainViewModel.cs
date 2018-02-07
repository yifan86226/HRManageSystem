using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using CO_IA.UI.Collection.MessageEntity;
using CO_IA.UI.Collection.Model;

namespace CO_IA.UI.Collection.ViewModel
{
    public enum PageEnum
    {
        路测规划,
        频谱数据采集,
        公众移动数据采集,
        数据回放,
        数据生成
    }

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = "";

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }
            set
            {
                Set(() => WelcomeTitle, ref _welcomeTitle, value);
            }
        }

        private int _MainWindowVisibility = 0;
        /// <summary>
        /// 0:隐藏，1：显示
        /// </summary>
        public int MainWindowVisibility
        {
            get
            {
                return _MainWindowVisibility;
            }
            set
            {
                Set(() => MainWindowVisibility, ref _MainWindowVisibility, value);
            }
        }



        private string _MultiPersonTrainingTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string MultiPersonTrainingTitle
        {
            get
            {
                return _MultiPersonTrainingTitle;
            }
            set
            {
                Set(() => MultiPersonTrainingTitle, ref _MultiPersonTrainingTitle, value);
            }
        }



        private string _ChestBitmapTitle = string.Empty;

        public string ChestBitmapTitle
        {
            get { return _ChestBitmapTitle; }
            set
            {
                Set(() => ChestBitmapTitle, ref _ChestBitmapTitle, value);
            }
        }




        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                    WelcomeTitle = item.Title;
                    MultiPersonTrainingTitle = item.MultiPersonTrainingTitle;
                    ChestBitmapTitle = item.ChestBitmapTitle;
                });
            PageGoMessageCmd = new RelayCommand<string>(ExecutePageGoMessageCmd, CanExecutePageGoMessageCmd);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        public RelayCommand<string> PageGoMessageCmd { get; private set; }

        void ExecutePageGoMessageCmd(string msg)
        {
            Messenger.Default.Send<PageGoMessage>(new PageGoMessage() { PageName = msg });
        }

        //最少保证有1条数据时命令可用
        bool CanExecutePageGoMessageCmd(string msg)
        {

            if (msg.Equals(PageEnum.公众移动数据采集.ToString()) || msg.Equals(PageEnum.路测规划.ToString()))
                return false;
            return true;
        }

    }
}
