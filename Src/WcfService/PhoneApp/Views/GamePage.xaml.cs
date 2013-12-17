using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

        void _pollTimer2_Tick(object sender, EventArgs e)
        {
            App.Client.GetALobbyRoomAsync(App.LobbyRoom.TheLobby);
        }

        public int GetTotalResources(OPlayer p)
        {
            return p.IronOre + p.Wheat + p.Wood + p.Brick + p.Sheep;
        }

        void Client_GetALobbyRoomCompleted(object sender, GetALobbyRoomCompletedEventArgs e)
        {
            App.LobbyRoom = e.Result;
            Player1Grid.DataContext = App.Me;
            App.Me = (from player in e.Result.PlayerList
                      where player.PlayerId == App.Me.PlayerId
                      select player).First();
            var plList = e.Result.PlayerList.Where(p => p.PlayerId != App.Me.PlayerId).ToList();

            Playerstk1.DataContext = plList.ElementAt(0);
            TotalResources1.Text = "# resources: " + GetTotalResources(plList.ElementAt(0)).ToString();
            Playerstk2.DataContext = plList.ElementAt(1);
            TotalResources2.Text = "# resources: " + GetTotalResources(plList.ElementAt(1)).ToString();
            Playerstk3.DataContext = plList.ElementAt(2);
            TotalResources3.Text = "# resources: " + GetTotalResources(plList.ElementAt(2)).ToString();


            if (App.Me.MyTurn && !_haveRolled)
            {
                _haveRolled = true;
                ApplicationBar.IsVisible = true;

                if (_gameState == GameState.FirstTurn || _gameState == GameState.SecondTurn)
                {
                    MessageBox.Show("Place a settlement and a road");
                    _placedHouse = false;
                    _placedRoad = false;
                }
                else
                {
                    App.LobbyRoom.TheLobby.DiceNum = _r.Next(2, 12);
                    MessageBox.Show(App.LobbyRoom.TheLobby.DiceNum.ToString());
                    App.LobbyRoom.TheLobby.IsUpdate = true;
                    App.Client.UpdateGameAsync(App.LobbyRoom);
                }
            }
            try
            {
                if (_currentPlayerId != e.Result.PlayerList.First(p => p.MyTurn).PlayerId && _gameState != GameState.NormalPlay)
                {
                    _currentPlayerId = e.Result.PlayerList.First(p => p.MyTurn).PlayerId;
                }

                if (_currentPlayerId != e.Result.PlayerList.First(p => p.MyTurn).PlayerId && _gameState == GameState.NormalPlay && App.LobbyRoom.TheLobby.IsUpdate)
                {
                    DiceBorder.Background = new SolidColorBrush(Colors.White);
                    _currentPlayerId = e.Result.PlayerList.First(p => p.MyTurn).PlayerId;
                    GetResources();
                    GetVictory();
                    MessageBox.Show(App.Me.IronOre.ToString() + App.Me.Sheep.ToString() +
                                    App.Me.Wheat.ToString() + App.Me.Wood.ToString() + App.Me.Brick.ToString());
                }
            }
            catch (Exception)
            {

            }

            if (App.LobbyRoom.TheLobby.DiceNum % 2 == 0)
            {
                DiceImg1.Source = new BitmapImage(new Uri(@"\Images\Dice\" + App.LobbyRoom.TheLobby.DiceNum / 2 + ".png", UriKind.Relative));
                DiceImg2.Source = new BitmapImage(new Uri(@"\Images\Dice\" + App.LobbyRoom.TheLobby.DiceNum / 2 + ".png", UriKind.Relative));
            }
            else
            {
                DiceImg1.Source = new BitmapImage(new Uri(@"\Images\Dice\" + (App.LobbyRoom.TheLobby.DiceNum / 2 + 1).ToString() + ".png", UriKind.Relative));
                DiceImg2.Source = new BitmapImage(new Uri(@"\Images\Dice\" + App.LobbyRoom.TheLobby.DiceNum / 2 + ".png", UriKind.Relative));
            }

            UpdateGameMap();
        }

        public void GenGameMap()
        {
            _board = new TileUserContol[19];

            for (int i = 0; i < _hexPosition.Length / 2; i++)
            {
                _board[i] = new TileUserContol(_hexArray[i], _numArray[i], new Point() { X = _hexPosition[i, 0], Y = _hexPosition[i, 1] });
                _board[i].SetValue(Canvas.LeftProperty, _board[i].Position.X);
                _board[i].SetValue(Canvas.TopProperty, _board[i].Position.Y);
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
                img.SetValue(Canvas.LeftProperty, (double)road.Position.x + road.ShiftX);
                img.SetValue(Canvas.TopProperty, (double)road.Position.y + road.ShiftY);
                Can.Children.Add(img);
            }
            foreach (var settlement in App.LobbyRoom.TheLobby.Settlements)
            {
                var img = new Image();
                img.Source = new BitmapImage(new Uri(settlement.ImageUrl, UriKind.Relative));
                img.SetValue(Canvas.LeftProperty, (double)settlement.Position.x - 50);
                img.SetValue(Canvas.TopProperty, (double)settlement.Position.y - 50);
                Can.Children.Add(img);
            }
        }

        private void Can_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Point p = e.GetPosition(Can);
            var closest2Points = _settlementPoints.Where(point => point != p).
                                              OrderBy(point => NotReallyDistanceButShouldDo(p, point)).
                                              Take(2).ToList();

            if (App.Me.MyTurn && _buildStatus != BuildStatusEnum.None || (_gameState == GameState.FirstTurn || _gameState == GameState.SecondTurn) && App.Me.MyTurn)
            {
                if (_buildStatus == BuildStatusEnum.Settlement || (_gameState == GameState.FirstTurn || _gameState == GameState.SecondTurn) && _placedHouse == false)
                {

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
                                            x = (int)closest2Points.First().X,
                                            y = (int)closest2Points.First().Y
                                        },
                                Owner = App.Me,
                                RoadId = -1,
                                Upgraded = false
                            };

                        _buildStatus = BuildStatusEnum.None;
                        App.Me.VictoryPoints++;
                        App.Client.UpdatePlayerAsync(App.Me);
                        App.LobbyRoom.TheLobby.Settlements.Add(settl);
                        App.Client.UpdateGameCompleted += ClientOnUpdateGameCompleted; // get update wanneer het is geupdate
                        App.Client.UpdateGameAsync(App.LobbyRoom);
                        App.LobbyRoom.TheLobby.Settlements.RemoveAt(App.LobbyRoom.TheLobby.Settlements.Count - 1);
                        _placedHouse = true;
                    }
                }
                else if (_buildStatus == BuildStatusEnum.City)
                {
                    foreach (var settle in App.LobbyRoom.TheLobby.Settlements)
                    {
<<<<<<< HEAD
                        if (new Point() { X = settle.Position.x + 50, Y = settle.Position.y + 50 } == closest2Points.First())
                        {
                            settle.Upgraded = true;
                            settle.ImageUrl = @"\Images\Pieces\City\City" + App.Me.Color + ".png";

                            var img = new Image { Source = new BitmapImage(new Uri(settle.ImageUrl, UriKind.Relative)) };
                            img.SetValue(Canvas.LeftProperty, (double)settle.Position.x);
                            img.SetValue(Canvas.TopProperty, (double)settle.Position.y);

                            Can.Children.Add(img);
=======
                        if (new Point() { X = settle.Position.x, Y = settle.Position.y } == closest2Points.First())
                        {
                            App.Me.VictoryPoints++;
                            App.Client.UpdatePlayerAsync(App.Me);
                            settle.Upgraded = true;
                            settle.ImageUrl = @"\Images\Pieces\City\City" + App.Me.Color + ".png";
                            App.Client.UpdateGameAsync(App.LobbyRoom);
>>>>>>> 568ca47... Foto's toegevoegd
                            _buildStatus = BuildStatusEnum.None;
                            return;
                        }
                    }
                }
                else if (_buildStatus == BuildStatusEnum.Road || (_gameState == GameState.FirstTurn || _gameState == GameState.SecondTurn) && _placedRoad == false)
                {
                    _placedRoad = true;
                    var road1 = new ORoad() { Owner = App.Me, RoadId = -1 };
<<<<<<< HEAD

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
=======

                    var neighborRoad =
                        closest2Points.Any(
                            point =>
                            App.LobbyRoom.TheLobby.Roads.Any(
                                road =>
                                (new Point() { X = road.Position.x, Y = road.Position.y } == point ||
                                 new Point() { X = road.Position2.x, Y = road.Position2.y } == point) &&
                                road.Owner.PlayerId == App.Me.PlayerId));

                    var neighborSettlement =
                        closest2Points.Any(
                            point =>
                            App.LobbyRoom.TheLobby.Settlements.Any(
                                settle => new Point() { X = settle.Position.x, Y = settle.Position.y } == point
                                && settle.Owner.PlayerId == App.Me.PlayerId)); //check of straat naast settlement ligt

                    if (!neighborSettlement && !neighborRoad)
                    {
                        _placedRoad = false;
                        return;
                    }

                    if (closest2Points.ElementAt(0).Y == closest2Points.ElementAt(1).Y) // horizontal roads
                    {

                        road1.ImageUrl = @"\Images\Pieces\Road\Road" + App.Me.Color + "1.png";
                        road1.Position = new ServiceReference.Point()
                        {
                            x = (int)closest2Points.ElementAt(1).X,
                            y = (int)closest2Points.ElementAt(1).Y
                        };
                        road1.Position2 = new ServiceReference.Point()
                        {
                            x = (int)closest2Points.ElementAt(0).X,
                            y = (int)closest2Points.ElementAt(0).Y
                        };

                        if (closest2Points.ElementAt(0).X < closest2Points.ElementAt(1).X)
                        {
                            road1.Position = new ServiceReference.Point()
                            {
                                x = (int)closest2Points.ElementAt(0).X,
                                y = (int)closest2Points.ElementAt(0).Y
                            };
                            road1.Position2 = new ServiceReference.Point()
                            {
                                x = (int)closest2Points.ElementAt(1).X,
                                y = (int)closest2Points.ElementAt(1).Y
                            };
                        }
                        road1.ShiftX = -25;
                        road1.ShiftY = -50;
>>>>>>> 568ca47... Foto's toegevoegd
                    }
                    else if (closest2Points.ElementAt(0).Y < closest2Points.ElementAt(1).Y) // klik dicht bij laagste punt
                    {
                        if (closest2Points.ElementAt(0).X > closest2Points.ElementAt(1).X)
                        {
                            road1.ImageUrl = @"\Images\Pieces\Road\Road" + App.Me.Color + "3.png";
<<<<<<< HEAD
                            road1.Position = new ServiceReference.Point() { x = (int)closest2Points.ElementAt(0).X - 55, y = (int)closest2Points.ElementAt(0).Y - 25 };
=======
                            road1.Position = new ServiceReference.Point()
                                {
                                    x = (int)closest2Points.ElementAt(0).X,
                                    y = (int)closest2Points.ElementAt(0).Y
                                };
                            road1.Position2 = new ServiceReference.Point()
                            {
                                x = (int)closest2Points.ElementAt(1).X,
                                y = (int)closest2Points.ElementAt(1).Y
                            };
                            road1.ShiftX = -55;
>>>>>>> 568ca47... Foto's toegevoegd
                        }
                        else
                        {
                            road1.ImageUrl = @"\Images\Pieces\Road\Road" + App.Me.Color + "2.png";
<<<<<<< HEAD
                            road1.Position = new ServiceReference.Point() { x = (int)closest2Points.ElementAt(0).X - 40, y = (int)closest2Points.ElementAt(0).Y - 25 };
=======
                            road1.Position = new ServiceReference.Point()
                                {
                                    x = (int)closest2Points.ElementAt(0).X,
                                    y = (int)closest2Points.ElementAt(0).Y
                                };
                            road1.Position2 = new ServiceReference.Point()
                                {
                                    x = (int)closest2Points.ElementAt(1).X,
                                    y = (int)closest2Points.ElementAt(1).Y
                                };
                            road1.ShiftX = -40;
>>>>>>> 568ca47... Foto's toegevoegd
                        }
                        road1.ShiftY = -25;

                    }
                    else if (closest2Points.ElementAt(0).Y > closest2Points.ElementAt(1).Y) // klik dicht bij hoogste punt
                    {
                        if (closest2Points.ElementAt(0).X < closest2Points.ElementAt(1).X)
                        {
                            road1.ImageUrl = @"\Images\Pieces\Road\Road" + App.Me.Color + "3.png";
<<<<<<< HEAD
                            road1.Position = new ServiceReference.Point() { x = (int)closest2Points.ElementAt(1).X - 55, y = (int)closest2Points.ElementAt(1).Y - 25 };
=======
                            road1.Position = new ServiceReference.Point()
                                {
                                    x = (int)closest2Points.ElementAt(1).X,
                                    y = (int)closest2Points.ElementAt(1).Y
                                };
                            road1.Position2 = new ServiceReference.Point()
                            {
                                x = (int)closest2Points.ElementAt(0).X,
                                y = (int)closest2Points.ElementAt(0).Y
                            };
                            road1.ShiftX = -55;
>>>>>>> 568ca47... Foto's toegevoegd
                        }
                        else
                        {
                            road1.ImageUrl = @"\Images\Pieces\Road\Road" + App.Me.Color + "2.png";
<<<<<<< HEAD
                            road1.Position = new ServiceReference.Point() { x = (int)closest2Points.ElementAt(1).X - 40, y = (int)closest2Points.ElementAt(1).Y - 25 };
=======
                            road1.Position = new ServiceReference.Point()
                                {
                                    x = (int)closest2Points.ElementAt(1).X,
                                    y = (int)closest2Points.ElementAt(1).Y
                                };
                            road1.Position2 = new ServiceReference.Point()
                            {
                                x = (int)closest2Points.ElementAt(0).X,
                                y = (int)closest2Points.ElementAt(0).Y
                            };
                            road1.ShiftX = -40;
>>>>>>> 568ca47... Foto's toegevoegd
                        }
                        road1.ShiftY = -25;
                    }
<<<<<<< HEAD

                    var img = new Image { Source = new BitmapImage(new Uri(road1.ImageUrl, UriKind.Relative)) };
                    img.SetValue(Canvas.LeftProperty, (double)road1.Position.x);
                    img.SetValue(Canvas.TopProperty, (double)road1.Position.y);
                    Can.Children.Add(img);
=======
>>>>>>> 568ca47... Foto's toegevoegd
                    _buildStatus = BuildStatusEnum.None;

                    if (App.LobbyRoom.TheLobby.Roads == null)
                    {
                        App.LobbyRoom.TheLobby.Roads = new ObservableCollection<ORoad>();
                    }
                    App.LobbyRoom.TheLobby.Roads.Add(road1);
<<<<<<< HEAD
                }
                App.Client.UpdateGameAsync(App.LobbyRoom);
=======
                    App.Client.UpdateGameCompleted += ClientOnUpdateGameCompleted;
                    App.Client.UpdateGameAsync(App.LobbyRoom);


                }
                UpdateGameMap();
>>>>>>> 568ca47... Foto's toegevoegd
            }
        }

        private void ClientOnUpdateGameCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            _pollTimer2_Tick(null, EventArgs.Empty);
            App.Client.UpdateGameCompleted -= ClientOnUpdateGameCompleted; // verwijder hendler zo dat deze niet altijd uitvoert als updateGame klaar is
        }

        private double NotReallyDistanceButShouldDo(Point source, Point target)
        {
            return Math.Pow(target.X - source.X, 2) + Math.Pow(target.Y - source.Y, 2);
        } //Maths :o

        private void Fin_Click(object sender, EventArgs e)
        {
<<<<<<< HEAD
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
=======
            if (_buildStatus == BuildStatusEnum.None && _placedRoad && _placedHouse)
            {
                _pollTimer2.Tick -= _pollTimer2_Tick;

                if ((int)_gameState <= 1)
                {
                    _gameState = _gameState + 1;
                }

                if (BuildPopup.IsOpen)
                {
                    BuildPopup.IsOpen = false;
                }

                ApplicationBar.IsVisible = false;

                App.LobbyRoom.TheLobby.IsUpdate = false;

                App.Client.UpdateGameCompleted += Client_UpdateGameCompleted;
                App.Client.UpdateGameAsync(App.LobbyRoom);

            }
            else
            {
                string message = "error";
                if (_buildStatus == BuildStatusEnum.None)
                {
                    if (_placedRoad)
                        message = " Road";

                    if (_placedHouse)
                        message = " Settlement";
                }
                else
                    message = _buildStatus.ToString();

                MessageBox.Show("error" + ": stil need to place: " + message);
            }
        }

        void Client_UpdateGameCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            App.Client.ChangeTurnAsync(App.LobbyRoom);
            App.Client.UpdateGameCompleted -= Client_UpdateGameCompleted;
        }

        void Client_ChangeTurnCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _haveRolled = false;
            App.Me.MyTurn = false;
            _pollTimer2.Tick += _pollTimer2_Tick;
>>>>>>> 568ca47... Foto's toegevoegd
        }

        private void Build_Click(object sender, EventArgs e)
        {
            if (App.Me.MyTurn)
<<<<<<< HEAD
            {
                BuildPopup.IsOpen = !BuildPopup.IsOpen;
            }
=======
                BuildPopup.IsOpen = !BuildPopup.IsOpen;
>>>>>>> 568ca47... Foto's toegevoegd
        }

        private void Buy_Road_Click(object sender, RoutedEventArgs e)
        {
            if (App.Me.Wood >= 1 && App.Me.Brick >= 1)
            {
                App.Me.Wood--;
                App.Me.Brick--;
                App.Client.UpdatePlayerAsync(App.Me);
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
                App.Me.Wood--;
                App.Me.Brick--;
                App.Me.Wheat--;
                App.Me.Sheep--;
                App.Client.UpdatePlayerAsync(App.Me);
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
                App.Client.UpdatePlayerAsync(App.Me);
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
                App.Me.Wheat--;
                App.Me.IronOre--;
                App.Me.Sheep--;
                App.Client.UpdatePlayerAsync(App.Me);
                _buildStatus = BuildStatusEnum.None;
                BuildPopup.IsOpen = false;
                App.Client.UpdatePlayerAsync(App.Me);
            }
            else
                MessageBox.Show("error: resources");
<<<<<<< HEAD
=======
        }
>>>>>>> 568ca47... Foto's toegevoegd

        private void GetResources()
        {
            var sTile = new List<string>();
            foreach (var oSettlement in App.LobbyRoom.TheLobby.Settlements.Where(s => s.Owner.PlayerId == App.Me.PlayerId))
            {
                sTile.AddRange(AddToList(oSettlement));
                if (oSettlement.Upgraded)
                {
                    sTile.AddRange(AddToList(oSettlement));
                }
            }

            foreach (var s in sTile.Where(s => s != null))
            {
                switch (s)
                {
                    case "Wheat":
                        App.Me.Wheat++;
                        break;
                    case "IronOre":
                        App.Me.IronOre++;
                        break;
                    case "Sheep":
                        App.Me.Sheep++;
                        break;
                    case "Brick":
                        App.Me.Brick++;
                        break;
                    case "Wood":
                        App.Me.Wood++;
                        break;
                }
            }
            App.Client.UpdatePlayerAsync(App.Me);
        }
        public List<string> AddToList(OSettlement oSettlement) // lelijke code
        {
            var sTile = new List<string>();
            try
            {
                sTile.Add(_board.Single(b => b.Position.X == oSettlement.Position.x - 25 && b.Position.Y == oSettlement.Position.y - 10 && b.TileNum == App.LobbyRoom.TheLobby.DiceNum).Resource);
            }
            catch (Exception)
            {

            }
            try
            {
                sTile.Add(_board.Single(b => b.Position.X == oSettlement.Position.x - 95 && b.Position.Y == oSettlement.Position.y - 50 && b.TileNum == App.LobbyRoom.TheLobby.DiceNum).Resource);
            }
            catch (Exception)
            {

            }
            try
            {
                sTile.Add(_board.Single(b => b.Position.X == oSettlement.Position.x - 25 && b.Position.Y == oSettlement.Position.y - 90 && b.TileNum == App.LobbyRoom.TheLobby.DiceNum).Resource);
            }
            catch (Exception)
            {

            }
            try
            {
                sTile.Add(_board.Single(b => b.Position.X == oSettlement.Position.x - 5 && b.Position.Y == oSettlement.Position.y - 50 && b.TileNum == App.LobbyRoom.TheLobby.DiceNum).Resource);
            }
            catch (Exception)
            {

            }
            try
            {
                sTile.Add(_board.Single(b => b.Position.X == oSettlement.Position.x - 75 && b.Position.Y == oSettlement.Position.y - 10 && b.TileNum == App.LobbyRoom.TheLobby.DiceNum).Resource);
            }
            catch
            {

            }
            try
            {
                sTile.Add(_board.Single(b => b.Position.X == oSettlement.Position.x - 75 && b.Position.Y == oSettlement.Position.y - 90 && b.TileNum == App.LobbyRoom.TheLobby.DiceNum).Resource);
            }
            catch (Exception)
            {

            }
            return sTile;
        }

        # region pinch
        // these two fields fully define the zoom state:
        private double TotalImageScale = 1d;
        private Point ImagePosition = new Point(0, 0);


        private const double MAX_IMAGE_ZOOM = 5;
        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;


        #region Event handlers

        /// <summary>
        /// Initializes the zooming operation
        /// </summary>
        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _oldFinger1 = e.GetPosition(Can, 0);
            _oldFinger2 = e.GetPosition(Can, 1);
            _oldScaleFactor = 1;
        }

        /// <summary>
        /// Computes the scaling and translation to correctly zoom around your fingers.
        /// </summary>
        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            var scaleFactor = e.DistanceRatio / _oldScaleFactor;
            if (!IsScaleValid(scaleFactor))
                return;

            var currentFinger1 = e.GetPosition(Can, 0);
            var currentFinger2 = e.GetPosition(Can, 1);

            var translationDelta = GetTranslationDelta(
                currentFinger1,
                currentFinger2,
                _oldFinger1,
                _oldFinger2,
                ImagePosition,
                scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            UpdateImageScale(scaleFactor);
            UpdateImagePosition(translationDelta);
        }

        /// <summary>
        /// Moves the image around following your finger.
        /// </summary>
        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            var translationDelta = new Point(e.HorizontalChange, e.VerticalChange);

            if (IsDragValid(1, translationDelta))
                UpdateImagePosition(translationDelta);
        }

        /// <summary>
        /// Resets the image scaling and position
        /// </summary>
        private void OnDoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            ResetImagePosition();
        }

        #endregion

        #region Utils

        /// <summary>
        /// Computes the translation needed to keep the image centered between your fingers.
        /// </summary>
        private Point GetTranslationDelta(
            Point currentFinger1, Point currentFinger2,
            Point oldFinger1, Point oldFinger2,
            Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(
             currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
             currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

            var newPos2 = new Point(
             currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
             currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

            var newPos = new Point(
                (newPos1.X + newPos2.X) / 2,
                (newPos1.Y + newPos2.Y) / 2);

            return new Point(
                newPos.X - currentPosition.X,
                newPos.Y - currentPosition.Y);
        }

        /// <summary>
        /// Updates the scaling factor by multiplying the delta.
        /// </summary>
        private void UpdateImageScale(double scaleFactor)
        {
            TotalImageScale *= scaleFactor;
            ApplyScale();
        }

        /// <summary>
        /// Applies the computed scale to the image control.
        /// </summary>
        private void ApplyScale()
        {
            ((CompositeTransform)Can.RenderTransform).ScaleX = TotalImageScale;
            ((CompositeTransform)Can.RenderTransform).ScaleY = TotalImageScale;
        }

        /// <summary>
        /// Updates the image position by applying the delta.
        /// Checks that the image does not leave empty space around its edges.
        /// </summary>
        private void UpdateImagePosition(Point delta)
        {
            var newPosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

            if (newPosition.X > 0) newPosition.X = 0;
            if (newPosition.Y > 0) newPosition.Y = 0;

            if ((Can.ActualWidth * TotalImageScale) + newPosition.X < Can.ActualWidth)
                newPosition.X = Can.ActualWidth - (Can.ActualWidth * TotalImageScale);

            if ((Can.ActualHeight * TotalImageScale) + newPosition.Y < Can.ActualHeight)
                newPosition.Y = Can.ActualHeight - (Can.ActualHeight * TotalImageScale);

            ImagePosition = newPosition;

            ApplyPosition();
        }

        /// <summary>
        /// Applies the computed position to the image control.
        /// </summary>
        private void ApplyPosition()
        {
            ((CompositeTransform)Can.RenderTransform).TranslateX = ImagePosition.X;
            ((CompositeTransform)Can.RenderTransform).TranslateY = ImagePosition.Y;
        }

        /// <summary>
        /// Resets the zoom to its original scale and position
        /// </summary>
        private void ResetImagePosition()
        {
            TotalImageScale = 1;
            ImagePosition = new Point(0, 0);
            ApplyScale();
            ApplyPosition();
        }

        /// <summary>
        /// Checks that dragging by the given amount won't result in empty space around the image
        /// </summary>
        private bool IsDragValid(double scaleDelta, Point translateDelta)
        {
            if (ImagePosition.X + translateDelta.X > 0 || ImagePosition.Y + translateDelta.Y > 0)
                return false;

            if ((Can.ActualWidth * TotalImageScale * scaleDelta) + (ImagePosition.X + translateDelta.X) < Can.ActualWidth)
                return false;

            if ((Can.ActualHeight * TotalImageScale * scaleDelta) + (ImagePosition.Y + translateDelta.Y) < Can.ActualHeight)
                return false;

            return true;
        }

        /// <summary>
        /// Tells if the scaling is inside the desired range
        /// </summary>
        private bool IsScaleValid(double scaleDelta)
        {
            return (TotalImageScale * scaleDelta >= 1) && (TotalImageScale * scaleDelta <= MAX_IMAGE_ZOOM);
        }

        #endregion
        #endregion

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit?", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
        }

        public void GetVictory()
        {
            if (App.LobbyRoom.PlayerList.Any(p => p.VictoryPoints >= 10))
            {
                _gameState = GameState.Finished;
                if (App.LobbyRoom.PlayerList.First(p => p.VictoryPoints >= 10).PlayerId == App.Me.PlayerId)
                {
                    (App.Current.RootVisual as PhoneApplicationFrame).Navigate(
                new Uri("/Views/WaitingPage.xaml", UriKind.Relative));
                }
                else
                {
                    (App.Current.RootVisual as PhoneApplicationFrame).Navigate(
                new Uri("/Views/WaitingPage.xaml", UriKind.Relative));
                }
            }
        }
    }
}