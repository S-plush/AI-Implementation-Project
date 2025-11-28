using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuStuff : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Level");
    }

    public void ExitLevel()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Title Screen");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
