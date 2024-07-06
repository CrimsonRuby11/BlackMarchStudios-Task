using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    // Singleton
    public static ObstacleManager instance;

    // Reference to data
    public ObstacleData data;

    public GameObject obstaclePf;
    public Transform obstacleParent;

    void Awake() {
        // Singleton initialization
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(instance);
        }
    }

    void Start() {
        // Initialize variables
        data.obstacles = new Dictionary<(int a, int b), GameObject>();

        // On game start, create obstacles according to list
        initiateObstacles();
    }

    public void initiateObstacles() {
        int k = 0;
        for(int i = 0; i < 10; i++) {
            for(int j = 0; j < 10; j++) {
                if(data.obstacleBools[k]) {
                    // instantiate obstacle
                    data.obstacles[(i, j)] = Instantiate(
                        obstaclePf, 
                        GridController.instance.grid.CellToWorld(new Vector3Int(i, 0, j)), 
                        Quaternion.identity, 
                        obstacleParent
                    );

                    // Update obstacleBools
                    GridController.instance.setObstacle(i, j, true);
                }
                
                k++;
            }
        }
    }

    public void updateObstacles() {
        int k = 0;
        for(int i = 0; i < 10; i++) {
            for(int j = 0; j < 10; j++) {
                if(data.obstacleBools[k] && !data.obstacles.ContainsKey((i, j))) {
                    // if obstacleBool is true but obstacle not present at (i, j) create an obstacle in that position
                    data.obstacles[(i, j)] = Instantiate(
                        obstaclePf, 
                        GridController.instance.grid.CellToWorld(new Vector3Int(i, 0, j)), 
                        Quaternion.identity, 
                        obstacleParent
                    );

                    // Update obstacle bools
                    GridController.instance.setObstacle(i, j, true);
                }
                else if(!data.obstacleBools[k] && data.obstacles.ContainsKey((i, j))){
                    // If obstacleBool is false but obstacle is present at (i, j) delete that obstacle
                    Destroy(data.obstacles[(i, j)]);
                    // remove from list
                    data.obstacles.Remove((i, j));

                    // update obstacle bools
                    GridController.instance.setObstacle(i, j, false);
                }
                k++;
            }
        }
    }

    public void resetObstacles() {
        for(int i = 0; i < 10; i++) {
            for(int j = 0; j < 10; j++) {
                data.obstacleBools[10*i + j] = false;
            }
        }

        updateObstacles();
    }
}
