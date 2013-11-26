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
   public class LobbyPageViewModel : ViewModelBase
    {
        public LobbyPageViewModel()
        {
            App.Client = new Service1Client();
            LobbyRoomList = new ObservableCollection<OLobbyRoom>();
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

        public event EventHandler PopDone;
        public void PopulateLobbyList()
        {
            App.Client.GetAvailableLobbyRoomsCompleted += Client_GetAvailableLobbyRoomsCompleted;
            App.Client.GetAvailableLobbyRoomsAsync();
        }

        void Client_GetAvailableLobbyRoomsCompleted(object sender, GetAvailableLobbyRoomsCompletedEventArgs e)
        {
            var q = from room in e.Result
                            where room.TheLobby.IsWaitingForPlayers == true
                            select room;
            LobbyRoomList.Clear();
            foreach (OLobbyRoom room in q)
            {
                LobbyRoomList.Add(room);
            }
            if(PopDone!=null)
                PopDone(this, EventArgs.Empty);
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
