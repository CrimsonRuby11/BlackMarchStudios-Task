using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int gridX;
    public int gridY;

    void Start() {
        gridX = GridController.instance.grid.WorldToCell(transform.position).x;
        gridY = GridController.instance.grid.WorldToCell(transform.position).z;
    }

    public enum Dir {
        up = 0,
        down = 1,
        left = 2,
        right = 3,
    }

    public void startMovement() {
        // List of distances
        List<float> distances = new List<float>();
    }
}
