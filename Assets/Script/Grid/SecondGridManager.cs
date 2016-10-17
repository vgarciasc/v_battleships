using UnityEngine;
using System.Collections;

public class SecondGridManager : GridSource {
    public void markFire(int tileID, bool hit, bool buff) {
        if (buff) {
            GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
            getTile(tileID).state = GridTile.State.BUFF_SHOT;
            gm.activateBuff();
            return;
        }

        if (hit)
            getTile(tileID).state = GridTile.State.DESTROYED;
        else
            getTile(tileID).state = GridTile.State.MISSED;
    }

    public void markLine(int[] line, int ID)
    {
        for (int i = 0; i < 10; i++)
        {
            int aux = ID - (ID % 10) + i;
            if (getTile(aux).state == GridTile.State.SEA &&
                (GridTile.State) line[aux] == GridTile.State.SHIP)
                getTile(aux).state = GridTile.State.SHIP_GHOST;
            if (getTile(aux).state == GridTile.State.SEA &&
                (GridTile.State)line[aux] == GridTile.State.BUFF)
                getTile(aux).state = GridTile.State.BUFF;
        }
    }
}
