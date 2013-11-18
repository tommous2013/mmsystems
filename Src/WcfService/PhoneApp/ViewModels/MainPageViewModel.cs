using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using PhoneApp.ServiceReference;

namespace PhoneApp.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel()
        {
            App.Client = new Service1Client();
            PlayerName = "test";
        }
        private string _playerName;
        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                if (_playerName != value)
                {
                    _playerName = value;
                }
                OnPropertyChanged("PlayerName");
            }
        }

        public void Login()
        {
            App.Client.MakePlayerCompleted += client_MakePlayerCompleted;
            App.Client.MakePlayerAsync(PlayerName);

        }

        void client_MakePlayerCompleted(object sender, MakePlayerCompletedEventArgs e)
        {
            App.Me = e.Result;
            (App.Current.RootVisual as PhoneApplicationFrame).Navigate(
                new Uri("/Views/LobbyPage.xaml", UriKind.Relative));
        }

    }
}
