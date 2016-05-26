using UnityEngine;
using System.Collections;

public class StageScript : MonoBehaviour {
	public float colornumber;
	Renderer rd;

	// Use this for initialization
	void Start () {
		colornumber = 0;
		rd = GetComponent<Renderer> ();
	
	}
	
	// Update is called once per frame
	void Update () {

		float r = rd.material.color.r;
		float b = rd.material.color.b*10;
		float g = rd.material.color.g*100;
		colornumber = r + b + g;

		
	}

	public void SetColorNumber(float num){
		colornumber = num;
	}
}
