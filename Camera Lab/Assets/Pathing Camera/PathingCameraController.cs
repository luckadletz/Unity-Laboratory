//https://sourcemaking.com/design_patterns/strategy

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This is the base strategy 
public class PathingCameraController : MonoBehaviour
{
    /// <summary>
    /// The focus of the camera, usually the player.
    /// </summary>
    public Vector3 lookTarget;

    /// <summary>
    /// How far away we want the camera to be from the player
    /// </summary>
    public float distance;

    /// <summary>
    /// Approximately the time it will take to reach the target.
    /// A smaller value will reach the target faster.
    /// </summary>
    public float smoothTime;

    public float maxSpeed;
    public float turnSpeed;

    private Vector3 currentVelocity = new Vector3();

    private void MoveTowardsTarget()
    { 
        Vector3 position = transform.position;
        // We want to be between 0 and distance away from our target
        Vector3 targetPosition = Vector3.MoveTowards(lookTarget, position, distance);
        // Smoothdamp to that position
        transform.position = Vector3.SmoothDamp(position, targetPosition,
            ref currentVelocity, smoothTime, maxSpeed, Time.deltaTime);
    }

    private void LookAtTarget()
    {
        Vector3 currentDir = transform.forward;
        Vector3 targetDir = (lookTarget - transform.position).normalized;
        transform.forward = Vector3.Slerp(currentDir, targetDir, turnSpeed * Time.deltaTime);
    }

	// Update is called once per frame
	void Update ()
    {
        MoveTowardsTarget();
        LookAtTarget();
	}


}
