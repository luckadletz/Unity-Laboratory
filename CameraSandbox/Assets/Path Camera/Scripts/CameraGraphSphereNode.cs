using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode()]
public class CameraGraphSphereNode : MonoBehaviour {

    public CameraGraph Graph;


    public bool DebugDrawVisibilities;

    public float Radius;
    
    // Unsorted collection of IntersectionPortals this sphere is a part of
    public ArrayList IntersectionPortals = new ArrayList();

    // Each other node's visibility values
        // e.g. I can see node x for value VisibityValues[x]

    public Dictionary<CameraGraphSphereNode, float> VisibilityValues = new Dictionary<CameraGraphSphereNode, float>();

    void OnDrawGizmosSelected( )
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, Radius);

        if(DebugDrawVisibilities)
        {
            foreach(CameraGraphSphereNode Target in VisibilityValues.Keys)
            {
                float Visibility = VisibilityValues[Target];
                if (Visibility >= 1.0f)
                    Gizmos.color = Color.green;
                else if (Visibility >= 0.5f)
                    Gizmos.color = Color.yellow;
                else if (Visibility >= 0.0f)
                    Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(Target.transform.position, Target.Radius *  Visibility *  Visibility);
            }
        }

    }

    public bool Intersects(CameraGraphSphereNode other)
    {
        // Get the distance
        Vector3 dist = transform.position - other.transform.position;
        
        float CombRad = Radius + other.Radius;

        return dist.sqrMagnitude < CombRad * CombRad;
    }

    public void ComputeVisibility(CameraGraphSphereNode Target)
    {
        float VisibleHits = 0;
        int NumSamples = Graph.VisibilityCastSamples;

        // Use MonteCarlo Sampling by raycasting a bunch randomly to see how many can see the target
        Vector3 Direction = (Target.transform.position - transform.position);
        float MaxCastDist = Direction.magnitude;

        Direction.Normalize();

        for(int CastNum = 0; CastNum < NumSamples; ++CastNum)
        {
            // Compute the random offset
            Vector3 RandomOffset = Random.insideUnitSphere * Radius;

            // Cast a ray towards the target
            Ray R = new Ray(transform.position + RandomOffset, Direction);
            RaycastHit Info;
            if(Physics.Raycast(R, out Info, MaxCastDist, Graph.CameraOcclusion) == true)
            {
                // The ray is blocked

                // Snazzy DebugDraw
                Debug.DrawLine(transform.position + RandomOffset, 
                    transform.position + RandomOffset + Direction * Info.distance, Color.red * 0.5f, 1.0f);
            }
            else
            {
                // The ray is not blocked
                VisibleHits++;

                // Snazzy DebugDraw
                //Debug.DrawLine(transform.position + RandomOffset,
                //    transform.position + RandomOffset + Direction * MaxCastDist, Color.green * 0.1f, 0.25f, true);
            }
        }

        // Our visibility value is the % of samples that hit
        VisibilityValues.Add(Target, VisibleHits / NumSamples);

        // Visibility is two way
        Target.VisibilityValues.Add(this, VisibleHits / NumSamples);
    }

}
