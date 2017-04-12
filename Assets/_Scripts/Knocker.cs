using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knocker : MonoBehaviour {

	private Transform trans;
	private float distanceTomove;
	private float distance = 1.5f;
	private float speed = 1f;
	private Rigidbody rb;
	private Control control;

	void Awake()
	{
		trans = GetComponent<Transform>();	
		control = GameObject.FindWithTag("Control").GetComponent<Control>();
	}

	// Use this for initialization
	void Start () {
		trans.gameObject.SetActive(false);

	}

	public void StartMoveUp()
	{
		StartCoroutine(MoveUp());
	}

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

		StopCoroutine("MoveUp");
		KnockDown();
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
		gameObject.AddComponent<BoxCollider>();
		gameObject.AddComponent<Rigidbody>();
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		rb.velocity = -control.dir * 10f;
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
