using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MessageTower : NetworkBehaviour {
    [ClientRpc]
    public void RpcChangeSecondGrid(int[] grid, int playerID) {
        SecondGridManager sgm = MethodReference.findGameObject("SecondGridManager").GetComponentInChildren<SecondGridManager>();

        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            if (p.GetComponent<PlayerIdentifier>().playerID == playerID)
                return;

        sgm.changeGrid(grid);

        //GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        //gm.toggleTurn();
    }

    [ClientRpc]
    public void RpcStartGame() {
        GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        if (isServer)
            gm.toggleTurn();
    }

    [ClientRpc]
    public void RpcFireAt(int tileID, int from_playerID) {
        GridManager gm = MethodReference.findGameObject("GridManager").GetComponentInChildren<GridManager>();
        bool playerWhoFired = false;

        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            if (p.GetComponent<PlayerIdentifier>().playerID == from_playerID)
                playerWhoFired = true;

        if (!playerWhoFired)
            gm.receiveFire(tileID);
    }

    [ClientRpc]
    public void RpcFireFeedback(int tileID, bool hit, bool buff, int from_playerID) {
        GridManager gm = MethodReference.findGameObject("GridManager").GetComponentInChildren<GridManager>();
        SecondGridManager sgm = MethodReference.findGameObject("SecondGridManager").GetComponentInChildren<SecondGridManager>();
        bool playerWhoFeedbacked = false;

        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            if (p.GetComponent<PlayerIdentifier>().playerID == from_playerID)
                playerWhoFeedbacked = true;

        if (!playerWhoFeedbacked)
        {
            gm.feedbackFire(hit);
            sgm.markFire(tileID, hit, buff);
        }
    }

    [ClientRpc]
    public void RpcToggleTurn() {
        GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        gm.toggleTurn();
    }

    [ClientRpc]
    public void RpcSendGrids() {
        GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        gm.sendGrid();
    }

    [ClientRpc]
    public void RpcSwapGrids(ServerGridController.playerGrids p1, ServerGridController.playerGrids p2) {
        int player = 2;
        GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        SecondGridManager sgm = MethodReference.findGameObject("SecondGridManager").GetComponentInChildren<SecondGridManager>();

        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            if (p.GetComponent<PlayerIdentifier>().playerID == p1.playerID)
                player = 1;

        if (player == 1) {
            gm.changeGrid(p2.grid_1);
            gm.changeGridShipID(p2.grid_1_shipID);
            sgm.changeGrid(p2.grid_2);
        } else {
            gm.changeGrid(p1.grid_1);
            gm.changeGridShipID(p1.grid_1_shipID);
            sgm.changeGrid(p1.grid_2);
        }
    }

    [ClientRpc]
    public void RpcPleaseSwapGrids() {
        GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        gm.sendGrid();
    }

    [ClientRpc]
    public void RpcActivateSwapAnimation() {
        GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        gm.activateSwap();
    }

    [ClientRpc]
    public void RpcSendVictory(int loserID) {
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            if (p.GetComponent<PlayerIdentifier>().playerID == loserID)
                return;

        GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        gm.endGame(true);
    }

    [ClientRpc]
    public void RpcSendBuffNumber(int buffNumber, int from_playerID) {
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            if (p.GetComponent<PlayerIdentifier>().playerID == from_playerID)
                return;

        GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        gm.enemyActivatedBuff(buffNumber);
    }

    [ClientRpc]
    public void RpcGetLine(int tileID, int from_playerID) {
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            if (p.GetComponent<PlayerIdentifier>().playerID == from_playerID)
                return;

        GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();
        gm.getLine(tileID);
    }

    [ClientRpc]
    public void RpcTransferLine(int[] grid, int shipID, int from_playerID)
    {
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
            if (p.GetComponent<PlayerIdentifier>().playerID == from_playerID)
                return;

        SecondGridManager sgm = MethodReference.findGameObject("SecondGridManager").GetComponentInChildren<SecondGridManager>();
        sgm.markLine(grid, shipID);
    }
}
