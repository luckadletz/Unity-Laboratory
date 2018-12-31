/* Luc Kadletz - 12/12/2018 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{
	public class Agent
	{
		public delegate IList<Step> RefreshActionsCallback(World possible);
		public delegate void RefreshWorldCallback(World possible);

		public RefreshActionsCallback RefreshActions = null;
		public RefreshWorldCallback RefreshWorld = null;

		public string Name; // Should be unique, TODO use for data indexing

		public Plan MakePlan( 
			World current, World goal,
			RefreshActionsCallback actionRefresh, RefreshWorldCallback worldRefresh)
		{
			worldRefresh(current);
			PlanTree tree = new PlanTree(start: current);

			Debug.Log("Current" + current);
			Debug.Log("Goal" + goal);

			while(!tree.IsEmpty())
			{
				PlanTree.Node n = tree.PopCheapestLeaf(); // This is bredth-first, try "closest" for A*like

				if(n.Step.Expected.Matches(goal))
				{
					Debug.Log("Plan found!");
					return tree.GetPlan(n);
				}

				IList<Step> actions = actionRefresh(n.Step.Expected);
				Debug.Log("Found " + actions.Count + " actions.");
				foreach(Step a in actions)
				{
					tree.AddStep(n, a);
				}
			}

			Debug.Log("No plan exists!");
			return new Plan(new List<Step>());
		}
	}
}
