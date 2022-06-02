using UnityEngine;
using Mirror;
using DapperDino.Tutorials.Lobby;

public enum PlayerStat
{
    RoundsWon,
    Kills,
    Deaths
}
public class PlayerScore
{
    public NetworkIdentity owner;

    public string playerName;
    public Color playerColor;

    public int roundsWon = 0;
    public int kills = 0;
    public int deaths = 0;
}


