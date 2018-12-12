using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PathingCameraController))]
public class PathingCameraTarget : MonoBehaviour
{

    public Transform target;
    private PathingCameraController controller;

    void Awake()
    {
        controller = GetComponent<PathingCameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        controller.lookTarget = target.position;
    }

}
