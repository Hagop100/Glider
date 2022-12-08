using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    [SerializeField] private float timerStart = 0f;
    [SerializeField] private Text timerText;
    [SerializeField] private LevelController levelController;

    private int timeCountDown; //container that is updated every frame for the timer
    private bool didCollideWithFinishLine = false;

    // Update is called once per frame
    void Update()
    {
        TimerTextHandler();
    }

    //Handles the timer text in the UI Canvas
    //Find the time counting down
    //convert to string
    //set the the text in the UI
    private void TimerTextHandler()
    {
        timeCountDown = Mathf.FloorToInt(timerStart - Time.timeSinceLevelLoad);
        string s = string.Format("Timer: {0} s", timeCountDown);
        timerText.text = s;

        //if timer reaches 0 we want to pause the game and show the player the try again and quit buttons on the EndGameCanvas
        if(timeCountDown == 0)
        {
            LevelController.PauseGame();
            levelController.SetLoseText();
            levelController.EnableEndGameCanvas();
        }
    }

    public void DidCollide(bool didCollide)
    {
        didCollideWithFinishLine = didCollide;
    }

    private void OnDestroy()
    {
        if(didCollideWithFinishLine) //for more explanation see FinishLine script!
        {
            ScoreKeeper.instance.SetBestTimeValue(timeCountDown);
            didCollideWithFinishLine = false;
        }
    }
}
