using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // holds interaction with player input, referenced in scripts that require user input

    public Vector2 _cameraLook;
    public Vector2 _playerMove;
    public bool _playerSprint;
    public bool _playerJump;
    public bool paused;
    public bool interacted;

    // creating an instance of input handler
    public static InputHandler instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // all plugged into player input on inspector to get user input
    public void CameraLook(InputAction.CallbackContext context)
    {
        // pointer delta from mouse
        _cameraLook = context.ReadValue<Vector2>();
    }

    public void PlayerMove(InputAction.CallbackContext context)
    {
        // WASD from keyboard
        _playerMove = context.ReadValue<Vector2>();
    }

    public void PlayerSprint(InputAction.CallbackContext context)
    {
        // Shift from keyboard
        _playerSprint = context.performed;
    }

    public void PlayerJump(InputAction.CallbackContext context)
    {
        // Spacebar from keyboard
        _playerJump = context.performed;
    }

    public void Pause(InputAction.CallbackContext context)
    {
        // inverts paused bool when activated with Escape
        if (context.performed)
        {
            paused = !paused;
        }
    }
    // used to toggle pause state outside of input handler
    public void TogglePauseState(){ this.paused = !this.paused; }

    public void Interaction(InputAction.CallbackContext context)
    {
        // E from keyboard
        if (context.started)
        {
            interacted = true;
        }
    }
    // used to toggle interaction state outside of input handler
    public void ToggleInteractionState(){ this.interacted = !this.interacted; }
}