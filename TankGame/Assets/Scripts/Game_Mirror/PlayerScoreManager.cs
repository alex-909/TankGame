using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DapperDino.Mirror.Tutorials.Lobby;

public class PlayerScoreManager : NetworkBehaviour
{
    private List<PlayerScore> playerScores = new List<PlayerScore>();

    private NetworkManagerTG room;
    private NetworkManagerTG Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerTG;
        }
    }

    #region Initialize
    public void SendPlayerList(PlayerScript[] players) 
    {
        if (playerScores.Count != 0) { SendScoreUpdate(); return; }

        foreach (PlayerScript player in players)
        {
            Color color = GameAssets.i.GetColor(player.GetComponent<ApplyMaterials>().tankColor);
            PlayerScore playerScore = new PlayerScore()
            {
                owner = player.netIdentity.connectionToClient,
                playerName = player.playerName,
                playerColor = color,
                kills = 0,
                roundsWon = 0,
                deaths = 0,
            };
            playerScores.Add(playerScore);
        }
        SendScoreUpdate();
    }

	#endregion
	public void IncreaseStatInPlayer(PlayerStat stat, NetworkConnection owner)
    {
        PlayerScore playerScore = GetScoreOfPlayer(owner);

        switch (stat)
        {
            case PlayerStat.RoundsWon:
                playerScore.roundsWon++;
                break;
            case PlayerStat.Kills:
                playerScore.kills++;
                break;
            case PlayerStat.Deaths:
                playerScore.deaths++;
                break;
            default:
                break;
        }
        SendScoreUpdate();
    }
    public int GetStatInPlayer(PlayerStat stat, NetworkConnection owner)
    {
        PlayerScore playerScore = GetScoreOfPlayer(owner);
        return stat switch
        {
            PlayerStat.RoundsWon => playerScore.roundsWon,
            PlayerStat.Kills => playerScore.kills,
            PlayerStat.Deaths => playerScore.deaths,
            _ => 0
        };
    }
    public PlayerScore GetScoreOfPlayer(NetworkConnection player) 
    {
        for (int i = 0; i < playerScores.Count; i++) 
        {
            if (playerScores[i].owner.Equals(player)) 
            {
                return playerScores[i];
            }
        }
        Debug.LogError("PlayerScore not found!");
        return null;
    }

    #region ScoreUpdates
    [Server]
    public void SendScoreUpdate()
    {
        ScoreUIManager scoreManager = FindObjectOfType<ScoreUIManager>();
        scoreManager.UpdateScores(playerScores.ToArray());
    }
    #endregion
}
