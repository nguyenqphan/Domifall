using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

	//Varible declarations
	#region

	public Color32[] gameColors = new Color32[4];
	private GameManager gameManager;
	private ColorManager colorManager;

	private const int holderAmount = 100;			//The number of dominos will combine into a mesh for better performance
	private const int numOfActiveDomi = 10;			//The number of active dominoes
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
	private Transform collideCheck;				//if collision detected, then continue to place dominos
	private Transform knockDomino;				//Store the Transform of a game object that is used to initially knock down a domino	

	[HideInInspector]
	public Transform[] dominoTransforms;		//Store the transform component of dominoes. Make it public so the Knocker component can have access to the first Domino for direction
	private Domino[] dominoes;					//Store the dominoes component

	private Knocker knockComponent;				//reference to Knocker script
	private CameraMove cameraMove;

	private List<Transform> activeDominoes;		//Store active dominoes that have rigibody and box collider

	private int dominoIndex = 2;

	private int currIndex = 2;					//the current index of an active domino. Starting with 2 because the first two game objects are parents of dominoes
	private int randomTarget = 1;				//a target for a random number to hit
	private bool isInteraciveDomino = false;	//Is the current domino interactive?
	private float preAngleX;					//Cache the rotation of x of previous dominos

	private Vector3 PosA;						//Cache the position of a domino before moving it
	private Vector3 PosB;						//Cache the position of a domino before moving it
	[HideInInspector]	
	public Vector3 dir;							//A direction for a knock domino (PosB - PosA)

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
		collideCheck = GameObject.FindWithTag("CollideCheck").GetComponent<Transform>();		
		knockDomino = GameObject.FindWithTag("KnockDomino").GetComponent<Transform>();
		gameoverCheck = GameObject.FindWithTag("GameOverCheck").GetComponent<Transform>();
		triggerCheck = GameObject.FindWithTag("TriggerCheck").GetComponent<Transform>();
		knockComponent = GameObject.FindWithTag("KnockDomino").GetComponent<Knocker>();
		cameraMove = GameObject.FindWithTag("CameraHolder").GetComponent<CameraMove>();
		gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
		colorManager = GameObject.FindWithTag("ColorManager").GetComponent<ColorManager>();
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

	private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t)
	{
		if(t < 0.33f)
		{
			return Color.Lerp(a,b,t/0.33f);
		}else if(t < 0.66f)
		{
			return Color.Lerp(b, c, (t -0.33f)/ 0.33f);
		}else{
			return Color.Lerp(c, d, (t-0.66f) /0.66f);
		}
	}

	private void ColorMesh(Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin(currIndex * 0.25f);

		for(int i =0; i < vertices.Length; i++)
		{
			colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
		}

		mesh.colors32 = colors;
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
//		return (currIndex % 2 == 0) && randomTarget == Random.Range(1, 100);
		return currIndex % 400 == 0;
	}

	//a method to place a domino
	void PlaceDomino()
	{
		if(isGameOver) {return;}											//if isGameOver is true, no need to do the rest

		MoveCamera();
	
			//Reached the last domino
			if(currIndex > dominoTransforms.Length  - 1)
			{
				gameManager.win = true;
				StartCoroutine(CombineAndDecombine());
				ActivateKnockDomino();
				lastDominoIndex = currIndex - numOfActiveDomi  -2;
			
				return;
			}

			dominoTransforms[currIndex].SetParent(null);						//remove the domino from its parent
//			ColorMesh(dominoTransforms[currIndex].GetComponent<MeshFilter>().mesh);
			colorManager.ColorMesh(dominoTransforms[currIndex].GetComponent<MeshFilter>().mesh, currIndex);

			isInteraciveDomino = IsActiveCube();								//Call a random function to decide whether a domino is active
			dominoes[currIndex - 2].transform.position = new Vector3(dominoes[currIndex - 2].transform.position.x, dominoes[currIndex - 2].transform.position.y + -1f, dominoes[currIndex - 2].transform.position.z );
			dominoes[currIndex - 2].StartMoveUp(isInteraciveDomino);			//Move the domino up	

			if(isInteraciveDomino)
			{
				collideCheck.position = dominoTransforms[currIndex].position;	
				collideCheck.rotation = dominoTransforms[currIndex].rotation;

				CancelInvoke();													//Stop Placing domino
			}
				
//			meshControlList[holderIndex].meshFilters[1] = dominoTransforms[currIndex - numOfActiveDomi].GetComponent<MeshFilter>(); //OLD

			
		meshControlList[holderIndex].meshFilters[1] = dominoTransforms[ currIndex - numOfActiveDomi].GetComponent<MeshFilter>();	//get the previous domino meshFilter to combine

			meshControlList[holderIndex].Combine();													
		dominoTransforms[ currIndex - numOfActiveDomi].gameObject.SetActive(false);

			currIndex++;

			//Creat a new domino holder after every 50 dominos
			if((currIndex - numOfActiveDomi - 2)  % holderAmount == 0 )
			{
				CreateNewHolder();
			}
		
	
	}
		

	private int remainingDominos = 0;
	private	IEnumerator CombineAndDecombine()
	{
		yield return new WaitForSeconds(2f);
		while(currIndex > currIndex - numOfActiveDomi + remainingDominos){
			
			meshControlList[holderIndex].meshFilters[1] = dominoTransforms[ currIndex - numOfActiveDomi + remainingDominos].GetComponent<MeshFilter>();	//get the previous domino meshFilter to combine

			meshControlList[holderIndex].Combine();													
			dominoTransforms[currIndex - numOfActiveDomi + remainingDominos].gameObject.SetActive(false);
			Decombine();

			remainingDominos++;
			yield return null;
		}

	}

	//Add a transform component to active domino list
	void AddActiveDomino(Transform trans)
	{
		trans.gameObject.AddComponent<BoxCollider>();
		trans.gameObject.AddComponent<Rigidbody>();
		activeDominoes.Add(trans);
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
		lastDominoIndex = currIndex - numOfActiveDomi - 2;
		MoveTriggerCheck();
	
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
		MoveCamera();

		if(dominoIndex > lastDominoIndex)
		{
			return;
		}

		dominoIndex++;
		currIndex--;
		fallenAmount++;



		if(fallenAmount % holderAmount == 0)
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

			meshControlList [tempHoderIndex].RemoveFirstSubmesh ();
			headDominoIndex++;
			if ((headDominoIndex - 2) % 100 == 0) {
				tempHoderIndex++;

			}
		}

	}

	void MoveCamera()
	{
		if(dominoTransforms[currIndex - 1].gameObject.CompareTag("CamPos") && !isGameOver)
		{
			cameraMove.MoveToTarget(dominoTransforms[currIndex]);
			return;
		}

		if(isGameOver && isCircleTowerRound && !isCircleBackCalled)
		{
			isCircleBackCalled = true;
			cameraMove.CircleBackToTop(lastDominoIndex/4);
		}
		else
		if(currIndex % 20 == 0)
		{
				if(isCircleBackCalled)
					return;
				if(isCircleTowerRound && currIndex % 20 == 0)
					cameraMove.CircleAroundTarget();
				else
					if(!isCircleTowerRound){
						if(!isGameOver){
							cameraMove.MoveToTarget(dominoTransforms[currIndex]);
						}else{
							if(!dominoTransforms[currIndex].gameObject.CompareTag("NoCamPos")){
								Debug.Log(dominoTransforms[currIndex].name);
								cameraMove.MoveToTarget(dominoTransforms[currIndex]);
							}
						}

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
