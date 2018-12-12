using UnityEngine;
using System.Collections;
using System.Collections.Generic; // List<>

namespace Planning
{
    [System.Serializable]
    public struct State
    {
        public State(string n, float v)
        {   
            Name = n;
            Value = v;
        }
        public string Name;
        public float Value;
    }
    
    [System.Serializable]
    public class StateList : ISerializationCallbackReceiver
    {
        // All states in this list
        public State[] states = new State[0];
        
        // DONE A dictionary (hash map) cache for nlogn searches 
        //      (dictionaries can't be serialized by unity)
        private Dictionary<string, float> stateTree = new Dictionary<string, float>();
        private bool stateTreeDirty;
        private bool useCache = true;  // if true then all operations are done using the cache (map), setting to false will disable all cache ops

        public StateList Copy()
        {
            SaveCache();
            StateList s = new StateList();
            s.states = states.Clone() as State[];
            s.LoadCache();
            return s;
        }

        public void OnBeforeSerialize()
        {
            SaveCache();
        }

        public void OnAfterDeserialize()
        {
            LoadCache();
        }

        public void LoadCache()
        {
            if (useCache == false)
            {
                return;
            }

            stateTree.Clear();
            for (int i = 0; i < states.Length; i++)
            {
                stateTree.Add(states[i].Name, states[i].Value);
            }
            stateTreeDirty = false;
        }

        public void SaveCache()
        {
            if (stateTreeDirty && useCache)
            {
                states = new State[stateTree.Count];
                int uIndex = 0;
                foreach (KeyValuePair<string, float> pair in stateTree)
                {
                    states[uIndex].Name = pair.Key;
                    states[uIndex].Value = pair.Value;
                    ++uIndex;
                }
                stateTreeDirty = false;
            }
            else
            {
                return;
            }
        }

        public void SetState(string name, float value)
        {
            if (useCache)
            {
                stateTree[name] = value;
                stateTreeDirty = true;
            }
            else
            {
                // If we have the state, change it
                for(int i = 0; i < states.Length; ++i)
                {
                    if (states[i].Name == name)
                    {
                        states[i].Value = value;
                        return;
                    }
                }
                // Otherwise, expand array and add it
                State[] old = states;
                states = new State[states.Length + 1];
                old.CopyTo(states, 0);
                states[states.Length - 1] = new State(name, value);
            }
        }

        public bool HasState(string name)
        {
            // DONE Use dictionary cache if avaliable
            if (useCache)
            {
                return stateTree.ContainsKey( name );
            }
            else
            {
                for (int i = 0; i < states.Length; ++i)
                {
                    if (states[i].Name == name) return true;
                }
                return false;
            }
        }

        public float GetState(string name)
        {
            if (useCache)
            {
                if ( HasState(name) )
                {
                    return stateTree[name];
                }
                else
                {
                    return 0.0f;
                }
            }
            else
            {
                for (int i = 0; i < states.Length; ++i)
                {
                    if (states[i].Name == name)
                        return states[i].Value;
                }
                // NOTE Asking about a state we don't have gives 0
                // We may want to instead throw an exception.
                return 0.0f;
            }
        }

        public bool Matches(StateList conditions)
        {
            // DONE Use dictionary cache if avaliable - otherwise this is gonna be real slow
            // Check each state for a conflict in world
            SaveCache();
            if (useCache)
            {
                foreach (KeyValuePair<string, float> pair in conditions.stateTree)
                {
                    if (GetState(pair.Key) != pair.Value)
                        return false;
                }
            }
            else
            {
                foreach (State state in conditions.states)
                {
                    // If it's not the right value, fail the match
                    if (GetState(state.Name) != state.Value)
                        return false;
                }
            }

            return true;
        }
    };
}
