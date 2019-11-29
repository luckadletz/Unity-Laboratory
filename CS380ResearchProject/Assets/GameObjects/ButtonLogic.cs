using UnityEngine;
using System.Collections;

public class ButtonLogic : MonoBehaviour
{
    [Tooltip("How the button moves to when it is pressed")]
    public Vector3 pressedOffset;
    private Vector3 originalPos;

    [Tooltip("The door to open when we're pressed")]
    public DoorLogic door;

    public bool isPressed = false;

    void Start()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        if (isPressed)
        {
            Vector3 targetPos = originalPos + pressedOffset;
            Vector3 newPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
            transform.position = newPos;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.tag == "Player")
        {
            isPressed = true;
            if(door) door.Open();
        }
    }

    public void OnDrawGizmos()
    {
        if(door)
            // Draw a line to the door we open
            Gizmos.DrawRay(transform.position, door.transform.position - transform.position);
    }
}
