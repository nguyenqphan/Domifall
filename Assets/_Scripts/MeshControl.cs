using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshControl : MonoBehaviour {
	private Control control;

	private const int DOMINO_VERT_UV = 24;								// Number of vertices for a domino
	private const int DOMINO_TRIANGlE = 36;								// Number of triangle for a domino

	private int meshLength;												// Length of a mesh
	private int i;														// Index	

	private Vector3[] vertsArray;										// Vertices of the mesth
	private Vector2[] uvsArray;											// UVs of the mesh	
	private int[] triangleArray;										// Triangles of the mesh
	private Vector3 tempVector3Zero = new Vector3(0f,0f,0f);			

	private Mesh[] newMesh;
	private int mIndex = 0;


	[HideInInspector] public MeshFilter[] meshFilters = new MeshFilter[2];
	[HideInInspector] public int indexMF = 0;
	private CombineInstance[] combine;

	private int numOfCombineDomi = 0;									// Number of dominoes combined
	private int numOfRemovedDomi = 0;									// Number of dominoes removed

	private MeshFilter mainMeshFilter;									
	private Matrix4x4 myTrans;

	private bool isLoaded  = false;										

	void Awake()
	{
		mainMeshFilter = GetComponent<MeshFilter>();
		myTrans = transform.worldToLocalMatrix;
		control = GameObject.FindWithTag("Control").GetComponent<Control>();
	}

	void Start()
	{
		combine		 	 = new CombineInstance[2];

		mainMeshFilter.mesh = new Mesh();					//Without this condition, we will get a warning "Combine instance mesh 0 is null" becuase there is no mesh for a dominoHolder when it first creates.
		meshFilters[0] = mainMeshFilter;
		newMesh = new Mesh[control.HOLDERAMOUNT + 1];
	}

	public void Combine()
	{
		mIndex++;
		numOfCombineDomi++;

		combine[0].mesh = meshFilters[indexMF].sharedMesh;
		combine[0].transform = myTrans * meshFilters[indexMF].transform.localToWorldMatrix;

		combine[1].mesh = meshFilters[indexMF + 1].sharedMesh;
		combine[1].transform = myTrans * meshFilters[indexMF + 1].transform.localToWorldMatrix;


		newMesh[mIndex] = new Mesh();
		newMesh[mIndex].CombineMeshes(combine, true);

		meshFilters[0].sharedMesh = newMesh[mIndex];

		DestroyImmediate(newMesh[mIndex - 1]);
	}

	//Remove a domino from a mesh made of out combined donino meshes
	public void RemoveLastSubmesh()
	{

		if(!isLoaded)
		{
			isLoaded = true;
			LoadLastMesh();
		}

		numOfRemovedDomi++;

		for(i = meshLength - DOMINO_VERT_UV * numOfRemovedDomi; i < meshLength; i++)
		{
			vertsArray[i] = tempVector3Zero;
		}

		meshFilters[0].mesh.vertices = vertsArray;

	}

	public void RemoveFirstSubmesh()
	{

		if(!isLoaded || !control.isLastMeshLoaded)
		{
			isLoaded = true;
			LoadLastMesh();
		}

		numOfRemovedDomi++;

		for(i = DOMINO_VERT_UV * (numOfRemovedDomi - 1); i < DOMINO_VERT_UV * numOfRemovedDomi; i++)
		{
			vertsArray[i] = tempVector3Zero;
		}


		meshFilters[0].mesh.vertices = vertsArray;

	}

	public void LoadLastMesh()
	{
		meshLength = meshFilters[0].mesh.vertices.Length;	
		vertsArray = new Vector3[meshFilters[0].mesh.vertices.Length];
		uvsArray = new Vector2[meshFilters[0].mesh.uv.Length];
		triangleArray = new int[meshFilters[0].mesh.triangles.Length]; 

		vertsArray = meshFilters[0].mesh.vertices;
		uvsArray = meshFilters[0].mesh.uv;
		triangleArray = meshFilters[0].mesh.triangles;
	}

	IEnumerator UnLoadUnsedMesh()
	{
		yield return null;
		Resources.UnloadUnusedAssets();
	}

}