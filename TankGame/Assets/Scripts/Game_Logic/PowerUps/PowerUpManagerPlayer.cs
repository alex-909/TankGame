using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManagerPlayer : MonoBehaviour
{
    private List<PowerUpInfo> ActivePowerUps;
	private void Update()
	{
        foreach (PowerUpInfo info in ActivePowerUps) 
        {
            info.AddActiveTime(Time.deltaTime);
        } 

		foreach (PowerUpInfo info in ActivePowerUps)
        {
            if (info.TimeOver()) 
            {
                RemovePowerUp(info.powerUpType);
            }
        }
	}
	public void AddPowerUp(PowerUpInfo info) 
    {
        RemovePowerUp(info.powerUpType);
        ActivePowerUps.Add(info);
    }
    public void RemovePowerUp(PowerUpType type) 
    {
        ActivePowerUps.RemoveAll(a => a.powerUpType == type);
    }
    public bool HasPowerUp(PowerUpType type)
    {
        foreach (PowerUpInfo info in ActivePowerUps)
        {
            if (info.powerUpType == type) { return true; }
        }
        return false;
    }
    public void ClearAllPowerUps() 
    {
        ActivePowerUps.Clear();
    }
    public void UpdatePowerUpUI() 
    {

    }
}
