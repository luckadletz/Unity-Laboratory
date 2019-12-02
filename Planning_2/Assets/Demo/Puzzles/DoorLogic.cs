using System.Collections;
using System.Collections.Generic;
using Planning;
using UnityEngine;

public class DoorLogic : MonoBehaviour, IStateSource
{

	public Vector3 OpenOffset;
	public ButtonLogic Button;

	private Vector3 OpenPosition;
	private Vector3 ClosedPosition;


	public bool IsOpen
	{
		get
		{
			if (Button == null)
			{
				return false;
			}

			return Button.IsPressed;
		}
	}

	public void ApplyCurrentState(World world)
	{
		// Can do nothing here - we'll get an initial update.
	}

	public void UpdateState(World possible)
	{
		// Do we even need this here?
		var buttonState = possible.GetState(Button.gameObject.name) as ButtonLogic.ButtonPlanningState;
		if(buttonState.IsPressed)
		{
			Planning.State expected = new DoorPlanningState(gameObject.name, OpenPosition);
			possible.SetState(expected);
		}
		else
		{
			Planning.State expected = new DoorPlanningState(gameObject.name, ClosedPosition);
			possible.SetState(expected);
		}		
	}

	public class DoorPlanningState : Planning.State
	{
		public Vector3 ExpectedPosition; 

		public DoorPlanningState(DoorLogic door)
		{
			Name = door.gameObject.name;

			if(door.Button.IsPressed)
			{
				ExpectedPosition = door.OpenPosition;
			}
			else
			{
				ExpectedPosition = door.ClosedPosition;
			}
		}

		public DoorPlanningState(string name, Vector3 expectedPosition)
		{
			Name = name;
			ExpectedPosition = expectedPosition;
		}

		public override object Clone()
		{
			return new DoorPlanningState(Name, ExpectedPosition);
		}
	}

	// Use this for initialization
	void Start()
	{
		if (IsOpen)
		{
			OpenPosition = transform.position;
			ClosedPosition = OpenPosition - OpenOffset;
		}
		else
		{
			ClosedPosition = transform.position;
			OpenPosition = ClosedPosition + OpenOffset;
		}
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 targetPos = IsOpen ? OpenPosition : ClosedPosition;
		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
	}

	// This is called when a planning path
	bool WillBlockPath(World state)
	{
		// Figure out where we are going to be given state
		return false;
	}

}
