using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Canvas endGameCanvas;
    [SerializeField] private Text levelText;

    // Start is called before the first frame update
    void Start()
    {
        ResumeGame(); //sets Time.timeScale to 1
        DisableEndGameCanvas(); //disable endGameCanvas
        SetLevelText(); //sets the level number text
    }

    //Function for Try Again button
    public void TryAgainButton()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    //Function for Quit Button
    public void QuitButton()
    {
        Application.Quit();
    }

    public void EnableEndGameCanvas()
    {
        endGameCanvas.enabled = true;
    }

    public void DisableEndGameCanvas()
    {
        endGameCanvas.enabled = false;
    }

    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    public static void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public static bool IsGamePaused()
    {
        return Time.timeScale == 0;
    }

    public static void GoToNextLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);
    }

    private void SetLevelText()
    {
        int sceneNumber = SceneManager.GetActiveScene().buildIndex;
        sceneNumber++;
        levelText.text = "Level: " + sceneNumber;
    }
}
