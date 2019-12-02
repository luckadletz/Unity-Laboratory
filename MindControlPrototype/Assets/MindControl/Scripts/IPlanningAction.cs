using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Planning
{
    [System.Serializable]
    public class StateList : SerializableDictionary<string, float>, ICloneable
    {
        public void SetState(string name, float value)
        {
            Remove(name);
            Add(name, value);
        }

        public float GetState(string name)
        {
            float value;
            if (TryGetValue(name, out value))
                return value;
            else
                return 0.0f;
        }

        // Returns a fuzzy value of how much we've met our goal
        public float FuzzyContains(StateList goal)
        {
            // Boom, easy!
            if (goal.Count == 0) return 1.0f;

            float matchWeight = 1.0f / goal.Count;
            float match = 0.0f;
            // Each goal is weighted equally
            foreach(KeyValuePair<string, float> state in goal)
            {
                if (Mathf.Approximately(GetState(state.Key) ,state.Value))
                    match += matchWeight;
            }

            return match;
        }

        public bool Contains(StateList goal)
        {
            // TODO If I import my fuzzy library, I can just use that to eval the fuzz
            if (goal.Count == 0) return true;

            // Each goal state is required
            foreach (KeyValuePair<string, float> state in goal)
            {
                if (!Mathf.Approximately(GetState(state.Key), state.Value))
                    return false;
            }

            return true;
        }

        public object Clone()
        {
            StateList cloned = new StateList();
            
            foreach(KeyValuePair<string, float> state in this)
            {
                cloned.Add(state.Key, state.Value);
            }

            return cloned;
        }
    }

    public abstract class PlanningAction : MonoBehaviour
    {
        // A percentage [0f..1f] on how much the action can move the world to the goal.
        // 1f means the action can get all the way to the goal (is the best choice)
        // 0f means the action is currently unavailable / is not a good choice
        public abstract float Heuristic(StateList world, StateList goal);

        public abstract StateList Simulate(StateList world, StateList goal);

        public abstract IEnumerable DoAction(StateList world, StateList goal, GameObject actor);
    }

}
