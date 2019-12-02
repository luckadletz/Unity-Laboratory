using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planning;
using System;
using UnityEngine.AI;

public class PatrolAction : PlanningAction
{
    [Range(0.0f,1.0f)]
    public float PatrolWeight;

    public Vector3[] PatrolPositions;
    
    private StateList preconditions;

    public override IEnumerable DoAction(StateList world, StateList goal, GameObject actor)
    {
        // Find the nearest patrol point
        int nextPatrolIdx = 0;
        float closestPatrolDistance = Mathf.Infinity;
        for(int i = 0; i < PatrolPositions.Length; ++i)
        {
            float sqDist = Vector3.Distance(actor.transform.position, PatrolPositions[i]);
            if(sqDist < closestPatrolDistance)
            {
                closestPatrolDistance = sqDist;
                nextPatrolIdx = i;
            }
        }

        while(goal.GetState("IsPatrolling") != 0.0f)
        {
            // Walk to the next patrol point in order
            Vector3 position = PatrolPositions[nextPatrolIdx];
            actor.GetComponent<NavMeshAgent>().SetDestination(position);
            yield return new WaitUntil(() =>
                goal.GetState("IsPatrolling") != 0.0f ||
                actor.GetComponent<NavMeshAgent>().remainingDistance < 1.0f);
        }

        yield return null;
    }

    public override float Heuristic(StateList world, StateList goal)
    {
        if (goal.GetState("IsPatrolling") != 0.0f)
            return PatrolWeight;
        else return 0.0f;
    }

    public override StateList Simulate(StateList world, StateList goal)
    {
        StateList simulated = (StateList)world.Clone();
        simulated.SetState("IsPatrolling", 1.0f);
        return simulated;
    }
}
