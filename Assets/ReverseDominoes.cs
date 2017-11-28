using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ReverseDominoes : MonoBehaviour {

	private Transform[] trans;
	private GameObject go;

	void Awake()
	{
		trans = GetComponentsInChildren<Transform>();

		go = new GameObject("GOAtRuntime");
		for(int i = trans.Length - 1; i > 1; i--){
			trans[i].SetParent(null);
			trans[i].SetParent(go.transform);
		}


	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
