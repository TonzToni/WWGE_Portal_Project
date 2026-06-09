using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float groundSmoothingSpeed = 10f;
    public float airSmoothingSpeed = 1f;
    public float walkSpeed = 7.0f;
    public float sprintModifier = 1.5f;

    [Header("Ground")]
    public float gravity = -20.0f;
    public float groundDistance = 0.5f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public bool isGrounded;

    [Header("Jumping")]
    public float jumpForce = 7.0f;
    public float jumpCooldown = 0.05f;

    [Header("Audio")]
    public AudioClip[] footSteps;
    public AudioClip jumping;
    public AudioClip landing;
    public float stepInterval = 0.35f;

    // Private
    private CharacterController characterController;
    private Vector3 velocity;
    private float curPlayerSpeed;
    private bool isSprinting;
    private Vector2 playerMovement;
    private InputHandler inputHandler;
    private UserInterface userInterface;
    private Camera cam;
    private float stepCounter;
    private bool midJump;
    private Vector2 smoothedMovement;
    private float curSmoothingSpeed;
    private bool canJump;
    private float jumpTimer;

    bool wasGroundedPrevFrame;

    private readonly Vector3[] rayPosOffset = new Vector3[]
    {
        new Vector3( 0.4f, 0f,  0.3f),
        new Vector3( 0.4f, 0f, -0.3f),
        new Vector3(-0.4f, 0f, -0.3f),
        new Vector3(-0.4f, 0f,  0.3f)
    };

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        inputHandler = InputHandler.instance;
        userInterface = GameObject.FindGameObjectWithTag("Player").GetComponent<UserInterface>();
        cam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();

        isGrounded = IsGrounded();
    }

    void Update()
    {
        Gravity();
        Move();
        Jump();
        Walking();
    }

    private bool IsGrounded()
    {
        // creates sphere around the feet of player to detect when on ground
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void Gravity()
    {
        wasGroundedPrevFrame = isGrounded;
        isGrounded = IsGrounded();

        // pulls player down to ground constantly
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = gravity;
        }
        // lowers -y velocity if about to start falling
        else if (!isGrounded && !midJump)
        {
            midJump = true;
            velocity.y = -2;
        }

        // applies gravity
        velocity.y += gravity * Time.deltaTime;

        // clamping velocity to a maximum of jumpforce + gravity stops higher than intended vertical momentum
        // without this player can gain large velocity by jumping on slope
        velocity.y = Mathf.Clamp(velocity.y, -50, jumpForce + gravity);

        // keeps velocity for longer if in air
        if (!isGrounded) curSmoothingSpeed = airSmoothingSpeed;
    }

    void Move()
    {
        // grab movement inputs from input handler
        isSprinting = inputHandler._playerSprint;
        playerMovement = inputHandler._playerMove;

        // smooth movement, smoothing amount changes depending on smoothingSpeed, which changes depending isGrounded state
        smoothedMovement = Vector2.Lerp(smoothedMovement, playerMovement, Time.deltaTime * curSmoothingSpeed);
        
        // multiply movement with correct directions reletive to player and move controller
        Vector3 move = transform.right * smoothedMovement.x + transform.forward * smoothedMovement.y;
        characterController.Move(move * curPlayerSpeed * Time.deltaTime);

        // alters player speed and FOV if sprint key is down and facing forward
        if (isSprinting && playerMovement.y > 0)
        {
            curPlayerSpeed = walkSpeed * sprintModifier;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, userInterface.FOVSlider.value + 10f, 20f * Time.deltaTime);
        }
        else
        {
            curPlayerSpeed = walkSpeed;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, userInterface.FOVSlider.value, 20f * Time.deltaTime);
        }
    }

    void Jump()
    {
        // checks if player just lander on ground
        if (!wasGroundedPrevFrame && isGrounded)
        {
            // plays landing sound
            AudioPlayer.instance.PlaySound(landing, transform, 0.5f);

            // set movement smoothing to ground smoothing
            curSmoothingSpeed = groundSmoothingSpeed;

            // allows jumping if landed
            midJump = false;
        }

        // gives player positive y velicity if key was pressed and conditions met
        if (wasGroundedPrevFrame && isGrounded && inputHandler._playerJump && !midJump)
        {
            // play jumping sound
            AudioPlayer.instance.PlaySound(jumping, transform, 0.5f);

            // lockout to avoid repeated hits
            midJump = true;

            // add velocity
            velocity.y += jumpForce;
        }

        // give player calculated velocity
        characterController.Move(velocity * Time.deltaTime);
    }

    void Walking()
    {
        // chosing random audio clip outside of timer so i can log it to destroy if needed
        AudioClip clip = footSteps[UnityEngine.Random.Range(0, footSteps.Length)];

        // when player is not on ground destroy any footstep clips
        if (!isGrounded)
            AudioPlayer.instance.DestroyAudioSource(clip);

        // used to lock out sound efects until interval reached
        if (playerMovement.magnitude > 0 && stepCounter <= stepInterval && isGrounded)
            stepCounter += curPlayerSpeed * Time.deltaTime;
        else
            // sets starting value to half of interval so initial step is quicker
            stepCounter = stepInterval / 2f;

        if (stepCounter >= stepInterval)
        {
            // play random footstep and reset counter
            AudioPlayer.instance.PlaySound(footSteps[UnityEngine.Random.Range(0, footSteps.Length)], transform, 0.75f);
            stepCounter = 0;
        }
    }
}
