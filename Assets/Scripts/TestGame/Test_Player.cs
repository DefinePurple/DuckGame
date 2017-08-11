using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_Player : MonoBehaviour {

    public Button shootButton;
    [SerializeField] private Text username, status;
    private int peerId;
    public int GetPeerId() {
        return peerId;
    }
    private bool isPlayer, alive;
    public bool CheckIsPlayer() {
        return isPlayer;
    }

    public void PlayerSetup(string _username, bool _isPlayer, int _peerId ) {
        username.text = _username + " " + _peerId;
        status.text = true.ToString();
        alive = true;
        isPlayer = _isPlayer;
        peerId = _peerId;

        if (isPlayer)
            shootButton.gameObject.SetActive(false);
    }

    private void OnEnable() {
        shootButton.onClick.AddListener(() => {
            Debug.Log("Player " + peerId + " Shooting");
            Shoot();
        });
    }

    private void Shoot() {
        GameSparksManager.Instance().ShootTest(peerId);
    }

    private void Update() {
        if (!alive)
            shootButton.gameObject.SetActive(false);
    }

    public void CheckAlive(int senderPeerId) {
        alive = false;
        status.text = "Dead";
        GameSparksManager.Instance().DieTest(senderPeerId);
        SetDead();
    }

    public void SetDead() {
        alive = false;
        status.text = "Dead";
    }
}
