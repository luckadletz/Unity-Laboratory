/* Luc Kadletz - 12/12/2018 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{
	public abstract class Step
	{
		public float Cost { get; protected set; }

		public World Expected { get; protected set; }

		// Returns true when finished, false to run again next frame
		public abstract bool Action(Agent agent);
	}
}
