using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCheck : MonoBehaviour {

	public delegate void AcctionCallGameOver();
	public static event AcctionCallGameOver CallGameOver;
	private string activeDomino = "ActiveDomino";

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag(activeDomino))
		{
			if(CallGameOver != null)
			{
				CallGameOver();
//				Debug.Log("GameOver");
//				Destroy(this.gameObject);
			}
		}


	}
}
