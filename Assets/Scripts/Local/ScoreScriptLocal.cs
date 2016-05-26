using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreScriptLocal: MonoBehaviour {


	public Color mycolor;
	public int myScore = 0;
	public Text scoreLabel;
	float mynumber;
	GameObject[] cubes;
	StageScript[] stage;
	ArrayList lst;
	int cubeCount;
	public bool isColor = false;
	float firstTimer = 4;
	float timer = 0;
	float LimitTime = 120.0f;

	public Text CountDown, TimerLabal;

	public static bool GAME = false;






	// Use this for initialization
	void Start () {
		cubes = GameObject.FindGameObjectsWithTag ("Stage");
		cubeCount = cubes.Length;
		stage = new StageScript[cubeCount];

		for (int i = 0; i < cubeCount; i++) {
			stage [i] = cubes [i].GetComponent<StageScript> ();
		}
		lst = new ArrayList ();
		mynumber = -1;

	}

	// Update is called once per frame
	void Update () {
		if (firstTimer >= 0) {
			firstTimer -= Time.deltaTime;
			CountDown.text = ((int)firstTimer).ToString ();
			GAME = false;
		} else {
			CountDown.text = "";
			timer += Time.deltaTime;
			TimerLabal.text = (LimitTime - timer).ToString ("f1");
			GAME = true;
		}





		if (mynumber <= 0 && isColor) {
			mynumber = mycolor.r + mycolor.b*10 + mycolor.g*100;
			scoreLabel.color = mycolor;
		}
		int count = 0;
		for (int i = 0; i < cubeCount; i++) {
			if (Abs(mynumber - stage [i].colornumber) < 0.01f) {
				count++;
			}
		}
		scoreLabel.color = mycolor;
		myScore = count;
		scoreLabel.text = myScore.ToString ();



	}
	//ステージの床を全て白にする
	public void clear(){
		foreach (GameObject cube in cubes) {
			cube.GetComponent<Renderer> ().material.color = Color.white;
		}
	}

	float Abs(float a){
		if (a >= 0)
			return a;
		else
			return -a;
	}

}
