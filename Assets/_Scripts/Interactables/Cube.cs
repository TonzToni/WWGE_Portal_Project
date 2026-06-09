using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Cube : Interactable
{
    private GameObject pickupPosition;
    private bool interactionState;
    private string uiText = "E To Pickup";
    public override string Name 
    {
        get 
        { 
            return "Cube";
        }
    }

    public override bool IsActive 
    {
        get 
        { 
            return interactionState; 
        }
    }

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

    void Start()
    {
        pickupPosition = GameObject.FindGameObjectWithTag("PickupPosition");
    }

    public override void Interaction()
    {
        if (!interactionState)
        {
            Pickup();
            interactionState = true;
        }
        else
        {
            Drop();
            interactionState = false;
        }
    }

    void Update()
    {
        if (interactionState)
        {
            transform.parent = pickupPosition.transform;
            transform.localPosition = Vector3.zero;
        }
    }

    void Pickup()
    {
        // change object layer to pickup layer

        // set position 0, 0, 0, keep rotation
        // set child to pickup empty object

        gameObject.layer = LayerMask.NameToLayer("Pickup");
        
        uiText = " ";

        gameObject.GetComponent<MeshCollider>().isTrigger = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;

        
    }

    void Drop()
    {
        gameObject.layer = LayerMask.NameToLayer("Ground");

        uiText = "E To Pickup";

        gameObject.GetComponent<MeshCollider>().isTrigger = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;

        transform.parent = null;
    }
}
