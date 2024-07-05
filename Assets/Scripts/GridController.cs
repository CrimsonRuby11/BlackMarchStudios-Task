using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private Vector3 mousePos;
    private Vector3 mousePosOnPlane;
    private Vector3 mousePosGrid;
    private Vector3 lastMousePosGrid;

    void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(this);
        }

        grid = GetComponent<Grid>();
    }

    void Start() {

        initiateCubes();
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

        // debugSphere.position = mousePosGrid;
        
    }

    void initiateCubes() {
        for(int i = 0; i < 10; i++) {
            for(int j = 0; j < 10; j++) {
                GameObject newcube = Instantiate(cubePF, grid.CellToWorld(new Vector3Int(i, 0, j)), Quaternion.identity, cubeParent);
                newcube.GetComponent<CubeController>().setGrid(i, j);
            }
        }
    }
}
