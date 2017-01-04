using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LineTrail : MonoBehaviour
{
    // How many points we want on this line
    public float maxPoints;
    // Ref to LineRenderer for performance/convenience
    private LineRenderer line;

	void Start ()
    {
        line = GetComponent<LineRenderer>();
        // This effect doesn't make much sense in local space
        line.useWorldSpace = true;
        // Get rid of previous line
        line.numPositions = 0;
	}
	
	void Update ()
    {
        AddPoint(transform.position);
	}

    void AddPoint(Vector3 point)
    {
        if(line.numPositions < maxPoints)
        {
            // If we're not at max points, add one more place
            line.numPositions = line.numPositions + 1;
        }
        // Shift all points down one in the array, dropping the last point
        for(int i = line.numPositions - 1; i > 0; --i)
        {
            Vector3 temp = line.GetPosition(i-1);
            line.SetPosition(i, temp);
        }
        // Set the last point in the line to the new position
        line.SetPosition(0, point);
    }
}
