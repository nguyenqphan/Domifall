using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCheck : MonoBehaviour {

	public delegate void AcctionCallGameOver();
	public static event AcctionCallGameOver CallGameOver;

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag("ActiveDomino"))
		{
			if(CallGameOver != null)
			{
				CallGameOver();
				Destroy(this.gameObject);
			}
		}


	}
}
