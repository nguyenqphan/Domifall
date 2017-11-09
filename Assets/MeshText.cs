using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshText : MonoBehaviour {

	private MeshFilter meshFiler;
	public GameObject gameObject;

	void Awake()
	{
		meshFiler = GetComponent<MeshFilter>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1"))
		{
			Debug.Log("Mouse Pressed");
			GameObject newObject =  Instantiate(gameObject, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;

		}
	}
}
