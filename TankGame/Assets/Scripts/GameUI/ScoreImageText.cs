using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreImageText : MonoBehaviour
{
    [SerializeField] private Image playerImage; 
    [SerializeField] private TextMeshProUGUI scoreText;

    public void SetScoreUI(string playerName, int roundsWon, int kills, int deaths, Color imageColor) 
    {
        playerImage.color = imageColor;
        scoreText.text = $"{playerName} || Wins: {roundsWon} || Kills: {kills}";
    }
}
