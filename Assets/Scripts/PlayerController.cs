using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int gridX;
    public int gridY;
    public bool canMove = true;
    public List<CubeController> path;

    public GameObject warningText;

    // Initiate variables
    void Start() {
        gridX = GridController.instance.grid.WorldToCell(transform.position).x;
        gridY = GridController.instance.grid.WorldToCell(transform.position).z;

        path = new List<CubeController>();

        // Disable warning text
        warningText.SetActive(false);
    }

    public void pathFind(int endX, int endY) {

        if(canMove) {
            // Reset Cubes and clear path list
            GridController.instance.resetCubes();
            path = new List<CubeController>();

            // If null path received, show warning. else start moving with the path
            if((path = GridController.instance.pathFind(GridController.instance.getCube(gridX, gridY), endX, endY)) == null) {
                warningText.SetActive(true);
            } else {
                warningText.SetActive(false);
                StartCoroutine(move());
            }
        }
        
    }

    IEnumerator move() {

        canMove = false;

        while(path.Count > 0) { 
            // If something exists in path list
            // set position to next position in path
            transform.position = new Vector3(path[0].gridPosition.x, 0, path[0].gridPosition.z);

            // remove element from path list
            path.RemoveAt(0);

            // Update player variables
            gridX = GridController.instance.grid.WorldToCell(transform.position).x;
            gridY = GridController.instance.grid.WorldToCell(transform.position).z;

            // Delay for 500ms
            yield return new WaitForSeconds(.5f);
        }

        canMove = true;
    }
}
