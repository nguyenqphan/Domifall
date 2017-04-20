using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshControl : MonoBehaviour {

	[HideInInspector]
	public MeshFilter[] meshFilters = new MeshFilter[2];
	private CombineInstance[] combine;
	private List<Vector3> vertices;
	private List<Vector2> uvs ;
	private List<int> triangles ;

	private int numOfCombineDomi;
	private MeshFilter mainMeshFilter;
	private Matrix4x4 myTrans;

	void Awake()
	{
		mainMeshFilter = GetComponent<MeshFilter>();
		myTrans = transform.worldToLocalMatrix;
	}

	void Start()
	{
		combine		 	 = new CombineInstance[2];
		vertices		 = new List<Vector3>();
		uvs 		     = new List<Vector2>();
		triangles 		 = new List<int>();

		mainMeshFilter.mesh = new Mesh();					//Without this condition, we will get a warning "Combine instance mesh 0 is null" becuase there is no mesh for a dominoHolder when it first creates.
		meshFilters[0] = mainMeshFilter;
	}

	//This medthod combines the meshes of meshFilters into one mesh.
	public void Combine()
	{
		numOfCombineDomi++;

		combine[0].mesh = meshFilters[0].sharedMesh;
		combine[0].transform = myTrans * meshFilters[0].transform.localToWorldMatrix;

		combine[1].mesh = meshFilters[1].sharedMesh;
		combine[1].transform = myTrans * meshFilters[1].transform.localToWorldMatrix;

		meshFilters[0].mesh = new Mesh();
		meshFilters[0].mesh.CombineMeshes(combine, true);

		if(numOfCombineDomi == 100)
		{

			vertices.AddRange(meshFilters[0].mesh.vertices);
			uvs.AddRange(meshFilters[0].mesh.uv);
			triangles.AddRange(meshFilters[0].mesh.triangles);
		

			vertices.RemoveRange(vertices.Count - 24 * 2, 24 * 2);
			uvs.RemoveRange(uvs.Count - 24 * 2, 24 * 2);
			triangles.RemoveRange(triangles.Count - 36* 2 , 36 * 2) ;

			meshFilters[0].mesh = new Mesh();
			meshFilters[0].mesh.vertices = vertices.ToArray();
			meshFilters[0].mesh.uv = uvs.ToArray();
			meshFilters[0].mesh.triangles = triangles.ToArray();

//			meshFilters[0].mesh.RecalculateBounds();
//			meshFilters[0].mesh.RecalculateNormals();

		}
	}


	public void RemoveLastSubmesh()
	{
		vertices.AddRange(meshFilters[0].mesh.vertices);
		uvs.AddRange(meshFilters[0].mesh.uv);
		triangles.AddRange(meshFilters[0].mesh.triangles);

	}


}
