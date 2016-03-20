using UnityEngine;
using System.Collections;

public class LookAtTarget : MonoBehaviour {

    // The game object being looked at by this camera
    public GameObject Target;

    // How quickly (in rad/s) the camera rotates towards this target
    public float LookSpeed = 0;
    public float MoveSpeed = 10;

    public float DampingTime;

    public Vector3 IdealLookAngle;
    public float IdealDistance;

    public bool RotateWithTarget = true;

    Vector3 CurrentLookVelocity = new Vector3(0,0,0);
    Vector3 CurrentLinearVelocity = new Vector3(0, 0, 0);
    void Awake()
    {
        IdealLookAngle.Normalize();
    }

    public Vector3 GetTargetOffset()
    {
        return (Target.transform.position - transform.position);
    }

	void Update ()
    {
        Vector3 currentLook = transform.forward;
        Vector3 targetLook = GetTargetOffset().normalized;

        Vector3 newLook = Vector3.SmoothDamp(currentLook, targetLook, 
            ref CurrentLookVelocity, DampingTime, LookSpeed);

        transform.forward = newLook;

        Vector3 currentPos = transform.position;
        Vector3 idealPos = Target.transform.position - IdealOffset(IdealDistance);
        transform.position = Vector3.SmoothDamp(currentPos, idealPos, 
            ref CurrentLinearVelocity, DampingTime, MoveSpeed);
	}

    Vector3 IdealOffset(float distance)
    {
        if (RotateWithTarget)
        {
            Quaternion rot = Target.transform.rotation;
            return rot * IdealLookAngle * distance;
        }
        else
            return IdealLookAngle * distance;
    }
}
