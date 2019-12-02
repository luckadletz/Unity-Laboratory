using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DoMovement();
    }

    void DoMovement()
    {
        Vector3 dir = new Vector3();
        // Read input direction
        dir.x += Input.GetAxis("Horizontal");
        dir.z += Input.GetAxis("Vertical");
        // Transform input to camera space
        float cameraRotation = Camera.main.transform.rotation.eulerAngles.y;
        dir = Quaternion.AngleAxis(cameraRotation, Vector3.up) * dir;
        // Apply movement as a velocity
        GetComponent<Rigidbody>().velocity = dir * speed;
    }
}
