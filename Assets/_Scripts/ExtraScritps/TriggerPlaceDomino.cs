using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlaceDomino : MonoBehaviour {

	public delegate void ActionTrigger();
	public static event ActionTrigger TriggerDomino;


	void OnTriggerEnter(Collider collider)
	{

		if(TriggerDomino != null)
		{
			TriggerDomino();
			Destroy(gameObject);
		}


	}
}
