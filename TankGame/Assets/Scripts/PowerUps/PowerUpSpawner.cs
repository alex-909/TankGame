using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PowerUpSpawner : NetworkBehaviour
{
    public GameObject[] PowerUps;

    #region testing

    private float fTimer = 0;
    float cooldown = 3;

    float firstTimer = 0;
    readonly float firstSpawn = 15;

    #endregion

    private PU_SpawnPoints _pU_SpawnPoints;
    private PU_SpawnPoints pU_SpawnPoints 
    {
        get 
        {
            if (_pU_SpawnPoints != null) { return _pU_SpawnPoints; }
            return _pU_SpawnPoints = GameObject.FindObjectOfType<PU_SpawnPoints>();
        }
    }

	void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.L)) 
        {
            SpawnPowerUp();
        }
        */
        if (firstTimer < firstSpawn) { firstTimer += Time.deltaTime; return; }


        bool powerUpExists = FindObjectOfType<PowerUpScript>() != null;
        if (powerUpExists) { fTimer = 0; return; }

        if (fTimer < cooldown)
        {
            fTimer += Time.deltaTime;
        }
        else 
        {
            fTimer = 0;
            cooldown = Random.Range(5f,15f);
            SpawnPowerUp();
        }
    }
    private void SpawnPowerUp() 
    {
        Debug.Log("spawning powerup!");
        var powerUp = GetPowerUpByIndex(randomIndex());
        CmdSpawnPowerUp(powerUp);
    }
    //no need for command attribute here, as this object only exists on the server
    private void CmdSpawnPowerUp(GameObject powerUpGameObject) 
    {
        var pos = GetPowerUpPosition();
        var obj = Instantiate(powerUpGameObject, pos, Quaternion.identity);
        NetworkServer.Spawn(obj);
    }
    private Vector3 GetPowerUpPosition()
    {
        return pU_SpawnPoints.GetRandomPoint().position;
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
