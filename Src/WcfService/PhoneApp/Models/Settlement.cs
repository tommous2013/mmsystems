using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using PhoneApp.ServiceReference;

namespace PhoneApp.Models
{
    class Settlement
    {
        public Image Image { get; set; }
        public bool Upgraded { get; set; }
        public OPlayer Owner { get; set; }
        public Point Position { get; set; }
    }
}
