using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTriggerCheck : MonoBehaviour {

	public delegate void ExtraActionTrigger();
	public static event ExtraActionTrigger ExtraTriggering;
	private bool isTriggered = false;

	void OnTriggerEnter(Collider collider)
	{
		if(ExtraTriggering != null && isTriggered)
		{
			ExtraTriggering();
		}

		isTriggered = !isTriggered;
	}
}
