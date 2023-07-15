using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{

    public static SteamLobby Instance;


    //callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;


    //Variables
    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;


    


    private void Start() {

      


        if (!SteamManager.Initialized) { return; }
        if (Instance == null) { Instance = this;  }


        manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
   
    }


    public void HostLobby() {

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);

    }


    private void OnLobbyCreated(LobbyCreated_t callback) {

        if(callback.m_eResult != EResult.k_EResultOK) { return; }

        Debug.Log("lobby created succesfully");

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s LOBBY");
    
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback) {
      
        Debug.Log("request to join lobby");

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);

    }
                                                                                             

    private void OnLobbyEntered(LobbyEnter_t callback) {


        //everyone

        CurrentLobbyID = callback.m_ulSteamIDLobby;
      
        //clients
        if (NetworkServer.active) { return; }
 
        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        manager.StartClient();


    }


}