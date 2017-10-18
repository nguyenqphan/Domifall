using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

	//Varible declarations
	#region
	[HideInInspector]
	public Transform originalTrans;					//Store the original position and rotation of active domino before moving it up
	private CamPosition camPosition;

	private int numOfDominoes;
	private GameManager gameManager;
	private ColorManager colorManager;

	private const int HOLDERAMOUNT = 100;			//The number of dominos will combine into a mesh for better performance
	private const int NUMOFACTIVEDOMINO = 10;			//The number of active dominoes
	private List<MeshControl> meshControlList;		//Store MeshControl scripts as a List

	private string placeDomino = "PlaceDomino";
	private string layout = "Layout";
	private string fire1Button = "Fire1";

	public bool isCircleTowerRound = false;
	private bool isCircleBackCalled = false;

	private int lastDominoIndex = 0;				//Store the last domino index

	private  int fallenAmount = 0;					//Store the number of dominos has fallen and need to combine into a mesh
	private int offsetDomiIndex = 7;				//The offset index used to place the triggerCheck 

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
	public Transform collideCheck;				//if collision detected, then continue to place dominos
	private Transform knockDomino;				//Store the Transform of a game object that is used to initially knock down a domino	

	[HideInInspector]
	public Transform[] dominoTransforms;		//Store the transform component of dominoes. Make it public so the Knocker component can have access to the first Domino for direction
	private Domino[] dominoes;					//Store the dominoes component

	private Knocker knockComponent;				//reference to Knocker script
	private CameraMove cameraMove;

	private int dominoIndex = 2;

	private int currIndex = 2;					//the current index of an active domino. Starting with 2 because the first two game objects are parents of dominoes
	private int randomTarget = 1;				//a target for a random number to hit
	private bool isInteraciveDomino = false;	//Is the current domino interactive?
	private float preAngleX;					//Cache the rotation of x of previous dominos

	private Vector3 PosA;						//Cache the position of a domino before moving it
	private Vector3 PosB;						//Cache the position of a domino before moving it
	[HideInInspector]	
	public Vector3 dir;							//A direction for a knock domino (PosB - PosA)

	private Vector3 previousDominoPos;
	private Quaternion previousDominoRotation;
	private Transform previousDominoTrans;

	private IEnumerator safeCheckCoroutine;
	private WaitForSeconds waitTime = new WaitForSeconds(2f);

	#endregion 
	//Register events
	void OnEnable()
	{
		CollideCheck.Colliding  += InvokeRepeatingDomino;										//Register an event
		TriggerCheck.Triggering += NextDomino;
		Domino.GameOverCheck += MoveGameOverCheck;
		GameOverCheck.CallGameOver += SetGameOver;

	}

	//remove regitered events
	void OnDisable()
	{
		CollideCheck.Colliding  -= InvokeRepeatingDomino;										//Unregister an event
		TriggerCheck.Triggering -= NextDomino;
		Domino.GameOverCheck -= MoveGameOverCheck;
		GameOverCheck.CallGameOver -= SetGameOver;

	}


	void Awake()
	{
		originalTrans = GameObject.FindWithTag("OriginalTrans").GetComponent<Transform>();
		collideCheck = GameObject.FindWithTag("CollideCheck").GetComponent<Transform>();		
		knockDomino = GameObject.FindWithTag("KnockDomino").GetComponent<Transform>();
		gameoverCheck = GameObject.FindWithTag("GameOverCheck").GetComponent<Transform>();
		triggerCheck = GameObject.FindWithTag("TriggerCheck").GetComponent<Transform>();
		knockComponent = GameObject.FindWithTag("KnockDomino").GetComponent<Knocker>();
		cameraMove = GameObject.FindWithTag("CameraHolder").GetComponent<CameraMove>();
		gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
		colorManager = GameObject.FindWithTag("ColorManager").GetComponent<ColorManager>();
		camPosition = GameObject.FindWithTag("CamPositions").GetComponent<CamPosition>();
//		cameraTrigger = GameObject.FindWithTag("CameraTrigger").GetComponent<CameraTrigger>();


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
		colorManager.ColorMesh(dominoTransforms[currIndex].GetComponent<MeshFilter>().mesh, currIndex);

		dominoes[currIndex - 2].transform.position = new Vector3(dominoes[currIndex - 2].transform.position.x, dominoes[currIndex - 2].transform.position.y + -1f, dominoes[currIndex - 2].transform.position.z );

		dominoes[currIndex - 2].StartMoveUp(isInteraciveDomino);				//Move it up (CurrIndex - 2 becuase the first to gameobjects dont have Domino component))

		currIndex++;

		if(currIndex  - 1 > NUMOFACTIVEDOMINO)									//Subtract 1 becuase becuase currIndex startd with 2 so the number of active dominos can be flexibly change for testing.
		{
			PosB = dominoTransforms[currIndex - 1].position;
			CancelInvoke(layout);														//Stop placing dominos after it meets a certain condition
		}
	}

	//Rondom method to decide if a domino should be interactive
	private bool IsActiveCube()
	{
//		return (currIndex % 2 == 0) && randomTarget == Random.Range(1, 100);
		return currIndex % 300 == 0;
	}

	//a method to place a domino
	void PlaceDomino()
	{
		if(isGameOver) {return;}											//if isGameOver is true, no need to do the rest

		MoveCamera();
	
		preAngleX = dominoTransforms[currIndex - 1].eulerAngles.x;
//		Debug.Log(dominoTransforms[currIndex - 1].name + "NAME");
			
		//A condition to figure out whether or not a domino is falled
		if (preAngleX < 15 || preAngleX > 345) {
			//Reached the last domino
			if (currIndex > dominoTransforms.Length - 1) {
				Debug.Log("Win, Reach the end of the domino length");
				gameManager.win = true;
				StartCoroutine (CombineAndDecombine ());
				ActivateKnockDomino ();
				lastDominoIndex = currIndex - NUMOFACTIVEDOMINO - 2;
				numOfDominoes = currIndex - 2;
				
				return;
			}

			previousDominoPos = dominoTransforms[currIndex - 2].position;			//Save position of the original domino before starting to move it up
			previousDominoRotation = dominoTransforms[currIndex - 2].rotation;		//save rotaion of the original domino before starting to move it up and rotate it.


//			Debug.Log(previousDominoTrans.position);
//			Debug.Log(previousDominoTrans.rotation);
//
			dominoTransforms [currIndex].SetParent (null);						//remove the domino from its parent

			//ColorMesh(dominoTransforms[currIndex].GetComponent<MeshFilter>().mesh);
			colorManager.ColorMesh (dominoTransforms [currIndex].GetComponent<MeshFilter> ().mesh, currIndex);
				
			isInteraciveDomino = IsActiveCube ();								//Call a random function to decide whether a domino is active

//			previousDominoTrans = dominoes [currIndex - 2].transform;

			dominoes [currIndex - 2].transform.position = new Vector3 (dominoes [currIndex - 2].transform.position.x, dominoes [currIndex - 2].transform.position.y + -1f, dominoes [currIndex - 2].transform.position.z);
			dominoes [currIndex - 2].StartMoveUp (isInteraciveDomino);			//Move the domino up	
				
			if (isInteraciveDomino) {
				dominoTransforms[currIndex - 1].GetComponent<Rigidbody>().isKinematic = true;
				collideCheck.position = dominoTransforms [currIndex].position;	
				collideCheck.rotation = dominoTransforms [currIndex].rotation;
//				Debug.Log("Inside the condition isInteractiveDomino");
				originalTrans.position = dominoTransforms[currIndex].position;
				originalTrans.rotation = dominoTransforms[currIndex].rotation;
//				origianlTrans = dominoTransforms[currIndex];
				
				CancelInvoke ();													//Stop Placing domino
			}
					
			//			meshControlList[holderIndex].meshFilters[1] = dominoTransforms[currIndex - numOfActiveDomi].GetComponent<MeshFilter>(); //OLD
				
				
			meshControlList [holderIndex].meshFilters [1] = dominoTransforms [currIndex - NUMOFACTIVEDOMINO].GetComponent<MeshFilter> ();	//get the previous domino meshFilter to combine
				
			meshControlList [holderIndex].Combine ();													
			dominoTransforms [currIndex - NUMOFACTIVEDOMINO].gameObject.SetActive (false);
				
			currIndex++;
				
			//Creat a new domino holder after every HOLDERAMOUNT dominos
			if ((currIndex - NUMOFACTIVEDOMINO - 2) % HOLDERAMOUNT == 0) {
				CreateNewHolder ();
			}
		}else{
			CancelInvoke ();
//			currIndex--;													//Go back to previous domino Index\
//			Destroy(dominoTransforms[currIndex].GetComponent<BoxCollider>());
//			Destroy(dominoTransforms[currIndex].GetComponent<Rigidbody>());
//			Debug.Log(previousDominoTrans.position + " Control Script");
//			Debug.Log(previousDominoTrans.rotation + " ControlScript");
			dominoes[currIndex - 3].StartMoveDown(originalTrans);					//currIndex - 3, not - 2 becuase of failing to place domino
//			Debug.Log("Inside the else the previous domino is fell");
//			dominoTransforms[currIndex].position = previousDominoPos;
//			dominoTransforms[currIndex].rotation = previousDominoRotation;

		}
		
	
	}
		

	private int remainingDominos = 0;
	private	IEnumerator CombineAndDecombine()
	{
		yield return new WaitForSeconds(2f);
		while(currIndex > currIndex - NUMOFACTIVEDOMINO + remainingDominos){
			
			meshControlList[holderIndex].meshFilters[1] = dominoTransforms[ currIndex - NUMOFACTIVEDOMINO + remainingDominos].GetComponent<MeshFilter>();	//get the previous domino meshFilter to combine

			meshControlList[holderIndex].Combine();													
			dominoTransforms[currIndex - NUMOFACTIVEDOMINO + remainingDominos].gameObject.SetActive(false);
		
			Decombine();

			remainingDominos++;
			yield return null;
		}

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

	//OnTriggerEnter this TriggerCheck will trigger NextDomino() which executes CombineFallDomino() and Decombine()
	void MoveTriggerCheck()
	{
		triggerCheck.position = dominoTransforms[dominoIndex + offsetDomiIndex].position;
		triggerCheck.rotation = dominoTransforms[dominoIndex + offsetDomiIndex].rotation;

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
		lastDominoIndex = currIndex - NUMOFACTIVEDOMINO - 2;
		MoveTriggerCheck();
//		colorManager.MakeDominoDisapear(dominoTransforms[currIndex].GetComponent<MeshFilter>().mesh);
	
	}

	//Will be called from a trigger event in OnTriggerEnter() on TriggerCheck
	void NextDomino()
	{
		CombineFallenDomino();
		MoveTriggerCheck();
	}



	void CombineFallenDomino()
	{
		fallenMeshControlList[fallenHolderIndex].meshFilters[1] = dominoTransforms[dominoIndex].GetComponent<MeshFilter>();

		fallenMeshControlList[fallenHolderIndex].Combine();													
		dominoTransforms[dominoIndex].gameObject.SetActive(false);

		Decombine();
//		MoveCamera();

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

	private int headDominoIndex = 2;
	private int tempHoderIndex = 0;
	void Decombine()
	{

		if (tempHoderIndex <= holderIndex) {

			dominoTransforms[headDominoIndex].gameObject.SetActive(true);
			MoveCamera();
			dominoTransforms[headDominoIndex].GetComponent<Rigidbody>().isKinematic = false;


			meshControlList [tempHoderIndex].RemoveFirstSubmesh ();
			headDominoIndex++;
			if ((headDominoIndex - 2) % 100 == 0) {
				tempHoderIndex++;

			}

			if(headDominoIndex == numOfDominoes + 2){
				cameraMove.MoveCamToLastPosition();
			}
				
		}

	}

//	void MoveCamera()
//	{
//		if(dominoTransforms[currIndex - 1].gameObject.CompareTag("CamPos") && !isGameOver)
//		{
//			cameraMove.MoveToTarget(dominoTransforms[currIndex]);
//			return;
//		}
//
//		if(isGameOver && isCircleTowerRound && !isCircleBackCalled)
//		{
//			isCircleBackCalled = true;
//			cameraMove.CircleBackToTop(lastDominoIndex/4);
//		}
//		else
//		if(currIndex % 20 == 0)
//		{
//				if(isCircleBackCalled)
//					return;
//				if(isCircleTowerRound && currIndex % 20 == 0)
//					cameraMove.CircleAroundTarget();
//				else
//					if(!isCircleTowerRound){
//						if(!isGameOver){
//							cameraMove.MoveToTarget(dominoTransforms[currIndex]);
//						}else{
//							if(!dominoTransforms[currIndex].gameObject.CompareTag("NoCamPos")){
////								Debug.Log(dominoTransforms[currIndex].name);
//								cameraMove.MoveToTarget(dominoTransforms[currIndex]);
//							}
//						}
//
//					}
//		}
//	}

	private int camPosIndex = 0;
	void MoveCamera(){
		
		if (!gameManager.win) {
			if (dominoTransforms [currIndex - 1].gameObject.CompareTag ("CamPos")) {
				
				camPosIndex++;
//				Debug.Log (camPosIndex + " CamPosIndex");
				cameraMove.MoveToTarget (camPosition.transArray [camPosIndex]);
			}
		}else{
			if(dominoTransforms[headDominoIndex - 1].gameObject.CompareTag("CamPositionWin"))
			{
				Debug.Log("Move Camera here................................");
				camPosIndex++;
//				Debug.Log (camPosIndex + " CamPosIndex");
				cameraMove.MoveToTarget (camPosition.transArray [camPosIndex]);
			}
		}
	}

	void ActivateKnockDomino()
	{
		isGameOver = true;
		MoveTriggerCheck();
		knockDomino.gameObject.SetActive(true);														
		CancelInvoke ();
		knockComponent.StartMoveUp ();
		knockComponent.StartRotateDomino ();
		
	}

}
