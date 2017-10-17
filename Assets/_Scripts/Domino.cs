using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Domino : MonoBehaviour {

	public delegate void ActionMoveGameOverCheck();
	public static event ActionMoveGameOverCheck GameOverCheck;

	private Control control;

	private Transform oriTrans;


	private IEnumerator corouMoveUp;
	private IEnumerator corouRotate;

	private string activeDominoTag = "ActiveDomino";
	private string fireButton = "Fire1";
	private bool isRotating = false;

	private Transform domiTrans;				//Store the transfrom component of the domino
	private float distanceTomove;				//Distance that a domino have to travel
	private float length;						//Distance that a domino have to travel
	private float minLength = 1f;				//1 unit to move
	private float maxLength = 3f;				//3 unit to move	
	private float speed;						//speed of the domino
	private float maxSpeed = 1f;
	private float minSpeed = .5f;	

//	private float maxSpeed = 5f;				//TEST
//	private float minSpeed = 3f;
	private Rigidbody rb;						//Rigibody of the domino

	private float localX;
	private float localY;
	private float localZ;
	private Vector3 tempPos;					//Store a position of the domino before moving it up
	private Vector3 prePos;						//Store a position of te domino before moving it down;

	void Awake()
	{
		domiTrans = GetComponent<Transform>();	
		rb = GetComponent<Rigidbody>();
		tempPos = domiTrans.position;
		corouRotate = RotateDomino();
		control = GameObject.FindWithTag("Control").GetComponent<Control>();
	}
		

	public void StartMoveUp(bool activeDomino)
	{
		//if it is an active domino, move domino up max length. Otherwise min length
		if(!activeDomino)
		{
			length = minLength;
			speed = minSpeed;

		}else{
			length = maxLength;
			speed = maxSpeed;
		}

		corouMoveUp = MoveUp(length);
		StartCoroutine(corouMoveUp);								//Start move up

	}

	//A coroutin method to move an domino up
	private IEnumerator MoveUp(float distance)
	{
//	  	Debug.Log("Inside the MoveUp function");
		 localX = domiTrans.localPosition.x;
		 localY = domiTrans.localPosition.y;
		 localZ = domiTrans.localPosition.z;
	
		while(distanceTomove < distance)
		{
//			
			distanceTomove += Time.deltaTime * speed;
			if(distanceTomove > distance)
			{
				distanceTomove = distance;
			}

			tempPos.y = localY + distanceTomove;
			domiTrans.localPosition = tempPos;
//
//			Debug.Log("Collider Check Postion " + control.collideCheck.position);

			yield return null;
		}

		StopCoroutine(corouMoveUp);

		//if distance is equal to max length, it is an active domino. 
		if(distance == maxLength)
		{
			gameObject.tag = activeDominoTag;					//Change the tag of an domino to ActiveDomino
			StartCoroutine(RotateDomino());						//Call RatateDomino() to rotate the domino. Will read input every frame in this method
		}else{
			SetupDomino();										//This function will Add physic components to a domino
		}

		yield break;
	}


	//if called, this method will be executed every frame.
	private IEnumerator RotateDomino()
	{

		isRotating = true;
		if(GameOverCheck != null)
		{
			GameOverCheck();
		}

		while(isRotating)
		{
//			Debug.Log(domiTrans.localRotation.eulerAngles.x);
			domiTrans.Rotate(Vector3.right, Space.Self);		//Rotate the domino around x axis

			if(Input.GetButtonDown(fireButton))					//if Fire1 is pressed
			{
				rb.isKinematic = false;
				SetupDomino();	

			}

			yield return null;
		}


	}

	public void StartMoveDown(Transform preTrans)
	{
		StartCoroutine(MoveDown(preTrans));
	}

	IEnumerator MoveDown(Transform preTrans)
	{
//		Debug.Log("Inside the MoveDown Function");
		localX = domiTrans.localPosition.x;
		localY = domiTrans.localPosition.y;
		localZ = domiTrans.localPosition.z;
		rb.isKinematic = true;
//		tempPos = preTrans.position;
		prePos = domiTrans.position;
		distanceTomove = 0f;
		while(distanceTomove < 1f)
		{
			distanceTomove += Time.deltaTime * (speed - .7f);
			if(distanceTomove > 1f)
			{
				distanceTomove = 1f;
			}

			prePos.y = localY - distanceTomove;
			domiTrans.localPosition = prePos;

//			Debug.Log("Original Transfrom " + control.origianlTrans.position);

			yield return null;
		}
	
//		Debug.Log("Reset Position here");
		domiTrans.position = control.originalTrans.position;
		domiTrans.rotation = control.originalTrans.rotation;

		StartCoroutine(MoveUp(maxLength));
	}

	//Add physic components to a domino
	private void SetupDomino()
	{
		isRotating = false;
		StopAllCoroutines();
//		gameObject.AddComponent<BoxCollider>();
//		gameObject.AddComponent<Rigidbody>();
//

//		rb.isKinematic = false;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}
}
