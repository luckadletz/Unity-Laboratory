using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{

	public interface IPlanningActionSource 
	{
		IList<Action> GetPossibleActions(WorldState state);

	}

}