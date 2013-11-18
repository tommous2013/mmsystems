using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp.ViewModels;

namespace PhoneApp.Views
{
    public partial class WaitingPage : PhoneApplicationPage
    {
        private WaitingPageViewModel vm;

        public WaitingPage()
        {
            InitializeComponent();
            vm = new WaitingPageViewModel();
            this.DataContext = vm;
        }
    }
}