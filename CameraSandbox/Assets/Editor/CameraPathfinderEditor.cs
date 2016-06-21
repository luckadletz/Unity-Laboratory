using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CameraPathfinder))]
public class CameraPathfinderEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraPathfinder script = (CameraPathfinder)target;
        if (GUILayout.Button("\nCompute Path\n"))
        {
            script.UpdatePath();
        }
    }
}