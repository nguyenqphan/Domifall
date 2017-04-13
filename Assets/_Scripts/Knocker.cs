using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knocker : MonoBehaviour {

	private Transform trans;								//Store the transform component 										
	private float distanceTomove;							//Store the current distance
	private float distance = 1.5f;							//Store a distance
	private float speed = 1f;								//Speed of moving
	private Rigidbody rb;									//Store rigibody
	private Control control;								//reference to the Control component 	

	void Awake()
	{
		trans = GetComponent<Transform>();	
		control = GameObject.FindWithTag("Control").GetComponent<Control>();
	}

	// Use this for initialization
	void Start () {
		trans.gameObject.SetActive(false);					//Set the knock domino inactive

	}

	public void StartMoveUp()
	{
		StartCoroutine(MoveUp());							
	}

	//A coroutine to move a domino up
	private IEnumerator MoveUp()
	{
		float localX = trans.localPosition.x;
		float localY = trans.localPosition.y;
		float localZ = trans.localPosition.z;

		while(distanceTomove < distance)
		{
			distanceTomove += Time.deltaTime * speed;
			if(distanceTomove > distance)
			{
				distanceTomove = distance;
			}

			trans.localPosition = new Vector3(localX, (localY + distanceTomove), localZ);

			yield return null;
		}

		StopCoroutine("MoveUp");								//Stop Coroutine after it is done
		KnockDown();											//Call KnockDown()
	}

	public void StartRotateDomino()
	{
		StartCoroutine(RotateDomino());
	}

	private IEnumerator RotateDomino()
	{
		while(true)
		{
			trans.Rotate(Vector3.up, Space.Self);		//Rotate the domino around x axis

			yield return null;
		}
	}
		
	private void KnockDown()
	{
		gameObject.AddComponent<BoxCollider>();					//Add a BoxCollider to the game object
		gameObject.AddComponent<Rigidbody>();					//Add a Rigibody to the game object	
		rb = GetComponent<Rigidbody>();							
		rb.useGravity = false;									//dont use gravity
		rb.velocity = -control.dir * 10f;						//Move the game object in a given direction	
	}

	void OnCollisionEnter(Collision collision)
	{
		StopAllCoroutines();

		if(collision.gameObject.CompareTag("Domino"))
		{
			collision.gameObject.GetComponent<Rigidbody>().AddForce(-control.dir, ForceMode.Acceleration);
		}

		rb.useGravity = true;
//		rb.velocity = Vector3.zero;

	}


}
