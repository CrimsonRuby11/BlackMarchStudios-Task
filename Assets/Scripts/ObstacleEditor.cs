using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObstacleEditor : EditorWindow
{
    [MenuItem("Window/Obstacle Editor")]
    public static void ShowWindow() {
        GetWindow<ObstacleEditor>("Obstacle Window");
    }

    private void OnGUI() {
        GUILayout.Label("Obstacles");
        GUILayout.BeginVertical();
        try {
            for(int i = 0; i < 10; i++) {
                try {
                    GUILayout.BeginHorizontal();
                    for(int j = 0; j < 10; j++) {
                        if(GUILayout.Button("" + i + "," + j)) {
                            if(ObstacleManager.instance.data.obstacleBools[10*j + i]) {
                                ObstacleManager.instance.data.obstacleBools[j*10 + i] = false;
                                ObstacleManager.instance.updateObstacles();
                            } else {
                                ObstacleManager.instance.data.obstacleBools[j*10 + i] = true;
                                ObstacleManager.instance.updateObstacles();
                            }
                        }
                    }
                }
                finally {
                    GUILayout.EndHorizontal();
                }
            }
        }
        finally {
            GUILayout.EndVertical();
        }

        GUILayout.Space(20);
        if (GUILayout.Button("Reset Obstacles")) {
            ObstacleManager.instance.resetObstacles();
        }
    }
}
