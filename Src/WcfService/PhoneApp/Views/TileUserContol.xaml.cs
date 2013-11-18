using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp.ViewModels;

namespace PhoneApp.Views
{
    public partial class TileUserContol : UserControl
    {
        public TileUserContol(string resource, int num)
        {
            InitializeComponent();
            HexSource.Source = new BitmapImage(new Uri("/Images/Map/" + resource + ".png", UriKind.Relative));
            NumSource.Source = new BitmapImage(new Uri("/Images/Nums/" + num + ".png", UriKind.Relative));
            TileNum = num;
            Resource = resource;
        }

        private int _tileNum;
        public int TileNum
        {
            get { return _tileNum; }
            set
            {
                _tileNum = value;
                //OnPropertyChanged("TileNum");
            }
        }

        private string _resource;
        public string Resource
        {
            get { return _resource; }
            set
            {
                _resource = value;
                //OnPropertyChanged("Resource");
            }
        }
    }
}
