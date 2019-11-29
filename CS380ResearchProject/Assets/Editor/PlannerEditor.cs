using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Planning.Planner))]
public class PlannerEditor : Editor {

    public override void OnInspectorGUI()
    {
        
        base.OnInspectorGUI();
        if(GUILayout.Button("\nPlan\n"))
        {
            // Say this three times fast
            Planning.Planner plan = (Planning.Planner)target;
            plan.DoPlanning();
        }
        if (GUILayout.Button("\nExecute\n"))
        {
            // Say this three times fast
            Planning.Planner plan = (Planning.Planner)target;
            plan.ExecutePlan();
        }
    }
}
