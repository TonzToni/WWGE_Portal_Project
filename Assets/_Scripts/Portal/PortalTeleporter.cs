using UnityEngine;
using UnityEngine.Animations;

public class PortalTeleporter : MonoBehaviour
{
    public Transform exitPortalCamera;
    
    private Transform player;
    private Transform playerCam;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerCam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Transform>();
    }

    void playerTP()
    {
        // grab and set portal cam rotation since its the rotation player cam needs
        playerCam.eulerAngles = new Vector3(exitPortalCamera.eulerAngles.x, 0, 0);
        player.localEulerAngles = new Vector3(0, exitPortalCamera.eulerAngles.y, 0);

        // toggle character controller while teleporting
        player.GetComponent<CharacterController>().enabled = false;

        // grabbing portal cam position since its in the correct location, requires y offset for some reason
        player.position = new Vector3(exitPortalCamera.position.x, exitPortalCamera.position.y - 0.7f, exitPortalCamera.position.z);

        player.GetComponent<CharacterController>().enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // only allow player to traverse portal
        if (other.tag == "Player")
            playerTP();
    }
}