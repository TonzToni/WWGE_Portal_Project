using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [Header("Pause")]
    public GameObject pauseMenu;

    [Header("Game Over")]
    public GameObject gameOver;

    [Header("You Win")]
    public GameObject youWin;

    [Header("Options")]
    public GameObject optionsMenu;
    public Slider FOVSlider;
    public TextMeshProUGUI FOVNumber;
    public Slider FPSSlider;
    public TextMeshProUGUI FPSNumber;

    [Header("HUD")]
    public GameObject hud;
    public TextMeshProUGUI hoverText;
    public TextMeshProUGUI collectableText;

    [Header("Health")]
    public Image healthBar;
    public TextMeshProUGUI health;

    [Header("Fall Distance")]
    public TextMeshProUGUI fallDistance;

    private Manager manager;
    private PlayerBehaviour playerBehaviour;
    private InputHandler inputHandler;
    private Camera cam;

    void Start()
    {
        manager = Manager.instance;
        inputHandler = InputHandler.instance;

        playerBehaviour = GetComponent<PlayerBehaviour>();
        cam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
        
        
        hud.SetActive(true);
        cam.fieldOfView = FOVSlider.value;
        Application.targetFrameRate = (int)FPSSlider.value;
    }

    void Update()
    {
        HealthBar();
        FallDistance();
        PauseMenu();
        GameOver();
        Sliders();
        CollectableCounting();
    }

    private void HealthBar()
    {
        // update health bar in UI
        health.text = manager.currentHealth.ToString();
        healthBar.transform.localScale = new Vector3((float)manager.currentHealth / 100f, 1f, 1f);
    }

    private void FallDistance()
    {
        // convert fall damage to string and round to nearest whole number
        fallDistance.text = Mathf.Round(playerBehaviour.GetFallDistance()).ToString();
    }


    private void PauseMenu()
    {
        // stop player changing pause state when in options menu
        if (optionsMenu.activeInHierarchy)
        {
            inputHandler.paused = true;
        }

        // shows or hides the pause menu depending on the pause/dead/gamewon state, ignored if options menu open
        if (manager.paused && !manager.isDead && !manager.isGameWon && !optionsMenu.activeInHierarchy)
        {
            UnPause(pauseMenu, true, 0, CursorLockMode.None);
            return;
        }
        else if (!manager.paused && !manager.isDead && !manager.isGameWon && !optionsMenu.activeInHierarchy)
        {
            UnPause(pauseMenu, false, 1, CursorLockMode.Locked);
        }
    }

    private void GameOver()
    {
        // if health at 0 initiate game over screen, if set back above 0 disable
        if (manager.currentHealth <= 0)
            gameOver.SetActive(true);
        else if (gameOver.activeInHierarchy)
            gameOver.SetActive(false);

        // if game is won show screen and initiate reset
        if (manager.isGameWon)
            youWin.SetActive(true);
        else if (youWin.activeInHierarchy)
            youWin.SetActive(false);
    }

    private void Sliders()
    {
        if (optionsMenu.activeInHierarchy)
        {
            // sets FOV to slider value and changes text in UI
            cam.fieldOfView = FOVSlider.value;
            FOVNumber.text = ((int)cam.fieldOfView).ToString();

            // sets FPS to slider value and changes text in UI
            Application.targetFrameRate = (int)FPSSlider.value;
            FPSNumber.text = Application.targetFrameRate.ToString();
        }
    }

    private void CollectableCounting()
    {
        // combines ints together to create totalCollected out of totalCollectable for UI
        collectableText.text = (manager.totalCollected.ToString() + "/" + manager.collectableTotal.ToString());
    }

    public void ResumeButton()
    {
        // unqause game and switch paused bool
        UnPause(pauseMenu, false, 1, CursorLockMode.Locked);
        inputHandler.TogglePauseState();
    }

    public void OptionsButton()
    {
        // toggles menu layers
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void QuitButton()
    {
        // loads back to  main menu scene
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void BackButton()
    {
        // toggles menu layers
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void RetryButton()
    {
        // unqause game and switch paused bool
        UnPause(gameOver, false, 1, CursorLockMode.Locked);
        inputHandler.TogglePauseState();

        //manager.ResetLevel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UnPause(GameObject menu, bool state, int time, CursorLockMode mode)
    {
        // prepare game for either pausing or unpausing
        Time.timeScale = time;
        menu.SetActive(state);

        Cursor.lockState = mode;
    }
}
