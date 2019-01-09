/* Luc Kadletz - 12/12/2018 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planning
{
	[Serializable]
	public class World
	{
		private Dictionary<string, State> States = new Dictionary<string, State>();

		public void SetState(State state)
		{
			States[state.Name] = state; // NOTE probably want a Clone() here...
		}

		public State GetState(string name)
		{
			if (States.ContainsKey(name))
			{
				return States[name];
			}

			throw new NotImplementedException(); // Probably want a concept of a null state here
		}

		public bool HasState(string name)
		{
			return States.ContainsKey(name);
		}

		public bool Matches(World expectations)
		{
			Debug.Log("Checking " + expectations + " against " + this);
			foreach (var expectation in expectations.States)
			{
				Debug.Log("Checking state: " + expectation.Value);

				if(!HasState(expectation.Key))
				{
					Debug.Log("no state");
					return false;
				}

				State current = GetState(expectation.Key);
				if (!current.Matches(expectation.Value)) // Order might be tricky here
				{
					Debug.Log(current + " does not match " + expectation.Value);
					return false;
				}
				
				Debug.Log(current + " matches" + expectation.Value);
			}
			return true;
		}

		public World Step()
		{
			World copy = new World();
			foreach(var state in States)
			{
				copy.SetState(state.Value.Clone() as State);
			}
			return copy;
		}

		public override string ToString()
		{
			string worldString = "[";
			foreach (State state in States.Values)
			{
				worldString += "\t" + state.ToString() + "\n";
			}
			worldString += "]";

			return worldString;
		}


	}
}