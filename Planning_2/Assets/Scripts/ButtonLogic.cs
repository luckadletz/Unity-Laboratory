using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLogic : MonoBehaviour 
{

	public bool IsPressed = false;

	public Vector3 PressedOffset = new Vector3(0.0f, -1.0f, 0.0f);

	private Vector3 UnpressedPosition;
	private Vector3 PressedPosition;

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
