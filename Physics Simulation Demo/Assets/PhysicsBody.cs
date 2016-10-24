using UnityEngine;
using System.Collections;

public class PhysicsBody : MonoBehaviour {


    public float mass;
    public Vector3 acceleration;
    public Vector3 velocity;
    public Vector3 position
    {
        get { return transform.position;    }
        set { transform.position = value;   }
    }

	// Use this for initialization
	void Start () {
        Debug.Assert(GetComponent<Rigidbody>() == null,
            "Rigidbody and PhysicsBody are incompatible!");
	}
	
	// Update is called once per frame
	void Update () {
	    // See PhysicsSolver.cs
	}
}
