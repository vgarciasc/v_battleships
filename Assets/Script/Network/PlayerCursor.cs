using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerCursor : NetworkBehaviour {
    public GameObject cursorIcon;

    void Start() {
        cursorIcon.GetComponent<Image>().color = new Color(Random.Range(0.7f, 1f), Random.Range(0f, 0.3f), Random.Range(0f, 0.3f));
        this.transform.SetParent(MethodReference.findGameObject("Canvas").transform, false);
    }

    void Update() {
        if (!isLocalPlayer) return;
        updatePosition();

        //if (Input.GetKeyDown(KeyCode.A))
        //    Cmd_changeNum(5);
        //if (Input.GetKeyDown(KeyCode.B))
        //    Cmd_changeNum(7);
    }

    void updatePosition() {
        this.transform.position = Input.mousePosition;
    }
}
