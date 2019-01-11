using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planning;
using System;
using UnityEngine.AI;

namespace Planning
{
	
	[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
	public class Agent : MonoBehaviour, IStateSource, IActionSource
	{

		public ExpectedWorldGoal goal = new ExpectedWorldGoal();

		private PlanFinder planner = new PlanFinder();

		private readonly string PlanningTag = "Planning";

		Plan CurrentPlan = null;

		void Start()
		{
			// Hardcode goal
			goal.Expected.SetState(new ButtonLogic.ButtonPlanningState("Goal", true));
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
				bool done = CurrentPlan.Step();
				if (done)
				{
					Debug.Log("Done!");
					this.enabled = false;
				}
			}
		}

		public bool CanPath(World world, Vector3 target)
		{
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
				foreach (Step act in actionSource.GetPossibleActions(possible, this))
				{
					actions.Add(act);
				}
			}

			return actions;
		}

		public IList<Step> GetPossibleActions(World state, Agent agent)
		{

			throw new NotImplementedException();
		}

		public void ApplyCurrentState(World world)
		{
			// TODO set position
		}

		class PositionState : Planning.State
		{
			public PositionState()
			{

			}

			public override object Clone()
			{
				throw new NotImplementedException();
			}
		};

		public void UpdateState(World possible)
		{
		}
	}
}
