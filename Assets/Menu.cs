using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Prospector()
    {
        SceneManager.LoadScene(1);
    }
    public void Othergame()
    {
        SceneManager.LoadScene(2);
    }
    public void BackToTitle()
    {
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
