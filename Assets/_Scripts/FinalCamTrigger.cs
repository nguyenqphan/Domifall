using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCamTrigger : MonoBehaviour {

	private CameraMove cameraMove;
	private GameManager gameManager;

	void Awake()
	{
		gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
		cameraMove = GameObject.FindWithTag("CameraHolder").GetComponent<CameraMove>();
	}


	void OnTriggerEnter(Collider c)
	{
		if(gameManager.win)
			cameraMove.MoveCamToLastPosition();
	}
}
