using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyAI ai;

    // Inspector references
    [SerializeField]
    PlayerController playerC;

    // Private variables
    bool moving = false;
    Vector3Int lastPlayerPos;
    Vector3Int currentPlayerPos;

    [SerializeField]
    List<CubeController> path;

    void Start() {
        // Initiate variables
        ai = new EnemyAI();
        path = new List<CubeController>();

        lastPlayerPos = new Vector3Int(playerC.gridX, 0, playerC.gridY);
    }

    void Update() {
        // Update current pos accordingly
        currentPlayerPos = new Vector3Int(playerC.gridX, 0, playerC.gridY);
        
        // if player has moved, find a new path for enemy to follow
        if(currentPlayerPos != lastPlayerPos) {

            // Update enemy pos
            lastPlayerPos = currentPlayerPos;

            // find new path
            this.path.Clear();
            this.path = ai.pathFind(GridController.instance.grid.WorldToCell(transform.position).x, GridController.instance.grid.WorldToCell(transform.position).z, currentPlayerPos.x, currentPlayerPos.z);
            
            // start moving
            if(!moving) {
                StartCoroutine(enemyMove());
            }
        }
    }

    IEnumerator enemyMove() {
        // Starts moving
        moving = true;

        while(path.Count > 0) {
            // If list is not empty, move to the next position in list
            // cache positions
            int nextPosX = path[0].gridPosition.x;
            int nextPosY = path[0].gridPosition.z;

            // If next position is player position, stop moving
            if(playerC.gridX == nextPosX && playerC.gridY == nextPosY) {
                path.RemoveAt(0);
                break;
            }
            
            // else,
            // move th enemy to next position in list
            transform.position = new Vector3(nextPosX, 0, nextPosY);

            // Remove the element from the list
            path.RemoveAt(0);

            // Delay for 500ms
            yield return new WaitForSeconds(.5f);
        }

        // Stop moving
        moving = false;

    }


}
