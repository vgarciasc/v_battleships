using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

public class PlayerIdentifier : NetworkBehaviour {
    public int playerID = -1;

    void Start() {
        if (!isLocalPlayer) return;

        playerID = Random.Range(0, 99999);
        Debug.Log("PlayerID: " + playerID);
    }

    public bool isIDLocalPlayer(int playerID) {
        if (!isLocalPlayer) return false;
        return (this.playerID == playerID);
    }

    public int getPlayerID() {
        if (!isLocalPlayer) return -1;
        return this.playerID;
    }

    public GameObject getOtherPlayer() {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) 
            if (g.GetComponent<PlayerIdentifier>().playerID == -1)
                return g;

        Debug.Log("Only 1 player.");
        return null;
    }

    //public GameObject getOtherPlayer() {
    //    foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
    //        if (g.GetComponent<PlayerIdentifier>().playerID == -1)
    //            return g.GetComponent<PlayerIdentifier>().playerID;
    //}
}
