/* Luc Kadletz - 12/12/2018 */

using System.Collections;
using System.Collections.Generic;

namespace Planning
{
	public class WorldState  
	{
		public void SetState(string name, float value);

		public void GetState(string name, float value);

		public bool Matches(WorldState requirements);
	}
}