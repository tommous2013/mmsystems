using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfService.DTO
{
    [DataContract]
    public class ORoad
    {
        [DataMember]
        public int RoadId { get; set; }
        [DataMember]
        public String ImageUrl { get; set; }
        [DataMember]
        public Point Position { get; set; }
        [DataMember]
<<<<<<< HEAD
=======
        public Point Position2 { get; set; }
        [DataMember]
        public int ShiftX { get; set; }
        [DataMember]
        public int ShiftY { get; set; }
        [DataMember]
>>>>>>> 568ca47... Foto's toegevoegd
        public OPlayer Owner { get; set; }
    }
}