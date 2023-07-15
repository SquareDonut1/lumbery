using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;


    public Text LobbyNameText;


    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalPlayerController;


    public Button StartGameButton;
    public Text ReadyButtonText;




    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get {
            if (manager != null) {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }


    private void Awake() {
       
        if (Instance == null) { Instance = this; }
    }


    public void ReadyPlayer() {
        LocalPlayerController.ChangeReady();
    }

    public void UpdateButton() {
        if (LocalPlayerController.Ready) {
            ReadyButtonText.text = "Unready";
        } else {
            ReadyButtonText.text = "Ready";
        }
    }

    public void CheckIfAllReady() {
        bool AllReady = false;

        foreach(PlayerObjectController player in Manager.GamePlayers) {
            if (player.Ready) {
                AllReady = true;
            } else {
                AllReady = false;
                break;
            }
        }

        if (AllReady) {
            if(LocalPlayerController.PlayerIdNumber == 1) {
                StartGameButton.interactable = true;
            } else {
                StartGameButton.interactable = false;
            }
        } else {
            StartGameButton.interactable = false;
        }

    }

    public void UpdateLobbyName() {

         

        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");

    }


    public void UpdatePlayerList() {

        if (!PlayerItemCreated) {
            CreateHostPlayerItem();
        }

        if(PlayerListItems.Count < Manager.GamePlayers.Count) {
            CreateClientPlayerItem();
        }

        if(PlayerListItems.Count > Manager.GamePlayers.Count) {
            RemovePlayerItem();
        }

        if(PlayerListItems.Count == Manager.GamePlayers.Count) {
            UpdatePlayerItem();
        }


    }


    public void FindLocalPlayer() {

        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();

    }


    public void CreateHostPlayerItem() {

        foreach(PlayerObjectController player in Manager.GamePlayers) {
            GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab);
            PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

            NewPlayerItemScript.PlayerName = player.PlayerName;
            NewPlayerItemScript.ConnectionID = player.ConnectionID;
            NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            NewPlayerItemScript.Ready = player.Ready;
            NewPlayerItemScript.SetPlayerValues();

            NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            NewPlayerItem.transform.localScale = Vector3.one;

            PlayerListItems.Add(NewPlayerItemScript);

        }
        PlayerItemCreated = true;
    }

    public void CreateClientPlayerItem() {

        foreach (PlayerObjectController player in Manager.GamePlayers) {
            
            if(!PlayerListItems.Any(b => b.ConnectionID == player.ConnectionID)) {
                GameObject NewPlayerItem = Instantiate(PlayerListItemPrefab) as GameObject;
                PlayerListItem NewPlayerItemScript = NewPlayerItem.GetComponent<PlayerListItem>();

                NewPlayerItemScript.PlayerName = player.PlayerName;
                NewPlayerItemScript.ConnectionID = player.ConnectionID;
                NewPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                NewPlayerItemScript.Ready = player.Ready;
                NewPlayerItemScript.SetPlayerValues();

                NewPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                NewPlayerItem.transform.localScale = Vector3.one;

                PlayerListItems.Add(NewPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem() {

        foreach (PlayerObjectController player in Manager.GamePlayers) {

            foreach(PlayerListItem playerListItemScript in PlayerListItems) {

                if(playerListItemScript.ConnectionID == player.ConnectionID) {

                    playerListItemScript.PlayerName = player.PlayerName;
                    playerListItemScript.Ready = player.Ready;
                    playerListItemScript.SetPlayerValues();
                    if(player == LocalPlayerController) {
                        UpdateButton();
                    }

                }
            }
        }
        CheckIfAllReady();
    }

    public void RemovePlayerItem() {

        List<PlayerListItem> playerListItemsToRemove = new List<PlayerListItem>();

        foreach(PlayerListItem playerlistItem in PlayerListItems) {

            if(!Manager.GamePlayers.Any(b => b.ConnectionID == playerlistItem.ConnectionID)) {

                playerListItemsToRemove.Add(playerlistItem);
            }
        }

        if(playerListItemsToRemove.Count > 0) {                                                                      
           
            foreach(PlayerListItem PlayerListItemToRemove in playerListItemsToRemove) {
                GameObject ObjectToRemove = PlayerListItemToRemove.gameObject;
                PlayerListItems.Remove(PlayerListItemToRemove);
                Destroy(ObjectToRemove);
                ObjectToRemove = null;
            }
        }
    }


    public void StartGame(string SceneName) {
        LocalPlayerController.CanStartGame(SceneName);
    }                                      

}
