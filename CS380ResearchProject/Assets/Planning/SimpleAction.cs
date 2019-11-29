using UnityEngine;
using System.Collections;
using Planning;

[AddComponentMenu("Planning/Actions/Simple Action")]
public class SimpleAction : Action
{
    public StateList preconditions = new StateList();
    public StateList postconditions = new StateList();

    // Called when checking if the action can be applied to the current world state
    public override bool CheckPreconditions(StateList world, StateList goal)
    {
        // goal is provided as a parameter to be used in derived functions
        // If all precons pass, then we're good
        return world.Matches(preconditions);
    }

    // Called when planning, to simulate the effects of the action to the world
    public override StateList Simulate(StateList world, StateList goal)
    {
        StateList applied = world.Copy();
        postconditions.SaveCache();
        // check each property in the state, and change it from precondition to postcondition
        foreach(State postcondition in postconditions.states)
        {
            applied.SetState(postcondition.Name, postcondition.Value);
        }
        return applied;
    }

    // Called after planning, when it's time to do the action.
    // Returns true if done with action, otherwise DoAction will be called again next frame
    // This should eventually do what it promised in Apply
    public override bool DoAction(StateList world, StateList goal, GameObject actor)
    {
        world = Simulate(world, goal);
        return true;
    }
};
