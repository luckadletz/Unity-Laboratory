using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planning;

public class PlanningAgent : MonoBehaviour 
{

	public WorldState goal;

	private Planner planner;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		WorldState current = GetCurrentWorldState();
		IList<Action> possibleActions = GetAllAvaliableActions(current);

		Planning.Planner.Plan plan = planner.MakePlan(current, goal, GetAllAvaliableActions);

	}

	WorldState GetCurrentWorldState()
	{
		// TODO Fix this - sending messages probably easiest way?
		// Build the current world state
		Object[] stateSources = GameObject.FindObjectsOfType(typeof(IPlanningStateSource));
		WorldState current = new WorldState();
		foreach(IPlanningStateSource source in stateSources)
		{
			if(source != null)
			{
				source.ApplyCurrentState(current);
			}
		}

		return current;
	}

	IList<Action> GetAllAvaliableActions(WorldState world)
	{
		// TODO Fix this - sending messages probably easiest way?
		// NOTE The planner is probably going
		// Get all actions from the scene
		Object[] sources = GameObject.FindObjectsOfType(typeof(IPlanningActionSource));
		IList<Planning.Action> actions = new List<Planning.Action>();
		foreach(IPlanningActionSource source in sources)
		{
			foreach(Action act in source.GetPossibleActions(current))
			{
				actions.Add(act);
			}
		}
		return actions;
	}

}
