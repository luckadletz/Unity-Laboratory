using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class debugDrawPlanning : MonoBehaviour
{

    public void DrawPlan(Queue<Planning.Action> plan)
    {
        LineRenderer l = GetComponent<LineRenderer>();
        List<Vector3> points = new List<Vector3>();

        points.Add(transform.position);
        foreach(Planning.Action act in plan)
        {
            points.Add(act.gameObject.transform.position + new Vector3(0, 1.0f, 0));
        }

        l.SetVertexCount(points.Count);
        l.SetPositions(points.ToArray());
    }

    public void DrawTree(Planning.Planner.Node n)
    {
        LineRenderer l = GetComponent<LineRenderer>();
        List<Vector3> points = new List<Vector3>();

        while(n != null)
        {
            Vector3 pos = n.action ? 
                n.action.transform.position : transform.position;
            points.Add(pos);
            n = n.parent;
        }

        l.SetVertexCount(points.Count);
        l.SetPositions(points.ToArray());
    }
}
