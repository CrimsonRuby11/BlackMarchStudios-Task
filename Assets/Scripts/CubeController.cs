using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public Vector3Int gridPosition;
    public int visited = -1;
    public bool isObstacle = false;

    public void setGrid(int x, int y) {
        gridPosition = new Vector3Int(x, 0, y);
    }

    public Vector3Int getGrid() {
        return gridPosition;
    }

    public void OnMouseDown() {
        GridController.instance.setDistance();
        GridController.instance.setPath(gridPosition.x, gridPosition.z);

        GridController.instance.playerC.startMovement();
    }
}
