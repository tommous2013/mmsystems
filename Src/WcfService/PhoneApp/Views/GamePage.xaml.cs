using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp.Models;

namespace PhoneApp.Views
{
    public partial class GamePage : PhoneApplicationPage
    {
        private int[] _numArray;
        private string[] _hexArray;
        private double[,] _hexPosition;
        private Dictionary<Point, Settlement> settlements;
        private DispatcherTimer pollTimer;
        private Random r;
        private int _dice;
        private BuildStatusEnum _buildStatus;

        public GamePage()
        {
            InitializeComponent();
            _buildStatus = BuildStatusEnum.None;
            BuildPopup.Width = Application.Current.Host.Content.ActualWidth;
            r = new Random();
            pollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };

            _numArray = new int[] { 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12 };
            Shuffle.ShuffleIt(_numArray, App.LobbyRoom.TheLobby.LobbyId); //lobbyId is random seed
            _hexArray = new string[]
                {
                    "Brick", "Brick", "Brick", "Brick", "IronOre", "IronOre", "IronOre", "Sheep", "Sheep", "Sheep", "Sheep"
                    , "Wood", "Wood", "Wood", "Wood", "Grain", "Grain", "Grain", "Grain"
                };
            Shuffle.ShuffleIt(_hexArray, App.LobbyRoom.TheLobby.LobbyId); //lobbyId is random seed

            _hexPosition = new double[,] // positions of hex on board
                {
                            {140, 0},
                        {70, 40},{210, 40},
                    {0, 80},{140, 80},{280, 80},
                        {70, 120},{210, 120},
                    {0, 160},{140, 160},{280, 160},
                        {70, 200},{210, 200},
                    {0, 240},{140, 240},{280, 240},
                        {70, 280},{210, 280},
                            {140, 320},
                };

            # region Dictionary<Point, Settlement>
            settlements = new Dictionary<Point, Settlement>
                {
                    {new Point(165, 10), null},
                    {new Point(215, 10), null},
                    {new Point(95, 50), null},
                    {new Point(145, 50), null},
                    {new Point(235, 50), null},
                    {new Point(285, 50), null},
                    {new Point(25, 90), null},
                    {new Point(75, 90), null},
                    {new Point(165, 90), null},
                    {new Point(215, 90), null},
                    {new Point(305, 90), null},
                    {new Point(355, 90), null},
                    {new Point(5, 130), null},
                    {new Point(95, 130), null},
                    {new Point(145, 130), null},
                    {new Point(235, 130), null},
                    {new Point(285, 130), null},
                    {new Point(375, 130), null},
                    {new Point(25, 170), null},
                    {new Point(75, 170), null},
                    {new Point(165, 170), null},
                    {new Point(215, 170), null},
                    {new Point(305, 170), null},
                    {new Point(355, 170), null},
                    {new Point(5, 210), null},
                    {new Point(95, 210), null},
                    {new Point(145, 210), null},
                    {new Point(235, 210), null},
                    {new Point(285, 210), null},
                    {new Point(375, 210), null},
                    {new Point(25, 250), null},
                    {new Point(75, 250), null},
                    {new Point(165, 250), null},
                    {new Point(215, 250), null},
                    {new Point(305, 250), null},
                    {new Point(355, 250), null},
                    {new Point(5, 290), null},
                    {new Point(95, 290), null},
                    {new Point(145, 290), null},
                    {new Point(235, 290), null},
                    {new Point(285, 290), null},
                    {new Point(375, 290), null},
                    {new Point(25, 330), null},
                    {new Point(75, 330), null},
                    {new Point(165, 330), null},
                    {new Point(215, 330), null},
                    {new Point(305, 330), null},
                    {new Point(355, 330), null},
                    {new Point(95, 370), null},
                    {new Point(145, 370), null},
                    {new Point(235, 370), null},
                    {new Point(285, 370), null},
                    {new Point(165, 410), null},
                    {new Point(215, 410), null}
                };
            # endregion

            GenGameMap();
            pollTimer.Tick += pollTimer_Tick;
            pollTimer.Start();

        }

        public int Dice
        {
            get { return _dice; }
            set
            {
                _dice = r.Next(2, 12);
            }
        }

        void pollTimer_Tick(object sender, EventArgs e)
        {
            App.Client.GetALobbyRoomCompleted += Client_GetALobbyRoomCompleted;
            App.Client.GetALobbyRoomAsync(App.LobbyRoom.TheLobby);
        }

        void Client_GetALobbyRoomCompleted(object sender, ServiceReference.GetALobbyRoomCompletedEventArgs e)
        {
            App.LobbyRoom = e.Result;
            App.Me = (from player in e.Result.PlayerList
                      where player.PlayerId == App.Me.PlayerId
                      select player).First();
            if (App.Me.MyTurn)
            {
                AppBar.IsVisible = true;
                Dice = 0; // any set wil give new random num

                //MessageBox.Show();
            }
        }

        public void GenGameMap()
        {
            TileUserContol[] board = new TileUserContol[19];

            for (int i = 0; i < _hexPosition.Length / 2; i++)
            {
                board[i] = new TileUserContol(_hexArray[i], _numArray[i]);
                board[i].SetValue(Canvas.LeftProperty, _hexPosition[i, 0]);
                board[i].SetValue(Canvas.TopProperty, _hexPosition[i, 1]);
                Can.Children.Add(board[i]);
            }
        }

        private void Can_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Point p = e.GetPosition(Can);

            //temp
            App.Me.Color = "Red";
            App.Me.MyTurn = true;
            _buildStatus = BuildStatusEnum.Road;
            //temp

            if (App.Me.MyTurn && _buildStatus != BuildStatusEnum.None)
            {
                if (_buildStatus == BuildStatusEnum.City || _buildStatus == BuildStatusEnum.Settlement)
                {
                    foreach (var entry in settlements)
                    {
                        if (p.X < entry.Key.X + 10 && p.X > entry.Key.X - 10 && p.Y < entry.Key.Y + 10 && p.Y > entry.Key.Y - 10 && entry.Value == null)
                        {
                            if (_buildStatus == BuildStatusEnum.Settlement)
                            {
                                Image img = new Image();
                                img.Source = new BitmapImage(new Uri(@"\Images\Pieces\Settlement\Settlement" + App.Me.Color + ".png", UriKind.Relative));
                                img.SetValue(Canvas.LeftProperty, entry.Key.X - 50);
                                img.SetValue(Canvas.TopProperty, entry.Key.Y - 50);
                                Can.Children.Add(img);

                                Settlement set = new Settlement();
                                set.Image = img;
                                set.Upgraded = false;
                                settlements[entry.Key] = set; //zet settlement in dictionary
                                _buildStatus = BuildStatusEnum.None;
                                return; // anders error omdat settlements niet mag worden aangepast in foreach
                            }
                            if (_buildStatus == BuildStatusEnum.City)
                            {
                                if (settlements[entry.Key].Upgraded == false)
                                {
                                    settlements[entry.Key].Upgraded = true;
                                    _buildStatus = BuildStatusEnum.None;
                                    //todo: update settlement img to city
                                }
                            }
                        }
                    }
                }
                if (_buildStatus == BuildStatusEnum.Road)
                {
                    Image img = new Image();
                    Point test = p;
                    var closestPoints = settlements.Keys.Where(point => point != test).
                                               OrderBy(point => NotReallyDistanceButShouldDo(test, point)).
                                               Take(2).ToList();

                    if (closestPoints.ElementAt(0).Y == closestPoints.ElementAt(1).Y) // horizontal roads
                    {
                        img.Source = new BitmapImage(new Uri(@"\Images\Pieces\Road\Road" + App.Me.Color + "1.png", UriKind.Relative));
                        img.SetValue(Canvas.LeftProperty, closestPoints.ElementAt(1).X - 25);
                        img.SetValue(Canvas.TopProperty, closestPoints.ElementAt(1).Y - 50);
                        if (closestPoints.ElementAt(0).X < closestPoints.ElementAt(1).X)
                        {
                            img.SetValue(Canvas.LeftProperty, closestPoints.ElementAt(0).X - 25);
                            img.SetValue(Canvas.TopProperty, closestPoints.ElementAt(0).Y - 50);
                        }
                        
                        
                    }
                    else if (closestPoints.ElementAt(0).Y < closestPoints.ElementAt(1).Y) // klik dicht bij laagste punt
                    {
                        if (closestPoints.ElementAt(0).X > closestPoints.ElementAt(1).X)
                        {
                            img.Source = new BitmapImage(new Uri(@"\Images\Pieces\Road\Road" + App.Me.Color + "3.png", UriKind.Relative));
                            img.SetValue(Canvas.LeftProperty, closestPoints.ElementAt(0).X - 55);
                            img.SetValue(Canvas.TopProperty, closestPoints.ElementAt(0).Y - 25);
                        }
                        else
                        {
                            img.Source = new BitmapImage(new Uri(@"\Images\Pieces\Road\Road" + App.Me.Color + "2.png", UriKind.Relative));
                            img.SetValue(Canvas.LeftProperty, closestPoints.ElementAt(0).X - 40);
                            img.SetValue(Canvas.TopProperty, closestPoints.ElementAt(0).Y - 25);
                        }
                    }
                    else if (closestPoints.ElementAt(0).Y > closestPoints.ElementAt(1).Y) // klik dicht bij hoogste punt
                    {
                        if (closestPoints.ElementAt(0).X < closestPoints.ElementAt(1).X)
                        {
                            img.Source = new BitmapImage(new Uri(@"\Images\Pieces\Road\Road" + App.Me.Color + "3.png", UriKind.Relative));
                            img.SetValue(Canvas.LeftProperty, closestPoints.ElementAt(1).X - 55);
                            img.SetValue(Canvas.TopProperty, closestPoints.ElementAt(1).Y - 25);
                        }
                        else
                        {
                            img.Source = new BitmapImage(new Uri(@"\Images\Pieces\Road\Road" + App.Me.Color + "2.png", UriKind.Relative));
                            img.SetValue(Canvas.LeftProperty, closestPoints.ElementAt(1).X - 40);
                            img.SetValue(Canvas.TopProperty, closestPoints.ElementAt(1).Y - 25);
                        }
                    }
                    Can.Children.Add(img);
                    _buildStatus = BuildStatusEnum.None;
                }
            }
        }

        private double NotReallyDistanceButShouldDo(Point source, Point target)
        {
            return Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2);
        }

        private void Fin_Click(object sender, EventArgs e)
        {
            _buildStatus = BuildStatusEnum.None; // redundant
            AppBar.IsVisible = false;
            App.Client.ChangeTurnAsync(App.LobbyRoom);
        }

        private void Build_Click(object sender, EventArgs e)
        {
            if (App.Me.MyTurn)
            {
                BuildPopup.IsOpen = true;
            }
        }

        private void Buy_Road_Click(object sender, RoutedEventArgs e)
        {

            if (App.Me.Wood >= 1 && App.Me.Brick >= 1)
            {
                --App.Me.Wood;
                --App.Me.Brick;
                _buildStatus = BuildStatusEnum.Road;
                BuildPopup.IsOpen = false;
            }
            MessageBox.Show("error: resources");
        }

        private void Buy_Settlement_Click(object sender, RoutedEventArgs e)
        {
            if (App.Me.Wood >= 1 && App.Me.Brick >= 1 && App.Me.Wheat >= 1 && App.Me.Sheep >= 1)
            {
                --App.Me.Wood;
                --App.Me.Brick;
                --App.Me.Wheat;
                --App.Me.Sheep;
                _buildStatus = BuildStatusEnum.Settlement;
                BuildPopup.IsOpen = false;
            }
            MessageBox.Show("error: resources");
        }

        private void Buy_City_Click(object sender, RoutedEventArgs e)
        {
            if (App.Me.Wheat >= 2 && App.Me.IronOre >= 3)
            {
                App.Me.Wheat -= 2;
                App.Me.IronOre -= 3;
                _buildStatus = BuildStatusEnum.City;
                BuildPopup.IsOpen = false;
            }
            MessageBox.Show("error: resources");
        }

        private void Buy_Dev_Click(object sender, RoutedEventArgs e)
        {
            if (App.Me.Wheat >= 1 && App.Me.IronOre >= 1 && App.Me.Sheep >= 1)
            {
                App.Me.Wheat -= 1;
                App.Me.IronOre -= 1;
                App.Me.Sheep -= 1;
                _buildStatus = BuildStatusEnum.None;
                BuildPopup.IsOpen = false;
            }
            MessageBox.Show("error: resources");

        }
    }
}