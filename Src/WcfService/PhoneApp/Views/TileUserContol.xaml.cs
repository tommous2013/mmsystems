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
            HexSource = new BitmapImage(new Uri("Images/Resources/" + resource + ".png", UriKind.Relative));
            NumSource = new BitmapImage(new Uri("Images/Numbers/" + num + ".png", UriKind.Relative));
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

        private BitmapImage _hexSource;
        public BitmapImage HexSource
        {
            get { return _hexSource; }
            set
            {
                _hexSource = value;
                OnPropertyChanged("HexSource");
            }
        }

        private BitmapImage _numSource;
        public BitmapImage NumSource
        {
            get { return _numSource; }
            set
            {
                _numSource = value;
                OnPropertyChanged("NumSource");
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
