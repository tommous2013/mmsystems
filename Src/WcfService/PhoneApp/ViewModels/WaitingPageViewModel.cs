using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using PhoneApp.ServiceReference;

namespace PhoneApp.ViewModels
{
    public class WaitingPageViewModel : ViewModelBase
    {
        private DispatcherTimer pollTimer ;
        public WaitingPageViewModel()
        {
            App.Client = new Service1Client();

            GetPlayers();
            pollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            pollTimer.Tick += newTimer_Tick;
            pollTimer.Start();
        }

        private ObservableCollection<OPlayer> _playerList;
        public ObservableCollection<OPlayer> PlayerList
        {
            get { return _playerList; }
            set
            {
                if (_playerList != value)
                {
                    _playerList = value;
                    OnPropertyChanged("PlayerList");
                }
            }
        }

        void newTimer_Tick(object sender, EventArgs e)
        {
            GetPlayers();
        }

        public void GetPlayers()
        {
            App.Client.GetAvailableLobbyRoomsCompleted += Client_GetAvailableLobbyRoomsCompleted;
            App.Client.GetAvailableLobbyRoomsAsync();
        }

        void Client_GetAvailableLobbyRoomsCompleted(object sender, GetAvailableLobbyRoomsCompletedEventArgs e)
        {
            var q = (from room in e.Result
                     where room.TheLobby.LobbyId == App.LobbyRoom.TheLobby.LobbyId
                     select room).First();
            App.LobbyRoom = q;
            if (PlayerList != null && PlayerList.Count >= 4)
            {
                pollTimer.Stop();
                (App.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Views/GamePage.xaml", UriKind.Relative));
            }
            PlayerList = q.PlayerList;
        }
    }
}
