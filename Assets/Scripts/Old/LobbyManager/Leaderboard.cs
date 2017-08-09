using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class Leaderboard : MonoBehaviour {

    [SerializeField] private Button killButton, winButton;
    [SerializeField] private GameObject prefab, parent;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private int entryCount = 20;
    GSEnumerable<LeaderboardDataResponse._LeaderboardData> killData = null;
    GSEnumerable<LeaderboardDataResponse._LeaderboardData> winData = null;

    private void Start() {
        killButton.onClick.AddListener(() => {
            killButton.GetComponent<Image>().color = Color.gray;
            winButton.GetComponent<Image>().color = Color.white;
            DestroyChildren();
            GetKillData();
            SetKillData();
        });

        winButton.onClick.AddListener(() => {
            killButton.GetComponent<Image>().color = Color.white;
            winButton.GetComponent<Image>().color = Color.gray;
            DestroyChildren();
            GetWinData();
            SetWinData();
        });
    }

    private void OnEnable() {
        Init();
    }

    private void OnDisable() {
        DestroyChildren();
    }

    public void Init() {
        killButton.GetComponent<Image>().color = Color.gray;
        winButton.GetComponent<Image>().color = Color.white;
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
