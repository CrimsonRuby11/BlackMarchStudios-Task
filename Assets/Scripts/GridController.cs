using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Unity.VisualScripting;

public class GridController : MonoBehaviour
{
    public static GridController instance;

    public Grid grid {get; private set;}

    [SerializeField]
    private GameObject cubePF;
    [SerializeField]
    private Transform cubeParent;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    [SerializeField]
    private Transform debugSphere;
    [SerializeField]
    private TMP_Text gridPosText;
    [SerializeField]
    public PlayerController playerC;

    private Vector3 mousePos;
    private Vector3 mousePosOnPlane;
    private Vector3 mousePosGrid;
    private Vector3 lastMousePosGrid;

    [SerializeField]
    private CubeController[,] cubes;
    [SerializeField]
    private List<CubeController> path;

    void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(this);
        }

        grid = GetComponent<Grid>();
        cubes = new CubeController[10,10];

        initiateCubes();
    }

    void Start() {

    }

    void Update() {
        mousePos = Input.mousePosition;
        mousePos.z = cam.nearClipPlane;

        Ray r = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(r, out hit, 1000, mask)) {
            mousePosGrid = hit.transform.gameObject.GetComponent<CubeController>().getGrid();
        }

        if(lastMousePosGrid != mousePosGrid) {
            gridPosText.text = mousePosGrid.x + ", " + mousePosGrid.z;
            lastMousePosGrid = mousePosGrid;
        }
    }

    void initiateCubes() {
        for(int i = 0; i < 10; i++) {
            for(int j = 0; j < 10; j++) {
                GameObject newcube = Instantiate(cubePF, grid.CellToWorld(new Vector3Int(i, 0, j)), Quaternion.identity, cubeParent);
                newcube.GetComponent<CubeController>().setGrid(i, j);
                newcube.gameObject.name = "Cube " + i + ", " + j;
                cubes[i, j] = newcube.GetComponent<CubeController>();
            }
        }
    }

    public void setObstacle(int x, int y, bool b) {
        cubes[x, y].isObstacle = b;
    }

    bool canVisit(int x, int y, int step) {
        return cubes[x, y] && !cubes[x, y].isObstacle && cubes[x, y].visited == step;
    }

    public List<CubeController> pathFind(CubeController currentCube, int endX, int endY, List<CubeController> list) {

        if(currentCube.gridPosition.x == endX && currentCube.gridPosition.z == endY) {
            return list;
        }

        // Check all directions of currentCube for next visit
        int t = currentCube.visited;
        int x = currentCube.gridPosition.x;
        int y = currentCube.gridPosition.z;

        List<float> tempList = new List<float>();
        float minDist = 100;
        CubeController tempCube = null;

        // left
        if(x-1 > -1 && canVisit(x-1, y, -1)) {
            if(Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x-1, 0, y)) < minDist) {
                minDist = Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x-1, 0, y));
                tempCube = cubes[x-1, y];
            }
        }
        if(x+1 < 10 && canVisit(x+1, y, -1)) {
            if(Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x+1, 0, y)) < minDist) {
                minDist = Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x+1, 0, y));
                tempCube = cubes[x+1, y];
            }
        }
        if(y-1 > -1 && canVisit(x, y-1, -1)) {
            if(Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x, 0, y-1)) < minDist) {
                minDist = Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x, 0, y-1));
                tempCube = cubes[x, y-1];
            }
        }
        if(y+1 < 10 && canVisit(x, y+1, -1)) {
            if(Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x, 0, y+1)) < minDist) {
                minDist = Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x, 0, y+1));
                tempCube = cubes[x, y+1];
            }
        }
        if(tempCube == null){
            Debug.Log("Could not reach destination.");
            return new List<CubeController>();
        }

        tempCube.visited = t+1;
        list.Add(tempCube);

        return pathFind(tempCube, endX, endY, list);

    }

    public CubeController getPlayerCube() {
        cubes[playerC.gridX, playerC.gridY].visited = 0;
        return cubes[playerC.gridX, playerC.gridY];
    }

    public void resetCubes() {
        foreach(CubeController c in cubes) {
            c.visited = -1;
        }
    }

}
