using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerScore : NetworkBehaviour
{
    public enum Stat 
    {
        RoundsWon,
        Kills,
        Deaths
    }

    [SyncVar] [ShowInInspector] private int roundsWon = 0;
    [SyncVar] [ShowInInspector] private int kills = 0;
    [SyncVar] [ShowInInspector] private int deaths = 0;
    public void IncreaseStat(Stat stat)
    {
		switch (stat)
		{
			case Stat.RoundsWon:
				roundsWon++;
				break;
			case Stat.Kills:
                kills++;
				break;
			case Stat.Deaths:
                deaths++;
				break;
			default:
				break;
		}
	}
    public int GetStat(Stat stat) 
    {
        return stat switch
        {
            Stat.RoundsWon => roundsWon,
            Stat.Kills => kills,
            Stat.Deaths => deaths,
            _ => 0
        };
    }
}
