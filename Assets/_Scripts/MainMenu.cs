using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelSelect;

    // all ported into menu buttons
    public void Play()
    {
        // toggles menu layers
        levelSelect.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void LevelOne()
    {
        // load main level
        SceneManager.LoadSceneAsync("LevelOne");
    }

    public void LevelTwo()
    {
        // load debug scene
        SceneManager.LoadSceneAsync("TestScene");
    }

    public void Back()
    {
        // toggles menu layers
        mainMenu.SetActive(true);
        levelSelect.SetActive(false);
    }

    public void Quit()
    {
        // quits to desktop
        Application.Quit();
    }
}