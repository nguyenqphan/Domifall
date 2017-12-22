using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour {

	public enum ColorType{SOLID, PATTERN, LERP};
	public enum NumOfColor{FIRSTCOLOR = 1, SECONDCOLOR,THIRDCOLOR, FOURTHCOLOR};
	public Color32 firstColor;
	public Color32 secondColor;




	private const int NUMBEROFCOLOR = 26;
//	[HideInInspector]
	public Color32[] colorArray = new Color32[NUMBEROFCOLOR];
	public ColorType colorType;

	void Awake(){
		DefineColor();
		colorType = ColorTypePicker();

		RanColorGroup();
//		firstColor = colorArray[23];

		firstColor = colorArray[Random.Range(0, colorArray.Length)];
		secondColor = colorArray[Random.Range(0, colorArray.Length)];

		Debug.Log(colorType);
		Debug.Log("First Color " + firstColor);
		Debug.Log("Second COlor " + secondColor);


	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public int colorGroup;
	private int RanColorGroup()
	{
		colorGroup = Random.Range(1, 20);
		return colorGroup;
	}

	private ColorType ColorTypePicker()
	{
		int randomNum;
		randomNum = Random.Range(0, System.Enum.GetValues(typeof(ColorType)).Length);
		switch(randomNum)
		{
			case 0: return ColorType.SOLID;
			case 1: return ColorType.PATTERN;
			case 2: return ColorType.LERP;
			default: return ColorType.SOLID;
		}
	}

	private int RandomAColor()
	{
		int ranSolidColorNum;
		ranSolidColorNum =  Random.Range(0, colorArray.Length);
		return ranSolidColorNum;
	}



	public void  DefineColor()
	{
		//NOTE: alpha does not work for current domino shader.
		colorArray[0] = new Color(255f, 153f , 0f  , 255f) / 255f; //yellow Orange 
		colorArray[1] = Color.white;
		colorArray[2] = Color.red;
		colorArray[3] = Color.green;							   //Bright Green
		colorArray[4] = new Color(0f,   150f,    255f)/255f;	   //Ocean blue 
		colorArray[5] = Color.yellow;
		colorArray[6] = Color.cyan;
		colorArray[7] = Color.magenta;
		colorArray[8] = new Color (192f, 192f , 192f, 255f) / 255f; //silver
		colorArray[9] = new Color (128f, 128f , 128f, 255f) / 255f; //Gray
		colorArray[10] = new Color(192f, 0f   , 0f  , 255f) / 255f; //Dark Orange Red
		colorArray[11] = new Color(255f, 75f , 0f  , 255f) / 255f; //Orange 
		colorArray[12] = new Color(0f  , 128f , 0f  , 255f) / 255f; //Dark Green
		colorArray[13] = new Color(192f, 0f   , 192f, 255f) / 255f; //Purple
		colorArray[14] = new Color(0f  , 128f , 128f, 255f) / 255f; //Teal
		colorArray[15] = new Color(255f, 128f , 128 , 255f) / 255f; //pinkish orange
		colorArray[16] = new Color(255f, 192f , 192f, 255f) / 255f; //very light pick
		colorArray[17] = new Color(255f, 0f   , 192f, 255f) / 255f; //bright pink
		colorArray[18] = new Color(100f, 180f , 255f, 255f) / 255f; //medium blue
		colorArray[19] = new Color(192f, 128f   , 255f, 255f) / 255f; //light Purple
		colorArray[20] = new Color(50f, 192f   , 192f, 255f) / 255f; // light blue
		colorArray[21] = new Color(255f, 192f , 128f  , 255f) / 255f; // light yellow 
		colorArray[22] = new Color(255f, 92f   , 192f, 255f) / 255f; //pink
		colorArray[23] = new Color(192f, 255f   , 192f, 255f) / 255f; //light green
		colorArray[24] = new Color(192f, 255f   , 70f, 255f) / 255f; //bright yellow green
		colorArray[25] = new Color(64f, 192f   , 32f, 255f) / 255f; //bright yellow green








	}

//	colorArray[13] = new Color(255f, 0f   , 192f, 255f) / 255f; //pink
}


