using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshControl : MonoBehaviour {

	public List<MeshFilter> meshFilterList = new List<MeshFilter>();
	private int i;
	private MeshFilter mainMeshFilter;
	private Matrix4x4 myTrans;

	void Awake()
	{
		mainMeshFilter = GetComponent<MeshFilter>();
		meshFilterList.Add(mainMeshFilter);
		myTrans = transform.worldToLocalMatrix;
	}

	public void Combine()
	{
		CombineInstance[] combine = new CombineInstance[meshFilterList.Count];

		i = 0;

		//Without this condition, we will get a warning "Combine instance mesh 0 is null"
		//becuase there is no mesh for a dominoHolder when it first creates.
		if(meshFilterList[i].mesh == null)
		{
			meshFilterList[i].mesh = new Mesh();
		}

		while(i < meshFilterList.Count)
		{
			combine[i].mesh = meshFilterList[i].sharedMesh;
			combine[i].transform = myTrans * meshFilterList[i].transform.localToWorldMatrix;
			i++;
		}

		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
	}
}
