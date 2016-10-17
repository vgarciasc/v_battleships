using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipBank : MonoBehaviour {
    GridManager gm;
    public int size = 4;

    public int[] row_0;
    public int[] row_1;
    public int[] row_2;
    public int[] row_3;
    int[,] tileArray;

    void Start() {
        tileArray = new int[size, size];

        gm = MethodReference.findGameObject("GridManager").GetComponentInChildren<GridManager>();
        for (int i = 0; i < size; i++) {
            tileArray[0, i] = row_0[i];
            tileArray[1, i] = row_1[i];
            tileArray[2, i] = row_2[i];

            if (size > 3)
                tileArray[3, i] = row_3[i];
        }

        Transform ship = transform.GetChild(0);
        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (tileArray[i, j] == 0)
                    ship.GetChild(i * size + j).gameObject.GetComponent<Image>().enabled = false;
                else
                    ship.GetChild(i * size + j).gameObject.GetComponent<Image>().enabled = true;
            }
        }
    }

    public void clickBank() {
        gm.addShipToCursor(tileArray);
    }
}
