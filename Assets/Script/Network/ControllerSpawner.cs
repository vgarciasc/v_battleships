using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public class ControllerSpawner : NetworkBehaviour {
    [Header("Prefabs")]
    public GameObject serverController;
    public GameObject msgTower;

    public override void OnStartServer() {
        GameObject go;
        go = Instantiate(serverController);
        NetworkServer.Spawn(go);

        go = Instantiate(msgTower);
        NetworkServer.Spawn(go);
    }
}
