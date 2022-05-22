using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class ScoreUIManager : NetworkBehaviour
{
    public GameObject ScoreUI;

    public GameObject[] scoreObjects;

    [Command(requiresAuthority = false)]
    public void UpdateScores(PlayerScore[] scoreInfos) 
    {
        UpdateClientScores(scoreInfos);
    }
    [ClientRpc]
    public void UpdateClientScores(PlayerScore[] scoreInfos) 
    {
        //deactivate all texts
        foreach (GameObject scoreObj in scoreObjects)
        {
            scoreObj.SetActive(false);
        }

        List<PlayerScore> infos = scoreInfos.ToList();
        scoreInfos = infos.OrderByDescending(x => x.roundsWon).ToArray();

        for (int i = 0; i < scoreInfos.Length; i++)
        {
            scoreObjects[i].SetActive(true);

            var scoreImageText = scoreObjects[i].GetComponent<ScoreImageText>();
            var s = scoreInfos[i];
            scoreImageText.SetScoreUI(s.playerName, s.roundsWon, s.kills, s.deaths, s.playerColor);
        }
    }
    void FixedUpdate()
    {
        bool pressed = Input.GetKey(KeyCode.Tab);
        ScoreUI.SetActive(pressed);
    }
}
