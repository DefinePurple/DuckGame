using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Requests;
public class LobbyManager_Profile : MonoBehaviour {

    public Text displayName, kills, deaths, wins, losses;
    private LobbyManager_Master lm_master;

    private void OnEnable() {
        SetInitial();
        lm_master.EventGoToProfile += GetData;
    }

    private void OnDisable() {
        lm_master.EventGoToProfile -= GetData;
    }

    private void SetInitial() {
        lm_master = GetComponent<LobbyManager_Master>();
    }

    private void GetData() {
        GSData currencies = null;
        new AccountDetailsRequest()
        .Send((response) => {
            //achievements = response.Achievements;
            currencies = response.Currencies;
            kills.text = currencies.GetLong("KILL").ToString();
            deaths.text = currencies.GetLong("DEATH").ToString();
            wins.text = currencies.GetLong("WIN").ToString();
            losses.text = currencies.GetLong("LOSS").ToString();
            displayName.text = response.DisplayName;
        });
    }
}
