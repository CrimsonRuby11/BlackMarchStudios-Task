using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CubeController : MonoBehaviour
{
    [SerializeField]
    public Vector3Int gridPosition;

    public int visited = -1;
    public bool isObstacle = false;

    public void setGrid(int x, int y) {
        gridPosition = new Vector3Int(x, 0, y);
    }

    public void OnMouseDown() {
        // On mouse click, call pathFind on the player
        GridController.instance.playerC.pathFind(gridPosition.x, gridPosition.z);
    }
}
