using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Planning;
using UnityEngine.AI;

public class ButtonLogic : MonoBehaviour, IActionSource, IStateSource
{
	public bool IsPressed = false;

	public Vector3 PressedOffset = new Vector3(0.0f, -1.0f, 0.0f);

	private Vector3 UnpressedPosition;

	private Vector3 PressedPosition;

	#region planning

	public IList<Step> GetPossibleActions(World world, Agent agent)
	{
		IList<Step> actions = new List<Step>();

		// We only care about this butotn's state
		ButtonPlanningState buttonState = world.GetState(gameObject.name) as ButtonPlanningState;
		Vector3 buttonPos = transform.position; // If button can move, this must be in world

		if (!buttonState.IsPressed)
		{
			Step act = new PressButtonStep(this, agent, world);
			actions.Add(act);
			Debug.Log("Adding press " + gameObject.name + " action");
		}

		return actions;
	}

	public void ApplyCurrentState(World world)
	{
		world.SetState(new ButtonPlanningState(this));
	}

	public void UpdateState(World world) { } 

	public class ButtonPlanningState : Planning.State
	{
		public bool IsPressed;
		public ButtonPlanningState(ButtonLogic button)
		{
			Name = button.gameObject.name;
			IsPressed = button.IsPressed;
		}

		public ButtonPlanningState(string name, bool isPressed)
		{
			Name = name;
			IsPressed = isPressed;
		}

		public ButtonPlanningState(ButtonPlanningState buttonState)
		{
			Name = buttonState.Name;
			this.IsPressed = buttonState.IsPressed;
		}

		public override string ToString()
		{
			return "{" + "Button " + Name + " : " + IsPressed.ToString() + " }";
		}

		public override object Clone()
		{
			return new ButtonPlanningState(this);
		}

		public override bool Matches(State other)
		{
			ButtonPlanningState otherButton = other as ButtonPlanningState;

			if(otherButton == null)
			{
				// Maybe log that you're comparing a state of a different type?
				// Maybe handle this logic in base class, and rely on IEquatable in most cases (neat!)
				return false;
			}

			return IsPressed == otherButton.IsPressed;
		}

	}

	public class PressButtonStep : Planning.Step
	{
		ButtonLogic Button;
		Agent Agent;

		public PressButtonStep(ButtonLogic button, Agent agent, World world)
		{
			Button = button;
			Agent = agent;
			Cost = 1.0f; // Distance from agent? Time?

			Expected = world.Step(); // Make a deep copy here?
			var results = Expected.GetState(button.gameObject.name) as ButtonPlanningState;
			results.IsPressed = true;
		}

		public override bool Action()
		{
			Debug.Log(Agent.gameObject.name + " wants to press " + Button.name);
			Agent.GetComponent<NavMeshAgent>().SetDestination(Button.transform.position);
			return Button.IsPressed;
		}
	}

	#endregion planning

	public void OnTriggerEnter(Collider other)
	{
		if (!IsPressed && other.gameObject.GetComponent<CharacterController>() != null)
		{
			IsPressed = true;
		}
	}

	// Use this for initialization
	void Start()
	{
		if (IsPressed)
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
	void Update()
	{
		Vector3 targetPos = IsPressed ? PressedPosition : UnpressedPosition;
		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
	}
}
