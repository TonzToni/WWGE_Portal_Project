using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float sensitivity = 0.5f;
    public Transform body;
    public Camera pickupCam;

    private Vector2 lookRotation;
    private float xRotation = 0f;
    private InputHandler inputHandler;
    

    
    public void Start()
    {
        inputHandler = InputHandler.instance;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraRotate();
    }

    void CameraRotate()
    {
        // update pickup cams FOV;
        pickupCam.fieldOfView = GetComponent<Camera>().fieldOfView;

        // grab mouse movement from input handler
        lookRotation = inputHandler._cameraLook * sensitivity;

        if (Time.timeScale == 1) // only allows movement if game is not paused
        {
            // clamps x rotation to min / max of -90/90
            xRotation -= lookRotation.y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // sets rotation of camera with x rotation, and player with y rotation
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            body.Rotate(Vector3.up * lookRotation.x); // y rotation on camera so it doesnt rotate along z
        }
    }
}
