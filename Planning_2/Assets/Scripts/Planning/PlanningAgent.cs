using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planning;

public class PlanningAgent : MonoBehaviour
{

	public WorldState goal = new WorldState();

	private Planner planner = new Planner();

	private readonly string PlanningTag = "Planning";

	void Update()
	{
		WorldState current = GetCurrentWorldState();
		IList<Action> possibleActions = GetAllAvaliableActions(current);

		Planning.Planner.Plan plan = planner.MakePlan(current, goal, GetAllAvaliableActions, UpdatePossibleWorldState);

	}

	WorldState GetCurrentWorldState()
	{
		// TODO Fix this - sending messages probably easiest way?

		// Build the current world state
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
	}

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
	}

	IList<Action> GetAllAvaliableActions(WorldState world)
	{
		// TODO Fix this - sending messages probably easiest way?
		// NOTE The planner is probably going
		// Get all actions from the scene
		Object[] sources = new Object[0]; // HACK GameObject.FindObjectsOfType(typeof(IPlanningActionSource));
		IList<Planning.Action> actions = new List<Planning.Action>();
		foreach (IPlanningActionSource source in sources)
		{
			foreach (Action act in source.GetPossibleActions(world))
			{
				actions.Add(act);
			}
		}
		return actions;
	}

}
