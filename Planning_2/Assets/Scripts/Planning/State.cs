using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planning
{
	[Serializable]
	public abstract class State : ICloneable
	{
		public string Name { get; protected set; }

		public abstract object Clone();

		public virtual bool Matches(State other)
		{
			return this == other;
		}

		public override string ToString()
		{
			return "{ " + Name + " }";
		}
	}

	// Apply this attribute
	public class Plannable : System.Attribute
	{
		public string Label;
	}

	public class State<T> : State
		where T : UnityEngine.MonoBehaviour
	{

		// private States<string, object> states
		// list of all properties on T with attribute [Planning]

		private Dictionary<string, object> plannables = new Dictionary<string, object>();

		public State(T instance)
		{
			Name = instance.name + "." + typeof(T).Name;

			var properties = typeof(T).GetProperties()
				.Where(prop => prop.IsDefined(typeof(Plannable), true));
			foreach (var prop in properties)
			{
				plannables.Add(prop.Name, prop.GetValue(instance, null));
				Debug.Log(Name + "." + prop.Name + " = " + plannables[prop.Name]);
			}

			var fields = typeof(T).GetFields()
				.Where(field => field.IsDefined(typeof(Plannable), true));
			foreach (var field in fields)
			{
				plannables.Add(field.Name, field.GetValue(instance));
				Debug.Log(Name + "." + field.Name + " = " + plannables[field.Name]);
			}
		}

		public override object Clone()
		{

			throw new NotImplementedException();
		}
	}
}