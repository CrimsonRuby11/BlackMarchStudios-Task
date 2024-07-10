using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridController : MonoBehaviour
{
    // Singleton
    public static GridController instance;

    // Grid reference
    public Grid grid {get; private set;}

    // Inspector references
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
    [SerializeField]
    private CubeController[,] cubes;
    // [SerializeField]
    // private List<CubeController> path;
    [SerializeField]
    private GameObject planeObject;

    // private variables
    private Vector3 mousePos;
    private Vector3 mousePosOnPlane;
    private Vector3 mousePosGrid;
    private Vector3 lastMousePosGrid;

    void Awake() {
        // Singleton initialization
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(this);
        }

        // Initialize variables
        grid = GetComponent<Grid>();
        cubes = new CubeController[10,10];

        // On awake, initiate the grid array with cubes
        initiateCubes();
    }

    void Start() {
        planeObject.SetActive(false);
    }

    void Update() {
        // UI UPDATE
        // cache mouse position
        mousePos = Input.mousePosition;
        mousePos.z = cam.nearClipPlane;

        // Use raycast to find the cube in mouse position
        Ray r = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if(Physics.Raycast(r, out hit, 1000, mask)) {
            // Update mousePosGrid to the cube's position
            mousePosGrid = hit.transform.gameObject.GetComponent<CubeController>().gridPosition;
        }

        // If mousePos changed, change the UI text
        if(lastMousePosGrid != mousePosGrid) {
            gridPosText.text = mousePosGrid.x + ", " + mousePosGrid.z;
            lastMousePosGrid = mousePosGrid;
        }
    }

    void initiateCubes() {

        // Loop through a 10x10 nested loop and create cubes, add them to the cubes list
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
        // Set the cube's obstacle bool to b
        cubes[x, y].isObstacle = b;
    }

    bool canVisit(int x, int y, int step) {
        // if cube exists, and cube is not an obstacle, and cube's visit value is step.
        return cubes[x, y] && !cubes[x, y].isObstacle && cubes[x, y].visited == step;
    }

    public float calculateCost(CubeController c, CubeController startC, CubeController endC) {
        float fCost, gCost, hCost;
        gCost = Mathf.Abs(c.gridPosition.x - startC.gridPosition.x) + Mathf.Abs(c.gridPosition.z - startC.gridPosition.z);
        hCost = Mathf.Abs(c.gridPosition.x - endC.gridPosition.x) + Mathf.Abs(c.gridPosition.z - endC.gridPosition.z);
        fCost = gCost + hCost;
        c.setCosts(gCost, hCost, fCost);

        return fCost;
    }

    public List<CubeController> pathFind(CubeController startingCube, int endX, int endY) {

        CubeController currentCube = startingCube;
        CubeController endCube = cubes[endX, endY];
        CubeController bestNext;

        List<CubeController> path = new List<CubeController>();

        while(currentCube != cubes[endX, endY]) {

            // Calculate fCost of neighbouring cubes
            int x = currentCube.gridPosition.x;
            int y = currentCube.gridPosition.z;
            float k = 0;

            List<CubeController> neighbours = new List<CubeController>();

            // initialize bestNext to left
            // Left
            if(x-1 > -1 && cubes[x-1, y] && !cubes[x-1,y].isObstacle) {
                neighbours.Add(cubes[x-1, y]);
            }
            // Right
            if(x+1 < 10 && cubes[x+1, y] && !cubes[x+1,y].isObstacle) {
                neighbours.Add(cubes[x+1, y]);
            }
            // Bottom
            if(y-1 > -1 && cubes[x, y-1] && !cubes[x,y-1].isObstacle) {
                neighbours.Add(cubes[x, y-1]);
            }
            // Left
            if(y+1 < 10 && cubes[x, y+1] && !cubes[x,y+1].isObstacle) {
                neighbours.Add(cubes[x, y+1]);
            }

            bestNext = neighbours[0];
            k = neighbours[0].fCost;
            foreach(CubeController c in neighbours) {
                calculateCost(c, startingCube, endCube);
                if(c.fCost < k) {
                    bestNext = c;
                    k = c.fCost;
                }
            }

            path.Add(bestNext);
            currentCube = bestNext;
        }

        return path;

    }

    public CubeController getCube(int x, int y) {
        // resets starting position.
        // Move visited = 0 to a different place as this is illogical.
        cubes[x, y].visited = 0;
        return cubes[x, y];
    }

    public void resetCubes() {
        // Reset every cube to not visited.
        foreach(CubeController c in cubes) {
            c.visited = -1;
        }
    }

}
