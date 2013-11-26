using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfService.DTO;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private DataClassesDataContext dc;
        public Service1()
        {
            dc = new DataClassesDataContext();
        }

        public OLobbyRoom CreateLobby(OPlayer host)
        {
            var maxLobId = (from lobMaxId in dc.Lobbies
                            select lobMaxId.LobbyId).Max();

            Lobby lobby = new Lobby();
            lobby.LobbyName = host.PlayerName + "'s lobby";
            lobby.LobbyId = ++maxLobId;
            lobby.IsWaitingForPlayers = true;

            PlayLobby pLobby = new PlayLobby();
            pLobby.HostPlayer = host.PlayerId;
            pLobby.LobbyId = lobby.LobbyId;
            pLobby.PlayerId = host.PlayerId;

            dc.PlayLobbies.InsertOnSubmit(pLobby);
            dc.Lobbies.InsertOnSubmit(lobby);
            dc.SubmitChanges();

            return new OLobbyRoom()
                {
                    HostPlayer = host,
                    PlayerList = new List<OPlayer>() { host },
                    TheLobby = new OLobby() { LobbyId = lobby.LobbyId, LobbyName = lobby.LobbyName }
                };
        }

        public OLobbyRoom GetALobbyRoom(OLobby lob)
        {
            OLobbyRoom result = new OLobbyRoom();
            var qlob = (from lobby in dc.Lobbies
                       where lobby.LobbyId == lob.LobbyId
                       select lobby).First();
            result.TheLobby = new OLobby(){IsWaitingForPlayers = (bool)qlob.IsWaitingForPlayers, LobbyId = qlob.LobbyId, LobbyName = qlob.LobbyName};
            var qplayer = from player in dc.Players
                          join playLobby in dc.PlayLobbies on player.PlayerId equals playLobby.PlayerId
                          where playLobby.LobbyId == lob.LobbyId
                          select player;
            result.PlayerList = new List<OPlayer>();
            foreach (Player player in qplayer)
            {
                result.PlayerList.Add(new OPlayer(){MyTurn = player.MyTurn,PlayerId = player.PlayerId, PlayerName = player.PlayerName,Color = player.Color, Brick = player.Brick, Wheat = player.Wheat, IronOre = player.IronOre, Sheep = player.Sheep, Wood = player.Wood});
            }
            var qhost = (from oPlayer in result.PlayerList
                        join playLobby in dc.PlayLobbies on oPlayer.PlayerId equals playLobby.PlayerId
                        where oPlayer.PlayerId == playLobby.HostPlayer && playLobby.LobbyId == lob.LobbyId
                        select oPlayer).First();
            result.HostPlayer = qhost;
            return result;
        }

        public List<OLobby> GetAvailableLobbies()
        {
            var result = from l in dc.Lobbies
                         where l.IsWaitingForPlayers == true
                         select l;

            List<OLobby> list = new List<OLobby>();
            foreach (Lobby lobby in result)
            {
                list.Add(new OLobby() { LobbyId = lobby.LobbyId, LobbyName = lobby.LobbyName });
            }
            return list;
        }

        public List<OLobbyRoom> GetAvailableLobbyRooms()
        {
            var result = from l in dc.Lobbies
                         select l;

            var list = new List<OLobbyRoom>();
            foreach (var item in result)
            {
                var plies = from pl in dc.PlayLobbies
                            where pl.LobbyId == item.LobbyId
                            select pl;
                var playerlisty = from playLobby in plies
                                  join player in dc.Players on playLobby.PlayerId equals player.PlayerId
                                  select new OPlayer() { PlayerName = player.PlayerName, PlayerId = player.PlayerId };

                var pList = new List<OPlayer>();
                foreach (OPlayer player in playerlisty)
                {
                    pList.Add(new OPlayer() { PlayerName = player.PlayerName, PlayerId = player.PlayerId });
                }
                var lr = new OLobbyRoom()
                    {
                        HostPlayer = new OPlayer() { PlayerId = (int)plies.First().HostPlayer },
                        TheLobby = new OLobby() { LobbyId = item.LobbyId, LobbyName = item.LobbyName, IsWaitingForPlayers = (bool)item.IsWaitingForPlayers },
                        PlayerList = pList
                    };
                list.Add(lr);
            }
            return list;
        }

        public void StartPlay(OPlayer hostPlayer)
        {
            var lst = (from l in dc.PlayLobbies
                       where l.HostPlayer == hostPlayer.PlayerId
                       select new { l.LobbyId }).First();

            var update = (from l in dc.Lobbies
                          where l.LobbyId == lst.LobbyId
                          select l).First();
            update.IsWaitingForPlayers = false;
            dc.SubmitChanges();

        }

        //public GameObject SendGameUpdate(OPlayer player)
        //{
        //    throw new NotImplementedException();
        //}

        public void SubscribeToLobbyRoom(OPlayer player, OLobbyRoom lobby)
        {
            PlayLobby pLobby = new PlayLobby();
            pLobby.LobbyId = lobby.TheLobby.LobbyId;
            pLobby.PlayerId = player.PlayerId;
            pLobby.HostPlayer = lobby.HostPlayer.PlayerId;

            dc.PlayLobbies.InsertOnSubmit(pLobby);
            dc.SubmitChanges();

            var q = from playLobby in dc.PlayLobbies
                    where playLobby.LobbyId == lobby.TheLobby.LobbyId
                    select playLobby;

            if (q.Count() >= 4)
            {

                //StartPlay(lobby.HostPlayer);

                var update = (from l in dc.Lobbies
                              where l.LobbyId == lobby.TheLobby.LobbyId
                              select l).Single();
                update.IsWaitingForPlayers = false;
                dc.SubmitChanges();

            }
        }

        //public OPlayer GetPlayer(int id)
        //{
        //    var pl = (from p in dc.Players
        //              where p.PlayerId == id
        //              select p).Single();
        //    return new OPlayer() { PlayerId = pl.PlayerId, PlayerName = pl.PlayerName };
        //}

        public OPlayer MakePlayer(string username)
        {
            try
            {
                int maxId = (from player in dc.Players
                             select player.PlayerId).Max();

                var pPlayer = new Player { PlayerName = username, PlayerId = ++maxId };

                dc.Players.InsertOnSubmit(pPlayer);
                dc.SubmitChanges();

                return new OPlayer() { PlayerId = pPlayer.PlayerId, PlayerName = pPlayer.PlayerName };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public void ChangeTurn(OLobbyRoom lobbyRoom)
        {
            var q = from oPlayer in lobbyRoom.PlayerList
                    where oPlayer.MyTurn == true
                    select oPlayer;

            for (int i = 0; i < lobbyRoom.PlayerList.Count; i++)
            {
                if (lobbyRoom.PlayerList.ElementAt(i).MyTurn)
                {
                    lobbyRoom.PlayerList.ElementAt(i).MyTurn = false;
                    if (++i >3)
                    {
                        lobbyRoom.PlayerList.ElementAt(0).MyTurn = true;
                    }
                    else
                    {
                        lobbyRoom.PlayerList.ElementAt(++i).MyTurn = true;
                    }
                    
                }
                
            }

        }
    }
}
