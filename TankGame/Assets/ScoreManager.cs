using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public class ScoreInfo
    {
        public string name;
        public int roundsWon;
        public int kills;
        public int deaths;
        public Color color;
    }

    public GameObject ScoreUI;

    public GameObject[] scoreObjects;

    public void UpdateScores(ScoreInfo[] scoreInfos) 
    {
        //deactivate all texts
        foreach (GameObject scoreObj in scoreObjects) 
        {
            scoreObj.SetActive(false);
        }

        List<ScoreInfo> infos = scoreInfos.ToList<ScoreInfo>();
        scoreInfos = infos.OrderByDescending(x => x.roundsWon).ToArray();

        for (int i = 0; i < scoreInfos.Length; i++) 
        {
            scoreObjects[i].SetActive(true);

            var scoreImageText = scoreObjects[i].GetComponent<ScoreImageText>();
            var s = scoreInfos[i];
            scoreImageText.SetScoreUI(s.name, s.roundsWon, s.kills, s.deaths, s.color);
        }

    }

    void FixedUpdate()
    {
        bool pressed = Input.GetKey(KeyCode.Tab);
        ScoreUI.SetActive(pressed);
    }
}
