﻿using System;
using System.Collections.Generic;
using System.Drawing;
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
            lobby.IsUpdate = false;
            lobby.DiceRollllllllllllllllllllol = 0;

            PlayLobby pLobby = new PlayLobby();
            pLobby.HostPlayer = host.PlayerId;
            pLobby.LobbyId = lobby.LobbyId;
            pLobby.PlayerId = host.PlayerId;

            dc.PlayLobbies.InsertOnSubmit(pLobby);
            dc.Lobbies.InsertOnSubmit(lobby);
            dc.SubmitChanges();

            return new OLobbyRoom
                {
                    HostPlayer = host,
                    PlayerList = new List<OPlayer> { host },
                    TheLobby = new OLobby { LobbyId = lobby.LobbyId, LobbyName = lobby.LobbyName, IsUpdate = false }
                };
        }

        public OLobbyRoom GetALobbyRoom(OLobby lob)
        {
            OLobbyRoom result = new OLobbyRoom();
            var qlob = (from lobby in dc.Lobbies
                        where lobby.LobbyId == lob.LobbyId
                        select lobby).First();
<<<<<<< HEAD
            result.TheLobby = new OLobby() { IsWaitingForPlayers = (bool)qlob.IsWaitingForPlayers, LobbyId = qlob.LobbyId, LobbyName = qlob.LobbyName };
=======
            result.TheLobby = new OLobby { IsWaitingForPlayers = qlob.IsWaitingForPlayers, LobbyId = qlob.LobbyId, LobbyName = qlob.LobbyName };
>>>>>>> 568ca47... Foto's toegevoegd
            var qplayer = from player in dc.Players
                          join playLobby in dc.PlayLobbies on player.PlayerId equals playLobby.PlayerId
                          where playLobby.LobbyId == lob.LobbyId
                          select player;
            result.PlayerList = new List<OPlayer>();
            foreach (Player player in qplayer)
            {
<<<<<<< HEAD
                result.PlayerList.Add(new OPlayer() { MyTurn = player.MyTurn, PlayerId = player.PlayerId, PlayerName = player.PlayerName, Color = player.Color, Brick = player.Brick, Wheat = player.Wheat, IronOre = player.IronOre, Sheep = player.Sheep, Wood = player.Wood });
=======
                result.PlayerList.Add(new OPlayer { MyTurn = player.MyTurn, PlayerId = player.PlayerId, PlayerName = player.PlayerName, Color = player.Color, Brick = player.Brick, Wheat = player.Wheat, IronOre = player.IronOre, Sheep = player.Sheep, Wood = player.Wood, VictoryPoints = player.VictoryPoints });
>>>>>>> 568ca47... Foto's toegevoegd
            }
            var qhost = (from oPlayer in result.PlayerList
                         join playLobby in dc.PlayLobbies on oPlayer.PlayerId equals playLobby.PlayerId
                         where oPlayer.PlayerId == playLobby.HostPlayer && playLobby.LobbyId == lob.LobbyId
                         select oPlayer).First();
            result.HostPlayer = qhost;

            var qSettlements = (from settlement in dc.Settlements
                                join road in dc.Roads on settlement.RoadID equals road.RoadID
                                join player in dc.Players on road.OwenrID equals player.PlayerId
                                where road.LobbyID == result.TheLobby.LobbyId
<<<<<<< HEAD
                                select new OSettlement()
                                {
                                    ImageUrl = road.ImgUrl,
                                    Owner = new OPlayer() { Brick = player.Brick, Color = player.Color, IronOre = player.IronOre, MyTurn = player.MyTurn, PlayerId = player.PlayerId, PlayerName = player.PlayerName, Sheep = player.Sheep, Wheat = player.Wheat, Wood = player.Wood },
                                    Position = new Point(road.PositionX, road.PositionY),
                                    Upgraded = settlement.Upgraded
                                }).ToList();

            result.TheLobby.Settlements = qSettlements;

            var qRoads = (from road in dc.Roads
                          join player in dc.Players on road.OwenrID equals player.PlayerId
                          where road.LobbyID == result.TheLobby.LobbyId && player.PlayerId == road.OwenrID
                          select new ORoad()
                          {
                              Owner = new OPlayer() { Brick = player.Brick, Color = player.Color, IronOre = player.IronOre, MyTurn = player.MyTurn, PlayerId = player.PlayerId, PlayerName = player.PlayerName, Sheep = player.Sheep, Wheat = player.Wheat, Wood = player.Wood },
                              Position = new Point(road.PositionX, road.PositionY),
                              ImageUrl = road.ImgUrl
                          }).ToList();
            result.TheLobby.Roads = qRoads;


=======
                                select new OSettlement
                                    {
                                        ImageUrl = road.ImgUrl,
                                        Owner = new OPlayer { Brick = player.Brick, Color = player.Color, IronOre = player.IronOre, MyTurn = player.MyTurn, PlayerId = player.PlayerId, PlayerName = player.PlayerName, Sheep = player.Sheep, Wheat = player.Wheat, Wood = player.Wood },
                                        Position = new Point(road.PositionX, road.PositionY),
                                        Upgraded = settlement.Upgraded
                                    }).ToList();

            result.TheLobby.Settlements = qSettlements;
            var preQ1 = dc.Roads.Where(r => r.LobbyID == result.TheLobby.LobbyId); // alle roads van de lobby
            var preQ2 = preQ1.Where(r => !dc.Settlements.Any(s => s.RoadID == r.RoadID)); // alle roads die niet van een settlement zijn
            var preQ3 = (from road in preQ2
                         join player in dc.Players on road.OwenrID equals player.PlayerId
                         select new ORoad
                             {
                                 Owner = new OPlayer { Brick = player.Brick, Color = player.Color, IronOre = player.IronOre, MyTurn = player.MyTurn, PlayerId = player.PlayerId, PlayerName = player.PlayerName, Sheep = player.Sheep, Wheat = player.Wheat, Wood = player.Wood },
                                 Position = new Point(road.PositionX, road.PositionY),
                                 Position2 = new Point((int)road.PositionX1, (int)road.PositionY2),
                                 ShiftY = (int)road.ShiftY,
                                 ShiftX = (int)road.ShiftX,
                                 ImageUrl = road.ImgUrl
                             }).ToList();
            result.TheLobby.Roads = preQ3;

            var dice = (from lobb in dc.Lobbies
                        where lob.LobbyId == lobb.LobbyId
                        select lobb.DiceRollllllllllllllllllllol).First();


            if (dice != null)
                result.TheLobby.DiceNum = (int)dice;

            result.TheLobby.IsUpdate = dc.Lobbies.First(l => l.LobbyId == lob.LobbyId).IsUpdate;
>>>>>>> 568ca47... Foto's toegevoegd

            return result;
        }

        public List<OLobby> GetAvailableLobbies()
        {
            var result = from l in dc.Lobbies
                         where l.IsWaitingForPlayers
                         select l;

            return result.Select(lobby => new OLobby { LobbyId = lobby.LobbyId, LobbyName = lobby.LobbyName }).ToList();
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
                                  select new OPlayer { PlayerName = player.PlayerName, PlayerId = player.PlayerId };

                var pList = playerlisty.Select(player => new OPlayer { PlayerName = player.PlayerName, PlayerId = player.PlayerId }).ToList();
                var hostPlayer = plies.First().HostPlayer;
                if (hostPlayer != null)
                {
                    var lr = new OLobbyRoom
                        {
                            HostPlayer = new OPlayer { PlayerId = (int)hostPlayer },
                            TheLobby = new OLobby { LobbyId = item.LobbyId, LobbyName = item.LobbyName, IsWaitingForPlayers = item.IsWaitingForPlayers },
                            PlayerList = pList
                        };
                    list.Add(lr);
                }
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

        public bool SubscribeToLobbyRoom(OPlayer player, OLobbyRoom lobby)
        {
            try
            {
                var playersInLobbyBeforeJoining = (from playLobby in dc.PlayLobbies
<<<<<<< HEAD
                          where playLobby.LobbyId == lobby.TheLobby.LobbyId
                          select playLobby).Count();

                if (playersInLobbyBeforeJoining >= 4)
                    return false;

                PlayLobby pLobby = new PlayLobby();
                pLobby.LobbyId = lobby.TheLobby.LobbyId;
                pLobby.PlayerId = player.PlayerId;
                pLobby.HostPlayer = lobby.HostPlayer.PlayerId;

                dc.PlayLobbies.InsertOnSubmit(pLobby);
                dc.SubmitChanges();

                var playersInLobbyAfterJoining = from playLobby in dc.PlayLobbies
                        where playLobby.LobbyId == lobby.TheLobby.LobbyId
                        select playLobby;

                if (playersInLobbyAfterJoining.Count() >= 4)
                {
                    var update = (from l in dc.Lobbies
                                  where l.LobbyId == lobby.TheLobby.LobbyId
                                  select l).Single();
                    update.IsWaitingForPlayers = false;
                    //dc.SubmitChanges();

                    var a = (from playLobby in dc.PlayLobbies
                             join player1 in dc.Players on playLobby.PlayerId equals player1.PlayerId
                             where playLobby.LobbyId == lobby.TheLobby.LobbyId
                             select player1);

                    a.First().MyTurn = true;
                    //dc.SubmitChanges();

                    var b = new string[] { "White", "Red", "Blue", "Orange" };
                    int i = 0;
                    foreach (var p in a)
                    {
                        p.Color = b[i++];
                    }
                    dc.SubmitChanges();
                }
                return true;
=======
                                                   where playLobby.LobbyId == lobby.TheLobby.LobbyId
                                                   select playLobby).Count();

                if (playersInLobbyBeforeJoining >= 4)
                    return false;

                var pLobby = new PlayLobby
                    {
                        LobbyId = lobby.TheLobby.LobbyId,
                        PlayerId = player.PlayerId,
                        HostPlayer = lobby.HostPlayer.PlayerId
                    };

                dc.PlayLobbies.InsertOnSubmit(pLobby);
                dc.SubmitChanges();
>>>>>>> 568ca47... Foto's toegevoegd

                var playersInLobbyAfterJoining = from playLobby in dc.PlayLobbies
                                                 where playLobby.LobbyId == lobby.TheLobby.LobbyId
                                                 select playLobby;

                if (playersInLobbyAfterJoining.Count() >= 4)
                {
                    var update = (from l in dc.Lobbies
                                  where l.LobbyId == lobby.TheLobby.LobbyId
                                  select l).Single();
                    update.IsWaitingForPlayers = false;
                    //dc.SubmitChanges();

                    var a = (from playLobby in dc.PlayLobbies
                             join player1 in dc.Players on playLobby.PlayerId equals player1.PlayerId
                             where playLobby.LobbyId == lobby.TheLobby.LobbyId
                             select player1);

                    a.First().MyTurn = true;
                    //dc.SubmitChanges();

                    var b = new[] { "White", "Red", "Blue", "Orange" };
                    int i = 0;
                    foreach (var p in a)
                    {
                        p.Color = b[i++];
                    }
                    dc.SubmitChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                ErrorReport("SubscribeToLobbyRoom catch: " + e.Message);
                return false;
            }
            catch (Exception e)
            {
                ErrorReport("SubscribeToLobbyRoom catch: " + e.Message);
                return false;
            }
            
        }

        public OPlayer MakePlayer(string username)
        {
            try
            {
                int maxId = (from player in dc.Players
                             select player.PlayerId).Max();

                var pPlayer = new Player { PlayerName = username, PlayerId = ++maxId };

                if (username == "teste") // Cheat!!!!!!!!!!!!!!!!!!!!!
                {
                    pPlayer.IronOre = 10;
                    pPlayer.Sheep = 10;
                    pPlayer.Brick = 10;
                    pPlayer.Wood = 10;
                    pPlayer.Wheat = 10;
                }
                dc.Players.InsertOnSubmit(pPlayer);
                dc.SubmitChanges();

                return new OPlayer { PlayerId = pPlayer.PlayerId, PlayerName = pPlayer.PlayerName };
            }
            catch (Exception)
            {
                ErrorReport("MakePlayer Catch");
                throw;
            }
        }

        public void UpdatePlayer(OPlayer playa)
        {
            var q = (from player in dc.Players
                     where player.PlayerId == playa.PlayerId
                     select player).Single();
            q.IronOre = playa.IronOre;
            q.Sheep = playa.Sheep;
            q.Wheat = playa.Wheat;
            q.Wood = playa.Wood;
            q.Brick = playa.Brick;
            q.Color = playa.Color;
            q.MyTurn = playa.MyTurn;
<<<<<<< HEAD
=======
            q.VictoryPoints = playa.VictoryPoints;
>>>>>>> 568ca47... Foto's toegevoegd
            dc.SubmitChanges();
        }

        public void ChangeTurn(OLobbyRoom lobbyRoom)
        {
<<<<<<< HEAD
            bool turn = false;
            foreach (var oPlayer in lobbyRoom.PlayerList)
            {
=======
            //dc.Lobbies.First(l => l.LobbyId == lobbyRoom.TheLobby.LobbyId).IsUpdate = true;
            bool turn = false;
            foreach (var oPlayer in lobbyRoom.PlayerList)
            {
                //ErrorReport(oPlayer.PlayerId + " " + oPlayer.MyTurn + " 1" + DateTime.Now);
>>>>>>> 568ca47... Foto's toegevoegd
                if (oPlayer.MyTurn)
                {
                    turn = true;
                    oPlayer.MyTurn = false;
                    UpdatePlayer(oPlayer);
                }
                else if (turn)
                {
                    turn = false;
                    oPlayer.MyTurn = true;
                    UpdatePlayer(oPlayer);
                }
<<<<<<< HEAD
=======
                //ErrorReport(oPlayer.PlayerId + " " + oPlayer.MyTurn + " 2" + DateTime.Now);
>>>>>>> 568ca47... Foto's toegevoegd
            }
            if (lobbyRoom.PlayerList.All(p => p.MyTurn == false))
            {
                lobbyRoom.PlayerList.First().MyTurn = true;
                UpdatePlayer(lobbyRoom.PlayerList.First());
            }
        }

        public void UpdateGame(OLobbyRoom lobbyRoom)
        {
            try
            {
<<<<<<< HEAD

=======
                dc.Lobbies.First(l => l.LobbyId == lobbyRoom.TheLobby.LobbyId).IsUpdate = lobbyRoom.TheLobby.IsUpdate;
>>>>>>> 568ca47... Foto's toegevoegd

                var qRoads = from oRoad in lobbyRoom.TheLobby.Roads
                             where oRoad.RoadId == -1
                             select oRoad;

                int qRoadIdMax;
                try
<<<<<<< HEAD
                {
                    qRoadIdMax = (from road in dc.Roads
                                  select road.RoadID).Max();
                }
                catch (Exception)
                {
                    ErrorReport("UpdateGame qRoadIdMax catch");
                    qRoadIdMax = 0;
                }

                foreach (var oRoad in qRoads)
                {
                    ++qRoadIdMax;
                    dc.Roads.InsertOnSubmit(new Road()
                    {
                        ImgUrl = oRoad.ImageUrl,
                        LobbyID = lobbyRoom.TheLobby.LobbyId,
                        OwenrID = oRoad.Owner.PlayerId,
                        PositionX = oRoad.Position.X,
                        PositionY = oRoad.Position.Y,
                        RoadID = qRoadIdMax
                    });
                }

                //find new settlements

                //var qSettlements = lobbyRoom.TheLobby.Settlements.Where(p => !dc.Settlements.Any(p2 => p2.RoadID == p.RoadId));
                var qSettlements = from oSettlement in lobbyRoom.TheLobby.Settlements
                                   where oSettlement.RoadId == -1
                                   select oSettlement;
=======
                {
                    qRoadIdMax = (from road in dc.Roads
                                  select road.RoadID).Max();
                }
                catch (Exception)
                {
                    ErrorReport("UpdateGame qRoadIdMax catch");
                    qRoadIdMax = 0;
                }
                foreach (var oRoad in qRoads)
                {
                    ++qRoadIdMax;
                    dc.Roads.InsertOnSubmit(new Road
                        {
                            ImgUrl = oRoad.ImageUrl,
                            LobbyID = lobbyRoom.TheLobby.LobbyId,
                            OwenrID = oRoad.Owner.PlayerId,
                            PositionX = oRoad.Position.X,
                            PositionY = oRoad.Position.Y,
                            PositionX1 = oRoad.Position2.X,
                            PositionY2 = oRoad.Position2.Y,
                            ShiftX = oRoad.ShiftX,
                            ShiftY = oRoad.ShiftY,
                            RoadID = qRoadIdMax
                        });
                }
                // make new settlements
>>>>>>> 568ca47... Foto's toegevoegd
                int qSettlementIdMax;
                try
                {
                    qSettlementIdMax = (from settlement in dc.Settlements
                                        select settlement.SettlementID).Max();
                }
                catch (Exception)
                {
                    ErrorReport("UpdateGame, qSettlementIdMax catch");
                    qSettlementIdMax = 0;
                }

<<<<<<< HEAD
=======
                var qSettlements = from oSettlement in lobbyRoom.TheLobby.Settlements
                                   where oSettlement.RoadId == -1
                                   select oSettlement;
>>>>>>> 568ca47... Foto's toegevoegd
                foreach (var oSettlement in qSettlements)
                {
                    ++qSettlementIdMax;
                    ++qRoadIdMax;
<<<<<<< HEAD
                    dc.Roads.InsertOnSubmit(new Road() { ImgUrl = oSettlement.ImageUrl, LobbyID = lobbyRoom.TheLobby.LobbyId, OwenrID = oSettlement.Owner.PlayerId, PositionX = oSettlement.Position.X, PositionY = oSettlement.Position.Y, RoadID = qRoadIdMax });
                    dc.Settlements.InsertOnSubmit(new Settlement() { RoadID = qRoadIdMax, Upgraded = oSettlement.Upgraded, SettlementID = qSettlementIdMax });
=======
                    dc.Roads.InsertOnSubmit(new Road { ImgUrl = oSettlement.ImageUrl, LobbyID = lobbyRoom.TheLobby.LobbyId, OwenrID = oSettlement.Owner.PlayerId, PositionX = oSettlement.Position.X, PositionY = oSettlement.Position.Y, RoadID = qRoadIdMax });
                    dc.Settlements.InsertOnSubmit(new Settlement { RoadID = qRoadIdMax, Upgraded = oSettlement.Upgraded, SettlementID = qSettlementIdMax });
>>>>>>> 568ca47... Foto's toegevoegd
                }

                //update settlements
                var qSettlementsUpdate = from settlement1 in dc.Settlements
                                         join road in dc.Roads on settlement1.RoadID equals road.RoadID
                                         where road.LobbyID == lobbyRoom.TheLobby.LobbyId
                                         select settlement1;

                foreach (var settlement in qSettlementsUpdate)
                {
<<<<<<< HEAD
                    foreach (var oSettlement in lobbyRoom.TheLobby.Settlements)
                    {
                        if (settlement.RoadID == oSettlement.RoadId)
                        {
                            settlement.Upgraded = oSettlement.Upgraded;
                        }
                    }
                }
=======
                    foreach (var oSettlement in lobbyRoom.TheLobby.Settlements.Where(oSettlement => settlement.RoadID == oSettlement.RoadId))
                    {
                        settlement.Upgraded = oSettlement.Upgraded;
                    }
                }

                foreach (var playa in lobbyRoom.PlayerList)
                {
                    //UpdatePlayer(playa);
                }

                var lobb = (from lob in dc.Lobbies
                            where lob.LobbyId == lobbyRoom.TheLobby.LobbyId
                            select lob).First();
                lobb.DiceRollllllllllllllllllllol = lobbyRoom.TheLobby.DiceNum;

>>>>>>> 568ca47... Foto's toegevoegd
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                ErrorReport(e.Message);
            }
        }

        public void ErrorReport(string msg)
        {
            int maxId = (from error in dc.Errors
                         select error.Id).Max();

<<<<<<< HEAD
            dc.Errors.InsertOnSubmit(new Error() { Id = ++maxId, Mesg = msg });
=======
            dc.Errors.InsertOnSubmit(new Error { Id = ++maxId, Mesg = msg });
>>>>>>> 568ca47... Foto's toegevoegd
            dc.SubmitChanges();
        }
    }
}
