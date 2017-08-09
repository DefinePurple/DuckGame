using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager_Text : MonoBehaviour {

    public Text details;
    private LobbyManager_Master lm_master;

    private void OnEnable() {
        SetInitial();
        lm_master.EventUpdateText += UpdateText;
    }

    private void OnDisable() {
        lm_master.EventUpdateText -= UpdateText;
    }

    private void SetInitial() {
        lm_master = GetComponent<LobbyManager_Master>();
    }

    private void SetText(string text) {
        if (details != null)
            details.text = text;
    }

    private void UpdateText(string text) {
        if(details != null)
            details.text += text;
    }
}
