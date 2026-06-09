using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform endPoint;
    public float doorSpeed = 0.1f;
    public float minDoorDistance = 5f;
    public List<Interactable> interactables;


    private GameObject player;
    private Vector3 storedPosition;
    private Vector3 currentEndPoint;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        storedPosition = gameObject.transform.position;
    }

    void Update()
    {
        DetermineState();

        // uses the calculated variables to either move the door or not
        if (Time.timeScale == 1)
            gameObject.transform.position = math.lerp(gameObject.transform.position, currentEndPoint, doorSpeed);
    }

    void DetermineState()
    {
        if (interactables.Count > 0)
        {
            // if there are interactables tied to door, only open door when all are active
            for (int i = 0; i < interactables.Count; i++)
            {
                // changes endpoint used to endpoint position chosen if interactable is active, if not revert to starting position
                if (interactables[i].IsActive)
                {
                    currentEndPoint = endPoint.position;
                }
                else
                {
                    currentEndPoint = storedPosition;
                    break;
                }
            }
        }
        else
        {
            // if no interactables attatched, open door if player is within chosen distance
            if (Vector3.Distance(storedPosition, player.transform.position) < minDoorDistance)
                currentEndPoint = endPoint.position;
            else
                currentEndPoint = storedPosition;
        }
    }
}
