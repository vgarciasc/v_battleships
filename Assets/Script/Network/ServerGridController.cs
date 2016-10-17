using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class GridMessage : MessageBase
{
    public int[] grid, grid_2, grid_1_shipID;
    public int id;
    public int from_playerID;
    public int to_playerID;
}

public class TileMessage : MessageBase
{
    public int tileID;
    public int buffNumber;
    public bool hit, buff;
    public int from_playerID;
    public int to_playerID;
}

public class ServerGridController : NetworkBehaviour {
    MessageTower tow;
    int startPresses = 0;
    int gridsReceived = 0;
    public struct playerGrids { public int[] grid_1, grid_2, grid_1_shipID; public int playerID; };
    playerGrids aux;

    int turn = 0;

    void Start() {
        tow = MethodReference.findGameObject("Tower").GetComponentInChildren<MessageTower>();
        NetworkServer.RegisterHandler(101, receiveGridMessage);
        NetworkServer.RegisterHandler(102, manageStartGame);
        NetworkServer.RegisterHandler(103, receiveTileFire);
        NetworkServer.RegisterHandler(104, receiveTileFireFeedback);
        NetworkServer.RegisterHandler(105, receiveGrids);
        NetworkServer.RegisterHandler(106, giveVictory);
        NetworkServer.RegisterHandler(107, toggleTurns);
        NetworkServer.RegisterHandler(108, getBuffFeedback);
        NetworkServer.RegisterHandler(109, visualizeLine);
        NetworkServer.RegisterHandler(110, transferLine);
    }

    void receiveGridMessage(NetworkMessage msg) {
        GridMessage gm = msg.ReadMessage<GridMessage>();
        Debug.Log("From: " + gm.from_playerID);
        tow.RpcChangeSecondGrid(gm.grid, gm.from_playerID);
    }

    void manageStartGame(NetworkMessage nm) {
        if (startPresses == 0) {
            startPresses++;
            return;
        }

        tow.RpcStartGame();
    }

    void visualizeLine(NetworkMessage nm) {
        TileMessage msg = nm.ReadMessage<TileMessage>();
        tow.RpcGetLine(msg.tileID, msg.from_playerID);
    }

    void receiveTileFire(NetworkMessage msg) {
        TileMessage tm = msg.ReadMessage<TileMessage>();
        Debug.Log("Firing from: " + tm.from_playerID);
        tow.RpcFireAt(tm.tileID, tm.from_playerID);
    }

    void receiveTileFireFeedback(NetworkMessage msg) {
        TileMessage tm = msg.ReadMessage<TileMessage>();
        Debug.Log("Feedback from: " + tm.from_playerID);
        tow.RpcFireFeedback(tm.tileID, tm.hit, tm.buff, tm.from_playerID);
    }

    IEnumerator waitAndToggleTurns() {
        Debug.Log("Waiting for toggling turns...");

        turn++;
        if (turn % 5 == 0) {
            tow.RpcActivateSwapAnimation();
            yield return new WaitForSeconds(2.0f);
            tow.RpcPleaseSwapGrids();
            tow.RpcToggleTurn();
        } else {
            tow.RpcToggleTurn();
        }
    }

    void receiveGrids(NetworkMessage nm) {
        gridsReceived++;
        GridMessage msg = nm.ReadMessage<GridMessage>();

        if (gridsReceived == 1) { //primeiro player enviou sua grid
            aux.grid_1 = msg.grid;
            aux.grid_2 = msg.grid_2;
            aux.grid_1_shipID = msg.grid_1_shipID;
            aux.playerID = msg.from_playerID;
            return;
        } else { //segundo player enviou sua grid
            playerGrids p2;
            p2.grid_1 = msg.grid;
            p2.grid_2 = msg.grid_2;
            p2.playerID = msg.from_playerID;
            p2.grid_1_shipID = msg.grid_1_shipID;
            tow.RpcSwapGrids(aux, p2);
            gridsReceived = 0;
        }
    }

    void giveVictory(NetworkMessage nm) {
        tow.RpcSendVictory(nm.ReadMessage<IntegerMessage>().value);
    }

    void toggleTurns(NetworkMessage nm) {
        StartCoroutine(waitAndToggleTurns());
    }

    void getBuffFeedback(NetworkMessage nm) {
        TileMessage msg = nm.ReadMessage<TileMessage>();
        tow.RpcSendBuffNumber(msg.buffNumber, msg.from_playerID);
    }

    void transferLine(NetworkMessage nm)
    {
        GridMessage msg = nm.ReadMessage<GridMessage>();
        tow.RpcTransferLine(msg.grid, msg.id, msg.from_playerID);
        Debug.Log(msg.grid);
    }
}
