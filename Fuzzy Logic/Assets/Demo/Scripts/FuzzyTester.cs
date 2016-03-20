using UnityEngine;
using System.Collections;

public class FuzzyTester : MonoBehaviour {

    public FuzzyComponent NumberA;
    public FuzzyComponent NumberB;

    public float input
    {
        get; set;
    }

    public enum Operation
    {
        Union,
        Intersection,
        Equivalence,
        Implication
    }

    public Operation operation;

    private IFuzzy system;
    private Vector3 originalScale;

    // Use this for initialization
    void Start()
    {
        originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float output;
        switch(operation)
        {
            case Operation.Union:
                output = Fuzzy.Union(NumberA.number, NumberB.number).Membership(input);
                break;

            case Operation.Intersection:
                output = Fuzzy.Intersection(NumberA.number, NumberB.number).Membership(input);
                break;

            case Operation.Equivalence:
                output = Fuzzy.Equivalence(NumberA.number, NumberB.number).Membership(input);
                break;

            case Operation.Implication:
                output = Fuzzy.Implication(NumberA.number, NumberB.number).Membership(input);
                break;

            default:
                output = -1.0f;
                Debug.LogWarning("Operation not implemented!");
                break;
        }
        transform.localScale = new Vector3(
            originalScale.x,
            output * originalScale.y,
            originalScale.z
            );
	}

}
