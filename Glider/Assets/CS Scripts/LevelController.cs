using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Canvas endGameCanvas;
    [SerializeField] private Text levelText;
    [SerializeField] private Text loseText;

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Confined; //re-enable this when exporting
        ResumeGame(); //sets Time.timeScale to 1
        DisableEndGameCanvas(); //disable endGameCanvas
        SetLevelText(); //sets the level number text
        ScoreKeeper.instance.UpdateLevelIndex();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !IsGamePaused())
        {
            SetPauseMenuText();
            PauseMenu();
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && IsGamePaused())
        {
            LeavePauseMenu();
        }
        
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
        Cursor.visible = true;
    }

    public void DisableEndGameCanvas()
    {
        endGameCanvas.enabled = false;
        Cursor.visible = false;
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

    private void PauseMenu()
    {
        PauseGame();
        EnableEndGameCanvas();
    }

    private void LeavePauseMenu()
    {
        ResumeGame();
        DisableEndGameCanvas();
    }

    public void SetPauseMenuText()
    {
        loseText.text = "Paused";
    }

    public void SetLoseText()
    {
        loseText.text = "YOU FAILED!";
    }
}
