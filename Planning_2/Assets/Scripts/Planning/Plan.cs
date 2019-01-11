/* Luc Kadletz - 12/23/2018 */

using System.Collections.Generic;
using Planning;
using UnityEngine;

namespace Planning
{
	public class Plan
	{
		private Queue<Step> Actions;

		public Plan(IList<Step> actions)
		{
			Actions = new Queue<Step>(actions);
		}

		public int Count { get { return Actions.Count; } }

		public bool Step()
		{
			if (Actions.Count == 0) return true;

			bool done = Actions.Peek().Action();
			if (done)
			{
				Actions.Dequeue();
			}
			return Actions.Count == 0;
		}

	}
}
