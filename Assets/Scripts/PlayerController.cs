using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int gridX;
    public int gridY;
    public bool canMove = true;
    public List<CubeController> path;

    void Start() {
        gridX = GridController.instance.grid.WorldToCell(transform.position).x;
        gridY = GridController.instance.grid.WorldToCell(transform.position).z;
        path = new List<CubeController>();
    }

    public void pathFind(int endX, int endY) {
        if(canMove) {
            GridController.instance.resetCubes();
            path.Clear();
            GridController.instance.pathFind(GridController.instance.getPlayerCube(), endX, endY, path);

            // Start moving
            StartCoroutine(move());
        }
    }

    IEnumerator move() {

        canMove = false;

        while(path.Count > 0) {
            transform.position = new Vector3(path[0].gridPosition.x, 0, path[0].gridPosition.z);
            path.RemoveAt(0);
            gridX = GridController.instance.grid.WorldToCell(transform.position).x;
            gridY = GridController.instance.grid.WorldToCell(transform.position).z;
            yield return new WaitForSeconds(.5f);
        }

        canMove = true;
    }
}
