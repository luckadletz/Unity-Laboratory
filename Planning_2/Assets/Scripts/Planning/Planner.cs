/* Luc Kadletz - 12/12/2018 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Planning
{
	public class Planner 
	{
		public delegate IList<Action> RefreshActionsCallback(WorldState possible);

		public class Plan
		{

		}

		public Plan MakePlan(
			WorldState current, 
			WorldState goal, 
			RefreshActionsCallback callback)
		{
			throw new NotImplementedException();
		}

	}
}
