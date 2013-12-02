using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using PhoneApp.ServiceReference;
using Point = System.Windows.Point;

namespace PhoneApp.Views
{
    public partial class GamePage : PhoneApplicationPage
    {
        private readonly int[] _numArray;
        private readonly string[] _hexArray;
        private readonly double[,] _hexPosition;
        private TileUserContol[] _board;
        private readonly List<Point> _settlementPoints;
        private readonly DispatcherTimer _pollTimer;
        private readonly Random _r;
        private int _dice;
        private bool _haveRolled;
        private BuildStatusEnum _buildStatus;
        private GameState _gameState;

        private bool _placedHouse;
        private bool _placedRoad;

        public GamePage()
        {
            InitializeComponent();
            _buildStatus = BuildStatusEnum.None;
            _gameState = GameState.FirstTurn;
            BuildPopup.Width = Application.Current.Host.Content.ActualWidth;
            _r = new Random();
            _pollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
            # region stuff
            _numArray = new[] { 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12 };
            Shuffle.ShuffleIt(_numArray, App.LobbyRoom.TheLobby.LobbyId); //lobbyId is random seed
            _hexArray = new[]
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
                            {140, 320}
                };


            _settlementPoints = new List<Point>
                {
                    new Point(165, 10),
                    new Point(215, 10),
                    new Point(95, 50),
                    new Point(145, 50),
                    new Point(235, 50),
                    new Point(285, 50),
                    new Point(25, 90),
                    new Point(75, 90),
                    new Point(165, 90),
                    new Point(215, 90),
                    new Point(305, 90),
                    new Point(355, 90),
                    new Point(5, 130),
                    new Point(95, 130),
                    new Point(145, 130),
                    new Point(235, 130),
                    new Point(285, 130),
                    new Point(375, 130),
                    new Point(25, 170),
                    new Point(75, 170),
                    new Point(165, 170),
                    new Point(215, 170),
                    new Point(305, 170),
                    new Point(355, 170),
                    new Point(5, 210),
                    new Point(95, 210),
                    new Point(145, 210),
                    new Point(235, 210),
                    new Point(285, 210),
                    new Point(375, 210),
                    new Point(25, 250),
                    new Point(75, 250),
                    new Point(165, 250),
                    new Point(215, 250),
                    new Point(305, 250),
                    new Point(355, 250),
                    new Point(5, 290),
                    new Point(95, 290),
                    new Point(145, 290),
                    new Point(235, 290),
                    new Point(285, 290),
                    new Point(375, 290),
                    new Point(25, 330),
                    new Point(75, 330),
                    new Point(165, 330),
                    new Point(215, 330),
                    new Point(305, 330),
                    new Point(355, 330),
                    new Point(95, 370),
                    new Point(145, 370),
                    new Point(235, 370),
                    new Point(285, 370),
                    new Point(165, 410),
                    new Point(215, 410)
                };
            # endregion
            GenGameMap();
            _pollTimer.Tick += pollTimer_Tick;
            _pollTimer.Start();

        }

        public int Dice
        {
            get
            {
                return _dice;
            }
            set
            {
                _dice = _r.Next(2, 12);
                MessageBox.Show(_dice.ToString());
                App.Client.UpdateGameAsync(App.LobbyRoom);
            }
        }

        void pollTimer_Tick(object sender, EventArgs e)
        {
            App.Client.GetALobbyRoomCompleted += Client_GetALobbyRoomCompleted;
            App.Client.GetALobbyRoomAsync(App.LobbyRoom.TheLobby);
        }

        void Client_GetALobbyRoomCompleted(object sender, GetALobbyRoomCompletedEventArgs e)
        {
            App.LobbyRoom = e.Result;
            App.Me = (from player in e.Result.PlayerList
                      where player.PlayerId == App.Me.PlayerId
                      select player).First();
            if (App.Me.MyTurn && !_haveRolled)
            {
                _haveRolled = true;
                ApplicationBar.IsVisible = true;
                Dice = 0; // any set will give new random num
                if (_gameState == GameState.FirstTurn || _gameState == GameState.SecondTurn)
                {
                    MessageBox.Show("Place a settlement and a road");
                    _placedHouse = false;
                    _placedRoad = false;
                }
            }
            UpdateGameMap();
        }

        public void GenGameMap()
        {
            _board = new TileUserContol[19];

            for (int i = 0; i < _hexPosition.Length / 2; i++)
            {
                _board[i] = new TileUserContol(_hexArray[i], _numArray[i]);
                _board[i].SetValue(Canvas.LeftProperty, _hexPosition[i, 0]);
                _board[i].SetValue(Canvas.TopProperty, _hexPosition[i, 1]);
                Can.Children.Add(_board[i]);
            }
        }

        public void UpdateGameMap()
        {
            Can.Children.Clear();


            foreach (var tile in _board)
            {
                Can.Children.Add(tile);
            }
            foreach (var road in App.LobbyRoom.TheLobby.Roads)
            {
                var img = new Image();
                img.Source = new BitmapImage(new Uri(road.ImageUrl, UriKind.Relative));
                img.SetValue(Canvas.LeftProperty, (double)road.Position.x);
                img.SetValue(Canvas.TopProperty, (double)road.Position.y);
                Can.Children.Add(img);

            }
            foreach (var settlement in App.LobbyRoom.TheLobby.Settlements)
            {
                var img = new Image();
                img.Source = new BitmapImage(new Uri(settlement.ImageUrl, UriKind.Relative));
                img.SetValue(Canvas.LeftProperty, (double)settlement.Position.x);
                img.SetValue(Canvas.TopProperty, (double)settlement.Position.y);
                Can.Children.Add(img);
            }
        }

        private void Can_Tap(object sender, System.Windows.Input.GestureEventArgs e) // todo: opkuisen dubble code verwijderen 
        {
            Point p = e.GetPosition(Can);
            var closest2Points = _settlementPoints.Where(point => point != p).
                                              OrderBy(point => NotReallyDistanceButShouldDo(p, point)).
                                              Take(2).ToList();

            if (App.Me.MyTurn && _buildStatus != BuildStatusEnum.None || (_gameState == GameState.FirstTurn || _gameState == GameState.SecondTurn) && App.Me.MyTurn)
            {
                if (_buildStatus == BuildStatusEnum.Settlement || (_gameState == GameState.FirstTurn || _gameState == GameState.SecondTurn) && _placedHouse == false)
                {
                    _placedHouse = true;
                    if (App.LobbyRoom.TheLobby.Settlements == null)
                    {
                        App.LobbyRoom.TheLobby.Settlements = new ObservableCollection<OSettlement>();
                    }
                    if (App.LobbyRoom.TheLobby.Settlements.All(settle => new Point() { X = settle.Position.x, Y = settle.Position.y } != closest2Points.First()))
                    {
                        var settl = new OSettlement
                            {
                                ImageUrl = @"\Images\Pieces\Settlement\Settlement" + App.Me.Color + ".png",
                                Position =
                                    new ServiceReference.Point()
                                        {
                                            x = (int)closest2Points.First().X - 50,
                                            y = (int)closest2Points.First().Y - 50
                                        },
                                Owner = App.Me,
                                RoadId = -1,
                                Upgraded = false
                            };

                        var img = new Image { Source = new BitmapImage(new Uri(settl.ImageUrl, UriKind.Relative)) };
                        img.SetValue(Canvas.LeftProperty, (double)settl.Position.x);
                        img.SetValue(Canvas.TopProperty, (double)settl.Position.y);

                        Can.Children.Add(img);
                        _buildStatus = BuildStatusEnum.None;

                        App.LobbyRoom.TheLobby.Settlements.Add(settl);

                    }
                }
                else if (_buildStatus == BuildStatusEnum.City)
                {
                    foreach (var settle in App.LobbyRoom.TheLobby.Settlements)
                    {
                        if (new Point() { X = settle.Position.x + 50, Y = settle.Position.y + 50 } == closest2Points.First())
                        {
                            settle.Upgraded = true;
                            settle.ImageUrl = @"\Images\Pieces\City\City" + App.Me.Color + ".png";

                            var img = new Image { Source = new BitmapImage(new Uri(settle.ImageUrl, UriKind.Relative)) };
                            img.SetValue(Canvas.LeftProperty, (double)settle.Position.x);
                            img.SetValue(Canvas.TopProperty, (double)settle.Position.y);

                            Can.Children.Add(img);
                            _buildStatus = BuildStatusEnum.None;
                            return;
                        }
                    }
                }
                else if (_buildStatus == BuildStatusEnum.Road || (_gameState == GameState.FirstTurn || _gameState == GameState.SecondTurn) && _placedRoad == false)
                {
                    _placedRoad = true;
                    var road1 = new ORoad() { Owner = App.Me, RoadId = -1 };

                    if (closest2Points.ElementAt(0).Y == closest2Points.ElementAt(1).Y) // horizontal roads
                    {
                        road1.ImageUrl = @"\Images\Pieces\Road\Road" + App.Me.Color + "1.png";
                        road1.Position = new ServiceReference.Point()
                        {
                            x = (int)closest2Points.ElementAt(1).X - 25,
                            y = (int)closest2Points.ElementAt(1).Y - 50
                        };

                        if (closest2Points.ElementAt(0).X < closest2Points.ElementAt(1).X)
                        {
                            road1.Position = new ServiceReference.Point()
                            {
                                x = (int)closest2Points.ElementAt(0).X - 25,
                                y = (int)closest2Points.ElementAt(0).Y - 50
                            };
                        }
                    }
                    else if (closest2Points.ElementAt(0).Y < closest2Points.ElementAt(1).Y) // klik dicht bij laagste punt
                    {
                        if (closest2Points.ElementAt(0).X > closest2Points.ElementAt(1).X)
                        {
                            road1.ImageUrl = @"\Images\Pieces\Road\Road" + App.Me.Color + "3.png";
                            road1.Position = new ServiceReference.Point() { x = (int)closest2Points.ElementAt(0).X - 55, y = (int)closest2Points.ElementAt(0).Y - 25 };
                        }
                        else
                        {
                            road1.ImageUrl = @"\Images\Pieces\Road\Road" + App.Me.Color + "2.png";
                            road1.Position = new ServiceReference.Point() { x = (int)closest2Points.ElementAt(0).X - 40, y = (int)closest2Points.ElementAt(0).Y - 25 };
                        }
                    }
                    else if (closest2Points.ElementAt(0).Y > closest2Points.ElementAt(1).Y) // klik dicht bij hoogste punt
                    {
                        if (closest2Points.ElementAt(0).X < closest2Points.ElementAt(1).X)
                        {
                            road1.ImageUrl = @"\Images\Pieces\Road\Road" + App.Me.Color + "3.png";
                            road1.Position = new ServiceReference.Point() { x = (int)closest2Points.ElementAt(1).X - 55, y = (int)closest2Points.ElementAt(1).Y - 25 };
                        }
                        else
                        {
                            road1.ImageUrl = @"\Images\Pieces\Road\Road" + App.Me.Color + "2.png";
                            road1.Position = new ServiceReference.Point() { x = (int)closest2Points.ElementAt(1).X - 40, y = (int)closest2Points.ElementAt(1).Y - 25 };
                        }
                    }

                    var img = new Image { Source = new BitmapImage(new Uri(road1.ImageUrl, UriKind.Relative)) };
                    img.SetValue(Canvas.LeftProperty, (double)road1.Position.x);
                    img.SetValue(Canvas.TopProperty, (double)road1.Position.y);
                    Can.Children.Add(img);
                    _buildStatus = BuildStatusEnum.None;

                    if (App.LobbyRoom.TheLobby.Roads == null)
                    {
                        App.LobbyRoom.TheLobby.Roads = new ObservableCollection<ORoad>();
                    }
                    App.LobbyRoom.TheLobby.Roads.Add(road1);
                }
                App.Client.UpdateGameAsync(App.LobbyRoom);
            }
        }

        private double NotReallyDistanceButShouldDo(Point source, Point target)
        {
            return Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2);
        } //Maths :o

        private void Fin_Click(object sender, EventArgs e)
        {
            if (_buildStatus == BuildStatusEnum.None && _placedRoad && _placedRoad)
            {
                ApplicationBar.IsVisible = false;
                _haveRolled = false;
                if ((int)_gameState <= 1)
                {
                    _gameState = _gameState + 1;
                    
                }
                App.Client.ChangeTurnAsync(App.LobbyRoom);
            }
            else
            {
                MessageBox.Show("error" + ": stil need to place: " + _buildStatus.ToString());
            }
        }

        private void Build_Click(object sender, EventArgs e)
        {
            if (App.Me.MyTurn)
            {
                BuildPopup.IsOpen = !BuildPopup.IsOpen;
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
                App.Client.UpdatePlayerAsync(App.Me);
            }
            else
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
                App.Client.UpdatePlayerAsync(App.Me);
            }
            else
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
                App.Client.UpdatePlayerAsync(App.Me);
            }
            else
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
                App.Client.UpdatePlayerAsync(App.Me);
            }
            else
                MessageBox.Show("error: resources");

        }
    }
}