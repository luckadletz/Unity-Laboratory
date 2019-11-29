using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControlPoint : MonoBehaviour
{

    public float radius;

    bool active;
    public string activeTag;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If active & Mouse down
        if (active && Input.GetMouseButtonDown(0))
        {
            StartCoroutine("GiveCommand");
        }
    }

    IEnumerator GiveCommand()
    {
        Debug.Log("Starting command...");
        Planning.Planner planner = null;
        bool IsClickOnPlanner = Mouseover<Planning.Planner>(out planner);
        if (!IsClickOnPlanner)
        {
            Debug.Log("Not a planner");
            yield return null;
        }
        else
        {
            Debug.Log("Hit" + planner.gameObject.name);
        }
        // Wait until they let go
        AIGoal goal = null;
        yield return new WaitUntil(()
           => Mouseover(out goal));

        Debug.Log("Commanding " + planner.gameObject.name + " to " + goal.gameObject.name);
        // Add the goal's conditions to the planner
        planner.goal = goal.goal.Copy();
        planner.DoPlanning();
        // Wait until it's ready
        yield return new WaitUntil(() => !planner.currentlyPlanning);
        // Start the AI routine to achieve whatever goal you just set;
        planner.ExecutePlan();

        Debug.Log("Ending Command");
        yield return null;
    }


    private bool Mouseover<ComponentType>(out ComponentType c)
    {
        // Raycast from the camera,
        RaycastHit hitInfo = default(RaycastHit);
        bool didHit = Physics.Raycast(
            Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo
        );

        // Quit if we didn't hit anything
        if (!didHit)
        {
            c = default(ComponentType);
            return false;
        }
        // Quit if we didn't hit a ControllableAI
        GameObject hitObject = hitInfo.collider.gameObject;
        c = hitObject.GetComponent<ComponentType>();
        if (c == null)
            return false;
        else
            return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(activeTag))
        {
            active = true;
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(activeTag))
        {
            active = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = active ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
