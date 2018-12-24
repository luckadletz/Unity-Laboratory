using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planning;

public class PlanningAgent : MonoBehaviour
{

	public WorldState goal = new WorldState();

	private Planner planner = new Planner();

	private readonly string PlanningTag = "Planning";

	Planner.Plan CurrentPlan = null;

	void Update()
	{

		if(CurrentPlan == null)
		{
			WorldState current = GetCurrentWorldState();
			CurrentPlan = planner.MakePlan(current, goal, GetAllAvaliableActions, UpdatePossibleWorldState);
		}
		else
		{
			bool done = CurrentPlan.Step(gameObject);
			if(done)
			{
				Debug.Log("Done!");
			}
		}
	}

/*
	Get the planning WorldState that matches the world as it is
 */
	WorldState GetCurrentWorldState()
	{
		WorldState current = new WorldState();

		GameObject[] planningObjects = GameObject.FindGameObjectsWithTag(PlanningTag);
		foreach (GameObject source in planningObjects)
		{
			if (source.GetComponent(typeof(IPlanningActionSource)))
			{
				source.SendMessage("ApplyCurrentState", current);
				Debug.Log("Building: " + current);
			}
		}

		return current;
		/* NOTE This is probably a bottlenecking function
			FindGameObjectsWithTag is slow, and SendMessage is slow
			Probably want to cache the list of Planning objects
			and call directly instead of going through messaging
		*/
	}

/*
	Update any secodary effects on the world that may not have been directly caused by the action
 */
	void UpdatePossibleWorldState(WorldState possible)
	{
		GameObject[] planningObjects = GameObject.FindGameObjectsWithTag(PlanningTag);
		foreach (GameObject source in planningObjects)
		{
			if (source.GetComponent(typeof(IPlanningStateSource)))
			{
				source.SendMessage("UpdateState", possible);
				Debug.Log("Updating: " + possible);
			}
		}
		
		/* NOTE This is probably a bottlenecking function
			FindGameObjectsWithTag is slow, and SendMessage is slow
			Probably want to cache the list of Planning objects
			and call directly instead of going through messaging
		*/
	}

/*
	Determine the list of actions this agent can do given a planning worldstate
 */
	IList<Action> GetAllAvaliableActions(WorldState possible)
	{
		IList<Planning.Action> actions = new List<Planning.Action>();

		GameObject[] sources =  GameObject.FindGameObjectsWithTag(PlanningTag);
		foreach (GameObject source in sources)
		{
			var actionSource = source.GetComponent(typeof(IPlanningActionSource)) as IPlanningActionSource;

			if(actionSource == null) continue;
			foreach (Action act in actionSource.GetPossibleActions(possible))
			{
				actions.Add(act);
			}
		}

		return actions;
	}

}
