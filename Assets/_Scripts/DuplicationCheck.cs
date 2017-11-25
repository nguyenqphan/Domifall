using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicationCheck : MonoBehaviour {

	private Transform[] dominoArray;


	void Awake()
	{

		dominoArray = GetComponentsInChildren<Transform>();
		Debug.Log("The length of dominoes " + dominoArray.Length );

	}


	// Use this for initialization
	void Start () {
		for(int i = 1; i < dominoArray.Length - 1; i++)
		{
			if(dominoArray[i].position == dominoArray[i + 1].position)
			{
				Debug.Log(dominoArray[i].name);
				if(dominoArray[i].CompareTag("CamPos") || dominoArray[i].CompareTag("CamPositionWin"))
				{
					Debug.Log(dominoArray[i].name);
				}

			}
		}

			
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
