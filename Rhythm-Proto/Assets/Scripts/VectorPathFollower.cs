/* === Copyright Luc Kadletz 2019 === */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorPathFollower : BaseBehavior
{
	public VectorPath Path;
	
	[Range(0.0f, 1.0f)]
	public float PathPosition;

	public float Speed;
	
	[Tooltip("If this is checked, Speed will use world speed instead of Percentage speed")]
	public bool WorldSpeed;

	void Update() 
	{
		UpdatePosition();
		
		if(PathPosition > 1.0)
		{
			PathPosition = 1.0f;
			OnPathComplete();
		}
	}

	private void UpdatePosition()
	{
		if(WorldSpeed)
		{
			PathPosition += (Speed / Path.Distance()) * Time.deltaTime; 
		}
		else
		{
			PathPosition += Speed * Time.deltaTime;
		}
		
		if(Path != null)
		{
			transform.position = Path.GetPointAlongPath(PathPosition);
		}
	}

    void OnPathComplete()
	{
		GameObject.Destroy(gameObject);
	}

}
