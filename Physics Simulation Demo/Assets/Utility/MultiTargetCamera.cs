using UnityEngine;
using System.Collections;

public class MultiTargetCamera : MonoBehaviour {

    public float zoom = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Player");

        // Find the center of these points
        Vector3 midpoint = GetMidpoint(obj);
        // Look at this point
        Quaternion Lookat = Quaternion.LookRotation(midpoint - transform.position);
        // transform.rotation = Lookat;
        // Move to be radius away from the point
        float distance = Vector3.Distance(midpoint, transform.position);
        transform.position = midpoint - transform.forward * distance;
	}

    Vector3 GetMidpoint(GameObject[] objects)
    {
        Vector3 mid = new Vector3();
        foreach(GameObject g in objects)
        {
            mid += g.transform.position;
        }
        mid *= 1.0f / objects.Length;
        return mid;
    }

    float GetRadius(GameObject[] objects, Vector3 midpoint)
    {
        float furthest = 0.0f;
        foreach(GameObject g in objects)
        {
            float dist = Vector3.Distance(midpoint, g.transform.position);
            if (dist > furthest)
                furthest = dist;
        }
        return furthest;
    }

}
