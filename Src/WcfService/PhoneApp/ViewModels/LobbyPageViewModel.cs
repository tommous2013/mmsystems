using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Phone.Controls;
using PhoneApp.ServiceReference;

namespace PhoneApp.ViewModels
{
    class LobbyPageViewModel : ViewModelBase
    {
        public LobbyPageViewModel()
        {
            App.Client = new Service1Client();
        }

        private ObservableCollection<OLobbyRoom> _lobbyRoomList;
        public ObservableCollection<OLobbyRoom> LobbyRoomList
        {
            get { return _lobbyRoomList; }
            set
            {
                if (_lobbyRoomList != value)
                {
                    _lobbyRoomList = value;
                    OnPropertyChanged("LobbyRoomList");
                }
            }
        }

        public void PopulateLobbyList()
        {
            App.Client.GetAvailableLobbyRoomsCompleted += (s, e) => { LobbyRoomList = e.Result; };
            App.Client.GetAvailableLobbyRoomsAsync();
        }

        public void JoinLobby(OLobbyRoom lr)
        {

            App.LobbyRoom = lr;
            App.Client.SubscribeToLobbyRoomCompleted += Client_SubscribeToLobbyRoomCompleted;
            App.Client.SubscribeToLobbyRoomAsync(App.Me, lr);

        }

        void Client_SubscribeToLobbyRoomCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            (App.Current.RootVisual as PhoneApplicationFrame).Navigate(
                new Uri("/Views/WaitingPage.xaml", UriKind.Relative));
        }

        public void CreateLobby()
        {
            App.Client.CreateLobbyCompleted += Client_CreateLobbyCompleted;
            App.Client.CreateLobbyAsync(App.Me);
        }

        void Client_CreateLobbyCompleted(object sender, CreateLobbyCompletedEventArgs e)
        {
            App.LobbyRoom = e.Result;
            (App.Current.RootVisual as PhoneApplicationFrame).Navigate(
                new Uri("/Views/WaitingPage.xaml", UriKind.Relative));
        }
    }
}
