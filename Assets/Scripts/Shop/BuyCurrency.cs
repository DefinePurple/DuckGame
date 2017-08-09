using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class BuyCurrency : MonoBehaviour {

    [SerializeField] private string eventKey;
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
            new LogEventRequest()
            .SetEventKey(eventKey)
            .Send((response) => {
                Debug.Log(response);
            });

            lm_master.CallEventUpdateEggs();
        });
    }
}
