using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfService.DTO
{
    [DataContract]
    public class OPlayer
    {
        [DataMember]
        public int PlayerId { get; set; }
        [DataMember]
        public string PlayerName { get; set; }
        [DataMember]
        public string Color { get; set; }
        [DataMember]
        public bool MyTurn { get; set; }
        [DataMember]
        public int Sheep { get; set; }
        [DataMember]
        public int IronOre { get; set; }
        [DataMember]
        public int Wood { get; set; }
        [DataMember]
        public int Wheat { get; set; }
        [DataMember]
        public int Brick { get; set; }
        [DataMember]
        public int VictoryPoints { get; set; }
    }
}