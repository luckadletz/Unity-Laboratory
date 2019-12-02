/* === Copyright Luc Kadletz 2019 === */	

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : BaseBehavior
{
	public BaseNote NotePrefab;

	public GameObject NoteTarget;

	private VectorPath NotePath;
	
	void SpawnNote()
	{
		GameObject noteInstance = GameObject.Instantiate(NotePrefab.gameObject, NotePath.Begining, transform.rotation);
		
		// We probably need a good interface to build / spawn notes as needed
		VectorPathFollower noteInstanceBehavior = noteInstance.GetComponent<VectorPathFollower>();
		noteInstanceBehavior.Path = NotePath;
	}

	void Start() 
	{
		NotePath = new VectorPath(new Vector3[] {
			transform.position,
			NoteTarget.transform.position
		});
	}
	
	void Update() 
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			SpawnNote();
		}
	}

}
