using System;

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
}