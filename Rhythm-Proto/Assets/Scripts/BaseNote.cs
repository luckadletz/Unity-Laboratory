/* === Copyright Luc Kadletz 2019 === */	

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(VectorPathFollower))]
public class BaseNote : BaseBehavior
{
    void OnEnable()
    {
        GetComponent<VectorPathFollower>().PathPosition = 0.0f;
    }
}
