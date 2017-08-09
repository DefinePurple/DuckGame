using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggedIn : MonoBehaviour {

    private void OnEnable() {
        LobbyManager_Master lm_master = FindObjectOfType<GameSparksManager>().GetComponent<LobbyManager_Master>();

        if (GameSparksManager.Instance().isLoggedIn) {
            lm_master.CallEventGoToMain();
        }
    }
}
