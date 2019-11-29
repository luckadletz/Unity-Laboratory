using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAction : MonoBehaviour
{
    public GameObject[] PatrolPoints;

    private SimpleAction patrolAction;
    private SimpleAction stopPatrolAction;

    public float patrolTime;

	// Use this for initialization
	void Start ()
    {
        patrolAction = gameObject.AddComponent<SimpleAction>();
        patrolAction.postconditions.SetState("Patrol", 1.0f);
        stopPatrolAction = gameObject.AddComponent<SimpleAction>();
        stopPatrolAction.postconditions.SetState("Patrol", 0.0f);

        // == PRECONDITIONS ARE NOT MODIFIED == //




    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    // Check if a given gameObject should stop it's patrol
    public void ShouldStopPatrolling(GameObject patrolee)
    {

    }

    // DO Action starts a corotine that changes the player's world back after a set amount of tme
}
