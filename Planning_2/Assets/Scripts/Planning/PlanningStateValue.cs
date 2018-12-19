using System;

namespace Planning
{
    [Serializable]
    public abstract class State
    {
        public string Name { get; protected set; }

        public bool Matches(State other)
        {
            return this == other;
        }
    }
}