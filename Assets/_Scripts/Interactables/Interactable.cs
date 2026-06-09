using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // holds name of object
    public virtual string Name { get; }

    // holds string for objects that need to be toggled
    public virtual string UIInfo { get; }

    // used to tell door its attatched to if abject is active
    public virtual bool IsActive { get; }

    // used to differentiate objects that need to be toggled or not
    public virtual bool Toggleable { get; }

    

    // allows objects to have custom logic while still being able to be called from raycast
    public virtual void Interaction(){}
}