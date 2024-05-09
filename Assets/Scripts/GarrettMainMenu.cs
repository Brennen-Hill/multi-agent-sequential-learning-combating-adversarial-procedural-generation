using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuscript : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("TakaScene");

    }
    public void QuitGame()
    {

        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Garrett Start");
    }
}