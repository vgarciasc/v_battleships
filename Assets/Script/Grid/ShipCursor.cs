using UnityEngine;
using System.Collections;

public class ShipCursor : MonoBehaviour {
    public int[,] shipDesign;
    GridTile.State ship = GridTile.State.SHIP_MOVING;

    void Start() {
        transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        this.transform.position = Input.mousePosition;
    }

    void Update () {
        this.transform.position = Input.mousePosition;
    }

    public void updateDesign(int[,] newDesign) {
        shipDesign = newDesign;

        for (int i = 0; i < shipDesign.GetLength(0); i++) {
            for (int j = 0; j < shipDesign.GetLength(1); j++) {
                GameObject tile = transform.GetChild(i * shipDesign.GetLength(0) + j).gameObject;
                tile.GetComponent<GridTile>().state = ship;
                if (newDesign[i, j] == 0) MethodReference.switchVisibilityOfChildren(tile, false);
                else if (newDesign[i, j] == 1) MethodReference.switchVisibilityOfChildren(tile, true);
            }
        }
    }

    public void rotateDesign() {
        int size = shipDesign.GetLength(0);
        int[,] rot = new int[size, size];

        for (int i = size - 1; i >= 0; i--)
            for (int j = 0; j < size; j++)
                rot[j, size - 1 - i] = shipDesign[i, j];

        updateDesign(rot);
    }
}
