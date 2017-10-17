using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

	private UIManager uiManager;

	public Transform camFinalPos;
	public Vector3 velocity = Vector3.one;
	public Vector3 vel = Vector3.one;

	private Transform trans;
	private float speed = 3f;
	private float distance;
	private Vector3 tempPos;
	private Quaternion tempRot;

	float angle = 0f;
	float rotateSpeed = 35f;
	float fixedAngleToRotate = 180f;
	float lengthOfSubTower = 5f;

	float lengthOfCurTower = 0f;
	float circleBackSpeed = 2.1f;

	private IEnumerator coroutine;

	void OnEnable()
	{
		CameraTrigger.StopCamera += StopCameraMove;
	}

	void OnDisable()
	{
		CameraTrigger.StopCamera -= StopCameraMove;
	}

	void Awake()
	{
		trans = GetComponent<Transform>();
		uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
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
		tempRot = targetTrans.rotation;
		while(trans.position != tempPos)
		{
			distance = speed * Time.deltaTime;
			trans.position = Vector3.SmoothDamp(trans.position, tempPos, ref vel, 1f);

			//condition to exit the loop - stop the camere
			if(vel.sqrMagnitude <  0.01f)
			{
				break;
			}
			yield return null;	
		}
	
	}

	private IEnumerator CircleAround()
	{
		 angle = 0f;
		 
		while(angle < fixedAngleToRotate)
		{
			
			trans.Rotate(rotateSpeed * Time.deltaTime * -Vector3.up , Space.World );

			angle = angle + Time.deltaTime * rotateSpeed;

			trans.Translate(Vector3.down * (rotateSpeed * Time.deltaTime) * lengthOfSubTower / fixedAngleToRotate    , Space.World);
			if(angle > fixedAngleToRotate)
			{
				angle = fixedAngleToRotate;
			}
			yield return null;
		}
	}

	private IEnumerator CircleBack(float dis)
	{
		
		while(lengthOfCurTower < dis){
		
			trans.Rotate( Time.deltaTime * Vector3.up, Space.World);
			trans.Translate(circleBackSpeed * Time.deltaTime * Vector3.up, Space.World);
			lengthOfCurTower = lengthOfCurTower + circleBackSpeed * Time.deltaTime;

			yield return null;
		}


	}

	private IEnumerator CamLastPosition()
	{
		tempPos = camFinalPos.position;

		//add the offset 0.1 to the camera y position to stop the camera
		while(trans.position.y  + 0.1 < tempPos.y)
		{
			trans.position = Vector3.SmoothDamp(trans.position, tempPos, ref velocity, 1f);

			yield return null;
		}

		Debug.Log("CamLastPosition called ");
		uiManager.ShowUI();
	}




	public void MoveCamToLastPosition()
	{
		StartCoroutine(CamLastPosition());
	}

	public void CircleBackToTop(float dis)
	{
		StartCoroutine(CircleBack(dis));
	}


	public void CircleAroundTarget()
	{
		StartCoroutine(CircleAround());
	}

	public  void StopCameraMove()
	{
//		StopCoroutine(coroutine);
		StopAllCoroutines();
	}

}
