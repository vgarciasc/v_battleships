using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridCreator : MonoBehaviour {
    [Header("Prefabs")]
    public GameObject tilePrefab;

    public bool firing = false;

    public int gridSize = 10;

    void Start () {
        generateGrid();
	}

    void generateGrid() {
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                GameObject go = Instantiate(tilePrefab);
                go.transform.SetParent(this.transform, false);
                go.GetComponent<GridTile>().changeId(i * gridSize + j);
                go.GetComponent<GridTile>().firingSquad = firing;
            }
        }
    }
}
