using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CameraBoundary))]
public class CameraGraph : MonoBehaviour {

    public LayerMask CameraOcclusion;
    
    // The camera space spheres and their underlying graph
    private ArrayList Spheres = new ArrayList();
    private ArrayList Portals = new ArrayList();
    public SimpleGraph SphereGraph
    { get; private set; }
    public SimpleGraph PortalGraph
    { get; private set; }

    
    public float MaxSphereSize = 10f;
    public float MinSphereSize = 5f;
    public float Overlap = 3f;

    public int OperationsPerFrame = 1;
    private int Operations = 0;

    // How many raycasts are done between nodes
    public int VisibilityCastSamples = 100;

    private bool ShouldOperationYield()
    {
        Operations++;
        return (Operations % OperationsPerFrame == 0);
    }

    public void StartPortalGraphGeneration()
    {
        StartCoroutine(GeneratePortalGraph());
    }
    
    public void ClearPortalGraphData()
    {
        foreach(CameraGraphSphereNode Sphere in Spheres)
        {
            DestroyImmediate(Sphere.gameObject);
        }

        Spheres.Clear();

        foreach(CameraGraphPortalNode Portal in Portals)
        {
            DestroyImmediate(Portal.gameObject);
        }
        Portals.Clear();

        SphereGraph = null;
        PortalGraph = null;
    }

	public IEnumerator GeneratePortalGraph()
    {
        Operations = 0;
        
        // Cleanup Here any nodes previously generated

        yield return StartCoroutine(GenerateSpheres());
        
        yield return StartCoroutine(GeneratePortals());
        

        // Connect Spheres
        yield return StartCoroutine(LinkSpheres());    

        // Connect Portals
        yield return StartCoroutine(LinkPortals());

        // Compute Visibility values
        yield return StartCoroutine(ComputeVisibilities());

        Debug.Log("Camera Portal Graph Complete!");
    }

    public CameraGraphPortalNode GetClosestPortal(Vector3 Position)
    {
        float BestSqrDist = Mathf.Infinity;
        CameraGraphPortalNode Best = Portals[0] as CameraGraphPortalNode;

        // This is unoptimized
        foreach(CameraGraphPortalNode Portal in Portals)
        {
            float SqrDist = (Position - Portal.transform.position).sqrMagnitude;

            if(SqrDist < BestSqrDist)
            {
                BestSqrDist = SqrDist;
                Best = Portal;
            }
        }

        return Best;
    }

    public CameraGraphSphereNode GetClosestSphere(Vector3 Position)
    {
        float BestSqrDist = Mathf.Infinity;
        CameraGraphSphereNode Best = Spheres[0] as CameraGraphSphereNode;

        // This is unoptimized
        foreach (CameraGraphSphereNode Sphere in Spheres)
        {
            float SqrDist = (Position - Sphere.transform.position).sqrMagnitude;

            if (SqrDist < BestSqrDist)
            {
                BestSqrDist = SqrDist;
                Best = Sphere;
            }
            // Early out for being inside a sphere
            if (SqrDist < Sphere.Radius * Sphere.Radius)
                return Sphere;
        }

        return Best;
    }

    public float CostEstimate(CameraGraphPortalNode Start, CameraGraphPortalNode End)
    {
        // From http://www.oskam.ch/research/camera_control_2009.pdf Section 4.1
        // H(n) = d(n, e)

        float Dist = (End.transform.position - Start.transform.position).magnitude;

        return Dist;
    }

    
    public float SphereCost(CameraGraphPortalNode Start, CameraGraphPortalNode End, CameraGraphSphereNode Focus)
    {
        // From http://www.oskam.ch/research/camera_control_2009.pdf Section 4.1
        // C(e_ij) = d(i,j) + a*d(i,j)(1-v(e_ij)

        // How much the cost is influenced by visibility.
        const float Alpha = 100f;

        // First, find the sphere shared between the portals
        CameraGraphSphereNode Sphere;

        if(Start.A == End.A)
            Sphere = Start.A;
        else 
            Sphere = Start.B;

        float Dist = (Start.transform.position - End.transform.position).magnitude;

        float Visibility = 0;

        if(!Sphere.VisibilityValues.TryGetValue(Focus, out Visibility))
        {
            Debug.Log("Couldn't get visibility from " + Sphere + " to " + Focus);
            Debug.DrawLine(Sphere.transform.position, Focus.transform.position, Color.magenta, 1.0f);
        }
        return Dist + Alpha * Dist * (1 - Visibility);
        
    }

    IEnumerator GenerateSpheres()
    {
        Debug.Log("Generating Camera Space Spheres ...");

        // Find a good sphere size

        
        // Make sure our bounds have updated positions
        CameraBoundary Bounds = GetComponent<CameraBoundary>();
        Bounds.CalcPositons();

        // Compute how much we move each step between spheres
        float Delta = 2f * (MaxSphereSize - Overlap);

        Debug.Assert(2f * (MinSphereSize - Overlap) > 1f, "Gap between spheres can become very small!");

        // The sum unit offsets between nodes
        Vector3 Offset = new Vector3(Delta, Delta, Delta);
        Debug.Assert(Delta > 0f);

        // Compute our starting and ending sphere positions
        Vector3 StartSpherePos = Bounds.FrontTopLeft;
        Vector3 EndSpherePos = Bounds.BackBottomRight + 0.5f * Offset;
        Debug.Log("Placing Nodes from " + StartSpherePos + " to " + EndSpherePos);

        // Loop through all space in the camera bounds 
        // and plop a sphere node down
        Vector3 CurrentSpherePos = StartSpherePos;

        int id = 0;
        // Loop on X
        for (CurrentSpherePos.x = StartSpherePos.x; // (Redundant)
            CurrentSpherePos.x <= EndSpherePos.x;
            CurrentSpherePos.x += Delta)
        {
            // Loop on Z
            for (CurrentSpherePos.z = StartSpherePos.z;
                CurrentSpherePos.z <= EndSpherePos.z;
                CurrentSpherePos.z += Delta)
            {
                // Loop on Y
                for (CurrentSpherePos.y = StartSpherePos.y;
                    CurrentSpherePos.y >= EndSpherePos.y;
                    CurrentSpherePos.y -= Delta) // Note the subtraction here -- we want our spheres to be connected vertically
                {

                    id++;
                    // Debug.Log("Adding node " + " at " + CurrentSpherePos);

                    float Radius = CheckNodeSize(CurrentSpherePos, MaxSphereSize);

                    if (Radius != 0f)
                    {
                        CameraGraphSphereNode Node = InstantiateNode(CurrentSpherePos, id);
                        Node.Radius = Radius;

                        Spheres.Add(Node);

                        if (ShouldOperationYield()) yield return new WaitForEndOfFrame();

                        // Compute how much we move each step between spheres
                        Delta = 2f * (Radius - Overlap);
                    }

                }
            }
        }

        Debug.Log("Created " + Spheres.Count + " Spheres" );
        yield return null;
    }

    IEnumerator GeneratePortals()
    {
        Debug.Log("Generating Portals");
        // Tell each sphere node to connect to it's neighbors
        // I'm gonna use n^2 because spheres are fast

        for (int i = 0; i < Spheres.Count; ++i )
        {
            CameraGraphSphereNode A = Spheres[i] as CameraGraphSphereNode;
            for(int j = i + 1; j < Spheres.Count; ++j)
            {
                CameraGraphSphereNode B = Spheres[j] as CameraGraphSphereNode;

                // Check if the nodes intersect, and create a portal if they do
                if (A.Intersects(B))
                {
                    CameraGraphPortalNode Node = ConnectPortal(A, B);
                    Portals.Add(Node);

                    // Yield to main thread if we've done enough operations
                    if (ShouldOperationYield()) yield return new WaitForEndOfFrame();

                }


            }
        }

            yield return null;
    }

    private IEnumerator LinkSpheres()
    {
        Debug.Log("Linking Spheres");

        SphereGraph = new SimpleGraph(Spheres.Count, 2 * Portals.Count);
        
        // How much is our offset for the second arc
        int BidirectionOffset = Portals.Count;
        // Every portal is a connection between spheres, so lets do that
        foreach(CameraGraphPortalNode portal in Portals)
        {
            // Indices for the sphere nodes
            int SphereAIndex = Spheres.IndexOf(portal.A);
            int SphereBIndex = Spheres.IndexOf(portal.B);

            // Index for the portal
            int PortalIndex = Portals.IndexOf(portal);
            // Links are all bidirectional
            SphereGraph.SetNodeArc(SphereAIndex, PortalIndex, SphereBIndex);
            SphereGraph.SetNodeArc(SphereBIndex, PortalIndex + BidirectionOffset, SphereAIndex);

            // Gotta show off that cool debugging
            Debug.DrawLine(portal.A.transform.position, portal.B.transform.position, Color.red, 1.0f);

            if (ShouldOperationYield()) yield return new WaitForEndOfFrame();

        }

        Debug.Log("Linked " + SphereGraph.numNodes + " spheres with " + SphereGraph.numArcs + " arcs");
        yield return null;
    }

    private IEnumerator LinkPortals()
    {

        Debug.Log("Linking Portals");

        // Figure out how big our graph is
        int NumNodes = Portals.Count;
        int NumArcs = 0;
        // The number of arcs is equal to the sum of the number of spheres exit arcs - 1
        for(int i = 0; i < SphereGraph.numNodes; ++i)
        {
            NumArcs += SphereGraph.NumArcsForNode(i) - 1;
        }
        // Remember to double your stuff
        int BidirectionOffset = NumArcs;
        NumArcs *= 2;

        PortalGraph = new SimpleGraph(NumNodes, NumArcs);

        // We can just look at all the spheres and connect their portals to make sure the web is complete
        foreach( CameraGraphSphereNode Sphere in Spheres)
        {
            int SphereIndex = Spheres.IndexOf(Sphere);

            for(int i = 0; i < Sphere.IntersectionPortals.Count; ++i)
            {
                CameraGraphPortalNode A = Sphere.IntersectionPortals[i] as CameraGraphPortalNode;
                for(int j = i; j < Sphere.IntersectionPortals.Count; ++j)
                {
                    CameraGraphPortalNode B = Sphere.IntersectionPortals[j] as CameraGraphPortalNode;

                    // Indices for the sphere nodes
                    int PortalAIndex = Portals.IndexOf(A);
                    int PortalBIndex = Portals.IndexOf(B);

                    // Link A and B bidirectionally
                    PortalGraph.SetNodeArc(PortalAIndex, SphereIndex, PortalBIndex);
                    PortalGraph.SetNodeArc(PortalBIndex, SphereIndex + BidirectionOffset, PortalAIndex);

                    // Debug draw
                    Debug.DrawLine(A.transform.position, B.transform.position, Color.magenta, 1.0f);
                    
                    // Yield to draw
                    if (ShouldOperationYield()) yield return new WaitForEndOfFrame();
                }
            }

        }
        Debug.Log("Linked " + PortalGraph.numNodes + " spheres with " + PortalGraph.numArcs + " arcs");

        yield return null;
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        // Convert the object's layer to a bitfield for comparison
        int objLayerMask = (1 << obj.layer);
        if ((layerMask.value & objLayerMask) > 0)  // Extra round brackets required!
            return true;
        else
            return false;
    }

    private IEnumerator ComputeVisibilities()
    {
        Debug.Log("Computing Visibilities...");

        Debug.Assert(!IsInLayerMask(gameObject, CameraOcclusion),
            "You can't have the camera graph block the camera graph!");

        // Gross, n!
        for (int i = 0; i < Spheres.Count; ++i)
        {
            CameraGraphSphereNode A = Spheres[i] as CameraGraphSphereNode;

            for (int j = i; j < Spheres.Count; ++j)
            {
                if (i == j) continue;

                CameraGraphSphereNode B = Spheres[j] as CameraGraphSphereNode;

                A.ComputeVisibility(B);

            }
            // Yield to draw
            if (ShouldOperationYield()) yield return new WaitForEndOfFrame();
        }

    }

    // Checks how big a node at a given position should be to not collide with the level geometry
    // Think of blowing up a baloon from Position to Radius
    private float CheckNodeSize(Vector3 Position, float Radius)
    {
        // How far we reduce a sphere's size per check
        const float SphereDownScale = 0.50f;

        while(Radius >= MinSphereSize)
        {
            // SphereCheck at the location
            if (Physics.CheckSphere(Position, Radius, CameraOcclusion))
            {
                // There's something in the way, reduce size and try again
                Radius *= SphereDownScale;
            }
            else
            {
                // The sphere is OK at this location
                return Radius;
            }

        }

        // Can't put a sphere here
        return 0f;
    }

    // Instantiates a child node at a position, but doesn't set much else
    private CameraGraphSphereNode InstantiateNode(Vector3 Position, int id)
    {
        GameObject Node = new GameObject("Node " + id.ToString());

        Node.transform.SetParent(transform);
        Node.transform.position = Position;

        CameraGraphSphereNode Sphere = Node.AddComponent<CameraGraphSphereNode>();
        Sphere.Graph = this; 

        return Sphere;
    }
    
    private CameraGraphPortalNode ConnectPortal(CameraGraphSphereNode A, CameraGraphSphereNode B)
    {
        // Create the child object
        GameObject PortalObj = new GameObject("Portal");
        PortalObj.transform.SetParent(transform);
        PortalObj.layer = gameObject.layer;

        // Attatch the portal node logic
        CameraGraphPortalNode Portal = PortalObj.AddComponent<CameraGraphPortalNode>();


        Portal.Connect(A, B);
        Portal.Graph = this;

        // Tell the Spheres that there's a portal connecting them
        A.IntersectionPortals.Add(Portal);
        B.IntersectionPortals.Add(Portal);

        return Portal;
    }


}