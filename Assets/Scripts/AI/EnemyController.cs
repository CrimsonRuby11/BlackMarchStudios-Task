using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyAI ai;
    [SerializeField]
    PlayerController playerC;

    bool moving = false;
    Vector3Int lastPlayerPos;
    Vector3Int lastEnemyPos;
    Vector3Int currentEnemyPos;
    Vector3Int currentPlayerPos;
    List<CubeController> path;

    void Start() {
        ai = new EnemyAI();
        path = new List<CubeController>();

        lastPlayerPos = new Vector3Int(playerC.gridX, 0, playerC.gridY);
        currentEnemyPos = new Vector3Int(GridController.instance.grid.WorldToCell(transform.position).x, 0, GridController.instance.grid.WorldToCell(transform.position).z);
        lastEnemyPos = currentEnemyPos;

        GridController.instance.setObstacle(currentEnemyPos.x, currentEnemyPos.z, true);
    }

    void Update() {
        currentPlayerPos = new Vector3Int(playerC.gridX, 0, playerC.gridY);
        currentEnemyPos = new Vector3Int(GridController.instance.grid.WorldToCell(transform.position).x, 0, GridController.instance.grid.WorldToCell(transform.position).z);
        
        if(currentPlayerPos != lastPlayerPos) {
            // cancel previous movement
            lastPlayerPos = currentPlayerPos;
            // find new path
            this.path.Clear();
            this.path = ai.pathFind(GridController.instance.grid.WorldToCell(transform.position).x, GridController.instance.grid.WorldToCell(transform.position).z, currentPlayerPos.x, currentPlayerPos.z);
            // start moving
            if(!moving) {
                StartCoroutine(enemyMove());
            }
            
        }

        if(lastEnemyPos != currentEnemyPos) {
            GridController.instance.setObstacle(lastEnemyPos.x, lastEnemyPos.z, false);
            GridController.instance.setObstacle(currentEnemyPos.x, currentEnemyPos.z, true);
        }
    }

    IEnumerator enemyMove() {
        moving = true;

        while(path.Count > 0) {
            // Check if next position has player
            int nextPosX = path[0].gridPosition.x;
            int nextPosY = path[0].gridPosition.z;

            if(playerC.gridX == nextPosX && playerC.gridY == nextPosY) {
                path.RemoveAt(0);
                break;
            }
            
            transform.position = new Vector3(nextPosX, 0, nextPosY);
            path.RemoveAt(0);
            yield return new WaitForSeconds(.5f);
        }

        moving = false;

    }


}
