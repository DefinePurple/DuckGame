using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
public class BuyVirtualGood : MonoBehaviour {

    [SerializeField] private string currencyShortCode;
    [SerializeField] private string shortCode;
    [SerializeField] private int quantity;
    [SerializeField] private Button buyButton;
    private LobbyManager_Master lm_master;

    private void OnEnable() {
        SetInitial();
    }

    private void SetInitial() {
        lm_master = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<LobbyManager_Master>();
    }

    private void Start() {
        buyButton.onClick.AddListener(() => {
            new BuyVirtualGoodsRequest()
            .SetCurrencyShortCode(currencyShortCode)
            .SetQuantity(quantity)
            .SetShortCode(shortCode)
            .Send((response) => {
                Debug.Log(response.JSONString);
                if (response.HasErrors)
                    Debug.Log("Purchase Failed");
            });

            lm_master.CallEventUpdateEggs();
        });
    }
}
