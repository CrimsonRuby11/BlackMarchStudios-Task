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

    [SerializeField]
    private float hCost;
    [SerializeField]
    private float gCost;

    public float fCost {get; private set;}

    private CubeController previousCube;

    public void setGrid(int x, int y) {
        gridPosition = new Vector3Int(x, 0, y);
    }

    public void OnMouseDown() {
        // On mouse click, call pathFind on the player
        GridController.instance.playerC.pathFind(gridPosition.x, gridPosition.z);
    }

    public void setCosts(float g, float h, float f) {
        gCost = g;
        hCost = h;
        fCost = f;
    }
}
