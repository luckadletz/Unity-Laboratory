using UnityEngine;
using System.Collections;
using Planning;


[RequireComponent(typeof(ButtonLogic))]
[AddComponentMenu("Planning/Actions/Press Button")]
public class ButtonAction : SimpleAction
{
    // This is the button we know how to press
    private ButtonLogic Button;
    // These are doors that are blocking us from this button
    public DoorLogic[] BlockingDoors;

    // Use this for initialization
    void Start ()
    {
        broadcast = true;
        Button = GetComponent<ButtonLogic>();
        // == PRECONDITIONS == //
        preconditions = new StateList();
        // All of the blocking doors must be opened
        foreach(DoorLogic d in BlockingDoors)
        {
            string state = d.gameObject.name + " is open";
            preconditions.SetState(state, 1.0f);
        }
        // The button has to not be already pressed
        preconditions.SetState(Button.gameObject.name + " is pressed", 0.0f);
        // == POSTCONDITIONS == //
        postconditions = new StateList();
        // Our postconditions is the button is pressed
        postconditions.SetState(Button.gameObject.name + " is pressed", 1.0f);
        // And the door the button was hooked up to is opened
        if(Button.door)
            postconditions.SetState(Button.door.gameObject.name + " is open", 1.0f);
    }


    // Called after planning, when it's time to do the action.
    // Returns true if done with action, otherwise DoAction will be called again next frame
    // This should eventually do what it promised in Apply
    public override bool DoAction(StateList world, StateList goal, GameObject actor)
    {
        // Walk to the button
        actor.GetComponent<NavMeshAgent>().destination = Button.transform.position;

        // If there was a door, and it's open, we're good to go!
        if (Button.isPressed)
        {
            world = Simulate(world);
            return true;
        }
        else
            return false;
    }
}
