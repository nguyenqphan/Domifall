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
		if(!activeDomino)
		{
			length = minLength;
		}else{
			length = maxLength;
		}

		StartCoroutine(MoveUp(length));

	}

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

		if(distance == maxLength)
		{
			gameObject.tag = "ActiveDomino";
			StartCoroutine(RotateDomino());
		}
	}

	private IEnumerator RotateDomino()
	{
		while(true)
		{
			domiTrans.Rotate(Vector3.right, Space.Self);

			if(Input.GetButtonDown("Fire1"))
			{
				SetupDomino();
			}

			yield return null;
		}
	}

	private void SetupDomino()
	{
		StopAllCoroutines();
		gameObject.AddComponent<Rigidbody>();
		rb = GetComponent<Rigidbody>();
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}
}
