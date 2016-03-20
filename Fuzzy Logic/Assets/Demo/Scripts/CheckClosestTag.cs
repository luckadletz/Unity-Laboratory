using UnityEngine;
using System.Collections;

public class CheckClosestTag : MonoBehaviour {

    public float closest = float.MaxValue;

    public string targetTag = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        // reset closest
        closest = float.MaxValue;

        float closest_squared = float.MaxValue;
        Vector3 position = transform.position;
        GameObject[] all_tags = GameObject.FindGameObjectsWithTag(targetTag);
        foreach(GameObject g in all_tags)
        {
            float sqdist = (position - g.transform.position).sqrMagnitude;
            if(sqdist < closest_squared)
            {
                closest_squared = sqdist;
            }
        }

        closest = Mathf.Sqrt(closest_squared);
	}
}
