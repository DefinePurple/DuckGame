using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameSparks.Api.Messages;
using GameSparks.Core;

public class LobbyManager_Panels : MonoBehaviour {

    //All Panels
    public GameObject panelMenu, panelShop, panelText, panelProfile, panelLeader, panelLobby;
    private LobbyManager_Master lm_master;

    private RTSessionInfo tempRTSessionInfo;


    private void OnEnable() {
        SetInitial();
        lm_master.EventGoToShop += GoToShop;
        lm_master.EventGoToProfile += GoToProfile;
        lm_master.EventGoToMain += GoToMainMenu;
        lm_master.EventGoToLeaderboard += GoToLeaderboards;
        lm_master.EventGoToLobby += GoToLobby;
    }

    private void OnDisable() {
        lm_master.EventGoToShop -= GoToShop;
        lm_master.EventGoToProfile -= GoToProfile;
        lm_master.EventGoToMain -= GoToMainMenu;
        lm_master.EventGoToLeaderboard -= GoToLeaderboards;
        lm_master.EventGoToLobby -= GoToLobby;
    }

    private void Start() {
        #region Messages
        MatchNotFoundMessage.Listener = (message) => {
            lm_master.CallEventUpdateText("No Match Found...\n");
        };
        #endregion
    }

    private void SetInitial() {
        lm_master = GetComponent<LobbyManager_Master>();
    }

    private void DisableAllPanels() {
        panelText.SetActive(false);
        panelMenu.SetActive(false);
        panelShop.SetActive(false);
        panelProfile.SetActive(false);
        panelLeader.SetActive(false);
        panelLobby.SetActive(false);
    }

    private void GoToMainMenu() {
        DisableAllPanels();
        panelMenu.SetActive(true);
        panelText.SetActive(true);
    }

    private void GoToShop() {
        DisableAllPanels();
        panelShop.SetActive(true);
        //GetComponent<Shop>().GetData();
    }

    private void GoToLeaderboards() {
        DisableAllPanels();
        panelLeader.SetActive(true);
        //GetComponent<Leaderboard>().Init();
    }

    private void GoToProfile() {
        DisableAllPanels();
        panelProfile.SetActive(true);
        //GetComponent<ProfileStats>().GetData();
    }

    private void GoToLobby() {
        DisableAllPanels();

        panelText.SetActive(true);
        panelLobby.SetActive(true);
    }
}
