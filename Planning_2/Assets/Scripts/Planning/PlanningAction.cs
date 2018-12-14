/* Luc Kadletz - 12/12/2018 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{
	public abstract class Action
	{
		public float Cost { get; protected set; }

		public WorldState Expected { get; protected set; }

		public abstract bool Execute(GameObject actor);
	}
}
	