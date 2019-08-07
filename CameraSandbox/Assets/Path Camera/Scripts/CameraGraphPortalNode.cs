using UnityEngine;
using System.Collections;
using UnityEditor;
public class CameraGraphPortalNode : MonoBehaviour {

    public float Radius;
    public Vector3 Normal;
    public CameraGraph Graph;
    public CameraGraphSphereNode A, B;



    public void Connect(CameraGraphSphereNode NodeA, CameraGraphSphereNode NodeB)
    {
        A = NodeA;
        B = NodeB;

        Vector3 dir = B.transform.position - A.transform.position;
        float dist = dir.magnitude;

        // Compute the intersection midpoint
        float Penetration = Mathf.Abs(dist - (A.Radius + B.Radius));
        float IntersectionDist = A.Radius - Penetration * 0.5f;
        transform.position = A.transform.position + dir.normalized * IntersectionDist;

        // Use pythagorean theorem to get our radius
        Radius = Mathf.Sqrt(A.Radius * A.Radius - IntersectionDist * IntersectionDist);

        // And our normal is just along the direction
        Normal = dir.normalized;
    }

    void OnDrawGizmosSelected()
    {
        Handles.color = Color.yellow;

        Handles.DrawWireDisc(transform.position, Normal, Radius);
    }
}
