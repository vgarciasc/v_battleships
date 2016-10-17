using UnityEngine;
using System.Collections;

public class GridSource : MonoBehaviour {
    public void changeGrid(int[] grid) {
        for (int i = 0; i < grid.Length; i++)
        {
            this.transform.GetChild(i).GetComponent<GridTile>().state = (GridTile.State)grid[i];
            if (getTile(i).state == GridTile.State.SHIP_BARRIER)
                getTile(i).barrier = true;
            else
                getTile(i).barrier = false;
        }
    }

    public void changeGridShipID(int[] grid) {
        for (int i = 0; i < grid.Length; i++)
            getTile(i).shipID = grid[i];
    }

    public int[] getGrid() {
        int[] aux = new int[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            aux[i] = (int) getTile(i).state;

        return aux;
    }

    public int[] getGridShipID() {
        int[] aux = new int[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
            aux[i] = getTile(i).shipID;

        return aux;
    }

    public GridTile getTile(int tileID) {
        if (tileID < 0 || tileID >= transform.childCount) return null;
        return this.transform.GetChild(tileID).GetComponent<GridTile>();
    }
}
