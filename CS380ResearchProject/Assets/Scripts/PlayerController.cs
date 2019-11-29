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
        Vector3 move = new Vector3();

        move.x += Input.GetAxis("Horizontal") * speed;
        move.z += Input.GetAxis("Vertical") * speed;

        // TODO Undo the camera's rotation

        GetComponent<Rigidbody>().velocity = move;
    }



}