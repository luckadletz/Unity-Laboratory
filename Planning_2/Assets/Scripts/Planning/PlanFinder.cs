/* Luc Kadletz - 12/12/2018 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{
	public class PlanFinder
	{
		public delegate IList<Step> RefreshActionsCallback(World possible);
		public delegate void RefreshWorldCallback(World possible);

		public Plan MakePlan( 
			World current, IGoal goal,
			RefreshActionsCallback actionRefresh, RefreshWorldCallback worldRefresh)
		{
			worldRefresh(current);
			PlanTree tree = new PlanTree(start: current);

			Debug.Log("Current" + current);
			Debug.Log("Goal" + goal);

			while(!tree.IsEmpty())
			{
				// Select the endpoint of the easiest plan we have
				// This is bredth-first, try "closest" for A*like
				PlanTree.Node n = tree.PopCheapestLeaf(); 

				if(goal.MeetsGoal(n.Expected))
				{	
					Debug.Log("Plan found!");
					return tree.GetPlan(n);
				}

				IList<Step> actions = actionRefresh(n.Expected);
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
