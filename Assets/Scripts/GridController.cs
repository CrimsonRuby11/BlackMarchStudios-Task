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
                cubes[i, j] = newcube.GetComponent<CubeController>();
            }
        }
    }

    public void setObstacle(int x, int y, bool b) {
        cubes[x, y].isObstacle = b;
    }

    // Pathfinding

    public enum Dir {
        top,
        right,
        bottom,
        left,
    }

    int startX;
    int startY;

    void setup() {
        startX = playerC.gridX;
        startY = playerC.gridY;

        Debug.Log("Start Pathfinding");

        cubes[startX, startY].visited = 0;
    }

    bool testDir(int x, int y, int step, Dir dir) {
        switch(dir) {
            case Dir.left:
                return x-1 > -1 && canVisit(x-1, y, step);
            case Dir.right:
                return x+1 < 10 && canVisit(x+1, y, step);
            case Dir.top:
                return y-1 > -1 && canVisit(x, y-1, step);
            case Dir.bottom:
                return y+1 < 10 && canVisit(x, y+1, step);
        }

        return false;
    }

    public bool canVisit(int x, int y, int step) {
        return cubes[x, y] && cubes[x, y].visited == step;
    }

    void setVisit(int x, int y, int step) {
        if(cubes[x, y]) {
            cubes[x, y].visited = step;
        }
    }

    public void setDistance() {
        setup();

        for(int step = 1; step < 100; step++) {
            foreach(CubeController c in cubes) {
                if(c && c.visited == step-1) {
                    TestAllDir(c.gridPosition.x, c.gridPosition.z, step);
                }
            }
        }
    }

    void TestAllDir(int x,int y, int step) {
        if(testDir(x, y, -1, Dir.top)) {
            setVisit(x, y+1, step);
        }
        if(testDir(x, y, -1, Dir.right)) {
            setVisit(x+1, y, step);
        }
        if(testDir(x, y, -1, Dir.bottom)) {
            setVisit(x, y-1, step);
        }
        if(testDir(x, y, -1, Dir.left)) {
            setVisit(x-1, y, step);
        }
    }

    public void setPath(int endX, int endY) {
        int step;

        int x = endX;
        int y = endY;

        List<CubeController> tempList = new List<CubeController>();
        path.Clear();

        if(cubes[endX, endY] && cubes[endX, endY].visited > 0) {
            path.Add(cubes[x, y]);
            step = cubes[x, y].visited - 1;
        } else {
            return;
        }

        for(int i = step; i > -1; i--) {
            if(testDir(x, y, step, Dir.top)) {
                tempList.Add(cubes[x, y+1]);
            }
            if(testDir(x, y, step, Dir.bottom)) {
                tempList.Add(cubes[x, y-1]);
            }
            if(testDir(x, y, step, Dir.left)) {
                tempList.Add(cubes[x-1, y]);
            }
            if(testDir(x, y, step, Dir.right)) {
                tempList.Add(cubes[x+1, y]);
            }
        }

        CubeController tempObj = getClosest(cubes[endX, endY].transform.position, tempList);
        path.Add(tempObj);
        x = tempObj.gridPosition.x;
        y = tempObj.gridPosition.z;
        tempList.Clear();

    }

    CubeController getClosest(Vector3 targetLocation, List<CubeController> list) {
        float minDist = 500;
        int index = 0;
        for(int i = 0; i < list.Count; i++) {
            if(Vector3.Distance(targetLocation, list[i].transform.position) < minDist) {
                minDist = Vector3.Distance(targetLocation, list[i].transform.position);
                index = i;
            }
        }

        return list[index];
    }

}
