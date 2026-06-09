using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 2f;
    public GameObject pickupPoint;

    private Camera cam;
    private InputHandler inputHandler;
    private UserInterface userInterface;
    private bool curHolding;
    private int cubeID;

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
        inputHandler = InputHandler.instance;
        userInterface = GameObject.FindGameObjectWithTag("Player").GetComponent<UserInterface>();
    }

    void Update()
    {
        HitDetector();
        PickupPosition();
    }

    void HitDetector()
    {
        // make cone around pickup point that detects walls to avoid cube being dropped when inside wall

        // creating ray and hit for interaction detection
        RaycastHit hit;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        // determines if the object hit is toggleable before attempting interaction logic to ignore things that dont need it
        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.gameObject.GetComponent<Interactable>())
            {
                // lock out logic is cube being held
                if (curHolding && hit.collider.gameObject.GetInstanceID() != cubeID) 
                {
                    // avoids storing interaction state when holding a cube and trying to pick another up
                    // repeat code kinda annoying, probably better way to do it
                    if (inputHandler.interacted)
                        inputHandler.ToggleInteractionState();
                    return;
                }

                // update UI hover text
                userInterface.hoverText.text = hit.collider.gameObject.GetComponent<Interactable>().UIInfo;

                // if key has not been used dont raycast
                if (!inputHandler.interacted) return;
                cubeID = 0;

                // if all parameters pass activate interaction function on object
                hit.collider.gameObject.GetComponent<Interactable>().Interaction();

                // stores object id of cube if one is being held and sets holding state to true
                if (hit.collider.gameObject.GetComponent<Interactable>().Name == "Cube")
                {
                    cubeID = hit.collider.gameObject.GetInstanceID();
                    curHolding = !curHolding;
                }
            }
        }
        else
            // reset UI text to blank if nothing being hovered
            userInterface.hoverText.text = " ";

        // toggle interaction state to avoid holding interaction until one is at raycast
        if (inputHandler.interacted)
            inputHandler.ToggleInteractionState();
    }

    void PickupPosition()
    {
        // grabs and converts cam x angle a range that i can easily gatekeep
        float angle = cam.transform.eulerAngles.x;
        if (angle > 180f)
            angle -= 360f;
        
        // angle checked if above or below 40 degrees, if either dont update position
        if (Math.Abs(angle) > 40) return;

        // update position of pickup point
        pickupPoint.transform.position = cam.transform.position + (cam.transform.forward * playerReach);
    }
}
