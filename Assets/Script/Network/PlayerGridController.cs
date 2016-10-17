using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;
using System.Text;

public class PlayerGridController : NetworkBehaviour {
    public void sendNumber() {
        if (!isLocalPlayer) return;

        PlayerIdentifier PI = this.GetComponent<PlayerIdentifier>();

        GridMessage msg = new GridMessage();
        msg.grid = MethodReference.findGameObject("GridManager").GetComponent<GridManager>().getGrid();
        msg.from_playerID = PI.getPlayerID();

        NetworkClient.allClients[0].Send(101, msg);
    }

    public void sendReady() {
        if (!isLocalPlayer) return;

        IntegerMessage msg = new IntegerMessage(GetComponent<PlayerIdentifier>().playerID);
        NetworkClient.allClients[0].Send(102, msg);
    }

    public void fireAt(int tileID) {
        if (!isLocalPlayer) return;

        Debug.Log("Now firing at 'tileID: " + tileID + "'!");
        TileMessage msg = new TileMessage();
        msg.tileID = tileID;
        msg.from_playerID = GetComponent<PlayerIdentifier>().getPlayerID();

        NetworkClient.allClients[0].Send(103, msg);
    }

    public void fireFeedback(int tileID, bool hit, bool buff) {
        if (!isLocalPlayer) return;

        Debug.Log("Giving feedback to fire at 'tileID: " + tileID + "'!");
        TileMessage msg = new TileMessage();
        msg.tileID = tileID;
        msg.from_playerID = GetComponent<PlayerIdentifier>().getPlayerID();
        msg.hit = hit;
        msg.buff = buff;

        NetworkClient.allClients[0].Send(104, msg);
    }

    public void sendGrid(int[] grid_1, int[] grid_2, int[] grid_1_shipID) {
        if (!isLocalPlayer) return;

        GridMessage msg = new GridMessage();
        msg.grid = grid_1;
        msg.grid_2 = grid_2;
        msg.grid_1_shipID = grid_1_shipID;
        msg.from_playerID = GetComponent<PlayerIdentifier>().getPlayerID();

        NetworkClient.allClients[0].Send(105, msg);
    }

    public void sendMessageLost() {
        if (!isLocalPlayer) return;

        IntegerMessage msg = new IntegerMessage(GetComponent<PlayerIdentifier>().getPlayerID());
        NetworkClient.allClients[0].Send(106, msg);
    }

    public void sendEndTurn()
    {
        if (!isLocalPlayer) return;

        IntegerMessage msg = new IntegerMessage(GetComponent<PlayerIdentifier>().getPlayerID());
        NetworkClient.allClients[0].Send(107, msg);
    }

    public void buffActivated(int buffNumber)
    {
        TileMessage msg = new TileMessage();
        msg.buffNumber = buffNumber;
        msg.from_playerID = GetComponent<PlayerIdentifier>().getPlayerID();
        NetworkClient.allClients[0].Send(108, msg);
    }

    public void visualizeLine(int lineID)
    {
        TileMessage msg = new TileMessage();
        msg.from_playerID = GetComponent<PlayerIdentifier>().getPlayerID();
        msg.tileID = lineID;
        NetworkClient.allClients[0].Send(109, msg);
    }

    public void sendLine(int[] line, int tileID)
    {
        GridMessage msg = new GridMessage();
        msg.grid = line;
        msg.id = tileID;
        msg.from_playerID = GetComponent<PlayerIdentifier>().getPlayerID();
        NetworkClient.allClients[0].Send(110, msg);
    }
}
