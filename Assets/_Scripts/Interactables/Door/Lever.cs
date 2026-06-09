using UnityEngine;

public class Lever : Interactable
{
    public GameObject leverHandle;
    public float maxLeverRotation;
    public AudioClip[] leverSounds;

    private bool interactionState;
    private int leverRotDirection = -1;
    private string uiText = "E To Activate";

    public override string Name 
    {
        get 
        { 
            return "Lever";
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

    // allows interaction to occur
    public override bool Toggleable
    {
        get 
        { 
            return true; 
        }
    }

    // UI text for Lever
    public override string UIInfo
    {
        get 
        { 
            return uiText; 
        }
    }

    public override void Interaction()
    {
        // rotates lever in local angles to the amount set in inspector, then flips the direction it needs to travel next
        leverHandle.transform.localEulerAngles = new Vector3(0, 0, leverHandle.transform.localEulerAngles.z - maxLeverRotation * leverRotDirection);
        leverRotDirection *= -1;

        // plays lever sound and changes ui text depending on direction of lever
        if (interactionState)
        {
            AudioPlayer.instance.PlaySound(leverSounds[0], transform);
            uiText = "E To Activate";
        }
        else
        {
            AudioPlayer.instance.PlaySound(leverSounds[1], transform);
            uiText = "E To Deactivate";
        }

        // flip interaction state each handle rotation
        interactionState = !interactionState;
    }
}
