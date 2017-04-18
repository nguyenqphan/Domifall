using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshControl : MonoBehaviour {

	public List<MeshFilter> meshFilterList = new List<MeshFilter>();
	private int last;
	private MeshFilter mainMeshFilter;
	private Matrix4x4 myTrans;
	private CombineInstance[] combine = new CombineInstance[2];

	void Awake()
	{
		mainMeshFilter = GetComponent<MeshFilter>();
		mainMeshFilter.mesh = new Mesh();					//Without this condition, we will get a warning "Combine instance mesh 0 is null" becuase there is no mesh for a dominoHolder when it first creates.
		meshFilterList.Add(mainMeshFilter);
		myTrans = transform.worldToLocalMatrix;
	}

	//This medthod combines the mesh of the first and last element in the meshFilterList.
	public void Combine()
	{
		last = meshFilterList.Count - 1;

		combine[0].mesh = meshFilterList[0].sharedMesh;
		combine[0].transform = myTrans * meshFilterList[0].transform.localToWorldMatrix;

		combine[1].mesh = meshFilterList[last].sharedMesh;
		combine[1].transform = myTrans * meshFilterList[last].transform.localToWorldMatrix;

		meshFilterList[0].mesh = new Mesh();
		meshFilterList[0].mesh.CombineMeshes(combine, true);
	}
}
