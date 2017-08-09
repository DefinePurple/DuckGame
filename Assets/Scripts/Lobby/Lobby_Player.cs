using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby_Player : MonoBehaviour {

    [SerializeField] private Text displayName;
    public void SetDisplayName(string name) {
        displayName.text = name;
    }

    private string peerId;
    public void SetPeerId(string peer) {
        peerId = peer;
    }
    public string GetPeerId() {
        return peerId;
    }
    
    public bool ready;
    public bool player;
    private Image image;

    private void Start() {
        image = this.gameObject.GetComponent<Image>();
    }

    private void Update() {
        if (ready && player)
            image.color = new Color(0, 255, 0);
        else if (!ready && player)
            image.color = new Color(255, 0, 0);
    }

    public void ResetText() {
        displayName.text = "";
    }
}
