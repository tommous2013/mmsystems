﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfService.DTO;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        OLobbyRoom CreateLobby(OPlayer host);

        [OperationContract]
        List<OLobby> GetAvailableLobbies();
        
        [OperationContract]
        OLobbyRoom GetALobbyRoom(OLobby lob);

        [OperationContract]
        List<OLobbyRoom> GetAvailableLobbyRooms();

        [OperationContract]
        void StartPlay(OPlayer hostPlayer);

        [OperationContract]
        bool SubscribeToLobbyRoom(OPlayer player, OLobbyRoom lobby);

        [OperationContract]
        void UpdatePlayer(OPlayer playa);

        [OperationContract]
        OPlayer MakePlayer(string username);

        [OperationContract]
        void ChangeTurn(OLobbyRoom lobbyRoom);

        [OperationContract]
        void UpdateGame(OLobbyRoom lobbyRoom);
    }
}
