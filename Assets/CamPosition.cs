using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPosition : MonoBehaviour {
	[HideInInspector]
	public Transform[] transArray;
	[HideInInspector]
	public int index = 1;

	void Awake()
	{
		transArray = GetComponentsInChildren<Transform>();
	}
		
}
