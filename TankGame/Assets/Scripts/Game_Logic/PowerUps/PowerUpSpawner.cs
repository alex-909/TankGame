using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject[] PowerUps;
    void Update()
    {
        
    }
    private int randomIndex() 
    {
        return Random.Range(0, PowerUps.Length);
    }
    private GameObject GetPowerUpByIndex(int index) 
    {
        return PowerUps[index];
    }
    private GameObject GetPowerUpByType(PowerUpType type) 
    {
        foreach (GameObject powerUp in PowerUps) 
        {
            PowerUpInfo info = powerUp.GetComponent<PowerUpInfo>();
            if (info.powerUpType == type) 
            {
                return powerUp;
            }
        }
        Debug.LogError($"powerup type {type} not found!");
        return null;
    }
}
