using UnityEngine;
using System.Collections;

// Camera must be looking at something specific to perform a dolly zoom
[RequireComponent(typeof(LookAtTarget))] 
[RequireComponent(typeof(Camera))]
public class DollyZoom : MonoBehaviour {

    public enum LockType
    {
        NoLock,     // No automatic zoom
        FovLock,    // Script sets FOV, manually set distance to zoom
        DistLock    // Script sets distance, manually set fov to zoom
    }

    public LockType Lock = LockType.NoLock;


    // Remember last frame's fov and distance so we adjust accordingly
    private float lastDistance;
    private float lastFrustrumHeight;

    void Start()
    {
        RemeberFovAndDist();
    }

    void RemeberFovAndDist()
    {
        lastDistance = GetComponent<LookAtTarget>().GetTargetOffset().magnitude;
        float theta = GetComponent<Camera>().fieldOfView * 0.5f * Mathf.Deg2Rad;
        lastFrustrumHeight = lastDistance * 2.0f * Mathf.Tan(theta);
    }

	// Update is called once per frame
	void Update ()
    {
        switch (Lock)
        {
            case LockType.FovLock:
                float dist = GetComponent<LookAtTarget>().GetTargetOffset().magnitude;
                GetComponent<Camera>().fieldOfView = GetFOVfromDist(dist);
                break;

            case LockType.DistLock:
                // Get our current fov
                float fov = GetComponent<Camera>().fieldOfView;
                // Compute our target distance
                float targetDist = GetDistfromFOV(fov);
                // Adjust our distance along our current offset from the target
                Vector3 moveDir = GetComponent<LookAtTarget>().GetTargetOffset().normalized;
                Vector3 newPos = GetComponent<LookAtTarget>().Target.transform.position 
                    - moveDir * targetDist;
                // TODO -- Check if the camera has a target position script and cooperate with that instead
                transform.position = newPos;
                break;

            default:
            case LockType.NoLock:
                // Do nothing
                break;
        }
        RemeberFovAndDist();
    }

    public float GetFOVfromDist(float currentDist)
    {
        return 2.0f * Mathf.Atan(lastFrustrumHeight * 0.5f / currentDist) * Mathf.Rad2Deg;
    }

    public float GetDistfromFOV(float currentFOV)
    {
        return lastFrustrumHeight / (2.0f * Mathf.Tan(0.5f * currentFOV * Mathf.Deg2Rad));
    }
}
