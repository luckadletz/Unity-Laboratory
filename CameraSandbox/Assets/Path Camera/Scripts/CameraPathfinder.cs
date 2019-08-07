using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

[System.Serializable]
public class CameraPath
{
    public Queue<Vector3> Points = new Queue<Vector3>();

    public Vector3 Next;

    // Called when a point along the queue has been reached. Won't remove the last point
    public void Step()
    {
        if (Points.Count >= 1)
            Next = Points.Dequeue();
    }
}

[RequireComponent (typeof(Camera))]
public class CameraPathfinder : MonoBehaviour {

    // Where the camera is looking
    public Transform FocusTarget;
    // How smoothly the camera rotates to look at it's target ( Bigger is faster )
    [Range(1f, 0.1f)]
    public float TurnSmoothing = .2f; 

    // Where the camera is moving towards
    public Vector3 Endpoint;

    // How quickly the camera moves along it's path
    public float MoveSpeed;

    public CameraPath Path;

    public CameraGraph Graph;


    private Thread PathComputeThread;


	// Update is called once per frame
	void Update () 
    {
        // Smooth our rotation towards our location
        RotateTowardsTarget();

        // Move along our path
        MoveAlongPath();
	}

    void RotateTowardsTarget()
    {
        Vector3 TargetDir = FocusTarget.position - transform.position;

        Quaternion TargetRotation = Quaternion.LookRotation(TargetDir);

        transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, TurnSmoothing);
    }

    void MoveAlongPath()
    {
        // Get the direction we need to move
        Vector3 MoveDir = Path.Next - transform.position;
        
        // Figure out how far we move (we don't want to move too far)
        float MoveDist = MoveSpeed * Time.deltaTime;
        
        // If we're close enough to the point, move to the next one
        if (MoveDist > MoveDir.magnitude)
        {
            MoveDist = MoveDir.magnitude;
            Path.Step();
        }

        transform.position += MoveDir.normalized * MoveDist;
    }

    public void UpdatePath()
    {
        Path = ComputePath(transform.position, Endpoint, FocusTarget.position);
        Path.Next = Path.Points.Peek();
    }

    CameraPath ComputePath(Vector3 Start, Vector3 End, Vector3 Focus)
    {
        CameraPath Path = new CameraPath();
        Path.Next = Start;

        // Figure out what portals we want
        CameraGraphPortalNode StartNode = Graph.GetClosestPortal(Start);
        CameraGraphPortalNode EndNode = Graph.GetClosestPortal(End);

        CameraGraphSphereNode FocusEdge = Graph.GetClosestSphere(Focus);
        

        // A* that shit!
        List<CameraGraphPortalNode> PortalPath = GetPortalPath(StartNode, EndNode, FocusEdge);
       
        // Get the points as positions
        foreach (CameraGraphPortalNode Portal in PortalPath)
        {
            Path.Points.Enqueue(Portal.transform.position);
        }

        // Finally, add the endpoint to the back of the queue
        Path.Points.Enqueue(End);
        return Path;
    }

    private List<CameraGraphPortalNode> GetPortalPath(CameraGraphPortalNode Start, CameraGraphPortalNode End, CameraGraphSphereNode Target )
    {
        
        // Nodes already evaluated
        HashSet<CameraGraphPortalNode> Closed = new HashSet<CameraGraphPortalNode>();
        // Nodes to be evaluated
        HashSet<CameraGraphPortalNode> Open = new HashSet<CameraGraphPortalNode>();
        // How we got to each node we visit
        Dictionary<CameraGraphPortalNode, CameraGraphPortalNode> CameFrom 
            = new Dictionary<CameraGraphPortalNode,CameraGraphPortalNode>();
        // The value of each of the nodes we've visited
        Dictionary<CameraGraphPortalNode, float> GScore = new Dictionary<CameraGraphPortalNode,float>();
        // The estimated total cost from start to each node
        Dictionary<CameraGraphPortalNode, float> FScore = new Dictionary<CameraGraphPortalNode, float>();

        // We start at the begining
        Open.Add(Start);
        GScore[Start] = 0;
        FScore[Start] = Graph.CostEstimate(Start, End);

        
        // Now the looping
        while(Open.Count != 0)
        {
            // Current portal is the node in open with the lowest F score value
            CameraGraphPortalNode Current = GetLowest(FScore, Open);

            // Are we there?
            if (Current == End)
            {
                return ReconstructPath(CameFrom, Current);
            }
            // Mark the node as visited
            Open.Remove(Current);
            Closed.Add(Current);

            Debug.Log("Visited " + Current);

            // Add the node's neighbors to the open set
            ArrayList Neighbors = new ArrayList();
            Neighbors.AddRange(Current.A.IntersectionPortals);
            Neighbors.AddRange(Current.B.IntersectionPortals);

            foreach (CameraGraphPortalNode Neighbor in Neighbors)
            {
                // Skip nodes we've already visited
                if (Closed.Contains(Neighbor)) continue;

                float TentativeGScore = GScore[Current] + Graph.SphereCost(Current, Neighbor, Target);

                // If the node is good enough, then add it to our path
                if(!Open.Contains(Neighbor) || TentativeGScore < GScore[Neighbor])
                {
                    CameFrom[Neighbor] = Current;
                    GScore[Neighbor] = TentativeGScore;
                    FScore[Neighbor] = TentativeGScore + Graph.CostEstimate(Neighbor, End);
                    if (!Open.Contains(Neighbor))
                        Open.Add(Neighbor);
                }
            }


        }

        // If we can't path, then we give up
        Debug.Log("Camera Pathing failure!");
        return new List<CameraGraphPortalNode>();
    }

    List<CameraGraphPortalNode> ReconstructPath(Dictionary<CameraGraphPortalNode, CameraGraphPortalNode> CameFrom, CameraGraphPortalNode Current)
    {
        List<CameraGraphPortalNode> Total = new List<CameraGraphPortalNode>();
        Total.Add(Current);

        while(CameFrom.ContainsKey(Current))
        {
            Current = CameFrom[Current];
            Total.Insert(0, Current);
        }

        return Total;
    }

    CameraGraphPortalNode GetLowest(Dictionary<CameraGraphPortalNode, float> D, HashSet<CameraGraphPortalNode> Set)
    {
        CameraGraphPortalNode BestPortal = null;
        float BestValue = Mathf.Infinity;

        foreach (CameraGraphPortalNode Portal in Set)
        {
            float PortalValue = Mathf.Infinity;
            D.TryGetValue(Portal, out PortalValue);

            if(PortalValue < BestValue)
            {
                BestValue = PortalValue;
                BestPortal = Portal;
            }
            
        }

        return BestPortal;
    }


    void OnDrawGizmosSelected()
    {
        // Draw the midpoints
        Gizmos.color = Color.blue;

        Vector3 LastPos = Path.Next;
        Debug.DrawLine(transform.position, LastPos, Color.blue);
        
        foreach(Vector3 point in Path.Points)
        {
            Gizmos.DrawSphere(point, 1.0f);
            Gizmos.DrawLine(LastPos, point);
            LastPos = point;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(Path.Next, 2.0f);


    }



}
