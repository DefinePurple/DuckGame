using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class LobbyManager_Leaderboard : MonoBehaviour {

    [SerializeField] private GameObject prefab, parent;
    [SerializeField] private int entryCount = 20;
    private LobbyManager_Master lm_master;
    GSEnumerable<LeaderboardDataResponse._LeaderboardData> killData = null;
    GSEnumerable<LeaderboardDataResponse._LeaderboardData> winData = null;

    private void OnEnable() {
        SetInitial();
        lm_master.EventLeaderboardKills += DestroyChildren;
        lm_master.EventLeaderboardKills += GetKillData;
        lm_master.EventLeaderboardKills += SetKillData;

        lm_master.EventLeaderboardWins += DestroyChildren;
        lm_master.EventLeaderboardWins += GetWinData;
        lm_master.EventLeaderboardWins += SetWinData;
    }

    private void OnDisable() {
        lm_master.EventLeaderboardKills -= DestroyChildren;
        lm_master.EventLeaderboardKills -= GetKillData;
        lm_master.EventLeaderboardKills -= SetKillData;

        lm_master.EventLeaderboardWins -= DestroyChildren;
        lm_master.EventLeaderboardWins -= GetWinData;
        lm_master.EventLeaderboardWins -= SetWinData;
    }

    public void SetInitial() {
        lm_master = GetComponent<LobbyManager_Master>();
        DestroyChildren();
        GetKillData();
        SetKillData();
        GetWinData();
    }

    private void GetKillData() {
        new LeaderboardDataRequest()
        .SetEntryCount(entryCount)
        .SetLeaderboardShortCode("LEAD_KILLS")
        .Send((response) => {
            killData = response.Data;
        });
    }


    private void SetKillData() {
        if (killData != null)
            foreach (LeaderboardDataResponse._LeaderboardData entry in killData) {
                SetPlayerData data = prefab.GetComponent<SetPlayerData>();
                data.Set(entry.Rank.ToString(), entry.UserName.ToString(), entry.JSONData["SUM-KILL_SCORE"].ToString());
                Instantiate(prefab, parent.transform);
            }
    }

    private void GetWinData() {
        new LeaderboardDataRequest()
        .SetEntryCount(entryCount)
        .SetLeaderboardShortCode("LEAD_WINS")
        .Send((response) => {
            winData = response.Data;
        });
    }

    private void SetWinData() {
        if (winData != null)
            foreach (LeaderboardDataResponse._LeaderboardData entry in winData) {
                SetPlayerData data = prefab.GetComponent<SetPlayerData>();
                data.Set(entry.Rank.ToString(), entry.UserName.ToString(), entry.JSONData["SUM-WIN_SCORE"].ToString());
                Instantiate(prefab, parent.transform);
            }
    }

    private void DestroyChildren() {
        if (parent.transform.childCount > 0)
            foreach (Transform child in parent.transform)
                Destroy(child.gameObject);

    }
}
