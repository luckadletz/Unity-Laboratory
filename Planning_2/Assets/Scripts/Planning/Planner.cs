/* Luc Kadletz - 12/12/2018 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{
	public class Planner
	{
		public delegate IList<Action> RefreshActionsCallback(WorldState possible);
		public delegate void RefreshWorldCallback(WorldState possible);

		public class Plan
		{
			private IList<Action> Actions;

			public Plan(IList<Action> actions)
			{
				Actions = actions;
			}

			public bool Step(GameObject actor)
			{
				if(Actions.Count == 0) return true;

				bool done = Actions[0].Execute(actor);
				if (done)
				{
					Actions.RemoveAt(0);
				}

				return Actions.Count == 0;
			}

		}

		public Plan MakePlan(
			WorldState current,
			WorldState goal,
			RefreshActionsCallback actionRefresh,
			RefreshWorldCallback worldRefresh)
		{

			WorldState possible = current.Step();

			worldRefresh(possible);

			IList<Action> actions = actionRefresh(possible);

		Debug.Log("Found " + actions.Count + " actions.");

			return new Plan(actions);
		}

	}
}
