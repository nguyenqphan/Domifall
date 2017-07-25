﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicationCheck : MonoBehaviour {

	private Transform[] dominoArray;


	void Awake()
	{

		dominoArray = GetComponentsInChildren<Transform>();
	}


	// Use this for initialization
	void Start () {
		for(int i = 1; i < dominoArray.Length - 1; i++)
		{
			if(dominoArray[i].position == dominoArray[i + 1].position)
			{
				Debug.Log(dominoArray[i].name);
			}
		}

//		Debug.Log("The length of dominoes " + dominoArray.Length );
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
