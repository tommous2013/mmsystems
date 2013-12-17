using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit?",
                                    MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                e.Cancel = true;

            }
        }
    }
}