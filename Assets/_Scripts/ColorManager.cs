using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour {

	private const int NUMBEROFCOLOR = 17;

	private Color32[] colorArray = new Color32[NUMBEROFCOLOR];
	private Color32[] colors = new Color32[24]; //24 vertices for primive cube
	private Vector3[] vertices = new Vector3[24];
	float lerpValue = 0f;
	private int i = 0;
	private int colorIndex;
	private Color32 tempColor;
	private bool isSwitched = true;

	void Awake()
	{
		DefineColor();
	}


	public void  DefineColor()
	{
		colorArray[0] = new Color(255f, 153f , 0f  , 255f) / 255f; //Orange 
		colorArray[1] = Color.white;
		colorArray[2] = Color.red;
		colorArray[3] = Color.green;							   //Bright Green
		colorArray[4] = Color.blue;
		colorArray[5] = Color.yellow;
		colorArray[6] = Color.cyan;
		colorArray[7] = Color.magenta;
		colorArray[8] = new Color (192f, 192f , 192f, 255f) / 255f; //silver
		colorArray[9] = new Color (128f, 128f , 128f, 255f) / 255f; //Gray
		colorArray[10] = new Color (128, 0f   , 0f  , 255f) / 255f; //Maroon
		colorArray[11] = new Color(128f, 128f , 0f  , 255f) / 255f; //Olive
		colorArray[12] = new Color(0f  , 128f , 0f  , 255f) / 255f; //Dark Green
		colorArray[13] = new Color(128f, 0f   , 128f, 255f) / 255f; //Purple
		colorArray[14] = new Color(0f  , 128f , 128f, 255f) / 255f; //Teal
		colorArray[15] = new Color(0f  , 0f   , 128f, 255f) / 255f; //Navy
	

	}

	public void ColorMesh(Mesh mesh, int numOfDomino)
	{
		if(lerpValue > 1f || lerpValue < 0f)
		{
			isSwitched = !isSwitched;
		}

		if(isSwitched)
		{
			lerpValue += 0.025f;
		}else{
			lerpValue -= 0.025f;
		}

		tempColor = Color32.Lerp(colorArray[12], colorArray[2], lerpValue);

		for(i = 0; i < vertices.Length; i++)
		{
				colors[i] = tempColor;
		}

		mesh.colors32 = colors;
	}




}
