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
                    data.obstacles[(j, i)] = Instantiate(
                        obstaclePf, 
                        GridController.instance.grid.CellToWorld(new Vector3Int(j, 0, i)), 
                        Quaternion.identity, 
                        obstacleParent
                    );
                }

                k++;
            }
        }
    }

    public void updateObstacles() {
        int k = 0;
        for(int i = 0; i < 10; i++) {
            for(int j = 0; j < 10; j++) {
                if(data.obstacleBools[k] && !data.obstacles.ContainsKey((j, i))) {
                    // instantiate obstacle
                    data.obstacles[(j, i)] = Instantiate(
                        obstaclePf, 
                        GridController.instance.grid.CellToWorld(new Vector3Int(j, 0, i)), 
                        Quaternion.identity, 
                        obstacleParent
                    );
                }
                else if(!data.obstacleBools[k] && data.obstacles.ContainsKey((j, i))){
                    Destroy(data.obstacles[(j, i)]);
                    data.obstacles.Remove((j, i));
                }
                k++;
            }
        }
    }

    public void resetObstacles() {
        for(int i = 0; i < 10; i++) {
            for(int j = 0; j < 10; j++) {
                data.obstacleBools[10*j + i] = false;
            }
        }

        updateObstacles();
    }
}
