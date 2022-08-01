using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PU_SpawnPoints : MonoBehaviour
{
    public Transform[] PU_spawnPoints;
    public Transform[] GetAllPoints() 
    {
        return PU_spawnPoints;
    }
    public Transform GetRandomPoint() 
    {
        int index = Random.Range(0, PU_spawnPoints.Length);
        return PU_spawnPoints[index];
    }
}
