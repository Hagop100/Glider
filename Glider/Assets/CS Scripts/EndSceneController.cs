using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void LoadLevelButton(string levelName)
    {
        ScoreKeeper.instance.ResetCoinCount(levelName);
        SceneManager.LoadScene(levelName);
    }
}
