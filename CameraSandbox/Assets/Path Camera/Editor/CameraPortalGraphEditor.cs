using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(CameraGraph))]
public class CameraPortalGraphEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraGraph script = (CameraGraph)target;
        if (GUILayout.Button("\nBuild Camera Graph\n"))
        {
            script.StartPortalGraphGeneration();
        }

        if(GUILayout.Button("Clear Camera Graph Data"))
        {
            script.ClearPortalGraphData();
        }
    }
}
