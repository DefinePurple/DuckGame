using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    #region Singleton
    private static GameController instance; // this is the singleton for the game-controller class
    public static GameController Instance() { return instance; }
    void Awake() { instance = this; }
    #endregion

    [SerializeField] private GameObject prefab, parent;
    private Test_Player[] playerList;
    // Use this for initialization
    void Start() {
        #region Setup Players
        RTSessionInfo sessionInfo = GameSparksManager.Instance().GetSessionInfo();
        playerList = new Test_Player[(int)sessionInfo.GetPlayerList().Count];

        for (int playerIndex = 0; playerIndex < sessionInfo.GetPlayerList().Count; playerIndex++) { // loop through all players
                // instantiate a new player-tank at the spawner's position and rotation. Rotation is important to make sure the player is facing the right direction //
                GameObject newPlayer = Instantiate(prefab);

                newPlayer.transform.SetParent(parent.transform); // add the tank to the game-controller objects to clean up the scene
                                                                 // If the player's peerId is the same as this player's peerId then we know this is the player and we can setup players and opponents //
                if (sessionInfo.GetPlayerList()[playerIndex].peerID == GameSparksManager.Instance().GetRTSession().PeerId) {
                    newPlayer.GetComponent<Test_Player>().PlayerSetup(sessionInfo.GetPlayerList()[playerIndex].displayName, true, sessionInfo.GetPlayerList()[playerIndex].peerID);
                } else {
                    newPlayer.GetComponent<Test_Player>().PlayerSetup(sessionInfo.GetPlayerList()[playerIndex].displayName, false, sessionInfo.GetPlayerList()[playerIndex].peerID);
                }

                playerList[playerIndex] = newPlayer.GetComponent<Test_Player>(); // add the new tank object to the corresponding refernce in the list
        }
        #endregion
    }

    public void ReceiveShot(int peerId, int senderPeerId) {
        Debug.Log("Received Shot of " + peerId + " by " + senderPeerId);
        for(int i = 0; i < playerList.Length; i++) {
            if(playerList[i].GetPeerId() == peerId) {
                playerList[i].CheckAlive(senderPeerId);
            }
        }
    }

    public void ReceiveDeath(int peerId) {
        Debug.Log("Received Death of " + peerId);
        for(int i = 0; i < playerList.Length; i++) {
            if(playerList[i].GetPeerId() == peerId) {
                playerList[i].SetDead();
            }
        }
    }
}
