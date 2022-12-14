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

    private void Update()
    {
        //Only for debugging!
        if(Input.GetKeyDown(KeyCode.N))
        {
            LevelController.GoToNextLevel();
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
        if(timeCountDown > bestTimeList[currentLevelIndex])
        {
            bestTimeList[currentLevelIndex] = timeCountDown;
        }
    }

    //Called in the start method of LevelController to update:
    // - level index
    // - list indices
    // we can't call it in this script because the start and awake methods only run once
    public void UpdateLevelIndex()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex - 1;
        coinCountList.Add(0);
        bestTimeList.Add(0);
    }

    public void ResetCoinCount()
    {
        coinCountList[currentLevelIndex] = 0;
    }

    public void ResetCoinCount(string levelName)
    {
        if(levelName.Equals("Level 1"))
        {
            coinCountList[0] = 0;
        }
        else if(levelName.Equals("Level 2"))
        {
            coinCountList[1] = 0;
        }
        else if(levelName.Equals("Level 3"))
        {
            coinCountList[2] = 0;
        }
    }

    public List<int> GetCoinCountList()
    {
        return coinCountList;
    }

    public List<int> GetBestTimeList()
    {
        return bestTimeList;
    }
}
