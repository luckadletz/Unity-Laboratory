using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planning;

public class ButtonLogic : MonoBehaviour, IPlanningActionSource
{
	public bool IsPressed = false;

	public Vector3 PressedOffset = new Vector3(0.0f, -1.0f, 0.0f);

	private Vector3 UnpressedPosition;

	private Vector3 PressedPosition;

#region  planning

    public IList<Action> GetPossibleActions(WorldState world)
    {
		IList<Action> actions = new List<Action>();

		// We only care about this butotn's state
		ButtonPlanningState buttonState = world.GetState(gameObject.name) as ButtonPlanningState;

		if(!buttonState.IsPressed)
		{
			actions.Add(new PressButtonAction(this, world));
		}

		return actions;
    }

	public class ButtonPlanningState : Planning.State
	{
		public bool IsPressed;
        public ButtonPlanningState(ButtonLogic button)
		{
			Name = button.gameObject.name;
			IsPressed = button.IsPressed;
		}
    }

	public class PressButtonAction : Planning.Action
	{
		ButtonLogic Button;

		public PressButtonAction(ButtonLogic button, WorldState world)
		{
			Button = button;

			Cost = 1.0f; // Distance from agent (how do we know agent?)
			
			Expected = world.Step(); // Make a lazy copy here
			var results = Expected.GetState(button.gameObject.name) as ButtonPlanningState;
			results.IsPressed = true;
		}

		public override bool Execute(GameObject actor)
		{
			Debug.Log(actor.name + " wants to press " + Button.name);
			return true; // ez pz
		}
	}

#endregion  planning

    public void OnTriggerEnter(Collider other)
	{
		if(!IsPressed && other.gameObject.GetComponent<CharacterController>() != null)
		{
			IsPressed = true;
		}
	}

	// Use this for initialization
	void Start () 
	{
		if(IsPressed)
		{
			PressedPosition = transform.position;
			UnpressedPosition = PressedPosition - PressedOffset;
		}
		else
		{
			UnpressedPosition = transform.position;
			PressedPosition = UnpressedPosition + PressedOffset;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 targetPos = IsPressed ? PressedPosition : UnpressedPosition;
		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
	}

	
}
