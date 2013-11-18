using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WcfService.DTO
{
    [DataContract]
    public class OLobby
    {
        [DataMember]
        public int LobbyId { get; set; }

        [DataMember]
        public string LobbyName { get; set; }

        public override string ToString()
        {
            return LobbyName;
        }
    }
}