using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCamTrigger : MonoBehaviour {

	private CameraMove cameraMove;
	private GameManager gameManager;
	private UIManager uiManager;

	void Awake()
	{
		gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
		cameraMove = GameObject.FindWithTag("CameraHolder").GetComponent<CameraMove>();
		uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
	}


	void OnTriggerEnter(Collider c)
	{
		

		if(gameManager.win)
		{
			cameraMove.MoveCamToLastPosition();
		}
		else
		{
			uiManager.ShowUI();
		}
			
	}
}
