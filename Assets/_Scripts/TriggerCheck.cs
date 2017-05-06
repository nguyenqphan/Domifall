using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheck : MonoBehaviour {

	public delegate void ActionTrigger();
	public static event ActionTrigger Triggering;
	private bool isTriggered = false;

	void OnTriggerEnter(Collider collider)
	{
		
		if(Triggering != null && isTriggered)
		{
			Triggering();
		}

		isTriggered = !isTriggered;
	}
}
