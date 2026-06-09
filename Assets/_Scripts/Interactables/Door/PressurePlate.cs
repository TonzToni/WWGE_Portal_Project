using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Interactable
{
    [Range(0.001f, 30.0f)] public float cooldown;
    public Transform plate;

    private Vector3 originalPlatePosition;
    private Vector3 pressedPlatePosition;
    private bool interactionState;
    private bool timerCooldown;
    private float totalTime;
    private List<GameObject> objects = new List<GameObject>();

    public override string Name 
    {
        get 
        { 
            return "PressurePlate";
        }
    }

    // tells door its attatched to its state
    public override bool IsActive 
    {
        get 
        { 
            return interactionState; 
        }
    }

    // blocks interaction from occurring
    public override bool Toggleable
    {
        get 
        { 
            return false; 
        }
    }

    void Start()
    {
        // storing plate local position and altered locations for when its stood on
        originalPlatePosition = plate.localPosition;
        pressedPlatePosition = new Vector3(plate.localPosition.x, plate.localPosition.y / 2, plate.localPosition.z);
    }

    void Update()
    {
        Timer();
    }

    void Timer()
    {
        // basic timer, instantly sets state to true, and wont turn it back to false until totalTime has reached cooldown
        if (timerCooldown)
            interactionState = true;
        else
            totalTime += Time.deltaTime;
        
        if (totalTime >= cooldown)
        {
            interactionState = false;
            totalTime = 0;
        }
    }

    // toggle timerCooldown bool and change position of plate depending on it its being triggered or not
    void OnTriggerEnter(Collider other)
    {
        // track all objects in trigger
        objects.Add(other.gameObject);

        // start plate
        timerCooldown = true;
        plate.localPosition = pressedPlatePosition;
    }
    void OnTriggerExit(Collider other)
    {
        // remove object that left and check if there are any remaining, if so dont stop plate until all gone
        objects.Remove(other.gameObject);

        if (objects.Count == 0)
        {
            // prepare for plate to stop
            timerCooldown = false;
            plate.localPosition = originalPlatePosition;
        }
    }
}
