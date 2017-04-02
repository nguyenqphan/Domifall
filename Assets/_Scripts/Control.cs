using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour {

	//NEED TO BE DELETED
	private bool test = false;

	private Transform collideCheck;				//if collision detected, then continue to place dominos
	private Transform[] dominoTransforms;		//Store the transform component of dominoes
	private Domino[] dominoes;					//Store the dominoes component
	private List<Transform> activeDominoes;		//Store active dominoes that have rigibody and box collider
	private int numOfActiveDomi = 15;			//The number of active dominoes
	private int currIndex = 2;					//the current indext of an active domino
	private int randomTarget = 1;				//a target for a random number to hit
	private bool isInteraciveDomino = false;	//Is the current domino interactive?

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
	}

	// Use this for initialization
	void Start () {
		dominoTransforms = GetComponentsInChildren<Transform>();		
		dominoes = GetComponentsInChildren<Domino>();
		dominoTransforms[1].gameObject.SetActive(false);						//Set the parent of all dominoes inactive
		activeDominoes = new List<Transform>();

		InvokeRepeating("Layout", 1f, 0.1f);
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

	void InvokeRepeatingDomino()
	{
		InvokeRepeating("PlaceDomino", 1f, 0.1f);
	}

	void Layout()
	{
		dominoTransforms[currIndex].SetParent(null);							//remove the domino from its parent
		dominoes[currIndex - 2].StartMoveUp(isInteraciveDomino);				//Move it up (CurrIndex - 2 becuase the first to gameobjects dont have Domino component))
		activeDominoes.Add(dominoTransforms[currIndex]);
		currIndex++;

		if(currIndex > numOfActiveDomi)
		{
			CancelInvoke();
		}
		
	}

	private bool IsActiveCube()
	{
		return (currIndex % 2 == 0) && randomTarget == Random.Range(1, 4);

	}

	void PlaceDomino()
	{
		dominoTransforms[currIndex].SetParent(null);
		isInteraciveDomino = IsActiveCube();
		dominoes[currIndex - 2].StartMoveUp(isInteraciveDomino);

		if(isInteraciveDomino)
		{
			collideCheck.position = dominoTransforms[currIndex].position;
			CancelInvoke();
		}

		currIndex++;
	}

}
