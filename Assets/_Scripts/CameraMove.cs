using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

	private Transform trans;
	private float speed = 3f;
	private float distance;
	private Vector3 tempPos;

	private IEnumerator coroutine;

	void Awake()
	{
		trans = GetComponent<Transform>();
	}

	public void MoveToTarget(Transform targetTrans)
	{
		if(coroutine != null)
		{
			StopCoroutine(coroutine);
		}

		coroutine = MoveTo(targetTrans);
		StartCoroutine(coroutine);
	}


	private IEnumerator MoveTo(Transform targetTrans)
	{
		tempPos = targetTrans.position;
		while(trans.position != tempPos)
		{
			distance = speed * Time.deltaTime;
			trans.position = Vector3.MoveTowards(trans.position, tempPos, distance);

			yield return null;	
		}

	
	}
		
}
