using UnityEngine;

public class Crown : Interactable
{
    [Range(1.0f, 3.0f)] public float bounceSpeed = 2f;
    [Range(1.0f, 200.0f)] public float rotationSpeed = 50f;
    public AnimationCurve positionCurve;
    public AudioClip sound;

    private Vector3 storedPosition;
    private bool interactionState;
    private float totalTime;
    private Manager manager;

    public override string Name 
    {
        get 
        { 
            return "Crown";
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

    // UI text for crown
    public override string UIInfo
    {
        get 
        { 
            return "E To Collect";
        }
    }

    void Start()
    {
        storedPosition = gameObject.transform.position;
        manager = Manager.instance;
    }

    void Update()
    {
        // total round cycle of animation curve equal 2, this will allow for smooth up and down motion
        if (totalTime >= bounceSpeed * 2)
            totalTime = 0;
        else
            totalTime += Time.deltaTime;

        // apply curve to y position
        gameObject.transform.position = new Vector3(storedPosition.x, storedPosition.y + positionCurve.Evaluate(totalTime * bounceSpeed), storedPosition.z);

        // smooth rotation
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y + (rotationSpeed * Time.deltaTime), gameObject.transform.eulerAngles.z);
    }

    public override void Interaction()
    {
        // disable crown and increase total collected by 1
        AudioPlayer.instance.PlaySound(sound, transform);
        interactionState = true;
        gameObject.SetActive(false);
        manager.totalCollected++;
    }
}
