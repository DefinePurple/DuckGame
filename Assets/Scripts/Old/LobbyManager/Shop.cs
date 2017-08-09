using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class Shop : MonoBehaviour {

    public Text eggs, goldenEggs;

    private void OnEnable() {
        GetData();
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
