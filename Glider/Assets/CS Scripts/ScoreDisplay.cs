using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        DisplayScore();
    }

    private void DisplayScore()
    {
        scoreText.text = "";
        List<int> coins = ScoreKeeper.instance.GetCoinCountList();
        List<int> times = ScoreKeeper.instance.GetBestTimeList();

        for(int i = 0; i < coins.Count; i++)
        {
            scoreText.text += "Level " + (i+1) + ": \n";
            scoreText.text += "Coins Collected: " + coins[i] + " | ";
            scoreText.text += "Best Time: " + times[i] + "\n";
            scoreText.text += "\n";
        }
    }
}
