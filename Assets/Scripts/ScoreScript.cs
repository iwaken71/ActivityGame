using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreScript : MonoBehaviour {
	//public static ScoreScript instance = null;
	public Color mycolor;
	public int myScore = 0;
	public Text scoreLabel;
	float mynumber;
	GameObject[] cubes;
	StageScript[] stage;
	//ArrayList lst;
	int cubeCount;
	public bool isColor = false;
	/*
	void Awake(){
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);

		} else {
			Destroy (this.gameObject);
		}
	}*/

	// Use this for initialization
	void Start () {
		
		cubes = GameObject.FindGameObjectsWithTag ("Stage");
		cubeCount = cubes.Length;
		stage = new StageScript[cubeCount];

		for (int i = 0; i < cubeCount; i++) {
			stage [i] = cubes [i].GetComponent<StageScript> ();
		}
		//lst = new ArrayList ();
		mynumber = -1;
		myScore = 0;

	}
	
	// Update is called once per frame
	void Update () {
		
		if (mynumber <= 0 && isColor) {
			mynumber = mycolor.r + mycolor.b*10 + mycolor.g*100;
			scoreLabel.color = mycolor;
		}
		CheckScore ();

	}
	//ステージの床を全て白にする
	public void Clear(){
		foreach (GameObject cube in cubes) {
			cube.GetComponent<Renderer> ().material.color = Color.white;
		}
		//GameManager.GetInstance ().GameStart ();
	}

	public void CheckScore(){
		int count = 0;
		for (int i = 0; i < cubeCount; i++) {
			if (Abs(mynumber - stage [i].colornumber) < 0.01f) {
				count++;
			}
		}
		scoreLabel.color = mycolor;
		if (myScore != count) {
			myScore = count;
			scoreLabel.text = myScore.ToString ();
		}
		
	}

	float Abs(float a){
		if (a >= 0)
			return a;
		else
			return -a;
	}

}
