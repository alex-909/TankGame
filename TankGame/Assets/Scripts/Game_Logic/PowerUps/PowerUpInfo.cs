using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType 
{
    BonusHealth,
    BonusAmmo,
    FastReload,
    ExtraSpeed
}
public class PowerUpInfo : MonoBehaviour
{
    public PowerUpType powerUpType;

    [SerializeField] private bool isPermanent;
    [SerializeField] private readonly float duration;
    [SerializeField] private float timeActive = 0;

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
