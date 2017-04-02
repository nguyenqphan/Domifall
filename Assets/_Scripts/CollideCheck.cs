using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideCheck : MonoBehaviour {

	public delegate void ActionCollide();
	public static event ActionCollide Colliding;

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag("ActiveDomino"))
		{
			if(Colliding != null)
			{
				Colliding();
			}
		}
	}
}
