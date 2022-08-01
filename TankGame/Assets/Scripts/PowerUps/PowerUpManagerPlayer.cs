using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManagerPlayer : MonoBehaviour
{
    [SerializeField] private List<PowerUpInfo> ActivePowerUps = new List<PowerUpInfo>();

    public event EventHandler<PowerUpEventArgs> GotPowerUp;

    private PowerUp_UI powerUp_UI;
    private PowerUp_UI PowerUp_UI 
    {
        get 
        {
            if (powerUp_UI != null) { return powerUp_UI; }
            powerUp_UI = FindObjectOfType<PowerUp_UI>();
            return powerUp_UI;
        }
    }


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
                break;
            }
        }
	}
	public void AddPowerUp(PowerUpInfo info) 
    {
        RemovePowerUp(info.powerUpType);
        ActivePowerUps.Add(info);
        UpdatePowerUpUI();
		GotPowerUp?.Invoke(this, new PowerUpEventArgs(info.powerUpType));
	}
    public void RemovePowerUp(PowerUpType type)
    {
        ActivePowerUps.RemoveAll(a => a.powerUpType == type);
        UpdatePowerUpUI();
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
        PowerUp_UI.UpdateUI(ActivePowerUps.ToArray());
    }
}
