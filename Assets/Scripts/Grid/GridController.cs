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
    [SerializeField]
    private List<CubeController> path;
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

    public List<CubeController> pathFind(CubeController currentCube, int endX, int endY, List<CubeController> list) {

        // RECURSIVE FUNCTION
        // If end position is reached, return list
        if(currentCube.gridPosition.x == endX && currentCube.gridPosition.z == endY) {
            return list;
        }

        // Else, 
        // Check all directions of currentCube for next visit
        int t = currentCube.visited;
        int x = currentCube.gridPosition.x;
        int y = currentCube.gridPosition.z;

        List<float> tempList = new List<float>();
        float minDist = 100;
        CubeController tempCube = null;

        // Left
        if(x-1 > -1 && canVisit(x-1, y, -1)) {
            if(Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x-1, 0, y)) < minDist) {
                minDist = Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x-1, 0, y));
                tempCube = cubes[x-1, y];
            }
        }
        // Right
        if(x+1 < 10 && canVisit(x+1, y, -1)) {
            if(Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x+1, 0, y)) < minDist) {
                minDist = Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x+1, 0, y));
                tempCube = cubes[x+1, y];
            }
        }
        // Bottom
        if(y-1 > -1 && canVisit(x, y-1, -1)) {
            if(Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x, 0, y-1)) < minDist) {
                minDist = Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x, 0, y-1));
                tempCube = cubes[x, y-1];
            }
        }
        // Top
        if(y+1 < 10 && canVisit(x, y+1, -1)) {
            if(Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x, 0, y+1)) < minDist) {
                minDist = Vector3.Distance(new Vector3(endX, 0, endY), new Vector3(x, 0, y+1));
                tempCube = cubes[x, y+1];
            }
        }
        // tempCube consists of the cube that is closest to the final destination.
        // In case destination is not reachable
        if(tempCube == null){
            Debug.Log("Could not reach destination.");
            return new List<CubeController>();
        }

        // set the next cube's visited to a higher value than current one, and add to the list
        tempCube.visited = t+1;
        list.Add(tempCube);

        // Using recursive function logic, pass the tempCube as the starting position for next loop.
        return pathFind(tempCube, endX, endY, list);

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
