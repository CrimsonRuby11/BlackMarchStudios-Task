using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField]
    private Vector3Int gridPosition;

    public void setGrid(int x, int y) {
        gridPosition = new Vector3Int(x, 0, y);
    }

    public Vector3Int getGrid() {
        return gridPosition;
    }
}
