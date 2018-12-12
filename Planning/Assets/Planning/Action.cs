using UnityEngine;
using System.Collections;
using System.Collections.Generic; // List<>
using System;

namespace Planning
{
    public interface IAction
    {
        float cost {get; set;} = 1.0f;

        // Called when checking if the action can be applied to the current world state
        bool CheckPreconditions(StateList world, StateList goal);

        // Called when planning, to simulate the effects of the action to the world
        StateList Simulate(StateList world);
        
        // Called after planning, when it's time to do the action.
        // Returns true if done with action, otherwise DoAction will be called again next frame
        // This should eventually do what it promised in Apply
        bool DoAction(StateList world, StateList goal, GameObject actor);
    }


} // End namespace Planning
