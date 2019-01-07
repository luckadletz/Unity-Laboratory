﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planning;
using System;

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class PlanningAgent : MonoBehaviour
{

	public ExpectedWorldGoal goal = new ExpectedWorldGoal();

	private Agent planner = new Agent();

	private readonly string PlanningTag = "Planning";

	Plan CurrentPlan = null;

	void Start()
	{
		
	}

	void Update()
	{

		if (CurrentPlan == null)
		{
			World current = GetCurrentWorldState();
			CurrentPlan = planner.MakePlan(current, goal, GetAllAvaliableActions, UpdatePossibleWorldState);
			Debug.Log(CurrentPlan.Count);
		}
		else
		{
			bool done = CurrentPlan.Step(planner);
			if (done)
			{
				Debug.Log("Done!");
			}
		}
	}

	public bool CanPath(World world)
	{
		// TODO store our position first, so we can get it here
		// var currentState = world.GetState(gameobject.name) as PlaningData<PlanningAgent>
		// Vector3 pos = currentState.position;
		// get all objects in the world to update their position (I don't like this bit...)
		// try and pathfind from wherever
		return true;
	}

	/*
		Get the planning WorldState that matches the world as it is
	 */
	World GetCurrentWorldState()
	{
		World current = new World();

		GameObject[] planningObjects = GameObject.FindGameObjectsWithTag(PlanningTag);
		foreach (GameObject source in planningObjects)
		{
			if (source.GetComponent(typeof(IActionSource)))
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
	void UpdatePossibleWorldState(World possible)
	{
		GameObject[] planningObjects = GameObject.FindGameObjectsWithTag(PlanningTag);
		foreach (GameObject source in planningObjects)
		{
			if (source.GetComponent(typeof(IStateSource)))
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
	IList<Step> GetAllAvaliableActions(World possible)
	{
		IList<Step> actions = new List<Step>();

		GameObject[] sources = GameObject.FindGameObjectsWithTag(PlanningTag);
		foreach (GameObject source in sources)
		{
			var actionSource = source.GetComponent(typeof(IActionSource)) as IActionSource;

			if (actionSource == null) continue;
			foreach (Step act in actionSource.GetPossibleActions(possible, planner))
			{
				actions.Add(act);
			}
		}

		return actions;
	}

}
