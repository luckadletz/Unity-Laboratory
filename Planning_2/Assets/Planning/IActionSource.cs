using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{

	public interface IActionSource 
	{
		IList<Step> GetPossibleActions(World state, Agent agent);
	}

} 