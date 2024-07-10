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

    bool canVisit(int x, int y) {
        // if cube exists, and cube is not an obstacle
        return cubes[x, y] && !cubes[x, y].isObstacle;
    }

    public List<CubeController> pathFind(CubeController startingCube, int endX, int endY) {

        // A* PATHFINDING
        // cache endPoint
        CubeController endCube = cubes[endX, endY];

        // Two lists to keep track of visited cubes and unvisited ones
        List<CubeController> openList = new List<CubeController>() {startingCube};
        List<CubeController> closedList = new List<CubeController>();

        // Reset all cubes to not visited
        for(int x = 0; x < 10; x++) {
            for(int y = 0; y < 10; y++) {
                cubes[x, y].gCost = int.MaxValue;
                cubes[x, y].calculateFCost();
                cubes[x, y].previousCube = null;
            }
        }

        // Initialize starting position with gCost = 0, hCost according to endCube
        startingCube.gCost = 0;
        startingCube.hCost = calculateDistanceCost(startingCube, endCube);
        startingCube.calculateFCost();

        // If openList contains nodes
        while(openList.Count > 0) {

            // Get element with lowest fCost and check if its the end point (if reached destination)
            CubeController currentCube = getLowestF(openList);
            if(currentCube == endCube) {
                // If reached, return the final path
                return calculatePath(endCube);
            }

            // Mark current cube as visited, remove from open list and add to closed
            openList.Remove(currentCube);
            closedList.Add(currentCube);

            // For each neighbour of currentCube
            foreach(CubeController c in getNeighbours(currentCube)) {
                // Ignore if neighbour already visited
                if(closedList.Contains(c)) continue;

                // Get gCost according to currentCube's gCost and new distance
                float newG = currentCube.gCost + calculateDistanceCost(currentCube, c);

                // Get the neighbour that we didnt already visit. those whose gCost is more than that of the currentCube
                if(newG < c.gCost) {

                    // Set currentCube as parent to new cube
                    c.previousCube = currentCube;

                    // Update gCost, hCost, and fCost
                    c.gCost = newG;
                    c.hCost = calculateDistanceCost(c, endCube);
                    c.calculateFCost();
                }

                // If neighbour wasn't already searched by other cubes, add to openList
                if(!openList.Contains(c)) {
                    openList.Add(c);
                }
            }
        }

        return null;
    }

    private List<CubeController> getNeighbours(CubeController currentCube) {
        // Initiate list
        List<CubeController> neighbours = new List<CubeController>();

        // Cache position
        int x = currentCube.gridPosition.x;
        int y = currentCube.gridPosition.z;

        // Left
        if(x-1 > -1 && canVisit(x-1, y)) neighbours.Add(cubes[x-1, y]);
        // Right
        if(x+1 < 10 && canVisit(x+1, y)) neighbours.Add(cubes[x+1, y]);
        // Bottom
        if(y-1 > -1 && canVisit(x, y-1)) neighbours.Add(cubes[x, y-1]);
        // Top
        if(y+1 < 10 && canVisit(x, y+1)) neighbours.Add(cubes[x, y+1]);

        return neighbours;
    }

    private List<CubeController> calculatePath(CubeController endCube) {
        // Initiate Path
        List<CubeController> path = new List<CubeController>() {endCube};

        // Start from the endCube
        CubeController currentCube = endCube;

        // If currentCube's parent exists, add to path and set currentCube to parent
        while(currentCube.previousCube != null) {
            path.Add(currentCube.previousCube);
            currentCube = currentCube.previousCube;
        }

        // Reverse the path and return
        path.Reverse();
        // Remove position of player from list
        path.RemoveAt(0);
        return path;
    }

    private CubeController getLowestF(List<CubeController> list) {
        CubeController lowestF = list[0];
        foreach(CubeController c in list) {
            if(c.fCost < lowestF.fCost) {
                lowestF = c;
            }
        }

        return lowestF;
    }

    private int calculateDistanceCost(CubeController a, CubeController b) {
        // calculate linear distances along x and y
        int x = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
        int y = Mathf.Abs(a.gridPosition.z - b.gridPosition.z);

        // Calculate diagonal distances
        int remaining = Mathf.Abs(x - y);

        // Return total distance
        return 14*Mathf.Min(x, y) + 10*remaining;
    }

    public CubeController getCube(int x, int y) {
        // // Move visited = true to a different place as this is illogical.
        // cubes[x, y].visited = true;
        return cubes[x, y];
    }

    public void resetCubes() {
        // Reset every cube to not visited.
        foreach(CubeController c in cubes) {
            c.visited = false;
        }
    }

}
