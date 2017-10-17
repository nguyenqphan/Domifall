using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraControl : MonoBehaviour {

	private const int HOLDERAMOUNT = 100;
	private const int NUMOFACTIVEDOMINO = 10;

	private List<MeshControl> meshControlList;

	private int fallenAmount = 0;
	private int offsetDomiIndex = 7;

	public GameObject dominoHolder;

	private List<GameObject> dominoHolderList;
	private int holderIndex = -1;

	private List<GameObject> fallenHolderList;
	private int fallenHolderIndex = -1;

	private List<MeshControl> fallenMeshControlList;

	private Transform extraTriggerCheck;
	private Transform[] dominoTransforms;
	private Domino[] dominoes;

	private List<Transform> activeDominoes;

	private int dominoIndex = 2;
	private int currIndex = 2;
	private ColorManager colorManager;
	void Awake()
	{
		extraTriggerCheck = GameObject.FindWithTag("ExtraTriggerCheck").GetComponent<Transform>();
		colorManager = GameObject.FindWithTag("ColorManager").GetComponent<ColorManager>();
	}

	void Start()
	{
		dominoHolderList = new List<GameObject>();								//initilize a list of domino
		meshControlList = new List<MeshControl>();								//initilize a list of MeshControl component scripts
		fallenHolderList = new List<GameObject>();
		fallenMeshControlList = new List<MeshControl>();
		CreateNewHolder();														//Create the first DominoHolder
		CreateNewFallenHolder();
		dominoTransforms = GetComponentsInChildren<Transform>();				//get all Transform components of the dominoes
		dominoes = GetComponentsInChildren<Domino>();							//get all Domino components of the dominoes
		dominoTransforms[1].gameObject.SetActive(false);						//Set the parent of all dominoes inactive
		activeDominoes = new List<Transform>();									//initilize a list of active dominoes

	}

	void CreateNewHolder()
	{
		holderIndex++;
		dominoHolderList.Add(Instantiate(dominoHolder) as GameObject);
		meshControlList.Add(dominoHolderList[holderIndex].GetComponent<MeshControl>());
	}

	void CreateNewFallenHolder()
	{
		fallenHolderIndex++;
		fallenHolderList.Add(Instantiate(dominoHolder) as GameObject);
		fallenMeshControlList.Add(fallenHolderList[fallenHolderIndex].GetComponent<MeshControl>());
	}

	void MoveTriggerCheck()
	{
		extraTriggerCheck.position = dominoTransforms[dominoIndex + offsetDomiIndex].position;
		extraTriggerCheck.rotation = dominoTransforms[dominoIndex + offsetDomiIndex].rotation;
	}

	private IEnumerator InstantiateDomino()
	{
		while(true)
		{
			dominoTransforms[currIndex].SetParent(null);
			colorManager.ColorMesh(dominoTransforms[currIndex].GetComponent<MeshFilter>().mesh, currIndex);

			dominoes[currIndex - 2].transform.position = new Vector3(dominoes[currIndex - 2].transform.position.x, dominoes[currIndex - 2].transform.position.y + -1f, dominoes[currIndex - 2].transform.position.z );

			dominoes[currIndex - 2].StartMoveUp(false);				//Move it up (CurrIndex - 2 becuase the first to gameobjects dont have Domino component))
			currIndex++;

			if(dominoTransforms[currIndex].gameObject.CompareTag("StopDomino"))
			{
				StopCoroutine(InstantiateDomino());	
			}
		
			yield return null;
		}
	}

}
