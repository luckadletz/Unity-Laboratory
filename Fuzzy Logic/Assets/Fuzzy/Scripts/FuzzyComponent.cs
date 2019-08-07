/*
 * Author: Luc Kadletz
 * 3/11/2016
 * 
 */

using UnityEngine;
using System.Collections;

public class FuzzyComponent : MonoBehaviour {

    public TriangleFuzzyNumber triangle;

    private Vector3 originalScale;


    public float input
    {
        get; set;
    }

    public IFuzzy number
    {
        get
        { return triangle; }
    }


	// Use this for initialization
	void Start () {
        originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(
            originalScale.x,
            originalScale.y * triangle.Membership(input),
            originalScale.z
            );
	}
}
