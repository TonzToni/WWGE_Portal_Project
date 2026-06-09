using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [Header("Player")]
    public static int health = 100;
    public GameObject playerStart;

    [HideInInspector] public bool isDead;
    [HideInInspector] public int currentHealth;
    [HideInInspector] public bool paused;
    [HideInInspector] public int totalCollected;
    [HideInInspector] public int collectableTotal;
    [HideInInspector] public bool isGameWon;

    // Private
    private InputHandler inputHandler;
    private GameObject player;
    

    // creating an instance of manager
    public static Manager instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        currentHealth = health;
        Time.timeScale = 1;
        inputHandler = InputHandler.instance;

        // count all collectables in scene
        player = GameObject.FindGameObjectWithTag("Player");
        collectableTotal = GameObject.FindGameObjectsWithTag("Collectable").Length;

        // set player position and rotation to start
        player.transform.position = playerStart.transform.position;
        player.transform.rotation = playerStart.transform.rotation;
    }

    void Update()
    {
        HealthCheck();
        WinCheck();
    }

    public void DoDamage(int damage)
    {
        if (!isDead)
        {
            // do damage and a catch to avoid it going negative
            if (damage > currentHealth)
                currentHealth = 0;
            else
                currentHealth -= damage;
        }
    }

    private void HealthCheck()
    {
        // grabs paused state from input handler
        paused = inputHandler.paused;

        // if health 0 or less and not dead/gamewon, kill player and prepare scene for gameover screen
        if (currentHealth <= 0 && !isDead && !isGameWon)
        {
            // "kill" and pause game
            isDead = true;

            // changes pause state if player is "dead" manually for the rest of the game
            inputHandler.TogglePauseState();
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void WinCheck()
    {
        // checks if win requirement has been met and hasnt already been checked
        if (totalCollected >= collectableTotal && !isGameWon)
        {
            // sets gamewon bool to true and prepares game for ui screen
            isGameWon = true;
            inputHandler.TogglePauseState();
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
        }
    }
}