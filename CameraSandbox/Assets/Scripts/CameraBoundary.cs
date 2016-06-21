using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]

public class CameraBoundary : MonoBehaviour {

    public Vector3 Center;
    public Vector3 Dimensions;
    private Vector3 Extents;

    public Vector3 FrontTopLeft
    { private set; get; }
    public Vector3 FrontTopRight
    { private set; get; }

    public Vector3 FrontBottomLeft
    { private set; get; }

    public Vector3 FrontBottomRight
    { private set; get; }

    public Vector3 BackTopLeft
    { private set; get; }

    public Vector3 BackTopRight
    { private set; get; }

    public Vector3 BackBottomLeft
    { private set; get; }

    public Vector3 BackBottomRight
    { private set; get; }


    private Color color = Color.green;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        CalcPositons();
        DrawBox();
    }
    public void CalcPositons()
    {
        //Bounds bounds;
        //BoxCollider bc = GetComponent<BoxCollider>();
        //if (bc != null)
        //    bounds = bc.bounds;
        //else
        //return;
        Extents = Dimensions * 0.5f;

        // No rotation for you..
        transform.rotation = Quaternion.identity;

        FrontTopLeft = new Vector3(Center.x - Extents.x, Center.y + Extents.y, Center.z - Extents.z);  // Front top left corner
        FrontTopRight = new Vector3(Center.x + Extents.x, Center.y + Extents.y, Center.z - Extents.z);  // Front top right corner
        FrontBottomLeft = new Vector3(Center.x - Extents.x, Center.y - Extents.y, Center.z - Extents.z);  // Front bottom left corner
        FrontBottomRight = new Vector3(Center.x + Extents.x, Center.y - Extents.y, Center.z - Extents.z);  // Front bottom right corner
        BackTopLeft = new Vector3(Center.x - Extents.x, Center.y + Extents.y, Center.z + Extents.z);  // Back top left corner
        BackTopRight = new Vector3(Center.x + Extents.x, Center.y + Extents.y, Center.z + Extents.z);  // Back top right corner
        BackBottomLeft = new Vector3(Center.x - Extents.x, Center.y - Extents.y, Center.z + Extents.z);  // Back bottom left corner
        BackBottomRight = new Vector3(Center.x + Extents.x, Center.y - Extents.y, Center.z + Extents.z);  // Back bottom right corner

        FrontTopLeft = transform.TransformPoint(FrontTopLeft);
        FrontTopRight = transform.TransformPoint(FrontTopRight);
        FrontBottomLeft = transform.TransformPoint(FrontBottomLeft);
        FrontBottomRight = transform.TransformPoint(FrontBottomRight);
        BackTopLeft = transform.TransformPoint(BackTopLeft);
        BackTopRight = transform.TransformPoint(BackTopRight);
        BackBottomLeft = transform.TransformPoint(BackBottomLeft);
        BackBottomRight = transform.TransformPoint(BackBottomRight);
    }

    void DrawBox()
    {

        //if (Input.GetKey (KeyCode.S)) {
        Debug.DrawLine(FrontTopLeft, FrontTopRight, color);
        Debug.DrawLine(FrontTopRight, FrontBottomRight, color);
        Debug.DrawLine(FrontBottomRight, FrontBottomLeft, color);
        Debug.DrawLine(FrontBottomLeft, FrontTopLeft, color);

        Debug.DrawLine(BackTopLeft, BackTopRight, color);
        Debug.DrawLine(BackTopRight, BackBottomRight, color);
        Debug.DrawLine(BackBottomRight, BackBottomLeft, color);
        Debug.DrawLine(BackBottomLeft, BackTopLeft, color);

        Debug.DrawLine(FrontTopLeft, BackTopLeft, color);
        Debug.DrawLine(FrontTopRight, BackTopRight, color);
        Debug.DrawLine(FrontBottomRight, BackBottomRight, color);
        Debug.DrawLine(FrontBottomLeft, BackBottomLeft, color);
        //}
    }


}
