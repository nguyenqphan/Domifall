  using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour {

	private Transform trans;

	public delegate void ActionStopCamera();
	public static event ActionStopCamera StopCamera;

	void Awake()
	{
		trans = GetComponent<Transform>();
	}

	void OnTriggerEnter(Collider collider)
	{
		if(StopCamera != null)
		{
			StopCamera();
		}
	}

	public void LookAtDomino(Transform target)
	{
		trans.LookAt(target); 
	}
}
