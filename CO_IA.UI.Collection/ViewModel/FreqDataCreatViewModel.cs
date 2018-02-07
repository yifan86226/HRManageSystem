using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace CO_IA.UI.Collection.ViewModel
{
    public class FreqDataCreatViewModel : ViewModelBase
    {

        public FreqDataCreatViewModel()
        {
            WinCloseCommand = new RelayCommand(() =>
            {

                Messenger.Default.Send<GenericMessage<string>>(new GenericMessage<string>(this, "showmainwindow"));
            });
        }



        public RelayCommand WinCloseCommand { get; private set; }
    }
}
