using UnityEngine;
using System.Collections;

public class BuffManager : MonoBehaviour {
    public enum BuffTypes { Barrier, Bonus_Action, Visibility };
    public BuffTypes currentBuff;
    public bool containBuff = false;

    public void useBuff() {
        if (!containBuff) return;
        GridManager gm = MethodReference.findGameObject("GridManager").GetComponent<GridManager>();

        switch (currentBuff) {
            case BuffTypes.Barrier:
                gm.selectBarrierShip();
                break;
            case BuffTypes.Bonus_Action:
                gm.activateBonusAction();
                break;
            case BuffTypes.Visibility:
                gm.viewEnemyLine();
                break;
            default:
                Debug.Log("Doesn't have buff");
                break;
        }
    }

}
