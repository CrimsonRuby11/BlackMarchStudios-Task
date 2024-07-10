using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : InterfaceAI
{
    public List<CubeController> path;

    public EnemyAI() {
        // Initiate variables
        path = new List<CubeController>();
    }

    public virtual List<CubeController> pathFind(int startX, int startY, int endX, int endY) {
        
        // Reset cubes
        GridController.instance.resetCubes();

        // Clear list
        path.Clear();

        // Get new path from pathFind
        path = GridController.instance.pathFind(GridController.instance.getCube(startX, startY), endX, endY);

        // return path
        return path;
    }
}
