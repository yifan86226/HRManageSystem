using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using CO_IA.UI.Collection.IService;
using CO_IA.UI.Collection.Model;
using CO_IA.UI.Collection.Service;

namespace CO_IA.UI.Collection.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            try
            {
                ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

                if (ViewModelBase.IsInDesignModeStatic)
                {
                    SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();

                    //SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
                }
                else
                {
                    SimpleIoc.Default.Register<IDataService, DataService>();
                    SimpleIoc.Default.Register<IChestBitmapService, ChestBitmapService>();
                    //SimpleIoc.Default.Register<IMultiPersionTrainingSerice, MultiPersionTrainingService>();
                }

                SimpleIoc.Default.Register<MainViewModel>();
                //SimpleIoc.Default.Register<FreqDataCollectViewModel>();
                //SimpleIoc.Default.Register<FreqDataCreatViewModel>();

                FreqDataCreat = new FreqDataCreatViewModel();
                FreqDataCollect = new FreqDataCollectViewModel();
                FreqDataCollectNew = new FreqDataCollectNewViewModel();
            }
            catch (Exception ex)
            {
                throw new Exception("ViewModelLocator初始化错误，错误信息:" + ex.Message);
            }
            return;


            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();

                //SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
                SimpleIoc.Default.Register<IChestBitmapService, ChestBitmapService>();
                //SimpleIoc.Default.Register<IMultiPersionTrainingSerice, MultiPersionTrainingService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<FreqDataCollectViewModel>();
            SimpleIoc.Default.Register<FreqDataCreatViewModel>();
            //SimpleIoc.Default.Register<FreqDataReloadViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            //set
            //{
            //    _Main = value;
            //}
            //get
            //{
            //    return _Main;
            //}
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        private MainViewModel _Main = null;


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public FreqDataCollectViewModel FreqDataCollect
        {
            set
            {
                _FreqDataCollect = value;
            }
            get
            {
                return _FreqDataCollect;
                //return ServiceLocator.Current.GetInstance<FreqDataCollectViewModel>();
            }
        }
        private FreqDataCollectViewModel _FreqDataCollect = null;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public FreqDataCollectNewViewModel FreqDataCollectNew
        {
            set
            {
                _FreqDataCollectNew = value;
            }
            get
            {
                return _FreqDataCollectNew;
                //return ServiceLocator.Current.GetInstance<FreqDataCollectViewModel>();
            }
        }
        private FreqDataCollectNewViewModel _FreqDataCollectNew = null;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public FreqDataCreatViewModel FreqDataCreat
        {
            //get
            //{
            //    return ServiceLocator.Current.GetInstance<FreqDataCreatViewModel>();
            //}
            set
            {
                _FreqDataCreat = value;
            }
            get
            {
                return _FreqDataCreat;
                //return ServiceLocator.Current.GetInstance<FreqDataCollectViewModel>();
            }
        }
        private FreqDataCreatViewModel _FreqDataCreat = null;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        //public FreqDataReloadViewModel FreqDataReload
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<FreqDataReloadViewModel>();
        //    }
        //}


        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}
