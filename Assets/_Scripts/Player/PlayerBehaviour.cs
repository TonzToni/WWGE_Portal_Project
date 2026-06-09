using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float maxFallDistance = 10f;
    public int fallDamageMultiplier = 4;

    private float totalFallDistance;
    private Manager manager;
    private PlayerMovement playerMovement;

    void Start()
    {
        manager = Manager.instance;
        playerMovement = gameObject.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        FallDistance();
        FallDamage();
    }

    void FallDistance()
    {
        // checks if velocity is downward while not grounded to calculate fall distance
        if (gameObject.GetComponent<CharacterController>().velocity.y < 0 && !playerMovement.isGrounded)
        {
            // dividing by 2 for a slower climb
            totalFallDistance -= (gameObject.GetComponent<CharacterController>().velocity.y * Time.deltaTime) / 2;
        }
    }

    void FallDamage()
    {
        // only resets fall distance when player is grounded
        if (playerMovement.isGrounded)
        {
            // checks if totalFallDistance is over threshhold and damages player depending on distance and multiplier
            if (totalFallDistance >= maxFallDistance)
                manager.DoDamage(((int)totalFallDistance + 1) * fallDamageMultiplier);
            
            // resets so it doesnt do multiple ticks of damage
            totalFallDistance = 0;
        }
    }

    // allows UI to get access to distance
    public float GetFallDistance()
    {
        return totalFallDistance;
    }
}
