using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace PhoneApp.Views
{
    public partial class EndPage : PhoneApplicationPage
    {
        public EndPage()
        {
            InitializeComponent();
            if (App.LobbyRoom.PlayerList.First(p => p.VictoryPoints >= 10).PlayerId == App.Me.PlayerId)
            {
                Title.Text = "You Win";
            }
            else
            {
                Title.Text = "You Lose";
            }
            PositionList.DataContext = App.LobbyRoom.PlayerList.OrderBy(p => p.VictoryPoints);
        }
    }
}