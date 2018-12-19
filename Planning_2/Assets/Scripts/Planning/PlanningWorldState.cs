/* Luc Kadletz - 12/12/2018 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace Planning
{
	public class WorldState  
	{

		public void SetState(State state)
		{
			States[state.Name] = state; // NOTE probably want a Clone() here...
		}

		public State GetState(string name)
		{
			if(States.ContainsKey(name))
			{
				return States[name];
			}
			
			throw new NotImplementedException(); // Probably want a concept of a null state here
		}

		public bool HasState(string name)
		{
			return States.ContainsKey(name);
		}

		public bool Matches(WorldState expectations)
		{
			foreach(State expectation in expectations.States.Values)
			{
				State current = GetState(expectation.Name);
				if(current.Matches(expectation)) // Order might be tricky here
				{
					return false;
				}
			}
			return true;
		}

		public WorldState Step()
		{
			// Do a (preferably lazy) copy here
			throw new NotImplementedException();
		}

		private Dictionary<string, State> States = new Dictionary<string, State>();

	}
}