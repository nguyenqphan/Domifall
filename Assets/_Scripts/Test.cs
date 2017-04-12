using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public Transform target;
	public Transform origin;
	private Rigidbody rb;
	Vector3 dir;

	void Start() {
		rb = origin.GetComponent<Rigidbody>();
	    dir = (target.position - origin.position).normalized * 5f;
		rb.velocity = dir;

	}
//	void FixedUpdate() {
//		rb.velocity = dir;
//	}

	void OnCollisionEnter(Collision collision)
	{
		rb.useGravity = true;
		rb.velocity = Vector3.zero;
		if(collision.gameObject.CompareTag("Domino"))
		{
			collision.gameObject.GetComponent<Rigidbody>().AddForce(-dir, ForceMode.Force);
		}

	}
}
