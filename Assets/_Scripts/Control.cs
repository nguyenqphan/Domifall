using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

	//NEED TO BE DELETED
	private bool test = false;

	private Transform collideCheck;				//if collision detected, then continue to place dominos
	private Transform knockDomino;				//Store the Transform of a game object that is used to initially knock down a domino	
	private Transform[] dominoTransforms;		//Store the transform component of dominoes
	private Domino[] dominoes;					//Store the dominoes component

	private Knocker knockComponent;

	private List<Transform> activeDominoes;		//Store active dominoes that have rigibody and box collider


	private int numOfActiveDomi = 15;			//The number of active dominoes
	private int currIndex = 2;					//the current index of an active domino
	private int randomTarget = 1;				//a target for a random number to hit
	private bool isInteraciveDomino = false;	//Is the current domino interactive?
	private float preAngleX;					//Cache the rotation of x of previous dominos

	[HideInInspector]
	public Vector3 dir;
	private Vector3 PosA;
	private Vector3 PosB;

	void OnEnable()
	{
		CollideCheck.Colliding += InvokeRepeatingDomino;										//Register an event
	}

	void OnDisable()
	{
		CollideCheck.Colliding -= InvokeRepeatingDomino;										//Unregister an event
	}


	void Awake()
	{
		collideCheck = GameObject.FindWithTag("CollideCheck").GetComponent<Transform>();		
		knockDomino = GameObject.FindWithTag("KnockDomino").GetComponent<Transform>();
		knockComponent = GameObject.FindWithTag("KnockDomino").GetComponent<Knocker>();
	}

	// Use this for initialization
	void Start () {
		dominoTransforms = GetComponentsInChildren<Transform>();				//get all Transform components of the dominoes
		dominoes = GetComponentsInChildren<Domino>();							//get all Domino components of the dominoes
		dominoTransforms[1].gameObject.SetActive(false);						//Set the parent of all dominoes inactive
		activeDominoes = new List<Transform>();
		InvokeRepeating("Layout", 1f, 0.3f);

//		Debug.DrawLine(Vector3.zero, new Vector3(0, 10f, 0), Color.red, 10f);
	}
	
	// Update is called once per frame
	void Update () {

	


		if(Input.GetButtonDown("Fire1"))
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
		InvokeRepeating("PlaceDomino", 2f, 0.3f);
	}

	void Layout()
	{
		dominoTransforms[currIndex].SetParent(null);							//remove the domino from its parent
		dominoes[currIndex - 2].StartMoveUp(isInteraciveDomino);				//Move it up (CurrIndex - 2 becuase the first to gameobjects dont have Domino component))
		activeDominoes.Add(dominoTransforms[currIndex]);


//		AddActiveDomino(dominoTransforms[currIndex]);							//Add the domino to active domino list
		currIndex++;

		if(currIndex > numOfActiveDomi)
		{
			PosB = dominoTransforms[currIndex - 1].position;
			CancelInvoke();														//Stop placing dominos after it meets the certain condition
		}
		
	}

	//Rondom method to decide if a domino should be interactive
	private bool IsActiveCube()
	{
		return (currIndex % 2 == 0) && randomTarget == Random.Range(1, 6);
	}

	//a method to place a domino
	void PlaceDomino()
	{
		preAngleX = dominoTransforms[currIndex - 1].eulerAngles.x;
		if(preAngleX < 15 || preAngleX > 345)
		{
			dominoTransforms[currIndex].SetParent(null);
			SaveOriginalPostion();
			isInteraciveDomino = IsActiveCube();
			dominoes[currIndex - 2].StartMoveUp(isInteraciveDomino);

			if(isInteraciveDomino)
			{
				collideCheck.position = dominoTransforms[currIndex].position;
				collideCheck.rotation = dominoTransforms[currIndex].rotation;
				CancelInvoke();
			}

			currIndex++;
		}
		else
		{
			if (dominoTransforms[currIndex - 2].eulerAngles.x < 15 || dominoTransforms[currIndex - 2].eulerAngles.x > 345) {
				Knock ();	
				CancelInvoke ();
				knockComponent.StartMoveUp ();
				knockComponent.StartRotateDomino ();
			}else{
				
			}
		}
	}

	void Knock()
	{	
		knockDomino.gameObject.SetActive(true);
		dir =  (PosB - PosA).normalized;
		knockDomino.position = PosA + dir * 2 ;
		Debug.DrawLine(PosA, PosB + dir , Color.red, 60f);
	}


	//Add a transform component to active domino list
	void AddActiveDomino(Transform trans)
	{
		trans.gameObject.AddComponent<BoxCollider>();
		trans.gameObject.AddComponent<Rigidbody>();
		activeDominoes.Add(trans);
	}


	void SaveOriginalPostion()
	{
		PosA = PosB;
		PosB = dominoTransforms[currIndex].position;
	}


}
