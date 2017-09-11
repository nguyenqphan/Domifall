using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	public GameObject mainCanvas;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadLevel(int level)
	{
		
	}

	public void Level1()
	{
		HideUI();
		SceneManager.LoadScene("Level_04");
	}

	public void Level2()
	{
		HideUI();
		SceneManager.LoadScene("Level_06");
	}

	public void HideUI()
	{
		mainCanvas.SetActive(false);		
	}

	public void ShowUI()
	{
		mainCanvas.SetActive(true);
	}

}
