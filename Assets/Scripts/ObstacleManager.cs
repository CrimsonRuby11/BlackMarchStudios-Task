using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager instance;

    public ObstacleData data;

    public GameObject obstaclePf;
    public Transform obstacleParent;

    void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(instance);
        }
    }

    void Start() {
        data.obstacles = new Dictionary<(int a, int b), GameObject>();

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
                    // instantiate obstacle
                    data.obstacles[(i, j)] = Instantiate(
                        obstaclePf, 
                        GridController.instance.grid.CellToWorld(new Vector3Int(i, 0, j)), 
                        Quaternion.identity, 
                        obstacleParent
                    );

                    GridController.instance.setObstacle(i, j, true);
                }
                else if(!data.obstacleBools[k] && data.obstacles.ContainsKey((i, j))){
                    Destroy(data.obstacles[(i, j)]);
                    data.obstacles.Remove((i, j));
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
