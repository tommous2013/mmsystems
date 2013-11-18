using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
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

        public GamePage()
        {
            InitializeComponent();
            _numArray = new int[] { 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12 };
            Shuffle.ShuffleIt(_numArray, App.LobbyRoom.TheLobby.LobbyId);
            _hexArray = new string[]
                {
                    "brick", "brick", "brick", "brick", "ironOre", "ironOre", "ironOre", "sheep", "sheep", "sheep", "sheep"
                    , "wood", "wood", "wood", "wood", "grain", "grain", "grain", "grain"
                };
            Shuffle.ShuffleIt(_hexArray, App.LobbyRoom.TheLobby.LobbyId);

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
    }
}