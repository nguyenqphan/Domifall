using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Domino : MonoBehaviour {

	public delegate void ActionMoveGameOverCheck();
	public static event ActionMoveGameOverCheck GameOverCheck;

	private IEnumerator corouMoveUp;
	private IEnumerator corouRotate;

	private string activeDominoTag = "ActiveDomino";
	private string fireButton = "Fire1";

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
	private Vector3 tempPos;					//Store a position of the domino

	void Awake()
	{
		domiTrans = GetComponent<Transform>();	
		tempPos = domiTrans.position;
		corouRotate = RotateDomino();
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
		 localX = domiTrans.localPosition.x;
		 localY = domiTrans.localPosition.y;
		 localZ = domiTrans.localPosition.z;

		while(distanceTomove < distance)
		{
			distanceTomove += Time.deltaTime * speed;
			if(distanceTomove > distance)
			{
				distanceTomove = distance;
			}

			tempPos.y = localY + distanceTomove;
			domiTrans.localPosition = tempPos;

			yield return null;
		}

		StopCoroutine(corouMoveUp);

		//if distance is equal to max length, it is an active domino. 
		if(distance == maxLength)
		{
			gameObject.tag = activeDominoTag;					//Change the tag of an domino to ActiveDomino
			StartCoroutine(corouRotate);						//Call RatateDomino() to rotate the domino. Will read input every frame in this method
		}else{
			SetupDomino();										//This function will Add physic components to a domino
		}
	}


	//if called, this method will be executed every frame.
	private IEnumerator RotateDomino()
	{
		if(GameOverCheck != null)
		{
			GameOverCheck();
		}

		while(true)
		{
//			Debug.Log(domiTrans.localRotation.eulerAngles.x);
//			domiTrans.Rotate(Vector3.right, Space.Self);		//Rotate the domino around x axis

			if(Input.GetButtonDown(fireButton))					//if Fire1 is pressed
			{
				SetupDomino();									
			}

			yield return null;
		}


	}

	//Add physic components to a domino
	private void SetupDomino()
	{
		StopAllCoroutines();
		gameObject.AddComponent<BoxCollider>();
		gameObject.AddComponent<Rigidbody>();

		rb = GetComponent<Rigidbody>();
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}
}
