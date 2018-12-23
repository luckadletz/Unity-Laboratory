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

				if (Actions.Count == 0)
				{
					return false;
				}

				if (!Actions[0].Execute(actor))
				{
					Actions.RemoveAt(0);
				}

				return true;
			}

		}

		public Plan MakePlan(
			WorldState current,
			WorldState goal,
			RefreshActionsCallback actionRefresh,
			RefreshWorldCallback worldRefresh)
		{

			WorldState possible = current.Step();

			worldRefresh(current);

			IList<Action> actions = actionRefresh(current);

			return new Plan(actions);
		}

	}
}
