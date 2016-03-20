using UnityEngine;
using System.Collections;

public class ReflectionTester : MonoBehaviour {

    [Range(0,10)]
    public float width;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 scale = transform.localScale;
        scale.z = width;
        transform.localScale = scale;
	}
}
