using System;
using UnityEngine;

public enum PowerUpType 
{
    BonusHealth, // done
    ExtraBullet, // 
    FastReload,  // done
    ExtraSpeed   // done
}
public class PowerUpEventArgs : EventArgs
{
    public PowerUpEventArgs(PowerUpType powerUpType) 
    {
        PowerUpType = powerUpType;
    }
    public PowerUpType PowerUpType { get; set; }
}

[System.Serializable]
public class PowerUpInfo
{
    public PowerUpType powerUpType;
    public Sprite powerUpImage;

    private readonly bool isPermanent;
    private readonly float duration;
    private float timeActive = 0;

    public PowerUpInfo(PowerUpType powerUpType, bool isPermament, float duration,  float timeActive, Sprite image) 
    {
        this.powerUpType = powerUpType;
        this.isPermanent = isPermament;
        this.duration = duration;
        this.timeActive = timeActive;
        this.powerUpImage = image;
    }

    public bool GetPermanent() 
    {
        return isPermanent;
    }
    public void AddActiveTime(float amount) 
    {
        timeActive += amount;
    }
    public bool TimeOver() 
    {
        if (isPermanent) { return false; }

        return timeActive > duration;
    }
}
