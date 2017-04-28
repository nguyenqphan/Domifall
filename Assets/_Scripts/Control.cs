using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

	private const int holderAmount = 100;			//The number of dominos will combine into a mesh for better performance
	private const int numOfActiveDomi = 10;			//The number of active dominoes
	private List<MeshControl> meshControlList;		//Store MeshControl scripts as a List

	private string placeDomino = "PlaceDomino";
	private string layout = "Layout";
	private string fire1Button = "Fire1";

	private int lastDominoIndex = 0;				//Store the last domino index

	private  int fallenAmount = 0;					//Store the number of dominos has fallen and need to combine into a mesh
	private int offsetDomiIndex = 9;				//The offset index used to place the triggerCheck 

	private bool isGameOver = false;				

	//NEED TO BE DELETED
	private bool test = false;

	public GameObject dominoHolder;					//a domino holder prefab

	private List<GameObject> dominoHolderList;		//A List of domino holders
	private int holderIndex = -1;					//index of the domino holder list

	private List<GameObject> fallenHolderList;		//Store a fallen domino list
	private int fallenHolderIndex = -1;				//Index of fallenHolderList

	private List<MeshControl> fallenMeshControlList;

	private Transform gameoverCheck;			//OnTrigger Enter, game is over.
	private Transform triggerCheck;				//on trigger enter, remove a domino from a  combined mesh and combine another fallen domino to another mesh. 
	private Transform collideCheck;				//if collision detected, then continue to place dominos
	private Transform knockDomino;				//Store the Transform of a game object that is used to initially knock down a domino	
	private Transform[] dominoTransforms;		//Store the transform component of dominoes
	private Domino[] dominoes;					//Store the dominoes component

	private Knocker knockComponent;				//reference to Knocker script

	private List<Transform> activeDominoes;		//Store active dominoes that have rigibody and box collider



	private int currIndex = 2;					//the current index of an active domino. Starting with 2 because the first two game objects are parents of dominoes
	private int randomTarget = 1;				//a target for a random number to hit
	private bool isInteraciveDomino = false;	//Is the current domino interactive?
	private float preAngleX;					//Cache the rotation of x of previous dominos

		
	private Vector3 PosA;						//Cache the position of a domino before moving it
	private Vector3 PosB;						//Cache the position of a domino before moving it
	[HideInInspector]	
	public Vector3 dir;							//A direction for a knock domino (PosB - PosA)

	//Register events
	void OnEnable()
	{
		CollideCheck.Colliding  += InvokeRepeatingDomino;										//Register an event
		TriggerCheck.Triggering += PreviousDomino;
		Domino.GameOverCheck += MoveGameOverCheck;
		GameOverCheck.CallGameOver += SetGameOver;

	}

	//remove regitered events
	void OnDisable()
	{
		CollideCheck.Colliding  -= InvokeRepeatingDomino;										//Unregister an event
		TriggerCheck.Triggering -= PreviousDomino;
		Domino.GameOverCheck -= MoveGameOverCheck;
		GameOverCheck.CallGameOver -= SetGameOver;

	}


	void Awake()
	{
		collideCheck = GameObject.FindWithTag("CollideCheck").GetComponent<Transform>();		
		knockDomino = GameObject.FindWithTag("KnockDomino").GetComponent<Transform>();
		gameoverCheck = GameObject.FindWithTag("GameOverCheck").GetComponent<Transform>();
		triggerCheck = GameObject.FindWithTag("TriggerCheck").GetComponent<Transform>();
		knockComponent = GameObject.FindWithTag("KnockDomino").GetComponent<Knocker>();

	}

	// Use this for initialization
	void Start ()
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
		InvokeRepeating(layout, 1f, 0.3f);	

	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetButtonDown(fire1Button))
		{
			if(!test)
			{
				test = true;
				InvokeRepeatingDomino();
			}
		}
	}

	//call this method to keep placing a domino at a certain interval time
	void InvokeRepeatingDomino()
	{
		InvokeRepeating(placeDomino, 1f, 0.3f);
	}

	void Layout()
	{
		dominoTransforms[currIndex].SetParent(null);							//remove the domino from its parent
		dominoes[currIndex - 2].StartMoveUp(isInteraciveDomino);				//Move it up (CurrIndex - 2 becuase the first to gameobjects dont have Domino component))
		activeDominoes.Add(dominoTransforms[currIndex]);
//		AddActiveDomino(dominoTransforms[currIndex]);							//Add the domino to active domino list

		currIndex++;

		if(currIndex  - 1 > numOfActiveDomi)									//Subtract 1 becuase becuase currIndex startd with 2 so the number of active dominos can be flexibly change for testing.
		{
			PosB = dominoTransforms[currIndex - 1].position;
			CancelInvoke(layout);														//Stop placing dominos after it meets a certain condition
		}
	}

	//Rondom method to decide if a domino should be interactive
	private bool IsActiveCube()
	{
		return (currIndex % 2 == 0) && randomTarget == Random.Range(1, 100);
	}

	//a method to place a domino
	void PlaceDomino()
	{
		if(isGameOver) {return;}											//if isGameOver is true, no need to do the rest

		preAngleX = dominoTransforms[currIndex - 1].eulerAngles.x;
													
		//A condition to figure out whether or not a domino is falled
		if(preAngleX < 15 || preAngleX > 345)
		{
			//Reached the last domino
			if(currIndex > dominoTransforms.Length  - 1)
			{
				MoveTriggerCheck();
				Knock ();														
				CancelInvoke ();
				knockComponent.StartMoveUp ();
				knockComponent.StartRotateDomino ();
				lastDominoIndex = currIndex - numOfActiveDomi  -2;
			
				return;
			}

			dominoTransforms[currIndex].SetParent(null);						//remove the domino from its parent
			SaveOriginalPostion();												//Save the domino position before moving it
			isInteraciveDomino = IsActiveCube();								//Call a random function to decide whether a domino is active
			dominoes[currIndex - 2].StartMoveUp(isInteraciveDomino);			//Move the domino up	

			if(isInteraciveDomino)
			{
				collideCheck.position = dominoTransforms[currIndex].position;	
				collideCheck.rotation = dominoTransforms[currIndex].rotation;

				CancelInvoke();													//Stop Placing domino
			}

//			dominoTransforms[currIndex - numOfActiveDomi].parent = dominoHolderList[holderIndex].transform;										//add a domino to a parent
//			meshControlList[holderIndex].meshFilters.Add(dominoTransforms[currIndex - numOfActiveDomi].GetComponent<MeshFilter>());			//add a Meshfiler to a MeshFilterList to combine
			meshControlList[holderIndex].meshFilters[1] = dominoTransforms[currIndex - numOfActiveDomi].GetComponent<MeshFilter>();

			meshControlList[holderIndex].Combine();													
			dominoTransforms[currIndex - numOfActiveDomi].gameObject.SetActive(false);

			currIndex++;

			//Creat a new domino holder after every 50 dominos
			if((currIndex - numOfActiveDomi - 2)  % holderAmount == 0 )
			{
				CreateNewHolder();
			}
		}
		else
		{
			lastDominoIndex = currIndex - numOfActiveDomi - 2;

			//if the domino falls backward, it would knock down the previous which is dominoTransforms[curr - 2]
			if (dominoTransforms[currIndex - 2].eulerAngles.x < 15 || dominoTransforms[currIndex - 2].eulerAngles.x > 345 ) {
				MoveTriggerCheck();
				Knock ();														
				CancelInvoke ();
				knockComponent.StartMoveUp ();
				knockComponent.StartRotateDomino ();
			}
		}
	}

	//This method will activate a knock domino
	void Knock()
	{	
		knockDomino.gameObject.SetActive(true);
		dir =  (PosB - PosA).normalized;
		knockDomino.position = PosA + dir * 2 ;
//		Debug.DrawLine(PosA, PosB + dir , Color.red, 60f);
	}


	//Add a transform component to active domino list
	void AddActiveDomino(Transform trans)
	{
		trans.gameObject.AddComponent<BoxCollider>();
		trans.gameObject.AddComponent<Rigidbody>();
		activeDominoes.Add(trans);
	}

	//Save postions for finding a knock domino's direction. These are the last two active dominos in a list
	void SaveOriginalPostion()
	{
		PosA = PosB;
		PosB = dominoTransforms[currIndex].position;
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

	//A trigger check to keep tracking of the current active dominoes
	void MoveTriggerCheck()
	{
//		Debug.Log(currIndex + " current index");
		triggerCheck.position = dominoTransforms[currIndex - offsetDomiIndex].position;
		triggerCheck.rotation = dominoTransforms[currIndex - offsetDomiIndex].rotation;

	}

	//A trigger Check to know if the active dominoes fall backward
	void MoveGameOverCheck()
	{
		gameoverCheck.position = dominoTransforms[currIndex - 2].position;
		gameoverCheck.rotation = dominoTransforms[currIndex - 2].rotation;
	}

	void SetGameOver()
	{
		isGameOver = true;
		lastDominoIndex = currIndex - numOfActiveDomi - 2;
		MoveTriggerCheck();
	
	}

	//Will be called from a trigger event in OnTriggerEnter() on TriggerCheck
	void PreviousDomino()
	{
		CombineFallenDomino();
		MoveTriggerCheck();
	}

	void CombineFallenDomino()
	{
//		dominoTransforms[currIndex - 1].parent = fallenHolderList[fallenHolderIndex].transform;
		fallenMeshControlList[fallenHolderIndex].meshFilters[1] = dominoTransforms[currIndex - 1].GetComponent<MeshFilter>();

		fallenMeshControlList[fallenHolderIndex].Combine();													
		dominoTransforms[currIndex - 1].gameObject.SetActive(false);

		Decombine();
		currIndex--;
		fallenAmount++;

		if(fallenAmount % holderAmount == 0)
		{
			CreateNewFallenHolder();
		}
	}

	void Decombine()
	{
		if (holderIndex >= 0) {

//			dominoTransforms[currIndex - numOfActiveDomi - 1].SetParent(null);
			dominoTransforms[currIndex - numOfActiveDomi - 1].gameObject.SetActive(true);

			meshControlList [holderIndex].RemoveLastSubmesh ();
			lastDominoIndex--;
			if (lastDominoIndex % 100 == 0) {
				holderIndex--;
			}
		}

	}

}
