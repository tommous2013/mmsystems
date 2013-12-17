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

        [DataMember]
        public bool IsWaitingForPlayers { get; set; }

        [DataMember]
        public bool IsUpdate { get; set; }

        [DataMember]
        public int DiceNum { get; set; }

        [DataMember]
        public List<ORoad> Roads { get; set; }

        [DataMember]
        public List<OSettlement> Settlements { get; set; }
            
        public override string ToString()
        {
            return LobbyName;
        }
    }
}