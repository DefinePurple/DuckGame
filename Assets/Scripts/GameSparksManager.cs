using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using GameSparks.Core;
using GameSparks.Api.Responses;
using GameSparks.Api.Requests;
using GameSparks.Api.Messages;
using GameSparks.RT;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Facebook.Unity;

public class GameSparksManager : MonoBehaviour {

    public bool isLoggedIn = false;

    /// <summary>The GameSparks Manager singleton</summary>
    private static GameSparksManager instance = null;
    /// <summary>This method will return the current instance of this class </summary>
    public static GameSparksManager Instance() {
        if (instance != null) {
            return instance; // return the singleton if the instance has been setup
        } else { // otherwise return an error
            Debug.LogError("GSM| GameSparksManager Not Initialized...");
        }
        return null;
    }

    private void OnEnable() {
        instance = this; // if not, give it a reference to this class...
        DontDestroyOnLoad(this.gameObject); // and make this object persistent as we load new scenes

        ScriptMessage.Listener += ScriptMessageFound;
        ScriptMessage.Listener += OnScriptMessage;
    }

    private void OnDisable() {
        ScriptMessage.Listener -= ScriptMessageFound;
        ScriptMessage.Listener -= OnScriptMessage;
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(this.gameObject);
        }

        if (!isLoggedIn)
            FB.Init();
    }

    void Start() {
        // Create client configuration
        PlayGamesClientConfiguration config = new
            PlayGamesClientConfiguration.Builder()
            .RequestEmail()
            .RequestServerAuthCode(true)
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;

        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

    }

    #region Registration & Authentication
    public delegate void AuthCallback(AuthenticationResponse _authresp);
    public delegate void RegCallback(RegistrationResponse _authresp);
    public delegate void GoogleCallback(bool success);

    public void RegisterUser(string _username, string _password, RegCallback _regcallback) {
        new RegistrationRequest()
            .SetDisplayName(_username)
            .SetUserName(_username)
            .SetPassword(_password)
            .Send((regResp) => {
                if (!regResp.HasErrors) {
                    Debug.Log("GSM| Registration Successful...");
                } else {
                    Debug.Log("GSM| Registration Failed...");
                }
                _regcallback(regResp);
            });
    }

    public void AuthenticateUser(string _username, string _password, AuthCallback _authcallback) {
        new AuthenticationRequest()
            .SetUserName(_username)
            .SetPassword(_password)
            .Send((authResp) => {
                if (!authResp.HasErrors) {
                    Debug.Log("GSM| Authentication Successful...");
                } else
                    Debug.LogWarning("GSM| Error Authenticating User \n" + authResp.Errors.JSON);
                _authcallback(authResp);

            });
    }

    #region Google
    public void GoogleSignIn(GoogleCallback _googlecallback) {
        PlayGamesPlatform.Instance.Authenticate((success) => {
            if (success) {
                Debug.Log("GPS | Google Authenticate Success");
                _googlecallback(true);
            } else {
                Debug.Log("GPS | Google Authenticate Fail");
                _googlecallback(false);
            }
        });
    }

    public void GameSparksGooglePlus(AuthCallback _authcallback) {
        new GooglePlusConnectRequest()
            .SetCode(PlayGamesPlatform.Instance.GetServerAuthCode())
            .SetSyncDisplayName(true)
            .Send((response) => {
                _authcallback(response);
            });
    }

    public void GameSparksGooglePlay(AuthCallback _authcallback) {
        PlayGamesLocalUser user = (PlayGamesLocalUser)Social.localUser;
        new GooglePlayConnectRequest()
            .SetCode(PlayGamesPlatform.Instance.GetServerAuthCode())
            .SetRedirectUri("https://www.gamesparks.com/oauth2callback")
            .SetDisplayName(user.userName)
            .SetSyncDisplayName(true)
            .Send((response) => {
                _authcallback(response);
            });
    }
    #endregion

    #region Facebook
    public void FacebookSignIn(AuthCallback _authcallback) {
        //Setup facebook permissions
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");

        //Login in using the set permissions
        FB.LogInWithReadPermissions(permissions, (result) => {
            //If there is an error, print it
            if (result.Error != null)
                Debug.Log(result.Error.ToString());
            else {
                //Otherwise, if FB is logged in, Try to connect with GameSparks
                if (FB.IsLoggedIn) {
                    Debug.Log("FB | Login Success");
                    GameSparksToFacebook(_authcallback);
                } else
                    Debug.Log("FB | Login Error");
            }
        });
    }

    void GameSparksToFacebook(AuthCallback _authcallback) {
        new FacebookConnectRequest()
            .SetAccessToken(AccessToken.CurrentAccessToken.TokenString)
            .SetDoNotLinkToCurrentPlayer(false)// we don't want to create a new account so link to the player that is currently logged in
            .SetSwitchIfPossible(true)//this will switch to the player with this FB account if they already have an account from a separate login
            .Send((response) => {
                if (!response.HasErrors) {
                    _authcallback(response);
                    isLoggedIn = true;
                } else {
                    Debug.LogWarning(response.Errors.JSON);//if we have errors, print them out
                }
            });
    }
    #endregion
    #endregion

    #region Matchmaking
    private GameSparksRTUnity gameSparksRTUnity;
    public GameSparksRTUnity GetRTSession() {
        return gameSparksRTUnity;
    }
    private RTSessionInfo sessionInfo;
    public RTSessionInfo GetSessionInfo() {
        return sessionInfo;
    }

    public void FindPlayers(string matchShortCode) {
        Debug.Log("Attempting Matchmaking...");
        new MatchmakingRequest()
            .SetMatchShortCode(matchShortCode)
            .SetSkill(0)
            .Send((response) => {
                if (response.HasErrors) {
                    Debug.LogError("GSM| MatchMaking Error \n" + response.Errors.JSON);
                }
            });
    }

    public void ReadyUp() {
        new LogEventRequest()
            .SetEventKey("ON_READY")
            .Send((response) => {
                if (response.HasErrors) {
                    Debug.Log(response.Errors.JSON.ToString());
                }
            });

        new LogEventRequest()
            .SetEventKey("START_GAME")
            .Send((response) => {
                if (response.HasErrors) {
                    Debug.Log(response.Errors.JSON.ToString());
                }
            });
    }
    #endregion

    #region Test Region Packet Sends
    public void ShootTest(int peerId) {
        using (RTData data = RTData.Get()) {  // we put a using statement here so that we can dispose of the RTData objects once the packet is sent
            data.SetInt(1, peerId);
            GameSparksManager.Instance().GetRTSession().SendData(1, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data);// send the data
        }
    }

    public void DieTest(int senderPeerId) {
        using (RTData data = RTData.Get()) {  // we put a using statement here so that we can dispose of the RTData objects once the packet is sent
            data.SetInt(1, senderPeerId);
            GameSparksManager.Instance().GetRTSession().SendData(2, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data, new int[] { 0 });// send the data
        }
    }

    //Used to start the next round. Just reloads the screen
    public void StartNewRound() {
        SceneManager.LoadScene("TestFFA");
    }

    public void ReturnToMenu() {
        gameSparksRTUnity.Disconnect();
        Destroy(gameSparksRTUnity);
        SceneManager.LoadScene("Menu");
    }
    #endregion

    public void StartNewRTSession(RTSessionInfo _info) {
        if (gameSparksRTUnity == null) {
            Debug.Log("GSM| Creating New RT Session Instance...");
            sessionInfo = _info;
            gameSparksRTUnity = this.gameObject.AddComponent<GameSparksRTUnity>(); // Adds the RT script to the game
                                                                                   // In order to create a new RT game we need a 'FindMatchResponse' //
                                                                                   // This would usually come from the server directly after a sucessful FindMatchRequest //
                                                                                   // However, in our case, we want the game to be created only when the first player decides using a button //
                                                                                   // therefore, the details from the response is passed in from the gameInfo and a mock-up of a FindMatchResponse //
                                                                                   // is passed in. In normal operation this mock-response may not be needed //
            GSRequestData mockedResponse = new GSRequestData().AddNumber("port", (double)_info.GetPortID()).AddString("host", _info.GetHostURL()).AddString("accessToken", _info.GetAccessToken()); // construct a dataset from the game-details
            FindMatchResponse response = new FindMatchResponse(mockedResponse); // create a match-response from that data and pass it into the game-config
                                                                                // So in the game-config method we pass in the response which gives the instance its connection settings //
                                                                                // In this example i use a lambda expression to pass in actions for 
                                                                                // OnPlayerConnect, OnPlayerDisconnect, OnReady and OnPacket actions //
                                                                                // These methods are self-explanitory, but the important one is the OnPacket Method //
                                                                                // this gets called when a packet is received //
            gameSparksRTUnity.Configure(response,
                (peerId) => { OnPlayerConnectedToGame(peerId); },
                (peerId) => { OnPlayerDisconnected(peerId); },
                (ready) => { OnRTReady(ready); },
                (packet) => { OnPacketReceived(packet); });
            gameSparksRTUnity.Connect(); // when the config is set, connect the game
        } else {
            Debug.LogError("Session Already Started");
        }
    }

    private void OnPlayerConnectedToGame(int _peerId) {
        Debug.Log("GSM| Player Connected, " + _peerId);
    }

    private void OnPlayerDisconnected(int _peerId) {
        Debug.Log("GSM| Player Disconnected, " + _peerId);
    }

    private void OnRTReady(bool _isReady) {
        if (_isReady) {
            Debug.Log("GSM| RT Session Connected...");
            SceneManager.LoadScene("TestFFA");
        }

    }

    private void OnScriptMessage(ScriptMessage _message) {
        Debug.Log(_message.ExtCode);
    }

    private void ScriptMessageFound(ScriptMessage _message) {
        if (_message.ExtCode == "TEST") {
            GSData data = _message.Data;
            Debug.Log(data.JSON.ToString());
        }

        if (_message.ExtCode == "MATCH_OVER") {
            GameSparksManager.Instance().ReturnToMenu();
        }

        if (_message.ExtCode == "NEW_ROUND") {
            GameSparksManager.Instance().StartNewRound();
        }
    }

    private void OnPacketReceived(RTPacket _packet) {
        switch (_packet.OpCode) {
            case 1:
                GameController.Instance().ReceiveShot((int)_packet.Data.GetInt(1), _packet.Sender);
                break;

            case 2:
                GameController.Instance().ReceiveDeath(_packet.Sender);
                break;
        }
    }
}

public class RTSessionInfo {
    private string hostURL;
    public string GetHostURL() { return this.hostURL; }
    private string acccessToken;
    public string GetAccessToken() { return this.acccessToken; }
    private int portID;
    public int GetPortID() { return this.portID; }
    private string matchID;
    public string GetMatchID() { return this.matchID; }

    private List<RTPlayer> playerList = new List<RTPlayer>();
    public List<RTPlayer> GetPlayerList() {
        return playerList;
    }

    /// <summary>
    /// Creates a new RTSession object which is held untill a new RT session is created
    /// </summary>
    /// <param name="_message">Message.</param>
    public RTSessionInfo(MatchFoundMessage _message) {
        portID = (int)_message.Port;
        hostURL = _message.Host;
        acccessToken = _message.AccessToken;
        matchID = _message.MatchId;
        // we loop through each participant and get thier peerId and display name //
        foreach (MatchFoundMessage._Participant p in _message.Participants) {
            playerList.Add(new RTPlayer(p.DisplayName, p.Id, (int)p.PeerId));
        }
    }

    public class RTPlayer {
        public RTPlayer(string _displayName, string _id, int _peerId) {
            this.displayName = _displayName;
            this.id = _id;
            this.peerID = _peerId;
        }

        public string displayName;
        public string id;
        public int peerID;
        public bool isOnline;
    }
}