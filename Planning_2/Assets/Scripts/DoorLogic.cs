using System.Collections;
using System.Collections.Generic;
using Planning;
using UnityEngine;

public class DoorLogic : MonoBehaviour, Planning.IPlanningStateSource
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

	public void ApplyCurrentState(WorldState world)
	{

	}

	public void UpdateState(WorldState possible)
	{
		// Do we even need this here? 
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
	bool WillBlockPath(Planning.WorldState state)
	{
		// Figure out where we are going to be given state
		return false;
	}

}
