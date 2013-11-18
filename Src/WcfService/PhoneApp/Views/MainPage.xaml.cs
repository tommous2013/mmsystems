using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using PhoneApp.ServiceReference;
using PhoneApp.ViewModels;

namespace PhoneApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        private MainPageViewModel vm;
        public MainPage()
        {
            InitializeComponent();

            vm = new MainPageViewModel();
            this.DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vm.Login();
        }
    }
}