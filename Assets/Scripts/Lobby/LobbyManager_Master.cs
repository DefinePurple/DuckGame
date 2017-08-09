using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;
using GameSparks.Api.Messages;

public class LobbyManager_Master : MonoBehaviour {

    public delegate void GeneralEventHandler();
    public event GeneralEventHandler EventAuthReq;
    public event GeneralEventHandler EventRegReq;
    public event GeneralEventHandler EventGoToMain;
    public event GeneralEventHandler EventGoToLobby;
    public event GeneralEventHandler EventGoToProfile;
    public event GeneralEventHandler EventGoToShop;
    public event GeneralEventHandler EventUpdateEggs;
    public event GeneralEventHandler EventGoToLeaderboard;
    public event GeneralEventHandler EventLeaderboardKills;
    public event GeneralEventHandler EventLeaderboardWins;
    public event GeneralEventHandler EventLogout;
    public event GeneralEventHandler EventExitGame;

    public delegate void GoogleEventHandler();
    public event GoogleEventHandler EventGoogleSignIn;

    public delegate void FacebookEventHandler();
    public event FacebookEventHandler EventFacebookSignIn;

    public delegate void TextEventHandler(string text);
    public event TextEventHandler EventUpdateText;
    public event TextEventHandler EventSetText;

    public delegate void MatchHandler(MatchFoundMessage message);
    public event MatchHandler EventMatchFound;

    public void CallEventAuthReq() {
        if (EventAuthReq != null)
            EventAuthReq();
    }
    public void CallEventRegReq() {
        if (EventRegReq != null)
            EventRegReq();
    }
    public void CallEventGoToMain() {
        if (EventGoToMain != null)
            EventGoToMain();
    }
    public void CallEventGoToLobby() {
        if (EventGoToLobby != null)
            EventGoToLobby();
    }
    public void CallEventGoToProfile() {
        if (EventGoToProfile != null)
            EventGoToProfile();
    }
    public void CallEventGoToShop() {
        if (EventGoToShop != null)
            EventGoToShop();
    }
    public void CallEventUpdateEggs() {
        if (EventUpdateEggs != null)
            EventUpdateEggs();
    }
    public void CallEventGoToLeaderboard() {
        if (EventGoToLeaderboard != null)
            EventGoToLeaderboard();
    }
    public void CallEventLeaderboardKills() {
        if (EventLeaderboardKills != null)
            EventLeaderboardKills();
    }
    public void CallEventLeaderboardWins() {
        if (EventLeaderboardWins != null)
            EventLeaderboardWins();
    }
    public void CallEventLogout() {
        if (EventLogout != null)
            EventLogout();
    }
    public void CallEventExitGame() {
        if (EventExitGame != null)
            EventExitGame();
    }

    public void CallEventGoogleSignIn() {
        if (EventGoogleSignIn != null)
            EventGoogleSignIn();
    }

    public void CallEventFacebookSignIn() {
        if (EventFacebookSignIn != null)
            EventFacebookSignIn();
    }

    public void CallEventUpdateText(string text) {
        if (EventUpdateText != null)
            EventUpdateText(text);
    }
    public void CallEventSetText(string text) {
        if (EventSetText != null)
            EventSetText(text);
    }

    public void CallEventMatchFound(MatchFoundMessage message) {
        if (EventMatchFound != null)
            EventMatchFound(message);
    }

    

    private void Start() {
        GS.GameSparksAvailable += (isAvailable) => {
            if (isAvailable) {
                CallEventUpdateText("GameSparks Connected\n");
            } else {
                CallEventUpdateText("GameSparks Disconnected\n");
            }
        };

        MatchFoundMessage.Listener = (message) => {
            CallEventMatchFound(message);
        };
    }
}
