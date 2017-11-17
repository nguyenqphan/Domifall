using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraControl : MonoBehaviour {

	public GameObject extraDominoHolder;

	private Transform[] dominoTransforms;
	private Domino[] dominoes;
	private int currIndex = 2;
	private ColorManager colorManager;
	private List<ExtraMeshControl> extraMeshControlList;

	private List<GameObject> dominoHolderList;
	private int holderIndex = -1;
	private List<GameObject> extraFallenHolderList;		//Store a fallen domino list
	private int fallenHolderIndex = -1;				//Index of fallenHolderList

	private List<ExtraMeshControl> fallenMeshControlList;

	private const int NUMOFACTIVEDOMINO = 10;
	 public int HOLDERAMOUNT = 50;
	[HideInInspector] public bool isLastMeshLoaded = false;

	private Transform triggerCheck;				

	void OnEnable()
	{
		ExtraTriggerCheck.ExtraTriggering += NextDomino;
		TriggerPlaceDomino.TriggerDomino += StartPlaceDomino;
	}

	void OnDisable()
	{
		ExtraTriggerCheck.ExtraTriggering -= NextDomino;
		TriggerPlaceDomino.TriggerDomino -= StartPlaceDomino;
	}

	void Awake()
	{
		dominoTransforms = GetComponentsInChildren<Transform>();
		dominoes = GetComponentsInChildren<Domino>();
		colorManager = GameObject.FindWithTag("ExtraColorManager").GetComponent<ColorManager>();
		triggerCheck = GameObject.FindWithTag("ExtraTriggerCheck").GetComponent<Transform>();
	}

	void Start()
	{
		extraMeshControlList = new List<ExtraMeshControl>();
		extraFallenHolderList = new List<GameObject>();
		dominoHolderList = new List<GameObject>();
		fallenMeshControlList = new List<ExtraMeshControl>();
//		CreateNewHolder();
		CreateNewFallenHolder();
		dominoTransforms[1].gameObject.SetActive(false);
//		StartPlaceDomino();
	}

	void StartPlaceDomino()
	{
		StartCoroutine("PlaceDomino");
	}
		
	private IEnumerator PlaceDomino()
	{

		while (true)
		{
			if (currIndex - 2 > dominoes.Length - 1) {
				lastDominoIndex = currIndex - NUMOFACTIVEDOMINO - 2;
				StartCoroutine (CombineAndDecombine ());
				MoveTriggerCheck ();
				StopCoroutine (PlaceDomino ());
				yield break;
			}
			
			dominoTransforms [currIndex].SetParent (null);
			dominoTransforms [currIndex].gameObject.SetActive (true);
			colorManager.ColorMesh (dominoTransforms [currIndex].GetComponent<MeshFilter> ().mesh, currIndex);
			
			dominoes [currIndex - 2].transform.position = new Vector3 (dominoTransforms [currIndex].position.x, dominoTransforms [currIndex].position.y - 1f, dominoTransforms [currIndex].position.z);
			dominoes [currIndex - 2].StartMoveUp (false);
			
			if (currIndex - 2 - NUMOFACTIVEDOMINO >= 0) {
				extraMeshControlList [holderIndex].meshFilters [1] = dominoTransforms [currIndex - NUMOFACTIVEDOMINO].GetComponent<MeshFilter> ();
				extraMeshControlList [holderIndex].Combine ();
				dominoTransforms [currIndex - NUMOFACTIVEDOMINO].gameObject.SetActive (false);
			}

			if(dominoTransforms[currIndex].CompareTag("StoppedDomino"))
			{
				StopCoroutine("PlaceDomino");

			}

			currIndex++;
			if ((currIndex - NUMOFACTIVEDOMINO - 2) % HOLDERAMOUNT == 0) {
				CreateNewHolder ();
			}

			yield return new WaitForSeconds(0.3f);;
		}

	}

	private int headDominoIndex = 2;
	private int tempHoderIndex = 0;
	void Decombine()
	{
		if (tempHoderIndex <= holderIndex) {

			dominoTransforms[headDominoIndex].gameObject.SetActive(true);
			dominoTransforms[headDominoIndex].GetComponent<Rigidbody>().isKinematic = false;
			extraMeshControlList [tempHoderIndex].RemoveFirstSubmesh ();
			headDominoIndex++;
			if ((headDominoIndex - 2) % HOLDERAMOUNT == 0) {
				tempHoderIndex++;

			}
		}
	}

	private int remainingDominos = 0;
	private	IEnumerator CombineAndDecombine()
	{
		yield return new WaitForSeconds(2f);
		while(currIndex > currIndex - NUMOFACTIVEDOMINO + remainingDominos){

			extraMeshControlList[holderIndex].meshFilters[1] = dominoTransforms[ currIndex - NUMOFACTIVEDOMINO + remainingDominos].GetComponent<MeshFilter>();	//get the previous domino meshFilter to combine
			extraMeshControlList[holderIndex].Combine();													
			dominoTransforms[currIndex - NUMOFACTIVEDOMINO + remainingDominos].gameObject.SetActive(false);

			Decombine();

			remainingDominos++;
			yield return null;
		}

		isLastMeshLoaded = true;

	}


	void CreateNewHolder()
	{
		holderIndex++;
		dominoHolderList.Add(Instantiate(extraDominoHolder) as GameObject);
		extraMeshControlList.Add(dominoHolderList[holderIndex].GetComponent<ExtraMeshControl>());
	}

	void CreateNewFallenHolder()
	{
		fallenHolderIndex++;
		extraFallenHolderList.Add(Instantiate(extraDominoHolder) as GameObject);
		fallenMeshControlList.Add(extraFallenHolderList[fallenHolderIndex].GetComponent<ExtraMeshControl>());
	}

	void NextDomino()
	{
		CombineFallenDomino();
		MoveTriggerCheck();
	}


	private int dominoIndex = 2;
	private int lastDominoIndex = 0;				//Store the last domino index
	private  int fallenAmount = 0;					//Store the number of dominos has fallen and need to combine into a mesh
	void CombineFallenDomino()
	{
		fallenMeshControlList[fallenHolderIndex].meshFilters[1] = dominoTransforms[dominoIndex].GetComponent<MeshFilter>();

		fallenMeshControlList[fallenHolderIndex].Combine();													
		dominoTransforms[dominoIndex].gameObject.SetActive(false);

		Decombine();

		if(dominoIndex > lastDominoIndex)
		{
			return;
		}

		dominoIndex++;
		currIndex--;
		fallenAmount++;



		if(fallenAmount % HOLDERAMOUNT == 0)
		{
			CreateNewFallenHolder();
		}
	}


	private int offsetDomiIndex = 7;				//The offset index used to place the triggerCheck 
	void MoveTriggerCheck()
	{
		triggerCheck.position = dominoTransforms[dominoIndex + offsetDomiIndex].position;
		triggerCheck.rotation = dominoTransforms[dominoIndex + offsetDomiIndex].rotation;
	}


}