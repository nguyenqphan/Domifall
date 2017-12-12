using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AlphaControl : MonoBehaviour {



	private UnityAction eventListener;

	private Renderer sr;

	void Awake()
	{
		sr = gameObject.GetComponent<Renderer> ();
		eventListener = new UnityAction(StartFading);
	}

	void OnEnable()
	{
		EventManager.StartListening("Fade", eventListener);
	}

	void OnDisable()
	{
		EventManager.StopListening("Fade", eventListener);
	}



	public float m_targetAlpha;
	public bool m_isVanish;
	public float m_duration;
	void StartFading()
	{
		StartCoroutine (FadeOut3D (m_targetAlpha, m_isVanish, m_duration));
	}



	IEnumerator FadeOut3D (float targetAlpha, bool isVanish, float duration)
	{
		
		float diffAlpha = (targetAlpha - sr.material.color.a);

		float counter = 0;
		while (counter < duration) {
			float alphaAmount = sr.material.color.a + (Time.deltaTime * diffAlpha) / duration;
			sr.material.color = new Color (sr.material.color.r, sr.material.color.g, sr.material.color.b, alphaAmount);

			counter += Time.deltaTime;
			yield return null;
		}
		sr.material.color = new Color (sr.material.color.r, sr.material.color.g, sr.material.color.b, targetAlpha);
		if (isVanish) {
			gameObject.SetActive (false);
		}
	}


}
