using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameSparks.Core;
using GameSparks.Api.Messages;
using System.Text;
using System.Linq;

public class LobbyManager_FFA : MonoBehaviour {

    private LobbyManager_Master lm_master;
    [SerializeField] private GameObject[] players = new GameObject[4];
    private RTSessionInfo tempRTSessionInfo;

    private void OnEnable() {
        SetInitial();
        lm_master.EventMatchFound += GetPlayerData;
        lm_master.EventMatchFound += GetRTData;
        lm_master.EventMatchFound += MatchFound;
        ScriptMessage.Listener += OnStartMessage;
        ScriptMessage.Listener += OnReadyMessage;
    }

    private void OnDisable() {
        lm_master.EventMatchFound -= GetPlayerData;
        lm_master.EventMatchFound -= GetRTData;
        lm_master.EventMatchFound -= MatchFound;
        ScriptMessage.Listener -= OnStartMessage;
        ScriptMessage.Listener -= OnReadyMessage;


        foreach (GameObject go in players) {
            Lobby_Player script = go.GetComponent<Lobby_Player>();
            script.ResetText();
        }
    }

    private void SetInitial() {
        lm_master = GetComponent<LobbyManager_Master>();
    }

    private void OnReadyMessage(ScriptMessage _message) {
        if (_message.ExtCode == "READY") {
            GSData data = _message.Data;
            int peerId = (int)data.GetLong("peerId");
            players[peerId - 1].GetComponent<Lobby_Player>().ready = !players[peerId - 1].GetComponent<Lobby_Player>().ready;
        }
    }

    private void OnStartMessage(ScriptMessage _message) {
        if (_message.ExtCode == "START") {
            GameSparksManager.Instance().StartNewRTSession(tempRTSessionInfo);
        }
    }

    private void GetRTData(MatchFoundMessage message) {
        tempRTSessionInfo = new RTSessionInfo(message);
    }

    private void GetPlayerData(MatchFoundMessage _message) {
        for (int i = 0; i < 4; i++) {
            foreach (MatchFoundMessage._Participant participant in _message.Participants) {
                if (participant.PeerId == i + 1) {
                    Lobby_Player script = players[i].GetComponent<Lobby_Player>();
                    script.SetPeerId(participant.PeerId.ToString());
                    script.SetDisplayName(participant.DisplayName);
                    script.player = true;
                    script.ready = false;
                }
            }
        }
    }

    private void MatchFound(MatchFoundMessage message) {
        tempRTSessionInfo = new RTSessionInfo(message);
        StringBuilder sBuilder = new StringBuilder();
        sBuilder.AppendLine("Match Found...");
        sBuilder.AppendLine("Opponents:" + message.Participants.Count());
        sBuilder.AppendLine("_________________");
        sBuilder.AppendLine(); // we'll leave a space between the player-list and the match data
        foreach (MatchFoundMessage._Participant player in message.Participants) {
            sBuilder.AppendLine(player.DisplayName + " joined the game..."); // add the player number and the display name to the list
        }
        lm_master.CallEventUpdateText(sBuilder.ToString()); // set the string to be the player-list field
        lm_master.CallEventGoToLobby();
    }
}
