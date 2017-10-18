using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCamPosition : MonoBehaviour {

	private Transform[] trans;

	void Awake()
	{
		Debug.Log("DebugCamPosition script is attached");
		trans = GetComponentsInChildren<Transform>();
		for(int i = 0; i < trans.Length; i++)
		{
			if(trans[i].gameObject.CompareTag("CamPos"))
				Debug.Log(trans[i].name);
			if(trans[i].gameObject.CompareTag("CamPositionWin"))
				Debug.Log(trans[i].name + " Win");
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
