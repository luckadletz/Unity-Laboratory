using UnityEngine;
using System.Collections;

public class DoorLogic : MonoBehaviour
{

    [Tooltip("How the door moves to when it is opened")]
    public Vector3 openOffset;
    private Vector3 originalPos;

    bool open = false;

    public bool IsOpen()
    {
        return open;
    }

    void Start()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        if (open)
        {
            Vector3 targetPos = originalPos + openOffset;
            Vector3 newPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
            transform.position = newPos;
        }
    }

    public void Open()
    {
        open = true;
    }


}


