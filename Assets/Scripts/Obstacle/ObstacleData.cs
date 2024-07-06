using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Obstacle Data", menuName = "New ObstacleData")]
public class ObstacleData : ScriptableObject
{
    public List<bool> obstacleBools;
    public Dictionary<(int a, int b), GameObject> obstacles;
}
