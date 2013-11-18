using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhoneApp.ServiceReference;

namespace PhoneApp.ViewModels
{
    class GamePageViewModel : ViewModelBase
    {
        public GamePageViewModel()
        {
            App.Client = new Service1Client();
        }
    }
}
