using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorScriptLocal : MonoBehaviour {

	Renderer _renderer;
	GameObject tofu;
	ScoreScriptLocal script;
	float timer = 0;
	float interval = 1.5f;
	int rensya = 1;
	float term = 0.05f;
	public Transform player;
	AudioSource audiosource;
	public AudioClip[] clips;
	int max = 12;
	GameObject[] tofus;//豆腐を予め作っておく
	TofuScriptLocal[] tofuScript;
	bool[] inGround;
	Vector3 okiba = new Vector3(0,-1000,0);
	int index = 0;
	public float chargetime = 0;
	GameObject ChargeObj;
	Image ChargeImage;



	// Use this for initialization
	void Awake(){

		_renderer = this.GetComponent<Renderer> ();
		tofu = Resources.Load ("TofuLocal") as GameObject;
		_renderer.material.color = new Color (Random.value, Random.value, Random.value, 1.0f);



		audiosource = GetComponent<AudioSource> ();
		tofus = new GameObject[max];
		tofuScript = new TofuScriptLocal[max];
		inGround = new bool[max];

	}
	void Start () {

		script = GameObject.Find ("ScoreManager").GetComponent<ScoreScriptLocal> ();
		script.mycolor = _renderer.material.color;
		script.isColor = true;
		player = transform.parent;

		for (int i = 0; i < max; i++) {
			tofus [i] = Instantiate (tofu, okiba, Quaternion.identity) as GameObject;
			tofus [i].GetComponent<Renderer> ().material.color = script.mycolor;
			inGround [i] = false;
			tofuScript [i] = tofus [i].GetComponent<TofuScriptLocal> ();
		}
		//ChargeObj = GameObject.FindGameObjectWithTag ("Charge");


	}

	// Update is called once per frame
	void Update () {


		if (ChargeObj == null) {
			ChargeObj = GameObject.FindGameObjectWithTag ("Charge");
			ChargeImage = ChargeObj.GetComponent<Image> ();
			ChargeImage.color = script.mycolor;
		} else {

			ChargeImage.fillAmount = chargetime / 3.0f;

		}
		transform.position = player.position + Vector3.up * (1.65f + transform.localScale.y * 0.5f);
		if (timer > 0) {
			timer -= Time.deltaTime;
		}

		if ((Input.GetKeyUp ("x")||Input.GetMouseButtonUp(0)||Input.GetKeyUp("k"))&& term >= 0 && timer <= 0) {
			Shot ();
			term -= Time.deltaTime;
			chargetime = 0;
			if (term < 0) {
				timer = interval;
				term = 0.2f;
			}
		}
		if ((Input.GetKey ("x") || Input.GetMouseButton (0) || Input.GetKey ("k"))) {
			if (chargetime <= 3) {
				chargetime += Time.deltaTime;
			}
		} else {
			chargetime = 0;
		}


		this.transform.localScale = Vector3.one * 0.1f + Vector3.one * script.myScore * 0.025f;
		Level ();

	}




	public void Shot(){
		int random = Random.Range (0,2);
		audiosource.clip = clips[random]; //声
		audiosource.Play ();
		for (int i = 0; i < rensya; i++) {
			if (tofus [index].transform.position.y < -500) {
				Vector3 pos = new Vector3 (transform.position.x, 1.65f, transform.position.z);
				tofus [index].transform.position = pos;
				tofuScript [index].GetRb ().velocity = transform.forward * (20* chargetime + 20);
				index++;
				if (index >= max) {
					index = 0;
				}
			}
			//GameObject obj = PhotonNetwork.Instantiate ("Tofu",pos + transform.forward.normalized, transform.rotation, 0) as GameObject;
			//obj.GetComponent<Renderer> ().material.color = script.mycolor;
			//obj.GetComponent<Rigidbody> ().velocity = transform.forward * 10;
		}
	}
	void Level(){
		int score = script.myScore;
		if (score < 5) {
		} else if (score < 10) {
			//rensya = 0.3f;
		} else if (score < 15) {
			rensya = 2;
			interval = 1.2f;
		} else if (score < 20) {
			interval = 2;
		} else if (score < 25) {
			//rensya = 0.5f;
		} else if (score < 30) {
			interval = 1.0f;
		} else if (score < 35) {
			interval = 0.8f;
		} else if (score < 40) {
			//tydrensya = 0.4f;
		} else if (score < 45) {
			interval = 0.5f;
		} else if (score < 50) {
			//interval = 0.25f;
		} else if (score < 55) {
			//interval = 0.1f;
		} else if (score < 60) {
			//interval = 0.01f;
		} else if (score < 65) {
		} else {
		}
	}
}
