using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Domino : MonoBehaviour {

	private Transform domiTrans;				//Store the transfrom component of the domino
	private float distanceTomove;				//Distance that a domino have to travel
	private float length;						//Distance that a domino have to travel
	private float minLength = 1f;				//1 unit to move
	private float maxLength = 3f;				//3 unit to move	
	private float speed = 0.5f;					//speed of the domino
	private Rigidbody rb;						//Rigibody of the domino

	void Awake()
	{
		domiTrans = GetComponent<Transform>();					
	}

	public void StartMoveUp(bool activeDomino)
	{
		//if it is an active domino, move domino up max length. Otherwise min length
		if(!activeDomino)
		{
			length = minLength;
		}else{
			length = maxLength;
		}

		StartCoroutine(MoveUp(length));								//Start move up

	}

	//A coroutin method to move an domino up
	private IEnumerator MoveUp(float distance)
	{
		float localX = domiTrans.localPosition.x;
		float localY = domiTrans.localPosition.y;
		float localZ = domiTrans.localPosition.z;

		while(distanceTomove < distance)
		{
			distanceTomove += Time.deltaTime;
			if(distanceTomove > distance)
			{
				distanceTomove = distance;
			}

			domiTrans.localPosition = new Vector3(localX, localY + distanceTomove, localZ);

			yield return null;
		}

		StopCoroutine("MoveUp");

		//if distance is equal to max length, it is an active domino. 
		if(distance == maxLength)
		{
			gameObject.tag = "ActiveDomino";					//Change the tag of an domino to ActiveDomino
			StartCoroutine(RotateDomino());						//Call RatateDomino() to rotate the domino. Will read input every frame in this method
		}else{
			SetupDomino();										//This function will Add physic components to a domino
		}
	}


	//if called, this method will be executed every frame.
	private IEnumerator RotateDomino()
	{
		while(true)
		{
			domiTrans.Rotate(Vector3.right, Space.Self);		//Rotate the domino around x axis

			if(Input.GetButtonDown("Fire1"))					//if Fire1 is pressed
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
