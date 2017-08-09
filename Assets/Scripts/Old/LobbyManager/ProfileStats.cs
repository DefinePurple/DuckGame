using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;

public class ProfileStats : MonoBehaviour {
    public Text displayName, kills, deaths, wins, losses;
    //IList<string> achievements = null;

    public void GetData() {
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
