using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreKeeper : MonoBehaviour
{

    public static ScoreKeeper instance { get; private set; }
    private List<int> coinCountList = new List<int>(); //List for how many coins collected per level
    private List<int> bestTimeList = new List<int>(); //List for fastest time completed per level
    private int currentLevelIndex;


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    //called in the Coin class
    //we had to finesse how the method was called due to the fact that multiple trigger enters were occurring touching a coin
    //so we did an OnDestroy call and that solved the issue HOWEVER
    //anytime the level stopped via the play button, all the other coins would be added to my score because it was called on OnDestroy
    //so we checked with a boolean if they've ever been touched
    //big win for the bois
    public void IncrementCoinCount()
    {
        int value = coinCountList[currentLevelIndex];
        coinCountList[currentLevelIndex] = value + 1;
    }

    //called in timer script
    public void SetBestTimeValue(int timeCountDown)
    {
        int value = bestTimeList[currentLevelIndex];
        bestTimeList[currentLevelIndex] = timeCountDown;
    }

    //Called in the start method of LevelController to update:
    // - level index
    // - list indices
    // we can't call it in this script because the start and awake methods only run once
    public void UpdateLevelIndex()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        coinCountList.Add(0);
        bestTimeList.Add(0);
    }
}
