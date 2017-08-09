using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Requests;

public class LobbyManager_Shop : MonoBehaviour {

    public Text eggs, goldenEggs;
    private LobbyManager_Master lm_master;

    private void OnEnable() {
        SetInitial();
        lm_master.EventGoToShop += GetData;
        lm_master.EventUpdateEggs += GetData;
    }

    private void OnDisable() {
        lm_master.EventGoToShop -= GetData;
        lm_master.EventUpdateEggs -= GetData;
    }

    private void SetInitial() {
        lm_master = GetComponent<LobbyManager_Master>();
    }

    public void GetData() {
        new AccountDetailsRequest()
        .Send((response) => {
            GSData currencies = response.Currencies;
            eggs.text = currencies.GetLong("EGG").ToString();
            goldenEggs.text = currencies.GetLong("G_EGG").ToString();
        });
    }
}
