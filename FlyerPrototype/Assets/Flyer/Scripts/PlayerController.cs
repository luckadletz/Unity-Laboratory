using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    [Header("Speed")]
    public float xSpeed;
    public float ySpeed;
    public float turnFactor;
    public float maxTurn;


    [Space(20.0f)]
    public Controls controls;
    [System.Serializable]
    public struct Controls
    {
        public KeyCode MoveLeft;
        public KeyCode MoveRight;
        public KeyCode MoveUp;
        public KeyCode MoveDown;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Move();
	}

    void Move()
    {
        Rigidbody r = GetComponent<Rigidbody>();

        // I dunno, forces I guess?
        if(Input.GetKey(controls.MoveLeft))
        {
            r.AddForce(Vector3.left * xSpeed, ForceMode.Acceleration);
        }
        else if (Input.GetKey(controls.MoveRight))
        {
            r.AddForce(Vector3.right * xSpeed, ForceMode.Acceleration);
        }
        if (Input.GetKey(controls.MoveUp))
        {
            r.AddForce(Vector3.up * xSpeed, ForceMode.Acceleration);
        }
        if (Input.GetKey(controls.MoveDown))
        {
            r.AddForce(Vector3.down * xSpeed, ForceMode.Acceleration);
        }
    }
}
