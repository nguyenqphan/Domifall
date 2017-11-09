using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Manager : MonoBehaviour {

	private const int NUMOFVERTICES = 24;
	private const int NUMOFUV = 24;
	private const int NUMOFTRIANGLE = 36;

	[HideInInspector] public Transform[] cubeTrans;

	Mesh mesh;

	private Vector3[] newVertices;
	private Vector2[] newUV;
	private int[] newTriangles;

	MeshFilter meshFilter;

	void Awake()
	{
		cubeTrans = GetComponentsInChildren<Transform>();
		meshFilter = GetComponent<MeshFilter>();

	}
	// Use this for initialization
	void Start () {
		cubeTrans[1].gameObject.SetActive(false);
		CreateMesh(100);
	}


	int index = 1;
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1"))
		{	
			index++;
//			Debug.Log("Cube index " + index);
			cubeTrans[index].SetParent(null);
			cubeTrans[index].gameObject.SetActive(true);
			MergeMesh(cubeTrans[index].GetComponent<MeshFilter>().mesh, cubeTrans[index]);

		}
	}


	void CreateMesh(int length)
	{
		mesh = meshFilter.mesh;


		newVertices = new Vector3[NUMOFVERTICES * length];
		newUV = new Vector2[NUMOFUV * length];
		newTriangles = new int[NUMOFTRIANGLE * length];

		mesh.vertices = newVertices;
		mesh.uv = newUV;
		mesh.triangles = newTriangles;
	}

	private int numOfAddedCube = 0;
	void MergeMesh(Mesh mesh, Transform trans)
	{
		for(int i = 0; i < mesh.vertices.Length; i++)
		{
			Vector3 worldPos = trans.TransformPoint(mesh.vertices[i]);
//			Vector3 localPos = trans.InverseTransformPoint(worldPos);

			newVertices[NUMOFVERTICES * numOfAddedCube + i] = worldPos;
			newUV[NUMOFUV * numOfAddedCube + i] = mesh.uv[i];

		}

		for(int i = 0; i < mesh.triangles.Length; i++)
		{
			newTriangles[NUMOFTRIANGLE * numOfAddedCube + i] = mesh.triangles[i] + (NUMOFVERTICES * numOfAddedCube);
		}

		meshFilter.sharedMesh.vertices = newVertices;
		meshFilter.sharedMesh.uv = newUV;
		meshFilter.sharedMesh.triangles = newTriangles;

		meshFilter.sharedMesh.RecalculateBounds();
		meshFilter.sharedMesh.RecalculateNormals();
		meshFilter.sharedMesh.RecalculateTangents();

		cubeTrans[index].gameObject.SetActive(false);
		numOfAddedCube++;
	}


}
