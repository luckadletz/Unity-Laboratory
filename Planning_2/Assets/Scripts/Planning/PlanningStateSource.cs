using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{

	public interface IPlanningStateSource
	{
		void ApplyCurrentState(WorldState world);
		// Do we want to pass in container??? seems sketch, but may be necessary for reactive objects...
		void UpdateState(WorldState possible);

	}

}