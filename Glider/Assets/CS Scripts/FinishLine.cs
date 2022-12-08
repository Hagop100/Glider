using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    /// <summary>
    /// The entire reason we need to have access to the timer Gameobject in this script is so we can manage the number of collisions the player has with the finishline
    /// Previously, the character would collide with multiple hurtboxes with the finishline, causing the OnDestroy() of the timer object to run multiple times in weird ways
    /// Our score was being reset to 0 for our best time. 
    /// Now with this didCollide variable, we can manage a single collision and prevent multiple collisions.
    /// </summary>

    private Timer timer;

    private void Awake()
    {
        timer = FindObjectOfType<Timer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        timer.DidCollide(true);
        LevelController.GoToNextLevel();
    }
}
