using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour {


	public ColorPicker colorPicker;

	private Color32[] colors = new Color32[24]; //24 vertices for primive cube
	private Vector3[] vertices = new Vector3[24];
	float lerpValue = 0f;
	private int i = 0;
	private int colorIndex;
	private Color32 tempColor;
	private bool isSwitched = true;

	void Awake()
	{
		colorPicker = GameObject.FindWithTag("ColorPicker").GetComponent<ColorPicker>();
	}

	void Start()
	{
		lerpValue = 0f;
	}

	public void ColorMesh(Mesh mesh, int numOfDomino)
	{
		
		if(colorPicker.colorType == ColorPicker.ColorType.LERP)
		{
			ColorLerp();
		}
		else
		{
			if(colorPicker.colorType == ColorPicker.ColorType.SOLID)
			{
				ColorSolid();
			}
			else
			{
				ColorPattern();
			}
				
		}
			
		for(i = 0; i < vertices.Length; i++)
		{
			colors[i] = tempColor;
		}

		mesh.colors32 = colors;
	}

	private void ColorLerp()
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

		tempColor = Color32.Lerp(colorPicker.firstColor, colorPicker.secondColor, lerpValue);
	}



	private void ColorSolid()
	{
		tempColor = colorPicker.firstColor;
	}

	int countColor;
	bool colorSwitch;
	private void ColorPattern()
	{
			
			countColor++;
		Debug.Log("Count Color " + countColor);
		Debug.Log("ColorSwitch "+ colorSwitch);
		Debug.Log("ColorGround "+ colorPicker.colorGroup);
			if(countColor == colorPicker.colorGroup){
				colorSwitch = !colorSwitch;
				countColor = 0;
			}

		if(colorSwitch)
			tempColor = colorPicker.firstColor;
		else
			tempColor = colorPicker.secondColor;
	}

	public void MakeDominoDisapear(Mesh mesh)
	{
		StartCoroutine(DominoDisapear(mesh));
	}

	IEnumerator DominoDisapear(Mesh mesh)
	{
		byte alphaColor = 255;
		byte TransparentSpeed = 1;
		while(alphaColor > 1)
		{
			
			alphaColor = (byte)(alphaColor - TransparentSpeed) ;
			for(i = 0; i < vertices.Length; i++)
			{
				colors[i].a = 0;
				Debug.Log(colors[i].a);
			}
		
			mesh.colors32 = colors;
			yield return null;
		}

	}




}
