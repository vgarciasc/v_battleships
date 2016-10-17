using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridTile : MonoBehaviour {
    public enum State { SEA, SHIP, SHIP_MOVING, SHIP_BARRIER, DESTROYED, MISSED, ON_CURSOR, BUFF, BUFF_SHOT, SHIP_GHOST };

    public bool firingSquad;
    public int id = -1;
    public int shipID = -1;
    public bool barrier;
    public bool missed = false;
    public State state;
    public GridManager gm;

    void Start() {
        gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        //state = State.SEA;
    }

    public void Update() {
        Color c;
        if (barrier && state == State.SHIP)
            state = State.SHIP_BARRIER;

        switch (state) {
            case State.SHIP:
                //c = Color.green;
                c = gm.ship;
                break;
            case State.SHIP_MOVING:
                //c = Color.yellow;
                c = gm.ship_moving;
                break;
            case State.SHIP_BARRIER:
                //c = Color.yellow;
                c = gm.ship + new Color(0.1f, 0.1f, 0.1f);
                break;
            case State.SEA:
                //c = Color.white;
                c = gm.sea;
                if (missed) c = gm.sea + new Color(0.1f, 0.1f, 0.1f);
                break;
            case State.DESTROYED:
                //c = Color.red;
                c = gm.destroyed;
                break;
            case State.MISSED:
                //c = Color.cyan;
                c = gm.missed;
                break;
            case State.BUFF:
                //c = Color.cyan;
                c = gm.buffColor;
                break;
            case State.BUFF_SHOT:
                //c = Color.cyan;
                c = gm.buffColor + new Color(0.1f, 0.1f, 0.1f);
                break;
            default:
                c = Color.white;
                break;
        }

        if (firingSquad) c = new Color(c.r, c.g, c.b, 0.5f);
        this.GetComponentInChildren<Image>().color = c;
    }

    public void changeId(int num) {
        id = num;
        this.GetComponentInChildren<Text>().text = id.ToString();
    }

    public void click() {
        if (firingSquad) {
            GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
            gm.fire(id);
            return;
        }

        GetComponentInParent<GridManager>().registerClick(id);
    }

    public void reset() {
        shipID = -1;
        state = State.SEA;
    }

    public void barrify()
    {
        state = State.SHIP_BARRIER;
        barrier = true;
    }

    public void toggleMoving()
    {
        if (state == State.SHIP || state == State.SHIP_BARRIER)
        {
            state = State.SHIP_MOVING;
        }
        else if (state == State.SHIP_MOVING)
        {
            if (barrier)
                state = State.SHIP_BARRIER;
            else
                state = State.SHIP;
        }
        //else
        //    state = State.MISSED;
    }

    public void destroy() {
        if (state == State.SHIP_BARRIER) {
            state = State.SHIP;
            barrier = false;
        } else {
            state = State.DESTROYED;
            shipID = -1;
        }
    }
}