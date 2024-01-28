using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static void ToMain()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public static void ToTitle()
    {
        SceneManager.LoadScene("Title", LoadSceneMode.Single);
    }

    public static void ToCredits()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Credits", LoadSceneMode.Single);
    }

    /*
     * Quits the game
     */
    public static void QuitGame()
    {
        Application.Quit();
    }
}
