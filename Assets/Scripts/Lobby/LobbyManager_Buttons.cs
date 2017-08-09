using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GooglePlayGames;
using UnityEngine.SceneManagement;

public class LobbyManager_Buttons : MonoBehaviour {

    private LobbyManager_Master lm_master;
    //All Buttons in main menu
    public Button ffaButton, teamsButton, profileButton, shopButton, leaderButton, logoutButton, exitButton;
    //All Buttons in shop
    public Button shopToMainButton;
    //All Buttons in Leaderboards
    public Button leaderToMainButton, killsButton, winsButton;
    //All Buttons in profile
    public Button profileToMainButton;
    //All Buttons in lobby
    public Button readyButton, lobbyToMainButton;

    private void OnEnable() {
        SetInitial();
    }

    private void SetInitial() {
        lm_master = GetComponent<LobbyManager_Master>();
    }

    // Use this for initialization
    void Start() {
        #region Main Menu Button Listeners
        ffaButton.onClick.AddListener(() => {
            GameSparksManager.Instance().FindPlayers("FFA");
            lm_master.CallEventUpdateText("Searching for Free For All match...\n");
        });

        teamsButton.onClick.AddListener(() => {
            GameSparksManager.Instance().FindPlayers("TEST_FFA");
            lm_master.CallEventUpdateText("Searching for Team match...\n");
        });

        profileButton.onClick.AddListener(() => {
            lm_master.CallEventGoToProfile();
            lm_master.CallEventUpdateText("Looking at Profile...\n");
        });

        shopButton.onClick.AddListener(() => {
            lm_master.CallEventGoToShop();
            lm_master.CallEventUpdateText("Entering Shop...\n");
        });

        leaderButton.onClick.AddListener(() => {
            lm_master.CallEventGoToLeaderboard();
            lm_master.CallEventUpdateText("Entering leaderboards...\n");

            killsButton.GetComponent<Image>().color = Color.gray;
            winsButton.GetComponent<Image>().color = Color.white;
            lm_master.CallEventLeaderboardKills();
        });

        logoutButton.onClick.AddListener(() => {
            GS.Reset();
            lm_master.CallEventLogout();

            PlayGamesPlatform.Instance.SignOut();

            SceneManager.LoadScene("Main");
        });

        exitButton.onClick.AddListener(() => {
            lm_master.CallEventLogout();
            PlayGamesPlatform.Instance.SignOut();

            lm_master.CallEventUpdateText("Exiting game...\n");
            GS.Disconnect();
            GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
            GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
            foreach (GameObject go in allGameObjects) {
                if (go != camera)
                    Destroy(go.gameObject);
            }

            Application.Quit();
        });
        #endregion

        #region Shop Button Listeners
        shopToMainButton.onClick.AddListener(() => {
            lm_master.CallEventGoToMain();
        });
        #endregion

        #region Profile Button Listeners
        profileToMainButton.onClick.AddListener(() => {
            lm_master.CallEventGoToMain();
        });
        #endregion

        #region Leaderboard Button Listeners
        leaderToMainButton.onClick.AddListener(() => {
            lm_master.CallEventGoToMain();
        });

        killsButton.onClick.AddListener(() => {
            killsButton.GetComponent<Image>().color = Color.gray;
            winsButton.GetComponent<Image>().color = Color.white;
            lm_master.CallEventLeaderboardKills();
        });

        winsButton.onClick.AddListener(() => {
            killsButton.GetComponent<Image>().color = Color.white;
            winsButton.GetComponent<Image>().color = Color.gray;
            lm_master.CallEventLeaderboardWins();
        });
        #endregion

        #region Lobby Button Listeners
        lobbyToMainButton.onClick.AddListener(() => {
            new LogEventRequest()
            .SetEventKey("DISCONNECT")
            .Send((response) => {
                if (response.HasErrors) {
                    Debug.Log(response.Errors.JSON.ToString());
                }
            });

            new LogEventRequest()
            .SetEventKey("CLS")
            .Send((response) => { });
            lm_master.CallEventGoToMain();
        });

        readyButton.onClick.AddListener(() => {
            GameSparksManager.Instance().ReadyUp();
        });
        #endregion
    }
}
