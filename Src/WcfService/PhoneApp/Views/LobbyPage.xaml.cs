﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using PhoneApp.ViewModels;

namespace PhoneApp.Views
{
    public partial class LobbyPage : PhoneApplicationPage
    {
        private LobbyPageViewModel vm;

        public LobbyPage()
        {
            InitializeComponent();
            vm = new LobbyPageViewModel();
            
            LobbyRefreshBtn_Click(null, null);
            
            //LobbyListBox.Items.Add("test");
        }

        private void LobbyRefreshBtn_Click(object sender, RoutedEventArgs e)
        {

            vm.PopDone += (s, ea) => this.DataContext = vm;
            vm.PopulateLobbyList();
        
        }

        private void LobbyListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            vm.JoinLobby((ServiceReference.OLobbyRoom)LobbyListBox.SelectedItem);
        }

        private void LobbyCreateNewBtn_Click(object sender, RoutedEventArgs e)
        {
            LobbyCreateNewBtn.IsEnabled = false;
            vm.CreateLobby();
        }
    }
}