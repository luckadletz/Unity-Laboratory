using UnityEngine;
using System.Collections;
using System.Collections.Generic; // List<>
using System;

namespace Planning
{
    public abstract class Action : MonoBehaviour
    {
        // Can all agents do this action?
        public bool broadcast;
        // How "expensive" is this action?
        public float cost = 1.0f;
        // Called when checking if the action can be applied to the current world state
        abstract public bool CheckPreconditions(StateList world, StateList goal);
        // Called when planning, to simulate the effects of the action to the world
        abstract public StateList Simulate(StateList world);
        // Called after planning, when it's time to do the action.
        // Returns true if done with action, otherwise DoAction will be called again next frame
        // This should eventually do what it promised in Apply
        abstract public bool DoAction(StateList world, StateList goal, GameObject actor);
    }

} // End namespace Planning
