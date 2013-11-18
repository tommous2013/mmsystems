using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfService.DTO
{
    [DataContract]
    public class OLobbyRoom
    {
        [DataMember]
        public OLobby TheLobby { get; set; }

        [DataMember]
        public List<OPlayer> PlayerList { get; set; }

        [DataMember]
        public OPlayer HostPlayer { get; set; }

    }
}