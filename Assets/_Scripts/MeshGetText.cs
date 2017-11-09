using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGetText : MonoBehaviour {

	private MeshFilter meshFilter;
	private Mesh mesh;

	void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
		mesh = meshFilter.mesh;
	}

	// Use this for initialization
	void Start () {
		DestroyImmediate(mesh);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire2"))
		{
			Debug.Log("Fire 2 press");
//			mesh = new Mesh();
			if(mesh == null)
			{
				mesh = new Mesh();
			}

		}	
	}
}
