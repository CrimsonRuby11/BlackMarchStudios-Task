using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : InterfaceAI
{
    public List<CubeController> path;

    public EnemyAI() {
        path = new List<CubeController>();
    }

    public virtual List<CubeController> pathFind(int startX, int startY, int endX, int endY) {
        GridController.instance.resetCubes();
        path.Clear();
        GridController.instance.pathFind(GridController.instance.getCube(startX, startY), endX, endY, path);
        return path;
    }
}
