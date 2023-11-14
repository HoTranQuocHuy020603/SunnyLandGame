using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Lv1");
    }

    public void StoryMenu()
    {
        SceneManager.LoadScene("Story");
    }

    public void OptionMenu()
    {
        SceneManager.LoadScene("OptionMenu");
    }
}
