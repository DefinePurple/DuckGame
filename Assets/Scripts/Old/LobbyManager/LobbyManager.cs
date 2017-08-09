using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Responses;
using GameSparks.Api.Requests;
using GameSparks.Core;
using GameSparks.Api.Messages;


public class LobbyManager : MonoBehaviour {
    public Text details;

    //All Panels
    public GameObject panelIO, panelMenu, panelShop, panelText, panelProfile, panelLeader, panelLobby;
    //All Buttons in login screen
    public Button loginButton, registerButton;
    public InputField username, password;
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

    private RTSessionInfo tempRTSessionInfo;

    private void Start() {
        GS.GameSparksAvailable += (isAvailable) => {
            if (isAvailable) {
                details.text += "GameSparks Connected\n";
            } else {
                if(details != null)
                    details.text += "GameSparks Disconnected\n";
            }
        };

        GoToLogin();

        #region Login Screen Button Listeners
        loginButton.onClick.AddListener(() => {
            details.text += "Authenticating...\n";
            GameSparksManager.Instance().AuthenticateUser(username.text, password.text, OnAuthentication);
        });

        registerButton.onClick.AddListener(() => {
            details.text += "Registering...\n";
            GameSparksManager.Instance().RegisterUser(username.text, password.text, OnRegistration);
        });
        #endregion

        #region Main Menu Button Listeners
        ffaButton.onClick.AddListener(() => {
            GameSparksManager.Instance().FindPlayers("FFA");
            details.text += "Searching for Free For All match...\n";
        });

        teamsButton.onClick.AddListener(() => {
            GameSparksManager.Instance().FindPlayers("TEST_FFA");
            details.text += "Searching for Team match...\n";
        });

        profileButton.onClick.AddListener(() => {
            GoToProfile();
            details.text += "Looking at Profile...\n";
        });

        shopButton.onClick.AddListener(() => {
            GoToShop();
            details.text += "Entering Shop...\n";
        });

        leaderButton.onClick.AddListener(() => {
            GoToLeaderboards();
            details.text += "Entering leaderboards...\n";
        });

        logoutButton.onClick.AddListener(() => {
            GS.Reset();
            GoToLogin();
        });

        exitButton.onClick.AddListener(() => {
            details.text += "Exiting game...\n";
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
            GoToMainMenu();
        });
        #endregion

        #region Profile Button Listeners
        profileToMainButton.onClick.AddListener(() => {
            GoToMainMenu();
        });
        #endregion

        #region Leaderboard Button Listeners
        leaderToMainButton.onClick.AddListener(() => {
            GoToMainMenu();
        });
        #endregion

        #region Lobby Button Listeners
        lobbyToMainButton.onClick.AddListener(() => {
            details.text = "";
            new LogEventRequest()
            .SetEventKey("DISCONNECT")
            .Send((response) => { });
            GoToMainMenu();
        });

        readyButton.onClick.AddListener(() => {
            GameSparksManager.Instance().ReadyUp();
        });
        #endregion

        #region Messages
        MatchNotFoundMessage.Listener = (message) => {
            details.text += "No Match Found...\n";
        };

        MatchFoundMessage.Listener += OnMatchFound;

        ScriptMessage.Listener += OnStartGame;

        MatchFoundMessage.Listener += GetData;
        ScriptMessage.Listener += UpdatePlayers;
        #endregion

        GetComponent<ProfileStats>().GetData();
    }

    private void OnStartGame(ScriptMessage _message) {
        Debug.Log(_message.ExtCode);
        if (_message.ExtCode == "START") {
            GameSparksManager.Instance().StartNewRTSession(tempRTSessionInfo);
        }
    }

    private void OnMatchFound(MatchFoundMessage _message) {
        DisableAllPanels();
        tempRTSessionInfo = new RTSessionInfo(_message);
        panelText.SetActive(true);
        panelLobby.SetActive(true);

        StringBuilder sBuilder = new StringBuilder();
        sBuilder.AppendLine("Match Found...");
        sBuilder.AppendLine("Opponents:" + _message.Participants.Count());
        sBuilder.AppendLine("_________________");
        sBuilder.AppendLine(); // we'll leave a space between the player-list and the match data
        foreach (MatchFoundMessage._Participant player in _message.Participants) {
            sBuilder.AppendLine(player.DisplayName + " joined the game..."); // add the player number and the display name to the list
        }
        details.text = sBuilder.ToString(); // set the string to be the player-list field
    }

    private void OnRegistration(RegistrationResponse _resp) {
        if ((bool)_resp.NewPlayer)
            details.text += "Account Created\n";
        else if (!(bool)_resp.NewPlayer)
            details.text += "Account already exists\n";
    }

    private void OnAuthentication(AuthenticationResponse _resp) {
        if (!_resp.HasErrors) {
            details.text += "Account Authenticated\n";
            GoToMainMenu();
        } else if (_resp.HasErrors)
            details.text += "Error Autenticating\n";
    }

    private void DisableAllPanels() {
        panelText.SetActive(false);
        panelIO.SetActive(false);
        panelMenu.SetActive(false);
        panelShop.SetActive(false);
        panelProfile.SetActive(false);
        panelLeader.SetActive(false);
        panelLobby.SetActive(false);
    }

    private void GoToLogin() {
        DisableAllPanels();
        panelIO.SetActive(true);
        panelText.SetActive(true);
        details.text = "Welcome!\n";
        username.text = "";
        password.text = "";
    }

    private void GoToMainMenu() {
        DisableAllPanels();
        panelMenu.SetActive(true);
        panelText.SetActive(true);
    }

    private void GoToShop() {
        DisableAllPanels();
        panelShop.SetActive(true);
        GetComponent<Shop>().GetData();
    }

    private void GoToLeaderboards() {
        DisableAllPanels();
        panelLeader.SetActive(true);
        GetComponent<Leaderboard>().Init();
    }

    private void GoToProfile() {
        DisableAllPanels();
        panelProfile.SetActive(true);
        GetComponent<ProfileStats>().GetData();
    }


    [SerializeField] private GameObject[] players = new GameObject[4];

    private void UpdatePlayers(ScriptMessage _message) {
        if (_message.ExtCode == "READY") {
            GSData data = _message.Data;
            int peerId = (int)data.GetLong("peerId");
            players[peerId - 1].GetComponent<Lobby_Player>().ready = !players[peerId - 1].GetComponent<Lobby_Player>().ready;
        }
    }

    private void GetData(MatchFoundMessage _message) {
        Debug.Log("What");
        for (int i = 0; i < 4; i++) {
            foreach (MatchFoundMessage._Participant participant in _message.Participants) {
                if (participant.PeerId == i + 1) {
                    Lobby_Player script = players[i].GetComponent<Lobby_Player>(); Debug.Log("Setting player");
                    script.SetPeerId(participant.PeerId.ToString());
                    script.SetDisplayName(participant.DisplayName);
                    script.player = true;
                    script.ready = false;
                }
            }
        }
    }


    private void OnDisable() {
        foreach (GameObject go in players) {
            Lobby_Player script = go.GetComponent<Lobby_Player>();
            script.ResetText();
        }
    }
}