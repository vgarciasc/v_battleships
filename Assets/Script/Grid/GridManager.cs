using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GridManager : GridSource {
    Dictionary<int, int[,]> id_design = new Dictionary<int, int[,]>();

    //Control variables
    public bool isShipOnCursor = false;
    public GameObject cursorShip;

    [Header("Colors")]
    public Color sea;
    public Color ship;
    public Color ship_moving;
    public Color destroyed;
    public Color buffColor = Color.magenta;
    public Color missed;

    int maxShips = 3;
    int currentShips = 0;

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject shipModel;

    [Header("------")]
    public bool nowFiring = false;
    public bool nowMoving = false;
    public bool shipSelected = false;
    public bool viewingLines = false;
    public bool undid = false;
    public bool yourTurn = false;
    public bool barrierSelect = false;

    public bool hasBuff = false;
    public bool setupPhase = true;
    public GameObject buffManager;
    public Text turnOperator;
    public Text fireOperator;
    public Text moveOperator;
    public GameObject actionsOperator;
    public GameObject victoryOperator;
    public Animator swap;
    public GameObject shipBanks;
    public Text buffText;
    public GameObject gridManagerPosition;

    [Header("Buttons")]
    public GameObject fire_button;
    public GameObject move_button;
    public GameObject start_button;
    public GameObject end_button;
    public GameObject undo_button;

    GameObject secondGridManager;

    int maxMoves = 2;
    int movesLeft;
    struct Movement { public int shipID; public Direction dir; };
    List<Movement> movements;

    enum Direction { Top = -10, Bottom = 10, Left = -1, Right = 1 };

    void Start() {
        movements = new List<Movement>();
        actionsOperator.SetActive(false);
        sumActions(maxMoves);
        secondGridManager = MethodReference.findGameObject("SecondGridManager");
        secondGridManager.SetActive(false);

        turnOperator.text = "PLACE YOUR SHIPS!";
        //turnOperator.enabled = false;
        fireOperator.enabled = false;
        moveOperator.enabled = false;

        buffManager.SetActive(false);
        move_button.SetActive(false);
        end_button.SetActive(false);
        fire_button.SetActive(false);
        undo_button.SetActive(false);
    }

    public void registerClick(int id) {
        Debug.Log("nowMoving: " + nowMoving);
        hasBuff = checkBuff();

        if (isShipOnCursor) placeShip(id);
        else if (setupPhase) getShip(id);
        else if (!hasBuff && yourTurn) putBuff(id);
        else if (nowMoving && yourTurn) select(id);
        else if (barrierSelect && yourTurn) barrifyShip(id);
    }

    void putBuff(int tileID) {
        if (getTile(tileID).state != GridTile.State.SEA) return;

        hasBuff = true;
        getTile(tileID).state = GridTile.State.BUFF;
        startTurn();
    }

    public void endTurn() {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) {
            PlayerGridController pgc = g.GetComponentInChildren<PlayerGridController>();
            pgc.sendEndTurn();
        }
    }

    void Update() {
        if (currentShips < maxShips)
            MethodReference.changeOpacity(start_button, 0.5f);
        else
            MethodReference.changeOpacity(start_button, 1f);

        if (Input.GetKeyDown(KeyCode.R) && isShipOnCursor)
            cursorShip.GetComponent<ShipCursor>().rotateDesign();

        if (Input.GetKeyDown(KeyCode.UpArrow))
            moveSelected(Direction.Top);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            moveSelected(Direction.Bottom);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            moveSelected(Direction.Right);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            moveSelected(Direction.Left);
    }

    void moveSelected(Direction dir) {
        if (movesLeft <= 0) return;

        List<int> aux = new List<int>();
        List<bool> aux_barriers = new List<bool>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (getTile(i).state == GridTile.State.SHIP_MOVING)
            {
                aux.Add(i);
                aux_barriers.Add(getTile(i).barrier);
            }
        }

        if (aux.Count == 0) return;

        List<int> aux2 = validMove(aux, dir);
        if (aux2 == null || aux.Count == 0) return;

        int shipID = getTile(aux[0]).shipID;

        for (int i = 0; i < aux.Count; i++)
            changeTile(aux[i], GridTile.State.SEA, -1, false);
        for (int i = 0; i < aux2.Count; i++)
            changeTile(aux2[i], GridTile.State.SHIP_MOVING, shipID, aux_barriers[i]);

        //for (int i = 0; i < aux.Count; i++)
        //    switchTiles(aux[i], aux2[i]);

        Movement mov;
        mov.dir = (Direction)(-1 * (int)dir);
        mov.shipID = getTile(aux2[0]).shipID;
        movements.Add(mov);

        setUndoability(true);
        sumActions(-1);
    }

    void switchTiles(int tileID_1, int tileID_2) {
        GridTile aux = getTile(tileID_1);

        getTile(tileID_1).state = getTile(tileID_2).state;
        getTile(tileID_1).barrier = getTile(tileID_2).barrier;
        getTile(tileID_1).shipID = getTile(tileID_2).shipID;

        getTile(tileID_2).state = aux.state;
        getTile(tileID_2).barrier = aux.barrier;
        getTile(tileID_2).shipID = aux.shipID;
    }

    void moveByID(Movement mow) {
        //Debug.Log(mow.dir);
        //Debug.Log(mow.shipID);
        List<int> aux = new List<int>();
        List<bool> aux_barriers = new List<bool>();

        for (int i = 0; i < transform.childCount; i++) {
            if (getTile(i).shipID == mow.shipID &&
                getTile(i).state != GridTile.State.DESTROYED)
            {
                Debug.Log("Id: " + i + ", barrier: " + getTile(i).barrier);
                aux.Add(i);
                aux_barriers.Add(getTile(i).barrier);
            }
        }
        if (aux.Count == 0) return;

        List<int> aux2 = validMove(aux, mow.dir);
        if (aux2 == null) return;

        sumActions(1);

        int shipID = getTile(aux[0]).shipID;

        for (int i = 0; i < aux.Count; i++)
            changeTile(aux[i], GridTile.State.SEA, -1, false);
        for (int i = 0; i < aux2.Count; i++)
            changeTile(aux2[i], GridTile.State.SHIP_MOVING, shipID, aux_barriers[i]);
    }

    public void undo() {
        //changeStates(GridTile.State.SHIP_MOVING, GridTile.State.SHIP);
        moveByID(movements[movements.Count - 1]);
        movements.RemoveAt(movements.Count - 1);
        //changeGrid(grid_saved);

        if (movements.Count == 0) setUndoability(false);
    }

    void setUndoability(bool zh) {
        undo_button.SetActive(zh);
    }

    List<int> validMove(List<int> currentPos, Direction dir) {
        List<int> newPos = new List<int>();
        bool output = true;
        int shipID = getTile(currentPos[0]).shipID;

        for (int i = 0; i < currentPos.Count; i++) {
            int aux = currentPos[i] + (int)dir;
            //Debug.Log("Aux: " + aux);
            if (aux >= transform.childCount ||
                aux < 0 ||
                (aux % 10 == 9 && currentPos[i] % 10 == 0) ||
                (aux % 10 == 0 && currentPos[i] % 10 == 9) ||
                (getTile(aux).state != GridTile.State.SEA &&
                getTile(aux).shipID != shipID))
                output = false;

            //if (output == false) Debug.Log("THE FALSEE IS: " + aux);
            newPos.Add(aux);
        }

        //Debug.Log("Output: " + output);

        if (output) return newPos;
        else return null;
    }

    public void startGame() {
        if (currentShips != maxShips) return;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) {
            PlayerGridController pgc = g.GetComponentInChildren<PlayerGridController>();
            pgc.sendReady();
        }

        setupPhase = false;
        start_button.SetActive(false);
        turnOperator.enabled = true;

        secondGridManager.SetActive(true);
        shipBanks.SetActive(false);
        transform.localPosition = new Vector3(transform.localPosition.x - 160, transform.localPosition.y);
        turnOperator.text = "OPPONENT'S TURN!";
    }

    public void fire(int tileID) {
        if (viewingLines)
        {
            selectLineToView(tileID);
            return;
        }
        if (!nowFiring || movesLeft <= 0) return;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerGridController pgc = g.GetComponentInChildren<PlayerGridController>();
            pgc.fireAt(tileID);
        }
    }

    public void sendGrid() {
        SecondGridManager sgm = MethodReference.findGameObject("SecondGridManager").GetComponent<SecondGridManager>();
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerGridController pgc = g.GetComponentInChildren<PlayerGridController>();
            pgc.sendGrid(getGrid(), sgm.getGrid(), getGridShipID());
        }
    }

    public void placeShip(int tileID_central) {
        List<int> shipTiles = new List<int>();
        ShipCursor cs = cursorShip.GetComponent<ShipCursor>();

        for (int i = 0; i < cs.shipDesign.GetLength(0); i++)
            if (cs.shipDesign[0, i] != 0) shipTiles.Add(tileID_central - 11 + i);
        for (int i = 0; i < cs.shipDesign.GetLength(0); i++)
            if (cs.shipDesign[1, i] != 0) shipTiles.Add(tileID_central - 1 + i);
        for (int i = 0; i < cs.shipDesign.GetLength(0); i++)
            if (cs.shipDesign[2, i] != 0) shipTiles.Add(tileID_central + 9 + i);
        for (int i = 0; i < cs.shipDesign.GetLength(0); i++)
            if (cs.shipDesign[3, i] != 0) shipTiles.Add(tileID_central + 19 + i);


        //for (int i = 0; i < shipTiles.Count; i++)
        //    Debug.Log(shipTiles[i]);
        if (!validPlacement(shipTiles, tileID_central)) return;

        int shipID = id_design.Count;
        //adiciona navio ao dicionario para resgatar depois
        id_design.Add(shipID, cs.shipDesign);

        for (int i = 0; i < shipTiles.Count; i++)
            changeTile(shipTiles[i], GridTile.State.SHIP, shipID);

        isShipOnCursor = false;
        Destroy(cursorShip);
        currentShips++;
    }

    bool validPlacement(List<int> tiles, int id) {
        bool zero = false;
        bool nine = false;

        for (int i = 0; i < tiles.Count; i++) {
            if (tiles[i] % 10 == 0)
                zero = true;
            else if (tiles[i] % 10 == 9)
                nine = true;
        }

        if (zero && nine) return false;

        //if ((id % 10 == 0 || id % 10 == 9)))
        //    return false;

        for (int i = 0; i < tiles.Count; i++)
            if (tiles[i] > transform.childCount ||
                tiles[i] < 0 ||
                getTile(tiles[i]).state != GridTile.State.SEA)
                return false;

        return true;
    }

    void changeTile(int tileID, GridTile.State st, int shipID) {
        getTile(tileID).state = st;
        getTile(tileID).shipID = shipID;
    }

    void changeTile(int tileID, GridTile.State st, int shipID, bool barrier)
    {
        getTile(tileID).state = st;
        getTile(tileID).shipID = shipID;
        getTile(tileID).barrier = barrier;
    }

    void getShip(int tileID) {
        int shipID = getTile(tileID).shipID;
        if (shipID == -1) return;

        foreach (Transform child in this.transform)
            if (child.GetComponent<GridTile>().shipID == shipID)
                child.GetComponent<GridTile>().reset();

        currentShips--;
        addShipToCursor(id_design[shipID]);
    }

    public void receiveFire(int tileID) {
        bool hit;
        bool buff = false;

        if (getTile(tileID).state == GridTile.State.SHIP || getTile(tileID).state == GridTile.State.SHIP_BARRIER) {
            getTile(tileID).destroy();
            hit = true;
        } else if (getTile(tileID).state == GridTile.State.BUFF) {
            changeTile(tileID, GridTile.State.SEA, -1);
            hit = true;
            buff = true;
            hasBuff = false;
        } else if (getTile(tileID).state == GridTile.State.SEA) {
            getTile(tileID).missed = true;
            hit = false;
        } else {
            Debug.Log("Missed at 'tileID: " + tileID + "'!");
            hit = false;
        }

        if (gameLost())
            endGame(false);

        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerGridController pgc = g.GetComponentInChildren<PlayerGridController>();
            pgc.fireFeedback(tileID, hit, buff);
        }
    }

    public void activateBuff() {
        Debug.Log("You shot a buff!");
        //int buffNumber = Random.Range(0, System.Enum.GetNames(typeof(BuffManager.BuffTypes)).Length);
        int buffNumber = 0;
        buffManager.GetComponent<BuffManager>().currentBuff = (BuffManager.BuffTypes) buffNumber;
        buffManager.GetComponent<BuffManager>().containBuff = true;
        buffText.text = "You got " + buffManager.GetComponent<BuffManager>().currentBuff;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerGridController pgc = g.GetComponentInChildren<PlayerGridController>();
            pgc.buffActivated(buffNumber);
        }
    }

    bool gameLost() {
        for (int i = 0; i < transform.childCount; i++)
            if (getTile(i).state == GridTile.State.SHIP) return false;

        return true;
    }

    public void endGame(bool victory) {
        victoryOperator.SetActive(true);

        if (!victory) {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) {
                PlayerGridController pgc = g.GetComponentInChildren<PlayerGridController>();
                pgc.sendMessageLost();
            }
            victoryOperator.GetComponentInChildren<Text>().text = "YOU LOSE";
        } else {
            victoryOperator.GetComponentInChildren<Text>().text = "YOU ARE GREAT! CONGRATULATIONS";
        }
    }

    public void addShipToCursor(int[,] tileArray) {
        if (currentShips >= maxShips) return;
        if (isShipOnCursor) {
            isShipOnCursor = false;
            Destroy(cursorShip);
        }

        GameObject newShip = Instantiate(shipModel);
        newShip.transform.SetParent(MethodReference.findGameObject("Canvas").transform, false);

        for (int i = 0; i < tileArray.GetLength(0); i++) {
            for (int j = 0; j < tileArray.GetLength(1); j++) {
                GameObject tile = Instantiate(tilePrefab);
                tile.transform.SetParent(newShip.transform, false);
            }
        }

        newShip.GetComponent<ShipCursor>().updateDesign(tileArray);
        isShipOnCursor = true;
        cursorShip = newShip;

        newShip.transform.localPosition = Input.mousePosition;
    }

    public void select(int tileID) {
        Debug.Log("select: " + tileID);
        int shipID = getTile(tileID).shipID;
        bool isThisTheOneSelected = false;

        if (shipSelected) {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (getTile(i).shipID == shipID && getTile(i).state == GridTile.State.SHIP_MOVING) //de-selecting ship, now can move others
                    isThisTheOneSelected = true;
            }

            if (!isThisTheOneSelected) changeStates(GridTile.State.SHIP_MOVING, GridTile.State.SHIP);
        }

        for (int i = 0; i < transform.childCount; i++)
            if (getTile(i).shipID == shipID)
                getTile(i).toggleMoving();

        Debug.Log("toggledMoving");

        shipSelected = !isThisTheOneSelected;
    }

    public void toggleFiring() {
        nowFiring = !nowFiring;
        fireOperator.enabled = !fireOperator.enabled;
    }

    public void toggleMoving() {
        if (nowMoving)
        {
            shipSelected = false;
            movements.Clear();
            undo_button.SetActive(false);
            for (int i = 0; i < transform.childCount; i++)
                if (getTile(i).state == GridTile.State.SHIP_MOVING)
                    getTile(i).toggleMoving();
        }

        nowMoving = !nowMoving;
        moveOperator.enabled = !moveOperator.enabled;
    }

    public void toggleTurn() {
        changeStates(GridTile.State.SHIP_MOVING, GridTile.State.SHIP);
        if (yourTurn) {
            undo_button.SetActive(false);
            moveOperator.enabled = false;
            fireOperator.enabled = false;
            nowFiring = false;
            movesLeft = maxMoves;
            updateActions();
            turnOperator.text = "OPPONENT'S TURN!";
            switchButtons(false);
        }
        else {
            hasBuff = checkBuff();
            shipSelected = false;
            nowMoving = false;

            if (!hasBuff)
                turnOperator.text = "PUT BUFF";
            else
                startTurn();
        }

        yourTurn = !yourTurn;
        actionsOperator.SetActive(!actionsOperator.activeSelf);
    }

    void startTurn() {
        turnOperator.text = "YOUR TURN!";
        switchButtons(true);
    }

    void switchButtons(bool zh)
    {
        buffManager.SetActive(zh);
        end_button.SetActive(zh);
        move_button.SetActive(zh);
        fire_button.SetActive(zh);
    }

    public void activateSwap() {
        changeStates(GridTile.State.SHIP_MOVING, GridTile.State.SHIP);
        swap.SetTrigger("swap");
    }

    void changeShip(int shipID, GridTile.State newState) {
        for (int i = 0; i < transform.childCount; i++)
            if (getTile(i).shipID == shipID)
                getTile(i).state = newState;
    }

    public void feedbackFire(bool hit) {
        //if (!hit)
        sumActions(-1);
    }

    void sumActions(int num) {
        movesLeft += num;
        updateActions();
    }

    void updateActions() {
        actionsOperator.GetComponentsInChildren<Text>()[1].text = movesLeft.ToString();
        if (movesLeft == 0)
        {
            bool zh = false;
            move_button.SetActive(zh);
            fire_button.SetActive(zh);
        }
        else
        {
            bool zh = true;
            move_button.SetActive(zh);
            fire_button.SetActive(zh);
        }
    }

    void changeStates(GridTile.State oldState, GridTile.State newState) {
        for (int i = 0; i < transform.childCount; i++)
            if (getTile(i).state == oldState)
                getTile(i).state = newState;
    }

    public void enemyActivatedBuff(int buffNumber)
    {
        buffText.text = "Enemy got buff " + (BuffManager.BuffTypes) buffNumber + "!";
    }

    public void selectBarrierShip()
    {
        barrierSelect = true;
        buffText.text = "Select ship to hold barrier";
    }

    void barrifyShip(int tileID) {
        if (getTile(tileID).state != GridTile.State.SHIP) return;

        for (int i = 0; i < transform.childCount; i++)
            if (getTile(i).shipID == getTile(tileID).shipID)
                getTile(i).barrify();

        hasBuff = false;
        barrierSelect = false;
        buffManager.GetComponent<BuffManager>().containBuff = false;
        buffText.text = "Barrier selected. No buff.";
    }

    bool checkBuff()
    {
        for (int i = 0; i < transform.childCount; i++)
            if (getTile(i).state == GridTile.State.BUFF)
                return true;

        return false;
    }

    public void activateBonusAction()
    {
        if (!hasBuff) return; 
        buffText.text = "Bonus action activated! No buff.";
        sumActions(2);
        hasBuff = false;
    }

    public void selectLineToView(int tileID)
    {
        viewingLines = false;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerGridController pgc = g.GetComponentInChildren<PlayerGridController>();
            pgc.visualizeLine(tileID);
        }
        buffText.text = "Line is now in vision. No buff.";
    }

    public void viewEnemyLine()
    {
        buffText.text = "Select enemy line to view.";
        hasBuff = false;
        viewingLines = true;
    }

    public void getLine(int tileID)
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerGridController pgc = g.GetComponentInChildren<PlayerGridController>();
            pgc.sendLine(getGrid(), tileID);
        }
    }
}