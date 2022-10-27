using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    [SerializeField] float timerStart = 0f;
    [SerializeField] Text timerText;

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
        int timeCountDown = Mathf.FloorToInt(timerStart - Time.time);
        string s = string.Format("Timer: {0} s", timeCountDown);
        timerText.text = s;
    }
}
